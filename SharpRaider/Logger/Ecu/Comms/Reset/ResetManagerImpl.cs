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
using RomRaider.Logger.Ecu.Comms.IO.Connection;
using RomRaider.Logger.Ecu.Comms.Reset;
using RomRaider.Logger.Ecu.UI;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Comms.Reset
{
	public sealed class ResetManagerImpl : ResetManager
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.
			GetLogger(typeof(RomRaider.Logger.Ecu.Comms.Reset.ResetManagerImpl));

		private readonly Settings settings;

		private readonly MessageListener messageListener;

		public ResetManagerImpl(Settings settings, MessageListener messageListener)
		{
			ParamChecker.CheckNotNull(settings, messageListener);
			this.settings = settings;
			this.messageListener = messageListener;
		}

		public bool ResetEcu()
		{
			string target = "ECU";
			try
			{
				LoggerConnection connection = LoggerConnectionFactory.GetConnection(Settings.GetLoggerProtocol
					(), settings.GetLoggerPort(), settings.GetLoggerConnectionProperties());
				try
				{
					if (Settings.GetDestinationId() == unchecked((int)(0x18)))
					{
						target = "TCU";
					}
					messageListener.ReportMessage("Sending " + target + " Reset...");
					connection.EcuReset(Settings.GetDestinationId());
					messageListener.ReportMessage("Sending " + target + " Reset...done.");
					return true;
				}
				finally
				{
					connection.Close();
				}
			}
			catch (Exception e)
			{
				messageListener.ReportMessage("Unable to reset " + target + " - check correct serial port has been selected, cable is connected and ignition is on."
					);
				LOGGER.Error("Error sending " + target + " reset", e);
				return false;
			}
		}
	}
}
