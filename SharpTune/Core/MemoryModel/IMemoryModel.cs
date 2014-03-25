using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTune.Core.MemoryModel
{
    public interface IMemoryModel
    {
        string name { get; }
        int cpubits {get; }
        int filesizebytes { get; }
        System.Text.Encoding encoding { get;}
    }

    public class MemoryModelDefault : IMemoryModel
    {
        public string name{
            get{return null;}
        }
        public int cpubits{
            get{return 32;}
        }
        public int filesizebytes{
            get{return 1048576;}
        }
        public System.Text.Encoding encoding{
            get{return System.Text.Encoding.UTF8;}
        }
    }

    public class MemoryModelSubaruSH7055 : IMemoryModel
    {
        public string name{
            get{return "sh7055";}
        }
        public int cpubits{
            get{return 32;}
        }
        public int filesizebytes{
            get{return 524288;}
        }
        public System.Text.Encoding encoding{
            get{return System.Text.Encoding.UTF8;}
        }
    }

    public class MemoryModelSubaruSH7058 : IMemoryModel
    {
        public string name{
            get{return "sh7058";}
        }
        public int cpubits{
            get{return 32;}
        }
        public int filesizebytes{
            get{return 1048576;}
        }
        public System.Text.Encoding encoding{
            get{return System.Text.Encoding.UTF8;}
        }
    }

    public class MemoryModelSubaruSH72531 : IMemoryModel
    {
        public string name{
            get{return "sh72531";}
        }
        public int cpubits{
            get{return 32;}
        }
        public int filesizebytes{
            get{return 1572864;}
        }
        public System.Text.Encoding encoding{
            get{return System.Text.Encoding.UTF8;}
        }
    }

    public class MemoryModelSubaruHC16 : IMemoryModel
    {
        public string name{
            get{return "68hc16y5";}
        }
        public int cpubits{
            get{return 16;}
        }
        public int filesizebytes{
            get{return 196608;}
        }
        public System.Text.Encoding encoding{
            get{return System.Text.Encoding.UTF8;}
        }
    }

    public class MemoryModelMitsubishiH8539F : IMemoryModel
    {
        public string name{
            get{return "H8539F";}
        }
        public int cpubits{
            get{return 32;}
        }
        public int filesizebytes{
            get{return 1048576;}
        }
        public System.Text.Encoding encoding{
            get{return System.Text.Encoding.UTF8;}
        }
    }

    public class MemoryModelMitsubishiSH7052 : IMemoryModel
    {
        public string name{
            get{return "SH7052";}
        }
        public int cpubits{
            get{return 32;}
        }
        public int filesizebytes{
            get{return 1048576;}
        }
        public System.Text.Encoding encoding{
            get{return System.Text.Encoding.UTF8;}
        }
    }

    public class MemoryModelMitsubishiM32186F8 : IMemoryModel
    {
        public string name{
            get{return "M32186F8";}
        }
        public int cpubits{
            get{return 32;}
        }
        public int filesizebytes{
            get{return 1048576;}
        }
        public System.Text.Encoding encoding{
            get{return System.Text.Encoding.UTF8;}
        }
    }

    public static class MemoryModels{

        public static IMemoryModel GetMemoryModel(string n){
            foreach(IMemoryModel fm in MemoryModels.memoryModels){
                if (n.ToLower() == fm.name.ToLower())
                    return fm;
            }
            throw new Exception(String.Format("MemoryModel {0} not found!!"));
        }

        public static MemoryModelDefault _Default = new MemoryModelDefault();

        static MemoryModelSubaruSH7055 _SubaruSH7055 = new MemoryModelSubaruSH7055();
        static MemoryModelSubaruSH7058 _SubaruSH7058 = new MemoryModelSubaruSH7058();
        static MemoryModelSubaruSH72531 _SubaruSH72531 = new MemoryModelSubaruSH72531();
        static MemoryModelSubaruHC16 _Subaru68HC16Y5 = new MemoryModelSubaruHC16();
        static MemoryModelMitsubishiH8539F _MitsubishiH8539F = new MemoryModelMitsubishiH8539F();
        static MemoryModelMitsubishiM32186F8 _MitsubishiM32186F8 = new MemoryModelMitsubishiM32186F8();
        static MemoryModelMitsubishiSH7052 _MitsubishiSH7052 = new MemoryModelMitsubishiSH7052();

        public static List<IMemoryModel> memoryModels = new List<IMemoryModel>() {
            _SubaruSH7055, _SubaruSH7058, _SubaruSH72531, _Subaru68HC16Y5,
            _MitsubishiH8539F, _MitsubishiM32186F8, _MitsubishiSH7052,
            _Default
        };
    }
}
