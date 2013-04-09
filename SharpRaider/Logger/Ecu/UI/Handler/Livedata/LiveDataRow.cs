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

using RomRaider.Logger.Ecu.Definition;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Handler.Livedata
{
	public sealed class LiveDataRow
	{
		private const double ZERO = 0.0;

		private readonly LoggerData loggerData;

		private double minValue;

		private double maxValue;

		private double currentValue;

		private bool updated = false;

		public LiveDataRow(LoggerData loggerData)
		{
			ParamChecker.CheckNotNull(loggerData, "loggerData");
			this.loggerData = loggerData;
		}

		public LoggerData GetLoggerData()
		{
			return loggerData;
		}

		public string GetName()
		{
			return loggerData.GetName();
		}

		public string GetMinValue()
		{
			return loggerData.GetSelectedConvertor().Format(minValue);
		}

		public string GetMaxValue()
		{
			return loggerData.GetSelectedConvertor().Format(maxValue);
		}

		public string GetCurrentValue()
		{
			return loggerData.GetSelectedConvertor().Format(currentValue);
		}

		public string GetUnits()
		{
			return loggerData.GetSelectedConvertor().GetUnits();
		}

		public void UpdateValue(double value)
		{
			currentValue = value;
			if (currentValue < minValue || !updated)
			{
				minValue = currentValue;
			}
			if (currentValue > maxValue || !updated)
			{
				maxValue = currentValue;
			}
			updated = true;
		}

		public void Reset()
		{
			minValue = ZERO;
			maxValue = ZERO;
			currentValue = ZERO;
			updated = false;
		}
	}
}
