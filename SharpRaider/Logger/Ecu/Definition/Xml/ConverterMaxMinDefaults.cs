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
using RomRaider.Logger.Ecu.Definition.Xml;
using RomRaider.Logger.Ecu.UI.Handler.Dash;
using Sharpen;

namespace RomRaider.Logger.Ecu.Definition.Xml
{
	public sealed class ConverterMaxMinDefaults
	{
		private static readonly IDictionary<string, GaugeMinMax> DEFAULTS = new Dictionary
			<string, GaugeMinMax>();

		private const double MIN_DEFAULT = 0.0;

		private const double MAX_DEFAULT = 100.0;

		private const double STEP_DEFAULT = 10.0;

		private static readonly GaugeMinMax DEFAULT = new GaugeMinMax(MIN_DEFAULT, MAX_DEFAULT
			, STEP_DEFAULT);

		static ConverterMaxMinDefaults()
		{
			Add("%", 0.0, 100.0, 10.0);
			Add("f", 0.0, 400.0, 40.0);
			Add("c", -20.0, 200.0, 20.0);
			Add("psi", -20.0, 40.0, 5.0);
			Add("bar", -1.5, 3.0, 0.5);
			Add("rpm", 0, 8000, 1000.0);
			Add("mph", 0.0, 200.0, 20.0);
			Add("km/h", 0.0, 300.0, 20.0);
			Add("degrees", -15, 60.0, 5.0);
			Add("g/s", 0.0, 400.0, 20.0);
			Add("v", 0.0, 5.0, 0.5);
			Add("ms", 0.0, 100.0, 10.0);
			Add("a", 0.0, 20.0, 5.0);
			Add("ma", 0.0, 100.0, 10.0);
			Add("steps", 0.0, 100.0, 10.0);
			Add("ohms", 0.0, 100.0, 10.0);
			Add("afr", 10.0, 20.0, 1.0);
			Add("lambda", 0.5, 1.5, 0.1);
			Add("gear", 0.0, 6.0, 1.0);
			Add("misfire count", 0.0, 20.0, 5.0);
			Add("MPa", 0.0, 0.5, 0.1);
			Add("2*g/rev", 0.0, 8.0, 1.0);
			Add("g/rev", 0.0, 4.0, 0.5);
			Add("g/cyl", 0.0, 2.0, 0.5);
			Add("multiplier", 0.0, 1.0, 0.1);
			Add("raw ecu value", 0.0, 16.0, 1.0);
			Add("status", 0.0, 10.0, 1.0);
			Add("mmHg", 0.0, 2000.0, 100.0);
		}

		public static double GetMin(string units)
		{
			string key = units.ToLower();
			if (!DEFAULTS.ContainsKey(key))
			{
				return MIN_DEFAULT;
			}
			return DEFAULTS.Get(key).min;
		}

		public static double GetMax(string units)
		{
			string key = units.ToLower();
			if (!DEFAULTS.ContainsKey(key))
			{
				return MAX_DEFAULT;
			}
			return DEFAULTS.Get(key).max;
		}

		public static double GetStep(string units)
		{
			string key = units.ToLower();
			if (!DEFAULTS.ContainsKey(key))
			{
				return STEP_DEFAULT;
			}
			return DEFAULTS.Get(key).step;
		}

		public static GaugeMinMax GetDefault()
		{
			return DEFAULT;
		}

		public static GaugeMinMax GetMaxMin(string units)
		{
			double min = GetMin(units);
			double max = GetMax(units);
			double step = GetStep(units);
			return new GaugeMinMax(min, max, step);
		}

		private static void Add(string units, double min, double max, double step)
		{
			string key = units.ToLower();
			GaugeMinMax value = new GaugeMinMax(min, max, step);
			DEFAULTS.Put(key, value);
		}
	}
}
