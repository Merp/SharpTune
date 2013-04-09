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
	/// IntfKitManager is used to discover all the attached PhidgetInterfaceKits
	/// by serial number, load the Phidgets library and get a list of all sensors
	/// on all PhidgetInterfaceKits.
	/// </summary>
	/// <remarks>
	/// IntfKitManager is used to discover all the attached PhidgetInterfaceKits
	/// by serial number, load the Phidgets library and get a list of all sensors
	/// on all PhidgetInterfaceKits.
	/// </remarks>
	public sealed class IntfKitManager
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(IntfKitManager));

		private static readonly string INTFKIT = "PhidgetInterfaceKit";

		private static InterfaceKitPhidget ik;

		/// <summary>Using the Phidgets Manager find all of the attached PhidgetInterfaceKits.
		/// 	</summary>
		/// <remarks>Using the Phidgets Manager find all of the attached PhidgetInterfaceKits.
		/// 	</remarks>
		/// <returns>an array of serial numbers</returns>
		/// <exception cref="Com.Phidgets.PhidgetException">Com.Phidgets.PhidgetException</exception>
		/// <exception cref="System.Exception">System.Exception</exception>
		public static int[] FindIntfkits()
		{
			IList<int> serials = new AList<int>();
			try
			{
				Manager fm = new Manager();
				fm.Open();
				Sharpen.Thread.Sleep(100);
				IList<Com.Phidgets.Phidget> phidgets = fm.GetPhidgets();
				foreach (Com.Phidgets.Phidget phidget in phidgets)
				{
					if (Sharpen.Runtime.EqualsIgnoreCase(phidget.GetDeviceType(), INTFKIT))
					{
						serials.AddItem(phidget.GetSerialNumber());
					}
				}
				fm.Close();
			}
			catch (PhidgetException e)
			{
				LOGGER.Error("Phidget Manager error: " + e);
			}
			catch (Exception e)
			{
				LOGGER.Info("Sleep interrupted " + e);
			}
			return Sharpen.Collections.ToArray(serials, new int[0]);
		}

		/// <summary>
		/// Initialise the Phidgets Library and report the library version in the
		/// RomRaider system log file.
		/// </summary>
		/// <remarks>
		/// Initialise the Phidgets Library and report the library version in the
		/// RomRaider system log file.
		/// </remarks>
		/// <exception cref="Com.Phidgets.PhidgetException">Com.Phidgets.PhidgetException</exception>
		public static void LoadIk()
		{
			try
			{
				ik = new InterfaceKitPhidget();
				LOGGER.Info(Com.Phidgets.Phidget.GetLibraryVersion());
			}
			catch (PhidgetException e)
			{
				LOGGER.Error("InterfaceKit error: " + e);
			}
		}

		/// <summary>
		/// For the serial number provided report the name of the
		/// associated PhidgetInterfaceKit.
		/// </summary>
		/// <remarks>
		/// For the serial number provided report the name of the
		/// associated PhidgetInterfaceKit.
		/// </remarks>
		/// <param name="serial">- the serial number previously discovered to be opened</param>
		/// <returns>a format string of the name and serial number</returns>
		/// <exception cref="Com.Phidgets.PhidgetException">Com.Phidgets.PhidgetException</exception>
		/// <exception cref="System.Exception">System.Exception</exception>
		public static string GetIkName(int serial)
		{
			string result = null;
			try
			{
				ik.Open(serial);
				WaitForAttached();
				try
				{
					if (Sharpen.Runtime.EqualsIgnoreCase(ik.GetDeviceType(), INTFKIT))
					{
						result = string.Format("%s serial: %d", ik.GetDeviceName(), serial);
					}
				}
				catch (PhidgetException e)
				{
					LOGGER.Error("InterfaceKit read device error: " + e);
				}
				finally
				{
					ik.Close();
				}
			}
			catch (PhidgetException e)
			{
				LOGGER.Error("InterfaceKit open serial error: " + e);
			}
			catch (Exception e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
			return result;
		}

		/// <summary>
		/// For the serial number provided create a Set of sensors found on the
		/// associated PhidgetInterfaceKit.
		/// </summary>
		/// <remarks>
		/// For the serial number provided create a Set of sensors found on the
		/// associated PhidgetInterfaceKit.
		/// </remarks>
		/// <param name="serial">- the serial number previously discovered to open</param>
		/// <returns>a Set of <b>IntfKitSensor</b></returns>
		/// <exception cref="Com.Phidgets.PhidgetException">Com.Phidgets.PhidgetException</exception>
		/// <exception cref="System.Exception">System.Exception</exception>
		public static ICollection<IntfKitSensor> GetSensors(int serial)
		{
			ICollection<IntfKitSensor> sensors = new HashSet<IntfKitSensor>();
			try
			{
				ik.Open(serial);
				WaitForAttached();
				try
				{
					if (ik.IsAttached())
					{
						if (Sharpen.Runtime.EqualsIgnoreCase(ik.GetDeviceType(), INTFKIT))
						{
							string result = string.Format("Plugin found: %s Serial: %d", ik.GetDeviceName(), 
								serial);
							LOGGER.Info(result);
							int inputCount = ik.GetSensorCount();
							for (int i = 0; i < inputCount; i++)
							{
								IntfKitSensor sensor = new IntfKitSensor();
								sensor.SetInputNumber(i);
								string inputName = string.Format("Sensor %d:%d", serial, i);
								sensor.SetInputName(inputName);
								sensor.SetUnits("raw value");
								sensor.SetMinValue(0);
								sensor.SetMaxValue(1000);
								sensors.AddItem(sensor);
							}
						}
						else
						{
							LOGGER.Info("No InterfaceKits attached");
						}
					}
					else
					{
						LOGGER.Info("No Phidget devices attached");
					}
				}
				catch (PhidgetException e)
				{
					LOGGER.Error("InterfaceKit read error: " + e);
				}
				finally
				{
					ik.Close();
				}
			}
			catch (PhidgetException e)
			{
				LOGGER.Error("InterfaceKit open error: " + e);
			}
			catch (Exception e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
			return sensors;
		}

		/// <summary>
		/// Wait for the Attach signal after opening the PhidgetInterfaceKit
		/// or a maximum timeout of 500msec.
		/// </summary>
		/// <remarks>
		/// Wait for the Attach signal after opening the PhidgetInterfaceKit
		/// or a maximum timeout of 500msec.
		/// </remarks>
		/// <exception cref="Com.Phidgets.PhidgetException">Com.Phidgets.PhidgetException</exception>
		/// <exception cref="System.Exception">System.Exception</exception>
		private static void WaitForAttached()
		{
			long timeout = Runtime.CurrentTimeMillis() + 500L;
			do
			{
				Sharpen.Thread.Sleep(50);
			}
			while (!ik.IsAttached() && (Runtime.CurrentTimeMillis() < timeout));
		}
	}
}
