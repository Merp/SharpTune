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

using Com.Phidgets;
using Com.Phidgets.Event;
using RomRaider.Logger.External.Phidget.Interfacekit.IO;
using Sharpen;

namespace RomRaider.Logger.External.Phidget.Interfacekit.IO
{
	/// <summary>IntfKitSensorChangeListener responds to Sensor changes.</summary>
	/// <remarks>
	/// IntfKitSensorChangeListener responds to Sensor changes.  It filters
	/// the exact sensor to be updated and provides the update to the
	/// IntfKitRunner's data items.
	/// </remarks>
	public class IntfKitSensorChangeListener : SensorChangeListener
	{
		private readonly IntfKitRunner ikr;

		/// <summary>
		/// Creates a new instance of IntfKitSensorChangeListener to be
		/// registered against each PhidgetInterfaceKit opened.
		/// </summary>
		/// <remarks>
		/// Creates a new instance of IntfKitSensorChangeListener to be
		/// registered against each PhidgetInterfaceKit opened.
		/// </remarks>
		/// <param name="ikr">- the instance of the InterfaceKitRunner</param>
		public IntfKitSensorChangeListener(IntfKitRunner ikr)
		{
			this.ikr = ikr;
		}

		/// <summary>
		/// Handles the sensor change, isolates the serial number, sensor
		/// number and value then calls to the InterfaceKitRunner to
		/// update the matching data item.
		/// </summary>
		/// <remarks>
		/// Handles the sensor change, isolates the serial number, sensor
		/// number and value then calls to the InterfaceKitRunner to
		/// update the matching data item.
		/// </remarks>
		/// <param name="sensorChangeEvent">- the event from the Phidget device</param>
		public virtual void SensorChanged(SensorChangeEvent sensorChangeEvent)
		{
			try
			{
				ikr.UpdateDataItem(sensorChangeEvent.GetSource().GetSerialNumber(), sensorChangeEvent
					.GetIndex(), sensorChangeEvent.GetValue());
			}
			catch (PhidgetException)
			{
				ikr.UpdateDataItem(-1, -1, -1);
			}
		}
	}
}
