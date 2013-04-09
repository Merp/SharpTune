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
using RomRaider.Logger.Ecu.UI.Handler.Dash;
using RomRaider.Logger.External.Core;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Definition
{
	public sealed class ExternalDataConvertorImpl : EcuDataConvertor
	{
		private readonly string units;

		private readonly string expression;

		private readonly GaugeMinMax gaugeMinMax;

		private readonly ExternalDataItem dataItem;

		private DecimalFormat format;

		public ExternalDataConvertorImpl(ExternalDataItem dataItem, string units, string 
			expression, string format, GaugeMinMax gaugeMinMax)
		{
			this.dataItem = dataItem;
			this.units = units;
			this.expression = expression;
			this.format = new DecimalFormat(format);
			this.gaugeMinMax = gaugeMinMax;
		}

		public double Convert(byte[] bytes)
		{
			double value = dataItem.GetData();
			double result = JEPUtil.Evaluate(expression, value);
			return double.IsNaN(result) || double.IsInfinite(result) ? 0.0 : result;
		}

		public string Format(double value)
		{
			return format.Format(value);
		}

		public string GetUnits()
		{
			return units;
		}

		public GaugeMinMax GetGaugeMinMax()
		{
			return gaugeMinMax;
		}

		public string GetFormat()
		{
			return format.ToPattern();
		}

		public override string ToString()
		{
			return GetUnits();
		}
	}
}
