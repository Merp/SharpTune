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
using System.Reflection;
using SharpTune.Core;
using System.Net;
using System.ComponentModel;
using System.Windows.Forms;

namespace SharpTune
{
    /// <summary>
    /// Container for a sharptune instance
    /// </summary>
    public static class SharpTuner
    {
        public const string GitHelpUrl = "http://github.com/Merp/SharpTune/tree/README.md";
        public const string DonateUrl = "http://github.com/Merp/SharpTune/tree/DONATE.md";
        public const string HomeUrl = "http://romraider.com";
        public const string ForumUrl = "http://romraider.com";

        public static MainWindow Window { get; set; }

        private static DeviceImage actImg;

        private static IPlugin[] Plugins;
        private static PluginContainer PluginHost = new PluginContainer();

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

        public static string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        static SharpTuner(){}

        public static void Init()
        {
            InitSettings();
            InitTraces();
            Trace.WriteLine("<--- Initializing SharpTuner --->");
            Trace.WriteLine("SharpTune Assembly Version: " + Version);
            ImageList = new List<DeviceImage>();
            DataScalings = new List<Scaling>();
            UnitScalings = new List<Scaling>();
            PopulateAvailableDevices();
            LoadMods();
            LoadPlugins();
            Trace.WriteLine("<--Finished Initializing SharpTuner --->");
        }

        public static void InitSettings()
        {
            string userdir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            Console.WriteLine("Found user directory: " + userdir);

            Settings.Default.SettingsPath = userdir + @"\.SharpTune";

            if (Settings.Default.LogFilePath == null | Settings.Default.LogFilePath == "")
                Settings.Default.LogFilePath = Settings.Default.SettingsPath;

            if (!Directory.Exists(Settings.Default.LogFilePath))
                Directory.CreateDirectory(Settings.Default.LogFilePath);

            if (Settings.Default.PluginPath == null | Settings.Default.PluginPath == "")
                 Settings.Default.PluginPath = Settings.Default.SettingsPath + @"\Plugins";

            if (!Directory.Exists(Settings.Default.PluginPath))
                Directory.CreateDirectory(Settings.Default.PluginPath);

            if (Settings.Default.SubaruDefsRepoPath == null | Settings.Default.SubaruDefsRepoPath == "")
                if (Directory.Exists(userdir + @"\Dev\SubaruDefs"))
                    Settings.Default.SubaruDefsRepoPath = userdir + @"\Dev\SubaruDefs";
                else
                    Settings.Default.SubaruDefsRepoPath = Settings.Default.SettingsPath + @"\SubaruDefs";

            //if (!Directory.Exists(Settings.Default.SubaruDefsRepoPath))
            //       Directory.CreateDirectory(Settings.Default.SubaruDefsRepoPath);

            if (Settings.Default.PatchPath == null || Settings.Default.PatchPath == "")
                Settings.Default.PatchPath = Settings.Default.SettingsPath + @"\Mods";

            if (!Directory.Exists(Settings.Default.PatchPath))
                    Directory.CreateDirectory(Settings.Default.PatchPath);

            if (Settings.Default.LogFilePath == null || Settings.Default.LogFilePath == "")
                Settings.Default.LogFilePath = Settings.Default.SettingsPath + @"\SharpTune.log";

             if (!Directory.Exists(Settings.Default.LogFilePath))
                    Directory.CreateDirectory(Settings.Default.LogFilePath);

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
            //LoadResourceMods();
            LoadExternalMods();
        }

        public static void LoadPlugins()
        {
            Trace.WriteLine("Settings dir: " + Settings.Default.SettingsPath);
            Trace.WriteLine("Loading plugins from " + Settings.Default.PluginPath);
            string[] pluginFiles = Directory.GetFiles(Settings.Default.PluginPath, "*.DLL");
            Plugins = new IPlugin[pluginFiles.Length];
            for (int i = 0; i < pluginFiles.Length; i++)
            {
                string args = pluginFiles[i].Substring(
                    pluginFiles[i].LastIndexOf("\\") + 1,
                    pluginFiles[i].IndexOf(".dll") -
                    pluginFiles[i].LastIndexOf("\\") - 1);
                Type ObjType = null;
                try
                {

                    //Plugins = (
                    // From each file in the files.
                    //from file in pluginFiles
                    // Load the assembly.
                    //let asm = Assembly.LoadFile(file)
                    // For every type in the assembly that is visible outside of
                    // the assembly.
                    //from type in asm.GetExportedTypes()
                    // Where the type implements the interface.
                    //where typeof(IPlugin).IsAssignableFrom(type)
                    // Create the instance.
                    //select (IPlugin)Activator.CreateInstance(type)
                    // Materialize to an array.
                    //).ToArray();

                    //foreach (IPlugin ip in Plugins)
                    //{
                    //    Trace.WriteLine("Loaded Plugin " + ip.Name);
                    //}
                    // load it
                    Assembly ass = null;
                    ass = Assembly.LoadFile(pluginFiles[i]);
                    if (ass != null)
                    {
                        ObjType = ass.GetType(args + ".PlugIn");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                try
                {
                    // OK Lets create the object as we have the Report Type
                    if (ObjType != null)
                    {
                        Plugins[i] = (IPlugin)Activator.CreateInstance(ObjType);
                        Plugins[i].Host = (IPluginHost)SharpTuner.PluginHost;
                        Trace.WriteLine("Loaded Plugin " + args);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        //public static bool AuthenticateMod(Stream outStream)
        //{
        //    foreach (IPlugin i in Plugins)
        //    {
        //        if (i != null && i.Name != null && i.Name == "SharpTune Vin Authentication")
        //            return i.Run(outStream);
        //    }
        //    DialogResult res;
        //    res = MessageBox.Show("Auth Plugin not found! Download?", "Plugin Missing!", MessageBoxButtons.YesNo);
        //    if (res == DialogResult.Yes)
        //    {
        //        if (InstallAuth())
        //        {
        //            SharpTuner.LoadPlugins();
        //            return AuthenticateMod(outStream);
        //        }
        //    }
        //    return false;
        //}

        //private static Uri AuthDownloadUri = new Uri("http://sharptuning.com/wp-content/uploads/edd/2013/05/SharpTuneAuth.dll");
        //private static bool InstallAuth()
        //{
        //    try
        //    {
        //        using (WebClient webClient = new WebClient())
        //        {
        //            webClient.DownloadFile(AuthDownloadUri, Settings.Default.PluginPath + @"\SharpTuneAuth.dll");
        //            webClient.Dispose();
        //        }
        //        return true;
        //    }
        //    catch (Exception E)
        //    {
        //        Trace.WriteLine(E.Message);
        //        MessageBox.Show("Error downloading auth plugin!");
        //        return false;
        //    }
        //}


        /// <summary>
        /// TODO FIX OR REMOVE THIS. Can't really embed mods as resources without source code anyway!!
        /// </summary>
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
                {
                    tm.Add(m);
                    Trace.WriteLine("Loaded Mod: " + m.FileName);
                }
                else if (m.ModIdent == d.CalId && m.TryCheckApplyMod(d.FilePath, d.FilePath + ".temp", false, false))
                {
                    tm.Add(m);
                    Trace.WriteLine("Loaded Mod: " + m.FileName);
                }
            }
            return tm;
        }

        public static void RefreshImages()
        {
            foreach (DeviceImage d in ImageList)
            {
                d.Refresh();
            }
            Window.Refresh();
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
