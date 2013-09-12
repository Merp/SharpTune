using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SharpTune;

namespace SharpTune.ConversionTools
{
    public class EcuRef
    {
        public string name { get; private set; }
        public string type { get; private set; } //TODO: use enum instead??
        public int priority { get; private set; }

        public EcuRef(XElement xi)
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

        public EcuRef(string n)
        {
            name = n;
            type = "default";
            priority = 1;
        }
    }
}
