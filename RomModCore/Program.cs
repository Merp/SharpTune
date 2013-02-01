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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace RomModCore
{
    public class Program
    {
        public const int Version = 6;

        /// <summary>
        /// Entry point.  Runs the utility with or without exception handling, depending on whether a debugger is attached.
        /// </summary>
        public static int Main(string[] args)
        {
            bool result = true;

            if (Debugger.IsAttached)
            {
                result = Program.Run(args);

                // This gives you time to examine the output before the console window closes.
                Debugger.Break();
            }
            else
            {
                try
                {
                    result = Program.Run(args);
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
            if (args.Length == 2 && args[0] == "help")
            {
                Program.PrintHelp(args[1]);
                return true;
            }

            if (args.Length == 2 && args[0] == "dump")
            {
                return Program.TryDumpSRecordFile(args[1]);
            }

            if (args.Length == 3 && args[0] == "test")
            {
                return Program.TryApply(args[1], args[2], true, false);
            }

            if (args.Length == 3 && args[0] == "apply")
            {
                return Program.TryApply(args[1], args[2], true, true);
            }

            if (args.Length == 3 && args[0] == "applied")
            {
                return Program.TryApply(args[1], args[2], false, false);
            }

            if (args.Length == 3 && args[0] == "remove")
            {
                return Program.TryApply(args[1], args[2], false, true);
            }

            if (args.Length == 3 && args[0] == "baseline")
            {
                return Program.TryGenerateBaseline(args[1], args[2]);
            }

            if (args.Length == 3 && args[0] == "baselinedefine")
            {
                return Program.TryBaselineAndDefine(args[1], args[2]);
            }

            Program.PrintHelp();
            return false;
        }

        /// <summary>
        /// Print generic usage instructions.
        /// </summary>
        private static void PrintHelp()
        {
            Console.WriteLine("RomPatch Version {0}.", Version);
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

                case "apply":
                    Console.WriteLine("RomPatch apply <patchfilename> <romfilename>");
                    Console.WriteLine();
                    Console.WriteLine("Verifies that a patch is suitable for the ROM file, then applies");
                    Console.WriteLine("the patch to the ROM (or prints an error message).");
                    break;

                case "applied":
                    Console.WriteLine("RomPatch applied <patchfilename> <romfilename>");
                    Console.WriteLine("Determines whether the given patch file was applied to the given ROM file.");
                    break;

                case "remove":
                    Console.WriteLine("RomPatch remove <patchfilename> <romfilename>");
                    Console.WriteLine();
                    Console.WriteLine("Verifies that a patch was applied to the ROM file, then removes");
                    Console.WriteLine("the patch from the ROM (or prints an error message).");
                    break;

                case "dump":
                    Console.WriteLine("RomPatch dump <filename>");
                    Console.WriteLine("Dumps the contents of the give patch file.");
                    break;

                case "baseline":
                    Console.WriteLine("RomPatch baseline <patchfilename> <romfilename>");
                    Console.WriteLine("Generates baseline SRecords for the given patch and ROM file.");
                    break;

                case "help":
                    Console.WriteLine("You just had to try that, didn't you?");
                    break;
            }
        }

        /// <summary>
        /// Determine whether a patch is suitable for a ROM, and optionally apply the patch if so.
        /// </summary>
        public static bool TryApply(string patchPath, string romPath, bool apply, bool commit)
        {
            SRecordReader reader = new SRecordReader(patchPath);
            Stream romStream;
            string workingPath = romPath + ".temp";
            if (commit)
            {
                File.Copy(romPath, workingPath, true);
                romStream = File.Open(workingPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            else
            {
                romStream = File.OpenRead(romPath);
            }

            Mod patcher = new Mod(reader, romStream);

            if (!patcher.TryReadPatches())
            {
                romStream.Dispose();
                reader.Dispose();
                return false;
            }

            Console.WriteLine("This patch file was intended for: {0}.", patcher.InitialCalibrationId);
            Console.WriteLine("This patch file converts ROM to:  {0}.", patcher.FinalCalibrationId);
            Console.WriteLine("This mod was created by: {0}.", patcher.ModAuthor);
            Console.WriteLine("Mod Information: {0} Version: {1}.", patcher.ModName, patcher.ModVersion);

            if (!apply)
            {
                Console.WriteLine("Preparing to remove patch.");
                patcher.TryReversePatches();

                if (!patcher.TryValidateUnPatches())
                {
                    Console.WriteLine("This patch file was NOT previously applied to this ROM file.");
                    romStream.Dispose();
                    reader.Dispose();
                    return false;
                }

                Console.WriteLine("This patch file was previously applied to this ROM file.");

                if (!commit)
                {
                    romStream.Dispose();
                    reader.Dispose();
                    return true;
                }

                Console.WriteLine("Removing patch.");

                if (patcher.TryRemoveMod())
                {
                    reader.Dispose();
                    romStream.Dispose();

                    Console.WriteLine("Verifying patch removal.");
                    using (Verifier verifier = new Verifier(patchPath, workingPath, apply))
                    {
                        if (!verifier.TryVerify(patcher.PatchList))
                        {
                            Console.WriteLine("Verification failed, ROM file not modified.");
                            return false;
                        }
                    }

                    File.Copy(workingPath, romPath, true);
                    File.Delete(workingPath);
                    Console.WriteLine("ROM file modified successfully, Mod has been removed.");
                }
                else
                {
                    Console.WriteLine("The ROM file has not been modified.");
                }

                romStream.Dispose();
                reader.Dispose();
                return true;

            }

            if (!patcher.TryValidatePatches())
            {
                Console.WriteLine("This mod NOT be applied to this ROM file.");
                romStream.Dispose();
                reader.Dispose();
                return false;
            }

            Console.WriteLine("This mod can be applied to this ROM file.");

            if (!commit)
            {
                romStream.Dispose();
                reader.Dispose();
                return true;
            }

            Console.WriteLine("Applying mod.");

            if (patcher.TryApplyMod())
            {
                reader.Dispose();
                romStream.Dispose();

                Console.WriteLine("Verifying mod.");
                using (Verifier verifier = new Verifier(patchPath, workingPath, apply))
                {
                    if (!verifier.TryVerify(patcher.PatchList))
                    {
                        Console.WriteLine("Verification failed, ROM file not modified.");
                        return false;
                    }
                }

                File.Copy(workingPath, romPath, true);
                File.Delete(workingPath);
                Console.WriteLine("ROM file modified successfully, mod has been applied.");
            }
            else
            {
                Console.WriteLine("The ROM file has not been modified.");
            }

            romStream.Dispose();
            reader.Dispose();
            return true;
        }
    
        /// <summary>
        /// Dump the contents of an SRecord file.  Mostly intended for development use.
        /// </summary>
        private static bool TryDumpSRecordFile(string path)
        {
            bool result = true;
            SRecord record;
            SRecordReader reader = new SRecordReader(path);
            reader.Open();
            BlobList list = new BlobList();
            while (reader.TryReadNextRecord(out record))
            {
                if (!record.IsValid)
                {
                    Console.WriteLine(record.ToString());
                    result = false;
                    continue;
                }

                list.ProcessRecord(record);

                Console.WriteLine(record.ToString());
            }

            Console.WriteLine("Aggregated:");
            foreach (Blob blob in list.Blobs)
            {
                Console.WriteLine(blob.ToString());
            }

            return result;
        }


        
        /// <summary>
        /// Extract data from an unpatched ROM file, for inclusion in a patch file.
        /// </summary>
        private static bool TryGenerateBaseline(string patchPath, string romPath)
        {

            SRecordReader reader = new SRecordReader(patchPath);
            Stream romStream = File.OpenRead(romPath);
            Mod patcher = new Mod(reader, romStream);

            if (!patcher.TryReadPatches()) return false;

            // Handy for manual work, but with this suppressed, you can append-pipe the output to the patch file.
            // Console.WriteLine("Generating baseline SRecords for:");
            // patcher.PrintPatches();

            return patcher.TryPrintBaselines(patchPath);
        }


        private static bool TryBaselineAndDefine(string patchPath, string romPath)
        {

            SRecordReader reader = new SRecordReader(patchPath);
            Stream romStream = File.OpenRead(romPath);
            Mod patcher = new Mod(reader, romStream);

            if (!patcher.TryReadPatches()) return false;
            if (!patcher.TryDefinition()) return false;

            // Handy for manual work, but with this suppressed, you can append-pipe the output to the patch file.
            // Console.WriteLine("Generating baseline SRecords for:");
            // patcher.PrintPatches();

            return patcher.TryPrintBaselines(patchPath);
        }


        public static bool ModCompCheck(string patchPath, string romPath, out bool isApplied)
        {
            isApplied = false;

            if (TryApply(patchPath, romPath, true, false))
            {
                //Can be applied!
                isApplied = false;
                return true;

            }
            
            if (TryApply(patchPath, romPath, false, false))
            {
                //Can be removed!
                isApplied = true;
                return true;
            }

            return false;
        }

        public static bool ModTest(string patchPath, string romPath, bool isApplied)
        {
            if (TryApply(patchPath, romPath, !isApplied, false))
            {
                //ITS A GO!
                return true;
            }

            return false;

        }

    }
}

    

