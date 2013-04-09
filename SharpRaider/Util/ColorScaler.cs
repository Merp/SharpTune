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

using Java.Awt;
using RomRaider;
using Sharpen;

namespace RomRaider.Util
{
	public sealed class ColorScaler
	{
		public ColorScaler()
		{
		}

		public static Color GetScaledColor(double scale, Settings settings)
		{
			Color minColor = settings.GetMinColor();
			Color maxColor = settings.GetMaxColor();
			float[] minColorHSB = new float[3];
			float[] maxColorHSB = new float[3];
			RgbToHsb(minColor, minColorHSB);
			RgbToHsb(maxColor, maxColorHSB);
			float h = minColorHSB[0] + (maxColorHSB[0] - minColorHSB[0]) * (float)scale;
			float s = minColorHSB[1] + (maxColorHSB[1] - minColorHSB[1]) * (float)scale;
			float b = minColorHSB[2] + (maxColorHSB[2] - minColorHSB[2]) * (float)scale;
			return Color.GetHSBColor(h, s, b);
		}

		private static void RgbToHsb(Color color, float[] colorHSB)
		{
			Color.RGBtoHSB(color.GetRed(), color.GetGreen(), color.GetBlue(), colorHSB);
		}
	}
}
