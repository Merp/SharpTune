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
using RomRaider.Maps;
using Sharpen;

namespace RomRaider.Xml
{
	public sealed class RomAttributeParser
	{
		public RomAttributeParser()
		{
		}

		// Parses attributes from ROM XML
		public static int ParseEndian(string input)
		{
			if (Sharpen.Runtime.EqualsIgnoreCase(input, "big") || Sharpen.Runtime.EqualsIgnoreCase
				(input, Table.ENDIAN_BIG.ToString()))
			{
				return Table.ENDIAN_BIG;
			}
			else
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(input, "little") || Sharpen.Runtime.EqualsIgnoreCase
					(input, Table.ENDIAN_LITTLE.ToString()))
				{
					return Table.ENDIAN_LITTLE;
				}
				else
				{
					return Table.ENDIAN_LITTLE;
				}
			}
		}

		public static int ParseHexString(string input)
		{
			if (input.Equals("0"))
			{
				return 0;
			}
			else
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
		}

		public static int ParseStorageType(string input)
		{
			if (Sharpen.Runtime.EqualsIgnoreCase(input, "float"))
			{
				return Table.STORAGE_TYPE_FLOAT;
			}
			else
			{
				if (input.StartsWith("uint"))
				{
					return System.Convert.ToInt32(Sharpen.Runtime.Substring(input, 4)) / 8;
				}
				else
				{
					if (input.StartsWith("int"))
					{
						return System.Convert.ToInt32(Sharpen.Runtime.Substring(input, 3)) / 8;
					}
					else
					{
						return System.Convert.ToInt32(input);
					}
				}
			}
		}

		public static bool ParseStorageDataSign(string input)
		{
			if (input.StartsWith("int"))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static int ParseScaleType(string input)
		{
			if (Sharpen.Runtime.EqualsIgnoreCase(input, "inverse"))
			{
				return Scale.INVERSE;
			}
			else
			{
				return Scale.LINEAR;
			}
		}

		public static int ParseTableType(string input)
		{
			if (Sharpen.Runtime.EqualsIgnoreCase(input, "3D") || Sharpen.Runtime.EqualsIgnoreCase
				(input, Table.TABLE_3D.ToString()))
			{
				return Table.TABLE_3D;
			}
			else
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(input, "2D") || Sharpen.Runtime.EqualsIgnoreCase
					(input, Table.TABLE_2D.ToString()))
				{
					return Table.TABLE_2D;
				}
				else
				{
					if (Sharpen.Runtime.EqualsIgnoreCase(input, "X Axis") || Sharpen.Runtime.EqualsIgnoreCase
						(input, "Static X Axis") || Sharpen.Runtime.EqualsIgnoreCase(input, Table.TABLE_X_AXIS
						.ToString()))
					{
						return Table.TABLE_X_AXIS;
					}
					else
					{
						if (Sharpen.Runtime.EqualsIgnoreCase(input, "Y Axis") || Sharpen.Runtime.EqualsIgnoreCase
							(input, "Static Y Axis") || Sharpen.Runtime.EqualsIgnoreCase(input, Table.TABLE_Y_AXIS
							.ToString()))
						{
							return Table.TABLE_Y_AXIS;
						}
						else
						{
							return Table.TABLE_1D;
						}
					}
				}
			}
		}

		/// <exception cref="System.IndexOutOfRangeException"></exception>
		public static long ParseByteValue(byte[] input, int endian, int address, int length
			, bool signed)
		{
			try
			{
				long output = 0L;
				ByteBuffer bb = ByteBuffer.Wrap(input, address, length);
				if (endian == Table.ENDIAN_LITTLE)
				{
					bb.Order(ByteOrder.LITTLE_ENDIAN);
				}
				switch (length)
				{
					case 1:
					{
						output = bb.Get();
						break;
					}

					case 2:
					{
						output = bb.GetShort();
						break;
					}

					case 4:
					{
						output = bb.GetInt();
						break;
					}
				}
				if (!signed)
				{
					switch (length)
					{
						case 1:
						{
							output = output & unchecked((int)(0xff));
							break;
						}

						case 2:
						{
							output = output & unchecked((int)(0xffff));
							break;
						}

						case 4:
						{
							output = output & unchecked((long)(0xffffffffL));
							break;
						}
					}
				}
				return output;
			}
			catch (IndexOutOfRangeException)
			{
				throw new IndexOutOfRangeException();
			}
		}

		public static byte[] ParseIntegerValue(int input, int endian, int length)
		{
			try
			{
				ByteBuffer bb = ByteBuffer.Allocate(length);
				if (endian == Table.ENDIAN_LITTLE)
				{
					bb.Order(ByteOrder.LITTLE_ENDIAN);
				}
				switch (length)
				{
					case 1:
					{
						bb.Put(unchecked((byte)input));
						break;
					}

					case 2:
					{
						bb.PutShort((short)input);
						break;
					}

					case 4:
					{
						bb.PutInt(input);
						break;
					}
				}
				return ((byte[])bb.Array());
			}
			catch (BufferOverflowException)
			{
				throw new BufferOverflowException();
			}
		}

		/// <exception cref="System.FormatException"></exception>
		public static int ParseFileSize(string input)
		{
			try
			{
				return System.Convert.ToInt32(input);
			}
			catch (FormatException)
			{
				if (Sharpen.Runtime.EqualsIgnoreCase(Sharpen.Runtime.Substring(input, input.Length
					 - 2), "kb"))
				{
					return System.Convert.ToInt32(Sharpen.Runtime.Substring(input, 0, input.Length - 
						2)) * 1024;
				}
				else
				{
					if (Sharpen.Runtime.EqualsIgnoreCase(Sharpen.Runtime.Substring(input, input.Length
						 - 2), "mb"))
					{
						return System.Convert.ToInt32(Sharpen.Runtime.Substring(input, 0, input.Length - 
							2)) * 1024 * 1024;
					}
				}
				throw new FormatException();
			}
		}

		public static byte[] FloatToByte(float input, int endian)
		{
			byte[] output = new byte[4];
			ByteBuffer bb = ByteBuffer.Wrap(output, 0, 4);
			if (endian == Table.ENDIAN_LITTLE)
			{
				bb.Order(ByteOrder.BIG_ENDIAN);
			}
			bb.PutFloat(input);
			return ((byte[])bb.Array());
		}

		public static float ByteToFloat(byte[] input, int endian)
		{
			ByteBuffer bb = ByteBuffer.Wrap(input, 0, 4);
			if (endian == Table.ENDIAN_LITTLE)
			{
				bb.Order(ByteOrder.BIG_ENDIAN);
			}
			return bb.GetFloat();
		}
	}
}
