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
using SharpTune.RomMod;
using System.IO;

namespace RomModCore
{
    class Define
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

        private Dictionary<string,XElement> xRomTableList {get; set;}
        private Dictionary<string,XElement> xRamTableList { get; set; }

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
        public Define(Mod parent)
        {
            this.parentMod = parent;
        }

        /// <summary>
        /// Cycles through the template definition and replaces "0" addresses with addresses from the patch file.
        /// TODO: Replace template with inheritance from 32BITBASE Tables and a patch parameter that specifies the child template to use.
        /// </summary>
        /// <returns></returns>
        public bool TryReadDefs(String defPath)
        {
            
            xRomTableList = new Dictionary<string,XElement>();
            xRamTableList = new Dictionary<string, XElement>();
            
            List<Blob> blobs = parentMod.blobList.Blobs;
            Blob metadataBlob;
            if (!this.parentMod.TryGetMetaBlob(defMetadataAddress, 10, out metadataBlob, blobs))
            {
                Console.WriteLine("This patch file does not contain metadata.");
                return false;
            }
            this.defBlob = metadataBlob;
            int offs = 0;

            this.baseDefinition = new Definition((Definition.DirectorySearch(defPath, "32BITBASE")));
            this.baseDefinition.ReadXML((Definition.DirectorySearch(defPath, "32BITBASE")), false, true);
            this.inheritedDefinition = new Definition(Definition.DirectorySearch(defPath, this.parentMod.InitialCalibrationId));
            this.inheritedDefinition.ReadXML(Definition.DirectorySearch(defPath, this.parentMod.InitialCalibrationId), false, true);
            this.definition = new Definition("temp.xml");

            if (!TryParseDefs(this.defBlob, ref offs, defPath)) return false;

            if (!TryCleanDef()) return false;

            if (this.parentMod.buildConfig != null)
                outputPath = defPath + "/MerpMod/" + this.parentMod.buildConfig + "/";

            outputPath += this.parentMod.ModIdent.ToString() + ".xml";
            this.definition.defPath = outputPath;
            
            definition.xRomTableList = xRomTableList;
            definition.xRamTableList = xRamTableList;
            this.definition.carInfo = this.inheritedDefinition.carInfo;
            this.definition.carInfo["internalidaddress"] = this.parentMod.ModIdentAddress.ToString("X");
            this.definition.carInfo["internalidstring"] = this.parentMod.ModIdent.ToString();
            this.definition.carInfo["xmlid"] = this.parentMod.ModIdent.ToString();
            this.definition.include = this.parentMod.InitialCalibrationId.ToString();

            //prompt to select logger type
            List<string> loggerdefs = new List<string>();
            List<string> remlist = new List<string>();

            loggerdefs.AddRange(Directory.GetFiles(SharpTuner.RRLoggerDefPath));
            loggerdefs.FilterOnly(".xml");

            for(int i = 0; i < loggerdefs.Count; i++)
            {
                loggerdefs[i] = Path.GetFileName(loggerdefs[i]);              
            }
            loggerdefs.Sort();
            loggerdefs.Reverse();

            string ld = SimpleCombo.ShowDialog("Select logger base", "Select logger base", loggerdefs);

            XDocument xmlDoc = XDocument.Load(SharpTuner.RRLoggerDefPath + ld);
            InheritRRLogger(ref xmlDoc);

            xmlDoc.Save(SharpTuner.RRLoggerDefPath + "/MerpMod/" + parentMod.buildConfig + "/" + parentMod.ModIdent + ".xml");
            

            XDocument xmlBase = XDocument.Load(SharpTuner.RRLoggerDefPath + "/MerpMod/base.xml");
            string bxp = "./logger/protocols/protocol/ecuparams/ecuparam";
            IEnumerable<XElement> xbase = xmlBase.XPathSelectElements(bxp);

            foreach (XElement xb in xbase)
            {
                xmlDoc.XPathSelectElement("./logger/protocols/protocol/ecuparams").Add(xb);
            }

            //xpath by name
            foreach (KeyValuePair<string, XElement> table in this.xRamTableList)
            {
                string xp = "./logger/protocols/protocol/ecuparams/ecuparam[@name='" + table.Key.ToString() + "']";
                XElement exp = xmlDoc.XPathSelectElement(xp);
                    
                
                if (exp != null)
                {
                    string ch = "//ecuparam[@name='" + table.Key.ToString() + "']/ecu[@id='" + this.parentMod.InitialEcuId.ToString() + "']";
                    XElement check = exp.XPathSelectElement(ch);
                    if(check != null) check.Remove();
                    exp.AddFirst(table.Value);
                }

                xmlDoc.Save(SharpTuner.RRLoggerDefPath + "/MerpMod/" + parentMod.buildConfig + "/" + parentMod.ModIdent + ".xml");
            }

            

            

            return true;
        }

        public void InheritRRLogger(ref XDocument xmlDoc)
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
                            if ( id == parentMod.InitialEcuId.ToString())
                            {
                                //found hit, copy the ecu xel
                                XElement newxecu = new XElement(xecu);
                                newxecu.Attribute("id").SetValue(parentMod.FinalEcuId.ToString());
                                xel.AddFirst(newxecu);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Populate tables here, read from an input XML files, and instantiate the tables
        /// Then update tables with proper info
        /// Then create a function to export XML
        /// </summary>
        private void PopulateTableList()
        {
            


            //XDocument xmlDoc = XDocument.Load(filepath);
            //XElement xmlEle = XElement.Load(filepath);
            //XElement romdata = new XElement("rom", 
            //    new XElement("romid",
            //        new XElement("xmlid",this.identifier.ToString()),
            //        new XElement("internalidaddress",this.identifierAddress.ToString()),
            //        new XElement("internalidstring",this.identifier.ToString()),
            //        new XElement("

            //try
            //{
            //    XElement xmlEle = XElement.Load(filepath);

            //    DataSet ds = new DataSet();
            //    ds.ReadXml(xmlEle.CreateReader());
            //    DataSet ds2 = new DataSet();
            //    ds2.ReadXml(xmlreader2);
            //    ds.Merge(ds2);
            //    ds.WriteXml("C:\\Books.xml");
            //    Console.WriteLine("Completed merging XML documents");
            //}
            //catch (System.Exception ex)
            //{
            //    Console.Write(ex.Message);
            //}



            
        }

        private bool TryCleanDef()
        {
            List<string> removelist = new List<string>();
            foreach (KeyValuePair<string, XElement> table in this.definition.xRomTableList)
            {
                if (table.Value.Attribute("address") != null)
                {
                    if (System.Int32.Parse(table.Value.Attribute("address").Value.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier) < tableaddresslimit )
                    {
                        removelist.Add(table.Key.ToString());
                        continue;
                    }
                
                }
            }
            foreach(string table in removelist)
            {
            this.definition.xRomTableList.Remove(table);
            }

            //same operation for ramtables
            removelist.Clear();
            foreach (KeyValuePair<string, XElement> table in this.definition.xRamTableList)
            {
                if (table.Value.Attribute("address") != null)
                {
                    if (System.Int32.Parse(table.Value.Attribute("address").Value.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier) < tableaddresslimit)
                    {
                        removelist.Add(table.Key.ToString());
                        continue;
                    }

                }
            }
            foreach(string table in removelist)
            {
            this.definition.xRamTableList.Remove(table);
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
                        Lut lut = new Lut(tableBlob,metaOffset);
                        KeyValuePair<String, XElement> tempTable = CreateTable(metaString, lut);
                        if (tempTable.Key != null) this.xRomTableList.Add(tempTable.Key, tempTable.Value);
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
                        //Lut2D lut = new Lut(tableBlob, metaOffset);
                        KeyValuePair<String, XElement> tempTable = CreateTable(metaString, (int)metaOffset);//lut);
                        if (tempTable.Key != null) this.xRomTableList.Add(tempTable.Key, tempTable.Value);
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
                            KeyValuePair<String, XElement> tempTable = CreateRomRaiderRamTable(paramName, (int)paramOffset, paramId, (int)paramLenght);
                            if (tempTable.Key != null) this.xRamTableList.Add(tempTable.Key, tempTable.Value);
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

        private Dictionary<string, XElement> RRTemplate = new Dictionary<string, XElement>
        {
            {"Maf Mode Switch",
                XElement.Parse(@"
                <ecuparam id=""E124"" name=""Maf Mode Switch"" desc="""">
                    <ecu id="""">
                        <address length=""""></address>
                    </ecu>
                    <conversions>
                        <conversion units=""estimated AFR"" expr=""14.7/(x*.0004882812)"" format=""0.00"" />
                        <conversion units=""lambda"" expr=""1/(x*.0004882812)"" format=""0.00"" />
                        <conversion units=""fuel-air equivalence ratio"" expr=""x*.0004882812"" format=""0.00"" />
                    </conversions>
                </ecuparam>
                ") },

            {"Volumetric Efficiency Direct", 
                XElement.Parse(@"
                <ecuparam id=""E124"" name=""Volumetric Efficiency Direct"" desc="""">
                    <ecu id="""">
                        <address length=""""></address>
                    </ecu>
                    <conversions>
                        <conversion units=""estimated AFR"" expr=""14.7/(x*.0004882812)"" format=""0.00"" />
                        <conversion units=""lambda"" expr=""1/(x*.0004882812)"" format=""0.00"" />
                        <conversion units=""fuel-air equivalence ratio"" expr=""x*.0004882812"" format=""0.00"" />
                    </conversions>
                </ecuparam>
                ") },

            {"Maf From Speed Density Direct",
            XElement.Parse(@"
                <ecuparam id=""E124"" name=""Maf From Speed Density Direct"" desc="""">
                    <ecu id="""">
                        <address length=""""></address>
                    </ecu>
                    <conversions>
                        <conversion units=""estimated AFR"" expr=""14.7/(x*.0004882812)"" format=""0.00"" />
                        <conversion units=""lambda"" expr=""1/(x*.0004882812)"" format=""0.00"" />
                        <conversion units=""fuel-air equivalence ratio"" expr=""x*.0004882812"" format=""0.00"" />
                    </conversions>
                </ecuparam>
                ") },

            {"Maf From Maf Sensor Direct",
                XElement.Parse(@"
                <ecuparam id=""E124"" name=""Maf Mode Switch"" desc="""">
                    <ecu id="""">
                        <address length=""""></address>
                    </ecu>
                    <conversions>
                        <conversion units=""estimated AFR"" expr=""14.7/(x*.0004882812)"" format=""0.00"" />
                        <conversion units=""lambda"" expr=""1/(x*.0004882812)"" format=""0.00"" />
                        <conversion units=""fuel-air equivalence ratio"" expr=""x*.0004882812"" format=""0.00"" />
                    </conversions>
                </ecuparam>
                ") 
            },

            {"SD Atmospheric Compensation Direct",
                XElement.Parse(@"
                <ecuparam id=""E124"" name=""SD Atmospheric Compensation Direct"" desc="""">
                    <ecu id="""">
                        <address length=""""></address>
                    </ecu>
                    <conversions>
                        <conversion units=""estimated AFR"" expr=""14.7/(x*.0004882812)"" format=""0.00"" />
                        <conversion units=""lambda"" expr=""1/(x*.0004882812)"" format=""0.00"" />
                        <conversion units=""fuel-air equivalence ratio"" expr=""x*.0004882812"" format=""0.00"" />
                    </conversions>
                </ecuparam>
                ") 
            },

            {"SD Blending Ratio Direct",
            XElement.Parse(@"
                <ecuparam id=""E124"" name=""SD Blending Ratio Direct"" desc="""">
                    <ecu id="""">
                        <address length=""""></address>
                    </ecu>
                    <conversions>
                        <conversion units=""estimated AFR"" expr=""14.7/(x*.0004882812)"" format=""0.00"" />
                        <conversion units=""lambda"" expr=""1/(x*.0004882812)"" format=""0.00"" />
                        <conversion units=""fuel-air equivalence ratio"" expr=""x*.0004882812"" format=""0.00"" />
                    </conversions>
                </ecuparam>
                ") 
            },

            {"SD Maf From Blending Direct",
                XElement.Parse(@"
                <ecuparam id=""E124"" name=""SD Maf From Blending Direct"" desc="""">
                    <ecu id="""">
                        <address length=""""></address>
                    </ecu>
                    <conversions>
                        <conversion units=""estimated AFR"" expr=""14.7/(x*.0004882812)"" format=""0.00"" />
                        <conversion units=""lambda"" expr=""1/(x*.0004882812)"" format=""0.00"" />
                        <conversion units=""fuel-air equivalence ratio"" expr=""x*.0004882812"" format=""0.00"" />
                    </conversions>
                </ecuparam>
                ") 
            }
        };

        private KeyValuePair<string,XElement> CreateRomRaiderRamTable(string name, int offset, string id, int length)
        {
            XElement xel = XElement.Parse(@"
                <ecu id="""">
                    <address length=""""></address>
                </ecu>
            ");
            
            xel.Attribute("id").Value = this.parentMod.FinalEcuId;
            xel.Element("address").Value = "0x" + offset.ToString("X6").Substring(2, 6);
            xel.Element("address").Attribute("length").Value = length.ToString();

            return new KeyValuePair<string, XElement>(name, xel);
        }

        /// <summary>
        /// Creates a table XEL from the template file, adding proper addresses
        /// </summary>
        /// <param name="name"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private KeyValuePair<string, XElement> CreateTable(string name, int offset)
        {
            Dictionary<string, XElement> list = this.baseDefinition.xRomTableList;

            foreach (KeyValuePair<string, XElement> table in this.baseDefinition.xRomTableList)
            {
                if (table.Key == name)
                {
                    KeyValuePair<string, XElement> temptable = table;
                    //TABLE WAS FOUND!
                    if (temptable.Value.Attribute("offset") != null)
                    {
                        temptable.Value.SetAttributeValue("address",(System.Int32.Parse(temptable.Value.Attribute("offset").Value.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier) + offset).ToString("X"));
                    }
                    IEnumerable<XAttribute> tempattr = temptable.Value.Attributes();
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
                        temptable.Value.Attribute(rem).Remove();
                    }

                    List<String> eleremlist = new List<String>();

                    foreach (XElement ele in temptable.Value.Elements())
                    {
                        IEnumerable<XAttribute> childtempattr = ele.Attributes();
                        List<String> childremattr = new List<String>();

                        if (ele.Name.ToString() != "table")
                        {
                            eleremlist.Add(ele.Name.ToString());
                            continue;
                        }
                        if (ele.Attribute("offset") != null)
                        {
                            ele.SetAttributeValue("address",(System.Int32.Parse(ele.Attribute("offset").Value, System.Globalization.NumberStyles.AllowHexSpecifier) + offset).ToString("X"));
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
                        temptable.Value.Element(rem).Remove();
                    }
                    return temptable;
                }
            }
            if (this.definition.xRamTableList != null)
            {
                foreach (KeyValuePair<string, XElement> table in this.baseDefinition.xRamTableList)
                {
                    if (table.Key == name)
                    {
                        KeyValuePair<string, XElement> temptable = table;
                        //TABLE WAS FOUND!
                        return temptable;
                    }
                }
            }
            return new KeyValuePair<string, XElement>();
        }

        /// <summary>
        /// Creates a table XEL from the template file, adding proper addresses
        /// </summary>
        /// <param name="name"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private KeyValuePair<string,XElement> CreateTable(string name, Lut lut) //int offset)
        { 
            Dictionary<string,XElement> list = this.baseDefinition.xRomTableList;

            foreach (KeyValuePair<string,XElement> table in this.baseDefinition.xRomTableList)
            {
                if (table.Key == name)
                {
                    KeyValuePair<string, XElement> temptable = table;
                    //TABLE WAS FOUND!
                    if (temptable.Value.Attribute("offset") != null)
                    {
                        //temptable.Value.SetAttributeValue("address",(System.Int32.Parse(temptable.Value.Attribute("offset").Value.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier) + offset).ToString("X"));
                        temptable.Value.SetAttributeValue("address", lut.dataAddress.ToString("X"));
                    }
                    IEnumerable<XAttribute> tempattr = temptable.Value.Attributes();
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
                        temptable.Value.Attribute(rem).Remove();
                    }

                    List<String> eleremlist = new List<String>();
                    
                    foreach (XElement ele in temptable.Value.Elements())
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
                        temptable.Value.Element(rem).Remove();
                    }
                    return temptable;
                }
            }
            if (this.definition.xRamTableList != null)
            {
                foreach (KeyValuePair<string, XElement> table in this.baseDefinition.xRamTableList)
                {
                    if (table.Key == name)
                    {
                        KeyValuePair<string, XElement> temptable = table;
                        //TABLE WAS FOUND!
                        return temptable;
                    }
                }
            }
            return new KeyValuePair<string, XElement>();
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
        public bool TryPrintDef(string calid)
        {
            this.definition.ExportXML(outputPath);
            return true;
        }

    }
}
