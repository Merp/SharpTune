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

using RomRaider.Logger.Car.Util;
using Sharpen;

namespace RomRaider.Logger.Car.Util
{
	public class SpeedCalculator
	{
		private static readonly double K2M = double.ParseDouble(Constants.KPH_2_MPH.value
			);

		public static double CalculateMph(double rpm, double ratio)
		{
			return (rpm / ratio);
		}

		public static double CalculateKph(double rpm, double ratio)
		{
			return CalculateMph(rpm, ratio) * K2M;
		}

		public static double CalculateRpm(double vs, double ratio, string units)
		{
			double rpm = 0;
			if (Sharpen.Runtime.EqualsIgnoreCase(units, Constants.IMPERIAL_UNIT.value))
			{
				rpm = (vs * ratio);
			}
			if (Sharpen.Runtime.EqualsIgnoreCase(units, Constants.METRIC_UNIT.value))
			{
				rpm = (vs * ratio / K2M);
			}
			return rpm;
		}
	}
}
