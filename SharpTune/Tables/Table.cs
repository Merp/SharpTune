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
using System.Xml.XPath;
using System.Xml.Linq;
using ModRom.Tables;


namespace ModRom.Tables
{
    public enum TableType
    {
        Float = 0x00,
        UInt8 = 0x04,
        UInt16 = 0x08,
        Int8 = 0x0C,
        Int16 = 0x10
    }

    public abstract class Table
    {



        //Every table has a name
        public string name { get; set; }

        public string category { get; set; }

        //Every table has a description
        private string description { get; set; }

        //Every table has a table type
        public string tableTypeString { get; set; }

        public TableType tableTypeHex { get; set; }

        public string scalingName { get; set; }

        public int level { get; set; }

        //Every table is either RAM or ROM
        private bool ramTable { get; set; }

        //Every table has a data address
        private int dataAddress { get; set; }

        private int dataScaling { get; set; }

        private int colorMin { get; set; }

        private int colorMax { get; set; }

        public Axis xAxis { get; set; }

        public Axis yAxis { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        public Table(string name)
        {
            this.name = name;

        }

        public List<string> Attributes { get; set; }

        /// <summary>
        /// Method to parse XML for adding a table axis
        /// </summary>
        public void AddAxis()
        {
        }
        /// <summary>
        /// Construct from XML Element
        /// </summary>
        /// <param name="xel"></param>
        public Table(XElement xel)
        {
            this.name = xel.Attribute("name").Value.ToString();
            if (xel.Attribute("category") != null) this.category = xel.Attribute("category").Value.ToString();
            else this.category = "Uncategorized";

            this.tableTypeString = xel.Attribute("type") != null ?  xel.Attribute("type").Value.ToString() : null;

            if (xel.Attribute("level") != null) this.level = (int)xel.Attribute("level");
            else this.level = 0;

            this.scalingName = xel.Attribute("scaling") != null ? xel.Attribute("scaling").Value.ToString() : null;


            //TODO USE THIS FORMAT TO PRODUCE ERRORS
            if (xel.Attribute("address") != null)
            {
                string hexaddr = xel.Attribute("address").Value.ToString();
                this.dataAddress = System.Int32.Parse(hexaddr, System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            foreach (XElement child in xel.Elements())
            {
                string name = child.Name.ToString();

                switch (name)
                {
                    case "table":
                        this.AddAxis(child);
                        break;

                    case "description":
                        this.description = child.Value.ToString();
                        break;

                    default:
                        break;
                }
            }

            //Check address for RAM Table
            if (this.dataAddress > Convert.ToInt64(0xFFFF0000)) ramTable = true; else ramTable = false;

        }

        public void AddAxis(XElement axis)
        {
            if (axis.Attribute("type") != null)
            {
                if (axis.Attribute("type").Value.ToString().Contains("X"))
                {
                    //Adding new X axis
                    this.xAxis = new Axis(axis);
                }
                else if (axis.Attribute("type").Value.ToString().Contains("Y"))
                {
                    this.yAxis = new Axis(axis);
                }
                else
                {
                }
            }


        }



        //public class BlobTable : Table
        //{

        //    //Blob Table Constructor
        //    //public BlobTable()
        //       //: base()
        //    //{
        //    //    //constructor here
        //    //}

        //}


        

    }
}
