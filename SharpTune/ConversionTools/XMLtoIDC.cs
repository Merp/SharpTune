/*
 * Copyright (C) 2012 NSFW@romraider.forum and Dale C. Schultz
 * RomRaider member ID: NSFW and dschultz
 *
 * You are free to use this script for any purpose, but please keep
 * notice of where it came from!
 */

//TODO: set up TRACE listeners and write to trace instead
// add input/output path to args

using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

namespace NSFW
{
    public class XMLtoIDC
    {
        private static HashSet<string> names = new HashSet<string>();
        private static IDictionary<string, string> tableList = new Dictionary<string, string>();
        private static String DefPath = "";

        public static void GuiRun(string[] args, string outpath, string def, string loggerdef, string loggerdtd)
        {

            Trace.Listeners.Clear();

            using(TextWriterTraceListener twtl = new TextWriterTraceListener(outpath))
            {
                twtl.Name = "TextLogger";
                twtl.TraceOutputOptions = TraceOptions.ThreadId | TraceOptions.DateTime;
                Trace.Listeners.Add(twtl);

                //No need to add console listener here
                //ConsoleTraceListener ctl = new ConsoleTraceListener(false);
                //ctl.TraceOutputOptions = TraceOptions.DateTime;
                //Trace.Listeners.Add(ctl);

                Trace.AutoFlush = true;

                DefPath = SharpTune.SharpTuner.RRDefRepoPath; //Todo: switch to RR specific path, and automate a search for RR defs whenever the path is changed/loaded.

                StringBuilder builder = new StringBuilder();
                builder.AppendLine("///////////////////////////////////////////////////////////////////////////////");
                builder.AppendLine(string.Format("// This file gernerated by XmlToIdc version: {0}",
                                  Assembly.GetExecutingAssembly().GetName().Version));
                builder.AppendLine(string.Format("// running on mscorlib.dll version: {0}",
                                  typeof(String).Assembly.GetName().Version));
                Trace.Write(builder.ToString());

                if (args.Length == 0)
                {
                    Usage();
                    return;
                }

                if (CategoryIs(args, "tables"))
                {
                    if (args.Length != 2)
                    {
                        UsageTables();
                    }
                    else
                    {
                        string calId = args[1].ToUpper();
                        string functionName = "Tables_" + calId;
                        WriteHeader1(functionName, string.Format("Table definitions for {0}", calId));
                        DefineTables(functionName, calId, def);
                    }
                }
                else if (CategoryIs(args, "stdparam"))
                {
                    if (args.Length != 5)
                    {
                        UsageStdParam();
                    }
                    else
                    {
                        string cpu = args[1];
                        string target = args[2].ToUpper();
                        string calId = args[3].ToUpper();
                        string ssmBaseString = args[4].ToUpper();
                        string functionName = "StdParams_" + calId;
                        uint ssmBase = ConvertBaseString(ssmBaseString);
                        WriteHeader1(functionName,
                                     string.Format("Standard parameter definitions for {0} bit {1}: {2} with SSM read vector base {3}",
                                      cpu, target, calId, ssmBase.ToString("X")));
                        DefineStandardParameters(functionName, target, calId, ssmBase, cpu, loggerdef, loggerdtd);
                    }
                }
                else if (CategoryIs(args, "extparam"))
                {
                    if (args.Length != 4)
                    {
                        UsageExtParam();
                        return;
                    }
                    else
                    {
                        string cpu = args[1];
                        string target = args[2].ToUpper();
                        string ecuId = args[3].ToUpper();
                        string functionName = "ExtParams_" + ecuId;
                        WriteHeader1(functionName,
                                     string.Format("Extended parameter definitions for {0} bit {1}: {2}",
                                      cpu, target, ecuId));
                        DefineExtendedParameters(functionName, target, ecuId, cpu, loggerdef, loggerdtd);
                    }
                }
                else if (CategoryIs(args, "makeall"))
                {
                    if (args.Length != 4)
                    {
                        UsageMakeAll();
                        return;
                    }
                    else
                    {
                        string target = args[1].ToUpper();
                        string calId = args[2].ToUpper();
                        string ssmBaseString = args[3].ToUpper();
                        string functionName1 = "Tables";
                        string functionName2 = "StdParams";
                        string functionName3 = "ExtParams";
                        WriteHeader3(functionName1, functionName2, functionName3,
                                     string.Format("All definitions for {0}: {1} with SSM read vector base {2}",
                                      target, calId, ssmBaseString));
                        string[] results = new string[2];
                        results = DefineTables(functionName1, calId, def);
                        uint ssmBase = ConvertBaseString(ssmBaseString);
                        DefineStandardParameters(functionName2, target, calId, ssmBase, results[1], loggerdef, loggerdtd);
                        DefineExtendedParameters(functionName3, target, results[0], results[1], loggerdef, loggerdtd);
                    }
                }
            }
        }

        //static void Run(string[] args)
        //{
        //    StringBuilder builder = new StringBuilder();
        //    builder.AppendLine("///////////////////////////////////////////////////////////////////////////////");
        //    builder.AppendLine(string.Format("// This file gernerated by XmlToIdc version: {0}",
        //                      Assembly.GetExecutingAssembly().GetName().Version));
        //    builder.AppendLine(string.Format("// running on mscorlib.dll version: {0}",
        //                      typeof(String).Assembly.GetName().Version));
        //    Trace.Write(builder.ToString());

        //    if (args.Length == 0)
        //    {
        //        Usage();
        //        return;
        //    }

        //    if (CategoryIs(args, "tables"))
        //    {
        //        if (args.Length != 2)
        //        {
        //            UsageTables();
        //        }
        //        else
        //        {
        //            string calId = args[1].ToUpper();
        //            string functionName = "Tables_" + calId;
        //            WriteHeader1(functionName, string.Format("Table definitions for {0}", calId));
        //            DefineTables(functionName, calId);
        //        }
        //    }
        //    else if (CategoryIs(args, "stdparam"))
        //    {
        //        if (args.Length != 5)
        //        {
        //            UsageStdParam();
        //        }
        //        else
        //        {
        //            string cpu = args[1];
        //            string target = args[2].ToUpper();
        //            string calId = args[3].ToUpper();
        //            string ssmBaseString = args[4].ToUpper();
        //            string functionName = "StdParams_" + calId;
        //            uint ssmBase = ConvertBaseString(ssmBaseString);
        //            WriteHeader1(functionName,
        //                         string.Format("Standard parameter definitions for {0} bit {1}: {2} with SSM read vector base {3}",
        //                          cpu, target, calId, ssmBase.ToString("X")));
        //            DefineStandardParameters(functionName, target, calId, ssmBase, cpu);
        //        }
        //    }
        //    else if (CategoryIs(args, "extparam"))
        //    {
        //        if (args.Length != 4)
        //        {
        //            UsageExtParam();
        //            return;
        //        }
        //        else
        //        {
        //            string cpu = args[1];
        //            string target = args[2].ToUpper();
        //            string ecuId = args[3].ToUpper();
        //            string functionName = "ExtParams_" + ecuId;
        //            WriteHeader1(functionName,
        //                         string.Format("Extended parameter definitions for {0} bit {1}: {2}",
        //                          cpu, target, ecuId));
        //            DefineExtendedParameters(functionName, target, ecuId, cpu);
        //        }
        //    }
        //    else if (CategoryIs(args, "makeall"))
        //    {
        //        if (args.Length != 4)
        //        {
        //            UsageMakeAll();
        //            return;
        //        }
        //        else
        //        {
        //            string target = args[1].ToUpper();
        //            string calId = args[2].ToUpper();
        //            string ssmBaseString = args[3].ToUpper();
        //            string functionName1 = "Tables";
        //            string functionName2 = "StdParams";
        //            string functionName3 = "ExtParams";
        //            WriteHeader3(functionName1, functionName2, functionName3,
        //                         string.Format("All definitions for {0}: {1} with SSM read vector base {2}",
        //                          target, calId, ssmBaseString));
        //            string[] results = new string[2];
        //            results = DefineTables(functionName1, calId);
        //            uint ssmBase = ConvertBaseString(ssmBaseString);
        //            DefineStandardParameters(functionName2, target, calId, ssmBase, results[1]);
        //            DefineExtendedParameters(functionName3, target, results[0], results[1]);
        //        }
        //    }
        //}

        #region DefineXxxx functions

        private static string[] DefineTables(string functionName, string calId, string def)
        {
            if (!File.Exists(def))
            {
                MessageBox.Show("ecu_defs.xml must be in the current directory.",
                                "Error - ECU Definitions File Missing",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1);
                return null;
            }
            string[] results = new string[2];
            WriteHeader2(functionName);
            results = WriteTableNames(calId,def);
            WriteFooter(functionName);
            return results;
        }

        private static void DefineStandardParameters(string functionName, string target, string calId, uint ssmBase, string cpu, string loggerdef, string loggerdtd)
        {
            if (!File.Exists(loggerdef))
            {
                MessageBox.Show("logger.xml must be in the current directory.",
                                "Error - Logger Definitions File Missing",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1);
                return;
            }

            if (!File.Exists(loggerdtd))
            {
                MessageBox.Show("logger.dtd must be in the current directory.",
                                "Error - Logger Type Definition File Missing",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1);
                return;
            }

            File.Delete("logger.dtd");
            File.Copy(loggerdtd, "logger.dtd");

            WriteHeader2(functionName);
            WriteStandardParameters(target, calId, ssmBase, cpu, loggerdef);
            WriteFooter(functionName);
        }

        private static void DefineExtendedParameters(string functionName, string target, string ecuId, string cpu, string loggerdef, string loggerdtd)
        {
            if (!File.Exists(loggerdef))
            {
                MessageBox.Show("logger.xml must be in the current directory.",
                                "Error - Logger Definitions File Missing",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1);
                return;
            }

            if (!File.Exists(loggerdtd))
            {
                MessageBox.Show("logger.dtd must be in the current directory.",
                                "Error - Logger Type Definition File Missing",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation,
                                MessageBoxDefaultButton.Button1);
                return;
            }

            WriteHeader2(functionName);
            WriteExtendedParameters(target, ecuId, cpu, loggerdef);
            WriteFooter(functionName);
        }

        #endregion

        private static string[] WriteTableNames(string xmlId, string def)
        {
            Trace.WriteLine("auto referenceAddress;");

            string ecuid = null;
            string memmodel = null;
            int tableCount = 0;
            string datatype = null;
            bool ecu16bit = false;
            int dtaddr = 0;
            string cpu = "32";

            string rombase = GetRomBase(xmlId, def);
            string[] roms = new string[2] { rombase, xmlId };

            using (Stream stream = File.OpenRead(def))
            {
                XPathDocument doc = new XPathDocument(stream);
                XPathNavigator nav = doc.CreateNavigator();
                foreach (string id in roms)
                {
                    tableCount = 0;
                    names.Clear();
                    if (id.Contains("BASE"))
                    {
                        continue;
                    }
                    else if (id.Equals(rombase))
                    {
                        Trace.WriteLine("Warning(\"Marking tables using addresses from inherited base ROM: " +
                                          id.ToUpper() + "\");");
                    }
                    string path = "/roms/rom/romid[xmlid='" + id + "']";
                    XPathNodeIterator iter = nav.Select(path);
                    iter.MoveNext();
                    nav = iter.Current;
                    nav.MoveToChild(XPathNodeType.Element);

                    while (nav.MoveToNext())
                    {
                        if (nav.Name == "ecuid")
                        {
                            ecuid = nav.InnerXml;
                        }
                        if (nav.Name == "memmodel")
                        {
                            memmodel = nav.InnerXml;
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(ecuid))
                    {
                        Trace.WriteLine("Could not find definition for " + id);
                        return null;
                    }
                    if (memmodel.Contains("68HC"))
                    {
                        ecu16bit = true;
                        cpu = "16";
                    }

                    nav.MoveToParent();
                    while (nav.MoveToNext())
                    {
                        if (nav.Name == "table")
                        {
                            string name = nav.GetAttribute("name", "");
                            string storageAddress = nav.GetAttribute("storageaddress", "");

                            name = ConvertName(name);
                            UpdateTableList(name, storageAddress);
                            if (ecu16bit)
                            {
                                datatype = ConvertName("Table_" + name);
                                dtaddr = Convert.ToInt32(storageAddress, 16);
                                dtaddr = dtaddr - 1;
                            }

                            List<string> axes = new List<string>();
                            if (nav.HasChildren)
                            {
                                nav.MoveToChild(XPathNodeType.Element);

                                do
                                {
                                    string axis = nav.GetAttribute("type", "");
                                    axes.Add(axis);
                                    string axisAddress = nav.GetAttribute("storageaddress", "");

                                    axis = ConvertName(name + "_" + axis);
                                    UpdateTableList(axis, axisAddress);
                                } while (nav.MoveToNext());

                                if (axes.Count == 2 &&
                                    (axes[0].ToUpper() == "X AXIS" && axes[1].ToUpper() == "Y AXIS") &&
                                    !ecu16bit)
                                {
                                    string tableName = ConvertName("Table_" + name);
                                    UpdateTableList(tableName, "2axis");
                                }
                                else if (axes.Count == 1 &&
                                         axes[0].ToUpper() == "Y AXIS" &&
                                         !ecu16bit)
                                {
                                    string tableName = ConvertName("Table_" + name);
                                    UpdateTableList(tableName, "1axis");
                                }
                                else if (axes.Count > 0 && ecu16bit && axes[0].ToUpper().Contains("AXIS"))
                                {
                                    string dataTypeAddr = "0x" + dtaddr.ToString("X");
                                    UpdateTableList(datatype, dataTypeAddr);
                                }
                                nav.MoveToParent();
                            }
                            tableCount++;
                        }
                    }
                    if (tableCount < 1)
                    {
                        Trace.WriteLine("// No tables found specifically for ROM " + id + ", used inherited ROM");
                    }
                }
                WriteIdcTableNames();
            }
            string[] results = new string[2] { ecuid, cpu };
            return results;
        }

        private static string GetRomBase(string xmlId, string def)
        {
            string rombase = "";
            using (Stream stream = File.OpenRead(def))
            {
                XPathDocument doc = new XPathDocument(stream);
                XPathNavigator nav = doc.CreateNavigator();
                string path = "/roms/rom/romid[xmlid='" + xmlId + "']";
                XPathNodeIterator iter = nav.Select(path);
                iter.MoveNext();
                nav = iter.Current;
                nav.MoveToChild(XPathNodeType.Element);
                do
                {
                    nav.MoveToParent();
                } while (nav.Name != "rom");

                if (nav.Name == "rom")
                {
                    rombase = nav.GetAttribute("base", "");
                }
            }
            return rombase;
        }

        private static void WriteIdcTableNames()
        {
            foreach (var pair in tableList)
            {
                string tableName = pair.Key;
                string tableAddress = pair.Value;
                if (System.Text.RegularExpressions.Regex.IsMatch(tableAddress, @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z") && !tableAddress.Contains("0x"))
                    tableAddress = "0x" + tableAddress;
                string refTableName = "Table_" + tableName;
                string refTableAddress = "";
                if (tableList.TryGetValue(refTableName, out refTableAddress))
                {
                    if (!tableName.StartsWith("Table_") &&
                        (refTableAddress.Equals("1axis") || refTableAddress.Equals("2axis")))
                    {
                        string offset = "";
                        if (refTableAddress.Equals("1axis"))
                        {
                            offset = "8";
                        }
                        if (refTableAddress.Equals("2axis"))
                        {
                            offset = "12";
                        }
                        MakeName(tableAddress, tableName);
                        StringBuilder builder = new StringBuilder();
                        builder.AppendLine("referenceAddress = DfirstB(" + tableAddress + ");");
                        builder.AppendLine("if (referenceAddress > 0)");
                        builder.AppendLine("{");
                        builder.AppendLine("    referenceAddress = referenceAddress - " + offset + ";");
                        string command = string.Format("    MakeNameEx(referenceAddress, \"{0}\", SN_CHECK);", refTableName);
                        builder.AppendLine(command);
                        builder.AppendLine("}");
                        builder.AppendLine("else");
                        builder.AppendLine("{");
                        builder.AppendLine("    Message(\"No reference to " + tableName + "\\n\");");
                        builder.AppendLine("}");
                        Trace.WriteLine(builder.ToString());
                    }
                    else
                    {
                        MakeName(tableAddress, tableName);
                    }
                }
                else
                {
                    if (!tableName.StartsWith("Table_") ||
                        tableName.StartsWith("Table_") && !tableAddress.Contains("axis"))
                    {
                        MakeName(tableAddress, tableName);
                    }
                }
            }
        }

        private static void WriteStandardParameters(string target, string ecuid, uint ssmBase, string cpu, string loggerdef)
        {
            Trace.WriteLine("auto addr;");

            if (target == "ecu" | target == "ECU")
            {
                target = "2";
            }
            if (target == "tcu" | target == "TCU")
            {
                target = "1";
            }

            string ptrName = "PtrSsmGet_";
            string funcName = "SsmGet_";

            if (cpu.Equals("16"))
            {
                ptrName = "PtrSsm_";
                funcName = "Ssm_";
                Trace.WriteLine("auto opAddr, seg;");
                Trace.WriteLine("if (SegByName(\"DATA\") != BADADDR) seg = 1;");
                Trace.WriteLine("");
            }

            using (Stream stream = File.OpenRead(loggerdef))
            {
                XPathDocument doc = new XPathDocument(stream);
                XPathNavigator nav = doc.CreateNavigator();
                string path = "/logger/protocols/protocol[@id='SSM']/parameters/parameter";
                XPathNodeIterator iter = nav.Select(path);
                string id = "";
                while (iter.MoveNext())
                {
                    XPathNavigator navigator = iter.Current;
                    if (navigator.GetAttribute("target", "") == target)
                    {
                        continue;
                    }
                    string name = navigator.GetAttribute("name", "");
                    id = navigator.GetAttribute("id", "");
                    if (cpu.Equals("16") && id.Equals("P89"))
                    {
                        continue;
                    }
                    name = name + "_" + id.Trim();
                    string pointerName = ConvertName(ptrName + name);
                    string functionName = ConvertName(funcName + name);

                    if (!navigator.MoveToChild("address", ""))
                    {
                        continue;
                    }

                    string addressString = iter.Current.InnerXml;
                    addressString = addressString.Substring(2);

                    uint address = uint.Parse(addressString, System.Globalization.NumberStyles.HexNumber);
                    if (cpu.Equals("16") && address > 0x18F)    // addresses over this will overrun DTC structure
                    {
                        continue;
                    }
                    address = address * 4;
                    address = address + ssmBase;
                    addressString = "0x" + address.ToString("X8");

                    Trace.WriteLine(string.Format("MakeUnknown({0}, 4, DOUNK_SIMPLE);", addressString));
                    Trace.WriteLine(string.Format("MakeDword({0});", addressString));
                    MakeName(addressString, pointerName);

                    string getAddress = string.Format("addr = Dword({0});", addressString);
                    Trace.WriteLine(getAddress);
                    MakeName("addr", functionName);

                    if (cpu.Equals("16"))
                    {
                        string length = "1";
                        length = navigator.GetAttribute("length", "");
                        FormatData("addr", length);
                        string fGetter = ConvertName("SsmGet_" + name);
                        StringBuilder builder = new StringBuilder();
                        builder.AppendLine("opAddr = FindImmediate(0, 0x21, (addr - 0x20000));");
                        builder.AppendLine("addr = GetFunctionAttr(opAddr, FUNCATTR_START);");
                        builder.AppendLine("if (addr != BADADDR)");
                        builder.AppendLine("{");
                        builder.AppendLine("    if (seg)");
                        builder.AppendLine("    {");
                        builder.AppendLine("        OpAlt(opAddr, 0, \"\");");
                        builder.AppendLine("        OpOff(MK_FP(\"ROM\", opAddr), 0, 0x20000);");
                        builder.AppendLine("    }");
                        builder.AppendLine("    add_dref(opAddr, Dword(" + addressString + "), dr_I);");
                        string command = string.Format("    MakeNameEx(addr, \"{0}\", SN_CHECK);", fGetter);
                        builder.AppendLine(command);
                        builder.AppendLine("}");
                        builder.AppendLine("else");
                        builder.AppendLine("{");
                        builder.AppendLine("    Message(\"No reference to " + name + "\\n\");");
                        builder.AppendLine("}");
                        Trace.Write(builder.ToString());
                    }
                    Trace.WriteLine("");
                }
                // now let's print the switch references
                // Name format: Switches_b7_b6_b5_b4_b3_b2_b1_b0
                IDictionary<string, Array> switchList = new Dictionary<string, Array>();
                path = "/logger/protocols/protocol[@id='SSM']/switches/switch";
                iter = nav.Select(path);
                string bit = "";
                while (iter.MoveNext())
                {
                    XPathNavigator navigator = iter.Current;
                    if (navigator.GetAttribute("target", "") == target)
                    {
                        continue;
                    }
                    id = navigator.GetAttribute("id", "");
                    id = id.Replace("S", "");
                    string addr = navigator.GetAttribute("byte", "");
                    addr = addr.Substring(2);
                    bit = navigator.GetAttribute("bit", "");
                    Array values;
                    if (!switchList.TryGetValue(addr, out values))
                    {
                        string[] temp = new string[8] { "x", "x", "x", "x", "x", "x", "x", "x" };
                        switchList.Add(addr, temp);
                    }
                    if (switchList.TryGetValue(addr, out values))
                    {
                        uint i = uint.Parse(bit, System.Globalization.NumberStyles.HexNumber);
                        values.SetValue(id.Trim(), i);
                        Array.Copy(values, switchList[addr], values.Length);
                    }
                }
                PrintSwitches(switchList, ssmBase, cpu);
            }
        }

        private static void WriteExtendedParameters(string target, string ecuid, string cpu, string loggerdef)
        {
            if (target == "ecu" | target == "ECU")
            {
                target = "2";
            }
            if (target == "tcu" | target == "TCU")
            {
                target = "1";
            }

            using (Stream stream = File.OpenRead(loggerdef))
            {
                XPathDocument doc = new XPathDocument(stream);
                XPathNavigator nav = doc.CreateNavigator();
                string path = "/logger/protocols/protocol[@id='SSM']/ecuparams/ecuparam/ecu[@id='" + ecuid + "']/address";
                XPathNodeIterator iter = nav.Select(path);
                while (iter.MoveNext())
                {
                    string addressString = iter.Current.InnerXml;
                    addressString = addressString.Substring(2);
                    uint address = uint.Parse(addressString, System.Globalization.NumberStyles.HexNumber);
                    if (cpu.Contains("32"))
                    {
                        address |= 0xFF000000;
                    }
                    addressString = "0x" + address.ToString("X8");

                    XPathNavigator n = iter.Current;
                    string length = n.GetAttribute("length", "");
                    n.MoveToParent();
                    n.MoveToParent();
                    if (n.GetAttribute("target", "") == target)
                    {
                        continue;
                    }
                    string name = n.GetAttribute("name", "");
                    string id = n.GetAttribute("id", "");
                    name = "E_" + ConvertName(name) + "_" + id.Trim();

                    MakeName(addressString, name);
                    FormatData(addressString, length);
                }
            }
        }

        #region Utility functions

        private static void WriteHeader1(string functionName, string description)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("///////////////////////////////////////////////////////////////////////////////");
            builder.AppendLine("// " + description);
            builder.AppendLine("///////////////////////////////////////////////////////////////////////////////");
            builder.AppendLine("#include <idc.idc>");
            builder.AppendLine("static main ()");
            builder.AppendLine("{");
            builder.AppendLine("    " + functionName + " ();");
            builder.AppendLine("}");
            Trace.WriteLine(builder.ToString());
        }

        private static void WriteHeader2(string functionName)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("static " + functionName + " ()");
            builder.AppendLine("{");
            builder.AppendLine("Message(\"--- Now marking " + functionName + " ---\\n\");");
            Trace.Write(builder.ToString());
        }

        private static void WriteHeader3(string functionName1, string functionName2, string functionName3, string description)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("///////////////////////////////////////////////////////////////////////////////");
            builder.AppendLine("// " + description);
            builder.AppendLine("///////////////////////////////////////////////////////////////////////////////");
            builder.AppendLine("#include <idc.idc>");
            builder.AppendLine("static main ()");
            builder.AppendLine("{");
            builder.AppendLine("    " + functionName1 + " ();");
            builder.AppendLine("    " + functionName2 + " ();");
            builder.AppendLine("    " + functionName3 + " ();");
            builder.AppendLine("}");
            Trace.WriteLine(builder.ToString());
        }

        private static void WriteFooter(string functionName)
        {
            Trace.WriteLine("}   // end of " + functionName);
            Trace.WriteLine("");
        }

        private static void PrintSwitches(IDictionary<string, Array> switchList, uint ssmBase, string cpu)
        {
            Trace.WriteLine("// Switch Bit Position Name format: Switches_b7_b6_b5_b4_b3_b2_b1_b0");

            string ptrName = "PtrSsmGet_";
            string funcName = "SsmGet_";

            if (cpu.Equals("16"))
            {
                ptrName = "PtrSsm_";
                funcName = "Ssm_";
            }

            foreach (var pair in switchList)
            {
                string[] switches = new string[8];
                Array.Copy(pair.Value, switches, 8);
                string bitString = "";
                for (int j = switches.Length; j != 0; j--)
                {
                    bitString = string.Format("{0}_{1}", bitString, switches[j - 1]);
                }

                string name = "Switches" + bitString;
                string pointerName = ConvertName(ptrName + name);
                string functionName = ConvertName(funcName + name);
                uint address = uint.Parse(pair.Key, System.Globalization.NumberStyles.HexNumber);
                if (cpu.Equals("16") && address > 0x18F)    // addresses over this will overrun DTC structure
                {
                    continue;
                }
                address = address * 4;
                address = address + ssmBase;
                string addressString = "0x" + address.ToString("X8");

                Trace.WriteLine(string.Format("MakeUnknown({0}, 4, DOUNK_SIMPLE);", addressString));
                Trace.WriteLine(string.Format("MakeDword({0});", addressString));
                MakeName(addressString, pointerName);

                string getAddress = string.Format("addr = Dword({0});", addressString);
                Trace.WriteLine(getAddress);
                if (cpu.Equals("16"))
                {
                    FormatData("addr", "1");
                }
                MakeName("addr", functionName);
                Trace.WriteLine("");
            }
        }

        private static void MakeName(string address, string name)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(address, @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z") && !address.Contains("0x"))
                address = "0x" + address;
            if (address.Length > 0 && name.Length > 0)
            {
                string command = string.Format("MakeNameEx({0}, \"{1}\", SN_CHECK);",
                                               address,
                                               name);
                Trace.WriteLine(command);
            }
        }

        private static void UpdateTableList(string name, string address)
        {
            if (address.Length > 0 && name.Length > 0)
            {
                string tmpAddr;
                if (tableList.TryGetValue(name, out tmpAddr))
                {
                    tableList[name] = address;
                }
                else
                {
                    tableList.Add(name, address);
                }
            }
        }

        private static string ConvertName(string original)
        {
            original = original.Replace(")(", "_");

            StringBuilder builder = new StringBuilder(original.Length);
            foreach (char c in original)
            {
                if (char.IsLetterOrDigit(c))
                {
                    builder.Append(c);
                    continue;
                }

                if (c == '_')
                {
                    builder.Append(c);
                    continue;
                }

                if (char.IsWhiteSpace(c))
                {
                    builder.Append('_');
                    continue;
                }

                if (c == '*')
                {
                    builder.Append("Ext");
                    continue;
                }
            }

            // Make sure it's unique
            string name = builder.ToString();
            while (names.Contains(name))
            {
                name = name + "_";
            }
            names.Add(name);

            return name;
        }

        private static bool CategoryIs(string[] args, string category)
        {
            return string.Compare(args[0], category, StringComparison.OrdinalIgnoreCase) == 0;
        }

        private static void FormatData(string address, string length)
        {
            string datatype = "";
            if (length == "" || length == "1")
            {
                datatype = "MakeByte";
                length = "1";
            }
            else if (length == "2")
            {
                datatype = "MakeWord";
            }
            else if (length == "4")
            {
                datatype = "MakeFloat";
            }
            if (datatype != "")
            {
                string unknown = string.Format("MakeUnknown({0}, {1}, DOUNK_SIMPLE);",
                                               address, length);
                Trace.WriteLine(unknown);
                string command = string.Format("{0}({1});",
                                               datatype,
                                               address);
                Trace.WriteLine(command);
            }
        }

        private static uint ConvertBaseString(string ssmBaseString)
        {

            ssmBaseString = ssmBaseString.ToUpper();
            uint ssmBase = uint.Parse(ssmBaseString, System.Globalization.NumberStyles.HexNumber);

            if (ssmBase < (uint)0x20000)
            {
                uint newBase = ssmBase + (uint)0x20000;
                MessageBox.Show("SSM base adjusted from 0x" + ssmBase.ToString("X") + " to 0x" + newBase.ToString("X"),
                                "Info - SSM Base Address Changed",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information,
                                MessageBoxDefaultButton.Button1);
                ssmBase = newBase;
            }
            return ssmBase;
        }

        #endregion

        #region Usage instructions

        private static void Usage()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Usage:");
            builder.AppendLine("XmlToIdc.exe <category> <options>...");
            builder.AppendLine();
            builder.AppendLine("Where <category> is one of the following:");
            builder.AppendLine("    tables   <cal-id>");
            builder.AppendLine("    stdparam <cpu> <target> <cal-id> <ssm-base>");
            builder.AppendLine("    extparam <cpu> <target> <ecu-id>");
            builder.AppendLine("    makeall  <target> <cal-id> <ssm-base>");
            builder.AppendLine();
            builder.AppendLine("Where <options> is the following as required by the category:");
            builder.AppendLine("    <cal-id>   is the Calibration id, e.g. A2WC522N");
            builder.AppendLine("    <cpu>      is the CPU bits identifier of the ECU, e.g. 16 or 32");
            builder.AppendLine("    <target>   is the Car control module,");
            builder.AppendLine("                 e.g. ecu (engine control unit) or tcu (transmission control unit)");
            builder.AppendLine("    <ecu-id>   is the ECU identifier, e.g. 2F12785606");
            builder.AppendLine("    <ssm-base> is the Base address of the SSM 'read' vector, e.g. 4EDDC");
            builder.AppendLine();
            builder.AppendLine("And you'll want to redirect stdout to a file, like:");
            builder.AppendLine("XmlToIdc.exe ... > Whatever.idc");
            MessageBox.Show(builder.ToString(), "XmlToIdc Usage Help");
        }

        private static void UsageTables()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Usage:");
            builder.AppendLine("XmlToIdc.exe tables <cal-id>");
            builder.AppendLine();
            builder.AppendLine("<cal-id> is the Calibration id, e.g. A2WC522N");
            builder.AppendLine();
            builder.AppendLine("And you'll want to redirect stdout to a file, like:");
            builder.AppendLine("XmlToIdc.exe tables A2WC522N > Tables.idc");
            MessageBox.Show(builder.ToString(), "XmlToIdc tables Usage Help");
        }

        private static void UsageStdParam()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Usage:");
            builder.AppendLine("XmlToIdc.exe stdparam <cpu> <target> <cal-id> <ssm-base>");
            builder.AppendLine();
            builder.AppendLine("<cpu>      is the CPU bits identifier of the ECU, e.g. 16 or 32");
            builder.AppendLine("<target>   is the Car control module,");
            builder.AppendLine("             e.g. ecu (engine control unit) or tcu (transmission control unit)");
            builder.AppendLine("<cal-id>   is the Calibration id, e.g. A2WC522N");
            builder.AppendLine("<ssm-base> is the Base address of the SSM 'read' vector, e.g. 4EDDC");
            builder.AppendLine();
            builder.AppendLine("And you'll want to redirect stdout to a file, like:");
            builder.AppendLine("XmlToIdc.exe stdparam 32 ecu A2WC522N 4EDDC > StdParam.idc");
            MessageBox.Show(builder.ToString(), "XmlToIdc stdparam Usage Help");
        }

        private static void UsageExtParam()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Usage:");
            builder.AppendLine("XmlToIdc.exe extparam <cpu> <target> <ecu-id> ");
            builder.AppendLine();
            builder.AppendLine("<cpu>    is the CPU bits identifier of the ECU, e.g. 16 or 32");
            builder.AppendLine("<target> is the Car control module,");
            builder.AppendLine("           e.g. ecu (engine control unit) or tcu (transmission control unit)");
            builder.AppendLine("<ecu-id> is the ECU identifier, e.g. 2E14486106");
            builder.AppendLine();
            builder.AppendLine("And you'll want to redirect stdout to a file, like:");
            builder.AppendLine("XmlToIdc.exe extparam 16 ecu 2E14486106 > ExtParam.idc");
            MessageBox.Show(builder.ToString(), "XmlToIdc extparam Usage Help");
        }

        private static void UsageMakeAll()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Usage:");
            builder.AppendLine("XmlToIdc.exe makeall <target> <cal-id> <ssm-base>");
            builder.AppendLine();
            builder.AppendLine("<target>   is the Car control module,");
            builder.AppendLine("             e.g. ecu (engine control unit) or tcu (transmission control unit)");
            builder.AppendLine("<cal-id>   is the Calibration id, e.g. A2WC522N");
            builder.AppendLine("<ssm-base> is the Base address of the SSM 'read' vector, e.g. 4EDDC");
            builder.AppendLine();
            builder.AppendLine("And you'll want to redirect stdout to a file, like:");
            builder.AppendLine("XmlToIdc.exe makeall ecu A2WC522N 4EDDC > AllParams.idc");
            MessageBox.Show(builder.ToString(), "XmlToIdc makeall Usage Help");
        }
        #endregion
    }
}
