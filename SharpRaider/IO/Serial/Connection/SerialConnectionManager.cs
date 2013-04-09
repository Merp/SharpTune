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

using Org.Apache.Log4j;
using RomRaider.IO.Connection;
using RomRaider.IO.Protocol.Ssm.Iso9141;
using RomRaider.IO.Serial.Connection;
using RomRaider.Logger.Ecu.Comms.Manager;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.IO.Serial.Connection
{
	public sealed class SerialConnectionManager : ConnectionManager
	{
		private static readonly Logger LOGGER = Logger.GetLogger(typeof(RomRaider.IO.Serial.Connection.SerialConnectionManager
			));

		private readonly SerialConnection connection;

		private readonly ConnectionProperties connectionProperties;

		private byte[] lastResponse;

		private readonly long timeout;

		private long readTimeout;

		public SerialConnectionManager(string portName, ConnectionProperties connectionProperties
			)
		{
			ParamChecker.CheckNotNullOrEmpty(portName, "portName");
			ParamChecker.CheckNotNull(connectionProperties, "connectionProperties");
			this.connectionProperties = connectionProperties;
			timeout = (long)connectionProperties.GetConnectTimeout();
			readTimeout = timeout;
			// Use TestSerialConnection for testing!!
			connection = new SerialConnectionImpl(portName, connectionProperties);
		}

		//        connection = new TestSerialConnection2(portName, connectionProperties);
		// Send request and wait for response with known length
		public void Send(byte[] request, byte[] response, PollingState pollState)
		{
			ParamChecker.CheckNotNull(request, "request");
			ParamChecker.CheckNotNull(response, "response");
			ParamChecker.CheckNotNull(pollState, "pollState");
			if (pollState.GetCurrentState() == 0 && pollState.GetLastState() == 1)
			{
				ClearLine();
			}
			if (pollState.GetCurrentState() == 0)
			{
				connection.ReadStaleData();
				connection.Write(request);
			}
			while (connection.Available() < response.Length)
			{
				ThreadUtil.Sleep(1);
				readTimeout -= 1;
				if (readTimeout <= 0)
				{
					byte[] badBytes = connection.ReadAvailable();
					LOGGER.Debug("SSM Bad read response (read timeout): " + HexUtil.AsHex(badBytes));
					return;
				}
			}
			// this will reinitialize the connection
			readTimeout = timeout;
			connection.Read(response);
			if (pollState.GetCurrentState() == 1)
			{
				if (response[0] == unchecked((byte)unchecked((int)(0x80))) && response[1] == unchecked(
					(byte)unchecked((int)(0xF0))) && (response[2] == unchecked((byte)unchecked((int)
					(0x10))) || response[2] == unchecked((byte)unchecked((int)(0x18)))) && response[
					3] == (response.Length - 5) && response[response.Length - 1] == SSMChecksumCalculator.CalculateChecksum
					(response))
				{
					lastResponse = new byte[response.Length];
					System.Array.Copy(response, 0, lastResponse, 0, response.Length);
				}
				else
				{
					LOGGER.Error("SSM Bad Data response: " + HexUtil.AsHex(response));
					System.Array.Copy(lastResponse, 0, response, 0, response.Length);
					pollState.SetNewQuery(true);
				}
			}
		}

		// Send request and wait specified time for response with unknown length
		public byte[] Send(byte[] bytes)
		{
			ParamChecker.CheckNotNull(bytes, "bytes");
			connection.ReadStaleData();
			connection.Write(bytes);
			int available = 0;
			bool keepLooking = true;
			long lastChange = Runtime.CurrentTimeMillis();
			while (keepLooking)
			{
				ThreadUtil.Sleep(2);
				if (connection.Available() != available)
				{
					available = connection.Available();
					lastChange = Runtime.CurrentTimeMillis();
				}
				keepLooking = (Runtime.CurrentTimeMillis() - lastChange) < timeout;
			}
			return connection.ReadAvailable();
		}

		public void ClearLine()
		{
			LOGGER.Debug("SSM sending line break");
			connection.SendBreak(1 / (connectionProperties.GetBaudRate() * (connectionProperties
				.GetDataBits() + connectionProperties.GetStopBits() + connectionProperties.GetParity
				() + 1)));
			do
			{
				ThreadUtil.Sleep(2);
				byte[] badBytes = connection.ReadAvailable();
				LOGGER.Debug("SSM clearing line (stale data): " + HexUtil.AsHex(badBytes));
				ThreadUtil.Sleep(10);
			}
			while (connection.Available() > 0);
		}

		public void Close()
		{
			connection.Close();
		}
	}
}
