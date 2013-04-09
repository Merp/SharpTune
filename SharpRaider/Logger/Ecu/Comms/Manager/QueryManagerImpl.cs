/*
 * This code is derived from the Java version of RomRaider
 *
 * RomRaider Open-Source Tuning, Logging and Reflashing
 * Copyright (C) 2006-2012 RomRaider.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;
using System.Collections.Generic;
using Javax.Swing;
using RomRaider;
using RomRaider.Logger.Ecu.Comms.IO.Connection;
using RomRaider.Logger.Ecu.Comms.Manager;
using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI;
using RomRaider.Logger.Ecu.UI.Handler;
using RomRaider.Logger.Ecu.UI.Handler.File;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Comms.Manager
{
	public sealed class QueryManagerImpl : QueryManager
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.
			GetLogger(typeof(RomRaider.Logger.Ecu.Comms.Manager.QueryManagerImpl));

		private readonly IList<StatusChangeListener> listeners = Sharpen.Collections.SynchronizedList
			(new AList<StatusChangeListener>());

		private readonly IDictionary<string, RomRaider.Logger.Ecu.Comms.Query.Query> queryMap
			 = Sharpen.Collections.SynchronizedMap(new Dictionary<string, RomRaider.Logger.Ecu.Comms.Query.Query
			>());

		private readonly IDictionary<string, RomRaider.Logger.Ecu.Comms.Query.Query> addList
			 = new Dictionary<string, RomRaider.Logger.Ecu.Comms.Query.Query>();

		private readonly IList<string> removeList = new AList<string>();

		private static readonly PollingState pollState = new PollingStateImpl();

		private static readonly string ECU = "ECU";

		private static readonly string TCU = "TCU";

		private static readonly string EXT = "Externals";

		private readonly Settings settings;

		private readonly EcuInitCallback ecuInitCallback;

		private readonly MessageListener messageListener;

		private readonly DataUpdateHandler[] dataUpdateHandlers;

		private FileLoggerControllerSwitchMonitor monitor;

		private EcuQuery fileLoggerQuery;

		private Sharpen.Thread queryManagerThread;

		private static bool started;

		private static bool stop;

		public QueryManagerImpl(Settings settings, EcuInitCallback ecuInitCallback, MessageListener
			 messageListener, params DataUpdateHandler[] dataUpdateHandlers)
		{
			ParamChecker.CheckNotNull(settings, ecuInitCallback, messageListener, dataUpdateHandlers
				);
			this.settings = settings;
			this.ecuInitCallback = ecuInitCallback;
			this.messageListener = messageListener;
			this.dataUpdateHandlers = dataUpdateHandlers;
			stop = true;
		}

		public void AddListener(StatusChangeListener listener)
		{
			lock (this)
			{
				ParamChecker.CheckNotNull(listener, "listener");
				listeners.AddItem(listener);
			}
		}

		public void SetFileLoggerSwitchMonitor(FileLoggerControllerSwitchMonitor monitor)
		{
			ParamChecker.CheckNotNull(monitor);
			this.monitor = monitor;
			fileLoggerQuery = new EcuQueryImpl(monitor.GetEcuSwitch());
		}

		public void AddQuery(string callerId, LoggerData loggerData)
		{
			lock (this)
			{
				ParamChecker.CheckNotNull(callerId, loggerData);
				//FIXME: This is a hack!!
				string queryId = BuildQueryId(callerId, loggerData);
				if (loggerData.GetDataType() == EcuDataType.EXTERNAL)
				{
					addList.Put(queryId, new ExternalQueryImpl((ExternalData)loggerData));
				}
				else
				{
					addList.Put(queryId, new EcuQueryImpl((EcuData)loggerData));
					pollState.SetLastQuery(false);
					pollState.SetNewQuery(true);
				}
			}
		}

		public void RemoveQuery(string callerId, LoggerData loggerData)
		{
			lock (this)
			{
				ParamChecker.CheckNotNull(callerId, loggerData);
				removeList.AddItem(BuildQueryId(callerId, loggerData));
				if (loggerData.GetDataType() != EcuDataType.EXTERNAL)
				{
					pollState.SetNewQuery(true);
				}
			}
		}

		public Sharpen.Thread GetThread()
		{
			return queryManagerThread;
		}

		public bool IsRunning()
		{
			return started && !stop;
		}

		public void Run()
		{
			started = true;
			queryManagerThread = Sharpen.Thread.CurrentThread();
			LOGGER.Debug("QueryManager started.");
			try
			{
				stop = false;
				while (!stop)
				{
					NotifyConnecting();
					if (!settings.IsLogExternalsOnly() && DoEcuInit(Settings.GetDestinationId()))
					{
						NotifyReading();
						RunLogger(Settings.GetDestinationId());
					}
					else
					{
						if (settings.IsLogExternalsOnly())
						{
							NotifyReading();
							RunLogger(unchecked((byte)-1));
						}
						else
						{
							ThreadUtil.Sleep(1000L);
						}
					}
				}
			}
			catch (Exception e)
			{
				messageListener.ReportError(e);
			}
			finally
			{
				NotifyStopped();
				messageListener.ReportMessage("Disconnected.");
				LOGGER.Debug("QueryManager stopped.");
			}
		}

		private bool DoEcuInit(byte id)
		{
			string target = null;
			if (id == unchecked((int)(0x10)))
			{
				target = ECU;
			}
			if (id == unchecked((int)(0x18)))
			{
				target = TCU;
			}
			try
			{
				LoggerConnection connection = LoggerConnectionFactory.GetConnection(Settings.GetLoggerProtocol
					(), settings.GetLoggerPort(), settings.GetLoggerConnectionProperties());
				try
				{
					messageListener.ReportMessage("Sending " + target + " Init...");
					connection.EcuInit(ecuInitCallback, id);
					messageListener.ReportMessage("Sending " + target + " Init...done.");
					return true;
				}
				finally
				{
					connection.Close();
				}
			}
			catch (Exception e)
			{
				messageListener.ReportMessage("Unable to send " + target + " init - check cable is connected and ignition is on."
					);
				LogError(e);
				return false;
			}
		}

		private void LogError(Exception e)
		{
			if (LOGGER.IsDebugEnabled())
			{
				LOGGER.Debug("Error sending init", e);
			}
			else
			{
				LOGGER.Info("Error sending init: " + e.Message);
			}
		}

		private void RunLogger(byte id)
		{
			string target = null;
			if (id == -1)
			{
				target = EXT;
			}
			if (id == unchecked((int)(0x10)))
			{
				target = ECU;
			}
			if (id == unchecked((int)(0x18)))
			{
				target = TCU;
			}
			TransmissionManager txManager = new TransmissionManagerImpl(settings);
			long start = Runtime.CurrentTimeMillis();
			long end = Runtime.CurrentTimeMillis();
			int count = 0;
			try
			{
				txManager.Start();
				bool lastPollState = settings.IsFastPoll();
				while (!stop)
				{
					pollState.SetFastPoll(settings.IsFastPoll());
					UpdateQueryList();
					if (queryMap.IsEmpty())
					{
						if (pollState.IsLastQuery() && pollState.GetCurrentState() == 0)
						{
							EndEcuQueries(txManager);
							pollState.SetLastState(0);
						}
						start = Runtime.CurrentTimeMillis();
						count = 0;
						messageListener.ReportMessage("Select parameters to be logged...");
						ThreadUtil.Sleep(1000L);
					}
					else
					{
						end = Runtime.CurrentTimeMillis() + 1L;
						// update once every 1msec
						IList<EcuQuery> ecuQueries = FilterEcuQueries(queryMap.Values);
						if (!settings.IsLogExternalsOnly())
						{
							if (!ecuQueries.IsEmpty())
							{
								SendEcuQueries(txManager);
								if (!pollState.IsFastPoll() && lastPollState)
								{
									EndEcuQueries(txManager);
								}
								if (pollState.IsFastPoll())
								{
									if (pollState.GetCurrentState() == 0 && pollState.IsNewQuery())
									{
										pollState.SetCurrentState(1);
										pollState.SetNewQuery(false);
									}
									if (pollState.GetCurrentState() == 0 && !pollState.IsNewQuery())
									{
										pollState.SetCurrentState(1);
									}
									if (pollState.GetCurrentState() == 1 && pollState.IsNewQuery())
									{
										pollState.SetCurrentState(0);
										pollState.SetLastState(1);
										pollState.SetNewQuery(false);
									}
									if (pollState.GetCurrentState() == 1 && !pollState.IsNewQuery())
									{
										pollState.SetLastState(1);
									}
									pollState.SetLastQuery(true);
								}
								else
								{
									pollState.SetCurrentState(0);
									pollState.SetLastState(0);
									pollState.SetNewQuery(false);
								}
								lastPollState = pollState.IsFastPoll();
							}
							else
							{
								if (pollState.IsLastQuery() && pollState.GetLastState() == 1)
								{
									EndEcuQueries(txManager);
									pollState.SetLastState(0);
									pollState.SetCurrentState(0);
									pollState.SetNewQuery(true);
								}
							}
						}
						SendExternalQueries();
						// waiting until at least 1msec has passed since last query set
						while (Runtime.CurrentTimeMillis() < end)
						{
							ThreadUtil.Sleep(1L);
						}
						HandleQueryResponse();
						count++;
						messageListener.ReportMessage("Querying " + target + "...");
						messageListener.ReportStats(BuildStatsMessage(start, count));
					}
				}
			}
			catch (Exception e)
			{
				messageListener.ReportError(e);
			}
			finally
			{
				txManager.Stop();
				pollState.SetCurrentState(0);
				pollState.SetNewQuery(true);
			}
		}

		private void SendEcuQueries(TransmissionManager txManager)
		{
			IList<EcuQuery> ecuQueries = FilterEcuQueries(queryMap.Values);
			if (fileLoggerQuery != null && settings.IsFileLoggingControllerSwitchActive())
			{
				ecuQueries.AddItem(fileLoggerQuery);
			}
			txManager.SendQueries(ecuQueries, pollState);
		}

		private void SendExternalQueries()
		{
			IList<ExternalQuery> externalQueries = FilterExternalQueries(queryMap.Values);
			foreach (ExternalQuery externalQuery in externalQueries)
			{
				//FIXME: This is a hack!!
				externalQuery.SetResponse(externalQuery.GetLoggerData().GetSelectedConvertor().Convert
					(null));
			}
		}

		private void EndEcuQueries(TransmissionManager txManager)
		{
			txManager.EndQueries();
			pollState.SetLastQuery(false);
		}

		private void HandleQueryResponse()
		{
			monitor.MonitorFileLoggerSwitch(fileLoggerQuery.GetResponse());
			Response response = BuildResponse(queryMap.Values);
			foreach (DataUpdateHandler dataUpdateHandler in dataUpdateHandlers)
			{
				ThreadUtil.RunAsDaemon(new _Runnable_331(dataUpdateHandler, response));
			}
		}

		private sealed class _Runnable_331 : Runnable
		{
			public _Runnable_331(DataUpdateHandler dataUpdateHandler, Response response)
			{
				this.dataUpdateHandler = dataUpdateHandler;
				this.response = response;
			}

			public void Run()
			{
				dataUpdateHandler.HandleDataUpdate(response);
			}

			private readonly DataUpdateHandler dataUpdateHandler;

			private readonly Response response;
		}

		private Response BuildResponse(ICollection<RomRaider.Logger.Ecu.Comms.Query.Query
			> queries)
		{
			Response response = new ResponseImpl();
			foreach (RomRaider.Logger.Ecu.Comms.Query.Query query in queries)
			{
				response.SetDataValue(query.GetLoggerData(), query.GetResponse());
			}
			return response;
		}

		//FIXME: This is a hack!!
		private IList<EcuQuery> FilterEcuQueries(ICollection<RomRaider.Logger.Ecu.Comms.Query.Query
			> queries)
		{
			IList<EcuQuery> filtered = new AList<EcuQuery>();
			foreach (RomRaider.Logger.Ecu.Comms.Query.Query query in queries)
			{
				if (typeof(EcuQuery).IsAssignableFrom(query.GetType()))
				{
					filtered.AddItem((EcuQuery)query);
				}
			}
			return filtered;
		}

		//FIXME: This is a hack!!
		private IList<ExternalQuery> FilterExternalQueries(ICollection<RomRaider.Logger.Ecu.Comms.Query.Query
			> queries)
		{
			IList<ExternalQuery> filtered = new AList<ExternalQuery>();
			foreach (RomRaider.Logger.Ecu.Comms.Query.Query query in queries)
			{
				if (typeof(ExternalQuery).IsAssignableFrom(query.GetType()))
				{
					filtered.AddItem((ExternalQuery)query);
				}
			}
			return filtered;
		}

		public void Stop()
		{
			stop = true;
		}

		private string BuildQueryId(string callerId, LoggerData loggerData)
		{
			return callerId + "_" + loggerData.GetName();
		}

		private void UpdateQueryList()
		{
			lock (this)
			{
				AddQueries();
				RemoveQueries();
			}
		}

		private void AddQueries()
		{
			foreach (string queryId in addList.Keys)
			{
				queryMap.Put(queryId, addList.Get(queryId));
			}
			addList.Clear();
		}

		private void RemoveQueries()
		{
			foreach (string queryId in removeList)
			{
				Sharpen.Collections.Remove(queryMap, queryId);
			}
			removeList.Clear();
		}

		private string BuildStatsMessage(long start, int count)
		{
			string state = "Slow-K:";
			if (pollState.IsFastPoll())
			{
				state = "Fast-K:";
			}
			if (Settings.GetTransportProtocol().Equals("ISO15765"))
			{
				state = "CAN bus:";
			}
			if (settings.IsLogExternalsOnly())
			{
				state = "Externals:";
			}
			double duration = ((double)(Runtime.CurrentTimeMillis() - start)) / 1000.0;
			string result = string.Format("%s[ %.2f queries/sec, %.2f sec/query ]", state, ((
				double)count) / duration, duration / ((double)count));
			return result;
		}

		private void NotifyConnecting()
		{
			SwingUtilities.InvokeLater(new _Runnable_418(this));
		}

		private sealed class _Runnable_418 : Runnable
		{
			public _Runnable_418(QueryManagerImpl _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				foreach (StatusChangeListener listener in this._enclosing.listeners)
				{
					listener.Connecting();
				}
			}

			private readonly QueryManagerImpl _enclosing;
		}

		private void NotifyReading()
		{
			SwingUtilities.InvokeLater(new _Runnable_428(this));
		}

		private sealed class _Runnable_428 : Runnable
		{
			public _Runnable_428(QueryManagerImpl _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				foreach (StatusChangeListener listener in this._enclosing.listeners)
				{
					listener.ReadingData();
				}
			}

			private readonly QueryManagerImpl _enclosing;
		}

		private void NotifyStopped()
		{
			SwingUtilities.InvokeLater(new _Runnable_438(this));
		}

		private sealed class _Runnable_438 : Runnable
		{
			public _Runnable_438(QueryManagerImpl _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				foreach (StatusChangeListener listener in this._enclosing.listeners)
				{
					listener.Stopped();
				}
			}

			private readonly QueryManagerImpl _enclosing;
		}
	}
}
