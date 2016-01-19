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
    public class ECUMetaData
    {
        public ECUIdentifier ident { get; private set; }
        
        public bool isBase { get; private set; }
        public String calibrationlId { get { return ident.CalibrationIdString; } } //TODO: put this in memorymodel
        public int calibrationIdAddress { get { return (int)ident.CalibrationIdAddress; } }

        public String EcuId
        {
            get {
                if (ident.EcuIdHexString != null)
                    return ident.EcuIdHexString;
                else
                    return "Unknown";
            }
        }

        public String CpuBits
        {
            get
            {
                if (ident.memoryModel != null)
                    return ident.memoryModel.cpubits.ToString();
                else
                    return "Unknown";
            }
        }

        /// <summary>
        /// Contains the file path to the XML definition (top)
        /// </summary>
        public string filePath { get; set; }

        public string include { get { return ident.include; } }
        
        public Dictionary<string,TableMetaData> ExposedRomTables { get; private set;}
        public Dictionary<string,TableMetaData> ExposedRamTables { get; private set;}
        
        public Dictionary<string,TableMetaData> InheritedExposedRomTables { 
            get{
                Dictionary<string,TableMetaData> ret = new Dictionary<string,TableMetaData>();
                foreach(ECUMetaData d in inheritList)
                {
                    foreach(TableMetaData t in d.ExposedRomTables.Values)
                    {
                        if(!ret.ContainsKey(t.name))
                            ret.Add(t.name,t);
                    }
                }
                return ret;
            }
            private set{}
        }

        public Dictionary<string,TableMetaData> InheritedExposedRamTables { 
            get{
                Dictionary<string,TableMetaData> ret = new Dictionary<string,TableMetaData>();
                foreach(ECUMetaData d in inheritList)
                {
                    foreach(TableMetaData t in d.ExposedRamTables.Values)
                    {
                        if(!ret.ContainsKey(t.name))
                            ret.Add(t.name,t);
                    }
                }
                return ret;
            }
            private set{}
        }

        public Dictionary<string, TableMetaData> AggregateExposedRomTables { 
            get{
                return Utils.AggregateDictionary(ExposedRomTables,InheritedExposedRomTables);
            }
            private set{}
        }

        public List<string> AggregateExposedRomTableCategories
        {
            get
            {
                List<string> cats = new List<string>();
                foreach (TableMetaData table in this.AggregateExposedRomTables.Values)
                {
                    if (!cats.ContainsCI(table.category))
                        cats.Add(table.category);
                }
                return cats;
            }
            private set { }
        }

        public Dictionary<string, TableMetaData> AggregateExposedRamTables { 
            get{
                return Utils.AggregateDictionary(ExposedRamTables,InheritedExposedRamTables);
            }
            private set{}
        }
        public Dictionary<string,TableMetaData> BaseRomTables { get; private set;}
        public Dictionary<string,TableMetaData> BaseRamTables { get; private set;}

        public Dictionary<string,TableMetaData> InheritedBaseRomTables { 
            get{
                Dictionary<string,TableMetaData> ret = new Dictionary<string,TableMetaData>();
                foreach(ECUMetaData d in inheritList)
                {
                    foreach(TableMetaData t in d.BaseRomTables.Values)
                    {
                        if(!ret.ContainsKey(t.name))
                            ret.Add(t.name,t);
                    }
                }
                return ret;
            }
            private set{}
        }

        public Dictionary<string,TableMetaData> InheritedBaseRamTables { 
            get{
                Dictionary<string,TableMetaData> ret = new Dictionary<string,TableMetaData>();
                foreach(ECUMetaData d in inheritList)
                {
                    foreach(TableMetaData t in d.BaseRamTables.Values)
                    {
                        if(!ret.ContainsKey(t.name))
                            ret.Add(t.name,t);
                    }
                }
                return ret;
            }
            private set{}
        }

        public Dictionary<string, TableMetaData> AggregateBaseRomTables { 
            get{
                return Utils.AggregateDictionary(BaseRomTables,InheritedBaseRomTables);
            }
            private set{}
        }
        public Dictionary<string, TableMetaData> AggregateBaseRamTables { 
            get{
                return Utils.AggregateDictionary(BaseRamTables,InheritedBaseRamTables);
            }
            private set{}
        }

        public List<TableMetaData> RomTables{
            get{
                List<TableMetaData> tlist = new List<TableMetaData>();
                tlist.AddRange(ExposedRomTables.Values);
                tlist.AddRange(BaseRomTables.Values);
                return tlist;
            }
            private set{}
        }

        public List<TableMetaData> RamTables{
            get{
                List<TableMetaData> tlist = new List<TableMetaData>();
                tlist.AddRange(ExposedRomTables.Values);
                tlist.AddRange(BaseRomTables.Values);
                return tlist;
            }
            private set{}
        }

        public Dictionary<string, Scaling> ScalingList { get; private set; }

        public List<ECUMetaData> inheritList { get; private set; }

        private readonly AvailableDevices availableDevices;

        /// <summary>
        /// Constructor
        /// </summary>
        public ECUMetaData(AvailableDevices ad)
        {
            availableDevices = ad; 
            isBase = false;
            ident = new ECUIdentifier();
            ExposedRomTables = new Dictionary<string, TableMetaData>();
            ExposedRamTables = new Dictionary<string, TableMetaData>();
            BaseRomTables = new Dictionary<string, TableMetaData>();
            BaseRamTables = new Dictionary<string, TableMetaData>();
            ScalingList = new Dictionary<string,Scaling>();
            inheritList = new List<ECUMetaData>();
        }

        /// <summary>
        /// Constructor
        /// TODO: Change to a factory to share already-opened definitions
        /// TODO: Include more information about inheritance in the class for def-editing
        /// </summary>
        /// <param name="calID"></param>
        public ECUMetaData(AvailableDevices ad, string filepath)
            : this(ad)
        {
            filePath = filepath;
            //TODO: switch based on current definition schema
            ParseMetaData_ECUFlash();
        }

        public ECUMetaData(AvailableDevices ad, string respath, bool isres)
            : this(ad)
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
        public ECUMetaData(AvailableDevices ad, string fp, Mod mod) : this(ad)
        {
            try
            {
                this.filePath = fp;

                ident = availableDevices.DefDictionary[mod.InitialCalibrationId].ident.Clone();
                ident.UpdateFromMod(mod); 
                
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
                throw;
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
                ident = new ECUIdentifier();
                string incl;

                if (xmlDoc.XPathSelectElement("/rom/include") != null)
                    incl = xmlDoc.XPathSelectElement("/rom/include").Value.ToString();
                else
                {
                    incl = null;
                    isBase = true;
                }

                ident.ParseEcuFlashXml(xRomId, incl);
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
            return ident.ExportRRMetaData();
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
                if (ReadXML())
                    return true;
            }
            return false;
        }

        private void Inherit()
        {
            Dictionary<string, ECUMetaData> dd = availableDevices.DefDictionary;
            if (dd.ContainsKey(include) && dd[include].calibrationlId != null)
                dd[include].Populate();

            if(availableDevices.DefDictionary[include].inheritList.Count > 0)
                inheritList.AddRange(availableDevices.DefDictionary[include].inheritList);

            inheritList.Add(availableDevices.DefDictionary[include]);
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
        public bool ReadXML()
        {
            if (this.filePath == null) return false;
            LoadOptions lo = LoadOptions.SetLineInfo | LoadOptions.PreserveWhitespace;
            XDocument xmlDoc = XDocument.Load(filePath, lo);

            //Read Scalings
            IXmlLineInfo info = null;

            bool scalingTest = ReadEcuFlashScalings(xmlDoc, info);
            bool romTableTest = ReadEcuFlashRomTables(xmlDoc, info);
            bool ramTableTest = ReadEcuFlashRamTables(xmlDoc, info);

            return scalingTest && romTableTest && ramTableTest;
        }

        public bool ReadEcuFlashScalings(XDocument xmlDoc, IXmlLineInfo info)
        {
            var scalingQuery = from sc in xmlDoc.XPathSelectElements("/rom/scaling")
                                //where table.Ancestors("table").First().IsEmpty
                                select sc;
            foreach (XElement scaling in scalingQuery)
            {
                try{
                    info = (IXmlLineInfo)scaling;
                    //skip scalings with no name
                    if (scaling.Attribute("name") == null) throw new Exception("Error, scaling name is null!");
                    string scalingname = scaling.Attribute("name").Value.ToString();
                    if (!this.ScalingList.ContainsKey(scalingname))
                    {
                        this.ScalingList.Add(scalingname, ScalingFactory.CreateScaling(scaling));
                    }
                }
                catch (Exception crap)
                {
                    Trace.WriteLine("Error reading scaling in " + filePath + " Line number: " + info.LineNumber);
                    Trace.WriteLine(crap.Message);
                    throw;
                }
            }
            return true;
        }

        public bool ReadEcuFlashRomTables(XDocument xmlDoc, IXmlLineInfo info)
        {
            // ROM table fetches here!
            var tableQuery = from t in xmlDoc.XPathSelectElements("/rom/table")
                                select t;
            foreach (XElement table in tableQuery)
            {
                try
                {
                    info = (IXmlLineInfo)table;
                    if (table.Attribute("name") == null)
                        throw new Exception("Error, table name is null!");
                    string tablename = table.Attribute("name").Value.ToString();
                    AddRomTable(TableFactory.CreateTable(table, tablename, this), info.LineNumber);
                }
                catch (Exception crap)
                {
                    Trace.WriteLine("Error reading tables in " + filePath + " Line number: " + info.LineNumber);
                    Trace.WriteLine(crap.Message);
                    throw;
                }
            }
            return true;
        }

        public bool ReadEcuFlashRamTables(XDocument xmlDoc, IXmlLineInfo info)
        {
            // RAM table feteches here!
            var ramtableQuery = from t in xmlDoc.XPathSelectElements("/ram/table")
                                select t;
            foreach (XElement table in ramtableQuery)
            {
                try
                {
                    info = (IXmlLineInfo)table;
                    if (table.Attribute("name") == null)
                        continue;
                    string tablename = table.Attribute("name").Value.ToString();

                    AddRamTable(TableFactory.CreateTable(table, tablename, this), info.LineNumber);
                }
                catch (Exception crap)
                {
                    Trace.WriteLine("Error reading RAM tables in " + filePath + " Line number: " + info.LineNumber);
                    Trace.WriteLine(crap.Message);
                    throw;
                }
            }
            return true;  
        }

        private void AddRomTable(TableMetaData table, int line)
        {
            if (table.isBase)
            {
                if (!BaseRomTables.ContainsKey(table.name))
                    BaseRomTables.Add(table.name, table);
                else
                    Trace.WriteLine("Warning, duplicate table: " + table.name + ". Please check the definition: " + this.filePath + " Line number: " + line);
            }
            else
            {
                if (!ExposedRomTables.ContainsKey(table.name))
                    ExposedRomTables.Add(table.name, table);
                else
                    Trace.WriteLine("Warning, duplicate table: " + table.name + ". Please check the definition: " + this.filePath + " Line number: " + line);
            }
        }

        private void AddRamTable(TableMetaData table, int line)
        {
            if (table.isBase)
            {
                if (!BaseRamTables.ContainsKey(table.name))
                    BaseRamTables.Add(table.name, table);
                else
                    Trace.WriteLine("Warning, duplicate table: " + table.name + ". Please check the definition: " + this.filePath + " Line number: " + line);
            }
            else
            {
                if (!ExposedRamTables.ContainsKey(table.name))
                    ExposedRamTables.Add(table.name, table);
                else
                    Trace.WriteLine("Warning, duplicate table: " + table.name + ". Please check the definition: " + this.filePath + " Line number: " + line);
                    
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

                    ident.EcuFlashXml_SH705x.WriteTo(writer);

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
                        List<TableMetaData> romExportList = (from entry in RomTables orderby entry.category ascending select entry)
                            .ToList();

                        foreach (TableMetaData table in romExportList)
                        {
                            try
                            {
                                table.xml.WriteTo(writer);
                            }
                            catch (Exception crap)
                            {
                                Trace.WriteLine("Error exporting xml for table {0}", table.name);
                                throw;
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

        private TableMetaData GetBaseTable(string name)
        {
            foreach (ECUMetaData d in inheritList)
            {
                if (availableDevices.DefDictionary[d.calibrationlId].AggregateBaseRomTables.ContainsKey(name))
                    return availableDevices.DefDictionary[d.calibrationlId].AggregateBaseRomTables[name];
                else if (availableDevices.DefDictionary[d.calibrationlId].AggregateBaseRamTables.ContainsKey(name))//TODO FIX RAMTABLES
                    return availableDevices.DefDictionary[d.calibrationlId].AggregateBaseRamTables[name];
            }
            Trace.WriteLine("Warning: base table for " + name + " not found");
            return null;
        }

        public void ExposeTable(string name, LookupTable lut)
        {
            TableMetaData baseTable = GetBaseTable(name);
            if (baseTable != null)
            {

                TableMetaData childTable = baseTable.CreateChild(lut, this);
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

        public void CopyTables(ECUMetaData d)
        {
            ExposedRomTables = new Dictionary<string,TableMetaData>(d.ExposedRomTables);
            ExposedRamTables = new Dictionary<string,TableMetaData>(d.ExposedRamTables);
            BaseRomTables = new Dictionary<string, TableMetaData>(d.BaseRomTables);
            BaseRamTables = new Dictionary<string, TableMetaData>(d.BaseRamTables);
            ScalingList = new Dictionary<string, Scaling>(d.ScalingList);
        }

        #region ECUFlash XML Code
        public void ImportMapFile(string filepath, ECU image)
        {
            EcuMap im = new EcuMap();
            im.ImportFromMapFileOrText(filepath);
            ReadMap(im,image);
        }

        public void ImportMapText(string text, ECU image)
        {
            EcuMap im = new EcuMap();
            im.ImportFromMapFileOrText(text);
            ReadMap(im,image);
        }

        public void ReadMap(EcuMap idaMap,ECU image)
        {
            //loop through base def and search for table names in map
            foreach (var romtable in AggregateBaseRomTables)
            {
                foreach (var idan in idaMap.CleanLocs)
                {
                    if (romtable.Key.EqualsDefineString(idan.Key))
                    {
                        ExposeTable(romtable.Key, LookupTableFactory.CreateLookupTable(romtable.Key, uint.Parse(idan.Value.ToString(), NumberStyles.AllowHexSpecifier), image.imageStream));
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

        public bool GetBaseRomTable(string tablename, out TableMetaData basetable)
        {
            basetable = null;

            foreach (ECUMetaData d in inheritList)
            {
                foreach (TableMetaData t in d.RomTables)
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
