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

using RomRaider.Logger.External.Innovate.Generic.Mts.IO;
using Sharpen;

namespace RomRaider.Logger.External.Innovate.Generic.Mts.IO
{
	public class MTSSensor
	{
		private int inputNumber = 0;

		private string inputName = string.Empty;

		private string deviceName = string.Empty;

		private int deviceChannel = 0;

		private string units = string.Empty;

		private float minValue = 0f;

		private float maxValue = 0f;

		/// <summary>
		/// MTSSensor contains all the relevant information about a sensor as
		/// reported from information gathered from the MTS stream.
		/// </summary>
		/// <remarks>
		/// MTSSensor contains all the relevant information about a sensor as
		/// reported from information gathered from the MTS stream.  A MTSSensor is
		/// created for each input found in the MTS stream.
		/// </remarks>
		public MTSSensor()
		{
		}

		public enum MTSSensorInputType
		{
			MTS_TYPE_LAMBDA,
			MTS_TYPE_AFR,
			MTS_TYPE_VDC
		}

		public static MTSSensor.MTSSensorInputType ValueOfMSIT(int inputType)
		{
			foreach (MTSSensor.MTSSensorInputType type in MTSSensor.MTSSensorInputType.Values
				())
			{
				if (type.inputType == inputType)
				{
					return type;
				}
			}
			return null;
		}

		public enum MTSSensorInputFunction
		{
			MTS_FUNC_LAMBDA,
			MTS_FUNC_O2,
			MTS_FUNC_INCALIB,
			MTS_FUNC_RQCALIB,
			MTS_FUNC_WARMUP,
			MTS_FUNC_HTRCAL,
			MTS_FUNC_ERROR,
			MTS_FUNC_FLASHLEV,
			MTS_FUNC_SERMODE,
			MTS_FUNC_NOTLAMBDA,
			MTS_FUNC_INVALID
		}

		public static MTSSensor.MTSSensorInputFunction ValueOfMSIF(int function)
		{
			foreach (MTSSensor.MTSSensorInputFunction type in MTSSensor.MTSSensorInputFunction
				.Values())
			{
				if (type.function == function)
				{
					return type;
				}
			}
			return MTSSensor.MTSSensorInputFunction.MTS_FUNC_INVALID;
		}

		public virtual int GetInputNumber()
		{
			return inputNumber;
		}

		public virtual void SetInputNumber(int input)
		{
			inputNumber = input;
		}

		public virtual string GetInputName()
		{
			return inputName;
		}

		public virtual void SetInputName(string name)
		{
			inputName = name;
		}

		public virtual string GetDeviceName()
		{
			return deviceName;
		}

		public virtual void SetDeviceName(string name)
		{
			deviceName = name;
		}

		public virtual int GetDeviceChannel()
		{
			return deviceChannel;
		}

		public virtual void SetDeviceChannel(int channel)
		{
			deviceChannel = channel;
		}

		public virtual string GetUnits()
		{
			return units;
		}

		public virtual void SetUnits(string unit)
		{
			units = unit;
		}

		public virtual float GetMinValue()
		{
			return minValue;
		}

		public virtual void SetMinValue(float value)
		{
			minValue = value;
		}

		public virtual float GetMaxValue()
		{
			return maxValue;
		}

		public virtual void SetMaxValue(float value)
		{
			maxValue = value;
		}
	}
}
