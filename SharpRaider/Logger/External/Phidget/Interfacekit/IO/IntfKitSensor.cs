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

using Sharpen;

namespace RomRaider.Logger.External.Phidget.Interfacekit.IO
{
	/// <summary>
	/// IntfKitSensor contains all the relevant information about a sensor as
	/// reported from information gathered from the Phidget device.
	/// </summary>
	/// <remarks>
	/// IntfKitSensor contains all the relevant information about a sensor as
	/// reported from information gathered from the Phidget device.  An IntfKitSensor
	/// is created for each input found on the Phidget device.
	/// </remarks>
	public sealed class IntfKitSensor
	{
		private int inputNumber;

		private string inputName;

		private string units;

		private float minValue;

		private float maxValue;

		/// <summary>Create an IntfKitSensor with all fields set to type default values.</summary>
		/// <remarks>Create an IntfKitSensor with all fields set to type default values.</remarks>
		public IntfKitSensor()
		{
		}

		public int GetInputNumber()
		{
			return inputNumber;
		}

		public void SetInputNumber(int input)
		{
			inputNumber = input;
		}

		public string GetInputName()
		{
			return inputName;
		}

		public void SetInputName(string name)
		{
			inputName = name;
		}

		public string GetUnits()
		{
			return units;
		}

		public void SetUnits(string unit)
		{
			units = unit;
		}

		public float GetMinValue()
		{
			return minValue;
		}

		public void SetMinValue(float value)
		{
			minValue = value;
		}

		public float GetMaxValue()
		{
			return maxValue;
		}

		public void SetMaxValue(float value)
		{
			maxValue = value;
		}
	}
}
