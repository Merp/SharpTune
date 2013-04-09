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
using RomRaider.IO.Connection;
using RomRaider.IO.Protocol;
using RomRaider.Logger.Ecu.Comms.IO.Connection;
using RomRaider.Logger.Ecu.Comms.IO.Protocol;
using RomRaider.Logger.Ecu.Comms.Manager;
using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Comms.IO.Connection
{
	public sealed class SSMLoggerConnection : LoggerConnection
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(RomRaider.Logger.Ecu.Comms.IO.Connection.SSMLoggerConnection));

		private readonly LoggerProtocol protocol;

		private readonly ConnectionManager manager;

		public SSMLoggerConnection(ConnectionManager manager)
		{
			ParamChecker.CheckNotNull(manager, "manager");
			this.manager = manager;
			this.protocol = ProtocolFactory.GetProtocol(Settings.GetLoggerProtocol(), Settings
				.GetTransportProtocol());
		}

		public void EcuReset(byte id)
		{
			byte[] request = protocol.ConstructEcuResetRequest(id);
			LOGGER.Debug("Ecu Reset Request  ---> " + HexUtil.AsHex(request));
			byte[] response = manager.Send(request);
			byte[] processedResponse = protocol.PreprocessResponse(request, response, new PollingStateImpl
				());
			LOGGER.Debug("Ecu Reset Response <--- " + HexUtil.AsHex(processedResponse));
			protocol.ProcessEcuResetResponse(processedResponse);
		}

		public void EcuInit(EcuInitCallback callback, byte id)
		{
			byte[] request = protocol.ConstructEcuInitRequest(id);
			LOGGER.Debug("Ecu Init Request  ---> " + HexUtil.AsHex(request));
			byte[] response = manager.Send(request);
			byte[] processedResponse = protocol.PreprocessResponse(request, response, new PollingStateImpl
				());
			LOGGER.Debug("Ecu Init Response <--- " + HexUtil.AsHex(processedResponse));
			protocol.ProcessEcuInitResponse(callback, processedResponse);
		}

		public void SendAddressReads(ICollection<EcuQuery> queries, byte id, PollingState
			 pollState)
		{
			byte[] request = protocol.ConstructReadAddressRequest(id, queries);
			if (pollState.GetCurrentState() == 0)
			{
				LOGGER.Debug("Mode:" + pollState.GetCurrentState() + " ECU Request  ---> " + HexUtil.AsHex
					(request));
			}
			byte[] response = protocol.ConstructReadAddressResponse(queries, pollState);
			manager.Send(request, response, pollState);
			byte[] processedResponse = protocol.PreprocessResponse(request, response, pollState
				);
			LOGGER.Debug("Mode:" + pollState.GetCurrentState() + " ECU Response <--- " + HexUtil.AsHex
				(processedResponse));
			protocol.ProcessReadAddressResponses(queries, processedResponse, pollState);
		}

		public void ClearLine()
		{
			manager.ClearLine();
		}

		public void Close()
		{
			manager.Close();
		}
	}
}
