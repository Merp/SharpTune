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
using Com4j;
using RomRaider.Logger.External.Innovate.Generic.Mts.IO;
using Sharpen;

namespace RomRaider.Logger.External.Innovate.Generic.Mts.IO
{
	public sealed class MTSConnector
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(RomRaider.Logger.External.Innovate.Generic.Mts.IO.MTSConnector));

		private static MTS mts;

		private static int[] ports;

		/// <summary>
		/// MTS Connector is a set of methods to create the MTS connection,
		/// retrieve a set of available ports and the sensor inputs available
		/// across all the found ports.
		/// </summary>
		/// <remarks>
		/// MTS Connector is a set of methods to create the MTS connection,
		/// retrieve a set of available ports and the sensor inputs available
		/// across all the found ports.
		/// </remarks>
		public MTSConnector()
		{
			{
				CreateMts();
			}
			try
			{
				SetMtsPorts();
			}
			catch (ArgumentNullException)
			{
			}
		}

		public MTSConnector(int mtsPort)
		{
			{
				CreateMts();
			}
			if (mtsPort != -1)
			{
				Mts(mtsPort);
			}
		}

		public MTS GetMts()
		{
			return mts;
		}

		public int[] GetMtsPorts()
		{
			return ports;
		}

		public void UsePort(int mtsPort)
		{
			Mts(mtsPort);
		}

		public void Dispose()
		{
			mts.Disconnect();
			mts.Dispose();
		}

		private void CreateMts()
		{
			// create mts interface
			try
			{
				mts = MTSFactory.CreateMTS();
				mts.Disconnect();
			}
			catch (ExecutionException e)
			{
				LOGGER.Error("COM4J error creating MTS interface: " + e);
			}
		}

		private void SetMtsPorts()
		{
			try
			{
				// check there are ports available
				int portCount = mts.PortCount();
				if (portCount <= 0)
				{
					throw new InvalidOperationException("No Innovate MTS ports found");
				}
				ports = new int[portCount];
				string names = string.Empty;
				for (int i = 0; i < portCount; i++)
				{
					ports[i] = i;
					mts.CurrentPort(i);
					names = names + " " + mts.PortName();
				}
				LOGGER.Info("Innovate MTS: found " + portCount + " ports," + names);
			}
			catch (RuntimeException t)
			{
				// cleanup mts and rethrow exception
				if (mts != null)
				{
					mts.Dispose();
				}
				throw;
			}
		}

		public void Mts(int mtsPort)
		{
			// bail out early if we know specified mts port is invalid
			if (mtsPort < 0)
			{
				throw new ArgumentException("Bad Innovate MTS port: " + mtsPort);
			}
			try
			{
				int portCount = mts.PortCount();
				if (portCount <= 0)
				{
					throw new InvalidOperationException("No Innovate MTS ports found");
				}
				// select the specified port
				mts.CurrentPort(mtsPort);
				string portName = mts.PortName();
				LOGGER.Info("Innovate MTS: current port [" + mtsPort + "]: " + portName);
			}
			catch (RuntimeException t)
			{
				// cleanup mts and rethrow exception
				if (mts != null)
				{
					mts.Dispose();
				}
				throw;
			}
		}

		public ICollection<MTSSensor> GetSensors()
		{
			ICollection<MTSSensor> sensors = new HashSet<MTSSensor>();
			try
			{
				// attempt to connect to the specified device
				mts.Connect();
				try
				{
					// get a count of available inputs
					int inputCount = mts.InputCount();
					LOGGER.Info("Innovate MTS: found " + inputCount + " inputs.");
					if (inputCount > 0)
					{
						for (int i = 0; i < inputCount; i++)
						{
							// report each input found
							mts.CurrentInput(i);
							MTSSensor sensor = new MTSSensor();
							sensor.SetInputNumber(i);
							sensor.SetInputName(mts.InputName());
							sensor.SetDeviceName(mts.InputDeviceName());
							sensor.SetDeviceChannel(mts.InputDeviceChannel());
							sensor.SetUnits(mts.InputUnit());
							sensor.SetMinValue(mts.InputMinValue());
							sensor.SetMaxValue(mts.InputMaxValue());
							sensors.AddItem(sensor);
							LOGGER.Debug(string.Format("Innovate MTS: InputNo: %02d, InputName: %s, InputType: %d, DeviceName: %s, DeviceType: %d, DeviceChannel: %d, Units: %s, Multiplier: %f, MinValue: %f, MaxValue: %f"
								, i, mts.InputName(), mts.InputType(), mts.InputDeviceName(), mts.InputDeviceType
								(), mts.InputDeviceChannel(), mts.InputUnit(), mts.InputAFRMultiplier(), mts.InputMinValue
								(), mts.InputMaxValue()));
						}
					}
					else
					{
						LOGGER.Error("Innovate MTS: Error - no input channels found to log from");
					}
				}
				finally
				{
					mts.Disconnect();
				}
			}
			finally
			{
			}
			return sensors;
		}
	}
}
