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
using RomRaider.Logger.External.Plx.IO;
using RomRaider.Logger.External.Plx.Plugin;
using Sharpen;

namespace RomRaider.Logger.External.Plx.IO
{
	public sealed class PlxParserImpl : PlxParser
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(PlxParserImpl));

		private PlxParserImpl.ParserState state = PlxParserImpl.ParserState.EXPECTING_START;

		private PlxSensorType sensorType;

		private int partialValue;

		private byte instance;

		public static PlxSensorType ValueOf(int value)
		{
			foreach (PlxSensorType type in PlxSensorType.Values())
			{
				if (type.value == value)
				{
					return type;
				}
			}
			throw new ArgumentException("Unknown PLX Sensor Type: " + value);
		}

		public PlxResponse PushByte(byte b)
		{
			if (b == unchecked((byte)unchecked((int)(0x80))))
			{
				state = PlxParserImpl.ParserState.EXPECTING_FIRST_HALF_OF_SENSOR_TYPE;
				return null;
			}
			if (b == unchecked((int)(0x40)))
			{
				state = PlxParserImpl.ParserState.EXPECTING_START;
				return null;
			}
			switch (state)
			{
				case PlxParserImpl.ParserState.EXPECTING_FIRST_HALF_OF_SENSOR_TYPE:
				{
					state = PlxParserImpl.ParserState.EXPECTING_SECOND_HALF_OF_SENSOR_TYPE;
					partialValue = b;
					break;
				}

				case PlxParserImpl.ParserState.EXPECTING_SECOND_HALF_OF_SENSOR_TYPE:
				{
					state = PlxParserImpl.ParserState.EXPECTING_INSTANCE;
					int value = (partialValue << 6) | b;
					sensorType = ValueOf(value);
					break;
				}

				case PlxParserImpl.ParserState.EXPECTING_INSTANCE:
				{
					state = PlxParserImpl.ParserState.EXPECTING_FIRST_HALF_OF_VALUE;
					instance = b;
					break;
				}

				case PlxParserImpl.ParserState.EXPECTING_FIRST_HALF_OF_VALUE:
				{
					state = PlxParserImpl.ParserState.EXPECTING_SECOND_HALF_OF_VALUE;
					partialValue = b;
					break;
				}

				case PlxParserImpl.ParserState.EXPECTING_SECOND_HALF_OF_VALUE:
				{
					state = PlxParserImpl.ParserState.EXPECTING_FIRST_HALF_OF_SENSOR_TYPE;
					int rawValue = (partialValue << 6) | b;
					LOGGER.Trace("PLX sensor: " + sensorType + " instance: " + instance + " value: " 
						+ rawValue);
					return new PlxResponse(sensorType, instance, rawValue);
				}
			}
			return null;
		}

		internal enum ParserState
		{
			EXPECTING_START,
			EXPECTING_FIRST_HALF_OF_SENSOR_TYPE,
			EXPECTING_SECOND_HALF_OF_SENSOR_TYPE,
			EXPECTING_INSTANCE,
			EXPECTING_FIRST_HALF_OF_VALUE,
			EXPECTING_SECOND_HALF_OF_VALUE
		}
	}
}
