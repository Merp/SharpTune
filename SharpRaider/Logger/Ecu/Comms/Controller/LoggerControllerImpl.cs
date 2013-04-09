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
using RomRaider;
using RomRaider.Logger.Ecu.Comms.Controller;
using RomRaider.Logger.Ecu.Comms.Manager;
using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI;
using RomRaider.Logger.Ecu.UI.Handler;
using RomRaider.Logger.Ecu.UI.Handler.File;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Comms.Controller
{
	public sealed class LoggerControllerImpl : LoggerController
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.
			GetLogger(typeof(RomRaider.Logger.Ecu.Comms.Controller.LoggerControllerImpl));

		private readonly QueryManager queryManager;

		public LoggerControllerImpl(Settings settings, EcuInitCallback ecuInitCallback, MessageListener
			 messageListener, params DataUpdateHandler[] dataUpdateHandlers)
		{
			ParamChecker.CheckNotNull(settings, ecuInitCallback, messageListener, dataUpdateHandlers
				);
			queryManager = new QueryManagerImpl(settings, ecuInitCallback, messageListener, dataUpdateHandlers
				);
		}

		public void AddListener(StatusChangeListener listener)
		{
			lock (this)
			{
				ParamChecker.CheckNotNull(listener, "listener");
				queryManager.AddListener(listener);
			}
		}

		public void SetFileLoggerSwitchMonitor(FileLoggerControllerSwitchMonitor monitor)
		{
			ParamChecker.CheckNotNull(monitor);
			LOGGER.Debug("Setting file logger switch monitor: [" + monitor.GetEcuSwitch().GetId
				() + "] " + monitor.GetEcuSwitch().GetName());
			queryManager.SetFileLoggerSwitchMonitor(monitor);
		}

		public void AddLogger(string callerId, LoggerData loggerData)
		{
			ParamChecker.CheckNotNull(loggerData);
			LOGGER.Debug("Adding logger:   [" + loggerData.GetId() + "] " + loggerData.GetName
				());
			queryManager.AddQuery(callerId, loggerData);
		}

		public void RemoveLogger(string callerId, LoggerData loggerData)
		{
			ParamChecker.CheckNotNull(loggerData, "ecuParam");
			LOGGER.Debug("Removing logger: [" + loggerData.GetId() + "] " + loggerData.GetName
				());
			queryManager.RemoveQuery(callerId, loggerData);
		}

		public bool IsStarted()
		{
			lock (this)
			{
				return queryManager.IsRunning();
			}
		}

		public void Start()
		{
			lock (this)
			{
				if (!IsStarted())
				{
					ThreadUtil.RunAsDaemon(queryManager);
				}
			}
		}

		public void Stop()
		{
			lock (this)
			{
				if (IsStarted() && queryManager.GetThread().IsAlive())
				{
					queryManager.Stop();
					try
					{
						LOGGER.Debug(string.Format("%s - Stopping QueryManager: %s", this.GetType().Name, 
							queryManager.GetThread().GetName()));
						queryManager.GetThread().Interrupt();
						LOGGER.Debug(string.Format("%s - Waiting for QueryManager %s to terminate", this.
							GetType().Name, queryManager.GetThread().GetName()));
						queryManager.GetThread().Join();
					}
					catch (Exception e)
					{
						Sharpen.Runtime.PrintStackTrace(e);
					}
					finally
					{
						LOGGER.Debug(string.Format("%s - QueryManager %s state: %s", this.GetType().Name, 
							queryManager.GetThread().GetName(), queryManager.GetThread().GetState()));
					}
				}
			}
		}
	}
}
