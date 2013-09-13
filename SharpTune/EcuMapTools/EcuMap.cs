using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpTune.EcuMapTools
{
    /// <summary>
    /// Encapsulates EcuRefs for an entire ecu
    /// Contains methods to import/export EcuRefs
    /// TODO: Refactor code from EcuMapTool into this
    /// TODO: Populate definiitions with this imported data
    /// </summary>
    public class EcuMap
    {
        public Dictionary<string, string> Locs { get; private set; }
        public Dictionary<List<string>, string> CleanLocs {
            get
            {
                cleanDefines.Clear();
                foreach (var n in Locs)
                {
                    cleanDefines.Add(n.Key.CleanDefineString(), n.Value);
                }
                return cleanDefines;
            }
        }
        private Dictionary<List<string>, string> cleanDefines;

        public EcuMap()
        {
            Locs = new Dictionary<string, string>();
            cleanDefines = new Dictionary<List<string>, string>();
        }

        public void ImportFromMapFileOrText(string fileOrText)
        {
            Locs.Clear();
            if (File.Exists(fileOrText))
                LoadFromMapFile(fileOrText);
            else
                LoadFromMapString(fileOrText);
        }

        public bool ImportFromHeaderAndMapFile(string mapFileName, string headerFileName)
        {
            Trace.WriteLine("Importing header and map file");
            Locs.Clear();

            if (!File.Exists(headerFileName) && !File.Exists(mapFileName))
                return false;

            if (File.Exists(headerFileName))
            {
                try
                {
                    LoadHeader(headerFileName);
                    Trace.WriteLine("Loaded header file: " + headerFileName);
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Error loading header file: " + headerFileName);
                    Trace.WriteLine(e.Message);
                }
            }
            else 
                Trace.WriteLine("No existing header file found: " + headerFileName);

            if (File.Exists(mapFileName))
            {
                try
                {
                    LoadFromMapFile(mapFileName);
                    Trace.WriteLine("Loaded map file: " + mapFileName);
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Error loading map file: " + mapFileName);
                    Trace.WriteLine(e.Message);
                }
            }
            else
                Trace.WriteLine("No existing map file found: " + mapFileName);
            return true;
        }
        
        private void LoadHeader(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                string line;
                List<EcuLoc> defines = new List<EcuLoc>();
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.ContainsCI("#define"))
                    {
                        string[] ln = line.Split(' ');
                        try
                        {
                            if(ln[2].Contains("0x"))
                                Locs.Add(ln[1], Regex.Split(ln[2], "0x")[1].Split(')')[0]);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine("Error processing header line:");
                            Trace.WriteLine(line);
                            Trace.WriteLine("Array length: " + ln.Length.ToString());
                            Trace.WriteLine("Error: " + e.Message);
                        }
                    }
                }
            }
        }

        private void LoadFromMapFile(string filename)
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
                            if (Locs.ContainsKey(name))
                                Locs.Remove(name);
                            Locs.Add(name, offsetstr);
                        }
                        else
                        {
                            if (!isram) offsetstr = line.Substring(6, 8);
                            else offsetstr = "FFFF" + line.Substring(10, 4);
                            string name = line.Substring(21, line.Length - 21);
                            if (Locs.ContainsKey(name))
                                Locs.Remove(name);
                            Locs.Add(name, offsetstr);
                        }
                    }
                }
            }
        }   

        private void LoadFromMapString(string str)
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
                            Locs.Add(l[0], addr.Substring(addr.Length-6));
                        else
                            Trace.WriteLine("error parsing line: " + line + Environment.NewLine + "Try using a map file");
                    }

                } while (line != null);
            }
        }
    }
}
