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
using RomRaider.Logger.External.TE.IO;
using RomRaider.Logger.External.TE.Plugin;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.TE.Plugin
{
	public sealed class TEDataSource : ExternalDataSource
	{
		private readonly IDictionary<ExternalSensorType, TEDataItem> dataItems = new Dictionary
			<ExternalSensorType, TEDataItem>();

		private TERunner runner;

		private string port;

		public string GetId()
		{
			return GetType().FullName;
		}

		public string GetName()
		{
			return "Tech Edge (Format 2.0)";
		}

		public string GetVersion()
		{
			return "0.03";
		}

		public IList<ExternalDataItem> GetDataItems()
		{
			return Sharpen.Collections.UnmodifiableList(new AList<TEDataItem>(dataItems.Values
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
			runner = new TERunner(port, dataItems);
			ThreadUtil.RunAsDaemon(runner);
		}

		public void Disconnect()
		{
			if (runner != null)
			{
				runner.Stop();
			}
		}

		public TEDataSource()
		{
			{
				dataItems.Put(ExternalSensorType.WIDEBAND, new TEDataItemImpl("Wideband", TESensorConversions
					.LAMBDA, TESensorConversions.AFR_147, TESensorConversions.AFR_90, TESensorConversions
					.AFR_146, TESensorConversions.AFR_64, TESensorConversions.AFR_155, TESensorConversions
					.AFR_172, TESensorConversions.AFR_34));
				dataItems.Put(ExternalSensorType.USER1, new TEDataItemImpl("User 1", TESensorConversions
					.VDC));
				dataItems.Put(ExternalSensorType.USER2, new TEDataItemImpl("User 2", TESensorConversions
					.VDC));
				dataItems.Put(ExternalSensorType.USER3, new TEDataItemImpl("User 3", TESensorConversions
					.VDC));
				dataItems.Put(ExternalSensorType.THERMACOUPLE1, new TEDataItemImpl("Thermocouple 1"
					, TESensorConversions.TC));
				dataItems.Put(ExternalSensorType.THERMACOUPLE2, new TEDataItemImpl("Thermocouple 2"
					, TESensorConversions.TC));
				dataItems.Put(ExternalSensorType.THERMACOUPLE3, new TEDataItemImpl("Thermocouple 3"
					, TESensorConversions.TC));
				dataItems.Put(ExternalSensorType.TorVss, new TEDataItemImpl("Thermistor or Vss", 
					TESensorConversions.THERM));
				dataItems.Put(ExternalSensorType.ENGINE_SPEED, new TEDataItemImpl("Engine Speed (4-cyl)"
					, TESensorConversions.RPM_4));
			}
		}
	}
}
