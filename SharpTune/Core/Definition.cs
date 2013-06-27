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
using System.Collections.ObjectModel;
using System.Text;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Linq;
using SharpTune;
using SharpTune.Properties;
using SharpTune.Core;
using SharpTune.ConversionTools;
using SharpTune.RomMod;
using System.Runtime.Serialization;
using SharpTune;
using System.Windows.Forms;

namespace SharpTuneCore
{
    
    /// <summary>
    /// Represents an individual device definition
    /// Includes ALL scalings from base to top
    /// </summary>
    /// 
    [Serializable]
    public class Definition
    {

        public string FilefPath { get; set; }
        public bool IsBase { get; private set; }
        public string CalId { get; set; } //TODO encapsulate better?
        public int CalIdAddress { get; private set; }
        public string EcuId
        {
            get { return CarInfo["ecuid"]; }
            set { }
        }
        public string CpuBits
        {
            get
            {
                if (this.CarInfo["memmodel"].ContainsCI("16"))
                    return "16";
                else
                    return "32";
                //TODO: make this better!
            }
            set { }
        }

        public string Include { get; set; }
        public List<Definition> InheritList { get; private set; }

        /// <summary>
        /// Contains basic info for the definition
        /// Make, Model, etc
        /// </summary>
        public Dictionary<string, string> CarInfo {
            get { return carInfo; } 
            private set{
                    carInfo = value;
                    CreateXRomId();//TODO CLEAN UP
           }
        }
        private Dictionary<string, string> carInfo;

        /// <summary>
        /// Holds all XElements pulled from XML for ROM tables
        /// Includes inherited XML
        /// </summary>
        public XElement xRomId { get; set; }

        public Dictionary<string, Scaling> ScalingList { get; private set; }//TODO: scalings should belong to higher object!

        #region tables

        public Dictionary<string,Table> DefinedRomTables { get; private set;}
        public Dictionary<string,Table> DefinedRamTables { get; private set;}
        
        public Dictionary<string,Table> InheritedRomTables { 
            get{
                Dictionary<string,Table> ret = new Dictionary<string,Table>();
                foreach(Definition d in InheritList)
                {
                    foreach(Table t in d.DefinedRomTables.Values)
                    {
                        if(!ret.ContainsKey(t.Name))
                            ret.Add(t.Name,t);
                    }
                }
                return ret;
            }
            private set{}
        }

        public Dictionary<string,Table> InheritedRamTables { 
            get{
                Dictionary<string,Table> ret = new Dictionary<string,Table>();
                foreach(Definition d in InheritList)
                {
                    foreach(Table t in d.DefinedRamTables.Values)
                    {
                        if(!ret.ContainsKey(t.Name))
                            ret.Add(t.Name,t);
                    }
                }
                return ret;
            }
            private set{}
        }

        public Dictionary<string, Table> ExposedRomTables { 
            get{
                return Utils.AggregateDictionary(DefinedRomTables,InheritedRomTables);
            }
            private set{}
        }
        public Dictionary<string, Table> ExposedRamTables { 
            get{
                return Utils.AggregateDictionary(DefinedRamTables,InheritedRamTables);
            }
            private set{}
        }
        public Dictionary<string,Table> DefinedBaseRomTables { get; private set;}
        public Dictionary<string,Table> DefinedBaseRamTables { get; private set;}

        public Dictionary<string,Table> InheritedBaseRomTables { 
            get{
                Dictionary<string,Table> ret = new Dictionary<string,Table>();
                foreach(Definition d in InheritList)
                {
                    foreach(Table t in d.DefinedBaseRomTables.Values)
                    {
                        if(!ret.ContainsKey(t.Name))
                            ret.Add(t.Name,t);
                    }
                }
                return ret;
            }
            private set{}
        }

        public Dictionary<string,Table> InheritedBaseRamTables { 
            get{
                Dictionary<string,Table> ret = new Dictionary<string,Table>();
                foreach(Definition d in InheritList)
                {
                    foreach(Table t in d.DefinedBaseRamTables.Values)
                    {
                        if(!ret.ContainsKey(t.Name))
                            ret.Add(t.Name,t);
                    }
                }
                return ret;
            }
            private set{}
        }

        public Dictionary<string, Table> ExposedBaseRomTables { 
            get{
                return Utils.AggregateDictionary(DefinedBaseRomTables,InheritedBaseRomTables);
            }
            private set{}
        }
        public Dictionary<string, Table> ExposedBaseRamTables { 
            get{
                return Utils.AggregateDictionary(DefinedBaseRamTables,InheritedBaseRamTables);
            }
            private set{}
        }

        public List<Table> RomTables{
            get{
                List<Table> tlist = new List<Table>();
                tlist.AddRange(DefinedRomTables.Values);
                tlist.AddRange(DefinedBaseRomTables.Values);
                return tlist;
            }
            private set{}
        }

        public List<Table> RamTables{
            get{
                List<Table> tlist = new List<Table>();
                tlist.AddRange(DefinedRomTables.Values);
                tlist.AddRange(DefinedBaseRomTables.Values);
                return tlist;
            }
            private set{}
        }
        #endregion

        #region constructors

        /// <summary>
        /// Base Constructor, only initializes
        /// </summary>
        public Definition()
        {
            IsBase = false;
            CarInfo = new Dictionary<string, string>();
            DefinedRomTables = new Dictionary<string, Table>();
            DefinedRamTables = new Dictionary<string, Table>();
            DefinedBaseRomTables = new Dictionary<string, Table>();
            DefinedBaseRamTables = new Dictionary<string, Table>();
            ScalingList = new Dictionary<string,Scaling>();
            InheritList = new List<Definition>();
            Include = null;
        }

        /// <summary>
        /// Constructor to load romid given a defintion file
        /// </summary>
        /// <param name="calID"></param>
        public Definition(string filepath) : this()
        {
            CalId = null;
            FilefPath = filepath;
            LoadRomId();
        }

        /// <summary>
        /// Constructor used to create new definitions using existing data
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="carinfo"></param>
        /// <param name="include"></param>
        /// <param name="xromt"></param>
        /// <param name="xramt"></param>
        //public Definition(string filepath, Dictionary<string, string> carinfo, string incl) : this()
        //{
        //    //TODO error handling
        //    include = incl;
        //    CarInfo = new Dictionary<string,string>(carinfo);
        //    CalId = carinfo["internalidstring"];
        //    CalIdAddress = int.Parse(carinfo["internalidaddress"], System.Globalization.NumberStyles.AllowHexSpecifier);
        //    defPath = filepath;
        //    Inherit();
        //}

        /// <summary>
        /// Constructor used to create new definitions for defining mods
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="carinfo"></param>
        /// <param name="include"></param>
        /// <param name="xromt"></param>
        /// <param name="xramt"></param>
        public Definition(string filepath, Mod mod) : this()
        {
            this.Include = mod.InitialCalibrationId;
            FilefPath = filepath;
            
            CloneRomIdFrom(Include,true);

            CarInfo["internalidaddress"] = mod.ModIdentAddress.ToString("X");
            CarInfo["internalidstring"] = mod.ModIdent.ToString();
            CarInfo["ecuid"] = mod.FinalEcuId.ToString();
            CarInfo["xmlid"] = mod.ModIdent.ToString();
            Inherit();

            //TODO: ADD THE TABLES FROM MOD
            //mod.modDef.
            foreach (var rt in mod.modDef.RomLutList)
            {
                //do something with each lut.
                ExposeTable(rt.Name, rt); //TODO: Fix this redundancy?
            }
        }
        #endregion

        #region romid methods

        /// <summary>
        /// Clones an existing definitions romid
        /// TODO: make the unknown rom handler use this.
        /// </summary>
        /// <param name="inh"></param>
        /// device identifier to clone from
        /// <param name="incl"></param>
        /// include the device (true) or include its base (false)
        /// <returns></returns>
        public bool CloneRomIdFrom(string inh, bool incl)
        {
            if(!SharpTuner.AvailableDevices.DefDictionary.ContainsKey(inh))
                return false;
            Definition d = SharpTuner.AvailableDevices.DefDictionary[inh];
            CarInfo = new Dictionary<string, string>(d.CarInfo);
            if (incl)
                Include = d.CalId;
            else
                Include = d.Include;
            return true;
        }

        public void ParseRomId()
        {
            CarInfo.Clear();
            foreach (XElement element in xRomId.Elements())
            {
                CarInfo.Add(element.Name.ToString(), element.Value.ToString());
            }
            if (CarInfo.ContainsKey("internalidstring"))
                CalId = CarInfo["internalidstring"];
            if (CarInfo.ContainsKey("internalidaddress"))
                CalIdAddress = int.Parse(CarInfo["internalidaddress"], System.Globalization.NumberStyles.AllowHexSpecifier);
            if (CarInfo.ContainsKey("xmlid"))
                if (CarInfo["xmlid"].Contains("BASE"))
                    CalId = CarInfo["xmlid"].ToString();
        }

        /// <summary>
        /// Read the rom identification header and include from a file
        /// </summary>
        /// <param name="fetchPath"></param>
        /// <returns></returns>
        public void LoadRomId()
        {
            XDocument xmlDoc = XDocument.Load(FilefPath, LoadOptions.PreserveWhitespace);
            this.xRomId = xmlDoc.XPathSelectElement("/rom/romid");
            ParseRomId();
            if (xmlDoc.XPathSelectElement("/rom/include") != null)
                Include = xmlDoc.XPathSelectElement("/rom/include").Value.ToString();
        }
        #endregion

        /// <summary>
        /// Populates a 'short def' (romid + include) into a full definition
        /// </summary>
        /// <returns></returns>
        public bool Populate()
        {
            if (CalId != null && FilefPath != null)
            {
                Clear();
                if (Include != null)
                    Inherit();
                if (ReadXML(FilefPath) && Include != null)
                    return true;
            }
            return false;
        }

        private void Inherit()
        {
            Dictionary<string, Definition> dd = SharpTuner.AvailableDevices.DefDictionary;
            if (dd.ContainsKey(Include) && dd[Include].CalId != null)
                dd[Include].Populate();

            if(SharpTuner.AvailableDevices.DefDictionary[Include].InheritList.Count > 0)
                InheritList.AddRange(SharpTuner.AvailableDevices.getDef(Include).InheritList);

            InheritList.Add(SharpTuner.AvailableDevices.DefDictionary[Include]);
            InheritList.Reverse();
        }

        private void Clear()
        {
            DefinedRomTables.Clear();
            DefinedRamTables.Clear();
            DefinedBaseRomTables.Clear();
            DefinedBaseRamTables.Clear();
            ScalingList.Clear();
            InheritList.Clear();
        }

        /// <summary>
        /// Load parameters from XML an XML file
        /// </summary>
        public bool ReadXML(string path)
        {
            if (path == null) return false;
            XDocument xmlDoc = XDocument.Load(path, LoadOptions.PreserveWhitespace);
            // ROM table fetches here!
            var tableQuery = from t in xmlDoc.XPathSelectElements("/rom/table")
                             select t;
            foreach (XElement table in tableQuery)
            {
                if (table.Attribute("name") == null) 
                    continue;
                string tablename = table.Attribute("name").Value.ToString();
                try{
                    AddRomTable(TableFactory.CreateRomTable(table,this));
                    }
                    catch (Exception e)
                    {
                        Trace.TraceWarning(e.Message);
                    }
            }
            // RAM table feteches here!
            var ramtableQuery = from t in xmlDoc.XPathSelectElements("/ram/table")
                                select t;
            foreach (XElement table in ramtableQuery)
            {
                if (table.Attribute("name") == null) 
                    continue;
                string tablename = table.Attribute("name").Value.ToString();

                try{
                    AddRamTable(TableFactory.CreateRomTable(table,this));
                    }
                    catch (Exception e)
                    {
                        Trace.TraceWarning(e.Message);
                    }
            }
            //Read Scalings
            var scalingQuery = from sc in xmlDoc.XPathSelectElements("/rom/scaling")
                               //where table.Ancestors("table").First().IsEmpty
                               select sc;
            foreach (XElement scaling in scalingQuery)
            {
                //skip scalings with no name
                if (scaling.Attribute("name") == null) continue;
                string scalingname = scaling.Attribute("name").Value.ToString();
                if (!this.ScalingList.ContainsKey(scalingname))
                {
                    this.ScalingList.Add(scalingname, ScalingFactory.CreateScaling(scaling));
                }
            }
            return true;
        }

        private void AddRomTable(Table table)
        {
            if (table.IsBase)
            {
                if (!DefinedBaseRomTables.ContainsKey(table.Name))
                    DefinedBaseRomTables.Add(table.Name, table);
                else
                    Trace.WriteLine("Warning, duplicate table: " + table.Name + ". Please check the definition!!");
            }
            else
            {
                if (!DefinedRomTables.ContainsKey(table.Name))
                    DefinedRomTables.Add(table.Name, table);
                else
                    Trace.WriteLine("Warning, duplicate table: " + table.Name + ". Please check the definition!!");
            }
        }

        private void AddRamTable(Table table)
        {
            if (table.IsBase)
            {
                if (!DefinedBaseRamTables.ContainsKey(table.Name))
                    DefinedBaseRamTables.Add(table.Name, table);
                else
                    Trace.WriteLine("Warning, duplicate table: " + table.Name + ". Please check the definition!!");
            }
            else
            {
                if (!DefinedRamTables.ContainsKey(table.Name))
                    DefinedRamTables.Add(table.Name, table);
                else
                    Trace.WriteLine("Warning, duplicate table: " + table.Name + ". Please check the definition!!");
                    
            }
        }

        /// <summary>
        /// Pulls the scaling xelement from the definition at fetchPath
        /// </summary>
        /// <returns>
        /// The scalings.
        /// </returns>
        /// <param name='fetchPath'>
        /// Fetch path.
        /// </param>
        public static void pullScalings(String fetchPath, ref List<XElement> xbs, ref List<XElement> xs)
        {

            if (fetchPath == null) return;
            List<XElement> xlist = new List<XElement>();
            XDocument xmlDoc = XDocument.Load(fetchPath, LoadOptions.PreserveWhitespace);
            var scalingQuery = from sc in xmlDoc.XPathSelectElements("/rom/scaling")
                               //where table.Ancestors("table").First().IsEmpty
                               select sc;
            foreach (XElement scaling in scalingQuery)
            {
                if (scaling.Attribute("storagetype") != null && scaling.Attribute("storagetype").Value == "bloblist")
                {
                    scaling.Attribute("storagetype").Remove();
                    xbs.Add(scaling);
                }
                else
                {
                    xs.Add(scaling);
                }
            }
            scalingQuery.ToList().ForEach(x => x.Remove());
            using (XmlTextWriter xmlWriter = new XmlTextWriter(fetchPath, new UTF8Encoding(false)))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.Indentation = 4;
                xmlDoc.Save(xmlWriter);
            }
        }

        /// <summary>
        /// Load parameters from XML an XML file
        /// </summary>
        public static void ConvertXML(string fetchPath, ref List<String> blobtables,
            ref Dictionary<String, List<String>> t3d,
            ref Dictionary<String, List<String>> t2d,
            ref Dictionary<String, List<String>> t1d,
            Dictionary<String, String> imap,
            bool isbase)
        {

            if (fetchPath == null) return;
            XDocument xmlDoc = XDocument.Load(fetchPath, LoadOptions.PreserveWhitespace);
            List<String> newtables = new List<String>();
            String rombase;

            Dictionary<String, List<String>> includes = new Dictionary<String, List<String>>();


            if (!isbase)
            {
                rombase = imap[fetchPath];
            }
            else
            {
                var xi = xmlDoc.XPathSelectElement("/rom/romid/xmlid");
                rombase = xi.Value.ToString();
            }

            // ROM table fetches here!
            var tableQuery = from t in xmlDoc.XPathSelectElements("/rom/table")
                             //where table.Ancestors("table").First().IsEmpty
                             select t;
            foreach (XElement table in tableQuery)
            {
                //skip tables with no name
                if (table.Attribute("name") == null) continue;
                foreach (String bt in blobtables)
                {
                    if ((table.Attribute("scaling") != null && table.Attribute("scaling").Value == bt) || (table.Attribute("name") != null && table.Attribute("name").Value == bt))
                    {
                        table.Name = "tableblob";

                        if (isbase)
                            newtables.Add(table.Attribute("name").Value);

                        if (table.Attribute("type") != null)
                            table.Attribute("type").Remove();
                        break;
                    }
                }
                if (isbase)
                {
                    blobtables.AddRange(newtables);
                    newtables.Clear();
                }

                if (table.Name == "tableblob")
                {
                    continue;
                }

                bool xaxis = false;
                bool yaxis = false;

                foreach (XElement xel in table.Descendants())
                {
                    if (xel.Name == "table")
                    {
                        if (xel.Attribute("name") != null && xel.Attribute("name").Value == "X")
                        {
                            xel.Name = "xaxis";
                            xel.Attribute("name").Remove();
                            xaxis = true;
                        }
                        else if (xel.Attribute("type") != null && xel.Attribute("type").Value.ContainsCI("static x axis"))
                        {
                            xel.Name = "staticxaxis";
                            xel.Attribute("type").Remove();
                            xaxis = true;
                        }
                        else if (xel.Attribute("type") != null && xel.Attribute("type").Value.ContainsCI("x axis"))
                        {
                            xel.Name = "xaxis";
                            xel.Attribute("type").Remove();
                            xaxis = true;
                        }
                        else if (xel.Attribute("name") != null && xel.Attribute("name").Value == "Y")
                        {
                            xel.Name = "yaxis";
                            xel.Attribute("name").Remove();
                            yaxis = true;
                        }
                        else if (xel.Attribute("type") != null && xel.Attribute("type").Value.ContainsCI("static y axis"))
                        {
                            xel.Name = "staticyaxis";
                            xel.Attribute("type").Remove();
                            yaxis = true;
                        }
                        else if (xel.Attribute("type") != null && xel.Attribute("type").Value.ContainsCI("y axis"))
                        {
                            xel.Name = "yaxis";
                            xel.Attribute("type").Remove();
                            yaxis = true;
                        }

                    }
                }

                if (!isbase)
                {
                    if (t3d[rombase].Contains(table.Attribute("name").Value.ToString()))
                    {
                        table.Name = "table3d";
                        if (table.Attribute("type") != null) table.Attribute("type").Remove();
                        continue;
                    }
                    if (t2d[rombase].Contains(table.Attribute("name").Value.ToString()))
                    {
                        table.Name = "table2d";
                        if (table.Attribute("type") != null) table.Attribute("type").Remove();
                        continue;
                    }
                    if (t1d[rombase].Contains(table.Attribute("name").Value.ToString()))
                    {
                        table.Name = "table1d";
                        if (table.Attribute("type") != null) table.Attribute("type").Remove();
                        continue;
                    }
                }
                if (xaxis && yaxis) table.Name = "table3d";
                else if (xaxis || yaxis) table.Name = "table2d";
                else table.Name = "table1d";
                if (table.Attribute("type") != null) table.Attribute("type").Remove();
                if (isbase)
                {
                    switch (table.Name.ToString())
                    {
                        case "table3d":
                            t3d[rombase].Add(table.Attribute("name").Value);
                            break;
                        case "table2d":
                            t2d[rombase].Add(table.Attribute("name").Value);
                            break;
                        case "table1d":
                            t1d[rombase].Add(table.Attribute("name").Value);
                            break;
                        default:
                            break;
                    }
                }

            }
            using (XmlTextWriter writer = new XmlTextWriter(fetchPath, new UTF8Encoding(false)))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                xmlDoc.Save(writer);
            }
        }

        public bool ExportXML()
        {
            return ExportXML(this.FilefPath);
        }

        public bool ExportXML(string filepath)
        {
            try
            {
                XmlWriterSettings objXmlWriterSettings = new XmlWriterSettings();
                objXmlWriterSettings.Indent = true;
                objXmlWriterSettings.OmitXmlDeclaration = false;
                using (XmlWriter writer = XmlWriter.Create(filepath, objXmlWriterSettings))
                {
                    MessageBox.Show("Writing definition to: " + filepath);
                    //Start writing doc
                    writer.WriteStartDocument();

                    //Write romid elements
                    //TODO THIS IS REDUNDANT
                    writer.WriteStartElement("rom");
                    writer.WriteStartElement("romid");
                    foreach (KeyValuePair<string, string> kvp in this.CarInfo)
                    {
                        writer.WriteElementString(kvp.Key.ToString(), kvp.Value.ToString());
                    }
                    writer.WriteEndElement();

                    //Write include
                    if (this.Include != null)
                        writer.WriteElementString("include", this.Include.ToString());

                    //Write scalings

                    if (this.ScalingList != null)
                    {
                        foreach (KeyValuePair<string, Scaling> table in this.ScalingList)
                        {
                            table.Value.xml.WriteTo(writer);
                        }
                    }
                    //Write ROM tables
                    if (RomTables != null)
                    {
                        List<Table> romExportList = (from entry in RomTables orderby entry.Category ascending select entry)
                            .ToList();

                        foreach (Table table in romExportList)
                        {
                            table.xml.WriteTo(writer);
                        }
                    }
                    writer.WriteEndDocument();
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error creating definition!");
                Trace.WriteLine(e.Message);
                return false;
            }
        }

        public void CreateXRomId()
        {
            XElement x = new XElement("romid");
            foreach (KeyValuePair<string, string> kvp in this.CarInfo)
            {
                x.Add(new XElement(kvp.Key.ToString(), kvp.Value.ToString()));
            }
            xRomId = x;
        }

        private Table GetBaseTable(string name)
        {
            foreach (Definition d in InheritList)
            {
                if (SharpTuner.AvailableDevices.DefDictionary[d.CalId].ExposedBaseRomTables.ContainsKey(name))
                    return SharpTuner.AvailableDevices.DefDictionary[d.CalId].ExposedBaseRomTables[name];
                else if (SharpTuner.AvailableDevices.DefDictionary[d.CalId].ExposedBaseRamTables.ContainsKey(name))//TODO FIX RAMTABLES
                    return SharpTuner.AvailableDevices.DefDictionary[d.CalId].ExposedBaseRamTables[name];
            }
            Trace.WriteLine("Warning: base table for " + name + " not found");
            return null;
        }

        public void ExposeTable(string name, Lut lut)
        {
            try{
                Table baseTable = GetBaseTable(name);
            if (baseTable != null)
            {

                Table childTable = baseTable.CreateChild(lut, this);
                //TODO: HANDLE STATIC AXES!!
                if (lut.dataAddress < 0x400000)
                {
                    //TODO: HANDLE UPDATES TO EXISTING TABLES!!??
                    if (DefinedRomTables.ContainsKey(childTable.Name))
                        DefinedRomTables.Remove(childTable.Name);
                    DefinedRomTables.Add(childTable.Name, childTable);
                }
                else
                {
                    if (DefinedRamTables.ContainsKey(childTable.Name))
                        DefinedRamTables.Remove(childTable.Name);
                    DefinedRamTables.Add(childTable.Name, childTable);
                }
            }

            //if (bt == null) return;
            //bt.SetAttributeValue("address", lut.dataAddress.ToString("X"));//(System.Int32.Parse(temptable.Value.Attribute("offset").Value.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier) + offset).ToString("X"));
            //IEnumerable<XAttribute> tempattr = bt.Attributes();
            //List<String> remattr = new List<String>();
            //foreach (XAttribute attr in tempattr)
            //{
            //    if (attr.Name != "address" && attr.Name != "name")
            //    {
            //        remattr.Add(attr.Name.ToString());
            //    }
            //}
            //foreach (String rem in remattr)
            //{
            //    bt.Attribute(rem).Remove();
            //}

            //List<String> eleremlist = new List<String>();

            //foreach (XElement ele in bt.Elements())
            //{
            //    IEnumerable<XAttribute> childtempattr = ele.Attributes();
            //    List<String> childremattr = new List<String>();

            //    if (ele.Name.ToString() != "table")
            //    {
            //        eleremlist.Add(ele.Name.ToString());
            //        continue;
            //    }
            //    if (ele.Attribute("type").Value.ContainsCI("static"))
            //    {
            //        eleremlist.Add(ele.Name.ToString());
            //    }
            //    else if (ele.Attribute("type").Value.ContainsCI("x axis"))
            //    {
            //        ele.Attribute("name").Value = "X";
            //    }
            //    else if (ele.Attribute("type").Value.ContainsCI("y axis"))
            //    {
            //        ele.Attribute("name").Value = "Y";
            //    }
            //    foreach (XAttribute attr in childtempattr)
            //    {
            //        if (attr.Name != "address" && attr.Name != "name")
            //        {
            //            childremattr.Add(attr.Name.ToString());
            //        }
            //    }
            //    foreach (String rem in childremattr)
            //    {
            //        ele.Attribute(rem).Remove();
            //    }
            //}
            //foreach (String rem in eleremlist)
            //{
            //    bt.Element(rem).Remove();
            //}
            }
            catch (Exception e)
            {
                Trace.TraceWarning(e.Message);
            }
        }

        public void CopyTables(Definition d)
        {
            DefinedRomTables = new Dictionary<string,Table>(d.DefinedRomTables);
            DefinedRamTables = new Dictionary<string,Table>(d.DefinedRamTables);
            DefinedBaseRomTables = new Dictionary<string, Table>(d.DefinedBaseRomTables);
            DefinedBaseRamTables = new Dictionary<string, Table>(d.DefinedBaseRamTables);
            ScalingList = new Dictionary<string, Scaling>(d.ScalingList);
        }

        #region IDAMAP Code
        public void ImportMapFile(string filepath, DeviceImage image)
        {
            IdaMap idaMap = new IdaMap(filepath);
            ReadMap(idaMap,image);
        }

        public void ImportMapText(string text, DeviceImage image)
        {
            IdaMap idaMap = new IdaMap(text);
            ReadMap(idaMap,image);
        }

        public void ReadMap(IdaMap idaMap,DeviceImage image)
        {
            //loop through base def and search for table names in map
            foreach (var romtable in ExposedBaseRomTables)
            {
                foreach (var idan in idaMap.IdaCleanNames)
                {
                    if (romtable.Key.EqualsIdaString(idan.Key))
                    {
                        ExposeTable(romtable.Key, LutFactory.CreateLut(romtable.Key, uint.Parse(idan.Value.ToString(), NumberStyles.AllowHexSpecifier), image.imageStream));
                        break;
                    }
                }
            }
            ////TODO RAMTABLES
            //foreach (var ramtable in baseDef.RamTableList)
            //{
            //    foreach (var idan in idaMap.IdaCleanNames)
            //    {
            //        if (ramtable.Key.EqualsIdaString(idan.Key))
            //        {
            //            break;
            //        }
            //    }
            //}
        }

        #endregion
    }
}
