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
	public class Test : MTSEvents
	{
		private MTS mts;

		private EventCookie connectionEventCookie;

		private int count = 0;

		public Test()
		{
			mts = null;
			connectionEventCookie = null;
		}

		private void PrintAvailableInputs()
		{
			System.Console.Out.WriteLine("Available inputs:");
			for (int idx = 0; idx < mts.InputCount(); ++idx)
			{
				mts.CurrentInput(idx);
				string inputName = mts.InputName();
				int inputType = mts.InputType();
				System.Console.Out.Printf("inputName:%s, inputType:%d%n", inputName, inputType);
			}
		}

		/// <exception cref="System.Exception"></exception>
		private void WaitFor(int milliseconds)
		{
			lock (this)
			{
				Sharpen.Runtime.Wait(this, milliseconds);
			}
		}

		/// <exception cref="System.Exception"></exception>
		private void GetSamples(int numberOfSamples)
		{
			// note:
			// it sounds like the SDK allows up to 12.21 samples per second, which
			// should be more than sufficient for RomRaider
			System.Console.Out.WriteLine("Getting data samples:");
			// give the device time to start the acquisition of data
			WaitFor(1000);
			for (int sampleCount = 0; sampleCount < numberOfSamples; ++sampleCount)
			{
				int data = mts.InputSample();
				int function = mts.InputFunction();
				// note if the sample data is lambda, you might need to multiply by 14.7 or inputAFRMultiplier()
				System.Console.Out.Printf("\tSample %d: data = %d, function = %d%n", sampleCount 
					+ 1, data, function);
				// wait 200 milliseconds before grabbing the next sample
				WaitFor(200);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Run()
		{
			// create an instance of the MTSSDK COM component
			mts = MTSFactory.CreateMTS();
			// Note: you MUST call portCount() at least one before attempting to set
			// the current inputPort or the inputPort(int) will not do anything!
			int portCount = mts.PortCount();
			System.Console.Out.Printf("Found %d ports.%n", portCount);
			// register for MTS component events
			connectionEventCookie = mts.Advise<MTSEvents>(this);
			// set the current port before attempting to connect
			for (int i = 0; i < portCount; i++)
			{
				mts.CurrentPort(i);
				string portName = mts.PortName();
				System.Console.Out.Printf("Current MTS port is %d; port name = %s%n", i, portName
					);
				// attempt to connect to the specified device
				System.Console.Out.WriteLine("connect() called.");
				mts.Connect();
				// show available inputs
				PrintAvailableInputs();
				mts.Disconnect();
			}
			// attempt to get data
			//        mts.currentInput(0);
			mts.CurrentPort(0);
			mts.Connect();
			mts.StartData();
			// notes:
			// the inputFunction() method retrieves the meaning of the sample - this can be lambda/AFR,
			// error code, etc.  The inputSample() method retrieves the current data.
			// for instance, running this without having the O2 sensor plugged in will cause inputFunction() to
			// return 6 and inputSample() to return 9.
			// see the LM-2 manual for more information on error codes.
			//
			// list of function codes:
			//
			// MTS_FUNC_LAMBDA 0
			// MTS_FUNC_O2 1
			// MTS_FUNC_INCALIB 2
			// MTS_FUNC_RQCALIB 3
			// MTS_FUNC_WARMUP 4
			// MTS_FUNC_HTRCAL 5
			// MTS_FUNC_ERROR 6
			// MTS_FUNC_FLASHLEV 7
			// MTS_FUNC_SERMODE 8
			// MTS_FUNC_NOTLAMBDA 9
			// MTS_FUNC_INVALID 10
			// retrieve 10 samples
			//        getSamples(10);
			WaitFor(60000);
			// dispose of the event handler instance
			connectionEventCookie.Close();
			// close the connection to the device
			System.Console.Out.WriteLine("disconnect() called.");
			mts.Disconnect();
			// release COM resources
			mts.Dispose();
		}

		// generated in response to a call to connect()
		// see the SDK doc for explanation of error codes
		public virtual void ConnectionEvent(int result)
		{
			System.Console.Out.Printf("connectionEvent raised.  result = %d%n", result);
		}

		public virtual void ConnectionError()
		{
		}

		// occurs when there is an error in the data stream (i.e. I assume connection lost, protocol error)
		public virtual void NewData()
		{
			int i;
			float data = 0f;
			for (i = 0; i < mts.InputCount(); i++)
			{
				//            i=0;
				mts.CurrentInput(i);
				float min = mts.InputMinValue();
				float max = mts.InputMaxValue();
				float sample = mts.InputSample();
				data = ((max - min) * ((float)sample / 1024f)) + min;
				//            float multiplier = mts.inputAFRMultiplier();
				//            int sampleMeaning = mts.inputFunction();
				string str = string.Format("%d, InputNo: %02d, InputName: %s, InputType: %d, DeviceName: %s, DeviceType: %d, DeviceChannel: %d, Units: %s, Multiplier: %f, MinValue: %f, MaxValue: %f, Sample: %f, Data: %f"
					, count, i, mts.InputName(), mts.InputType(), mts.InputDeviceName(), mts.InputDeviceType
					(), mts.InputDeviceChannel(), mts.InputUnit(), mts.InputAFRMultiplier(), mts.InputMinValue
					(), mts.InputMaxValue(), sample, data);
				System.Console.Out.Printf("%s%n", str);
				//            System.out.printf("newData raised for Input: %02d, Function: %d, Data: %f,\tMultiplier: %f%n", i, sampleMeaning, data, multiplier);
				count++;
			}
		}

		/// <param name="args"></param>
		/// <exception cref="System.Exception">System.Exception</exception>
		public static void Main(string[] args)
		{
			RomRaider.Logger.External.Innovate.Generic.Mts.IO.Test test = new RomRaider.Logger.External.Innovate.Generic.Mts.IO.Test
				();
			test.Run();
		}
	}
}
