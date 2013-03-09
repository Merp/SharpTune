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
using RomModCore;
using SharpTune.Properties;
using System.Resources;
using System.Collections;

namespace SharpTune
{
    /// <summary>
    /// Container for a sharptune instance
    /// </summary>
    public static class SharpTuner
    {
        public static MainWindow mainWindow { get; set; }

        public static DeviceImage activeImage { get; set; }

        public static List<DeviceImage> imageList { get; set; }

        public static AvailableDevices availableDevices { get; set; }

        public static string DefRepoPath;
        public static string EcuFlashDefRepoPath;
        public static string RRDefRepoPath;
        public static string RREcuDefPath;
        public static string RRLoggerDefPath;

        public static string activePort { get; set; }

        public static SerialPort Port { get; set; }

        //public static SsmInterface ssmInterface { get; set; }

        public static bool fileQueued { get; set; }

        public static string QueuedFilePath { get; set; }

        public static void initSettings()
        {
            if (Settings.Default.SubaruDefsRepoPath == null | Settings.Default.SubaruDefsRepoPath == "")
            {
                Settings.Default.SubaruDefsRepoPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Dev\SubaruDefs";
            }
            if (Settings.Default.PatchPath == null || Settings.Default.PatchPath == "")
            {
                Settings.Default.PatchPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Dev\MerpMod\MerpMod\TestRom";
            }

            EcuFlashDefRepoPath = Settings.Default.SubaruDefsRepoPath + @"\ECUFlash\subaru standard";
            RRDefRepoPath = Settings.Default.SubaruDefsRepoPath + @"\RomRaider";
            RREcuDefPath = RRDefRepoPath + @"\ecu\standard\";
            RRLoggerDefPath = RRDefRepoPath + @"\logger\";
            Settings.Default.Save();
        }

        public static void populateAvailableDevices()
        {
            availableDevices = new AvailableDevices(EcuFlashDefRepoPath.ToString());
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
