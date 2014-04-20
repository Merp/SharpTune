using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTune.Core.Checksum
{
    [Serializable]
    public class ChecksumSubaruDBW : IChecksumModule
    {
        public string Name
        {
            get { return "subarudbw"; }
        }
    }
}
