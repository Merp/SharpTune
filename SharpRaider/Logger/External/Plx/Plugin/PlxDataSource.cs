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
using Javax.Swing;
using RomRaider.Logger.Ecu;
using RomRaider.Logger.External.Core;
using RomRaider.Logger.External.Plx.IO;
using RomRaider.Logger.External.Plx.Plugin;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Plx.Plugin
{
	public sealed class PlxDataSource : ExternalDataSource
	{
		private readonly IDictionary<PlxSensorType, PlxDataItem> dataItems = new Dictionary
			<PlxSensorType, PlxDataItem>();

		private PlxRunner runner;

		private string port;

		public string GetId()
		{
			return GetType().FullName;
		}

		public string GetName()
		{
			return "PLX SM-AFR";
		}

		public string GetVersion()
		{
			return "0.04";
		}

		public IList<ExternalDataItem> GetDataItems()
		{
			return new AList<ExternalDataItem>(dataItems.Values);
		}

		public Action GetMenuAction(EcuLogger logger)
		{
			return null;
		}

		public void SetPort(string port)
		{
			this.port = port;
		}

		public string GetPort()
		{
			return port;
		}

		public void Connect()
		{
			runner = new PlxRunner(port, dataItems);
			ThreadUtil.RunAsDaemon(runner);
		}

		public void Disconnect()
		{
			if (runner != null)
			{
				runner.Stop();
			}
		}

		public PlxDataSource()
		{
			{
				dataItems.Put(PlxSensorType.WIDEBAND, new PlxDataItemImpl("O2 - Wideband", 0, PlxSensorConversions
					.AFR_147, PlxSensorConversions.LAMBDA, PlxSensorConversions.AFR_90, PlxSensorConversions
					.AFR_146, PlxSensorConversions.AFR_64, PlxSensorConversions.AFR_155, PlxSensorConversions
					.AFR_172, PlxSensorConversions.AFR_34));
				dataItems.Put(PlxSensorType.EXHAUST_GAS_TEMPERATURE, new PlxDataItemImpl("Temperature - Exhaust Gas"
					, 0, SensorConversionsOther.EXHAUST_DEG_C, SensorConversionsOther.EXHAUST_DEG_C2F
					));
				dataItems.Put(PlxSensorType.FLUID_TEMPERATURE, new PlxDataItemImpl("Temperature - Oil/H20"
					, 0, SensorConversionsOther.FLUID_DEG_C, SensorConversionsOther.FLUID_DEG_C2F));
				dataItems.Put(PlxSensorType.VACUUM, new PlxDataItemImpl("Manifold Vaccum", 0, PlxSensorConversions
					.VACUUM_IN, PlxSensorConversions.VACUUM_MM));
				dataItems.Put(PlxSensorType.BOOST, new PlxDataItemImpl("Manifold Boost", 0, PlxSensorConversions
					.BOOST_PSI, PlxSensorConversions.BOOST_BAR, PlxSensorConversions.BOOST_KPA, PlxSensorConversions
					.BOOST_KGCM2));
				dataItems.Put(PlxSensorType.AIR_INTAKE_TEMPERATURE, new PlxDataItemImpl("Temperature - Intake Air"
					, 0, SensorConversionsOther.AIR_DEG_C, SensorConversionsOther.AIR_DEG_C2F));
				dataItems.Put(PlxSensorType.ENGINE_SPEED, new PlxDataItemImpl("Engine Speed", 0, 
					PlxSensorConversions.RPM));
				dataItems.Put(PlxSensorType.VEHICLE_SPEED, new PlxDataItemImpl("Vehicle Speed", 0
					, PlxSensorConversions.MPH, PlxSensorConversions.KPH));
				dataItems.Put(PlxSensorType.THROTTLE_POSITION, new PlxDataItemImpl("Throttle Position"
					, 0, SensorConversionsOther.PERCENT));
				dataItems.Put(PlxSensorType.ENGINE_LOAD, new PlxDataItemImpl("Engine Load", 0, SensorConversionsOther
					.PERCENT));
				dataItems.Put(PlxSensorType.FLUID_PRESSURE, new PlxDataItemImpl("Fuel/0il Pressure"
					, 0, PlxSensorConversions.FLUID_PSI, PlxSensorConversions.FLUID_BAR, PlxSensorConversions
					.FLUID_KPA, PlxSensorConversions.FLUID_KGCM2));
				dataItems.Put(PlxSensorType.TIMING, new PlxDataItemImpl("Engine Timing", 0, PlxSensorConversions
					.DEGREES));
				dataItems.Put(PlxSensorType.MANIFOLD_ABSOLUTE_PRESSURE, new PlxDataItemImpl("Manifold Absolute Pressure"
					, 0, SensorConversionsOther.AIR_ABS_KPA2PSI, SensorConversionsOther.AIR_ABS_KPA2BAR
					, SensorConversionsOther.AIR_ABS_KPA, SensorConversionsOther.AIR_ABS_KPA2KGCM2));
				dataItems.Put(PlxSensorType.MASS_AIR_FLOW, new PlxDataItemImpl("Mass Air Flow", 0
					, SensorConversionsOther.MAF_GS, SensorConversionsOther.MAF_GS2LB));
				dataItems.Put(PlxSensorType.SHORT_TERM_FUEL_TRIM, new PlxDataItemImpl("Fuel Trim - Short Term"
					, 0, PlxSensorConversions.FUEL_TRIM));
				dataItems.Put(PlxSensorType.LONG_TERM_FUEL_TRIM, new PlxDataItemImpl("Fuel Trim - Long Term"
					, 0, PlxSensorConversions.FUEL_TRIM));
				dataItems.Put(PlxSensorType.NARROWBAND_AFR, new PlxDataItemImpl("O2 - Narrowband"
					, 0, PlxSensorConversions.NB_P, PlxSensorConversions.NB_V));
				dataItems.Put(PlxSensorType.FUEL_LEVEL, new PlxDataItemImpl("Fuel Level", 0, SensorConversionsOther
					.PERCENT));
				dataItems.Put(PlxSensorType.VOLTAGE, new PlxDataItemImpl("Battery Voltage", 0, PlxSensorConversions
					.BATTERY));
				dataItems.Put(PlxSensorType.KNOCK, new PlxDataItemImpl("Knock", 0, PlxSensorConversions
					.KNOCK_VDC));
				dataItems.Put(PlxSensorType.DUTY_CYCLE, new PlxDataItemImpl("Duty Cycle", 0, PlxSensorConversions
					.DC_POS, PlxSensorConversions.DC_NEG));
			}
		}
	}
}
