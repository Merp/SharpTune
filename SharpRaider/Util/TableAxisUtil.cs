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

using RomRaider.Maps;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Util
{
	public sealed class TableAxisUtil
	{
		public TableAxisUtil()
		{
		}

		public static AxisRange GetLiveDataRangeForAxis(Table1D axis)
		{
			int startIdx = 0;
			int endIdx = 0;
			double liveAxisValue = axis.GetLiveValue();
			DataCell[] data = axis.GetData();
			for (int i = 0; i < data.Length; i++)
			{
				DataCell cell = data[i];
				double axisValue = cell.GetValue();
				if (liveAxisValue == axisValue)
				{
					startIdx = i;
					endIdx = i;
					break;
				}
				else
				{
					if (liveAxisValue < axisValue)
					{
						startIdx = i - 1;
						endIdx = i;
						break;
					}
					else
					{
						startIdx = i;
						endIdx = i + 1;
					}
				}
			}
			if (startIdx < 0)
			{
				startIdx = 0;
			}
			if (startIdx >= data.Length)
			{
				startIdx = data.Length - 1;
			}
			if (endIdx < 0)
			{
				endIdx = 0;
			}
			if (endIdx >= data.Length)
			{
				endIdx = data.Length - 1;
			}
			return new AxisRange(startIdx, endIdx);
		}
	}
}
