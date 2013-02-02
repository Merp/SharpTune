/*
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;
using SharpTuneCore;

namespace SharpTune
{
    /// <summary>
    /// Container for a sharptune instance
    /// </summary>
    public static class SharpTuner
    {
        public static DeviceImage activeImage { get; set; }

        public static List<DeviceImage> imageList { get; set; }

        public static AvailableDevices availableDevices { get; set; }

        public static string definitionPath = "rommetadata";

        public static string activePort { get; set; }

        public static SerialPort Port { get; set; }

        //public static SsmInterface ssmInterface { get; set; }

        public static bool fileQueued { get; set; }

        public static string QueuedFilePath { get; set; }

        public static void populateAvailableDevices()
        {
            availableDevices = new AvailableDevices(definitionPath.ToString());
        }

        //public static void setSsmInterface(SsmInterface s)
        //{
        //    ssmInterface = s;
        //}

        public static void AddImage(DeviceImage d)
        {
            imageList.Add(d);
        }
    }
}
