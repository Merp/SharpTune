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
using RomRaider.IO.Connection;
using RomRaider.IO.Serial.Connection;
using RomRaider.Logger.External.Core;
using RomRaider.Logger.External.Mrf.IO;
using RomRaider.Logger.External.Mrf.Plugin;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Mrf.IO
{
	public sealed class MrfRunner : Stoppable
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(RomRaider.Logger.External.Mrf.IO.MrfRunner));

		private static readonly ConnectionProperties CONNECTION_PROPS = new MrfConnectionProperties
			();

		private readonly IDictionary<MrfSensor.MrfSensorType, MrfDataItem> dataItems;

		private readonly SerialConnection connection;

		private bool stop;

		public MrfRunner(string port, IDictionary<MrfSensor.MrfSensorType, MrfDataItem> dataItems
			)
		{
			this.connection = new SerialConnectionImpl(port, CONNECTION_PROPS);
			this.dataItems = dataItems;
		}

		public void Run()
		{
			try
			{
				while (!stop)
				{
					string response = connection.ReadLine();
					if (ParamChecker.IsNullOrEmpty(response))
					{
						continue;
					}
					LOGGER.Trace("MRF Stealth Gauge Response: " + response);
					string[] values = response.Split(",");
					for (int i = 0; i < values.Length; i++)
					{
						MrfDataItem dataItem = dataItems.Get(MrfSensor.TypeOf(i));
						if (dataItem != null)
						{
							dataItem.SetData(ParseDouble(values[i]));
						}
					}
				}
				connection.Close();
			}
			catch (Exception t)
			{
				LOGGER.Error("Error occurred", t);
			}
			finally
			{
				connection.Close();
			}
		}

		public void Stop()
		{
			stop = true;
		}

		private double ParseDouble(string value)
		{
			try
			{
				return double.ParseDouble(value);
			}
			catch (Exception)
			{
				return 0.0;
			}
		}
	}
}
