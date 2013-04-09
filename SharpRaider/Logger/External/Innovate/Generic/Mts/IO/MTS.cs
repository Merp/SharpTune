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

using Com4j;
using RomRaider.Logger.External.Innovate.Generic.Mts.IO;
using Sharpen;

namespace RomRaider.Logger.External.Innovate.Generic.Mts.IO
{
	public interface MTS : Com4jObject
	{
		/// <summary>Get number of MTS ports available</summary>
		int PortCount();

		/// <summary>CurrentMTSPort</summary>
		int CurrentPort();

		/// <summary>CurrentMTSPort</summary>
		void CurrentPort(int pVal);

		/// <summary>MTS Port Name</summary>
		string PortName();

		/// <summary>Attempt an MTS Connection</summary>
		void Connect();

		/// <summary>Disconnect MTS Port</summary>
		void Disconnect();

		/// <summary>Number of MTS Inputs</summary>
		int InputCount();

		/// <summary>Current MTS Input</summary>
		int CurrentInput();

		/// <summary>Current MTS Input</summary>
		void CurrentInput(int pVal);

		/// <summary>Name of Current Input</summary>
		string InputName();

		/// <summary>Units used by Input</summary>
		string InputUnit();

		/// <summary>Name of Device Providing Input</summary>
		string InputDeviceName();

		/// <summary>Type of Device Providing Input</summary>
		int InputDeviceType();

		/// <summary>Type of Input</summary>
		int InputType();

		/// <summary>Channel on Device providing Input</summary>
		int InputDeviceChannel();

		/// <summary>AFR Multiplier for Input (if used)</summary>
		float InputAFRMultiplier();

		/// <summary>Minimum Value (units) for Input</summary>
		float InputMinValue();

		/// <summary>Max value (in units) for Input</summary>
		float InputMaxValue();

		/// <summary>Voltage equivelent to Input Min Value</summary>
		float InputMinVolt();

		/// <summary>Voltage equivelent to Input Max Value</summary>
		float InputMaxVolt();

		/// <summary>Raw Sample (0-1023) for Input</summary>
		int InputSample();

		/// <summary>Status and Function of Input Sample</summary>
		int InputFunction();

		/// <summary>Call to start NewData events from MTS Connection</summary>
		void StartData();
	}
}
