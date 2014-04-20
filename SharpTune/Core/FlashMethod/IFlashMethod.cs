using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTune.Core.FlashMethod
{
    public interface IFlashMethod
    {
        string name { get; }
    }

    [Serializable]
    public class FlashMethodWRX02 : IFlashMethod
    {
        public string name{
            get{return "wrx02";}
        }
    }

    [Serializable]
    public class FlashMethodWRX04 : IFlashMethod
    {
        public string name{
            get{return "wrx04";}
        }
    }

    [Serializable]
    public class FlashMethodSTI04 : IFlashMethod
    {
        public string name{
            get{return "sti04";}
        }
    }

    [Serializable]
    public class FlashMethodSTI05 : IFlashMethod
    {
        public string name{
            get{return "sti05";}
        }
    }

    [Serializable]
    public class FlashMethodSubaruCAN : IFlashMethod
    {
        public string name{
            get{return "subarucan";}
        }
    }

    [Serializable]
    public class FlashMethodSubaruBRZ : IFlashMethod
    {
        public string name{
            get{return "subarubrz";}
        }
    }

    [Serializable]
    public static class FlashMethods{

        public static IFlashMethod GetFlashMethod(string n){
            foreach(IFlashMethod fm in FlashMethods.flashMethods){
                if (n.ToLower() == fm.name.ToLower())
                    return fm;
            }
            throw new Exception(String.Format("FlashMethod {0} not found!!"));
        }

        static FlashMethodWRX02 _wrx02 = new FlashMethodWRX02();
        static FlashMethodWRX04 _wrx04 = new FlashMethodWRX04();
        static FlashMethodSTI04 _sti04 = new FlashMethodSTI04();
        static FlashMethodSTI05 _sti05 = new FlashMethodSTI05();
        static FlashMethodSubaruCAN _subarucan = new FlashMethodSubaruCAN();
        static FlashMethodSubaruBRZ _subarubrz = new FlashMethodSubaruBRZ();

        public static List<IFlashMethod> flashMethods = new List<IFlashMethod>() {
            _wrx02, _wrx04, _sti04, _sti05, _subarucan, _subarubrz
        };
    }
}
