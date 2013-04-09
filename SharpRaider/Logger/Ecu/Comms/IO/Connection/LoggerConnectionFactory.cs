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
using RomRaider.IO.Connection;
using RomRaider.Logger.Ecu.Comms.IO.Connection;
using RomRaider.Logger.Ecu.Exception;
using Sharpen;

namespace RomRaider.Logger.Ecu.Comms.IO.Connection
{
	public sealed class LoggerConnectionFactory
	{
		public LoggerConnectionFactory()
		{
		}

		public static LoggerConnection GetConnection(string protocolName, string portName
			, ConnectionProperties connectionProperties)
		{
			ConnectionManager manager = ConnectionManagerFactory.GetManager(portName, connectionProperties
				);
			return InstantiateConnection(protocolName, manager);
		}

		private static LoggerConnection InstantiateConnection(string protocolName, ConnectionManager
			 manager)
		{
			try
			{
				Type cls = Sharpen.Runtime.GetType(typeof(RomRaider.Logger.Ecu.Comms.IO.Connection.LoggerConnectionFactory
					).Assembly.GetName() + "." + protocolName + "LoggerConnection");
				return (LoggerConnection)cls.GetConstructor(typeof(ConnectionManager)).NewInstance
					(manager);
			}
			catch (Exception e)
			{
				manager.Close();
				throw new UnsupportedProtocolException(e.InnerException.Message, e);
			}
		}
	}
}
