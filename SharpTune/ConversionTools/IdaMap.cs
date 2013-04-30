using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpTune.ConversionTools
{
    public class IdaMap
    {
        public Dictionary<string, string> IdaNames { get; private set; }
        public Dictionary<List<string>, string> IdaCleanNames { get; private set; }

        public IdaMap()
        {
            IdaNames = new Dictionary<string, string>();
            IdaCleanNames = new Dictionary<List<string>, string>();
        }

        public IdaMap(string fileOrText)
            : this()
        {
            if (File.Exists(fileOrText))
                LoadFromFile(fileOrText);
            else
                LoadFromString(fileOrText);
        }

        public void LoadFromFile(string filename)
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
                            IdaNames.Add(name, offsetstr);
                        }
                        else
                        {
                            if (!isram) offsetstr = line.Substring(6, 8);
                            else offsetstr = "FFFF" + line.Substring(10, 4);
                            string name = line.Substring(21, line.Length - 21);
                            IdaNames.Add(name, offsetstr);
                        }
                    }
                }
            }
            CleanNames();
        }   

        public void LoadFromString(string str)
        {
            Dictionary<string, string> inputMap = new Dictionary<string, string>();
            using (StringReader reader = new StringReader(str))
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
                            IdaNames.Add(l[0], addr.Substring(addr.Length-6));
                        else
                            Trace.WriteLine("error parsing line: " + line + Environment.NewLine + "Try using a map file");
                    }

                } while (line != null);
            }
            CleanNames();
        }

        public void CleanNames()
        {
            IdaCleanNames.Clear();
            foreach (var n in IdaNames)
            {
                IdaCleanNames.Add(n.Key.CleanIdaString(), n.Value);
            }
        }

    //TODO build from raw string input
    }
}
