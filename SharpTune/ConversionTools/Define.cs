using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Merp;
using System.IO;

namespace ComvTools
{
    class Define
    {
        public string name { get; private set; }
        public string type { get; private set; }
        public Dictionary<int,IdaDef> idaDefs { get; private set; }
        public string define {get; private set; }

        public Define(XElement xe)
        {
            idaDefs = new Dictionary<int, IdaDef>();
            name = xe.Attribute("name").Value.ToString();
            if (xe.Attribute("type") != null)
                type = xe.Attribute("type").Value.ToString();
            else
                type = null;
            IEnumerable<XElement> te = xe.Elements();
            idaDefs.Add(0,new IdaDef(name));
            foreach (var xi in xe.Elements())
            {
                IdaDef tid = new IdaDef(xi);
                idaDefs.Add(tid.priority, tid);
            }
        }

        public Define(string n, string hexaddr)
        {
            name = n;
            define = hexaddr;
        }

        public string printType()
        {
            if (type == null)
                return null;
            return "(" + type + ")";
        }

        public void find(List<IdaName> inames)
        {
            string tdef = null;
            for (int i = 0; i < idaDefs.Count; i++)
            {
                tdef = idaDefs[i].findName(inames);
                if(tdef != null) 
                    break;
            }
            define = tdef;
        }

        public void print(StreamWriter writer)
        {
            if (define == null)
                Console.WriteLine("COULD NOT FIND DEFINE: " + name);
            else
                writer.WriteLine("#define " + name + " (" + this.printType() + "0x" + define + ")");
        }
    }
}
