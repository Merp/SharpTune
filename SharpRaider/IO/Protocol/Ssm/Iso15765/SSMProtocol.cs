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
using System.IO;
using RomRaider.IO.Connection;
using RomRaider.IO.Protocol.Ssm.Iso15765;
using RomRaider.Logger.Ecu.Comms.Manager;
using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Logger.Ecu.Exception;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.IO.Protocol.Ssm.Iso15765
{
	public sealed class SSMProtocol : RomRaider.IO.Protocol.Protocol
	{
		private static readonly byte[] ECU_TESTER = new byte[] { unchecked((byte)unchecked(
			(int)(0x00))), unchecked((byte)unchecked((int)(0x00))), unchecked((byte)unchecked(
			(int)(0x07))), unchecked((byte)unchecked((int)(0xe0))) };

		private static readonly byte[] TCU_TESTER = new byte[] { unchecked((byte)unchecked(
			(int)(0x00))), unchecked((byte)unchecked((int)(0x00))), unchecked((byte)unchecked(
			(int)(0x07))), unchecked((byte)unchecked((int)(0xe1))) };

		private static byte[] ECU_CALID = new byte[] { unchecked((byte)unchecked((int)(0x00
			))), unchecked((byte)unchecked((int)(0x00))), unchecked((byte)unchecked((int)(0x07
			))), unchecked((byte)unchecked((int)(0xe8))) };

		private static readonly byte[] TCU_CALID = new byte[] { unchecked((byte)unchecked(
			(int)(0x00))), unchecked((byte)unchecked((int)(0x00))), unchecked((byte)unchecked(
			(int)(0x07))), unchecked((byte)unchecked((int)(0xe9))) };

		private static readonly byte[] resetAddress = new byte[] { unchecked((byte)unchecked(
			(int)(0x00))), unchecked((byte)unchecked((int)(0x00))), unchecked((byte)unchecked(
			(int)(0x60))) };

		private static byte[] TESTER = ECU_TESTER;

		public static byte[] ECU_ID = ECU_CALID;

		public const byte READ_MEMORY_PADDING = unchecked((byte)unchecked((int)(0x00)));

		public const byte READ_MEMORY_COMMAND = unchecked((byte)unchecked((int)(0xA0)));

		public const byte READ_MEMORY_RESPONSE = unchecked((byte)unchecked((int)(0xE0)));

		public const byte READ_ADDRESS_COMMAND = unchecked((byte)unchecked((int)(0xA8)));

		public const byte READ_ADDRESS_RESPONSE = unchecked((byte)unchecked((int)(0xE8)));

		public const byte WRITE_MEMORY_COMMAND = unchecked((byte)unchecked((int)(0xB0)));

		public const byte WRITE_MEMORY_RESPONSE = unchecked((byte)unchecked((int)(0xF0)));

		public const byte WRITE_ADDRESS_COMMAND = unchecked((byte)unchecked((int)(0xB8)));

		public const byte WRITE_ADDRESS_RESPONSE = unchecked((byte)unchecked((int)(0xF8))
			);

		public const byte ECU_INIT_COMMAND = unchecked((byte)unchecked((int)(0xAA)));

		public const byte ECU_INIT_RESPONSE = unchecked((byte)unchecked((int)(0xEA)));

		public const byte ECU_NRC = unchecked((byte)unchecked((int)(0x7F)));

		public const int ADDRESS_SIZE = 3;

		public const int DATA_SIZE = 1;

		public const int RESPONSE_NON_DATA_BYTES = 5;

		public byte[] ConstructEcuInitRequest(byte id)
		{
			ParamChecker.CheckGreaterThanZero(id, "ECU_ID");
			SetIDs(id);
			// 000007E0 AA
			return BuildRequest(ECU_INIT_COMMAND, false, new byte[0]);
		}

		public byte[] ConstructWriteMemoryRequest(byte id, byte[] address, byte[] values)
		{
			ParamChecker.CheckGreaterThanZero(id, "ECU_ID");
			ParamChecker.CheckNotNullOrEmpty(address, "address");
			ParamChecker.CheckNotNullOrEmpty(values, "values");
			SetIDs(id);
			// 000007E0 B0 from_address value1 value2 ... valueN
			throw new UnsupportedProtocolException("Write memory command is not supported on CAN for address: "
				 + HexUtil.AsHex(address));
		}

		public byte[] ConstructWriteAddressRequest(byte[] address, byte value)
		{
			ParamChecker.CheckNotNullOrEmpty(address, "address");
			ParamChecker.CheckNotNull(value, "value");
			// 000007E0 B8 address value
			return BuildRequest(WRITE_ADDRESS_COMMAND, false, address, new byte[] { value });
		}

		public byte[] ConstructReadMemoryRequest(byte id, byte[] address, int numBytes)
		{
			ParamChecker.CheckGreaterThanZero(id, "ECU_ID");
			ParamChecker.CheckNotNullOrEmpty(address, "address");
			ParamChecker.CheckGreaterThanZero(numBytes, "numBytes");
			SetIDs(id);
			// 000007E0 A0 padding from_address num_bytes-1
			throw new UnsupportedProtocolException("Read memory command is not supported on CAN for address: "
				 + HexUtil.AsHex(address));
		}

		public byte[] ConstructReadAddressRequest(byte id, IList<byte[]> addresses)
		{
			ParamChecker.CheckGreaterThanZero(id, "ECU_ID");
			ParamChecker.CheckNotNullOrEmpty(addresses, "addresses");
			SetIDs(id);
			// 000007E0 A8 padding [address1] [address2] ... [addressN]
			return BuildRequest(READ_ADDRESS_COMMAND, true, addresses);
		}

		public byte[] PreprocessResponse(byte[] request, byte[] response, PollingState pollState
			)
		{
			return SSMResponseProcessor.FilterRequestFromResponse(request, response, pollState
				);
		}

		public byte[] ParseResponseData(byte[] processedResponse)
		{
			ParamChecker.CheckNotNullOrEmpty(processedResponse, "processedResponse");
			return SSMResponseProcessor.ExtractResponseData(processedResponse);
		}

		public void CheckValidEcuInitResponse(byte[] processedResponse)
		{
			ParamChecker.CheckNotNullOrEmpty(processedResponse, "processedResponse");
			SSMResponseProcessor.ValidateResponse(processedResponse);
			// four byte - CAN ID
			// one byte  - Response Code
			// 3_unknown_bytes 5_ecu_id_bytes readable_params_switches...
			// 000007E8 EA A21011 5B125A4007 F3FAC98E0B81FEAC00820046CE54F...
			byte responseType = processedResponse[4];
			if (responseType != ECU_INIT_RESPONSE)
			{
				throw new InvalidResponseException("Unexpected ECU Init response type: " + HexUtil.AsHex
					(new byte[] { responseType }));
			}
		}

		public EcuInit ParseEcuInitResponse(byte[] processedResponse)
		{
			return new SSMEcuInit(ParseResponseData(processedResponse));
		}

		public byte[] ConstructEcuResetRequest(byte id)
		{
			ParamChecker.CheckGreaterThanZero(id, "ECU_ID");
			SetIDs(id);
			//  000007E0 B8 000060 40
			return ConstructWriteAddressRequest(resetAddress, unchecked((byte)unchecked((int)
				(0x40))));
		}

		public void CheckValidEcuResetResponse(byte[] processedResponse)
		{
			ParamChecker.CheckNotNullOrEmpty(processedResponse, "processedResponse");
			// 000007E8 F8 40
			byte responseType = processedResponse[4];
			if (responseType != WRITE_ADDRESS_RESPONSE || processedResponse[5] != unchecked((
				byte)unchecked((int)(0x40))))
			{
				throw new InvalidResponseException("Unexpected ECU Reset response: " + HexUtil.AsHex
					(processedResponse));
			}
		}

		public ConnectionProperties GetDefaultConnectionProperties()
		{
			return new _ConnectionProperties_166();
		}

		private sealed class _ConnectionProperties_166 : ConnectionProperties
		{
			public _ConnectionProperties_166()
			{
			}

			public int GetBaudRate()
			{
				return 500000;
			}

			public void SetBaudRate(int b)
			{
			}

			public int GetDataBits()
			{
				return 8;
			}

			public int GetStopBits()
			{
				return 1;
			}

			public int GetParity()
			{
				return 0;
			}

			public int GetConnectTimeout()
			{
				return 2000;
			}

			public int GetSendTimeout()
			{
				return 55;
			}
		}

		private byte[] BuildRequest(byte command, bool padContent, params byte[][] content
			)
		{
			int length = 0;
			foreach (byte[] tmp in content)
			{
				length += tmp.Length;
			}
			ByteArrayOutputStream bb = new ByteArrayOutputStream(length);
			try
			{
				bb.Write(TESTER);
				bb.Write(command);
				if (padContent)
				{
					bb.Write(READ_MEMORY_PADDING);
				}
				foreach (byte[] tmp_1 in content)
				{
					bb.Write(tmp_1);
				}
			}
			catch (IOException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
			return bb.ToByteArray();
		}

		private byte[] BuildRequest(byte command, bool padContent, IList<byte[]> content)
		{
			int length = 0;
			foreach (byte[] tmp in content)
			{
				length += tmp.Length;
			}
			ByteArrayOutputStream bb = new ByteArrayOutputStream(length);
			try
			{
				bb.Write(TESTER);
				bb.Write(command);
				if (padContent)
				{
					bb.Write(READ_MEMORY_PADDING);
				}
				foreach (byte[] tmp_1 in content)
				{
					bb.Write(tmp_1);
				}
			}
			catch (IOException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
			return bb.ToByteArray();
		}

		private void SetIDs(byte id)
		{
			ECU_ID = ECU_CALID;
			TESTER = ECU_TESTER;
			if (id == unchecked((int)(0x18)))
			{
				ECU_ID = TCU_CALID;
				TESTER = TCU_TESTER;
			}
		}
	}
}
