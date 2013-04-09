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
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.Definition.Xml;
using RomRaider.Logger.Ecu.UI.Handler.Dash;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.Definition
{
	public sealed class EcuParameterConvertorImpl : EcuDataConvertor
	{
		private readonly string units;

		private readonly string expression;

		private readonly DecimalFormat format;

		private readonly int bit;

		private readonly bool isFloat;

		private readonly IDictionary<string, string> replaceMap;

		private readonly GaugeMinMax gaugeMinMax;

		public EcuParameterConvertorImpl() : this("Raw data", "x", "0", -1, false, new Dictionary
			<string, string>(), ConverterMaxMinDefaults.GetDefault())
		{
		}

		public EcuParameterConvertorImpl(string units, string expression, string format, 
			int bit, bool isFloat, IDictionary<string, string> replaceMap, GaugeMinMax gaugeMinMax
			)
		{
			ParamChecker.CheckNotNullOrEmpty(units, "units");
			ParamChecker.CheckNotNullOrEmpty(expression, "expression");
			ParamChecker.CheckNotNullOrEmpty(format, "format");
			ParamChecker.CheckNotNull(replaceMap, "replaceMap");
			this.units = units;
			this.expression = expression;
			this.format = new DecimalFormat(format);
			this.bit = bit;
			this.isFloat = isFloat;
			this.replaceMap = replaceMap;
			this.gaugeMinMax = gaugeMinMax;
		}

		public double Convert(byte[] bytes)
		{
			if (ParamChecker.IsValidBit(bit))
			{
				return (bytes[0] & (1 << bit)) > 0 ? 1 : 0;
			}
			else
			{
				double value = (double)(isFloat ? Sharpen.Runtime.IntBitsToFloat(ByteUtil.AsUnsignedInt
					(bytes)) : ByteUtil.AsUnsignedInt(bytes));
				double result = JEPUtil.Evaluate(expression, value);
				return double.IsNaN(result) || double.IsInfinite(result) ? 0.0 : result;
			}
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

		public string Format(double value)
		{
			string formattedValue = format.Format(value);
			if (replaceMap.ContainsKey(formattedValue))
			{
				return replaceMap.Get(formattedValue);
			}
			else
			{
				return formattedValue;
			}
		}

		public override string ToString()
		{
			return GetUnits();
		}
	}
}
