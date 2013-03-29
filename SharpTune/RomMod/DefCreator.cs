using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;
using System.Data;
using SharpTune;
using SharpTuneCore;
using System.IO;
using System.Text.RegularExpressions;
using SharpTune.ConversionTools;
using SharpTune.Core;

namespace SharpTune.RomMod
{
    class DefCreator
    {
        private const int tableaddresslimit = 1024;

        private const string filepath = "ModDefTemplate.xml";
        private const uint defMetadataAddress = 0x00F00000;
        private const uint OpIdent = 0x43210000;
        private const uint OpIdentAddr = 0x43210001;
        private const uint OpTable1d = 0x43210002;
        private const uint OpTable2d = 0x43210003;
        private const uint OpTable3d = 0x43210004;
        private const uint OpX = 0x43210005;
        private const uint OpY = 0x43210006;
        private const uint OpStaticY = 0x43210007;
        private const uint OpRAM = 0x43210008;

        private Dictionary<string,Table> RomTableList {get; set;}
        private Dictionary<string,Table> RamTableList { get; set; }

        private Mod parentMod { get; set; }
        private Blob defBlob { get; set; }

        //private string finalEcuId {get; set;}
        //private int EcuIdAddress {get; set; }
        //private string inheritedIdentifier { get; set; }
        //private string inheritedEcuId {get; set; }

        private string outputPath {get; set;}

        private Definition definition { get; set; }
        private Definition inheritedDefinition {get; set;}
        private Definition baseDefinition { get; set; }
        public DefCreator(Mod parent)
        {
            this.parentMod = parent;
        }

        #region Patch Reading Code
        /// <summary>
        /// Cycles through the template definition and replaces "0" addresses with addresses from the patch file.
        /// TODO: Replace template with inheritance from 32BITBASE Tables and a patch parameter that specifies the child template to use.
        /// </summary>
        /// <returns></returns>
        public bool TryReadDefs(String defPath)//TODO remove defpath and ref the static global
        {
            
            RomTableList = new Dictionary<string,Table>();
            RamTableList = new Dictionary<string, Table>();
            
            List<Blob> blobs = parentMod.blobList.Blobs;
            Blob metadataBlob;
            if (!this.parentMod.TryGetMetaBlob(defMetadataAddress, 10, out metadataBlob, blobs))
            {
                Console.WriteLine("This patch file does not contain metadata.");
                return false;
            }
            this.defBlob = metadataBlob;
            int offs = 0;

            this.baseDefinition = SharpTuner.availableDevices.DefDictionary["32BITBASE"];// new Definition((Utils.DirectorySearch(defPath, "32BITBASE")));
            this.baseDefinition.Populate(); // ReadXML((Utils.DirectorySearch(defPath, "32BITBASE")));
            this.inheritedDefinition = SharpTuner.availableDevices.DefDictionary[parentMod.InitialCalibrationId];// new Definition(Utils.DirectorySearch(defPath, this.parentMod.InitialCalibrationId));
            this.inheritedDefinition.Populate();// ReadXML(Utils.DirectorySearch(defPath, this.parentMod.InitialCalibrationId));
            
            if (this.parentMod.buildConfig != null)
                outputPath = defPath + "/MerpMod/" + this.parentMod.buildConfig + "/";
            outputPath += this.parentMod.ModIdent.ToString() + ".xml";
            XElement xci = inheritedDefinition.xRomId;
            Dictionary<string,string> tci = this.inheritedDefinition.carInfo;
            tci["internalidaddress"] = this.parentMod.ModIdentAddress.ToString("X");
            tci["internalidstring"] = this.parentMod.ModIdent.ToString();
            tci["ecuid"] = this.parentMod.FinalEcuId.ToString();
            tci["xmlid"] = this.parentMod.ModIdent.ToString();
            string tincl = this.parentMod.InitialCalibrationId.ToString();
            //TODO: First create short definition, then add tables 
            definition = new Definition(outputPath, tci, tincl);
  
            if (!TryParseDefs(this.defBlob, ref offs, defPath)) return false;

            if (!TryCleanDef()) return false;

            //prompt to select logger type
            NewRRLogDefInheritWithTemplate(this.RamTableList, SharpTuner.RRLoggerDefPath + @"\MerpMod\" + parentMod.buildConfig + @"\" + parentMod.ModIdent + ".xml", SharpTuner.RRLoggerDefPath + @"\MerpMod\base.xml", parentMod.InitialEcuId.ToString(),parentMod.FinalEcuId.ToString());
            return true;
        }

        public static void NewRRLogDefInheritWithTemplate(Dictionary<string, Table> ramTableList, string outPath, string template, string inheritIdent, string ident)
        {
            XDocument xmlDoc = SelectGetRRLogDef();
            InheritRRLogger(ref xmlDoc, outPath, inheritIdent, ident);
            AddRRLogDefBase(ref xmlDoc, outPath, template);
            PopulateRRLogDefTables(ref xmlDoc, outPath, ramTableList, ident);
            xmlDoc.SaveToFile(outPath);
        }

        public static void DefineRRLogEcu(string mapFile, string ident)
        {
            IdaMap im = new IdaMap(mapFile);
            Dictionary<string, string> addMap = new Dictionary<string, string>();
            Dictionary<string, string> defMap = ReadRRLogDefExtTables();
            foreach (KeyValuePair<string, string> def in im.IdaNames)
            {
                foreach (KeyValuePair<string, string> table in defMap)
                {
                    if (def.Key.ContainsCI("Ext_"))
                    {
                        string[] a = Regex.Split(def.Key, "Ext_E");
                        string[] b = Regex.Split(table.Key, "E");
                        if (a.Length == 2 && b.Length == 2 && a[1].EqualsCI(b[1]))
                        {
                            addMap.Add(table.Value, def.Value.Replace("FFFF", "FF"));
                            break;
                        }
                    }
                }
            }
            string ld = SelectRRLogDef();
            XDocument xmlDoc = XDocument.Load(SharpTuner.RRLoggerDefPath + ld);//, LoadOptions.PreserveWhitespace);
            PopulateRRLogDefTables(ref xmlDoc, SharpTuner.RRLoggerDefPath + ld, addMap, ident);
            //xmlDoc.SaveToFile(SharpTuner.RRLoggerDefPath + ld);
        }

        private static Dictionary<string, string> ReadRRLogDefExtTables()
        {
            Dictionary<string, string> ls = new Dictionary<string, string>();
            string ld = SelectRRLogDef();
            XDocument xmlDoc = XDocument.Load(SharpTuner.RRLoggerDefPath + ld);//, LoadOptions.PreserveWhitespace);
            string bxp = "./logger/protocols/protocol/ecuparams/ecuparam";
            IEnumerable<XElement> xbase = xmlDoc.XPathSelectElements(bxp);

            foreach (XElement xb in xbase)
            {
                ls.Add(xb.Attribute("id").Value.ToString(), xb.Attribute("name").Value.ToString());
            }
            return ls;
        }

        private static List<string> GetRRLoggerDefs()
        {
            List<string> loggerdefs = new List<string>();
            List<string> remlist = new List<string>();

            loggerdefs.AddRange(Directory.GetFiles(SharpTuner.RRLoggerDefPath));
            loggerdefs.FilterOnly(".xml");

            for (int i = 0; i < loggerdefs.Count; i++)
            {
                loggerdefs[i] = Path.GetFileName(loggerdefs[i]);
            }
            loggerdefs.Sort();
            loggerdefs.Reverse();
            return loggerdefs;
        }

        private static string SelectRRLogDef()
        {
            return SimpleCombo.ShowDialog("Select logger base", "Select logger base", GetRRLoggerDefs());
        }

        private static XDocument SelectGetRRLogDef()
        {
            string ld = SelectRRLogDef();
            XDocument xmlDoc = XDocument.Load(SharpTuner.RRLoggerDefPath + ld);//, LoadOptions.PreserveWhitespace);
            XDocument xmlDoc2 = new XDocument(xmlDoc);
            return xmlDoc2;
        }

        private static void AddRRLogDefBase(ref XDocument xmlDoc, string outPath, string templatePath)
        {
            XDocument xmlBase = XDocument.Load(templatePath);//, LoadOptions.PreserveWhitespace);
            string bxp = "./logger/protocols/protocol/ecuparams/ecuparam";
            IEnumerable<XElement> xbase = xmlBase.XPathSelectElements(bxp);

            foreach (XElement xb in xbase)
            {
                xmlDoc.XPathSelectElement("./logger/protocols/protocol/ecuparams").Add(xb);
            }
            xmlDoc.SaveToFile(outPath);
        }

        private static void PopulateRRLogDefTables(ref XDocument xmlDoc, string outPath, Dictionary<string, Table> ramTableList, string ident)
        {
            foreach (KeyValuePair<string, Table> table in ramTableList)
            {
                string xp = "./logger/protocols/protocol/ecuparams/ecuparam[@name='" + table.Key.ToString() + "']";
                XElement exp = xmlDoc.XPathSelectElement(xp);

                string cxp = "./logger/protocols/protocol/ecuparams/ecuparam[@name='" + table.Key.ToString() + "']/conversions";
                XElement cexp = xmlDoc.XPathSelectElement(cxp);


                if (exp != null)
                {
                    string ch = "//ecuparam[@name='" + table.Key.ToString() + "']/ecu[@id='" + ident + "']";
                    XElement check = exp.XPathSelectElement(ch);
                    if (check != null) check.Remove();
                    cexp.AddBeforeSelf(table.Value.xml);
                }
            }
            xmlDoc.SaveToFile(outPath);
        }

        private static void PopulateRRLogDefTables(ref XDocument xmlDoc, string outPath, Dictionary<string, string> ramTableList, string ident)
        {
            foreach (KeyValuePair<string, string> table in ramTableList)
            {
                string xp = "./logger/protocols/protocol/ecuparams/ecuparam[@name='" + table.Key.ToString() + "']";
                XElement exp = xmlDoc.XPathSelectElement(xp);

                string cxp = "./logger/protocols/protocol/ecuparams/ecuparam[@name='" + table.Key.ToString() + "']/conversions";
                XElement cexp = xmlDoc.XPathSelectElement(cxp);

                if (exp != null)
                {
                    string ch = "//ecuparam[@name='" + table.Key.ToString() + "']/ecu[@id='" + ident + "']";
                    XElement check = exp.XPathSelectElement(ch);
                    XElement ad;
                    if (check == null)
                        ad = new XElement(exp.Descendants("ecu").First());
                    else
                        ad = new XElement(check);
                    ad.Attribute("id").Value = ident;
                    ad.Descendants().First().Value = "0x" + table.Value;
                    cexp.AddBeforeSelf(ad);

                    //if(exp.Descendants("ecu") != null)
                    //{
                    //    if (exp.Descendants("ecu").First().Descendants("address") != null)
                    //        len = exp.Descendants("ecu").First().Descendants("address").First().Attribute("length").Value.ToString();
                    //}
                    //exp.AddFirst("0x" + table.Value);
                }
            }
            xmlDoc.SaveToFile(outPath);
        }

        private static void InheritRRLogger(ref XDocument xmlDoc, string outPath, string inheritIdent, string newIdent)
        {
            //inherit from a base file
            //todo add list of exemptions to skip???
            string paramxp = "./logger/protocols/protocol/ecuparams";
            XElement pexp = xmlDoc.XPathSelectElement(paramxp);
            try
            {
                foreach (XElement xel in pexp.Elements())
                {
                    if (xel.Elements("ecu") != null)
                    {

                        foreach (XElement xecu in xel.Elements("ecu"))
                        {
                            string id = xecu.Attribute("id").Value.ToString();
                            if (id == inheritIdent)//parentMod.InitialEcuId.ToString())
                            {
                                //found hit, copy the ecu xel
                                XElement newxecu = new XElement(xecu);
                                newxecu.Attribute("id").SetValue(newIdent);///parentMod.FinalEcuId.ToString());
                                xel.AddFirst(newxecu);
                            }
                        }
                    }
                }
                xmlDoc.SaveToFile(outPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private KeyValuePair<string, Table> CreateRomRaiderRamTable(string name, int offset, string id, int length)
        {
            XElement xel = XElement.Parse(@"
                <ecu id="""">
                    <address length=""""></address>
                </ecu>
            ");

            xel.Attribute("id").Value = this.parentMod.FinalEcuId;
            xel.Element("address").Value = "0x" + offset.ToString("X6").Substring(2, 6);
            xel.Element("address").Attribute("length").Value = length.ToString();

            return new KeyValuePair<string, Table>(name, TableFactory.CreateTable(xel));
        }

        private bool TryCleanDef()
        {
            List<string> removelist = new List<string>();
            foreach (KeyValuePair<string, Table> table in this.definition.RomTableList)
            {
                if (table.Value.xml.Attribute("address") != null)
                {
                    if (System.Int32.Parse(table.Value.xml.Attribute("address").Value.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier) < tableaddresslimit)
                    {
                        removelist.Add(table.Key.ToString());
                        continue;
                    }

                }
            }
            foreach (string table in removelist)
            {
                this.definition.RomTableList.Remove(table);
            }

            //same operation for ramtables
            removelist.Clear();
            foreach (KeyValuePair<string, Table> table in this.definition.RamTableList)
            {
                if (table.Value.xml.Attribute("address") != null)
                {
                    if (System.Int32.Parse(table.Value.xml.Attribute("address").Value.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier) < tableaddresslimit)
                    {
                        removelist.Add(table.Key.ToString());
                        continue;
                    }

                }
            }
            foreach (string table in removelist)
            {
                this.definition.RamTableList.Remove(table);
            }
            return true;
        }

        /// <summary>
        /// Try to read the Patch metadata from the file.
        /// </summary>
        private bool TryParseDefs(Blob metadata, ref int offset, String defPath)
        {
            UInt32 cookie = 0;
            while ((metadata.Content.Count > offset + 8) &&
                metadata.TryGetUInt32(ref cookie, ref offset))
            {
                //if (cookie == OpIdent)
                //{
                //    string metaString = null;
                //    string metaString1 = null;
                //    string metaString2 = null;
                //    uint metaOffset = 0;
                //    uint metaOffset1 = 0;
                //    uint metaOffset2 = 0;
                //    if (this.TryReadDefData(metadata, out metaString, out metaOffset, ref offset))
                //    {
                //        if (this.TryReadDefData(metadata, out metaString1, out metaOffset1, ref offset))
                //        {
                //            if (this.TryReadDefData(metadata, out metaString2, out metaOffset2, ref offset))
                //            {
                //                // found modName, output to string!
                //                this.finalEcuId = metaString;
                //                this.EcuIdAddress = (int)metaOffset;
                //                this.inheritedIdentifier = metaString1;
                //                this.inheritedEcuId = metaString2;

                //                //READ INHERITED DEFINITION
                //                inheritedDefinition = new Definition((Definition.DirectorySearch(defPath, this.inheritedIdentifier)));
                //                inheritedDefinition.ReadXML(inheritedDefinition.defPath, false, false);
                //            }
                //        }
                //        else
                //        {
                //            Console.WriteLine("Invalid definition found.");
                //            return false;
                //        }
                //    }
                //}
                if (cookie == OpTable3d)//TODO CONDITIONALLY READ LUTS!
                {
                    string metaString = null;
                    uint metaOffset = 0;
                    if (this.TryReadDefData(metadata, out metaString, out metaOffset, ref offset))
                    {
                        Blob tableBlob;
                        this.parentMod.TryGetMetaBlob(metaOffset, 10, out tableBlob, this.parentMod.blobList.Blobs);
                        Lut3D lut = new Lut3D(tableBlob, metaOffset);
                        //KeyValuePair<String, Table> tempTable = CreateTable(metaString, lut);
                        if (metaString != null) definition.ExposeTable(metaString, lut);// this.RomTableList.Add(tempTable.Key, tempTable.Value);
                    }
                    else
                    {
                        Console.WriteLine("Invalid definition found.");
                        return false;
                    }
                }
                else if (cookie == OpTable1d)// //TODO!! FIX FOR TABLE2D || cookie == OpTable2d)
                {
                    string metaString = null;
                    uint metaOffset = 0;
                    if (this.TryReadDefData(metadata, out metaString, out metaOffset, ref offset))
                    {
                        //Blob tableBlob;
                        //this.parentMod.TryGetMetaBlob(metaOffset, 10, out tableBlob, this.parentMod.blobList.Blobs);
                        Lut lut = new Lut(metaOffset);
                        //KeyValuePair<String,Table> tempTable = CreateTable(metaString, (int)metaOffset);//lut);
                        if (metaString != null) definition.ExposeTable(metaString, lut); //this.RomTableList.Add(tempTable.Key, tempTable.Value);
                    }
                    else
                    {
                        Console.WriteLine("Invalid definition found.");
                        return false;
                    }
                }
                else if (cookie == OpRAM)
                {
                    string paramName = null;
                    string paramId = null;
                    uint paramOffset = 0;
                    uint paramLenght = 0;
                    if (this.TryReadDefData(metadata, out paramId, out paramOffset, ref offset))
                    {
                        if (this.TryReadDefData(metadata, out paramName, out paramLenght, ref offset))
                        {
                            // found modName, output to string!
                            KeyValuePair<String, Table> tempTable = CreateRomRaiderRamTable(paramName, (int)paramOffset, paramId, (int)paramLenght);
                            if (tempTable.Key != null) this.RamTableList.Add(tempTable.Key, tempTable.Value);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid definition found.");
                        return false;
                    }
                }
                else if (cookie == Mod.endoffile)
                {
                    break;
                }
            }

            return true;
        }

        /// <summary>
        /// Read a single string from the metadata blob.
        /// </summary>
        /// <remarks>
        /// Consider returning false, printing error message.  But, need to 
        /// be certain to abort the whole process at that point...
        /// </remarks>
        public bool TryReadDefData(Blob metadata, out string metaString, out uint metaOffset, ref int offset)
        {
            metaString = null;
            metaOffset = 0;
            UInt32 cookie = 0;
            List<byte> tempbytelist = new List<byte>();
            metadata.TryGetUInt32(ref metaOffset, ref offset);


            while ((metadata.Content.Count > offset + 8) &&
                metadata.TryGetUInt32(ref cookie, ref offset))
            {
                if ((cookie < 0x43210010 && cookie > 0x43210000) || cookie == 0x00090009)
                {
                    if (cookie != 0x00090009)
                    {
                        offset -= 4;
                    }

                    char[] splitter = { '\0' };
                    string tempstring = System.Text.Encoding.ASCII.GetString(tempbytelist.ToArray());
                    metaString = tempstring.Split(splitter)[0];
                    return true;
                }

                byte tempbyte = new byte();
                offset -= 4;
                for (int i = 0; i < 4; i++)
                {
                    if (!metadata.TryGetByte(ref tempbyte, ref offset))
                    {
                        return false;
                    }
                    tempbytelist.Add(tempbyte);

                }

            }

            tempbytelist.ToString();
            return false;
        }

        /// <summary>
        /// Prints out an ECUFlash XML definition of the table
        /// </summary>
        public bool TryPrintEcuFlashDef()
        {
            if(this.definition.ExportXML(outputPath))
                return true;
            return false;
        }
        #endregion

        #region RR XML Code
        /// <summary>
        /// Creates a table XEL from the template file, adding proper addresses
        /// </summary>
        /// <param name="name"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private KeyValuePair<string, Table> CreateTable(string name, int offset)
        {
            //Dictionary<string, Table> list = 

            foreach (KeyValuePair<string, Table> table in this.baseDefinition.RomTableList)
            {
                if (table.Key == name)
                {
                    KeyValuePair<string, Table> temptable = new KeyValuePair<string,Table>(name, table.Value);
                    temptable.Value.xml.SetAttributeValue("address",offset.ToString("X"));//(System.Int32.Parse(temptable.Value.Attribute("offset").Value.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier) + offset).ToString("X"));
                    IEnumerable<XAttribute> tempattr = temptable.Value.xml.Attributes();
                    List<String> remattr = new List<String>();
                    foreach (XAttribute attr in tempattr)
                    {
                        if (attr.Name != "address" && attr.Name != "name")
                        {
                            remattr.Add(attr.Name.ToString());
                        }
                    }
                    foreach (String rem in remattr)
                    {
                        temptable.Value.xml.Attribute(rem).Remove();
                    }

                    List<String> eleremlist = new List<String>();

                    foreach (XElement ele in temptable.Value.xml.Elements())
                    {
                        IEnumerable<XAttribute> childtempattr = ele.Attributes();
                        List<String> childremattr = new List<String>();

                        if (ele.Name.ToString() != "table")
                        {
                            eleremlist.Add(ele.Name.ToString());
                            continue;
                        }
                        if (ele.Attribute("type").Value.ContainsCI("static"))
                        {
                            eleremlist.Add(ele.Name.ToString());
                        }
                        else if (ele.Attribute("type").Value.ContainsCI("x axis"))
                        {
                            ele.Attribute("name").Value = "X";
                        }
                        else if (ele.Attribute("type").Value.ContainsCI("y axis"))
                        {
                            ele.Attribute("name").Value = "Y";
                        }
                        foreach (XAttribute attr in childtempattr)
                        {
                            if (attr.Name != "address" && attr.Name != "name")
                            {
                                childremattr.Add(attr.Name.ToString());
                            }
                        }
                        foreach (String rem in childremattr)
                        {
                            ele.Attribute(rem).Remove();
                        }
                    }
                    foreach (String rem in eleremlist)
                    {
                        temptable.Value.xml.Element(rem).Remove();
                    }
                    return temptable;
                }
            }
            if (this.baseDefinition.RamTableList != null)
            {
                foreach (KeyValuePair<string, Table> table in this.baseDefinition.RamTableList)
                {
                    if (table.Key == name)
                    {
                        KeyValuePair<string, Table> temptable = new KeyValuePair<string,Table>(name,table.Value);
                        //TABLE WAS FOUND!
                        return temptable;
                    }
                }
            }
            return new KeyValuePair<string,Table>();
        }

        /// <summary>
        /// Creates a table XEL from the template file, adding proper addresses
        /// </summary>
        /// <param name="name"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private KeyValuePair<string,Table> CreateTable(string name, Lut3D lut) //int offset)
        { 
            Dictionary<string,Table> list = this.baseDefinition.RomTableList;

            foreach (KeyValuePair<string,Table> table in this.baseDefinition.RomTableList)
            {
                if (table.Key == name)
                {
                    KeyValuePair<string, Table> temptable = new KeyValuePair<string,Table>(name, table.Value);
                    temptable.Value.xml.SetAttributeValue("address", lut.dataAddress.ToString("X"));
                    IEnumerable<XAttribute> tempattr = temptable.Value.xml.Attributes();
                    List<String> remattr = new List<String>();
                    foreach (XAttribute attr in tempattr)
                    {
                        if (attr.Name != "address" && attr.Name != "name")
                        {
                            remattr.Add(attr.Name.ToString());
                        }
                    }
                    foreach (String rem in remattr)
                    {
                        temptable.Value.xml.Attribute(rem).Remove();
                    }

                    List<String> eleremlist = new List<String>();
                    
                    foreach (XElement ele in temptable.Value.xml.Elements())
                    {
                        IEnumerable<XAttribute> childtempattr = ele.Attributes();
                        List<String> childremattr = new List<String>();
                        
                        if (ele.Name.ToString() != "table")
                        {
                            eleremlist.Add(ele.Name.ToString());
                            continue;
                        }
                        if (ele.Attribute("type").Value.ContainsCI("static"))
                        {
                            eleremlist.Add(ele.Name.ToString());
                        }
                        else if (ele.Attribute("type").Value.ContainsCI("x axis"))
                        {
                            ele.Attribute("name").Value = "X";
                            ele.SetAttributeValue("address", lut.colsAddress.ToString("X"));
                        }
                        else if (ele.Attribute("type").Value.ContainsCI("y axis"))
                        {
                            ele.Attribute("name").Value = "Y";
                            ele.SetAttributeValue("address", lut.rowsAddress.ToString("X"));
                        }
                        foreach (XAttribute attr in childtempattr)
                        {
                            if (attr.Name != "address" && attr.Name != "name")
                            {
                                childremattr.Add(attr.Name.ToString());
                            }
                        }
                        foreach (String rem in childremattr)
                        {
                            ele.Attribute(rem).Remove();
                        }
                    }
                    foreach (String rem in eleremlist)
                    {
                        temptable.Value.xml.Element(rem).Remove();
                    }
                    return temptable;
                }
            }
            if (this.baseDefinition.RamTableList != null)
            {
                foreach (KeyValuePair<string, Table> table in this.baseDefinition.RamTableList)
                {
                    if (table.Key == name)
                    {
                        KeyValuePair<string, Table> temptable = new KeyValuePair<string,Table>(name, table.Value);
                        //TABLE WAS FOUND!
                        return temptable;
                    }
                }
            }
            return new KeyValuePair<string, Table>();
        }

        #endregion
        #region ECUFlash XML Code
        public static void MapToECUFlash(string filepath,string ident)
        {
            IdaMap idaMap = new IdaMap(filepath);
            //TODO: add ability to select a new base or use multiple inheritance.
            SharpTuner.availableDevices.DefDictionary["32BITBASE"].Populate();
            Definition baseDef = SharpTuner.availableDevices.DefDictionary["32BITBASE"].DeepClone();
            //loop through base def and search for table names in map
            Dictionary<Table, string> foundRomTables = new Dictionary<Table, string>();
            Dictionary<Table, string> foundRamTables = new Dictionary<Table, string>();
            foreach (var romtable in baseDef.RomTableList)
            {
                foreach(var idan in idaMap.IdaCleanNames)
                {
                    if (romtable.Key.EqualsIdaString(idan.Key))
                    {
                        foundRomTables.Add(romtable.Value, idan.Value);
                        break;
                    }
                }
            }
            foreach (var ramtable in baseDef.RamTableList)
            {
                foreach (var idan in idaMap.IdaCleanNames)
                {
                    if (ramtable.Key.EqualsIdaString(idan.Key))
                    {
                        foundRomTables.Add(ramtable.Value, idan.Value);
                        break;
                    }
                }
            }
        }
        #endregion
    }
}


