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
using System.Text;
using RomRaider.IO.Protocol.Ssm.Iso15765;
using RomRaider.Logger.Ecu.Comms.Manager;
using RomRaider.Logger.Ecu.Exception;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.IO.Protocol.Ssm.Iso15765
{
	public sealed class SSMResponseProcessor
	{
		public SSMResponseProcessor()
		{
			throw new NotSupportedException();
		}

		public static byte[] FilterRequestFromResponse(byte[] request, byte[] response, PollingState
			 pollState)
		{
			ParamChecker.CheckNotNullOrEmpty(response, "response");
			return response;
		}

		public static void ValidateResponse(byte[] response)
		{
			AssertTrue(response.Length > SSMProtocol.RESPONSE_NON_DATA_BYTES, "Invalid response length"
				);
			AssertEquals(SSMProtocol.ECU_ID, response, "Invalid ECU id");
			if (response.Length == 7)
			{
				AssertNrc(SSMProtocol.ECU_NRC, response[4], response[5], response[6], "Request type not supported"
					);
			}
			AssertOneOf(new byte[] { SSMProtocol.ECU_INIT_RESPONSE, SSMProtocol.READ_ADDRESS_RESPONSE
				, SSMProtocol.READ_MEMORY_RESPONSE, SSMProtocol.WRITE_ADDRESS_RESPONSE, SSMProtocol
				.WRITE_MEMORY_RESPONSE }, response[4], "Invalid response code");
		}

		public static byte[] ExtractResponseData(byte[] response)
		{
			ParamChecker.CheckNotNullOrEmpty(response, "response");
			// 0x00 0x00 0x07 0xe0 response_command response_data
			ValidateResponse(response);
			byte[] data = new byte[response.Length - SSMProtocol.RESPONSE_NON_DATA_BYTES];
			System.Array.Copy(response, (SSMProtocol.RESPONSE_NON_DATA_BYTES), data, 0, data.
				Length);
			return data;
		}

		private static void AssertTrue(bool condition, string msg)
		{
			if (!condition)
			{
				throw new InvalidResponseException(msg);
			}
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
	}
}
