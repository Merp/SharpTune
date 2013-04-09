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

using System.Collections.Generic;
using RomRaider;
using RomRaider.Logger.Ecu.Comms.IO.Connection;
using RomRaider.Logger.Ecu.Comms.Manager;
using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Logger.Ecu.Exception;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Comms.Manager
{
	public sealed class TransmissionManagerImpl : TransmissionManager
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(RomRaider.Logger.Ecu.Comms.Manager.TransmissionManagerImpl));

		private readonly Settings settings;

		private LoggerConnection connection;

		public TransmissionManagerImpl(Settings settings)
		{
			ParamChecker.CheckNotNull(settings, "settings");
			this.settings = settings;
		}

		public void Start()
		{
			try
			{
				connection = LoggerConnectionFactory.GetConnection(Settings.GetLoggerProtocol(), 
					settings.GetLoggerPort(), settings.GetLoggerConnectionProperties());
				LOGGER.Info("TX Manager Started.");
			}
			catch
			{
				Stop();
			}
		}

		public void SendQueries(ICollection<EcuQuery> queries, PollingState pollState)
		{
			ParamChecker.CheckNotNull(queries, "queries");
			ParamChecker.CheckNotNull(pollState, "pollState");
			if (connection == null)
			{
				throw new NotConnectedException("TransmissionManager must be started before queries can be sent!"
					);
			}
			connection.SendAddressReads(queries, Settings.GetDestinationId(), pollState);
		}

		public void EndQueries()
		{
			if (connection == null)
			{
				throw new NotConnectedException("TransmissionManager must be started before ending queries!"
					);
			}
			connection.ClearLine();
		}

		public void Stop()
		{
			if (connection != null)
			{
				EndQueries();
				connection.Close();
			}
			LOGGER.Info("TX Manager Stopped.");
		}
	}
}
