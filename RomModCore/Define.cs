using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpTune;
using SharpTune.Tables;
using System.Xml.Linq;
using System.Xml;
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
                    uint metaOffset = 0;
                    uint metaOffset1 = 0;
                    if (this.TryReadDefData(metadata, out metaString, out metaOffset, ref offset))
                    {
                        if (this.TryReadDefData(metadata, out metaString1, out metaOffset1, ref offset))
                        {
                            // found modName, output to string!
                            this.identifier = metaString;
                            this.identifierAddress = (int)metaOffset;
                            this.inheritedIdentifier = metaString1;

                            //READ INHERITED DEFINITION
                            inheritedDefinition = new Definition((Definition.DirectorySearch("rommetadata", this.inheritedIdentifier)));
                            inheritedDefinition.ReadXML(inheritedDefinition.defPath, false);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid definition found.");
                        return false;
                    }
                }
                else if (cookie == OpTable1d || cookie == OpTable2d || cookie == OpTable3d)
                {
                    string metaString = null;
                    uint metaOffset = 0;
                    if (this.TryReadDefData(metadata, out metaString, out metaOffset, ref offset))
                    {
                        // found modName, output to string!
                        this.xRomTableList.Add(CreateTable(metaString, (int)metaOffset));
                    }
                    else
                    {
                        Console.WriteLine("Invalid definition found.");
                        return false;
                    }
                }
                else if (cookie == OpRAM)
                {
                    string metaString = null;
                    uint metaOffset = 0;
                    if (this.TryReadDefData(metadata, out metaString, out metaOffset, ref offset))
                    {
                        // found modName, output to string!
                        this.xRamTableList.Add(CreateTable(metaString, (int)metaOffset));
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
        /// Creates a table XEL from the template file, adding proper addresses
        /// </summary>
        /// <param name="name"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private KeyValuePair<string,XElement> CreateTable(string name, int offset)
        {
            foreach (KeyValuePair<string,XElement> table in this.definition.xRomTableList)
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
