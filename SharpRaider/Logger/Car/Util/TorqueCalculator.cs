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
	public class TorqueCalculator
	{
		public static double CalculateTorque(double rpm, double hp, string units)
		{
			double tq = 0;
			if (Sharpen.Runtime.EqualsIgnoreCase(units, Constants.IMPERIAL.value))
			{
				tq = hp / rpm * double.ParseDouble(Constants.TQ_CONSTANT_I.value);
			}
			if (Sharpen.Runtime.EqualsIgnoreCase(units, Constants.METRIC.value))
			{
				tq = hp / rpm * double.ParseDouble(Constants.TQ_CONSTANT_M.value);
			}
			return tq;
		}
	}
}
