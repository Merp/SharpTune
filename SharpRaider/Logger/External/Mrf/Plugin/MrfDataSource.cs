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
using RomRaider.Logger.External.Mrf.IO;
using RomRaider.Logger.External.Mrf.Plugin;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Mrf.Plugin
{
	public sealed class MrfDataSource : ExternalDataSource
	{
		private readonly IDictionary<MrfSensor.MrfSensorType, MrfDataItem> dataItems = new 
			Dictionary<MrfSensor.MrfSensorType, MrfDataItem>();

		private MrfRunner runner;

		private string port;

		public string GetId()
		{
			return GetType().FullName;
		}

		public string GetName()
		{
			return "MRF Stealth Gauge";
		}

		public string GetVersion()
		{
			return "0.02";
		}

		public IList<ExternalDataItem> GetDataItems()
		{
			return Sharpen.Collections.UnmodifiableList(new AList<MrfDataItem>(dataItems.Values
				));
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
			runner = new MrfRunner(port, dataItems);
			ThreadUtil.RunAsDaemon(runner);
		}

		public void Disconnect()
		{
			if (runner != null)
			{
				runner.Stop();
			}
		}

		public MrfDataSource()
		{
			{
				dataItems.Put(MrfSensor.MrfSensorType.AFR, new MrfDataItem("Wideband", SensorConversionsAFR
					.AFR_147, SensorConversionsAFR.LAMBDA, SensorConversionsAFR.AFR_90, SensorConversionsAFR
					.AFR_146, SensorConversionsAFR.AFR_64, SensorConversionsAFR.AFR_155, SensorConversionsAFR
					.AFR_172, SensorConversionsAFR.AFR_34));
				dataItems.Put(MrfSensor.MrfSensorType.MAP, new MrfDataItem("MAP", SensorConversionsOther
					.AIR_REL_PSI, SensorConversionsOther.AIR_REL_PSI2BAR, SensorConversionsOther.AIR_REL_PSI2KPA
					));
				dataItems.Put(MrfSensor.MrfSensorType.EGT, new MrfDataItem("EGT", SensorConversionsOther
					.EXHAUST_DEG_F, SensorConversionsOther.EXHAUST_DEG_F2C));
				dataItems.Put(MrfSensor.MrfSensorType.OIL_TEMP, new MrfDataItem("Oil Temp", SensorConversionsOther
					.FLUID_DEG_F, SensorConversionsOther.FLUID_DEG_F2C));
				dataItems.Put(MrfSensor.MrfSensorType.OIL_PRESS, new MrfDataItem("Oil Press", SensorConversionsOther
					.OIL_PSI, SensorConversionsOther.OIL_PSI2BAR, SensorConversionsOther.OIL_PSI2KPA
					));
				dataItems.Put(MrfSensor.MrfSensorType.FUEL_PRESS, new MrfDataItem("Fuel Press", SensorConversionsOther
					.FUEL_PSI, SensorConversionsOther.FUEL_PSI2BAR, SensorConversionsOther.FUEL_PSI2KPA
					));
				dataItems.Put(MrfSensor.MrfSensorType.MANIFOLD_TEMP, new MrfDataItem("Manifold Temp"
					, SensorConversionsOther.AIR_DEG_F, SensorConversionsOther.AIR_DEG_F2C));
			}
		}
	}
}
