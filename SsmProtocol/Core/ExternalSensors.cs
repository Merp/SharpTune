using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NSFW.PlxSensors;

namespace NateW.Ssm
{
    public class ExternalSensors
    {
        /// <summary>
        /// Port name to disable an external sensor.
        /// </summary>
        public const string NullSerialPortName = "None";

        /// <summary>
        /// This class is singleton.
        /// </summary>
        private static ExternalSensors instance;

        /// <summary>
        /// PLX sensor API.
        /// </summary>
        private PlxSensors plxSensors;

        /// <summary>
        /// PLX sensor API.
        /// </summary>
        public PlxSensors PlxSensors { get { return this.plxSensors; } }

        /// <summary>
        /// Private constructor, use factory instead
        /// </summary>
        private ExternalSensors()
        {
        }

        /// <summary>
        /// Factory method.
        /// </summary>
        public static ExternalSensors GetInstance()
        {
            if (instance == null)
            {
                instance = new ExternalSensors();
            }

            return instance;
        }

        /// <summary>
        /// Sets the PLX serial port, and begins reading data.
        /// </summary>
        public void SetPlxSerialPort(string portName)
        {
            if (portName == NullSerialPortName)
            {
                Trace.WriteLine("ExternalSensors.SetPlxSerialPort: releasing port.");
                if (this.plxSensors != null)
                {
                    this.plxSensors.Close();
                    this.plxSensors = null;
                }
            }
            else
            {
                Trace.WriteLine("ExternalSensors.SetPlxSerialPort: creating with port: " + portName);
                this.plxSensors = PlxSensors.GetInstance(portName);
            }
        }
    }
}
