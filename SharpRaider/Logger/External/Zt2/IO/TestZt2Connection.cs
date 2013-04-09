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
using RomRaider.Logger.External.Zt2.IO;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Zt2.IO
{
	public sealed class TestZt2Connection : SerialConnection
	{
		private readonly byte[] data = new byte[] { unchecked((byte)unchecked((int)(0x00)
			)), unchecked((int)(0x01)), unchecked((int)(0x02)), unchecked((byte)unchecked((int
			)(0x89))), unchecked((int)(0x00)), unchecked((int)(0x04)), unchecked((int)(0x00)
			), unchecked((int)(0x01)), unchecked((int)(0x00)), unchecked((int)(0x00)), unchecked(
			(int)(0x0f)), unchecked((int)(0x40)), unchecked((int)(0x00)), unchecked((int)(0x00
			)) };

		private int index;

		private byte[] result = new byte[1];

		public void Write(byte[] bytes)
		{
			throw new NotSupportedException();
		}

		public int Available()
		{
			return 1;
		}

		public void Read(byte[] bytes)
		{
			if (bytes.Length != 1)
			{
				throw new ArgumentException();
			}
			if (index >= data.Length)
			{
				index = 0;
			}
			bytes[0] = data[index++];
			ThreadUtil.Sleep(10);
		}

		public byte[] ReadAvailable()
		{
			throw new NotSupportedException();
		}

		public void ReadStaleData()
		{
			throw new NotSupportedException();
		}

		public string ReadLine()
		{
			throw new NotSupportedException();
		}

		public int Read()
		{
			Read(result);
			return result[0];
		}

		public void Close()
		{
			index = 0;
		}

		public void SendBreak(int duration)
		{
		}
	}
}
