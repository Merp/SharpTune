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
using System.Collections.Generic;
using RomRaider.IO.Protocol.Ssm.Iso9141;
using RomRaider.Logger.Ecu.Comms.IO.Protocol;
using RomRaider.Logger.Ecu.Comms.Manager;
using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.IO.Protocol.Ssm.Iso9141
{
	public sealed class SSMLoggerProtocol : LoggerProtocol
	{
		private readonly RomRaider.IO.Protocol.Protocol protocol = new SSMProtocol();

		//import static com.romraider.io.protocol.ssm.iso15765.SSMProtocol.ADDRESS_SIZE;
		public byte[] ConstructEcuInitRequest(byte id)
		{
			return protocol.ConstructEcuInitRequest(id);
		}

		public byte[] ConstructEcuResetRequest(byte id)
		{
			return protocol.ConstructEcuResetRequest(id);
		}

		public byte[] ConstructReadAddressRequest(byte id, ICollection<EcuQuery> queries)
		{
			ICollection<EcuQuery> filteredQueries = FilterDuplicates(queries);
			return protocol.ConstructReadAddressRequest(id, ConvertToByteAddresses(filteredQueries
				));
		}

		public byte[] ConstructReadAddressResponse(ICollection<EcuQuery> queries, PollingState
			 pollState)
		{
			ParamChecker.CheckNotNullOrEmpty(queries, "queries");
			ParamChecker.CheckNotNull(pollState, "pollState");
			// 0x80 0xF0 0x10 data_length 0xE8 value1 value2 ... valueN checksum
			ICollection<EcuQuery> filteredQueries = FilterDuplicates(queries);
			int numAddresses = 0;
			foreach (EcuQuery ecuQuery in filteredQueries)
			{
				numAddresses += (ecuQuery.GetBytes().Length / SSMProtocol.ADDRESS_SIZE);
			}
			switch (pollState.GetCurrentState())
			{
				case 0:
				{
					return new byte[(numAddresses * SSMProtocol.DATA_SIZE + SSMProtocol.RESPONSE_NON_DATA_BYTES
						) + (numAddresses * SSMProtocol.ADDRESS_SIZE + SSMProtocol.REQUEST_NON_DATA_BYTES
						)];
				}

				case 1:
				{
					return new byte[(numAddresses * SSMProtocol.DATA_SIZE + SSMProtocol.RESPONSE_NON_DATA_BYTES
						)];
				}

				default:
				{
					throw new NotSupportedException("Poll mode not supported:" + pollState.GetCurrentState
						());
				}
			}
		}

		public byte[] PreprocessResponse(byte[] request, byte[] response, PollingState pollState
			)
		{
			return SSMResponseProcessor.FilterRequestFromResponse(request, response, pollState
				);
		}

		public void ProcessEcuInitResponse(EcuInitCallback callback, byte[] response)
		{
			ParamChecker.CheckNotNull(callback, "callback");
			ParamChecker.CheckNotNullOrEmpty(response, "response");
			protocol.CheckValidEcuInitResponse(response);
			EcuInit ecuInit = protocol.ParseEcuInitResponse(response);
			callback.Callback(ecuInit);
		}

		public void ProcessEcuResetResponse(byte[] response)
		{
			ParamChecker.CheckNotNullOrEmpty(response, "response");
			protocol.CheckValidEcuResetResponse(response);
		}

		// processes the response bytes and sets individual responses on corresponding query objects
		public void ProcessReadAddressResponses(ICollection<EcuQuery> queries, byte[] response
			, PollingState pollState)
		{
			ParamChecker.CheckNotNullOrEmpty(queries, "queries");
			ParamChecker.CheckNotNullOrEmpty(response, "response");
			byte[] responseData = SSMResponseProcessor.ExtractResponseData(response);
			ICollection<EcuQuery> filteredQueries = FilterDuplicates(queries);
			IDictionary<string, byte[]> addressResults = new Dictionary<string, byte[]>();
			int i = 0;
			foreach (EcuQuery filteredQuery in filteredQueries)
			{
				byte[] bytes = new byte[SSMProtocol.DATA_SIZE * (filteredQuery.GetBytes().Length 
					/ SSMProtocol.ADDRESS_SIZE)];
				System.Array.Copy(responseData, i, bytes, 0, bytes.Length);
				addressResults.Put(filteredQuery.GetHex(), bytes);
				i += bytes.Length;
			}
			foreach (EcuQuery query in queries)
			{
				query.SetResponse(addressResults.Get(query.GetHex()));
			}
		}

		public RomRaider.IO.Protocol.Protocol GetProtocol()
		{
			return protocol;
		}

		private ICollection<EcuQuery> FilterDuplicates(ICollection<EcuQuery> queries)
		{
			ICollection<EcuQuery> filteredQueries = new AList<EcuQuery>();
			foreach (EcuQuery query in queries)
			{
				if (!filteredQueries.Contains(query))
				{
					filteredQueries.AddItem(query);
				}
			}
			return filteredQueries;
		}

		private IList<byte[]> ConvertToByteAddresses(ICollection<EcuQuery> queries)
		{
			int byteCount = 0;
			foreach (EcuQuery query in queries)
			{
				byteCount += query.GetAddresses().Length;
			}
			IList<byte[]> addresses = new AList<byte[]>();
			byte[] tempBytes = new byte[byteCount];
			int i = 0;
			foreach (EcuQuery query_1 in queries)
			{
				byte[] bytes = query_1.GetBytes();
				for (int j = 0; j < bytes.Length / SSMProtocol.ADDRESS_SIZE; j++)
				{
					System.Array.Copy(bytes, j * SSMProtocol.ADDRESS_SIZE, tempBytes, 0, SSMProtocol.
						ADDRESS_SIZE);
					addresses.AddItem(tempBytes);
				}
			}
			return addresses;
		}
	}
}
