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

using RomRaider.IO.J2534.Api;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.IO.J2534.Api
{
	/// <summary>
	/// This class is used to exercise the J2534 API against a real J2534 device and
	/// an active ECU using the ISO14230 protocol.
	/// </summary>
	/// <remarks>
	/// This class is used to exercise the J2534 API against a real J2534 device and
	/// an active ECU using the ISO14230 protocol.
	/// </remarks>
	public sealed class TestJ2534OBD
	{
		private static readonly RomRaider.IO.J2534.Api.J2534 api = new J2534Impl(J2534Impl.Protocol
			.ISO14230, "op20pt32");

		private const int LOOPBACK = 0;

		/// <exception cref="System.Exception"></exception>
		public TestJ2534OBD()
		{
			//op20pt32 MONGI432
			int deviceId = api.Open();
			try
			{
				Version(deviceId);
				int channelId = api.Connect(deviceId, 0, 10400);
				double vBatt = api.GetVbattery(channelId);
				System.Console.Out.WriteLine("Pin 16 Volts = " + vBatt);
				int msgId = api.StartPassMsgFilter(channelId, unchecked((byte)unchecked((int)(0x00
					))), unchecked((byte)unchecked((int)(0x00))));
				try
				{
					byte[] startReq = new byte[] { unchecked((byte)unchecked((int)(0xC1))), unchecked(
						(byte)unchecked((int)(0x33))), unchecked((byte)unchecked((int)(0xF1))), unchecked(
						(byte)unchecked((int)(0x81))) };
					byte[] response = api.FastInit(channelId, startReq);
					System.Console.Out.WriteLine("Start Response = " + HexUtil.AsHex(response));
					if (response.Length > 0 && response[4] == unchecked((byte)unchecked((int)(0xE9)))
						 && response[5] == unchecked((byte)unchecked((int)(0x8F))))
					{
						System.Console.Out.WriteLine("Standard timing = " + HexUtil.AsHex(response));
					}
					SetConfig(channelId);
					GetConfig(channelId);
					Sharpen.Thread.Sleep(10L);
					byte[] timingReq = new byte[] { unchecked((byte)unchecked((int)(0xC2))), unchecked(
						(byte)unchecked((int)(0x33))), unchecked((byte)unchecked((int)(0xF1))), unchecked(
						(byte)unchecked((int)(0x83))), unchecked((byte)unchecked((int)(0x01))) };
					api.WriteMsg(channelId, timingReq, 55L, J2534Impl.TxFlags.NO_FLAGS);
					System.Console.Out.WriteLine("Timing Request  = " + HexUtil.AsHex(timingReq));
					response = api.ReadMsg(channelId, 1, 2000L);
					System.Console.Out.WriteLine("Timing Response = " + HexUtil.AsHex(response));
					Sharpen.Thread.Sleep(20L);
					byte[] mode09pid01 = new byte[] { unchecked((byte)unchecked((int)(0xC2))), unchecked(
						(byte)unchecked((int)(0x33))), unchecked((byte)unchecked((int)(0xF1))), unchecked(
						(byte)unchecked((int)(0x09))), unchecked((byte)unchecked((int)(0x01))) };
					api.WriteMsg(channelId, mode09pid01, 55L, J2534Impl.TxFlags.NO_FLAGS);
					System.Console.Out.WriteLine("PID0901 Request  = " + HexUtil.AsHex(mode09pid01));
					response = api.ReadMsg(channelId, 1, 2000L);
					System.Console.Out.WriteLine("PID0901 Response = " + HexUtil.AsHex(response));
					int numMsg = 0;
					switch (LOOPBACK)
					{
						case 0:
						{
							numMsg = response[5];
							break;
						}

						case 1:
						{
							numMsg = response[10];
						}
					}
					Sharpen.Thread.Sleep(10L);
					byte[] mode09pid02 = new byte[] { unchecked((byte)unchecked((int)(0xC2))), unchecked(
						(byte)unchecked((int)(0x33))), unchecked((byte)unchecked((int)(0xF1))), unchecked(
						(byte)unchecked((int)(0x09))), unchecked((byte)unchecked((int)(0x02))) };
					api.WriteMsg(channelId, mode09pid02, 55L, J2534Impl.TxFlags.NO_FLAGS);
					System.Console.Out.WriteLine("PID0902 Request  = " + HexUtil.AsHex(mode09pid02));
					response = api.ReadMsg(channelId, numMsg, 2000L);
					System.Console.Out.WriteLine("PID0902 Response = " + HexUtil.AsHex(response));
					Sharpen.Thread.Sleep(10L);
					byte[] stopReq = new byte[] { unchecked((byte)unchecked((int)(0xC1))), unchecked(
						(byte)unchecked((int)(0x33))), unchecked((byte)unchecked((int)(0xF1))), unchecked(
						(byte)unchecked((int)(0x82))) };
					api.WriteMsg(channelId, stopReq, 55L, J2534Impl.TxFlags.NO_FLAGS);
					System.Console.Out.WriteLine("Stop Request  = " + HexUtil.AsHex(stopReq));
					response = api.ReadMsg(channelId, 1, 2000L);
					System.Console.Out.WriteLine("Stop Response = " + HexUtil.AsHex(response));
				}
				finally
				{
					api.StopMsgFilter(channelId, msgId);
					api.Disconnect(channelId);
				}
			}
			finally
			{
				api.Close(deviceId);
			}
		}

		private static void Version(int deviceId)
		{
			RomRaider.IO.J2534.Api.Version version = api.ReadVersion(deviceId);
			System.Console.Out.Printf("Version => Firmware:[%s], DLL:[%s], API:[%s]%n", version
				.firmware, version.dll, version.api);
		}

		private static void SetConfig(int channelId)
		{
			ConfigItem p1Max = new ConfigItem(J2534Impl.Config.P1_MAX.value, 40);
			ConfigItem p3Min = new ConfigItem(J2534Impl.Config.P3_MIN.value, 110);
			ConfigItem p4Min = new ConfigItem(J2534Impl.Config.P4_MIN.value, 10);
			ConfigItem loopback = new ConfigItem(J2534Impl.Config.LOOPBACK.value, LOOPBACK);
			api.SetConfig(channelId, p1Max, p3Min, p4Min, loopback);
		}

		private static void GetConfig(int channelId)
		{
			ConfigItem[] configs = api.GetConfig(channelId, J2534Impl.Config.LOOPBACK.value, 
				J2534Impl.Config.P1_MAX.value, J2534Impl.Config.P3_MIN.value, J2534Impl.Config.P4_MIN
				.value);
			//Config.P1_MIN.getValue(),
			//Config.P2_MIN.getValue(),
			//Config.P2_MAX.getValue(),
			//Config.P3_MAX.getValue(),
			//Config.P4_MAX.getValue()
			int i = 1;
			foreach (ConfigItem item in configs)
			{
				System.Console.Out.Printf("Config item %d: Parameter: %s, value:%d%n", i, J2534Impl
					.GetConfig(item.parameter), item.value);
				i++;
			}
		}

		/// <exception cref="System.Exception"></exception>
		public static void Main(string[] args)
		{
			LogManager.InitDebugLogging();
			RomRaider.IO.J2534.Api.TestJ2534OBD test1 = new RomRaider.IO.J2534.Api.TestJ2534OBD
				();
		}
	}
}
