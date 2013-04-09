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
using RomRaider.IO.Protocol.Ssm.Iso9141;
using RomRaider.Logger.Ecu.Comms.Manager;
using RomRaider.Logger.Ecu.Exception;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.IO.Protocol.Ssm.Iso9141
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
			ParamChecker.CheckNotNull(request, "request");
			ParamChecker.CheckNotNullOrEmpty(response, "response");
			ParamChecker.CheckNotNull(pollState, "pollState");
			byte[] filteredResponse = new byte[0];
			if (request[4] != SSMProtocol.READ_ADDRESS_COMMAND || pollState.GetCurrentState()
				 == 0)
			{
				filteredResponse = new byte[response.Length - request.Length];
				System.Array.Copy(response, request.Length, filteredResponse, 0, filteredResponse
					.Length);
			}
			if (request[4] == SSMProtocol.READ_ADDRESS_COMMAND && pollState.GetCurrentState()
				 == 1)
			{
				filteredResponse = new byte[response.Length];
				System.Array.Copy(response, 0, filteredResponse, 0, filteredResponse.Length);
			}
			return filteredResponse;
		}

		public static void ValidateResponse(byte[] response)
		{
			int i = 0;
			AssertTrue(response.Length > SSMProtocol.RESPONSE_NON_DATA_BYTES, "Invalid response length"
				);
			AssertEquals(SSMProtocol.HEADER, response[i++], "Invalid header");
			AssertEquals(SSMProtocol.DIAGNOSTIC_TOOL_ID, response[i++], "Invalid diagnostic tool id"
				);
			AssertEquals(SSMProtocol.ECU_ID, response[i++], "Invalid ECU id");
			AssertEquals(ByteUtil.AsByte(response.Length - SSMProtocol.RESPONSE_NON_DATA_BYTES
				 + 1), response[i++], "Invalid response data length");
			AssertOneOf(new byte[] { SSMProtocol.ECU_INIT_RESPONSE, SSMProtocol.READ_ADDRESS_RESPONSE
				, SSMProtocol.READ_MEMORY_RESPONSE, SSMProtocol.WRITE_ADDRESS_RESPONSE, SSMProtocol
				.WRITE_MEMORY_RESPONSE }, response[i], "Invalid response code");
			AssertEquals(SSMChecksumCalculator.CalculateChecksum(response), response[response
				.Length - 1], "Invalid checksum");
		}

		public static byte[] ExtractResponseData(byte[] response)
		{
			ParamChecker.CheckNotNullOrEmpty(response, "response");
			// 0x80 0xF0 0x10 data_length 0xE8 response_data checksum
			ValidateResponse(response);
			byte[] data = new byte[response.Length - SSMProtocol.RESPONSE_NON_DATA_BYTES];
			System.Array.Copy(response, (SSMProtocol.RESPONSE_NON_DATA_BYTES - 1), data, 0, data
				.Length);
			return data;
		}

		private static void AssertTrue(bool condition, string msg)
		{
			if (!condition)
			{
				throw new InvalidResponseException(msg);
			}
		}

		private static void AssertEquals(byte expected, byte actual, string msg)
		{
			if (actual != expected)
			{
				throw new InvalidResponseException(msg + ". Expected: " + HexUtil.AsHex(new byte[
					] { expected }) + ". Actual: " + HexUtil.AsHex(new byte[] { actual }) + ".");
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
