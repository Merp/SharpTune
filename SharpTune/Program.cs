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
using RomModCore;
using ConvTools;
using System.IO;

namespace SharpTune
{
    public static class Program
    {
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
                    Console.WriteLine(exception);
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
            if (args.Length < 1)
            {
                SharpTune.Program.RomModGui();
            }
            else if (args[0] == "convtools")
            {
                ConvTools.ConvTools.Run(args);
            }
            else if (args[0] == "rommod")
            {
                RomModCore.RomMod.Run(args);
            }
            if (args.Length == 2 && args[0] == "help")
            {
                PrintHelp(args[1]);
                return true;
            }
            PrintHelp();
            return false;
        }

        /// <summary>
        /// Print generic usage instructions.
        /// </summary>
        private static void PrintHelp()
        {
            Console.WriteLine("RomPatch Version {0}.", RomModCore.RomMod.Version);
            Console.WriteLine("Commands:");
            Console.WriteLine();
            Console.WriteLine("test       - determine whether a patch is suitable for a ROM");
            Console.WriteLine("apply      - apply a patch to a ROM file");
            Console.WriteLine("applied    - determine whether a patch has been applied to a ROM");
            Console.WriteLine("remove     - remove a patch from a ROM file");
            Console.WriteLine("dump       - dump the contents of a patch file");
            Console.WriteLine("baseline   - generate baseline data for a ROM and a partial patch");
            Console.WriteLine();
            Console.WriteLine("Use \"RomPatch help <command>\" to show help for that command.");
        }

        /// <summary>
        /// Print usage instructions for a particular command.
        /// </summary>
        private static void PrintHelp(string command)
        {
            switch (command)
            {
                case "test":
                    Console.WriteLine("RomPatch test <patchfilename> <romfilename>");
                    Console.WriteLine("Determines whether the given patch file matches the given ROM file.");
                    break;

                case "help":
                    Console.WriteLine("You just had to try that, didn't you?");
                    break;
            }
        }
        
        public static void RomModGui()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }

    }

    
}
