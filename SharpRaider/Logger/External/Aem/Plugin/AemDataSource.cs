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
using RomRaider.Logger.External.Aem.IO;
using RomRaider.Logger.External.Aem.Plugin;
using RomRaider.Logger.External.Core;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Aem.Plugin
{
	public sealed class AemDataSource : ExternalDataSource
	{
		private AemDataItem dataItem = new AemDataItem(SensorConversionsAFR.AFR_147, SensorConversionsAFR
			.LAMBDA, SensorConversionsAFR.AFR_90, SensorConversionsAFR.AFR_146, SensorConversionsAFR
			.AFR_64, SensorConversionsAFR.AFR_155, SensorConversionsAFR.AFR_172, SensorConversionsAFR
			.AFR_34);

		private AemRunner runner;

		private string port;

		public string GetId()
		{
			return GetType().FullName;
		}

		public string GetName()
		{
			return "AEM UEGO AFR [9600 baud]";
		}

		public string GetVersion()
		{
			return "0.05";
		}

		public IList<ExternalDataItem> GetDataItems()
		{
			return Arrays.AsList(dataItem);
		}

		public Action GetMenuAction(EcuLogger logger)
		{
			return null;
		}

		public void SetPort(string port)
		{
			this.port = port;
		}

		public string GetPort()
		{
			return port;
		}

		public void Connect()
		{
			runner = new AemRunner(port, dataItem);
			ThreadUtil.RunAsDaemon(runner);
		}

		public void Disconnect()
		{
			if (runner != null)
			{
				runner.Stop();
			}
		}
	}
}
