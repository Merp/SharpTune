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
using RomRaider.Logger.External.Phidget.Interfacekit.IO;
using RomRaider.Logger.External.Phidget.Interfacekit.Plugin;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Phidget.Interfacekit.Plugin
{
	/// <summary>
	/// The IntfKitDataSource class is called when the Logger starts up and the
	/// call to load the external plug-ins is made.
	/// </summary>
	/// <remarks>
	/// The IntfKitDataSource class is called when the Logger starts up and the
	/// call to load the external plug-ins is made.  This class with its helpers
	/// will open each PhidgetInterfaceKit and find all available inputs.  It will
	/// interrogate the inputs then dynamically build a list of inputs found based
	/// on the serial number and input number.
	/// </remarks>
	/// <seealso cref="RomRaider.Logger.External.Core.ExternalDataSource">RomRaider.Logger.External.Core.ExternalDataSource
	/// 	</seealso>
	public sealed class IntfKitDataSource : ExternalDataSource
	{
		private readonly IDictionary<string, IntfKitDataItem> dataItems = new Dictionary<
			string, IntfKitDataItem>();

		private IntfKitRunner runner;

		private int[] kits;

		public string GetId()
		{
			return GetType().FullName;
		}

		public string GetName()
		{
			return "Phidget InterfaceKit";
		}

		public string GetVersion()
		{
			return "0.01";
		}

		public IList<ExternalDataItem> GetDataItems()
		{
			return Sharpen.Collections.UnmodifiableList(new AList<IntfKitDataItem>(dataItems.
				Values));
		}

		public Action GetMenuAction(EcuLogger logger)
		{
			return new IntfKitPluginMenuAction(logger);
		}

		public void Connect()
		{
			runner = new IntfKitRunner(kits, dataItems);
			ThreadUtil.RunAsDaemon(runner);
		}

		public void Disconnect()
		{
			if (runner != null)
			{
				runner.Stop();
			}
		}

		public void SetPort(string port)
		{
		}

		public string GetPort()
		{
			return "HID USB";
		}

		public IntfKitDataSource()
		{
			{
				kits = IntfKitManager.FindIntfkits();
				if (kits.Length > 0)
				{
					IntfKitManager.LoadIk();
					foreach (int serial in kits)
					{
						ICollection<IntfKitSensor> sensors = IntfKitManager.GetSensors(serial);
						if (!sensors.IsEmpty())
						{
							foreach (IntfKitSensor sensor in sensors)
							{
								string inputName = string.Format("%d:%d", serial, sensor.GetInputNumber());
								dataItems.Put(inputName, new IntfKitDataItem(sensor.GetInputName(), sensor.GetUnits
									(), sensor.GetMinValue(), sensor.GetMaxValue()));
							}
						}
					}
				}
			}
		}
	}
}
