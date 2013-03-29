using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SharpTune;

namespace SharpTune.ConversionTools
{
    public class IdaDef
    {
        public string name { get; private set; }
        public string type { get; private set; }
        public int priority { get; private set; }

        public IdaDef(XElement xi)
        {
            name = xi.Attribute("name").Value.ToString();
            if (xi.Attribute("type") != null)
                type = xi.Attribute("type").Value.ToString();
            else
                type = "default";
            if (xi.Attribute("priority") != null)
                priority = int.Parse(xi.Attribute("priority").Value.ToString());
            else
                priority = 1;
        }

        public IdaDef(string n)
        {
            name = n;
            type = "default";
            priority = 1;
        }
    }
}
