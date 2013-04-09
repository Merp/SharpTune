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

using RomRaider.Logger.External.Fourteenpoint7.Plugin;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.External.Fourteenpoint7.Plugin
{
	public sealed class NawConvertorImpl : NawConvertor
	{
		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.GetLogger
			(typeof(NawConvertorImpl));

		public double Convert(byte[] bytes)
		{
			int temp = 0;
			double lambda = 0.0;
			double unCalIp = 0.0;
			//        float facpc = asFloat(bytes, 0, 4);
			float upc = ByteUtil.AsFloat(bytes, 4, 4);
			unCalIp = upc;
			// uncalibrated pump current cast to double
			if (unCalIp > 128)
			{
				// Lean
				double o2conc = (-0.00000359 * unCalIp * unCalIp) + (0.003894 * unCalIp) - 0.4398;
				if (o2conc > 0.210)
				{
					// O2 concentration 20.9% this is equal to 207 lambda
					o2conc = 0.210;
				}
				lambda = ((o2conc / 3) + 1) / (1 - (4.76 * o2conc));
				if (lambda >= 1.43)
				{
					lambda = 1.43;
				}
			}
			else
			{
				// Rich
				lambda = ((0.00003453 * unCalIp * unCalIp) - (0.00159 * unCalIp) + 0.6368);
			}
			temp = (unchecked((int)(0x000000FF)) & ((int)bytes[8]));
			// convert signed byte to unsigned byte
			if (temp < 11)
			{
				// check sensor temperature range for valid reading
				lambda = 99.99;
			}
			else
			{
				// sensor too hot
				if (temp > 19)
				{
					lambda = -99.99;
				}
			}
			// sensor too cold
			temp = 1500 / temp;
			// temperature in percent, 100% is best
			LOGGER.Trace("Converting NAW_7S response: " + HexUtil.AsHex(bytes) + " --> Ip:" +
				 unCalIp + " --> temp:" + temp + "% --> lambda:" + lambda);
			return lambda;
		}
	}
}
