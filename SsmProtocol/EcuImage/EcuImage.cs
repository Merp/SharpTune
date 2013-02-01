using System;
using System.Collections.Generic;
using System.Text;

namespace NateW.Ssm
{
    public class EcuImage
    {
        private List<EcuImageRange> ranges;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        protected List<EcuImageRange> Ranges { get { return this.ranges; } }

        public EcuImage()
        {
            this.ranges = new List<EcuImageRange>();
        }

        public byte GetValue(int address)
        {
            foreach (EcuImageRange range in this.ranges)
            {
                byte result;
                if (range.TryGetValue(address, out result))
                {
                    return result;
                }
            }
            return 0;
        }
    }
}
