using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpTune;
using SharpTune.Tables;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;
using System.Data;

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

        private List<KeyValuePair<string,XElement>> xRomTableList {get; set;}
        private List<KeyValuePair<string,XElement>> xRamTableList { get; set; }

        private Mod parentMod { get; set; }
        private Blob defBlob { get; set; }
        private string identifier {get; set;}
        private int identifierAddress {get; set; }

        private string outputPath {get; set;}

        private string inheritedIdentifier { get; set; }
        private string inheritedEcuId {get; set; }

        private Definition definition { get; set; }
        private Definition inheritedDefinition {get; set;}
        public Define(Mod parent)
        {
            this.parentMod = parent;
        }

        public bool TryReadDefs()
        {
            outputPath = "test.xml";
            xRomTableList = new List<KeyValuePair<string,XElement>>();
            xRamTableList = new List<KeyValuePair<string, XElement>>();
            
            List<Blob> blobs = parentMod.blobList.Blobs;
            Blob metadataBlob;
            if (!this.parentMod.TryGetMetaBlob(defMetadataAddress, 10, out metadataBlob, blobs))
            {
                Console.WriteLine("This patch file does not contain metadata.");
                return false;
            }
            this.defBlob = metadataBlob;
            int offs = 0;

            this.definition = new Definition(filepath);
            this.definition.ReadXML(filepath, false);

            if (!TryParseDefs(this.defBlob, ref offs)) return false;

            if (!TryCleanDef()) return false;

            this.definition.carInfo = this.inheritedDefinition.carInfo;
            this.definition.carInfo["internalidaddress"] = this.identifierAddress.ToString("X");
            this.definition.carInfo["internalidstring"] = this.identifier.ToString();
            this.definition.carInfo["xmlid"] = this.identifier.ToString();
            this.definition.include = this.inheritedIdentifier.ToString();

            XDocument xmlDoc = XDocument.Load(defPath + "merpslogger.xml");

            //xpath by name
            foreach (KeyValuePair<string, XElement> table in this.xRamTableList)
            {
                string xp = "./logger/protocols/protocol/ecuparams/ecuparam[@name='" + table.Key.ToString() + "']";
                XElement exp = xmlDoc.XPathSelectElement(xp);
                    
                
                if (exp != null)
                {
                    string ch = "//ecuparam[@name='" + table.Key.ToString() + "']/ecu[@id='" + this.inheritedEcuId.ToString() + "']";
                    XElement check = exp.XPathSelectElement(ch);
                    if(check != null) check.Remove();
                    exp.AddFirst(table.Value);
                }
                
                xmlDoc.Save(defPath + "merpslogger.xml");
            }

            

            return true;
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
        private bool TryParseDefs(Blob metadata, ref int offset)
        {
            UInt32 cookie = 0;
            while ((metadata.Content.Count > offset + 8) &&
                metadata.TryGetUInt32(ref cookie, ref offset))
            {
                if (cookie == OpIdent)
                {
                    string metaString = null;
                    string metaString1 = null;
                    string metaString2 = null;
                    uint metaOffset = 0;
                    uint metaOffset1 = 0;
                    uint metaOffset2 = 0;
                    if (this.TryReadDefData(metadata, out metaString, out metaOffset, ref offset))
                    {
                        if (this.TryReadDefData(metadata, out metaString1, out metaOffset1, ref offset))
                        {
<<<<<<< HEAD
                            // found modName, output to string!
                            this.identifier = metaString;
                            this.identifierAddress = (int)metaOffset;
                            this.inheritedIdentifier = metaString1;

                            //READ INHERITED DEFINITION
                            inheritedDefinition = new Definition((Definition.DirectorySearch("rommetadata", this.inheritedIdentifier)));
                            inheritedDefinition.ReadXML(inheritedDefinition.defPath, false);
=======
                            if (this.TryReadDefData(metadata, out metaString2, out metaOffset2, ref offset))
                            {
                                // found modName, output to string!
                                this.identifier = metaString;
                                this.identifierAddress = (int)metaOffset;
                                this.inheritedIdentifier = metaString1;
                                this.inheritedEcuId = metaString2;

                                //READ INHERITED DEFINITION
                                inheritedDefinition = new Definition((Definition.DirectorySearch(defPath, this.inheritedIdentifier)));
                                inheritedDefinition.ReadXML(inheritedDefinition.defPath, false, false);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid definition found.");
                            return false;
>>>>>>> 087ab17... Switched RAM parameter creation to old style RomRaider definition format to facilitate earlier testing schedule.
                        }
                    }
                }
                    else if (cookie == OpTable1d || cookie == OpTable2d || cookie == OpTable3d)
                    {
<<<<<<< HEAD
                        // found modName, output to string!
<<<<<<< HEAD
                        this.xRomTableList.Add(CreateTable(metaString, (int)metaOffset));
=======
                       KeyValuePair<String,XElement> tempTable = CreateTable(metaString, (int)metaOffset, false);
                       if(tempTable.Key != null) this.xRomTableList.Add(tempTable.Key, tempTable.Value);
>>>>>>> 9e648e3... Fixed bug in creation of RAM parameters from base definition.
                    }
                    else
                    {
                        Console.WriteLine("Invalid definition found.");
                        return false;
=======
                        string metaString = null;
                        uint metaOffset = 0;
                        if (this.TryReadDefData(metadata, out metaString, out metaOffset, ref offset))
                        {
                            // found modName, output to string!
                            KeyValuePair<String, XElement> tempTable = CreateTable(metaString, (int)metaOffset);
                            if (tempTable.Key != null) this.xRomTableList.Add(tempTable.Key, tempTable.Value);
                        }
                        else
                        {
                            Console.WriteLine("Invalid definition found.");
                            return false;
                        }
>>>>>>> 087ab17... Switched RAM parameter creation to old style RomRaider definition format to facilitate earlier testing schedule.
                    }
                    else if (cookie == OpRAM)
                    {
<<<<<<< HEAD
                        // found modName, output to string!
<<<<<<< HEAD
                        this.xRamTableList.Add(CreateTable(metaString, (int)metaOffset));
=======
                        KeyValuePair<String, XElement> tempTable = CreateTable(metaString, (int)metaOffset, true);
                        if(tempTable.Key != null) this.xRamTableList.Add(tempTable.Key, tempTable.Value);
>>>>>>> 9e648e3... Fixed bug in creation of RAM parameters from base definition.
=======
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
>>>>>>> 087ab17... Switched RAM parameter creation to old style RomRaider definition format to facilitate earlier testing schedule.
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
            
            xel.Attribute("id").Value = this.inheritedEcuId;
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
<<<<<<< HEAD
<<<<<<< HEAD
        private KeyValuePair<string,XElement> CreateTable(string name, int offset)
        {
            foreach (KeyValuePair<string,XElement> table in this.definition.xRomTableList)
=======
        private KeyValuePair<string,XElement> CreateTable(string name, int offset, bool isRam)
=======
        private KeyValuePair<string,XElement> CreateTable(string name, int offset)
>>>>>>> 087ab17... Switched RAM parameter creation to old style RomRaider definition format to facilitate earlier testing schedule.
        { 
            Dictionary<string,XElement> list = this.baseDefinition.xRomTableList;

            foreach (KeyValuePair<string,XElement> table in this.baseDefinition.xRomTableList)
>>>>>>> 9e648e3... Fixed bug in creation of RAM parameters from base definition.
            {
                if (table.Key == name)
                {
                    //TABLE WAS FOUND!
                    if (table.Value.Attribute("address") != null)
                    {
                        table.Value.Attribute("address").SetValue((System.Int32.Parse(table.Value.Attribute("address").Value.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier) + offset).ToString("X"));
                    }
                    foreach(XElement ele in table.Value.Elements())
                    {
                        if (ele.Attribute("address") != null)
                        {
                            ele.Attribute("address").SetValue((System.Int32.Parse(ele.Attribute("address").Value, System.Globalization.NumberStyles.AllowHexSpecifier) + offset).ToString("X"));
                        }
                    }
                    return table;
                }
            }
            if (this.definition.xRamTableList != null)
            {
                foreach (KeyValuePair<string, XElement> table in this.definition.xRamTableList)
                {
                    if (table.Key == name)
                    {
                        //TABLE WAS FOUND!
                        return table;
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
