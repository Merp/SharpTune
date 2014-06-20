using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTune.Core.Checksum
{
    public interface IChecksumModule
    {
        string Name { get; }
    }

    public static class ChecksumModules
    {
        public static IChecksumModule GetChecksumModule(string n)
        {
            foreach (IChecksumModule csm in checksumModules)
            {
                if (n.ToLower() == csm.Name.ToLower())
                    return csm;
            }
            throw new Exception(String.Format("ChecksumModule {0} not found !!", n));
        }

        static ChecksumSubaruDBW _subarudbw = new ChecksumSubaruDBW();//TODO: need to use a factory!

        public static List<IChecksumModule> checksumModules = new List<IChecksumModule>() {
            _subarudbw,
        };
    }
}
