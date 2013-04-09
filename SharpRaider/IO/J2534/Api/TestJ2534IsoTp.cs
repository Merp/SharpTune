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
using System.IO;
using System.Text;
using RomRaider.IO.J2534.Api;
using RomRaider.Logger.Ecu.Exception;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.IO.J2534.Api
{
	/// <summary>
	/// This class is used to exercise the J2534 API against a real J2534 device and
	/// an active ECU using the ISO15765-2 protocol.
	/// </summary>
	/// <remarks>
	/// This class is used to exercise the J2534 API against a real J2534 device and
	/// an active ECU using the ISO15765-2 protocol.
	/// </remarks>
	public sealed class TestJ2534IsoTp
	{
		private static RomRaider.IO.J2534.Api.J2534 api;

		private const int LOOPBACK = 0;

		private static readonly byte[] mask1 = new byte[] { unchecked((byte)unchecked((int
			)(0xff))), unchecked((byte)unchecked((int)(0xff))), unchecked((byte)unchecked((int
			)(0xff))), unchecked((byte)unchecked((int)(0xff))) };

		private static readonly byte[] match1 = new byte[] { unchecked((byte)unchecked((int
			)(0x00))), unchecked((byte)unchecked((int)(0x00))), unchecked((byte)unchecked((int
			)(0x07))), unchecked((byte)unchecked((int)(0xe8))) };

		private static readonly byte[] fc1 = new byte[] { unchecked((byte)unchecked((int)
			(0x00))), unchecked((byte)unchecked((int)(0x00))), unchecked((byte)unchecked((int
			)(0x07))), unchecked((byte)unchecked((int)(0xe0))) };

		private const byte ECU_NRC = unchecked((byte)unchecked((int)(0x7F)));

		private const byte READ_MODE9_RESPONSE = unchecked((byte)unchecked((int)(0x49)));

		private const byte READ_MODE9_COMMAND = unchecked((byte)unchecked((int)(0x09)));

		private static readonly byte[] READ_MODE9_PIDS = new byte[] { unchecked((byte)unchecked(
			(int)(0x02))), unchecked((byte)unchecked((int)(0x04))), unchecked((byte)unchecked(
			(int)(0x06))) };

		private const byte READ_MODE3_RESPONSE = unchecked((byte)unchecked((int)(0x43)));

		private const byte READ_MODE3_COMMAND = unchecked((byte)unchecked((int)(0x03)));

		private const byte ECU_INIT_COMMAND = unchecked((byte)unchecked((int)(0x01)));

		private const byte ECU_INIT_RESPONSE = unchecked((byte)unchecked((int)(0x41)));

		private static readonly StringBuilder sb = new StringBuilder();

		/// <exception cref="System.Exception"></exception>
		public TestJ2534IsoTp()
		{
			int deviceId = api.Open();
			sb.Delete(0, sb.Capacity());
			try
			{
				Version(deviceId);
				int channelId = api.Connect(deviceId, 0, 500000);
				double vBatt = api.GetVbattery(deviceId);
				//            System.out.println("Pin 16: " + vBatt + " VDC");
				sb.Append(string.Format("J2534 Interface Pin 16: %sVDC%n", vBatt));
				byte[] mask2 = new byte[] { unchecked((byte)unchecked((int)(0xff))), unchecked((byte
					)unchecked((int)(0xff))), unchecked((byte)unchecked((int)(0xff))), unchecked((byte
					)unchecked((int)(0xff))) };
				byte[] match2 = new byte[] { unchecked((byte)unchecked((int)(0x00))), unchecked((
					byte)unchecked((int)(0x00))), unchecked((byte)unchecked((int)(0x07))), unchecked(
					(byte)unchecked((int)(0xdf))) };
				byte[] fc2 = new byte[] { unchecked((byte)unchecked((int)(0x00))), unchecked((byte
					)unchecked((int)(0x00))), unchecked((byte)unchecked((int)(0x07))), unchecked((byte
					)unchecked((int)(0xdf))) };
				//Tester
				byte[] mask3 = new byte[] { unchecked((byte)unchecked((int)(0xff))), unchecked((byte
					)unchecked((int)(0xff))), unchecked((byte)unchecked((int)(0xff))), unchecked((byte
					)unchecked((int)(0xff))) };
				byte[] match3 = new byte[] { unchecked((byte)unchecked((int)(0x00))), unchecked((
					byte)unchecked((int)(0x00))), unchecked((byte)unchecked((int)(0x07))), unchecked(
					(byte)unchecked((int)(0xe0))) };
				byte[] fc3 = new byte[] { unchecked((byte)unchecked((int)(0x00))), unchecked((byte
					)unchecked((int)(0x00))), unchecked((byte)unchecked((int)(0x07))), unchecked((byte
					)unchecked((int)(0xe0))) };
				int msgId = api.StartFlowCntrlFilter(channelId, mask1, match1, fc1, J2534Impl.TxFlags
					.ISO15765_FRAME_PAD);
				//            final int msgId1 = api.startFlowCntrlFilter(
				//                    channelId, mask2, match2, fc2, TxFlags.ISO15765_FRAME_PAD);
				//            final int msgId2 = api.startFlowCntrlFilter(
				//                    channelId, mask3, match3, fc3, TxFlags.ISO15765_FRAME_PAD);
				try
				{
					SetConfig(channelId);
					GetConfig(channelId);
					sb.Append(string.Format("%n--- Vehicle Information ---%n"));
					foreach (byte pid in READ_MODE9_PIDS)
					{
						byte[] mode9 = BuildRequest(READ_MODE9_COMMAND, pid, true, fc1);
						// mode
						// pid
						// pid valid
						// source
						api.WriteMsg(channelId, mode9, 1000L, J2534Impl.TxFlags.ISO15765_FRAME_PAD);
						byte[] response;
						response = api.ReadMsg(channelId, 1, 1000L);
						//                    System.out.println("Response = " +
						//                            HexUtil.asHex(response));
						HandleResponse(response);
					}
					byte[] mode3 = BuildRequest(READ_MODE3_COMMAND, unchecked((byte)unchecked((int)(0x00
						))), false, fc1);
					// mode
					// pid
					// pid valid
					// source
					api.WriteMsg(channelId, mode3, 1000L, J2534Impl.TxFlags.ISO15765_FRAME_PAD);
					byte[] response_1;
					response_1 = api.ReadMsg(channelId, 1, 1000L);
					//                System.out.println("Response = " +
					//                        HexUtil.asHex(response));
					HandleResponse(response_1);
				}
				catch (Exception e)
				{
					//                System.out.println(e);
					sb.Append(e);
				}
				finally
				{
					api.StopMsgFilter(channelId, msgId);
					//                api.stopMsgFilter(channelId, msgId1);
					//                api.stopMsgFilter(channelId, msgId2);
					api.Disconnect(channelId);
				}
			}
			finally
			{
				api.Close(deviceId);
			}
		}

		public sealed override string ToString()
		{
			return sb.ToString();
		}

		private static void HandleResponse(byte[] response)
		{
			ValidateResponse(response);
			int responseLen = response.Length;
			if (response[4] == READ_MODE9_RESPONSE)
			{
				byte[] data = new byte[responseLen - 7];
				System.Array.Copy(response, 7, data, 0, data.Length);
				if (response[5] == unchecked((int)(0x02)))
				{
					//                System.out.println("VIN: " + new String(data));
					sb.Append(string.Format("VIN: %s%n", Sharpen.Runtime.GetStringForBytes(data)));
				}
				if (response[5] == unchecked((int)(0x04)))
				{
					int i;
					for (i = 0; i < data.Length && data[i] != 0; i++)
					{
					}
					string str = Sharpen.Runtime.GetStringForBytes(data, 0, i);
					//                System.out.println("CAL ID: " + str);
					sb.Append(string.Format("CAL ID: %s%n", str));
				}
				if (response[5] == unchecked((int)(0x06)))
				{
					//                System.out.println("CVN: " + HexUtil.asHex(data));
					sb.Append(string.Format("CVN: %s%n", HexUtil.AsHex(data)));
				}
				if (response[5] == unchecked((int)(0x08)))
				{
				}
				//                System.out.println("PID_8: " + HexUtil.asHex(data));
				if (response[5] == unchecked((int)(0x0A)))
				{
					int i;
					for (i = 0; i < data.Length && data[i] != 0; i++)
					{
					}
					string str = Sharpen.Runtime.GetStringForBytes(data, 0, i);
					//                System.out.println("Module: " + str);
					sb.Append(string.Format("Module: %s%n", str));
				}
			}
			if (response[4] == READ_MODE3_RESPONSE)
			{
				if (response[5] > unchecked((int)(0x00)))
				{
					byte[] data = new byte[responseLen - 6];
					System.Array.Copy(response, 6, data, 0, data.Length);
					int i;
					int j = 1;
					byte[] codeHex = new byte[2];
					for (i = 0; i < data.Length; i = i + 2)
					{
						System.Array.Copy(data, i, codeHex, 0, 2);
						byte module = unchecked((byte)((codeHex[0] & unchecked((int)(0xC0))) >> 6));
						string moduleTxt = null;
						switch (module)
						{
							case 0:
							{
								moduleTxt = "P";
								break;
							}

							case 1:
							{
								moduleTxt = "C";
								break;
							}

							case 2:
							{
								moduleTxt = "B";
								break;
							}

							case 3:
							{
								moduleTxt = "U";
								break;
							}
						}
						byte dtcB1 = unchecked((byte)((codeHex[0] & unchecked((int)(0x30))) >> 4));
						byte dtcB2 = unchecked((byte)(codeHex[0] & unchecked((int)(0x0F))));
						byte dtcB3 = unchecked((byte)((codeHex[1] & unchecked((int)(0xF0))) >> 4));
						byte dtcB4 = unchecked((byte)(codeHex[1] & unchecked((int)(0x0F))));
						//                    System.out.print(
						//                            String.format("DTC %d: %s%s%s%s%s%n",
						//                                    j,
						//                                    moduleTxt,
						//                                    Character.forDigit(dtcB1, 16),
						//                                    Character.forDigit(dtcB2, 16),
						//                                    Character.forDigit(dtcB3, 16),
						//                                    Character.forDigit(dtcB4, 16)));
						sb.Append(string.Format("DTC %d: %s%s%s%s%s%n", j, moduleTxt, char.ForDigit(dtcB1
							, 16), char.ForDigit(dtcB2, 16), char.ForDigit(dtcB3, 16), char.ForDigit(dtcB4, 
							16)));
						j++;
					}
				}
			}
		}

		private static void Version(int deviceId)
		{
			RomRaider.IO.J2534.Api.Version version = api.ReadVersion(deviceId);
			//        System.out.printf("Version => Firmware:[%s], DLL:[%s], API:[%s]%n",
			//                version.firmware, version.dll, version.api);
			sb.Append(string.Format("J2534 Firmware:[%s], DLL:[%s], API:[%s]%n", version.firmware
				, version.dll, version.api));
		}

		private static void SetConfig(int channelId)
		{
			ConfigItem loopback = new ConfigItem(J2534Impl.Config.LOOPBACK.value, LOOPBACK);
			ConfigItem bs = new ConfigItem(J2534Impl.Config.ISO15765_BS.value, 0);
			ConfigItem stMin = new ConfigItem(J2534Impl.Config.ISO15765_STMIN.value, 0);
			ConfigItem bs_tx = new ConfigItem(J2534Impl.Config.BS_TX.value, unchecked((int)(0xffff
				)));
			ConfigItem st_tx = new ConfigItem(J2534Impl.Config.STMIN_TX.value, unchecked((int
				)(0xffff)));
			ConfigItem wMax = new ConfigItem(J2534Impl.Config.ISO15765_WFT_MAX.value, 0);
			api.SetConfig(channelId, loopback, bs, stMin, bs_tx, st_tx, wMax);
		}

		private static void GetConfig(int channelId)
		{
			ConfigItem[] configs = api.GetConfig(channelId, J2534Impl.Config.LOOPBACK.value, 
				J2534Impl.Config.ISO15765_BS.value, J2534Impl.Config.ISO15765_STMIN.value, J2534Impl.Config
				.BS_TX.value, J2534Impl.Config.STMIN_TX.value, J2534Impl.Config.ISO15765_WFT_MAX
				.value);
			int i = 1;
			foreach (ConfigItem item in configs)
			{
				//            System.out.printf("J2534 Config item %d: Parameter: %s, value:%d%n",
				//                    i, Config.get(item.parameter), item.value);
				sb.Append(string.Format("J2534 Config item %d: Parameter: %s, value:%d%n", i, J2534Impl
					.GetConfig(item.parameter), item.value));
				i++;
			}
		}

		private static void ValidateResponse(byte[] response)
		{
			AssertEquals(match1, response, "Invalid ECU id");
			if (response.Length == 7)
			{
				AssertNrc(ECU_NRC, response[4], response[5], response[6], "Request type not supported"
					);
			}
			AssertOneOf(new byte[] { ECU_INIT_RESPONSE, READ_MODE3_RESPONSE, READ_MODE9_RESPONSE
				 }, response[4], "Invalid response code");
		}

		private static void AssertNrc(byte expected, byte actual, byte command, byte code
			, string msg)
		{
			if (actual == expected)
			{
				string ec = " unsupported.";
				if (code == unchecked((int)(0x13)))
				{
					ec = " invalid format or length.";
				}
				throw new InvalidResponseException(msg + ". Command: " + HexUtil.AsHex(new byte[]
					 { command }) + ec);
			}
		}

		private static void AssertEquals(byte[] expected, byte[] actual, string msg)
		{
			byte[] idBytes = new byte[4];
			System.Array.Copy(actual, 0, idBytes, 0, 4);
			int idExpected = ByteUtil.AsUnsignedInt(expected);
			int idActual = ByteUtil.AsUnsignedInt(idBytes);
			if (idActual != idExpected)
			{
				throw new InvalidResponseException(msg + ". Expected: " + HexUtil.AsHex(expected)
					 + ". Actual: " + HexUtil.AsHex(idBytes) + ".");
			}
		}

		private static void AssertOneOf(byte[] validOptions, byte actual, string msg)
		{
			foreach (byte option in validOptions)
			{
				if (option == actual)
				{
					return;
				}
			}
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < validOptions.Length; i++)
			{
				if (i > 0)
				{
					builder.Append(", ");
				}
				builder.Append(HexUtil.AsHex(new byte[] { validOptions[i] }));
			}
			throw new InvalidResponseException(msg + ". Expected one of [" + builder.ToString
				() + "]. Actual: " + HexUtil.AsHex(new byte[] { actual }) + ".");
		}

		private static byte[] BuildRequest(byte mode, byte pid, bool pidValid, params byte
			[][] content)
		{
			ByteArrayOutputStream bb = new ByteArrayOutputStream(6);
			try
			{
				foreach (byte[] tmp in content)
				{
					bb.Write(tmp);
				}
				bb.Write(mode);
				if (pidValid)
				{
					bb.Write(pid);
				}
			}
			catch (IOException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
			return bb.ToByteArray();
		}

		/// <exception cref="System.Exception"></exception>
		public static void Main(string[] args)
		{
			LogManager.InitDebugLogging();
			if (args.Length == 0)
			{
				//op20pt32 MONGI432
				api = new J2534Impl(J2534Impl.Protocol.ISO15765, "op20pt32");
			}
			else
			{
				api = new J2534Impl(J2534Impl.Protocol.ISO15765, args[0]);
			}
			RomRaider.IO.J2534.Api.TestJ2534IsoTp test1 = new RomRaider.IO.J2534.Api.TestJ2534IsoTp
				();
			System.Console.Out.Write(test1.ToString());
		}
	}
}
