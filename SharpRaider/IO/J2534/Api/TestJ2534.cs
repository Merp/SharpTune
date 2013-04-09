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
	/// an active ECU using the ISO9141 protocol.
	/// </summary>
	/// <remarks>
	/// This class is used to exercise the J2534 API against a real J2534 device and
	/// an active ECU using the ISO9141 protocol.
	/// </remarks>
	public class TestJ2534
	{
		private static readonly RomRaider.IO.J2534.Api.J2534 api = new J2534Impl(J2534Impl.Protocol
			.ISO9141, "op20pt32");

		public TestJ2534()
		{
			int deviceId = api.Open();
			try
			{
				Version(deviceId);
				int channelId = api.Connect(deviceId, J2534Impl.Flag.ISO9141_NO_CHECKSUM.value, 4800
					);
				try
				{
					SetConfig(channelId);
					GetConfig(channelId);
					int msgId = api.StartPassMsgFilter(channelId, unchecked((byte)unchecked((int)(0x00
						))), unchecked((byte)unchecked((int)(0x00))));
					try
					{
						byte[] ecuInit = new byte[] { unchecked((byte)unchecked((int)(0x80))), unchecked(
							(byte)unchecked((int)(0x10))), unchecked((byte)unchecked((int)(0xF0))), unchecked(
							(byte)unchecked((int)(0x01))), unchecked((byte)unchecked((int)(0xBF))), unchecked(
							(byte)unchecked((int)(0x40))) };
						api.WriteMsg(channelId, ecuInit, 55L, J2534Impl.TxFlags.NO_FLAGS);
						System.Console.Out.WriteLine("Request  = " + HexUtil.AsHex(ecuInit));
						byte[] response = api.ReadMsg(channelId, 1, 2000L);
						System.Console.Out.WriteLine("Response = " + HexUtil.AsHex(response));
					}
					finally
					{
						api.StopMsgFilter(channelId, msgId);
					}
				}
				finally
				{
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
			ConfigItem p1Max = new ConfigItem(J2534Impl.Config.P1_MAX.value, 2);
			ConfigItem p3Min = new ConfigItem(J2534Impl.Config.P3_MIN.value, 0);
			ConfigItem p4Min = new ConfigItem(J2534Impl.Config.P4_MIN.value, 0);
			ConfigItem loopback = new ConfigItem(J2534Impl.Config.LOOPBACK.value, 1);
			api.SetConfig(channelId, p1Max, p3Min, p4Min, loopback);
		}

		private static void GetConfig(int channelId)
		{
			ConfigItem[] configs = api.GetConfig(channelId, J2534Impl.Config.LOOPBACK.value, 
				J2534Impl.Config.P1_MAX.value, J2534Impl.Config.P3_MIN.value, J2534Impl.Config.P4_MIN
				.value);
			int i = 1;
			foreach (ConfigItem item in configs)
			{
				System.Console.Out.Printf("Config item %d: Parameter: %d, value:%d%n", i, item.parameter
					, item.value);
				i++;
			}
		}

		public static void Main(string[] args)
		{
			LogManager.InitDebugLogging();
			RomRaider.IO.J2534.Api.TestJ2534 a = new RomRaider.IO.J2534.Api.TestJ2534();
		}
	}
}
