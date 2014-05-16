using SharpTuneCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpTune.EcuMapTools
{
    public static class SSMBase
    {
        public static string getSSMBase(Stream imageStream)
        {
            byte[] byc = new byte[4];
            long highlimit = 5000000;
            long lowlimit = 100000;
            long difflimit = 100000;
            if (imageStream.Length < highlimit)
                highlimit = imageStream.Length;
            for (long i = lowlimit; i < highlimit; i += 4)
            {
                long start = i;
                imageStream.Seek(i, SeekOrigin.Begin);
                if (SSMBaseRecursion(imageStream, i, 0, 0, difflimit))
                    return start.ToString("X");
                else
                    continue;
            }
            difflimit += 40000;
            for (long i = lowlimit; i < highlimit; i += 4)
            {
                long start = i;
                imageStream.Seek(i, SeekOrigin.Begin);
                if (SSMBaseRecursion(imageStream, i, 0, 0, difflimit))
                    return start.ToString("X");
                else
                    continue;
            }
            return "Enter SSM Base";
        }

        private static bool SSMBaseRecursion(Stream imageStream, long currentoffset, int lastaddress, int recursionlevel, long min)
        {
            int addinc;
            if (recursionlevel < 6)
                addinc = 17;
            else
                addinc = 1000;
            byte[] byc = new byte[4];
            int bc = 0;
            imageStream.Read(byc, 0, 4);

            byc.ReverseBytes();
            bc = BitConverter.ToInt32(byc, 0);
            if (recursionlevel == 0)
                lastaddress = bc;
            if (bc > 0 && Math.Abs(currentoffset - bc) < min && lastaddress + addinc > bc)
            {
                if (recursionlevel > 40)
                    return true;
                recursionlevel++;
                currentoffset += 4;
                return SSMBaseRecursion(imageStream, currentoffset, bc, recursionlevel, min);
            }
            else
                return false;
        }
    }
}
