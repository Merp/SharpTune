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
using Org.Apache.Log4j;
using RomRaider.IO.Connection;
using RomRaider.IO.J2534.Api;
using RomRaider.IO.Protocol.Ssm.Iso9141;
using RomRaider.Logger.Ecu.Comms.Manager;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.IO.J2534.Api
{
	public sealed class J2534ConnectionISO9141 : ConnectionManager
	{
		private static readonly Logger LOGGER = Logger.GetLogger(typeof(RomRaider.IO.J2534.Api.J2534ConnectionISO9141
			));

		private RomRaider.IO.J2534.Api.J2534 api = null;

		private int channelId;

		private int deviceId;

		private int msgId;

		private byte[] lastResponse;

		private long timeout;

		public J2534ConnectionISO9141(ConnectionProperties connectionProperties, string library
			)
		{
			ParamChecker.CheckNotNull(connectionProperties, "connectionProperties");
			timeout = (long)connectionProperties.GetConnectTimeout();
			InitJ2534(connectionProperties.GetBaudRate(), library);
			LOGGER.Info("J2534/ISO9141 connection initialised");
		}

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
				api.WriteMsg(channelId, request, timeout, J2534Impl.TxFlags.NO_FLAGS);
			}
			api.ReadMsg(channelId, response, timeout);
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
					LOGGER.Error("J2534/ISO9141 Bad Data response: " + HexUtil.AsHex(response));
					System.Array.Copy(lastResponse, 0, response, 0, response.Length);
					pollState.SetNewQuery(true);
				}
			}
		}

		// Send request and wait specified time for response with unknown length
		public byte[] Send(byte[] request)
		{
			ParamChecker.CheckNotNull(request, "request");
			api.WriteMsg(channelId, request, timeout, J2534Impl.TxFlags.NO_FLAGS);
			return api.ReadMsg(channelId, 1, timeout);
		}

		public void ClearLine()
		{
			LOGGER.Debug("J2534/ISO9141 sending line break");
			api.WriteMsg(channelId, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 100L, J2534Impl.TxFlags
				.NO_FLAGS);
			bool empty = false;
			do
			{
				byte[] badBytes = api.ReadMsg(channelId, 100L);
				if (badBytes.Length > 0)
				{
					LOGGER.Debug("J2534/ISO9141 clearing line (stale data): " + HexUtil.AsHex(badBytes
						));
					empty = false;
				}
				else
				{
					empty = true;
				}
			}
			while (!empty);
		}

		public void Close()
		{
			StopMsgFilter();
			DisconnectChannel();
			CloseDevice();
		}

		private void InitJ2534(int baudRate, string library)
		{
			api = new J2534Impl(J2534Impl.Protocol.ISO9141, library);
			deviceId = api.Open();
			try
			{
				Version(deviceId);
				channelId = api.Connect(deviceId, J2534Impl.Flag.ISO9141_NO_CHECKSUM.value, baudRate
					);
				SetConfig(channelId);
				msgId = api.StartPassMsgFilter(channelId, unchecked((byte)unchecked((int)(0x00)))
					, unchecked((byte)unchecked((int)(0x00))));
				LOGGER.Debug(string.Format("J2534/ISO9141 success: deviceId:%d, channelId:%d, msgId:%d"
					, deviceId, channelId, msgId));
			}
			catch (Exception e)
			{
				LOGGER.Debug(string.Format("J2534/ISO9141 exception: deviceId:%d, channelId:%d, msgId:%d"
					, deviceId, channelId, msgId));
				Close();
				throw new J2534Exception("J2534/ISO9141 Error opening device: " + e.Message, e);
			}
		}

		private void Version(int deviceId)
		{
			if (!LOGGER.IsDebugEnabled())
			{
				return;
			}
			RomRaider.IO.J2534.Api.Version version = api.ReadVersion(deviceId);
			LOGGER.Info("J2534 Version => firmware: " + version.firmware + ", dll: " + version
				.dll + ", api: " + version.api);
		}

		private void SetConfig(int channelId)
		{
			ConfigItem p1Max = new ConfigItem(J2534Impl.Config.P1_MAX.value, 2);
			ConfigItem p3Min = new ConfigItem(J2534Impl.Config.P3_MIN.value, 0);
			ConfigItem p4Min = new ConfigItem(J2534Impl.Config.P4_MIN.value, 0);
			ConfigItem loopback = new ConfigItem(J2534Impl.Config.LOOPBACK.value, 1);
			api.SetConfig(channelId, p1Max, p3Min, p4Min, loopback);
		}

		private void StopMsgFilter()
		{
			try
			{
				api.StopMsgFilter(channelId, msgId);
				LOGGER.Debug("J2534/ISO9141 stopped message filter:" + msgId);
			}
			catch (Exception e)
			{
				LOGGER.Warn("J2534/ISO9141 Error stopping msg filter: " + e.Message);
			}
		}

		private void DisconnectChannel()
		{
			try
			{
				api.Disconnect(channelId);
				LOGGER.Debug("J2534/ISO9141 disconnected channel:" + channelId);
			}
			catch (Exception e)
			{
				LOGGER.Warn("J2534/ISO9141 Error disconnecting channel: " + e.Message);
			}
		}

		private void CloseDevice()
		{
			try
			{
				api.Close(deviceId);
				LOGGER.Info("J2534/ISO9141 closed connection to device:" + deviceId);
			}
			catch (Exception e)
			{
				LOGGER.Warn("J2534/ISO9141 Error closing device: " + e.Message);
			}
		}
	}
}
