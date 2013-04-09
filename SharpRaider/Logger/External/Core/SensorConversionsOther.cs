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
	public class SensorConversionsOther
	{
		public static readonly ExternalSensorConversion AIR_DEG_C = new ExternalSensorConversion
			("C", "x", "0.0", new GaugeMinMax(-40, 60, 10));

		public static readonly ExternalSensorConversion AIR_DEG_F = new ExternalSensorConversion
			("F", "x", "0.0", new GaugeMinMax(-40, 140, 20));

		public static readonly ExternalSensorConversion AIR_DEG_F2C = new ExternalSensorConversion
			("C", "(x-32)*5/9", "0.0", new GaugeMinMax(-40, 60, 10));

		public static readonly ExternalSensorConversion AIR_DEG_C2F = new ExternalSensorConversion
			("F", "x*9/5+32 ", "0.0", new GaugeMinMax(-40, 140, 20));

		public static readonly ExternalSensorConversion EXHAUST_DEG_C = new ExternalSensorConversion
			("C", "x", "0.0", new GaugeMinMax(-40, 1000, 100));

		public static readonly ExternalSensorConversion EXHAUST_DEG_F = new ExternalSensorConversion
			("F", "x", "0.0", new GaugeMinMax(-40, 2000, 200));

		public static readonly ExternalSensorConversion EXHAUST_DEG_F2C = new ExternalSensorConversion
			("C", "(x-32)*5/9", "0.0", new GaugeMinMax(-40, 1000, 100));

		public static readonly ExternalSensorConversion EXHAUST_DEG_C2F = new ExternalSensorConversion
			("F", "x*9/5+32 ", "0.0", new GaugeMinMax(-40, 2000, 200));

		public static readonly ExternalSensorConversion FLUID_DEG_C = new ExternalSensorConversion
			("C", "x", "0.0", new GaugeMinMax(-40, 160, 20));

		public static readonly ExternalSensorConversion FLUID_DEG_F = new ExternalSensorConversion
			("F", "x", "0.0", new GaugeMinMax(-40, 320, 40));

		public static readonly ExternalSensorConversion FLUID_DEG_F2C = new ExternalSensorConversion
			("C", "(x-32)*5/9", "0.0", new GaugeMinMax(-40, 160, 20));

		public static readonly ExternalSensorConversion FLUID_DEG_C2F = new ExternalSensorConversion
			("F", "x*9/5+32 ", "0.0", new GaugeMinMax(-40, 320, 40));

		public static readonly ExternalSensorConversion AIR_ABS_PSI = new ExternalSensorConversion
			("psi", "x", "0.00", new GaugeMinMax(10, 40, 5));

		public static readonly ExternalSensorConversion AIR_ABS_PSI2BAR = new ExternalSensorConversion
			("bar", "x*0.0689475728", "0.00", new GaugeMinMax(0.5, 4.5, 0.5));

		public static readonly ExternalSensorConversion AIR_ABS_PSI2KPA = new ExternalSensorConversion
			("kPa", "x*6.89475728", "0.0", new GaugeMinMax(100, 300, 20));

		public static readonly ExternalSensorConversion AIR_ABS_PSI2KGCM2 = new ExternalSensorConversion
			("kg/cm^2", "x*0.0703068835943", "0.0", new GaugeMinMax(0.5, 4.5, 0.5));

		public static readonly ExternalSensorConversion AIR_ABS_KPA2PSI = new ExternalSensorConversion
			("psi", "x*0.14503774", "0.00", new GaugeMinMax(10, 40, 5));

		public static readonly ExternalSensorConversion AIR_ABS_KPA2BAR = new ExternalSensorConversion
			("bar", "x*0.01", "0.00", new GaugeMinMax(0.5, 4.5, 0.5));

		public static readonly ExternalSensorConversion AIR_ABS_KPA = new ExternalSensorConversion
			("kPa", "x", "0.0", new GaugeMinMax(100, 300, 20));

		public static readonly ExternalSensorConversion AIR_ABS_KPA2KGCM2 = new ExternalSensorConversion
			("kg/cm^2", "x*0.01019716", "0.00", new GaugeMinMax(0.5, 4.5, 0.5));

		public static readonly ExternalSensorConversion AIR_REL_PSI = new ExternalSensorConversion
			("psi", "x", "0.00", new GaugeMinMax(-10, 30, 5));

		public static readonly ExternalSensorConversion AIR_REL_PSI2BAR = new ExternalSensorConversion
			("bar", "x*0.0689475728", "0.00", new GaugeMinMax(-0.5, 2.5, 0.3));

		public static readonly ExternalSensorConversion AIR_REL_PSI2KPA = new ExternalSensorConversion
			("kPa", "x*6.89475728", "0.0", new GaugeMinMax(98, 120, 2));

		public static readonly ExternalSensorConversion AIR_REL_PSI2KGCM2 = new ExternalSensorConversion
			("kg/cm^2", "x*0.0703068835943", "0.0", new GaugeMinMax(-0.5, 2.5, 0.3));

		public static readonly ExternalSensorConversion AIR_REL_KPA2PSI = new ExternalSensorConversion
			("psi", "x*0.14503774", "0.00", new GaugeMinMax(-10, 30, 5));

		public static readonly ExternalSensorConversion AIR_REL_KPA2BAR = new ExternalSensorConversion
			("bar", "x*0.01", "0.00", new GaugeMinMax(-0.5, 2.5, 0.3));

		public static readonly ExternalSensorConversion AIR_REL_KPA = new ExternalSensorConversion
			("kPa", "x", "0.0", new GaugeMinMax(98, 120, 2));

		public static readonly ExternalSensorConversion AIR_REL_KPA2KGCM2 = new ExternalSensorConversion
			("kg/cm^2", "x*0.01019716", "0.00", new GaugeMinMax(-0.5, 2.5, 0.3));

		public static readonly ExternalSensorConversion FUEL_PSI = new ExternalSensorConversion
			("psi", "x", "0.00", new GaugeMinMax(0, 50, 5));

		public static readonly ExternalSensorConversion FUEL_PSI2BAR = new ExternalSensorConversion
			("bar", "x*0.0689475728", "0.00", new GaugeMinMax(0, 4, 0.5));

		public static readonly ExternalSensorConversion FUEL_PSI2KPA = new ExternalSensorConversion
			("kPa", "x*6.89475728", "0.0", new GaugeMinMax(0, 350, 50));

		public static readonly ExternalSensorConversion FUEL_PSI2KGCM2 = new ExternalSensorConversion
			("kg/cm^2", "x*0.0703068835943", "0.0", new GaugeMinMax(0, 4, 0.5));

		public static readonly ExternalSensorConversion FUEL_KPA2PSI = new ExternalSensorConversion
			("psi", "x*0.14503774", "0.00", new GaugeMinMax(0, 50, 5));

		public static readonly ExternalSensorConversion FUEL_KPA2BAR = new ExternalSensorConversion
			("bar", "x*0.01", "0.00", new GaugeMinMax(0, 4, 0.5));

		public static readonly ExternalSensorConversion FUEL_KPA = new ExternalSensorConversion
			("kPa", "x", "0.0", new GaugeMinMax(0, 350, 50));

		public static readonly ExternalSensorConversion FUEL_KPA2KGCM2 = new ExternalSensorConversion
			("kg/cm^2", "x*0.01019716", "0.00", new GaugeMinMax(0, 4, 0.5));

		public static readonly ExternalSensorConversion OIL_PSI = new ExternalSensorConversion
			("psi", "x", "0.00", new GaugeMinMax(0, 150, 15));

		public static readonly ExternalSensorConversion OIL_PSI2BAR = new ExternalSensorConversion
			("bar", "x*0.0689475728", "0.00", new GaugeMinMax(0, 10, 1));

		public static readonly ExternalSensorConversion OIL_PSI2KPA = new ExternalSensorConversion
			("kPa", "x*6.89475728", "0.0", new GaugeMinMax(0, 1035, 100));

		public static readonly ExternalSensorConversion OIL_PSI2KGCM2 = new ExternalSensorConversion
			("kg/cm^2", "x*0.0703068835943", "0.0", new GaugeMinMax(0, 11, 1));

		public static readonly ExternalSensorConversion OIL_KPA2PSI = new ExternalSensorConversion
			("psi", "x*0.14503774", "0.00", new GaugeMinMax(0, 150, 15));

		public static readonly ExternalSensorConversion OIL_KPA2BAR = new ExternalSensorConversion
			("bar", "x*0.01", "0.00", new GaugeMinMax(0, 10, 1));

		public static readonly ExternalSensorConversion OIL_KPA = new ExternalSensorConversion
			("kPa", "x", "0.0", new GaugeMinMax(0, 1035, 100));

		public static readonly ExternalSensorConversion OIL_KPA2KGCM2 = new ExternalSensorConversion
			("kg/cm^2", "x*0.01019716", "0.00", new GaugeMinMax(0, 11, 1));

		public static readonly ExternalSensorConversion MAF_GS = new ExternalSensorConversion
			("g/sec", "x", "0.00", new GaugeMinMax(0, 400, 50));

		public static readonly ExternalSensorConversion MAF_GS2LB = new ExternalSensorConversion
			("lb/min", "x/7.54", "0.00", new GaugeMinMax(0, 50, 5));

		public static readonly ExternalSensorConversion PERCENT = new ExternalSensorConversion
			("%", "x", "0.0", new GaugeMinMax(0, 100, 10));

		public static readonly ExternalSensorConversion VOLTS_5DC = new ExternalSensorConversion
			("VDC", "x", "0.0", new GaugeMinMax(0, 5, 0.5));

		public static readonly ExternalSensorConversion VOLTS_12DC = new ExternalSensorConversion
			("VDC", "x", "0.0", new GaugeMinMax(0, 15, 1.5));
		// converts from PSI to bar
		// converts from PSI to kpa
		// converts from PSI to kpa
		// converts from kPa
		// converts from kPa
		// converts from kPa
		// converts from PSI to bar
		// converts from PSI to kpa
		// converts from PSI to kpa
		// converts from kPa
		// converts from kPa
		// converts from kPa
		// converts from PSI to bar
		// converts from PSI to kpa
		// converts from PSI to kpa
		// converts from kPa
		// converts from kPa
		// converts from kPa
		// converts from PSI to bar
		// converts from PSI to kpa
		// converts from PSI to kpa
		// converts from kPa
		// converts from kPa
		// converts from kPa
	}
}
