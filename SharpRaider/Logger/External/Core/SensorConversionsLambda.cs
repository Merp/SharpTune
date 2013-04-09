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

using RomRaider.Logger.Ecu.UI.Handler.Dash;
using RomRaider.Logger.External.Core;
using Sharpen;

namespace RomRaider.Logger.External.Core
{
	public class SensorConversionsLambda
	{
		public static readonly ExternalSensorConversion LAMBDA = new ExternalSensorConversion
			("Lambda", "x", "0.00", new GaugeMinMax(0.6, 1.4, 0.1));

		public static readonly ExternalSensorConversion AFR_147 = new ExternalSensorConversion
			("AFR Gasoline", "x*14.7", "0.00", new GaugeMinMax(9, 20, 1));

		public static readonly ExternalSensorConversion AFR_90 = new ExternalSensorConversion
			("AFR Ethonal", "x*9.0", "0.00", new GaugeMinMax(5, 12, 1));

		public static readonly ExternalSensorConversion AFR_146 = new ExternalSensorConversion
			("AFR Diesel", "x*14.6", "0.00", new GaugeMinMax(9, 20, 1));

		public static readonly ExternalSensorConversion AFR_64 = new ExternalSensorConversion
			("AFR Methonal", "x*6.4", "0.00", new GaugeMinMax(4, 9, 1));

		public static readonly ExternalSensorConversion AFR_155 = new ExternalSensorConversion
			("AFR LPG", "x*15.5", "0.00", new GaugeMinMax(9, 20, 1));

		public static readonly ExternalSensorConversion AFR_172 = new ExternalSensorConversion
			("AFR CNG", "x*17.2", "0.00", new GaugeMinMax(9, 20, 1));

		public static readonly ExternalSensorConversion AFR_34 = new ExternalSensorConversion
			("AFR Hydrogen", "x*34", "0.00", new GaugeMinMax(20, 46, 2.5));
		// AFR conversion assumes reported DATA value is Lambda
		// gasoline
		// ethanol
		// diesel
		// methanol
		// LPG
		// CNG
		// Hydrogen
	}
}
