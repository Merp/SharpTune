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
using RomRaider.IO.Serial.Connection;
using RomRaider.Logger.External.Aem.IO;
using RomRaider.Logger.External.Aem.Plugin;
using RomRaider.Logger.External.Core;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Aem.IO
{
	public sealed class AemRunner : Stoppable
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(RomRaider.Logger.External.Aem.IO.AemRunner));

		private static readonly AemConnectionProperties CONNECTION_PROPS = new AemConnectionProperties
			();

		private readonly SerialConnection connection;

		private readonly AemDataItem dataItem;

		private bool stop;

		public AemRunner(string port, AemDataItem dataItem)
		{
			this.connection = new SerialConnectionImpl(port, CONNECTION_PROPS);
			this.dataItem = dataItem;
		}

		public void Run()
		{
			try
			{
				while (!stop)
				{
					string response = connection.ReadLine();
					LOGGER.Trace("AEM UEGO AFR Response: " + response);
					if (!ParamChecker.IsNullOrEmpty(response))
					{
						dataItem.SetData(ParseDouble(response));
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
				string[] substr = value.Split("\t");
				double result = double.ParseDouble(substr[0]);
				return result;
			}
			catch (Exception)
			{
				return 0.0;
			}
		}
	}
}
