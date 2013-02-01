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
using System.IO;

namespace SharpTune
{
    public class ModInfo
    {
        public long FileSize { get; private set; }

        public string FileName { get; private set; }

        public string FilePath { get; private set; }

        public string CalId { get; private set; }

        public int CalIdOffset { get; private set; }

        public bool isApplied { get; set; }

        public string direction { get; set; }


        public ModInfo(string modpath, bool isapplied)
        {
            FileInfo f = new FileInfo(modpath);
            this.FileSize = f.Length;
            this.FileName = f.Name;
            this.FilePath = modpath;

            this.isApplied = isapplied;

            if (isapplied) this.direction = "REMOVE";
            else this.direction = "APPLY";

        }
    }
}
