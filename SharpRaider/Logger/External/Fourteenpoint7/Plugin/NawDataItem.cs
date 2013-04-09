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
using RomRaider.Logger.External.Fourteenpoint7.Plugin;
using Sharpen;

namespace RomRaider.Logger.External.Fourteenpoint7.Plugin
{
	public sealed class NawDataItem : ExternalDataItem, RawDataListener
	{
		private readonly NawConvertor convertor = new NawConvertorImpl();

		private EcuDataConvertor[] convertors;

		private byte[] bytes;

		public NawDataItem(params IExternalSensorConversion[] convertorList) : base()
		{
			convertors = new EcuDataConvertor[convertorList.Length];
			convertors = ExternalDataConvertorLoader.LoadConvertors(this, convertors, convertorList
				);
		}

		public string GetName()
		{
			return "14Point7 NAW_7S UEGO";
		}

		public string GetDescription()
		{
			return "14Point7 NAW_7S Wideband data";
		}

		public double GetData()
		{
			if (bytes == null)
			{
				return 0.0;
			}
			return convertor.Convert(bytes);
		}

		public void SetBytes(byte[] bytes)
		{
			this.bytes = bytes;
		}

		public EcuDataConvertor[] GetConvertors()
		{
			return convertors;
		}
	}
}
