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
using RomRaider.IO.Serial.Connection;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Innovate.Generic.Serial.IO
{
	public sealed class TestInnovateConnection : SerialConnection
	{
		private readonly byte[] source;

		private int i;

		public TestInnovateConnection(string hex)
		{
			this.source = HexUtil.AsBytes(hex);
		}

		public void Write(byte[] bytes)
		{
			throw new NotSupportedException();
		}

		public int Available()
		{
			return source.Length;
		}

		public void Read(byte[] bytes)
		{
			for (int j = 0; j < bytes.Length; j++)
			{
				bytes[j] = source[(i + j) % source.Length];
			}
			i = (i + bytes.Length);
			if (i >= source.Length)
			{
				i %= source.Length;
			}
		}

		public byte[] ReadAvailable()
		{
			byte[] result = new byte[Available()];
			Read(result);
			return result;
		}

		public void ReadStaleData()
		{
			throw new NotSupportedException();
		}

		public void Close()
		{
		}

		public string ReadLine()
		{
			throw new NotSupportedException();
		}

		public int Read()
		{
			byte[] result = new byte[1];
			Read(result);
			return result[0];
		}

		public void SendBreak(int duration)
		{
		}
	}
}
