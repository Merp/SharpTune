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
using System.Xml.Linq;
using System.Data;
using System.IO;
using SharpTune;
using System.Windows.Forms;
using System.Drawing;
using SharpTune.Core;
using System.Runtime.Serialization;

namespace SharpTuneCore
{

    public class Table1DMetaData : TableMetaData
    {


        public Table1DMetaData(XElement xel, Definition def, TableMetaData basetable)
            : base(xel, def, basetable)
        { this.type = "1D"; }

        public override TableMetaData CreateChild(Lut lut,Definition d)
        {
            return base.CreateChild(lut,d);
        }
    }


    public class RamTable1DMetaData : Table1DMetaData
    {
        public RamTable1DMetaData(XElement xel, Definition def, TableMetaData basetable)// DeviceImage image)
            : base(xel, def, basetable)
        {

        }
    }
}
