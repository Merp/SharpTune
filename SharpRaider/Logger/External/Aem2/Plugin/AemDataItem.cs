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

using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.External.Core;
using Sharpen;

namespace RomRaider.Logger.External.Aem2.Plugin
{
	public sealed class AemDataItem : ExternalDataItem, DataListener
	{
		private EcuDataConvertor[] convertors;

		private double data;

		public AemDataItem(params IExternalSensorConversion[] convertorList) : base()
		{
			convertors = new EcuDataConvertor[convertorList.Length];
			convertors = ExternalDataConvertorLoader.LoadConvertors(this, convertors, convertorList
				);
		}

		public string GetName()
		{
			return "AEM UEGO Wideband [19200 baud]";
		}

		public string GetDescription()
		{
			return "AEM UEGO Wideband data";
		}

		public double GetData()
		{
			return data;
		}

		public void SetData(double data)
		{
			this.data = data;
		}

		public EcuDataConvertor[] GetConvertors()
		{
			return convertors;
		}
	}
}