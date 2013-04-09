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
using RomRaider.Logger.Ecu;
using RomRaider.Logger.Ecu.UI.Swing.Menubar.Action;
using RomRaider.Logger.External.Core;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Core
{
	public sealed class GenericDataSourceManager : ExternalDataSource
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(RomRaider.Logger.External.Core.GenericDataSourceManager));

		private readonly IList<Stoppable> connectors = new AList<Stoppable>();

		private readonly ExternalDataSource dataSource;

		private int connectCount;

		public GenericDataSourceManager(ExternalDataSource dataSource)
		{
			ParamChecker.CheckNotNull(dataSource, "dataSource");
			this.dataSource = dataSource;
		}

		public string GetId()
		{
			return dataSource.GetId();
		}

		public string GetName()
		{
			return dataSource.GetName();
		}

		public string GetVersion()
		{
			return dataSource.GetVersion();
		}

		public IList<ExternalDataItem> GetDataItems()
		{
			return dataSource.GetDataItems();
		}

		public Action GetMenuAction(EcuLogger logger)
		{
			Action action = dataSource.GetMenuAction(logger);
			return action == null ? new GenericPluginMenuAction(logger, this) : action;
		}

		public void SetPort(string port)
		{
			lock (this)
			{
				if (port == null || port.Length == 0)
				{
					return;
				}
				if (port.Equals(GetPort()))
				{
					return;
				}
				LOGGER.Info(dataSource.GetName() + ": port " + port + " selected");
				DoDisconnect();
				dataSource.SetPort(port);
			}
		}

		public string GetPort()
		{
			return dataSource.GetPort();
		}

		public void Connect()
		{
			lock (this)
			{
				if (connectCount++ == 0)
				{
					DoConnect();
				}
				LOGGER.Trace("Connect count [" + dataSource.GetName() + "]: " + connectCount);
			}
		}

		public void Disconnect()
		{
			lock (this)
			{
				if (connectCount-- == 1)
				{
					DoDisconnect();
				}
				if (connectCount < 0)
				{
					connectCount = 0;
				}
				LOGGER.Trace("Connect count [" + dataSource.GetName() + "]: " + connectCount);
			}
		}

		private void DoConnect()
		{
			Stoppable connector = new GenericDataSourceConnector(dataSource);
			connectors.AddItem(connector);
			ThreadUtil.RunAsDaemon(connector);
		}

		private void DoDisconnect()
		{
			if (dataSource.GetPort() == null)
			{
				return;
			}
			try
			{
				string message = string.Format("%s: disconnecting port %s", dataSource.GetName(), 
					dataSource.GetPort());
				LOGGER.Info(message);
				while (!connectors.IsEmpty())
				{
					connectors.Remove(0).Stop();
				}
				dataSource.Disconnect();
				message = string.Format("%s: disconnected", dataSource.GetName());
				LOGGER.Info(message);
			}
			catch (Exception e)
			{
				LOGGER.Error("External Datasource [" + dataSource.GetName() + "] disconnect error"
					, e);
			}
		}

		private void Reconnect(string port)
		{
			DoDisconnect();
			dataSource.SetPort(port);
			DoConnect();
		}
	}
}
