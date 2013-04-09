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
using RomRaider.Logger.External.Plx.Plugin;
using Sharpen;

namespace RomRaider.Logger.External.Plx.Plugin
{
	public class PlxSensorConversions
	{
		public static readonly ExternalSensorConversion LAMBDA = new ExternalSensorConversion
			("Lambda", "(x/3.75+68)/100", "0.00", new GaugeMinMax(0.6, 1.4, 0.08));

		public static readonly ExternalSensorConversion AFR_147 = new ExternalSensorConversion
			("AFR Gasoline", "(x/2.55+100)/10", "0.00", new GaugeMinMax(9, 20, 1));

		public static readonly ExternalSensorConversion AFR_90 = new ExternalSensorConversion
			("AFR Ethonal", "(x/4.167+61.7)/10", "0.00", new GaugeMinMax(5, 12, 1));

		public static readonly ExternalSensorConversion AFR_146 = new ExternalSensorConversion
			("AFR Diesel", "(x/2.58+100)/10", "0.00", new GaugeMinMax(9, 20, 1));

		public static readonly ExternalSensorConversion AFR_64 = new ExternalSensorConversion
			("AFR Methonal", "(x/5.856+43.5)/10", "0.00", new GaugeMinMax(4, 9, 1));

		public static readonly ExternalSensorConversion AFR_155 = new ExternalSensorConversion
			("AFR LPG", "(x/2.417+105.6)/10", "0.00", new GaugeMinMax(9, 20, 1));

		public static readonly ExternalSensorConversion AFR_172 = new ExternalSensorConversion
			("AFR CNG", "(x/2.18+117)/10", "0.00", new GaugeMinMax(9, 20, 1));

		public static readonly ExternalSensorConversion AFR_34 = new ExternalSensorConversion
			("AFR Hydrogen", "(x/3.75+68)*0.34", "0.00", new GaugeMinMax(20, 46, 2.5));

		public static readonly ExternalSensorConversion VACUUM_IN = new ExternalSensorConversion
			("in/Hg", "-(x/11.39-29.93)", "0.00", new GaugeMinMax(-20, 0, 2));

		public static readonly ExternalSensorConversion VACUUM_MM = new ExternalSensorConversion
			("mm/Hg", "-(x*2.23+760.4)", "0.00", new GaugeMinMax(-500, 0, 50));

		public static readonly ExternalSensorConversion BOOST_PSI = new ExternalSensorConversion
			("psi", "x/22.73", "0.00", new GaugeMinMax(-10, 30, 5));

		public static readonly ExternalSensorConversion BOOST_BAR = new ExternalSensorConversion
			("bar", "x*0.00303333", "0.00", new GaugeMinMax(-0.5, 2.5, 0.3));

		public static readonly ExternalSensorConversion BOOST_KPA = new ExternalSensorConversion
			("kPa", "x*0.30333292", "0.0", new GaugeMinMax(98, 120, 2));

		public static readonly ExternalSensorConversion BOOST_KGCM2 = new ExternalSensorConversion
			("kg/cm^2", "x/329.47", "0.00", new GaugeMinMax(-0.5, 2.5, 0.3));

		public static readonly ExternalSensorConversion RPM = new ExternalSensorConversion
			("rpm", "x*19.55", "0", new GaugeMinMax(0, 10000, 1000));

		public static readonly ExternalSensorConversion MPH = new ExternalSensorConversion
			("mph", "x/6.39", "0.0", new GaugeMinMax(0, 200, 20));

		public static readonly ExternalSensorConversion KPH = new ExternalSensorConversion
			("km/h", "x/3.97", "0.0", new GaugeMinMax(0, 300, 30));

		public static readonly ExternalSensorConversion FLUID_PSI = new ExternalSensorConversion
			("psi", "x/5.115", "0.00", new GaugeMinMax(0, 150, 15));

		public static readonly ExternalSensorConversion FLUID_BAR = new ExternalSensorConversion
			("bar", "x/74.22", "0.00", new GaugeMinMax(0, 10, 1));

		public static readonly ExternalSensorConversion FLUID_KPA = new ExternalSensorConversion
			("kPa", "x*1.34794864", "0.00", new GaugeMinMax(0, 1035, 100));

		public static readonly ExternalSensorConversion FLUID_KGCM2 = new ExternalSensorConversion
			("kg/cm^2", "x/72.73", "0.00", new GaugeMinMax(0, 10, 1));

		public static readonly ExternalSensorConversion DEGREES = new ExternalSensorConversion
			("deg", "x-64", "0.00", new GaugeMinMax(-10, 50, 5));

		public static readonly ExternalSensorConversion FUEL_TRIM = new ExternalSensorConversion
			("%", "x-100", "0.00", new GaugeMinMax(-30, 30, 5));

		public static readonly ExternalSensorConversion NB_P = new ExternalSensorConversion
			("%", "x", "0.00", new GaugeMinMax(0, 100, 10));

		public static readonly ExternalSensorConversion NB_V = new ExternalSensorConversion
			("vdc", "x/78.43", "0.00", new GaugeMinMax(-5, 5, 1));

		public static readonly ExternalSensorConversion BATTERY = new ExternalSensorConversion
			("vdc", "x/51.15", "0.00", new GaugeMinMax(0, 12, 1));

		public static readonly ExternalSensorConversion KNOCK_VDC = new ExternalSensorConversion
			("vdc", "x/204.6", "0.00", new GaugeMinMax(0, 5, 0.5));

		public static readonly ExternalSensorConversion DC_POS = new ExternalSensorConversion
			("+%", "x/10.23", "0.0", new GaugeMinMax(0, 100, 10));

		public static readonly ExternalSensorConversion DC_NEG = new ExternalSensorConversion
			("-%", "100-(x/10.23)", "0.0", new GaugeMinMax(-100, 0, 10));
		// gasoline
		// ethanol
		// diesel
		// methanol
		// LPG
		// CNG
		// Hydrogen
		// converts from PSI to bar
		// converts from PSI to kpa
		// converts from PSI to kpa
	}
}
