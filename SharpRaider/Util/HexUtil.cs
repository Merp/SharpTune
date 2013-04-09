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
using Sharpen;

namespace RomRaider.Util
{
	public sealed class HexUtil
	{
		public HexUtil()
		{
		}

		public static string AsHex(byte b)
		{
			return AsHex(new byte[] { b });
		}

		public static string AsHex(byte[] @in)
		{
			return BytesToHex(@in).ToUpper();
		}

		public static byte[] AsBytes(string hex)
		{
			if (hex.IndexOf(' ') >= 0)
			{
				hex = hex.ReplaceAll(" ", string.Empty);
			}
			if (hex.StartsWith("0x"))
			{
				hex = Sharpen.Runtime.Substring(hex, 2);
			}
			return HexToBytes(hex);
		}

		public static string BytesToHex(byte[] bs, int off, int length)
		{
			StringBuilder sb = new StringBuilder(length * 2);
			BytesToHexAppend(bs, off, length, sb);
			return sb.ToString();
		}

		public static void BytesToHexAppend(byte[] bs, int off, int length, StringBuilder
			 sb)
		{
			sb.EnsureCapacity(sb.Length + length * 2);
			for (int i = off; (i < (off + length)) && (i < bs.Length); i++)
			{
				sb.Append(char.ForDigit((bs[i] >> 4) & unchecked((int)(0xf)), 16));
				sb.Append(char.ForDigit(bs[i] & unchecked((int)(0xf)), 16));
			}
		}

		public static string BytesToHex(byte[] bs)
		{
			return BytesToHex(bs, 0, bs.Length);
		}

		public static byte[] HexToBytes(string s)
		{
			return HexToBytes(s, 0);
		}

		public static byte[] HexToBytes(string s, int off)
		{
			byte[] bs = new byte[off + (1 + s.Length) / 2];
			HexToBytes(s, bs, off);
			return bs;
		}

		/// <exception cref="System.FormatException"></exception>
		/// <exception cref="System.IndexOutOfRangeException"></exception>
		public static void HexToBytes(string s, byte[] @out, int off)
		{
			int slen = s.Length;
			if ((slen % 2) != 0)
			{
				s = '0' + s;
			}
			if (@out.Length < off + slen / 2)
			{
				throw new IndexOutOfRangeException("Output buffer too small for input (" + @out.Length
					 + "<" + off + slen / 2 + ")");
			}
			// Safe to assume the string is even length
			byte b1;
			byte b2;
			for (int i = 0; i < slen; i += 2)
			{
				b1 = unchecked((byte)char.Digit(s[i], 16));
				b2 = unchecked((byte)char.Digit(s[i + 1], 16));
				if ((((sbyte)b1) < 0) || (((sbyte)b2) < 0))
				{
					throw new FormatException();
				}
				@out[off + i / 2] = unchecked((byte)(b1 << 4 | b2));
			}
		}

		public static int HexToInt(string input)
		{
			if (input.Length > 2 && Sharpen.Runtime.EqualsIgnoreCase(Sharpen.Runtime.Substring
				(input, 0, 2), "0x"))
			{
				return System.Convert.ToInt32(Sharpen.Runtime.Substring(input, 2), 16);
			}
			else
			{
				return System.Convert.ToInt32(input, 16);
			}
		}

		public static string IntToHexString(int input)
		{
			return "0x" + Sharpen.Extensions.ToHexString(input).ToUpper();
		}
	}
}
