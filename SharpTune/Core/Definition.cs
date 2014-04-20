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
using SharpTune.EcuMapTools;
using SharpTune.RomMod;
using System.Runtime.Serialization;
using System.Reflection;

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
        public DefinitionMetaData MetaData { get; private set; }
        
        public bool isBase { get; private set; }
        public String calibrationlId { get { return MetaData.CalibrationIdString; } } //TODO: put this in memorymodel
        public int calibrationIdAddress { get { return (int)MetaData.CalibrationIdAddress; } }

        public String EcuId
        {
            get {
                if (MetaData.EcuIdHexString != null)//TODO: put this in memorymodel.
                    return MetaData.EcuIdHexString;
                else
                    return "Unknown";
            }
        }

        public String CpuBits
        {
            get
            {
                if (MetaData.memoryModel != null)
                    return MetaData.memoryModel.cpubits.ToString();
                else
                    return "Unknown";
            }
        }

        /// <summary>
        /// Contains the file path to the XML definition (top)
        /// </summary>
        public string filePath { get; set; }

        public string include { get { return MetaData.include; } }
        
        public Dictionary<string,Table> ExposedRomTables { get; private set;}
        public Dictionary<string,Table> ExposedRamTables { get; private set;}
        
        public Dictionary<string,Table> InheritedExposedRomTables { 
            get{
                Dictionary<string,Table> ret = new Dictionary<string,Table>();
                foreach(Definition d in inheritList)
                {
                    foreach(Table t in d.ExposedRomTables.Values)
                    {
                        if(!ret.ContainsKey(t.name))
                            ret.Add(t.name,t);
                    }
                }
                return ret;
            }
            private set{}
        }

        public Dictionary<string,Table> InheritedExposedRamTables { 
            get{
                Dictionary<string,Table> ret = new Dictionary<string,Table>();
                foreach(Definition d in inheritList)
                {
                    foreach(Table t in d.ExposedRamTables.Values)
                    {
                        if(!ret.ContainsKey(t.name))
                            ret.Add(t.name,t);
                    }
                }
                return ret;
            }
            private set{}
        }

        public Dictionary<string, Table> AggregateExposedRomTables { 
            get{
                return Utils.AggregateDictionary(ExposedRomTables,InheritedExposedRomTables);
            }
            private set{}
        }
        public Dictionary<string, Table> AggregateExposedRamTables { 
            get{
                return Utils.AggregateDictionary(ExposedRamTables,InheritedExposedRamTables);
            }
            private set{}
        }
        public Dictionary<string,Table> BaseRomTables { get; private set;}
        public Dictionary<string,Table> BaseRamTables { get; private set;}

        public Dictionary<string,Table> InheritedBaseRomTables { 
            get{
                Dictionary<string,Table> ret = new Dictionary<string,Table>();
                foreach(Definition d in inheritList)
                {
                    foreach(Table t in d.BaseRomTables.Values)
                    {
                        if(!ret.ContainsKey(t.name))
                            ret.Add(t.name,t);
                    }
                }
                return ret;
            }
            private set{}
        }

        public Dictionary<string,Table> InheritedBaseRamTables { 
            get{
                Dictionary<string,Table> ret = new Dictionary<string,Table>();
                foreach(Definition d in inheritList)
                {
                    foreach(Table t in d.BaseRamTables.Values)
                    {
                        if(!ret.ContainsKey(t.name))
                            ret.Add(t.name,t);
                    }
                }
                return ret;
            }
            private set{}
        }

        public Dictionary<string, Table> AggregateBaseRomTables { 
            get{
                return Utils.AggregateDictionary(BaseRomTables,InheritedBaseRomTables);
            }
            private set{}
        }
        public Dictionary<string, Table> AggregateBaseRamTables { 
            get{
                return Utils.AggregateDictionary(BaseRamTables,InheritedBaseRamTables);
            }
            private set{}
        }

        public List<Table> RomTables{
            get{
                List<Table> tlist = new List<Table>();
                tlist.AddRange(ExposedRomTables.Values);
                tlist.AddRange(BaseRomTables.Values);
                return tlist;
            }
            private set{}
        }

        public List<Table> RamTables{
            get{
                List<Table> tlist = new List<Table>();
                tlist.AddRange(ExposedRomTables.Values);
                tlist.AddRange(BaseRomTables.Values);
                return tlist;
            }
            private set{}
        }

        public Dictionary<string, Scaling> ScalingList { get; private set; }

        public List<Definition> inheritList { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Definition()
        {
            isBase = false;
            MetaData = new DefinitionMetaData();
            ExposedRomTables = new Dictionary<string, Table>();
            ExposedRamTables = new Dictionary<string, Table>();
            BaseRomTables = new Dictionary<string, Table>();
            BaseRamTables = new Dictionary<string, Table>();
            ScalingList = new Dictionary<string,Scaling>();
            inheritList = new List<Definition>();
        }

        /// <summary>
        /// Constructor
        /// TODO: Change to a factory to share already-opened definitions
        /// TODO: Include more information about inheritance in the class for def-editing
        /// </summary>
        /// <param name="calID"></param>
        public Definition(string filepath)
            : this()
        {
            filePath = filepath;
            //TODO: switch based on current definition schema
            ParseMetaData_ECUFlash();
        }

        public Definition(string respath, bool isres)
            : this()
        {
            filePath = respath;
            ParseMetaData_ECUFlash();
        }

        /// <summary>
        /// Constructor used to create new definitions using existing data
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="carinfo"></param>
        /// <param name="include"></param>
        /// <param name="xromt"></param>
        /// <param name="xramt"></param>
        public Definition(string fp, Mod mod) : this()
        {
            try
            {
                this.filePath = fp;

                MetaData = SharpTuner.AvailableDevices.DefDictionary[mod.InitialCalibrationId].MetaData.Clone();
                MetaData.UpdateFromMod(mod); 
                
                Inherit();

                //TODO: ADD THE TABLES FROM MOD
                //mod.modDef.
                foreach (var rt in mod.modDef.RomLutList)
                {
                    //do something with each lut.
                    ExposeTable(rt.Name, rt); //TODO: Fix this redundancy?
                }
                //TODO: Add RAM tables!
            }
            catch (Exception crap)
            {
                Trace.Write("Error creating definition OBJECT at " + fp);
                throw crap;
            }
        }

        /// <summary>
        /// Read the rom identification header and include from a file
        /// </summary>
        /// <param name="fetchPath"></param>
        /// <returns></returns>
        public void ParseMetaData_ECUFlash()
        {
            try
            {
                XDocument xmlDoc = XDocument.Load(filePath, LoadOptions.PreserveWhitespace);
                XElement xRomId = xmlDoc.XPathSelectElement("/rom/romid");
                MetaData = new DefinitionMetaData();
                string incl;

                if (xmlDoc.XPathSelectElement("/rom/include") != null)
                    incl = xmlDoc.XPathSelectElement("/rom/include").Value.ToString();
                else
                {
                    incl = null;
                    isBase = true;
                }

                MetaData.ParseEcuFlashXml(xRomId, incl);
            }
            catch (Exception e)
            {
                Trace.TraceWarning("Error parsing definition metadata for {0}.",filePath);
                Trace.TraceError(e.Message);
                Trace.TraceError(e.StackTrace);
            }
        }

        public XElement ExportRRRomId()
        {
            return MetaData.ExportRRMetaData();
        }
            
        /// <summary>
        /// Populates a 'short def' (romid + include) into a full definition
        /// </summary>
        /// <returns></returns>
        public bool Populate()
        {
            if (calibrationlId != null && filePath != null)
            {
                Clear();
                if (include != null)
                    Inherit();
                if (ReadXML(filePath))
                    return true;
            }
            return false;
        }

        private void Inherit()
        {
            Dictionary<string, Definition> dd = SharpTuner.AvailableDevices.DefDictionary;
            if (dd.ContainsKey(include) && dd[include].calibrationlId != null)
                dd[include].Populate();

            if(SharpTuner.AvailableDevices.DefDictionary[include].inheritList.Count > 0)
                inheritList.AddRange(SharpTuner.AvailableDevices.DefDictionary[include].inheritList);

            inheritList.Add(SharpTuner.AvailableDevices.DefDictionary[include]);
            inheritList.Reverse();
        }

        private void Clear()
        {
            ExposedRomTables.Clear();
            ExposedRamTables.Clear();
            BaseRomTables.Clear();
            BaseRamTables.Clear();
            ScalingList.Clear();
            inheritList.Clear();
        }

        /// <summary>
        /// Load parameters from XML an XML file
        /// </summary>
        public bool ReadXML(string path)
        {
            if (path == null) return false;
            XDocument xmlDoc = XDocument.Load(path, LoadOptions.PreserveWhitespace);

            //Read Scalings
            try
            {
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
            }
            catch (Exception crap)
            {
                Trace.WriteLine("Error reading scaling in " + path);
                throw crap;
            }

            // ROM table fetches here!
            try
            {
                var tableQuery = from t in xmlDoc.XPathSelectElements("/rom/table")
                                 select t;
                foreach (XElement table in tableQuery)
                {
                    if (table.Attribute("name") == null)
                        continue;
                    string tablename = table.Attribute("name").Value.ToString();
                    AddRomTable(TableFactory.CreateTable(table, tablename, this));
                }
            }
            catch (Exception crap)
            {
                Trace.WriteLine("Error reading tables in " + path);
                throw crap;
            }

            // RAM table feteches here!
            try
            {
                var ramtableQuery = from t in xmlDoc.XPathSelectElements("/ram/table")
                                    select t;
                foreach (XElement table in ramtableQuery)
                {
                    if (table.Attribute("name") == null)
                        continue;
                    string tablename = table.Attribute("name").Value.ToString();

                    AddRamTable(TableFactory.CreateTable(table, tablename, this));
                }
            }
            catch (Exception crap)
            {
                Trace.WriteLine("Error reading RAM tables in " + path);
                throw crap;
            }
            return true;
        }

        private void AddRomTable(Table table)
        {
            if (table.isBase)
            {
                if (!BaseRomTables.ContainsKey(table.name))
                    BaseRomTables.Add(table.name, table);
                else
                    Trace.WriteLine("Warning, duplicate table: " + table.name + ". Please check the definition: " + this.filePath);
            }
            else
            {
                if (!ExposedRomTables.ContainsKey(table.name))
                    ExposedRomTables.Add(table.name, table);
                else
                    Trace.WriteLine("Warning, duplicate table: " + table.name + ". Please check the definition: " + this.filePath);
            }
        }

        private void AddRamTable(Table table)
        {
            if (table.isBase)
            {
                if (!BaseRamTables.ContainsKey(table.name))
                    BaseRamTables.Add(table.name, table);
                else
                    Trace.WriteLine("Warning, duplicate table: " + table.name + ". Please check the definition: " + this.filePath);
            }
            else
            {
                if (!ExposedRamTables.ContainsKey(table.name))
                    ExposedRamTables.Add(table.name, table);
                else
                    Trace.WriteLine("Warning, duplicate table: " + table.name + ". Please check the definition: " + this.filePath);
                    
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

        public bool ExportEcuFlashXML()
        {
            return ExportEcuFlashXML(this.filePath);
        }

        public bool ExportEcuFlashXML(string filepath)
        {
            try
            {
                XmlWriterSettings objXmlWriterSettings = new XmlWriterSettings();
                objXmlWriterSettings.Indent = true;
                objXmlWriterSettings.OmitXmlDeclaration = false;
                using (XmlWriter writer = XmlWriter.Create(filepath, objXmlWriterSettings))
                {
                    //Start writing doc
                    writer.WriteStartDocument();

                    //Write romid elements
                    //TODO THIS IS REDUNDANT
                    writer.WriteStartElement("rom");

                    MetaData.EcuFlashXml.WriteTo(writer);

                    //Write include
                    if (this.include != null)
                        writer.WriteElementString("include", this.include.ToString());

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
                        List<Table> romExportList = (from entry in RomTables orderby entry.category ascending select entry)
                            .ToList();

                        foreach (Table table in romExportList)
                        {
                            try
                            {
                                table.xml.WriteTo(writer);
                            }
                            catch (Exception crap)
                            {
                                Trace.WriteLine("Error exporting xml for table {0}", table.name);
                                throw crap;
                            }
                        }
                    }
                    writer.WriteEndDocument();
                }
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine("Error exporting xml for mod {0}", filepath);
                Trace.WriteLine(e.Message);
                return false;
            }
        }

        private Table GetBaseTable(string name)
        {
            foreach (Definition d in inheritList)
            {
                if (SharpTuner.AvailableDevices.DefDictionary[d.calibrationlId].AggregateBaseRomTables.ContainsKey(name))
                    return SharpTuner.AvailableDevices.DefDictionary[d.calibrationlId].AggregateBaseRomTables[name];
                else if (SharpTuner.AvailableDevices.DefDictionary[d.calibrationlId].AggregateBaseRamTables.ContainsKey(name))//TODO FIX RAMTABLES
                    return SharpTuner.AvailableDevices.DefDictionary[d.calibrationlId].AggregateBaseRamTables[name];
            }
            Trace.WriteLine("Warning: base table for " + name + " not found");
            return null;
        }

        public void ExposeTable(string name, Lut lut)
        {
            Table baseTable = GetBaseTable(name);
            if (baseTable != null)
            {

                Table childTable = baseTable.CreateChild(lut, this);
                //TODO: HANDLE STATIC AXES!!
                if (lut.dataAddress < 0x400000)
                {
                    //TODO: HANDLE UPDATES TO EXISTING TABLES!!??
                    if (ExposedRomTables.ContainsKey(childTable.name))
                        ExposedRomTables.Remove(childTable.name);
                    ExposedRomTables.Add(childTable.name, childTable);
                }
                else
                {
                    if (ExposedRamTables.ContainsKey(childTable.name))
                        ExposedRamTables.Remove(childTable.name);
                    ExposedRamTables.Add(childTable.name, childTable);
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


        ///// <summary>
        ///// Creates a table XEL from the template file, adding proper addresses
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="offset"></param>
        ///// <returns></returns>
        //public void ExposeTable(string name, Lut3D lut) //int offset)
        //{
        //    XElement bt = GetTableBase(name);
        //    if (bt == null) return;
        //    bt.SetAttributeValue("address", lut.dataAddress.ToString("X"));
        //    IEnumerable<XAttribute> tempattr = bt.Attributes();
        //    List<String> remattr = new List<String>();
        //    foreach (XAttribute attr in tempattr)
        //    {
        //        if (attr.Name != "address" && attr.Name != "name")
        //        {
        //            remattr.Add(attr.Name.ToString());
        //        }
        //    }
        //    foreach (String rem in remattr)
        //    {
        //        bt.Attribute(rem).Remove();
        //    }

        //    List<String> eleremlist = new List<String>();

        //    foreach (XElement ele in bt.Elements())
        //    {
        //        IEnumerable<XAttribute> childtempattr = ele.Attributes();
        //        List<String> childremattr = new List<String>();

        //        if (ele.Name.ToString() != "table")
        //        {
        //            eleremlist.Add(ele.Name.ToString());
        //            continue;
        //        }
        //        if (ele.Attribute("type").Value.ContainsCI("static"))
        //        {
        //            eleremlist.Add(ele.Name.ToString());
        //        }
        //        else if (ele.Attribute("type").Value.ContainsCI("x axis"))
        //        {
        //            ele.Attribute("name").Value = "X";
        //            ele.SetAttributeValue("address", lut.colsAddress.ToString("X"));
        //        }
        //        else if (ele.Attribute("type").Value.ContainsCI("y axis"))
        //        {
        //            ele.Attribute("name").Value = "Y";
        //            ele.SetAttributeValue("address", lut.rowsAddress.ToString("X"));
        //        }
        //        foreach (XAttribute attr in childtempattr)
        //        {
        //            if (attr.Name != "address" && attr.Name != "name")
        //            {
        //                childremattr.Add(attr.Name.ToString());
        //            }
        //        }
        //        foreach (String rem in childremattr)
        //        {
        //            ele.Attribute(rem).Remove();
        //        }
        //    }
        //    foreach (String rem in eleremlist)
        //    {
        //        bt.Element(rem).Remove();
        //    }
        //    if (lut.dataAddress < 0x400000)
        //    {
        //        RomTableList.Add(name, TableFactory.CreateTable(bt));
        //    }
        //    else
        //    {
        //        RamTableList.Add(name, TableFactory.CreateTable(bt));
        //    }
        //}

        public void CopyTables(Definition d)
        {
            ExposedRomTables = new Dictionary<string,Table>(d.ExposedRomTables);
            ExposedRamTables = new Dictionary<string,Table>(d.ExposedRamTables);
            BaseRomTables = new Dictionary<string, Table>(d.BaseRomTables);
            BaseRamTables = new Dictionary<string, Table>(d.BaseRamTables);
            ScalingList = new Dictionary<string, Scaling>(d.ScalingList);
        }

        #region ECUFlash XML Code
        public void ImportMapFile(string filepath, DeviceImage image)
        {
            EcuMap im = new EcuMap();
            im.ImportFromMapFileOrText(filepath);
            ReadMap(im,image);
        }

        public void ImportMapText(string text, DeviceImage image)
        {
            EcuMap im = new EcuMap();
            im.ImportFromMapFileOrText(text);
            ReadMap(im,image);
        }

        public void ReadMap(EcuMap idaMap,DeviceImage image)
        {
            //loop through base def and search for table names in map
            foreach (var romtable in AggregateBaseRomTables)
            {
                foreach (var idan in idaMap.CleanLocs)
                {
                    if (romtable.Key.EqualsDefineString(idan.Key))
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

        public bool GetBaseRomTable(string tablename, out Table basetable)
        {
            basetable = null;

            foreach (Definition d in inheritList)
            {
                foreach (Table t in d.RomTables)
                {
                    if (t.name.ToLower() == tablename.ToLower())
                    {
                        basetable = t;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
