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
using Javax.Swing;
using RomRaider.Logger.Ecu;
using RomRaider.Logger.External.Core;
using RomRaider.Logger.External.Innovate.Generic.Mts.IO;
using RomRaider.Logger.External.Innovate.Lm2.Mts.Plugin;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Innovate.Lm2.Mts.Plugin
{
	public sealed class Lm2MtsDataSource : ExternalDataSource
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(Lm2MtsDataSource));

		private readonly IDictionary<int, Lm2MtsDataItem> dataItems = new Dictionary<int, 
			Lm2MtsDataItem>();

		private MTSRunner runner;

		private int mtsPort = -1;

		// a default entry
		public string GetId()
		{
			return GetType().FullName;
		}

		public string GetName()
		{
			return "Innovate MTS";
		}

		public string GetVersion()
		{
			return "0.04";
		}

		public IList<ExternalDataItem> GetDataItems()
		{
			return Sharpen.Collections.UnmodifiableList(new AList<Lm2MtsDataItem>(dataItems.Values
				));
		}

		public Action GetMenuAction(EcuLogger logger)
		{
			return new Lm2MtsPluginMenuAction(logger, this);
		}

		public void SetPort(string port)
		{
			mtsPort = MtsPort(port);
		}

		public string GetPort()
		{
			return string.Empty + mtsPort;
		}

		public void Connect()
		{
			runner = new MTSRunner(mtsPort, dataItems);
			ThreadUtil.RunAsDaemon(runner);
		}

		public void Disconnect()
		{
			if (runner != null)
			{
				runner.Stop();
			}
		}

		private int MtsPort(string port)
		{
			try
			{
				return System.Convert.ToInt32(port);
			}
			catch (Exception)
			{
				LOGGER.Warn("Bad Innovate MTS port: " + port);
				return -1;
			}
		}

		public Lm2MtsDataSource()
		{
			{
				MTSConnector connector = new MTSConnector();
				int[] ports = connector.GetMtsPorts();
				if (ports != null)
				{
					for (int i = 0; i < ports.Length; i++)
					{
						connector.UsePort(i);
						ICollection<MTSSensor> sensors = connector.GetSensors();
						if (sensors.IsEmpty())
						{
							continue;
						}
						dataItems.Put(0, new Lm2MtsDataItem("LM-2", 0, "AFR", 9, 20));
						foreach (MTSSensor sensor in sensors)
						{
							dataItems.Put(sensor.GetInputNumber(), new Lm2MtsDataItem(sensor.GetDeviceName(), 
								sensor.GetDeviceChannel(), sensor.GetUnits(), sensor.GetMinValue(), sensor.GetMaxValue
								()));
						}
					}
				}
				else
				{
					throw new InvalidOperationException("Innovate LogWorks MTS control does not appear to be installed on this computer"
						);
				}
				connector.Dispose();
			}
		}
	}
}
