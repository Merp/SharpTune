using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SharpTune;
using System.IO;
using System.Diagnostics;

namespace SharpTune.EcuMapTools
{
    /// <summary>
    /// Represents a location within the ecu
    /// Encapsulates the EcuLocCandidates (potential names in .map file)
    /// used to identify the locations
    /// </summary>
    public class EcuLoc
    {

        public string name { get; private set; }
        public string dataType { get; private set; } //TODO: enums??? 
        public Dictionary<int,EcuLocCandidate> ecuRefCandidates { get; private set; }
        public string offset {get; private set; }
        public string bit {get; private set; }

        public EcuLoc(XElement xe)
        {
            ecuRefCandidates = new Dictionary<int, EcuLocCandidate>();
            name = xe.Attribute("name").Value.ToString();
            if (xe.Attribute("type") != null)
                dataType = xe.Attribute("type").Value.ToString();
            else
                dataType = null;
            IEnumerable<XElement> te = xe.Elements();
            ecuRefCandidates.Add(0,new EcuLocCandidate(name));
            foreach (var xi in xe.Elements())
            {
                EcuLocCandidate tid = new EcuLocCandidate(xi);
                ecuRefCandidates.Add(tid.priority, tid);
            }
        }

        public EcuLoc(string n, string hexaddr)
        {
            name = n;
            offset = hexaddr;
        }

        public string printType()
        {
            if (dataType == null)
                return null;
            return "(" + dataType + ")";
        }

        public void print(StreamWriter writer)
        {
            if (offset == null)
                Trace.WriteLine("COULD NOT FIND DEFINE: " + name);
            else
                writer.WriteLine("#define " + name + " (" + this.printType() + "0x" + offset + ")");
        }

        public void findOffset(Dictionary<string, string> map)
        {
            foreach (var entry in map)
            {
                for (int i = 0; i < ecuRefCandidates.Count; i++)
                {
                    if (entry.Key.EqualsCI(ecuRefCandidates[i].name))
                    {
                        offset = entry.Value.ToString();
                        return;
                    }
                }
            }
        }
    }
}
