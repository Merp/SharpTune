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
using Gnu.IO;
using Org.Apache.Log4j;
using RomRaider.IO.Serial.Port;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.IO.Serial.Port
{
	public sealed class SerialPortRefresher : Runnable
	{
		private static readonly Logger LOGGER = Logger.GetLogger(typeof(RomRaider.IO.Serial.Port.SerialPortRefresher
			));

		private const long PORT_REFRESH_INTERVAL = 15000L;

		private readonly SerialPortDiscoverer serialPortDiscoverer = new SerialPortDiscovererImpl
			();

		private readonly SerialPortRefreshListener listener;

		private readonly string defaultLoggerPort;

		private bool started;

		private bool refreshMode;

		public SerialPortRefresher(SerialPortRefreshListener listener, string defaultLoggerPort
			)
		{
			ParamChecker.CheckNotNull(listener);
			this.listener = listener;
			this.defaultLoggerPort = defaultLoggerPort;
		}

		public void Run()
		{
			RefreshPortList();
			started = true;
			while (true)
			{
				ThreadUtil.Sleep(PORT_REFRESH_INTERVAL);
				if (refreshMode)
				{
					RefreshPortList();
				}
			}
		}

		public bool IsStarted()
		{
			return started;
		}

		public void SetRefreshMode(bool b)
		{
			refreshMode = b;
			if (refreshMode)
			{
				RefreshPortList();
			}
		}

		private void RefreshPortList()
		{
			try
			{
				listener.RefreshPortList(ListSerialPorts(), defaultLoggerPort);
			}
			catch (Exception e)
			{
				LOGGER.Error("Error refreshing serial ports", e);
			}
		}

		private ICollection<string> ListSerialPorts()
		{
			IList<CommPortIdentifier> portIdentifiers = serialPortDiscoverer.ListPorts();
			ICollection<string> portNames = new TreeSet<string>();
			foreach (CommPortIdentifier portIdentifier in portIdentifiers)
			{
				string portName = portIdentifier.GetName();
				if (!portNames.Contains(portName))
				{
					portNames.AddItem(portName);
				}
			}
			return portNames;
		}
	}
}
