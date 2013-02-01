using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Merp;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;
using System.Text.RegularExpressions;

namespace ComvTools
{
    class ConvTools
    {
        public static Dictionary<string, List<Define>> Defines;
        public static Dictionary<string, List<string>> Sections;
        public static List<IdaName> IdaNames;

        static void Run(string[] args)
        {
            ConvTools prog = new ConvTools();
            Defines = new Dictionary<string, List<Define>>();
            Sections = new Dictionary<string, List<string>>();
            IdaNames = new List<IdaName>();
            if (args.Length < 1 || args[0].EqualsCI("-h") || args[0].EqualsCI("-help"))
            {
                Console.WriteLine("IDAtoHEW CopyRight Merrill A. Myers III 2012");
                Console.WriteLine("USAGE:");
                Console.WriteLine("Convert .map file to C defines header (.h) and section file (.txt) using .xml translation: IDAtoHEW <file.xml> <file.map> <file.h> <file.txt>");
                Console.WriteLine("Convert .map file to IDC script: IDAtoHEW <file.map> <file.idc>");
                Console.WriteLine("Convert .h file to IDC script: IDAtoHEW <file.h> <file.idc>");
            }
            else if (args[0].ContainsCI(".h"))
            {
                prog.ReadHeader(args[0]);
                if (args.Length < 3)
                    prog.WriteIDC(Regex.Split(args[0], ".h")[0] + ".idc");
                else
                    prog.WriteIDC(args[1]);
            }
            else if (args[0].ContainsCI(".map"))
            {
                prog.ReadLines(args[0]);
                prog.NamesToDefines();
                if (args.Length < 3)
                    prog.WriteIDC(Regex.Split(args[0], ".map")[0] + ".idc");
                else
                    prog.WriteIDC(args[1]);
            }
            else if (args[0].ContainsCI(".xml") && args.Length == 4)
            {
                prog.LoadXML(args[0]);
                prog.ReadLines(args[1]);
                prog.FindAndWriteDefines(args[2]);
                prog.FindAndWriteSections(args[3]);
            }
            else
                Console.WriteLine("invalid command!");
        }

        public void LoadXML(string file)
        {
            XDocument xmlDoc = XDocument.Load(file);
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
                foreach (XElement s in xel.Elements())
                {
                    ls.Add(s.Attribute("name").Value.ToString());
                }
                Sections.Add(xel.Attribute("name").Value.ToString(), ls);
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
                        if(d.define.Length == 8)
                            writer.WriteLine("MakeNameEx(0x" + d.define + ", \"" + d.name + "\", SN_CHECK);");
                    }
                }
                writer.WriteLine("}");
                writer.WriteLine("static main () { Params(); }");
            }
        }

        public void NamesToDefines()
        {
            List<Define> defines = new List<Define>();
            foreach (var idn in IdaNames)
            {
                defines.Add(new Define(idn.name, idn.define));
            }
            Defines.Add(" ", defines);
        }

        public void ReadLines(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                bool isram = false;
                string line;
                int offset = 0;
                int offsetdelta = 0;
                bool start = false;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.ContainsCI("Publics by Value"))
                    {
                        start = true;
                        continue;
                    }
                    if (start && line.Length > 21)
                    {
                        string offsetstr = line.Substring(6, 8);
                        int offs = int.Parse(offsetstr, System.Globalization.NumberStyles.HexNumber);
                        offsetdelta = offs - offset;
                        offset = offs;
                        if (offsetdelta < 0) isram = true;
                        if (line.ToString().ContainsCI("BITMASK"))
                        {
                            offsetstr = line.Substring(line.Length - 2, 2);
                            string name = line.Substring(21, line.Length - 26);
                            IdaNames.Add(new IdaName(name, offsetstr));
                        }
                        else
                        {
                            if (!isram) offsetstr = line.Substring(6, 8);
                            else offsetstr = "FFFF" + line.Substring(10, 4);
                            string name = line.Substring(21, line.Length - 21);
                            IdaNames.Add(new IdaName(name, offsetstr));
                        }
                    }
                }
            }
        }

        public void FindAndWriteDefines(string outfilename)
        {
            using (StreamWriter writer = new StreamWriter(outfilename))
            {
                foreach (var category in Defines)
                {
                    writer.WriteLine("/////////////////////");
                    writer.WriteLine("// " + category.Key.ToString());
                    writer.WriteLine("/////////////////////");
                    writer.WriteLine("");
                    foreach (var def in category.Value)
                    {
                        def.find(IdaNames);
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
                writer.WriteLine("SECTIONS");
                writer.WriteLine("{");
                foreach (KeyValuePair<string, List<string>> sec in Sections)
                {
                    foreach (var idan in IdaNames)
                    {
                        if (idan.name.ToString().EqualsCI(sec.Key))
                        {
                            writer.WriteLine("\t" + idan.name + " 0x" + idan.define + " : AT (0x" + idan.define + ")");
                            writer.WriteLine("\t{");
                            foreach (string section in sec.Value)
                            {
                                writer.WriteLine("\t\t*(" + section.ToString() + ")");
                            }
                            writer.WriteLine("\t}");
                        }
                    }
                }
                writer.WriteLine("}");
            }
        }            
    }
}
