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
using RomRaider.IO.Protocol.Ssm.Iso9141;
using RomRaider.IO.Serial.Connection;
using RomRaider.Logger.Ecu.Exception;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.IO.Serial.Connection
{
	internal sealed class TestSerialConnection : SerialConnection
	{
		private static readonly Logger LOGGER = Logger.GetLogger(typeof(RomRaider.IO.Serial.Connection.TestSerialConnection
			));

		private static readonly Random RANDOM = new Random(Runtime.CurrentTimeMillis());

		private static readonly string ECU_INIT_RESPONSE_01_UP = "8010F001BF4080F01039FFA21011315258400673FACB842B83FEA800000060CED4FDB060000F200000000000DC0000551E30C0F222000040FB00E1000000000000000059";

		private static readonly string ECU_INIT_RESPONSE_PRE_01 = "8010F001BF4080F01029FFA1100B195458050561C4EB800808000000000070CE64F8BA080000E00000000000DC0000108000007B";

		private static readonly string ECU_INIT_RESPONSE_OBXT = "8010F001BF4080F01039FFA210112F1279560673FACBA62B81FEA800000060CE54F9B1E4000C200000000000DC00005D1F30C0F226000043FB00E1000000000000C1F02D";

		private static readonly string ECU_INIT_RESPONSE_TCU = "8018F001BF4080F01839FFA6102291E02074000100800400000000A1462C000800000000000000DE06000B29C0047E011E003E00000000000080A20000FEFE0000000012";

		private static readonly string ECU_INIT_RESPONSE = ECU_INIT_RESPONSE_OBXT;

		private byte[] request = new byte[0];

		public TestSerialConnection(string portName, ConnectionProperties connectionProperties
			)
		{
			//    private static final String ECU_INIT_RESPONSE = ECU_INIT_RESPONSE_PRE_01;
			ParamChecker.CheckNotNullOrEmpty(portName, "portName");
			ParamChecker.CheckNotNull(connectionProperties, "connectionProperties");
			LOGGER.Info("*** TEST *** Opening connection: " + portName);
		}

		public void Write(byte[] bytes)
		{
			//LOGGER.("*** TEST *** Write bytes = " + asHex(bytes));
			request = bytes;
		}

		public int Available()
		{
			if (IsEcuInitRequest())
			{
				string init = string.Empty;
				if (SSMProtocol.ECU_ID == unchecked((int)(0x10)))
				{
					init = ECU_INIT_RESPONSE;
				}
				if (SSMProtocol.ECU_ID == unchecked((int)(0x18)))
				{
					init = ECU_INIT_RESPONSE_TCU;
				}
				return HexUtil.AsBytes(init).Length;
			}
			else
			{
				if (IsReadAddressRequest())
				{
					return request.Length + (SSMProtocol.RESPONSE_NON_DATA_BYTES + CalculateNumResponseDataBytes
						());
				}
				else
				{
					if (IsReadMemoryRequest())
					{
						return request.Length + (SSMProtocol.RESPONSE_NON_DATA_BYTES + ByteUtil.AsInt(request
							[9]) + 1);
					}
					else
					{
						if (IsWriteMemoryRequest())
						{
							return request.Length + (SSMProtocol.RESPONSE_NON_DATA_BYTES + (request.Length - 
								6 - SSMProtocol.ADDRESS_SIZE));
						}
						else
						{
							throw new SerialCommunicationException("*** TEST *** Unsupported request: " + HexUtil.AsHex
								(request));
						}
					}
				}
			}
		}

		public void Read(byte[] bytes)
		{
			if (IsEcuInitRequest())
			{
				if (SSMProtocol.ECU_ID == unchecked((int)(0x10)))
				{
					System.Array.Copy(HexUtil.AsBytes(ECU_INIT_RESPONSE), 0, bytes, 0, bytes.Length);
				}
				if (SSMProtocol.ECU_ID == unchecked((int)(0x18)))
				{
					System.Array.Copy(HexUtil.AsBytes(ECU_INIT_RESPONSE_TCU), 0, bytes, 0, bytes.Length
						);
				}
			}
			else
			{
				if (IsIamRequest())
				{
					byte[] response = HexUtil.AsBytes("0x80F01006E83F600000000D");
					System.Array.Copy(response, 0, bytes, request.Length, response.Length);
				}
				else
				{
					if (IsEngineLoadRequest())
					{
						byte[] response = HexUtil.AsBytes("0x80F01006E83EC74A760033");
						System.Array.Copy(response, 0, bytes, request.Length, response.Length);
					}
					else
					{
						if (IsReadAddressRequest())
						{
							byte[] responseData = GenerateResponseData(CalculateNumResponseDataBytes());
							int i = 0;
							byte[] response = new byte[SSMProtocol.RESPONSE_NON_DATA_BYTES + CalculateNumResponseDataBytes
								()];
							response[i++] = SSMProtocol.HEADER;
							response[i++] = SSMProtocol.DIAGNOSTIC_TOOL_ID;
							response[i++] = SSMProtocol.ECU_ID;
							response[i++] = unchecked((byte)(1 + responseData.Length));
							response[i++] = SSMProtocol.READ_ADDRESS_RESPONSE;
							System.Array.Copy(responseData, 0, response, i, responseData.Length);
							response[i += responseData.Length] = SSMChecksumCalculator.CalculateChecksum(response
								);
							System.Array.Copy(request, 0, bytes, 0, request.Length);
							System.Array.Copy(response, 0, bytes, request.Length, response.Length);
						}
						else
						{
							if (IsReadMemoryRequest())
							{
								byte[] responseData = GenerateResponseData(ByteUtil.AsInt(request[9]) + 1);
								int i = 0;
								byte[] response = new byte[SSMProtocol.RESPONSE_NON_DATA_BYTES + responseData.Length
									];
								response[i++] = SSMProtocol.HEADER;
								response[i++] = SSMProtocol.DIAGNOSTIC_TOOL_ID;
								response[i++] = SSMProtocol.ECU_ID;
								response[i++] = unchecked((byte)(1 + responseData.Length));
								response[i++] = SSMProtocol.READ_MEMORY_RESPONSE;
								System.Array.Copy(responseData, 0, response, i, responseData.Length);
								response[i += responseData.Length] = SSMChecksumCalculator.CalculateChecksum(response
									);
								System.Array.Copy(request, 0, bytes, 0, request.Length);
								System.Array.Copy(response, 0, bytes, request.Length, response.Length);
							}
							else
							{
								if (IsWriteMemoryRequest())
								{
									int numDataBytes = request.Length - 6 - SSMProtocol.ADDRESS_SIZE;
									byte[] response = new byte[SSMProtocol.RESPONSE_NON_DATA_BYTES + numDataBytes];
									int i = 0;
									response[i++] = SSMProtocol.HEADER;
									response[i++] = SSMProtocol.DIAGNOSTIC_TOOL_ID;
									response[i++] = SSMProtocol.ECU_ID;
									response[i++] = unchecked((byte)(numDataBytes + 1));
									response[i++] = SSMProtocol.WRITE_MEMORY_RESPONSE;
									System.Array.Copy(request, 8, response, i, numDataBytes);
									response[i += numDataBytes] = SSMChecksumCalculator.CalculateChecksum(response);
									System.Array.Copy(request, 0, bytes, 0, request.Length);
									System.Array.Copy(response, 0, bytes, request.Length, response.Length);
								}
								else
								{
									throw new SerialCommunicationException("*** TEST *** Unsupported request: " + HexUtil.AsHex
										(request));
								}
							}
						}
					}
				}
			}
			//LOGGER.("*** TEST *** Read bytes  = " + asHex(bytes));
			ThreadUtil.Sleep(200);
		}

		public byte[] ReadAvailable()
		{
			byte[] response = new byte[Available()];
			Read(response);
			return response;
		}

		public void ReadStaleData()
		{
		}

		public void Close()
		{
			LOGGER.Info("*** TEST *** Connection closed.");
		}

		public string ReadLine()
		{
			throw new NotSupportedException();
		}

		public int Read()
		{
			throw new NotSupportedException();
		}

		public void SendBreak(int duration)
		{
		}

		private int CalculateNumResponseDataBytes()
		{
			return ((request.Length - SSMProtocol.REQUEST_NON_DATA_BYTES) / SSMProtocol.ADDRESS_SIZE
				) * SSMProtocol.DATA_SIZE;
		}

		private bool IsIamRequest()
		{
			string hex = HexUtil.AsHex(request);
			return hex.StartsWith("8010F011A8") && hex.Contains("FF8228FF8229FF822AFF822B");
		}

		private bool IsEngineLoadRequest()
		{
			string hex = HexUtil.AsHex(request);
			return hex.StartsWith("8010F011A8") && hex.Contains("FFA6FCFFA6FDFFA6FEFFA6FF");
		}

		private byte[] GenerateResponseData(int dataLength)
		{
			byte[] responseData = new byte[dataLength];
			for (int i = 0; i < responseData.Length; i++)
			{
				responseData[i] = unchecked((byte)RANDOM.Next(255));
			}
			return responseData;
		}

		private bool IsEcuInitRequest()
		{
			byte command = SSMProtocol.ECU_INIT_COMMAND;
			return IsCommand(command);
		}

		private bool IsReadAddressRequest()
		{
			return IsCommand(SSMProtocol.READ_ADDRESS_COMMAND);
		}

		private bool IsReadMemoryRequest()
		{
			return IsCommand(SSMProtocol.READ_MEMORY_COMMAND);
		}

		private bool IsWriteMemoryRequest()
		{
			return IsCommand(SSMProtocol.WRITE_MEMORY_COMMAND);
		}

		private bool IsCommand(byte command)
		{
			return request[4] == command;
		}
	}
}
