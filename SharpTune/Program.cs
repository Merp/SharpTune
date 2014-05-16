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
using System.Windows.Forms;
using ConsoleRedirection;
using System.Diagnostics;
using SharpTune.RomMod;
using System.IO;
using SharpTune.EcuMapTools;
using System.Text;
using EcuMapTools;
using SharpTuneCore;

namespace SharpTune
{
    public static class Program
    {
        private static SharpTuner sharpTuner;
        /// <summary>
        /// Entry point.  Runs the utility with or without exception handling, depending on whether a debugger is attached.
        /// </summary>
        [STAThread]
        public static int Main(string[] args)
        {
            bool result = true;
            if (Debugger.IsAttached)
            {
                result = Run(args);
                // This gives you time to examine the output before the console window closes.
                Debugger.Break();
            }
            else
            {
                try
                {
                    result = Run(args);
                }
                catch (Exception exception)
                {
                    // This makes diagnostics much much easier.
                    Trace.WriteLine(exception);
                }
            }
            // For parity with "fc /b" return 0 on success, 1 on failure.
            return result ? 0 : 1;
        }

        /// <summary>
        /// Determines which command to run.
        /// </summary>
        private static bool Run(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            sharpTuner = new SharpTuner();
            sharpTuner.Init();
            if (args.Length < 1)
            {
                SharpTune.Program.RomModGui();
                return true;
            }
            if (args[0] == "ecumaptool")
            {
                return EcuMapTool.Run(sharpTuner.AvailableDevices, Utils.ShiftLeftTruncate(args));
            }
            else if (args[0] == "rommod")
            {
                return SharpTune.RomMod.RomMod.Run(sharpTuner.AvailableDevices, Utils.ShiftLeftTruncate(args));
            }
            else if (args[0] == "xmlconvertor")
            {
                DeviceImage di = new DeviceImage(sharpTuner, args[1]);
                XMLtoIDC xti = new XMLtoIDC(di);
                //TODO clean up this routine: xti.Run(args);
            }
            else if (args.Length == 2 && args[0] == "help")
            {
                PrintHelp_RomMod(args[1]);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Print generic usage instructions.
        /// </summary>
        private static void PrintHelp_RomMod()
        {
            Trace.WriteLine("SharpTune RomMod Version " + SharpTune.RomMod.RomMod.Version);
            Trace.WriteLine("Commands:");
            Trace.WriteLine("");
            Trace.WriteLine("test       - determine whether a patch is suitable for a ROM");
            Trace.WriteLine("apply      - apply a patch to a ROM file");
            Trace.WriteLine("applied    - determine whether a patch has been applied to a ROM");
            Trace.WriteLine("remove     - remove a patch from a ROM file");
            Trace.WriteLine("dump       - dump the contents of a patch file");
            Trace.WriteLine("baseline   - generate baseline data for a ROM and a partial patch");
            Trace.WriteLine("");
            Trace.WriteLine("Use \"sharptune rommod help <command>\" to show help for that command.");
        }

        /// <summary>
        /// Print usage instructions for a particular command.
        /// </summary>
        private static void PrintHelp_RomMod(string command)
        {
            switch (command)
            {
                case "test":
                    Trace.WriteLine("RomPatch test <patchfilename> <romfilename>");
                    Trace.WriteLine("Determines whether the given patch file matches the given ROM file.");
                    break;

                case "help":
                    Trace.WriteLine("You just had to try that, didn't you?");
                    break;
            }
        }
        
        public static void RomModGui()
        {
            Application.Run(sharpTuner.mainWindow);
        }

    }

    
}
