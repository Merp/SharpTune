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
using RomRaider.IO.Protocol.Ssm.Iso9141;
using RomRaider.Logger.Ecu.Comms.Manager;
using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Logger.Ecu.Exception;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.IO.Protocol.Ssm.Iso9141
{
	public sealed class SSMProtocol : RomRaider.IO.Protocol.Protocol
	{
		public const byte HEADER = unchecked((byte)unchecked((int)(0x80)));

		public static byte ECU_ID = unchecked((byte)unchecked((int)(0x10)));

		public const byte DIAGNOSTIC_TOOL_ID = unchecked((byte)unchecked((int)(0xF0)));

		public const byte READ_ADDRESS_ONCE = unchecked((byte)unchecked((int)(0x00)));

		public const byte READ_ADDRESS_CONTINUOUS = unchecked((byte)unchecked((int)(0x01)
			));

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

		public const byte ECU_INIT_COMMAND = unchecked((byte)unchecked((int)(0xBF)));

		public const byte ECU_INIT_RESPONSE = unchecked((byte)unchecked((int)(0xFF)));

		public const int ADDRESS_SIZE = 3;

		public const int DATA_SIZE = 1;

		public const int RESPONSE_NON_DATA_BYTES = 6;

		public const int REQUEST_NON_DATA_BYTES = 7;

		private readonly PollingState pollState = new PollingStateImpl();

		public byte[] ConstructEcuInitRequest(byte id)
		{
			ParamChecker.CheckGreaterThanZero(id, "ECU_ID");
			SSMProtocol.ECU_ID = id;
			// 0x80 0x10 0xF0 0x01 0xBF 0x40
			return BuildRequest(ECU_INIT_COMMAND, false, new byte[0]);
		}

		public byte[] ConstructWriteMemoryRequest(byte id, byte[] address, byte[] values)
		{
			ParamChecker.CheckGreaterThanZero(id, "ECU_ID");
			ParamChecker.CheckNotNullOrEmpty(address, "address");
			ParamChecker.CheckNotNullOrEmpty(values, "values");
			SSMProtocol.ECU_ID = id;
			// 0x80 0x10 0xF0 data_length 0xB0 from_address value1 value2 ... valueN checksum
			return BuildRequest(WRITE_MEMORY_COMMAND, false, address, values);
		}

		public byte[] ConstructWriteAddressRequest(byte[] address, byte value)
		{
			ParamChecker.CheckNotNullOrEmpty(address, "address");
			ParamChecker.CheckNotNull(value, "value");
			// 0x80 0x10 0xF0 data_length 0xB8 from_address value checksum
			return BuildRequest(WRITE_ADDRESS_COMMAND, false, address, new byte[] { value });
		}

		public byte[] ConstructReadMemoryRequest(byte id, byte[] address, int numBytes)
		{
			ParamChecker.CheckGreaterThanZero(id, "ECU_ID");
			ParamChecker.CheckNotNullOrEmpty(address, "address");
			ParamChecker.CheckGreaterThanZero(numBytes, "numBytes");
			SSMProtocol.ECU_ID = id;
			// 0x80 0x10 0xF0 data_length 0xA0 padding from_address num_bytes-1 checksum
			return BuildRequest(READ_MEMORY_COMMAND, true, address, new byte[] { ByteUtil.AsByte
				(numBytes - 1) });
		}

		public byte[] ConstructReadAddressRequest(byte id, IList<byte[]> addresses)
		{
			ParamChecker.CheckGreaterThanZero(id, "ECU_ID");
			ParamChecker.CheckNotNullOrEmpty(addresses, "addresses");
			SSMProtocol.ECU_ID = id;
			// 0x80 0x10 0xF0 data_length 0xA8 padding address1 address2 ... addressN checksum
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
			// response_header 3_unknown_bytes 5_ecu_id_bytes readable_params_switches... checksum
			// 80F01039FF A21011315258400673FACB842B83FEA800000060CED4FDB060000F200000000000DC0000551E30C0F222000040FB00E10000000000000000 59
			ParamChecker.CheckNotNullOrEmpty(processedResponse, "processedResponse");
			SSMResponseProcessor.ValidateResponse(processedResponse);
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
			//  80 10 F0 05 B8 00 00 60 40 DD
			ParamChecker.CheckGreaterThanZero(id, "ECU_ID");
			SSMProtocol.ECU_ID = id;
			byte[] resetAddress = new byte[] { unchecked((byte)unchecked((int)(0x00))), unchecked(
				(byte)unchecked((int)(0x00))), unchecked((byte)unchecked((int)(0x60))) };
			byte reset = unchecked((int)(0x40));
			return ConstructWriteAddressRequest(resetAddress, reset);
		}

		public void CheckValidEcuResetResponse(byte[] processedResponse)
		{
			// 80 F0 10 02 F8 40 BA
			ParamChecker.CheckNotNullOrEmpty(processedResponse, "processedResponse");
			SSMResponseProcessor.ValidateResponse(processedResponse);
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
			return new _ConnectionProperties_150();
		}

		private sealed class _ConnectionProperties_150 : ConnectionProperties
		{
			public _ConnectionProperties_150()
			{
			}

			public int GetBaudRate()
			{
				return 4800;
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
			bb.Write(HEADER);
			bb.Write(ECU_ID);
			bb.Write(DIAGNOSTIC_TOOL_ID);
			bb.Write(Sharpen.Extensions.ValueOf(length + (padContent ? 2 : 1)));
			bb.Write(command);
			if (padContent)
			{
				bb.Write((pollState.IsFastPoll() ? READ_ADDRESS_CONTINUOUS : READ_ADDRESS_ONCE));
			}
			foreach (byte[] tmp_1 in content)
			{
				try
				{
					bb.Write(tmp_1);
				}
				catch (IOException e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
			bb.Write(unchecked((byte)unchecked((int)(0x00))));
			byte[] request = bb.ToByteArray();
			byte cs = SSMChecksumCalculator.CalculateChecksum(request);
			request[request.Length - 1] = cs;
			return request;
		}

		private byte[] BuildRequest(byte command, bool padContent, IList<byte[]> content)
		{
			int length = 0;
			foreach (byte[] tmp in content)
			{
				length += tmp.Length;
			}
			ByteArrayOutputStream bb = new ByteArrayOutputStream(length);
			bb.Write(HEADER);
			bb.Write(ECU_ID);
			bb.Write(DIAGNOSTIC_TOOL_ID);
			bb.Write(Sharpen.Extensions.ValueOf(length + (padContent ? 2 : 1)));
			bb.Write(command);
			if (padContent)
			{
				bb.Write((pollState.IsFastPoll() ? READ_ADDRESS_CONTINUOUS : READ_ADDRESS_ONCE));
			}
			foreach (byte[] tmp_1 in content)
			{
				try
				{
					bb.Write(tmp_1);
				}
				catch (IOException e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}
			bb.Write(unchecked((byte)unchecked((int)(0x00))));
			byte[] request = bb.ToByteArray();
			byte cs = SSMChecksumCalculator.CalculateChecksum(request);
			request[request.Length - 1] = cs;
			return request;
		}
	}
}
