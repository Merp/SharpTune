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
using RomRaider.Logger.External.Zt2.IO;
using RomRaider.Logger.External.Zt2.Plugin;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Zt2.Plugin
{
	public sealed class ZT2DataSource : ExternalDataSource
	{
		private readonly IDictionary<ExternalSensorType, ZT2DataItem> dataItems = new Dictionary
			<ExternalSensorType, ZT2DataItem>();

		private ZT2Runner runner;

		private string port;

		public string GetId()
		{
			return GetType().FullName;
		}

		public string GetName()
		{
			return "Zeitronix ZT-2";
		}

		public string GetVersion()
		{
			return "0.03";
		}

		public IList<ExternalDataItem> GetDataItems()
		{
			return Sharpen.Collections.UnmodifiableList(new AList<ZT2DataItem>(dataItems.Values
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
			runner = new ZT2Runner(port, dataItems);
			ThreadUtil.RunAsDaemon(runner);
		}

		public void Disconnect()
		{
			if (runner != null)
			{
				runner.Stop();
			}
		}

		public ZT2DataSource()
		{
			{
				dataItems.Put(ExternalSensorType.WIDEBAND, new ZT2DataItemImpl("Wideband O2", ZT2SensorConversions
					.AFR_147, ZT2SensorConversions.LAMBDA, ZT2SensorConversions.AFR_90, ZT2SensorConversions
					.AFR_146, ZT2SensorConversions.AFR_64, ZT2SensorConversions.AFR_155, ZT2SensorConversions
					.AFR_172, ZT2SensorConversions.AFR_34));
				dataItems.Put(ExternalSensorType.TPS, new ZT2DataItemImpl("Throttle Poistion", SensorConversionsOther
					.PERCENT));
				dataItems.Put(ExternalSensorType.ENGINE_SPEED, new ZT2DataItemImpl("Engine Speed"
					, ZT2SensorConversions.RPM));
				dataItems.Put(ExternalSensorType.MAP, new ZT2DataItemImpl("MAP", ZT2SensorConversions
					.BOOST_PSI, ZT2SensorConversions.BOOST_BAR, ZT2SensorConversions.BOOST_KPA, ZT2SensorConversions
					.BOOST_KGCM2));
				dataItems.Put(ExternalSensorType.EGT, new ZT2DataItemImpl("EGT", SensorConversionsOther
					.EXHAUST_DEG_C, SensorConversionsOther.EXHAUST_DEG_C2F));
				dataItems.Put(ExternalSensorType.USER1, new ZT2DataItemImpl("User Input", SensorConversionsOther
					.VOLTS_5DC));
			}
		}
	}
}
