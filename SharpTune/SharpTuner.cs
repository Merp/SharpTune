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
using SharpTune.RomMod;
using SharpTune.Properties;
using System.Resources;
using System.Collections;
using System.Threading;
using System.Diagnostics;

namespace SharpTune
{
    /// <summary>
    /// Container for a sharptune instance
    /// </summary>
    public static class SharpTuner
    {
        public const string GitHelpUrl = "http://github.com/Merp/SubaruDefs/tree/Alpha";

        public static MainWindow Window { get; set; }

        private static DeviceImage actImg;

        public static DeviceImage ActiveImage 
        {
            get { return actImg; }
            set
            {
                actImg = value;
                //TODO update the mainwindow!!!
                Window.Refresh();
            }
        }

        public static List<DeviceImage> ImageList { get; set; }

        public static AvailableDevices AvailableDevices { get; set; }

        public static List<Mod> AvailableMods { get; private set; }

        public static List<Scaling> DataScalings {get; set; }
        public static List<Scaling> UnitScalings { get; set; }

        public static string DefRepoPath;
        public static string EcuFlashDefRepoPath;
        public static string RRDefRepoPath;
        public static string RREcuDefPath;
        public static string RRLoggerDefPath;

        public static string ActivePort { get; set; }

        public static SerialPort Port { get; set; }

        //public static SsmInterface ssmInterface { get; set; }

        public static bool fileQueued { get; set; }

        public static string QueuedFilePath { get; set; }

        static SharpTuner(){}

        public static void Init()
        {
            InitSettings();
            InitTraces();
            Trace.WriteLine("<--- Initializing SharpTuner --->");
            ImageList = new List<DeviceImage>();
            DataScalings = new List<Scaling>();
            UnitScalings = new List<Scaling>();
            PopulateAvailableDevices();
            LoadMods();
            Trace.WriteLine("<--Finished Initializing SharpTuner --->");
        }

        public static void InitSettings()
        {
            if (Settings.Default.SubaruDefsRepoPath == null | Settings.Default.SubaruDefsRepoPath == "")
            {
                Settings.Default.SubaruDefsRepoPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Dev\SubaruDefs";
            }
            if (Settings.Default.PatchPath == null || Settings.Default.PatchPath == "")
            {
                Settings.Default.PatchPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Dev\MerpMod\MerpMod\TestRom";
            }
            if (Settings.Default.LogFilePath == null || Settings.Default.LogFilePath == "")
            {
                Settings.Default.LogFilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\SharpTune.log";
            }

            EcuFlashDefRepoPath = Settings.Default.SubaruDefsRepoPath + @"\ECUFlash\subaru standard";//TODO support metric
            RRDefRepoPath = Settings.Default.SubaruDefsRepoPath + @"\RomRaider";
            RREcuDefPath = RRDefRepoPath + @"\ecu\standard\";
            RRLoggerDefPath = RRDefRepoPath + @"\logger\";
            Settings.Default.Save();
        }

        public static void PopulateAvailableDevices()
        {
            AvailableDevices = new AvailableDevices(EcuFlashDefRepoPath.ToString());
        }

        //public static void setSsmInterface(SsmInterface s)
        //{
        //    ssmInterface = s;
        //}

        public static void AddImage(DeviceImage d)
        {
            ImageList.Add(d);
            ActiveImage = d;
        }

        public static void LoadMods()
        {
            AvailableMods = new List<Mod>();
            LoadResourceMods();
            LoadExternalMods();
        }

        private static void LoadResourceMods()
        {
            int i = AvailableMods.Count;
            ResourceSet ress = Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);
            ResourceManager rm = SharpTune.Properties.Resources.ResourceManager;
            foreach (DictionaryEntry r in ress)
            {
                MemoryStream stream = new MemoryStream((byte[])rm.GetObject(r.Key.ToString()));
                //if (tempMod.TryCheckApplyMod(FilePath, FilePath + ".temp", 2, false))
                AvailableMods.Add(new Mod(stream, r.Key.ToString()));
            }
        }

        private static void LoadExternalMods()
        {
            int i = AvailableMods.Count;
            string[] terms = { ".patch" };
            List<string> searchresults = ResourceUtil.directorySearchRecursive(Settings.Default.PatchPath, terms);
            if (searchresults == null)
            {
                Trace.WriteLine("No External Mods found");
            }
            else
            {
                foreach (string modpath in searchresults)
                {
                    if (!modpath.ContainsCI("debug") && !modpath.ContainsCI("currentbuild"))
                    {
                        AvailableMods.Add(new Mod(modpath));
                    }
                }
            }
        }

        public static List<Mod> GetValidMods(this DeviceImage d)
        {
            List<Mod> tm = new List<Mod>();
            foreach (Mod m in AvailableMods)
            {
                //TODO: When a mod is loaded, detect "FFFFFFF" CALID!!!
                if (m.InitialCalibrationId == d.CalId && m.TryCheckApplyMod(d.FilePath, d.FilePath + ".temp", true, false))
                    tm.Add(m);
                else if (m.FinalCalibrationId == d.CalId && m.TryCheckApplyMod(d.FilePath, d.FilePath + ".temp", false, false))
                    tm.Add(m);
            }
            return tm;
        }

        public static void InitTraces(){

        // First step: create the trace source object
        TraceSource ts = new TraceSource("myTraceSource");
        ts.Switch = new SourceSwitch("mySwitch", "my switch");
        ts.Switch.Level = SourceLevels.All; // Enable only warning, error and critical events

        ts.Listeners.Clear();
        Trace.Listeners.Clear();

        ConsoleTraceListener cl = new ConsoleTraceListener();
        cl.TraceOutputOptions = TraceOptions.None;
        cl.Filter = new EventTypeFilter(SourceLevels.Information);
        ts.Listeners.Add(cl);
        Trace.Listeners.Add(cl);

        if (Settings.Default.LogFilePath == null)
            InitSettings();

        TextWriterTraceListener tr = new TextWriterTraceListener(Settings.Default.LogFilePath);
        tr.TraceOutputOptions = TraceOptions.DateTime | TraceOptions.Timestamp | TraceOptions.Callstack;
        tr.Filter = new EventTypeFilter(SourceLevels.Warning);
        ts.Listeners.Add(tr);
        Trace.Listeners.Add(tr);

        // Setting autoflush to save files automatically
        Trace.AutoFlush = true;

        // Writing out some events
        //ts.TraceEvent(TraceEventType.Warning, 0, "warning message");
        //ts.TraceEvent(TraceEventType.Error, 0, "error message");
        //ts.TraceEvent(TraceEventType.Information, 0, "information message");
        //ts.TraceEvent(TraceEventType.Critical, 0, "critical message");
        }
    }
}
