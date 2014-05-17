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
using SharpTuneCore;
using SharpTune.Core;
using System.Runtime.Serialization;

namespace SharpTuneCore
{
    public class Table2DMetaData : TableMetaData
    {

        public Table2DMetaData(XElement xel,Definition def, TableMetaData basetable)
            : base(xel, def, basetable)
        {
            this.type = "2D";
        }

        public override TableMetaData CreateChild(Lut ilut,Definition d)
        {
            //TODO: This is a major KLUDGE.
            if (ilut.GetType() != typeof(Lut2D))
                return base.CreateChild(ilut, d);

            XElement xel;
            Lut2D lut = (Lut2D)ilut;
            xel = new XElement("table");
            xel.SetAttributeValue("name", name);
            xel.SetAttributeValue("address", ilut.dataAddress.ToString("X"));
            if (this.xAxis != null)
            {
                XElement tx = new XElement("table");
                tx.SetAttributeValue("name", "X");
                tx.SetAttributeValue("address", lut.colsAddress.ToString("X"));
                tx.SetAttributeValue("elements", lut.cols);
                xel.Add(tx);
            }
            else
            {
                XElement ty = new XElement("table");
                ty.SetAttributeValue("name", "Y");
                ty.SetAttributeValue("address", lut.colsAddress.ToString("X"));
                ty.SetAttributeValue("elements", lut.cols);
                xel.Add(ty);
            }
            return TableFactory.CreateTable(xel, name, d);
            //TODO also set attirbutes and split this up! Copy to table2D!!
            //return base.CreateChild(lut,d);
            //TODO FIX?? AND CHECK FOR STATIC AXES!!
        }
    }

    public class RamTable2DMetaData : Table2DMetaData
    {

        public RamTable2DMetaData(XElement xel,Definition def, TableMetaData basetable)
            : base(xel,def,basetable)
        {

        }

    }
}
