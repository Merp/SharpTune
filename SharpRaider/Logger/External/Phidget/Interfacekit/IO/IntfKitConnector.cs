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

using System;
using System.Collections.Generic;
using Com.Phidgets;
using RomRaider.Logger.External.Phidget.Interfacekit.IO;
using Sharpen;

namespace RomRaider.Logger.External.Phidget.Interfacekit.IO
{
	/// <summary>
	/// IntfKitConnector will open a connection to each serial number provided
	/// and return those connections in a Set for use by the runner.
	/// </summary>
	/// <remarks>
	/// IntfKitConnector will open a connection to each serial number provided
	/// and return those connections in a Set for use by the runner.
	/// </remarks>
	public sealed class IntfKitConnector
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(IntfKitConnector));

		/// <summary>
		/// Open a connection to each serial number provided and
		/// return those connections in a Set for use by the runner.
		/// </summary>
		/// <remarks>
		/// Open a connection to each serial number provided and
		/// return those connections in a Set for use by the runner.
		/// </remarks>
		/// <param name="ikr">- the instance of IntfKitRunner calling this class</param>
		/// <param name="serials">- array of serial numbers to open</param>
		/// <returns>a Set of InterfaceKitPhidget connections</returns>
		/// <exception cref="System.Exception">System.Exception</exception>
		/// <exception cref="Com.Phidgets.PhidgetException">Com.Phidgets.PhidgetException</exception>
		/// <seealso cref="IntfKitRunner">IntfKitRunner</seealso>
		public static ICollection<InterfaceKitPhidget> OpenIkSerial(IntfKitRunner ikr, int
			[] serials)
		{
			ICollection<InterfaceKitPhidget> kits = new HashSet<InterfaceKitPhidget>();
			try
			{
				foreach (int serial in serials)
				{
					InterfaceKitPhidget ik = new InterfaceKitPhidget();
					IntfKitSensorChangeListener scl = new IntfKitSensorChangeListener(ikr);
					ik.AddSensorChangeListener(scl);
					ik.Open(serial);
					long timeout = Runtime.CurrentTimeMillis() + 500L;
					do
					{
						Sharpen.Thread.Sleep(50);
					}
					while (!ik.IsAttached() && (Runtime.CurrentTimeMillis() < timeout));
					int inputCount = ik.GetSensorCount();
					for (int i = 0; i < inputCount; i++)
					{
						ik.SetSensorChangeTrigger(i, 1);
					}
					kits.AddItem(ik);
				}
			}
			catch (PhidgetException e)
			{
				LOGGER.Error("InterfaceKit open error: " + e);
			}
			catch (Exception e)
			{
				LOGGER.Info("Sleep interrupted " + e);
			}
			return kits;
		}
	}
}
