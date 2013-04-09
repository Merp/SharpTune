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
using Com.Phidgets;
using RomRaider.Logger.External.Core;
using RomRaider.Logger.External.Phidget.Interfacekit.IO;
using RomRaider.Logger.External.Phidget.Interfacekit.Plugin;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Phidget.Interfacekit.IO
{
	/// <summary>IntfKitRunner manages the connections to the PhidgetInterfaceKits.</summary>
	/// <remarks>
	/// IntfKitRunner manages the connections to the PhidgetInterfaceKits.  It
	/// also responds to the sensor change events to update the appropriate
	/// sensor's value.
	/// </remarks>
	public sealed class IntfKitRunner : Stoppable
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(RomRaider.Logger.External.Phidget.Interfacekit.IO.IntfKitRunner));

		private readonly IDictionary<string, IntfKitDataItem> dataItems;

		private ICollection<InterfaceKitPhidget> connections;

		private bool stop;

		/// <summary>
		/// IntfKitRunner interrogates the PhidgetInterfaceKits for data and updates
		/// the appropriate sensor result.
		/// </summary>
		/// <remarks>
		/// IntfKitRunner interrogates the PhidgetInterfaceKits for data and updates
		/// the appropriate sensor result.
		/// </remarks>
		/// <param name="kits">- array of serial numbers of the PhidgetInterfaceKits</param>
		/// <param name="dataItems">- a Map of PhidgetInterfaceKit data items (sensors)</param>
		public IntfKitRunner(int[] kits, IDictionary<string, IntfKitDataItem> dataItems)
		{
			this.dataItems = dataItems;
			this.connections = IntfKitConnector.OpenIkSerial(this, kits);
		}

		/// <summary>
		/// This method is used to start and stop the reading of the
		/// PhidgetInterfaceKits data.
		/// </summary>
		/// <remarks>
		/// This method is used to start and stop the reading of the
		/// PhidgetInterfaceKits data.  Each data item value is set by the
		/// sensor change Listener. When stop is issued the connection is
		/// closed to the device.
		/// </remarks>
		/// <exception cref="Com.Phidgets.PhidgetException">Com.Phidgets.PhidgetException</exception>
		/// <seealso cref="IntfKitSensorChangeListener">IntfKitSensorChangeListener</seealso>
		public void Run()
		{
			try
			{
				while (!stop)
				{
					ThreadUtil.Sleep(500L);
				}
				foreach (InterfaceKitPhidget connector in connections)
				{
					connector.Close();
				}
			}
			catch (PhidgetException e)
			{
				LOGGER.Error("InterfaceKit close error: " + e);
			}
		}

		/// <summary>
		/// This method is used stop the reading of the PhidgetInterfaceKits data
		/// and close the connections to the devices.
		/// </summary>
		/// <remarks>
		/// This method is used stop the reading of the PhidgetInterfaceKits data
		/// and close the connections to the devices.
		/// </remarks>
		public void Stop()
		{
			stop = true;
		}

		/// <summary>
		/// This method is event driven and called by the SensorChangeListner
		/// with the sensor ID and the new value to be set.
		/// </summary>
		/// <remarks>
		/// This method is event driven and called by the SensorChangeListner
		/// with the sensor ID and the new value to be set.
		/// </remarks>
		/// <param name="serial">- serial number of the InterfaceKit reporting the change</param>
		/// <param name="sensor">- the sensor number reporting the change</param>
		/// <param name="value">- the new value to set</param>
		public void UpdateDataItem(int serial, int sensor, int value)
		{
			if (serial != -1)
			{
				string inputName = string.Format("%d:%d", serial, sensor);
				dataItems.Get(inputName).SetData((double)value);
				string result = string.Format("Phidget InterfaceKit sensor %s event - raw value: %d"
					, inputName, sensor, value);
				LOGGER.Trace(result);
			}
			else
			{
				LOGGER.Error("Phidget InterfaceKit dataitem update error");
			}
		}
	}
}
