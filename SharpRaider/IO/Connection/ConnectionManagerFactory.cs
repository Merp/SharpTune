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
using Org.Apache.Log4j;
using RomRaider;
using RomRaider.IO.Connection;
using RomRaider.IO.J2534.Api;
using RomRaider.IO.Serial.Connection;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.IO.Connection
{
	public sealed class ConnectionManagerFactory
	{
		private static readonly Logger LOGGER = Logger.GetLogger(typeof(RomRaider.IO.Connection.ConnectionManagerFactory
			));

		private const bool ENABLE_TIMER = false;

		public ConnectionManagerFactory()
		{
		}

		public static ConnectionManager GetManager(string portName, ConnectionProperties 
			connectionProperties)
		{
			ConnectionManager manager = Manager(portName, connectionProperties);
			return manager;
		}

		private static ConnectionManager Manager(string portName, ConnectionProperties connectionProperties
			)
		{
			try
			{
				if (!Platform.IsPlatform(Platform.WINDOWS))
				{
					throw new RuntimeException("J2534 is not support on this platform");
				}
				ICollection<J2534Library> libraries = J2534DllLocator.ListLibraries(Settings.GetTransportProtocol
					().ToUpper());
				if (libraries.IsEmpty())
				{
					throw new RuntimeException("No J2534 libraries found that support protocol " + Settings
						.GetTransportProtocol());
				}
				// if the J2534 device has not been previously defined, search for it
				// else use the defined device
				if (Settings.GetJ2534Device() == null)
				{
					foreach (J2534Library dll in libraries)
					{
						LOGGER.Info(string.Format("Trying new J2534/%s connection: %s", Settings.GetTransportProtocol
							(), dll.GetVendor()));
						try
						{
							Settings.SetJ2534Device(dll.GetLibrary());
							return J2534TransportFactory.GetManager(Settings.GetTransportProtocol().ToUpper()
								, connectionProperties, dll.GetLibrary());
						}
						catch (Exception t)
						{
							Settings.SetJ2534Device(null);
							LOGGER.Info(string.Format("%s is not available: %s", dll.GetVendor(), t.Message));
						}
					}
				}
				else
				{
					foreach (J2534Library dll in libraries)
					{
						if (dll.GetLibrary().ToLower().Contains(Settings.GetJ2534Device().ToLower()))
						{
							LOGGER.Info(string.Format("Re-trying previous J2534/%s connection: %s", Settings.
								GetTransportProtocol(), dll.GetVendor()));
							try
							{
								Settings.SetJ2534Device(dll.GetLibrary());
								return J2534TransportFactory.GetManager(Settings.GetTransportProtocol().ToUpper()
									, connectionProperties, dll.GetLibrary());
							}
							catch (Exception t)
							{
								Settings.SetJ2534Device(null);
								LOGGER.Info(string.Format("%s is not available: %s", dll.GetVendor(), t.Message));
							}
						}
					}
				}
				throw new RuntimeException("J2534 connection not available");
			}
			catch (Exception t)
			{
				Settings.SetJ2534Device(null);
				LOGGER.Info(string.Format("%s, trying serial connection...", t.Message));
				return new SerialConnectionManager(portName, connectionProperties);
			}
		}
	}
}
