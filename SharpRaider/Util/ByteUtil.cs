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
using Sharpen;

namespace RomRaider.Util
{
	public sealed class ByteUtil
	{
		public ByteUtil()
		{
			throw new NotSupportedException();
		}

		public static int AsUnsignedInt(byte b)
		{
			return AsUnsignedInt(new byte[] { b });
		}

		public static int AsUnsignedInt(byte[] bytes)
		{
			int i = 0;
			for (int j = 0; j < bytes.Length; j++)
			{
				if (j > 0)
				{
					i <<= 8;
				}
				i |= bytes[j] & unchecked((int)(0xFF));
			}
			return i;
		}

		public static byte AsByte(int i)
		{
			return Sharpen.Extensions.ValueOf(i);
		}

		public static float AsFloat(byte[] b, int offset, int length)
		{
			ByteBuffer buf = ByteBuffer.Wrap(b, offset, length);
			return buf.GetFloat();
		}

		public static int AsInt(byte b)
		{
			return byte.ValueOf(b);
		}

		public static bool MatchOnes(byte b, int mask)
		{
			return (b & mask) == mask;
		}

		public static bool MatchZeroes(byte b, int mask)
		{
			return (b & mask) == 0;
		}

		public static void ByteListToBytes(IList<byte> buffer, byte[] response)
		{
			for (int i = 0; i < buffer.Count; i++)
			{
				response[i] = buffer[i];
			}
		}

		public static int IndexOfBytes(byte[] bytes, byte[] pattern)
		{
			int[] failure = ComputeFailure(pattern);
			int j = 0;
			for (int i = 0; i < bytes.Length; i++)
			{
				while (j > 0 && pattern[j] != bytes[i])
				{
					j = failure[j - 1];
				}
				if (pattern[j] == bytes[i])
				{
					j++;
				}
				if (j == pattern.Length)
				{
					return i - pattern.Length + 1;
				}
			}
			return -1;
		}

		private static int[] ComputeFailure(byte[] pattern)
		{
			int[] failure = new int[pattern.Length];
			int j = 0;
			for (int i = 1; i < pattern.Length; i++)
			{
				while (j > 0 && pattern[j] != pattern[i])
				{
					j = failure[j - 1];
				}
				if (pattern[j] == pattern[i])
				{
					j++;
				}
				failure[i] = j;
			}
			return failure;
		}
	}
}
