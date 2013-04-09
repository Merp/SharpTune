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
using RomRaider.Logger.Ecu.Comms.Query;
using RomRaider.Logger.Ecu.Definition;
using Sharpen;

namespace RomRaider.Logger.Ecu.Comms.Query
{
	public class ResponseImpl : Response
	{
		private const double ZERO = 0.0;

		private readonly IDictionary<LoggerData, double> dataValues = new LinkedHashMap<LoggerData
			, double>();

		private readonly long timestamp;

		public ResponseImpl()
		{
			timestamp = Runtime.CurrentTimeMillis();
		}

		public virtual void SetDataValue(LoggerData data, double value)
		{
			dataValues.Put(data, value);
		}

		public virtual ICollection<LoggerData> GetData()
		{
			return dataValues.Keys;
		}

		public virtual double GetDataValue(LoggerData data)
		{
			double value = dataValues.Get(data);
			return value == null ? ZERO : value;
		}

		public virtual long GetTimestamp()
		{
			return timestamp;
		}
	}
}
