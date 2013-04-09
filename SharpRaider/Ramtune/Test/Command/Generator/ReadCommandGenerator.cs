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
using Mono.Math;
using RomRaider.Ramtune.Test.Command.Generator;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Ramtune.Test.Command.Generator
{
	public sealed class ReadCommandGenerator : AbstractCommandGenerator
	{
		private const int INCREMENT_SIZE = 128;

		public ReadCommandGenerator(RomRaider.IO.Protocol.Protocol protocol) : base(protocol
			)
		{
		}

		public override IList<byte[]> CreateCommands(byte id, byte[] data, byte[] address
			, int length)
		{
			ParamChecker.CheckGreaterThanZero(id, "Target ID");
			ParamChecker.CheckNotNullOrEmpty(address, "address");
			ParamChecker.CheckGreaterThanZero(length, "length");
			if (length == 1)
			{
				return Arrays.AsList(CreateCommandForAddress(id, address));
			}
			else
			{
				return CreateCommandsForRange(id, address, length);
			}
		}

		private byte[] CreateCommandForAddress(byte id, byte[] address)
		{
			IList<byte[]> tl = new AList<byte[]>();
			tl.AddItem(address);
			return protocol.ConstructReadAddressRequest(id, tl);
		}

		private IList<byte[]> CreateCommandsForRange(byte id, byte[] address, int length)
		{
			IList<byte[]> commands = new AList<byte[]>();
			byte[] readAddress = Copy(address);
			int i = 0;
			while (i < length)
			{
				int readLength = (length - i) > INCREMENT_SIZE ? INCREMENT_SIZE : length - i;
				if (readLength == 1)
				{
					commands.AddItem(CreateCommandForAddress(id, readAddress));
				}
				else
				{
					commands.AddItem(protocol.ConstructReadMemoryRequest(id, readAddress, readLength)
						);
				}
				i += INCREMENT_SIZE;
				System.Array.Copy(IncrementAddress(readAddress, readLength), 0, readAddress, 0, readAddress
					.Length);
			}
			return commands;
		}

		private byte[] Copy(byte[] bytes)
		{
			byte[] bytes2 = new byte[bytes.Length];
			System.Array.Copy(bytes, 0, bytes2, 0, bytes2.Length);
			return bytes2;
		}

		private byte[] IncrementAddress(byte[] address, int increment)
		{
			BigInteger currentAddr = new BigInteger(1, address);
			string strIncrement = increment.ToString();
			BigInteger bintIncrement = new BigInteger(strIncrement);
			BigInteger newAddress = currentAddr.Add(bintIncrement);
			byte[] incAddr = newAddress.GetBytes();
			if (incAddr.Length == 1)
			{
				address[0] = 0;
				address[1] = 0;
				address[2] = incAddr[0];
				return address;
			}
			if (incAddr.Length == 2)
			{
				address[0] = 0;
				address[1] = incAddr[0];
				address[2] = incAddr[1];
				return address;
			}
			if (incAddr.Length == 4)
			{
				System.Array.Copy(incAddr, 1, address, 0, 3);
				return address;
			}
			else
			{
				return incAddr;
			}
		}

		public override string ToString()
		{
			return "Read";
		}
	}
}
