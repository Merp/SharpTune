using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConvTools
{
    public class IdaName
    {
        public string name { get; private set; }
        public string rawData { get; private set; }
        public string type { get; private set; }
        public string define { get; private set; }

        public IdaName(string n, string offset)
        {
            name = n;
            define = offset;
        }

        public string getBitMask()
        {
            return define;
        }

        public string getOffset()
        {
            return define;
        }
    }
}
