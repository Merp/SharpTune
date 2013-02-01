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
using System.Reflection;
using SharpTune;
using SharpTuneCore;

namespace RomModCore
{
    public class RomMod
    {
        public const int Version = 7;

        public static bool TryApply(string patchPath, string romPath, bool apply, bool commit)
        {
            Mod currentMod = new Mod(patchPath);
            return currentMod.TryCheckApplyMod(romPath, romPath, commit);

            //Stream romStream;
            //string workingPath = romPath + ".temp";
            //if (commit)
            //{
            //    File.Copy(romPath, workingPath, true);
            //    romStream = File.Open(workingPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            //}
            //else
            //{
            //    romStream = File.OpenRead(romPath);
            //}

            //using (romStream)
            //{
                

            //    if (!currentMod.TryReadPatches())
            //        return false;

            //    Console.WriteLine("This patch file was intended for: {0}.", currentMod.InitialCalibrationId);
            //    Console.WriteLine("This patch file converts ROM to:  {0}.", currentMod.FinalCalibrationId);
            //    Console.WriteLine("This mod was created by: {0}.", currentMod.ModAuthor);
            //    Console.WriteLine("Mod Information: {0} Version: {1}.", currentMod.ModName, currentMod.ModVersion);
            //    Console.WriteLine(currentMod.ModInfo);

            //    if (!apply)
            //    {
            //        Console.WriteLine("Preparing to remove patch.");
            //        currentMod.TryReversePatches();

            //        if (!currentMod.TryValidateUnPatches(romStream))
            //        {
            //            Console.WriteLine("This patch file was NOT previously applied to this ROM file.");
            //            return false;
            //        }

            //        Console.WriteLine("This patch file was previously applied to this ROM file.");

            //        if (!commit)
            //            return true;

            //        Console.WriteLine("Removing patch.");

            //        if (currentMod.TryRemoveMod(romStream))
            //        {
            //            Console.WriteLine("Verifying patch removal.");
            //            using (Verifier verifier = new Verifier(patchPath, workingPath, apply))
            //            {
            //                if (!verifier.TryVerify(currentMod.patchList))
            //                {
            //                    Console.WriteLine("Verification failed, ROM file not modified.");
            //                    return false;
            //                }
            //            }
            //            File.Copy(workingPath, romPath, true);
            //            File.Delete(workingPath);
            //            Console.WriteLine("ROM file modified successfully, Mod has been removed.");
            //        }
            //        else
            //            Console.WriteLine("The ROM file has not been modified.");

            //        return true;
            //    }

            //    if (!currentMod.TryValidatePatches(romStream))
            //    {
            //        Console.WriteLine("This mod can NOT be applied to this ROM file.");
            //        return false;
            //    }

            //    Console.WriteLine("This mod can be applied to this ROM file.");

            //    if (!commit)
            //        return true;

            //    Console.WriteLine("Applying mod.");

            //    if (currentMod.TryApplyMod(romStream))
            //    {
            //        Console.WriteLine("Verifying mod.");
            //        using (Verifier verifier = new Verifier(patchPath, workingPath, apply))
            //        {
            //            if (!verifier.TryVerify(currentMod.patchList))
            //            {
            //                Console.WriteLine("Verification failed, ROM file not modified.");
            //                return false;
            //            }
            //        }
            //        File.Copy(workingPath, romPath, true);
            //        File.Delete(workingPath);
            //        Console.WriteLine("ROM file modified successfully, mod has been applied.");
            //    }
            //    else
            //    {
            //        Console.WriteLine("The ROM file has not been modified.");
            //    }
            //    return true;
            //}
        }
    
        /// <summary>
        /// Dump the contents of an SRecord file.  Mostly intended for development use.
        /// </summary>
        public static bool TryDumpSRecordFile(string path)
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
        public static bool TryGenerateBaseline(string patchPath, string romPath)
        {
            Stream romStream = File.OpenRead(romPath);
            Mod patcher = new Mod(patchPath);

            if (!patcher.TryReadPatches()) return false;

            // Handy for manual work, but with this suppressed, you can append-pipe the output to the patch file.
            // Console.WriteLine("Generating baseline SRecords for:");
            // patcher.PrintPatches();

            return patcher.TryPrintBaselines(patchPath,romStream);
        }


        public static bool TryBaselineAndDefine(string patchPath, string romPath, string defPath)
        {
            Stream romStream = File.OpenRead(romPath);
            Mod patcher = new Mod(patchPath);

            if (!patcher.TryReadPatches()) return false;
            if (!patcher.TryDefinition(defPath)) return false;

            // Handy for manual work, but with this suppressed, you can append-pipe the output to the patch file.
            // Console.WriteLine("Generating baseline SRecords for:");
            // patcher.PrintPatches();

            return patcher.TryPrintBaselines(patchPath,romStream);
        }
    }
}

    

