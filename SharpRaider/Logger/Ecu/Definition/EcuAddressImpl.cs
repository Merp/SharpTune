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
using System.Text;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Definition
{
	public sealed class EcuAddressImpl : EcuAddress
	{
		private readonly string[] addresses;

		private readonly byte[] bytes;

		private readonly int bit;

		public EcuAddressImpl(string address, int length, int bit)
		{
			ParamChecker.CheckNotNullOrEmpty(address, "address");
			ParamChecker.CheckGreaterThanZero(length, "length");
			this.addresses = BuildAddresses(address, length);
			this.bytes = GetAddressBytes(addresses);
			this.bit = bit;
		}

		public EcuAddressImpl(string[] addresses)
		{
			ParamChecker.CheckNotNullOrEmpty(addresses, "addresses");
			this.addresses = addresses;
			this.bytes = GetAddressBytes(addresses);
			this.bit = -1;
		}

		public string[] GetAddresses()
		{
			return addresses;
		}

		public byte[] GetBytes()
		{
			return bytes;
		}

		public int GetBit()
		{
			return bit;
		}

		public int GetLength()
		{
			return addresses.Length;
		}

		private string[] BuildAddresses(string startAddress, int addressLength)
		{
			IList<string> addresses = new List<string>();
			int start = HexUtil.HexToInt(startAddress);
			for (int i = 0; i < addressLength; i++)
			{
				addresses.AddItem(PadAddress(HexUtil.IntToHexString(start + i), startAddress.Length
					));
			}
			return Sharpen.Collections.ToArray(addresses, new string[addresses.Count]);
		}

		private string PadAddress(string address, int length)
		{
			if (address.Length == length)
			{
				return address;
			}
			else
			{
				StringBuilder builder = new StringBuilder(length);
				builder.Append("0x");
				string s = Sharpen.Runtime.Substring(address, 2);
				for (int i = 0; i < length - s.Length - 2; i++)
				{
					builder.Append('0');
				}
				builder.Append(s);
				return builder.ToString();
			}
		}

		private byte[] GetAddressBytes(string[] addresses)
		{
			byte[] bytes = new byte[0];
			foreach (string address in addresses)
			{
				byte[] tmp1 = HexUtil.AsBytes(address);
				byte[] tmp2 = new byte[bytes.Length + tmp1.Length];
				System.Array.Copy(bytes, 0, tmp2, 0, bytes.Length);
				System.Array.Copy(tmp1, 0, tmp2, bytes.Length, tmp1.Length);
				bytes = tmp2;
			}
			return bytes;
		}
	}
}
