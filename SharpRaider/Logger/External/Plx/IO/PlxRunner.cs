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
using RomRaider.Logger.External.Core;
using RomRaider.Logger.External.Plx.IO;
using RomRaider.Logger.External.Plx.Plugin;
using Sharpen;

namespace RomRaider.Logger.External.Plx.IO
{
	public sealed class PlxRunner : Stoppable
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(RomRaider.Logger.External.Plx.IO.PlxRunner));

		private readonly IDictionary<PlxSensorType, PlxDataItem> dataItems;

		private readonly PlxConnection connection;

		private bool stop;

		public PlxRunner(string port, IDictionary<PlxSensorType, PlxDataItem> dataItems)
		{
			this.connection = new PlxConnectionImpl(port);
			this.dataItems = dataItems;
		}

		public void Run()
		{
			try
			{
				PlxParser parser = new PlxParserImpl();
				while (!stop)
				{
					byte b = connection.ReadByte();
					PlxResponse response = parser.PushByte(b);
					if (!IsValid(response))
					{
						continue;
					}
					PlxDataItem item = dataItems.Get(response.sensor);
					if (item != null && (response.instance == item.GetInstance()))
					{
						item.SetRaw(response.value);
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

		private bool IsValid(PlxResponse response)
		{
			if (response == null)
			{
				return false;
			}
			return response.sensor != PlxSensorType.UNKNOWN;
		}
	}
}
