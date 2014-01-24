using SharpTuneCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SharpTune
{
    class DefinitionTools
    {
        public static bool DefinitionTool(string[] args){

            if(args[1] == "updaterrecudefs" && args.Length == 4)
            {
                return UpdateRRECUDefs(args[2], args[3]);
            }

            return false;
        }

        public static bool UpdateRRECUDefs(string filename, string search)
        {
            try
            {
                //Load the definition
                Trace.WriteLine("Updating RR ECU Defs in: " + filename + " using search pattern: " + search);
                XDocument xmlDoc = XDocument.Load(filename);//, LoadOptions.PreserveWhitespace);

                //Get the stubs]
                List<XElement> stubs = new List<XElement>();
                foreach (KeyValuePair<string, Definition> entry in SharpTuner.AvailableDevices.DefDictionary)
                {
                    if (entry.Key.ContainsCI(search))
                    {
                        stubs.Add(entry.Value.ExportRRRomId());
                    }
                }
                Trace.WriteLine("Found " + stubs.Count + " definitions");
                foreach (XElement stub in stubs)
                {
                    xmlDoc.Element("roms").Add(stub);
                }

                xmlDoc.Save(filename.Replace(".xml","") + "_" + search + ".xml");
            }
            catch (Exception e)
            {
                Trace.WriteLine("Error: " + e.Message);
                return false;
            }

            return true;
        }
    }
}
