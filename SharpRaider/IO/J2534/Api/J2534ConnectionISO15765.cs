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
using RomRaider;
using RomRaider.IO.Connection;
using RomRaider.IO.J2534.Api;
using RomRaider.Logger.Ecu.Comms.Manager;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.IO.J2534.Api
{
	public sealed class J2534ConnectionISO15765 : ConnectionManager
	{
		private static readonly Logger LOGGER = Logger.GetLogger(typeof(RomRaider.IO.J2534.Api.J2534ConnectionISO15765
			));

		private RomRaider.IO.J2534.Api.J2534 api;

		private int channelId;

		private int deviceId;

		private int msgId;

		private long timeout;

		public J2534ConnectionISO15765(ConnectionProperties connectionProperties, string 
			library)
		{
			api = null;
			timeout = (long)2000;
			InitJ2534(500000, library);
			LOGGER.Info("J2534/ISO1576 connection initialized");
		}

		// Send request and wait for response with known length
		public void Send(byte[] request, byte[] response, PollingState pollState)
		{
			ParamChecker.CheckNotNull(request, "request");
			ParamChecker.CheckNotNull(response, "response");
			ParamChecker.CheckNotNull(pollState, "pollState");
			pollState.SetFastPoll(false);
			pollState.SetCurrentState(0);
			api.WriteMsg(channelId, request, timeout, J2534Impl.TxFlags.ISO15765_FRAME_PAD);
			System.Array.Copy(api.ReadMsg(channelId, 1, timeout), 0, response, 0, response.Length
				);
		}

		// Send request and wait specified time for one response with unknown length
		public byte[] Send(byte[] request)
		{
			ParamChecker.CheckNotNull(request, "request");
			api.WriteMsg(channelId, request, timeout, J2534Impl.TxFlags.ISO15765_FRAME_PAD);
			return api.ReadMsg(channelId, 1, timeout);
		}

		public void ClearLine()
		{
		}

		//        LOGGER.debug("J2534/ISO1576 clearing buffers");
		//        api.clearBuffers(channelId);
		public void Close()
		{
			StopFcFilter();
			DisconnectChannel();
			CloseDevice();
		}

		private void InitJ2534(int baudRate, string library)
		{
			api = new J2534Impl(J2534Impl.Protocol.ISO15765, library);
			deviceId = api.Open();
			try
			{
				Version(deviceId);
				channelId = api.Connect(deviceId, 0, baudRate);
				SetConfig(channelId);
				byte[] mask = new byte[] { unchecked((byte)unchecked((int)(0xff))), unchecked((byte
					)unchecked((int)(0xff))), unchecked((byte)unchecked((int)(0xff))), unchecked((byte
					)unchecked((int)(0xff))) };
				byte[] pattern = new byte[] { unchecked((byte)unchecked((int)(0x00))), unchecked(
					(byte)unchecked((int)(0x00))), unchecked((byte)unchecked((int)(0x07))), unchecked(
					(byte)unchecked((int)(0xe8))) };
				byte[] flowCntrl = new byte[] { unchecked((byte)unchecked((int)(0x00))), unchecked(
					(byte)unchecked((int)(0x00))), unchecked((byte)unchecked((int)(0x07))), unchecked(
					(byte)unchecked((int)(0xe0))) };
				if (Settings.GetDestinationId() == unchecked((int)(0x18)))
				{
					pattern = new byte[] { unchecked((byte)unchecked((int)(0x00))), unchecked((byte)unchecked(
						(int)(0x00))), unchecked((byte)unchecked((int)(0x07))), unchecked((byte)unchecked(
						(int)(0xe9))) };
					flowCntrl = new byte[] { unchecked((byte)unchecked((int)(0x00))), unchecked((byte
						)unchecked((int)(0x00))), unchecked((byte)unchecked((int)(0x07))), unchecked((byte
						)unchecked((int)(0xe1))) };
				}
				msgId = api.StartFlowCntrlFilter(channelId, mask, pattern, flowCntrl, J2534Impl.TxFlags
					.ISO15765_FRAME_PAD);
				LOGGER.Debug(string.Format("J2534/ISO1576 success: deviceId:%d, channelId:%d, msgId:%d"
					, deviceId, channelId, msgId));
			}
			catch (Exception e)
			{
				LOGGER.Debug(string.Format("J2534/ISO1576 exception: deviceId:%d, channelId:%d, msgId:%d"
					, deviceId, channelId, msgId));
				Close();
				throw new J2534Exception("J2534/ISO1576 Error opening device: " + e.Message, e);
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
			ConfigItem loopback = new ConfigItem(J2534Impl.Config.LOOPBACK.value, 0);
			ConfigItem bs = new ConfigItem(J2534Impl.Config.ISO15765_BS.value, 0);
			ConfigItem stMin = new ConfigItem(J2534Impl.Config.ISO15765_STMIN.value, 0);
			ConfigItem bs_tx = new ConfigItem(J2534Impl.Config.BS_TX.value, unchecked((int)(0xffff
				)));
			ConfigItem st_tx = new ConfigItem(J2534Impl.Config.STMIN_TX.value, unchecked((int
				)(0xffff)));
			ConfigItem wMax = new ConfigItem(J2534Impl.Config.ISO15765_WFT_MAX.value, 0);
			api.SetConfig(channelId, loopback, bs, stMin, bs_tx, st_tx, wMax);
		}

		private void StopFcFilter()
		{
			try
			{
				api.StopMsgFilter(channelId, msgId);
				LOGGER.Debug("J2534/ISO1576 stopped message filter:" + msgId);
			}
			catch (Exception e)
			{
				LOGGER.Warn("J2534/ISO1576 Error stopping msg filter: " + e.Message);
			}
		}

		private void DisconnectChannel()
		{
			try
			{
				api.Disconnect(channelId);
				LOGGER.Debug("J2534/ISO1576 disconnected channel:" + channelId);
			}
			catch (Exception e)
			{
				LOGGER.Warn("J2534/ISO1576 Error disconnecting channel: " + e.Message);
			}
		}

		private void CloseDevice()
		{
			try
			{
				api.Close(deviceId);
				LOGGER.Info("J2534/ISO1576 closed connection to device:" + deviceId);
			}
			catch (Exception e)
			{
				LOGGER.Warn("J2534/ISO1576 Error closing device: " + e.Message);
			}
		}
	}
}
