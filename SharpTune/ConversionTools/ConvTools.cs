using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SharpTune;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Text.RegularExpressions;
using SharpTuneCore;
using System.Diagnostics;

namespace SharpTune.ConversionTools
{
    public class ConvTool
    {
        public static Dictionary<string, List<Define>> Defines;
        public static Dictionary<KeyValuePair<string,List<string>>, List<string>> Sections;
        public static EcuMap ecuMap;


        public ConvTool()
        {
            Defines = new Dictionary<string, List<Define>>();
            Sections = new Dictionary<KeyValuePair<string,List<string>>, List<string>>();
        }

        public static void WriteHelp()
        {
            Trace.WriteLine("SharpTune ConvTools CopyRight Merrill A. Myers III 2012");
            Trace.WriteLine("USAGE:");
            Trace.WriteLine("Convert .map file to C defines header (.h) and section file (.txt) using .xml translation: IDAtoHEW <file.xml> <file.map> <file.h> <file.txt> <target CALID> <Build Config>");
            Trace.WriteLine("Convert .map file to IDC script: IDAtoHEW <file.map> <file.idc>");
            Trace.WriteLine("Convert .h file to IDC script: IDAtoHEW <file.h> <file.idc>");
        }

        public static bool Run(string[] args)
        {
            ConvTool prog = new ConvTool();

            if (args.Length < 1 || args[0].EqualsCI("-h") || args[0].EqualsCI("-help"))
            {
                WriteHelp();
            }
            else if (args[0].ContainsCI(".h"))
            {
                prog.ReadHeader(args[0]);
                if (args.Length < 2) //no idc filename specified, use header name
                    prog.WriteIDC(Regex.Split(args[0], ".h")[0] + ".idc");
                else if (args[1].ContainsCI(".idc"))
                    prog.WriteIDC(args[1]);
                else
                    WriteHelp();
            }
            else if (args[0].ContainsCI(".map"))
            {
                prog.MapToDefines(args[0]);
                if (args.Length < 2) //no filename specified
                    prog.WriteIDC(Regex.Split(args[0], ".map")[0] + ".idc");
                else if (args[1].ContainsCI(".idc"))
                    prog.WriteIDC(args[1]);
                else
                    WriteHelp();
            }
            else if (args[0].ContainsCI(".xml") && args[1].ContainsCI(".map") && args[2].ContainsCI(".h") && args[3].ContainsCI(".txt") && args.Length == 6)
            {
                prog.UpdateTargetHeader(args[0], args[1], args[2], args[3], args[4], args[5]);
            }
            else
                WriteHelp();
            return true; //TODO FIX!!
        }

        public void UpdateTargetHeader(string xmlFileName, string mapFileName, string headerFileName, string linkerScriptFileName, string romCalId, string buildConfig)
        {
            String Build;
            String Config = buildConfig.Split('_')[0];
            if (buildConfig.Split('_').Length > 1)
                Build = buildConfig.Split('_')[1];
            else
                Build = "Debug";

            LoadXML(xmlFileName);
            ecuMap = new EcuMap(mapFileName,headerFileName);
            Definition def;

            if(SharpTuner.AvailableDevices.DefDictionary.ContainsKey(romCalId))
                def = SharpTuner.AvailableDevices.DefDictionary[romCalId];
            else
            {
                Trace.WriteLine("Error, rom calid not found!!");
                return;
            }

            FindAndWriteDefines(headerFileName, Build, Config, def.CarInfo["internalidstring"].ToString(), def.CarInfo["ecuid"].ToString());
        }

        public void LoadXML(string file)
        {
            XDocument xmlDoc = XDocument.Load(file, LoadOptions.PreserveWhitespace);
            XElement xdefs = xmlDoc.XPathSelectElement("//idatohew/defines");
            foreach (XElement xel in xdefs.Elements("category"))
            {
                List<Define> td = new List<Define>();
                foreach (XElement xe in xel.Elements("define"))
                {
                    td.Add(new Define(xe));
                }
                Defines.Add(xel.Attribute("name").Value.ToString(), td);
            }
            foreach (XElement xel in xmlDoc.XPathSelectElements("//idatohew/sections").Elements("section"))
            {
                List<string> ls = new List<string>();
                foreach (XElement s in xel.Elements("subsection"))
                {
                    ls.Add(s.Attribute("name").Value.ToString());
                }
                List<string>als = new List<string>();
                foreach (XElement s in xel.Elements("alias"))
                {
                    als.Add(s.Attribute("name").Value.ToString());
                }
                Sections.Add(new KeyValuePair<string,List<string>>(xel.Attribute("name").Value.ToString(),als) , ls);
            }
        }

        /*public void DumpXML(string file)
        {
            XDocument xmlDoc = new XDocument();
            XElement xc = new XElement("idatohew");
            xmlDoc.Add(xc);
            //create xml file
            XElement xd = new XElement("defines");
            foreach (KeyValuePair<string, string> kvp in Defines)
            {
                XElement xel = new XElement("define");
                xel.SetAttributeValue("name",kvp.Key);
                xel.SetAttributeValue("type", kvp.Value);
                xd.Add(xel);
            }
            xc.Add(xd);
            
            XElement xs = new XElement("sections");
            foreach (KeyValuePair<string, List<string>> sec in Sections)
            {
                XElement xel = new XElement("section");
                xel.SetAttributeValue("name", sec.Key);
                foreach (string s in sec.Value)
                {
                    XElement cxel = new XElement("subsection");
                    cxel.SetAttributeValue("name", s);
                    xel.Add(cxel);
                }
                xs.Add(xel);
            }

            xc.Add(xs);

            xmlDoc.Save(file);
        }*/

        public void ReadHeader(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                string line;
                List<Define> defines = new List<Define>();
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.ContainsCI("#define"))
                    {
                        string[] ln = line.Split(' ');
                        defines.Add(new Define(ln[1], Regex.Split(ln[2],"0x")[1].Split(')')[0]));
                    }
                }
                Defines.Add("defines", defines);
            }
        }

        public void MapToDefines(String map)
        {
            ecuMap = new EcuMap(map);
            foreach (List<Define> category in Defines.Values)
            {
                foreach (Define define in category)
                {
                    define.findOffset(ecuMap.IdaNames);
                }
            }
        }

        public void WriteIDC(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.WriteLine("#include <idc.idc>");
                writer.WriteLine("static Params()");
                writer.WriteLine("{");
                foreach(var c in Defines)
                {
                    writer.WriteLine("/////////////////////");
                    writer.WriteLine("//"+c.Key);
                    writer.WriteLine("/////////////////////");
                    foreach (Define d in c.Value)
                    {
                        if(d.offset.Length == 8)
                            writer.WriteLine("MakeNameEx(0x" + d.offset + ", \"" + d.name + "\", SN_CHECK);");
                    }
                }
                writer.WriteLine("}");
                writer.WriteLine("static main () { Params(); }");
            }
        }        

        public void FindAndWriteDefines(string outfilename, String Build, String Config, String CalId, String EcuId)
        {
            //write date tag!
            using (StreamWriter writer = new StreamWriter(outfilename))
            {
                writer.WriteLine("#define MOD_DATE " + DateTime.Today.Year.ToString().Substring(2) + "." + DateTime.Today.Month.ToString() + "." + DateTime.Today.Day.ToString() + "." + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + System.Environment.NewLine);
                writer.WriteLine(@"#include """ + Config + @".h""");
                writer.WriteLine("#define MOD_CONFIG " + Config);
                writer.WriteLine("#define MOD_BUILD " + Build);
                if (Build.ContainsCI("release"))
                    writer.WriteLine("#define MOD_RELEASE 1");
                else
                    writer.WriteLine("#define MOD_RELEASE 0");
                writer.WriteLine("#define ECU_CALIBRATION_ID " + CalId);
                writer.WriteLine("#define ECU_IDENTIFIER " + EcuId);
                foreach (var category in Defines)
                {
                    Trace.WriteLine("Defining Category " + category.Key.ToString());
                    writer.WriteLine("/////////////////////");
                    writer.WriteLine("// " + category.Key.ToString());
                    writer.WriteLine("/////////////////////");
                    writer.WriteLine("");
                    foreach (var def in category.Value)
                    {
                        def.findOffset(ecuMap.IdaNames);
                        def.print(writer);
                    }
                    writer.WriteLine("");
                }
            }
        }

        public void FindAndWriteSections(string outfilename)
        {
            using (StreamWriter writer = new StreamWriter(outfilename))
            {
                bool found = false;
                writer.WriteLine("SECTIONS");
                writer.WriteLine("{");
                foreach (KeyValuePair<KeyValuePair<string,List<string>>, List<string>> ls in Sections)
                {
                    List<string> tl = new List<string>();
                    tl.Add(ls.Key.Key);
                    tl.AddRange(ls.Key.Value);
                    found = false;

                    foreach (string sec in tl)
                    {
                        foreach (var idan in ecuMap.IdaNames)
                        {
                            if (idan.Key.ToString().EqualsCI(sec))
                            {
                                writer.WriteLine("\t" + ls.Key.Key.ToString() + " 0x" + idan.Value + " : AT (0x" + idan.Value + ")");
                                writer.WriteLine("\t{");
                                foreach (string section in ls.Value)
                                {
                                    writer.WriteLine("\t\t*(" + section.ToString() + ")");
                                }
                                writer.WriteLine("\t}");
                                found = true;
                                break;
                            }
                            if (found)
                                break;
                        }
                        if (found)
                            break;
                    }
                }
                writer.WriteLine("}");
            }
        }            
    }
}
