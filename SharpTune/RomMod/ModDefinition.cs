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
using SharpTune.EcuMapTools;
using SharpTune.Core;
using System.Diagnostics;

namespace SharpTune.RomMod
{
    public class ModDefinition
    {
        private const int tableaddresslimit = 1024;

        private const string filepath = "ModDefTemplate.xml";
        private const uint defMetadataAddress = 0x00F00000;

        private const uint cookieMin = 0x43210000;
        private const uint OpIdent = 0x43210000;
        private const uint OpIdentAddr = 0x43210001;
        private const uint OpTable1d = 0x43210002;
        private const uint OpTable2d = 0x43210003;
        private const uint OpTable3d = 0x43210004;
        private const uint OpX = 0x43210005;
        private const uint OpY = 0x43210006;
        private const uint OpStaticY = 0x43210007;
        private const uint OpRAM = 0x43210008;
        private const uint OpRAMBit = 0x43210010;
        private const uint OpRAMAllBits = 0x43210011;
        private const uint cookieMax = 0x43210012;

        private const uint cookieEnd = 0x00090009;

        public List<Lut> RomLutList {get; private set;}
        public Dictionary<string,Table> RamTableList { get; private set; }

        private Mod parentMod { get; set; }
        private Blob defBlob { get; set; }

        //private string finalEcuId {get; set;}
        //private int EcuIdAddress {get; set; }
        //private string inheritedIdentifier { get; set; }
        //private string inheritedEcuId {get; set; }

        private string outputPath {get; set;}

        public Definition definition { get; private set; }

        public ModDefinition(Mod parent)
        {
            this.parentMod = parent;
            RomLutList = new List<Lut>();
            RamTableList = new Dictionary<string, Table>();
        }

        #region Patch ReadingCode
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool TryReadDefs(String defPath)
        {
            try
            {
                Trace.WriteLine("Attempting to read patch definition metadata");
                List<Blob> blobs = parentMod.blobList.Blobs;
                Blob metadataBlob;
                if (!this.parentMod.TryGetMetaBlob(defMetadataAddress, 10, out metadataBlob, blobs))
                {
                    Trace.WriteLine("This patch file does not contain metadata.");
                    return false;
                }
                this.defBlob = metadataBlob;
                int offs = 0;

                if (!TryParseDefs(this.defBlob, ref offs, defPath)) return false;

                definition = new Definition(defPath, this.parentMod);
            }
            catch (Exception crap)
            {
                Trace.WriteLine("Error parsing definition metadata for {0}", this.parentMod.ModIdent);
                throw crap;
            }
            try
            {
                Trace.WriteLine("Attempting inheriting RR log def template");
                //TODO: move RR stuff into definition?
                //prompt to select logger type
                NewRRLogDefInheritWithTemplate(this.RamTableList, SharpTuner.RRLoggerDefPath + @"\MerpMod\" + parentMod.ModBuild + @"\" + parentMod.ModIdent + ".xml", SharpTuner.RRLoggerDefPath + @"\MerpMod\base.xml", parentMod.InitialEcuId.ToString(), parentMod.FinalEcuId.ToString());
            }
            catch (Exception crap)
            {
                Trace.WriteLine("Error inheriting RR log def template");
                throw crap;
            }
            return true;
        }

        /// <summary>
        /// Try to read the Patch metadata from the file and add this data to table lists.
        /// </summary>
        private bool TryParseDefs(Blob metadata, ref int offset, String defPath)
        {
            UInt32 cookie = 0;
            while ((metadata.Content.Count > offset + 8) &&
                metadata.TryGetUInt32(ref cookie, ref offset))
            {
                if (cookie == OpTable3d)//TODO CONDITIONALLY READ LUTS!
                {
                    string metaString = null;
                    uint metaOffset = 0;
                    if (this.TryReadDefData(metadata, out metaString, out metaOffset, ref offset))
                    {
                        Blob tableBlob;
                        this.parentMod.TryGetMetaBlob(metaOffset, 10, out tableBlob, this.parentMod.blobList.Blobs);
                        Lut3D lut = new Lut3D(metaString,tableBlob, metaOffset);
                        if (metaString != null) RomLutList.Add(lut);
                    }
                    else
                    {
                        Trace.WriteLine("Invalid definition found.");
                        return false;
                    }
                }
                else if (cookie == OpTable2d)//TODO CONDITIONALLY READ LUTS!
                {
                    string metaString = null;
                    uint metaOffset = 0;
                    if (this.TryReadDefData(metadata, out metaString, out metaOffset, ref offset))
                    {
                        Blob tableBlob;
                        this.parentMod.TryGetMetaBlob(metaOffset, 10, out tableBlob, this.parentMod.blobList.Blobs);
                        Lut2D lut = new Lut2D(metaString,tableBlob, metaOffset);
                        if (metaString != null) RomLutList.Add(lut);
                    }
                    else
                    {
                        Trace.WriteLine("Invalid definition found.");
                        return false;
                    }
                }
                else if (cookie == OpTable1d)
                {
                    string metaString = null;
                    uint metaOffset = 0;
                    if (this.TryReadDefData(metadata, out metaString, out metaOffset, ref offset))
                    {
                        Lut lut = new Lut(metaString,metaOffset);
                        if (metaString != null) RomLutList.Add(lut);
                    }
                    else
                    {
                        Trace.WriteLine("Invalid definition found.");
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
                        Trace.WriteLine("Invalid definition found.");
                        return false;
                    }
                }
                else if (cookie == OpRAMAllBits)
                {
                    string paramName = null;
                    string paramId = null;
                    uint paramOffset = 0;
                    uint paramLength = 0;
                    if (this.TryReadDefData(metadata, out paramId, out paramOffset, ref offset))
                    {
                        if (this.TryReadDefData(metadata, out paramName, out paramLength, ref offset))
                        {
                            // found modName, output to string!
                            int len = (int)paramLength;
                            uint address = paramOffset;
                            for(int i=0;i<len;i++)
                            {
                                
                                for(int j=0;j<8;j++){
                                    int bit = ((j)+(8*(i)));
                                    string bitstring = bit.ToString();
                                    KeyValuePair<String, Table> tempTable = CreateRomRaiderRamTableBit(paramName + " Bit " + bitstring , (int)address, paramId, j);
                                    if (tempTable.Key != null) this.RamTableList.Add(tempTable.Key, tempTable.Value);
                               
                                }
                                address++;
                            }
                        }
                    }
                    else
                    {
                        Trace.WriteLine("Invalid definition found.");
                        return false;
                    }
                }
                else if (cookie == OpRAMBit)
                {
                    string paramName = null;
                    string paramId = null;
                    uint paramOffset = 0;
                    uint paramBit = 0;
                    if (this.TryReadDefData(metadata, out paramId, out paramOffset, ref offset))
                    {
                        if (this.TryReadDefData(metadata, out paramName, out paramBit, ref offset))
                        {
                            int bit = bit = Utils.SingleBitBitmaskToBit((int)paramBit);
                            // found modName, output to string!
                            KeyValuePair<String, Table> tempTable = CreateRomRaiderRamTableBit(paramName, (int)paramOffset, paramId, bit);
                            if (tempTable.Key != null) this.RamTableList.Add(tempTable.Key, tempTable.Value);
                        }
                    }
                    else
                    {
                        Trace.WriteLine("Invalid definition found.");
                        return false;
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
                if ((cookie < cookieMax && cookie > cookieMin) || cookie == cookieEnd)
                {
                    if (cookie != cookieEnd)
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

    #endregion

        #region Deprecated
        /// <summary>
        /// TODO: Deprecated
        /// </summary>
        /// <returns></returns>
        private bool TryCleanDef()
        {
            List<string> removelist = new List<string>();
            foreach (KeyValuePair<string, Table> table in this.definition.ExposedRomTables)
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
                this.definition.ExposedRomTables.Remove(table);
            }

            //same operation for ramtables
            removelist.Clear();
            foreach (KeyValuePair<string, Table> table in this.definition.AggregateExposedRamTables)
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
                this.definition.AggregateExposedRamTables.Remove(table);
            }
            return true;
        }

        //TODO: is this needed anywhere?
        ///// <summary>
        ///// Creates a table XEL from the template file, adding proper addresses
        ///// TODO: Deprecated
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="offset"></param>
        ///// <returns></returns>
        //private KeyValuePair<string, Table> CreateTable(string name, int offset)
        //{
        //    //Dictionary<string, Table> list = 

        //    foreach (KeyValuePair<string, Table> table in this.baseDefinition.RomTableList)
        //    {
        //        if (table.Key == name)
        //        {
        //            KeyValuePair<string, Table> temptable = new KeyValuePair<string, Table>(name, table.Value);
        //            temptable.Value.xml.SetAttributeValue("address", offset.ToString("X"));//(System.Int32.Parse(temptable.Value.Attribute("offset").Value.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier) + offset).ToString("X"));
        //            IEnumerable<XAttribute> tempattr = temptable.Value.xml.Attributes();
        //            List<String> remattr = new List<String>();
        //            foreach (XAttribute attr in tempattr)
        //            {
        //                if (attr.Name != "address" && attr.Name != "name")
        //                {
        //                    remattr.Add(attr.Name.ToString());
        //                }
        //            }
        //            foreach (String rem in remattr)
        //            {
        //                temptable.Value.xml.Attribute(rem).Remove();
        //            }

        //            List<String> eleremlist = new List<String>();

        //            foreach (XElement ele in temptable.Value.xml.Elements())
        //            {
        //                IEnumerable<XAttribute> childtempattr = ele.Attributes();
        //                List<String> childremattr = new List<String>();

        //                if (ele.Name.ToString() != "table")
        //                {
        //                    eleremlist.Add(ele.Name.ToString());
        //                    continue;
        //                }
        //                if (ele.Attribute("type").Value.ContainsCI("static"))
        //                {
        //                    eleremlist.Add(ele.Name.ToString());
        //                }
        //                else if (ele.Attribute("type").Value.ContainsCI("x axis"))
        //                {
        //                    ele.Attribute("name").Value = "X";
        //                }
        //                else if (ele.Attribute("type").Value.ContainsCI("y axis"))
        //                {
        //                    ele.Attribute("name").Value = "Y";
        //                }
        //                foreach (XAttribute attr in childtempattr)
        //                {
        //                    if (attr.Name != "address" && attr.Name != "name")
        //                    {
        //                        childremattr.Add(attr.Name.ToString());
        //                    }
        //                }
        //                foreach (String rem in childremattr)
        //                {
        //                    ele.Attribute(rem).Remove();
        //                }
        //            }
        //            foreach (String rem in eleremlist)
        //            {
        //                temptable.Value.xml.Element(rem).Remove();
        //            }
        //            return temptable;
        //        }
        //    }
        //    if (this.baseDefinition.RamTableList != null)
        //    {
        //        foreach (KeyValuePair<string, Table> table in this.baseDefinition.RamTableList)
        //        {
        //            if (table.Key == name)
        //            {
        //                KeyValuePair<string, Table> temptable = new KeyValuePair<string, Table>(name, table.Value);
        //                //TABLE WAS FOUND!
        //                return temptable;
        //            }
        //        }
        //    }
        //    return new KeyValuePair<string, Table>();
        //}

        ///// <summary>
        ///// Creates a table XEL from the template file, adding proper addresses
        ///// TODO: Deprecated
        ///// </summary>
        ///// <param name="name"></param>
        ///// <param name="offset"></param>
        ///// <returns></returns>
        //private KeyValuePair<string, Table> CreateTable(string name, Lut3D lut) //int offset)
        //{
        //    Dictionary<string, Table> list = this.baseDefinition.RomTableList;

        //    foreach (KeyValuePair<string, Table> table in this.baseDefinition.RomTableList)
        //    {
        //        if (table.Key == name)
        //        {
        //            KeyValuePair<string, Table> temptable = new KeyValuePair<string, Table>(name, table.Value);
        //            temptable.Value.xml.SetAttributeValue("address", lut.dataAddress.ToString("X"));
        //            IEnumerable<XAttribute> tempattr = temptable.Value.xml.Attributes();
        //            List<String> remattr = new List<String>();
        //            foreach (XAttribute attr in tempattr)
        //            {
        //                if (attr.Name != "address" && attr.Name != "name")
        //                {
        //                    remattr.Add(attr.Name.ToString());
        //                }
        //            }
        //            foreach (String rem in remattr)
        //            {
        //                temptable.Value.xml.Attribute(rem).Remove();
        //            }

        //            List<String> eleremlist = new List<String>();

        //            foreach (XElement ele in temptable.Value.xml.Elements())
        //            {
        //                IEnumerable<XAttribute> childtempattr = ele.Attributes();
        //                List<String> childremattr = new List<String>();

        //                if (ele.Name.ToString() != "table")
        //                {
        //                    eleremlist.Add(ele.Name.ToString());
        //                    continue;
        //                }
        //                if (ele.Attribute("type").Value.ContainsCI("static"))
        //                {
        //                    eleremlist.Add(ele.Name.ToString());
        //                }
        //                else if (ele.Attribute("type").Value.ContainsCI("x axis"))
        //                {
        //                    ele.Attribute("name").Value = "X";
        //                    ele.SetAttributeValue("address", lut.colsAddress.ToString("X"));
        //                }
        //                else if (ele.Attribute("type").Value.ContainsCI("y axis"))
        //                {
        //                    ele.Attribute("name").Value = "Y";
        //                    ele.SetAttributeValue("address", lut.rowsAddress.ToString("X"));
        //                }
        //                foreach (XAttribute attr in childtempattr)
        //                {
        //                    if (attr.Name != "address" && attr.Name != "name")
        //                    {
        //                        childremattr.Add(attr.Name.ToString());
        //                    }
        //                }
        //                foreach (String rem in childremattr)
        //                {
        //                    ele.Attribute(rem).Remove();
        //                }
        //            }
        //            foreach (String rem in eleremlist)
        //            {
        //                temptable.Value.xml.Element(rem).Remove();
        //            }
        //            return temptable;
        //        }
        //    }
        //    if (this.baseDefinition.RamTableList != null)
        //    {
        //        foreach (KeyValuePair<string, Table> table in this.baseDefinition.RamTableList)
        //        {
        //            if (table.Key == name)
        //            {
        //                KeyValuePair<string, Table> temptable = new KeyValuePair<string, Table>(name, table.Value);
        //                //TABLE WAS FOUND!
        //                return temptable;
        //            }
        //        }
        //    }
        //    return new KeyValuePair<string, Table>();
        //}

        //#endregion
#endregion

        #region RR XML Code
        
        public static void NewRRLogDefInheritWithTemplate(Dictionary<string, Table> ramTableList, string outPath, string template, string inheritIdent, string ident)
        {
            try
            {
                FileInfo outpfi = new FileInfo(outPath);
                Directory.CreateDirectory(outpfi.Directory.FullName);
                XDocument xmlDoc = SelectGetRRLogDef();
                InheritRRLogger(ref xmlDoc, outPath, inheritIdent, ident);
                AddRRLogDefBase(ref xmlDoc, outPath, ramTableList, template);
                PopulateRRLogDefTables(ref xmlDoc, outPath, ramTableList, ident);
                Trace.WriteLine("Attempting to write RR logger definition file to: " + outPath);
                xmlDoc.SaveToFile(outPath);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Error creating RR logger definition for " + ident);
                Trace.WriteLine(e.Message);
            }
        }

        public static void DefineRRLogEcuFromMap(string mapFile, string ident)
        {
            EcuMap im = new EcuMap();
            im.ImportFromMapFileOrText(mapFile);
            DefineRRLogEcu(im.Locs, ident);
        }

        //TODO: Maybe this belongs in IdaMap Class?
        public static void DefineRRLogEcuFromText(string text, string ident)
        {
            Dictionary<string, string> inputMap = new Dictionary<string, string>();
            using (StringReader reader = new StringReader(text))
            {
                string line = string.Empty;
                do
                {
                    string[] l;
                    line = reader.ReadLine();
                    int i = 1;
                    if (line != null)
                    {
                       l = line.Split(' ');
                       string addr;
                        do{
                            addr = l[l.Length - i];
                            i++;
                        }while(addr.Length < 8 && i < l.Length);
                        if (addr.Length > 7)
                            inputMap.Add(l[0], addr.Substring(addr.Length-6));
                        else
                            Trace.WriteLine("error parsing line: " + line + Environment.NewLine + "Try using a map file");
                    }

                } while (line != null);
            }
            DefineRRLogEcu(inputMap, ident);
        }

        private static void DefineRRLogEcu(Dictionary<string, string> inputMap, string ident)
        {
            foreach (var defFile in GetRRLoggerDefs())
            {
                Dictionary<string, string> addMap = new Dictionary<string, string>();
                string defPath = SharpTuner.RRLoggerDefPath + defFile;
                Dictionary<string, string> defMap = ReadRRLogDefExtTables(defPath);
                foreach (KeyValuePair<string, string> def in inputMap)
                {
                    foreach (KeyValuePair<string, string> table in defMap)
                    {
                        if (def.Key.ContainsCI("Ext_"))
                        {
                            string[] a = Regex.Split(def.Key, "Ext_E");
                            string[] b = Regex.Split(table.Key, "E");
                            if (a.Length == 2 && b.Length == 2 && a[1].EqualsCI(b[1]))
                            {
                                try
                                {
                                    addMap.Add(table.Key, def.Value.Replace("FFFF", "FF"));
                                }
                                catch (Exception e)
                                {
                                    Trace.WriteLine(e.Message);
                                }

                                break;
                            }
                        }
                    }
                }
                XDocument xmlDoc = XDocument.Load(defPath);//, LoadOptions.PreserveWhitespace);
                PopulateRRLogDefTables(ref xmlDoc, defPath, addMap, ident);
            }
        }

        private static Dictionary<string, string> ReadRRLogDefExtTables()
        {
            string ld = SelectRRLogDef();
            return ReadRRLogDefExtTables(ld);
        }

        private static Dictionary<string,string> ReadRRLogDefExtTables(string ld)
        {
            Dictionary<string, string> ls = new Dictionary<string, string>();
            XDocument xmlDoc = XDocument.Load(ld);//, LoadOptions.PreserveWhitespace);
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

        private static void AddRRLogDefBase(ref XDocument xmlDoc, string outPath, Dictionary<string,Table> ramTableList, string templatePath)
        {
            XDocument xmlBase = XDocument.Load(templatePath);//, LoadOptions.PreserveWhitespace);
            string bxp = "./logger/protocols/protocol/ecuparams/ecuparam";
            IEnumerable<XElement> xbase = xmlBase.XPathSelectElements(bxp);

            foreach (XElement xb in xbase)
            {
                if (ramTableList.ContainsKeyCI(xb.Attribute("name").Value))
                {
                    xmlDoc.XPathSelectElement("./logger/protocols/protocol/ecuparams").Add(xb);
                }
            }
            xmlDoc.SaveToFile(outPath);
        }

        //TODO: USE table ID instead
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
                    string ch = "//ecuparam[@id='" + table.Key.ToString() + "']/ecu[@id='" + ident + "']";
                    XElement check = exp.XPathSelectElement(ch);
                    if (check != null)
                    {
                        if (check.Value != table.Value.xml.Value)
                            check.Remove();
                        else
                            continue;
                    }
                    cexp.AddBeforeSelf(table.Value.xml);
                }
            }
            xmlDoc.SaveToFile(outPath);
        }

        //TODO add check for existing def!!
        private static void PopulateRRLogDefTables(ref XDocument xmlDoc, string outPath, Dictionary<string, string> ramTableList, string ident)
        {
            foreach (KeyValuePair<string, string> table in ramTableList)
            {
                string xp = "./logger/protocols/protocol/ecuparams/ecuparam[@id='" + table.Key.ToString() + "']";
                XElement exp = xmlDoc.XPathSelectElement(xp);

                string cxp = "./logger/protocols/protocol/ecuparams/ecuparam[@id='" + table.Key.ToString() + "']/conversions";
                XElement cexp = xmlDoc.XPathSelectElement(cxp);

                if (exp != null)
                {
                    string ch = "//ecuparam[@id='" + table.Key.ToString() + "']/ecu[@id='" + ident + "']";
                    XElement check = exp.XPathSelectElement(ch);
                    XElement ad;
                    if (check != null)
                    {
                        ad = new XElement(check);
                        ad.Attribute("id").Value = ident;
                        ad.Descendants().First().Value = "0x" + table.Value;
                        if (check.Value != ad.Value)
                            check.Remove();
                        else 
                            continue;
                    }
                    else
                    {
                        ad = new XElement(exp.Descendants("ecu").First());
                        ad.Attribute("id").Value = ident;
                        ad.Descendants().First().Value = "0x" + table.Value;
                    }                   

                    cexp.AddBeforeSelf(ad);
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
                Trace.WriteLine(e.Message);
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
            string ts = offset.ToString("X6");
            ts = ts.Substring(2, ts.Length-2);
            if (ts.Length < 6 && ts.Substring(0, 2) != "FF")
            {
                Trace.WriteLine("!!!!!!!!!!!!!!!!!!WARNING!!!!!!!!!!!!!!!!!!!!!");
                Trace.WriteLine("WARNING! bad ram table: " + name + " with offset: " + offset.ToString("X"));
            }
            xel.Element("address").Value = "0x" + ts;
            xel.Element("address").Attribute("length").Value = length.ToString();

            return new KeyValuePair<string, Table>(name, TableFactory.CreateTable(xel,name,null));
        }
        #endregion

    }
}


        