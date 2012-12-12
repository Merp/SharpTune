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
using System.Reflection;
using Merp;
using System.IO;
using RomModCore;
using SharpTuneCore;

namespace SharpTune
{
    public static class ModUtils
    {
        public static bool getValidMods(this DeviceImage image, string path)
        {

            //image.ModList.Clear();

            //Get patches stored in binary
            //List<string> tempModPaths = ResourceUtil.GetPatchPaths();

            List<string> tempModPaths = new List<string>();
            //Get patches stored in working directory

            string calid = image.CalId.ToString();
            string[] terms = {".patch"};

            List<string> searchresults =  ResourceUtil.directorySearch(path, terms);

            if (searchresults == null) return false;

            foreach (string filepath in searchresults)
            {
                tempModPaths.Add(filepath);
            }

            foreach (string modpath in tempModPaths)
            {
                Mod tempMod = new Mod(modpath);
                if(tempMod.CompatibilityCheck(image.FilePath.ToString()))
                {
                    image.ModList.Add(tempMod);
                }
            }
            return true;
        }

        public static void getValidMods(this DeviceImage image, Assembly assembly)
        {
            image.ModList.Clear();
            string calid = image.CalId.ToString();
            string[] mods = assembly.GetManifestResourceNames();
            foreach (string modpath in mods)
            {
                if (modpath.ContainsCI(image.CalId.ToString()))
                {
                    Stream stream = assembly.GetManifestResourceStream(modpath);
                     Mod tempMod = new Mod(stream,modpath);
                    if(tempMod.CompatibilityCheck(image.FilePath.ToString()))
                    {
                        image.ModList.Add(new Mod(modpath));
                    }
                }
            }
        }
    }
}
