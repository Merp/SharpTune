using SharpTune.Core.Checksum;
using SharpTune.Core.FlashMethod;
using SharpTune.Core.MemoryModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SharpTune.Core
{    
    public static class ECUIdTags{
        public const string caseid = "caseid";
        public const string xmlid = "xmlid";
        public const string filesize = "filesize";
        public const string calidaddress = "internalidaddress";
        public const string calidstring = "internalidstring";
        public const string calidhex = "internalidhex";
        public const string ecuidstring = "ecuidstring";
        public const string ecuidhex = "ecuid";
        public const string year = "year";
        public const string make = "make";
        public const string model = "model";
        public const string transmission = "transmission";
        public const string market = "market";
        public const string submodel = "submodel";
        public const string memorymodel = "memmodel";
        public const string flashmethod = "flashmethod";
        public const string checksummodule = "checksummodule";
        public const string include = "include";
    }

    [Serializable]
    public class ECUIdentifier
    {

        public Dictionary<string, string> propertyBag { get; private set; }

        public bool isReady {get; private set;}
        public string xmlid
        {
            get
            {
                if (propertyBag.ContainsKey(ECUIdTags.xmlid))
                    return propertyBag[ECUIdTags.xmlid];
                else
                    return null;
            }
            private set { propertyBag[ECUIdTags.xmlid] = value; }
        }

        public string caseid {
            get { 
                if(propertyBag.ContainsKey(ECUIdTags.caseid))
                    return propertyBag[ECUIdTags.caseid];
                else
                    return memoryModel.filesizebytes.ToKiloBytesString();
            }
            private set { propertyBag[ECUIdTags.caseid] = value;}
        }

        public string filesize {
            get { 
                if(propertyBag.ContainsKey(ECUIdTags.filesize))
                    return propertyBag[ECUIdTags.filesize];
                else
                    return memoryModel.filesizebytes.ToKiloBytesString();
            }
            private set { propertyBag[ECUIdTags.filesize] = value;}
        }

        public int? CalibrationIdAddress
        {
            get
            {
                if (propertyBag.ContainsKey(ECUIdTags.calidaddress))
                    return propertyBag[ECUIdTags.calidaddress].ConvertHexToInt();
                else
                    return null;
            }
            private set
            {
                propertyBag[ECUIdTags.calidaddress] = value.ConvertIntToHexString();
            }
        }

        private void setCalibrationIdAddressFromHex(string hex){
            CalibrationIdAddress = hex.ConvertHexToInt();
        }

        public string CalibrationIdString {
            get{
                if (propertyBag.ContainsKey(ECUIdTags.calidstring))
                    return propertyBag[ECUIdTags.calidstring];
                else
                    return null;
             }
             private set{
                 propertyBag[ECUIdTags.calidstring] = value;
                 propertyBag[ECUIdTags.calidhex]= value.ConvertStringToHex(memoryModel.encoding);
             }
        }

        public string CalibrationIdHexString {
            get{
                if (propertyBag.ContainsKey(ECUIdTags.calidhex))
                    return propertyBag[ECUIdTags.calidhex];
                else
                    return null;
            }
            private set{
                propertyBag[ECUIdTags.calidhex] = value;
                propertyBag[ECUIdTags.calidstring] = value.ConvertHexToString(memoryModel.encoding);
            }
        }

        public Byte[] CalibrationIdHexBytes
        {
            get
            {
                if (propertyBag.ContainsKey(ECUIdTags.calidhex))
                    return propertyBag[ECUIdTags.calidhex].ConvertHexStringToByteArray();
                else if (propertyBag.ContainsKey(ECUIdTags.calidstring))
                    return propertyBag[ECUIdTags.calidstring].ConvertStringToBytes(memoryModel.encoding);
                else
                    return null;
            }
            private set
            {
                propertyBag[ECUIdTags.calidstring] = value.ConvertBytesToString(memoryModel.encoding);
                propertyBag[ECUIdTags.calidhex] = value.ConvertBytesToHexString();
            }
        }

        public string EcuIdString {
            get{
                if(propertyBag.ContainsKey(ECUIdTags.ecuidstring))
                    return propertyBag[ECUIdTags.ecuidstring];
                else
                    return null;
            }
             private set{
                 try
                 {
                     propertyBag[ECUIdTags.ecuidstring] = value;
                 }
                 catch (Exception crap)
                 {
                     Trace.WriteLine(string.Format("Error setting ECUID string in definition: {0} , ecuid: {1}", this.xmlid, value));
                     Trace.WriteLine(crap.Message);
                 }
             }
        }

        public string EcuIdHexString {
            get{
                if(propertyBag.ContainsKey(ECUIdTags.ecuidhex))
                    return propertyBag[ECUIdTags.ecuidhex];
                else
                    return null;
            }
            private set{
                try
                {
                    propertyBag[ECUIdTags.ecuidhex] = value;
                }
                catch (Exception crap)
                {
                    Trace.WriteLine(string.Format("Error setting ECUID HEX string in definition: {0} , ecuid: {1}", this.xmlid, value));
                    Trace.WriteLine(crap.Message);
                }
            }
        }

        public Byte[] EcuIdHexBytes {
            get{
                if (propertyBag.ContainsKey(ECUIdTags.ecuidstring))
                    return propertyBag[ECUIdTags.ecuidstring].ConvertStringToBytes(memoryModel.encoding);
                else if (propertyBag.ContainsKey(ECUIdTags.ecuidhex))
                    return propertyBag[ECUIdTags.ecuidhex].ConvertHexStringToByteArray();
                else
                    return null;
            }
            private set{
                propertyBag[ECUIdTags.ecuidhex] = value.ConvertBytesToHexString();
            }
        }

        public string year
        {
            get
            {
                if (propertyBag.ContainsKey(ECUIdTags.year))
                    return propertyBag[ECUIdTags.year];
                else
                    return null;
            }
            private set
            {
                propertyBag[ECUIdTags.year] = value;
            }
        }

        public string market
        {
            get
            {
                if (propertyBag.ContainsKey(ECUIdTags.market))
                    return propertyBag[ECUIdTags.market];
                else
                    return null;
            }
            private set
            {
                propertyBag[ECUIdTags.market] = value;
            }
        }
        public string make
        {
            get
            {
                if (propertyBag.ContainsKey(ECUIdTags.make))
                    return propertyBag[ECUIdTags.make];
                else
                    return null;
            }
            private set
            {
                propertyBag[ECUIdTags.make] = value;
            }
        }
        public string model
        {
            get
            {
                if (propertyBag.ContainsKey(ECUIdTags.model))
                    return propertyBag[ECUIdTags.model];
                else
                    return null;
            }
            private set
            {
                propertyBag[ECUIdTags.model] = value;
            }
        }
        public string submodel
        {
            get
            {
                if (propertyBag.ContainsKey(ECUIdTags.submodel))
                    return propertyBag[ECUIdTags.submodel];
                else
                    return null;
            }
            private set
            {
                propertyBag[ECUIdTags.submodel] = value;
            }
        }
        public string transmission
        {
            get
            {
                if (propertyBag.ContainsKey(ECUIdTags.transmission))
                    return propertyBag[ECUIdTags.transmission];
                else
                    return null;
            }
            private set
            {
                propertyBag[ECUIdTags.transmission] = value;
            }
        }
        public string include
        {
            get
            {
                if (propertyBag.ContainsKey(ECUIdTags.include))
                    return propertyBag[ECUIdTags.include];
                else
                    return null;
            }
            set //TODO FIX KLUDGE
            {
                propertyBag[ECUIdTags.include] = value;
            }
        }

        public IMemoryModel memoryModel
        {
            get
            {
                if (propertyBag.ContainsKey(ECUIdTags.memorymodel))
                    return MemoryModels.GetMemoryModel(propertyBag[ECUIdTags.memorymodel]);
                else
                    return MemoryModels._Default;
            }
            private set
            {
                propertyBag[ECUIdTags.memorymodel] = value.name;
                filesize = (value.filesizebytes / 1000).ToString("D") + "kb";
            }
        }

        private void setMemoryModel(string mm)
        {
            memoryModel = MemoryModels.GetMemoryModel(mm);
        }
   
        public IFlashMethod flashMethod
        {
            get
            {
                if (propertyBag.ContainsKey(ECUIdTags.flashmethod))
                    return FlashMethods.GetFlashMethod(propertyBag[ECUIdTags.flashmethod]);
                else
                    return null;
            }
            private set
            {
                propertyBag[ECUIdTags.flashmethod] = value.name;
            }
        }

        public void setFlashMethod(string fm)
        {
            flashMethod = FlashMethods.GetFlashMethod(fm);
        }

        public IChecksumModule checksumModule
        {
            get
            {
                if (propertyBag.ContainsKey(ECUIdTags.checksummodule))
                    return ChecksumModules.GetChecksumModule(propertyBag[ECUIdTags.checksummodule]);
                else
                    return null;
            }
            private set
            {
                propertyBag[ECUIdTags.checksummodule] = value.Name;
            }
        }

        public void setChecksumModule(string csm)
        {
            checksumModule = ChecksumModules.GetChecksumModule(csm);
        }

        public void UpdateFromMod(RomMod.Mod mod)
        {
            if(memoryModel == null)
                memoryModel = MemoryModels._Default;
            include = mod.InitialCalibrationId;
            CalibrationIdAddress = (int?)mod.ModIdentAddress;
            CalibrationIdString = mod.ModIdent.ToString();
            EcuIdHexString = mod.FinalEcuId.ToString();
            xmlid = mod.ModIdent.ToString();

            isReady = true;
        }

        public void setIdForUndefined(string id) //todo internalize this, make private.
        {
            CalibrationIdString = id;
            xmlid = id;
        }

        private bool SanityCheck()
        {
            if (include == null && !xmlid.ContainsCI("base"))
            {
                Trace.TraceWarning(String.Format("Error in definition {0}, definition does not appear to be a BASE, but has no INCLUDE!!", this.xmlid));
                return false;
            }

            if (include == null && xmlid.ContainsCI("base"))
            {
                if (CalibrationIdString == null)
                    CalibrationIdString = xmlid.ToString();
            }

            return true;
        }

        public void ParseEcuFlashXml(XElement xRomId, string incl)
        {
            this.include = incl;

            if (xRomId.Element("memmodel") != null)
                setMemoryModel(xRomId.Element("memmodel").Value.ToString());
            else
                memoryModel = MemoryModels._Default;
            foreach (XElement element in xRomId.Elements())
            {
                string v = element.Value.ToString();
                if (v != null && v.Length > 0)
                {
                    switch (element.Name.ToString().ToLower())
                    {
                        case ECUIdTags.xmlid:
                            xmlid = v;
                            break;

                        case ECUIdTags.calidaddress:
                            setCalibrationIdAddressFromHex(v);
                            break;

                        case ECUIdTags.calidstring:
                            CalibrationIdString = v;
                            break;

                        case ECUIdTags.calidhex:
                            CalibrationIdHexString = v;
                            break;

                        case ECUIdTags.ecuidhex:
                            EcuIdHexString = v; //TODO: move these to memorymodels??? in subaru this is HEX, but this is inconsistent with calibrationid :(
                            break;

                        case ECUIdTags.caseid:
                            caseid = v;
                            break;

                        case ECUIdTags.checksummodule:
                            setChecksumModule(v);
                            break;

                        case ECUIdTags.flashmethod:
                            setFlashMethod(v);
                            break;

                        case ECUIdTags.include:
                            include = v;
                            break;

                        case ECUIdTags.year:
                            year = v;
                            break;

                        case ECUIdTags.market:
                            market = v;
                            break;

                        case ECUIdTags.make:
                            make = v;
                            break;

                        case ECUIdTags.model:
                            model = v;
                            break;

                        case ECUIdTags.submodel:
                            submodel = v;
                            break;

                        case ECUIdTags.transmission:
                            transmission = v;
                            break;

                        default:
                            break;
                    }
                }
            }
            isReady = SanityCheck();
        }

        public ECUIdentifier()
        {
            propertyBag = new Dictionary<string, string>();
            isReady = false;
        }
       
        /// <summary>
        /// Holds all XElements pulled from XML for ROM tables
        /// Includes inherited XML
        /// </summary>
        /// //todo fix kludge!!!!
        public XElement EcuFlashXml_SH705x {
            get
            {
                XElement x = new XElement("romid");
                foreach (KeyValuePair<string, string> prop in propertyBag)
                {
                    if(!prop.Key.ContainsCI("calidhex"))
                        x.Add(new XElement(prop.Key, prop.Value));
                }
                //if(xmlid != null)
                //    x.Add(new XElement("xmlid", xmlid));
                //if(caseid != null)
                //    x.Add(new XElement("caseid", caseid));
                //if (filesize != null)
                //    x.Add(new XElement("filesize", filesize));
                //if (memoryModel != null && memoryModel.name != null)
                //    x.Add(new XElement("memmodel", memoryModel.name));
                //if(flashMethod != null && flashMethod.name != null)
                //    x.Add(new XElement("flashmethod", flashMethod.name));
                //if(checksumModule!= null && checksumModule.Name != null)
                //    x.Add(new XElement("checksummodule", checksumModule.Name));
                //if(CalibrationIdAddress != null)
                //    x.Add(new XElement("internalidaddress", CalibrationIdAddress.ConvertLongToHexString()));
                //if(CalibrationIdString != null)
                //    x.Add(new XElement("internalidstring", CalibrationIdString));//TODO: conditionally output hex??
                //if(EcuIdHexString != null)
                //    x.Add(new XElement(ECUIdTags.ecuidstring, EcuIdHexString));
                //if(year != null)
                //    x.Add(new XElement("year", year));
                //if(market != null)
                //    x.Add(new XElement("market", market));
                //if(make != null)
                //    x.Add(new XElement("make", make));
                //if(model != null)
                //    x.Add(new XElement("model", model));
                //if(submodel != null)
                //    x.Add(new XElement("submodel", submodel));
                //if(transmission != null)
                //    x.Add(new XElement("transmission", transmission));
                return x;
            }            
        }


        public XElement ExportRRMetaData()
        {
            XElement xe = new XElement("rom");
            xe.SetAttributeValue("base",this.include);
            XElement xeromid = EcuFlashXml_SH705x;
            xe.Add(xeromid);
            return xe;
        }
    }
}
