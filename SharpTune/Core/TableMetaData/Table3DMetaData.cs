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

    public class Table3DMetaData : TableMetaData
    {

        public Table3DMetaData(XElement xel, ECUMetaData def, TableMetaData basetable)
            : base(xel, def, basetable)
        {
            this.type = "3D";
        }

        public override TableMetaData CreateChild(LookupTable ilut, ECUMetaData d)
        {
            XElement xel;
            LookupTable3D lut = (LookupTable3D)ilut;
            xel = new XElement("table");
            xel.SetAttributeValue("name", name);
            xel.SetAttributeValue("address", ilut.dataAddress.ToString("X"));
            XElement tx = new XElement("table");
            tx.SetAttributeValue("name", "X");
            tx.SetAttributeValue("address", lut.colsAddress.ToString("X"));
            tx.SetAttributeValue("elements", lut.cols);
            xel.Add(tx);
            XElement ty = new XElement("table");
            ty.SetAttributeValue("name", "Y");
            ty.SetAttributeValue("address", lut.rowsAddress.ToString("X"));
            ty.SetAttributeValue("elements", lut.rows);
            xel.Add(ty);
            return TableFactory.CreateTable(xel, name, d);
            //TODO also set attirbutes and split this up! Copy to table2D!!
        }
    }

    public class RamTable3DMetaData : Table3DMetaData
    {

        public RamTable3DMetaData(XElement xel, ECUMetaData def, TableMetaData basetable)
            : base(xel, def, basetable)
        {

        }

    }
}

