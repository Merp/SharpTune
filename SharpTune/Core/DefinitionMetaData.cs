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
    [Serializable]
    public class DefinitionMetaData
    {
        public bool isReady {get; private set;}
        public string xmlid { get; private set;}
        public string caseid { get; private set; }
        
        private string _filesize;
        public string filesize {
            get { 
                if(_filesize != null)
                    return _filesize;
                else
                    return memoryModel.filesizebytes.ToKiloBytesString();
            }
            private set { _filesize = value;}
        }

        public long CalibrationIdAddress {get; private set;}
        private void setCalibrationIdAddressFromHex(string hex){
            CalibrationIdAddress = hex.ConvertHexToLong();
        }

        private string _calibrationIdHexString;
        private string _calibrationIdString;
        private Byte[] _calibrationIdHexBytes;

        public string CalibrationIdString {
            get{
                return _calibrationIdString;}
             private set{
                 _calibrationIdString = value;
                 _calibrationIdHexString = value.ConvertStringToHex(memoryModel.encoding);
                 _calibrationIdHexBytes = value.ConvertStringToBytes(memoryModel.encoding);
             }
        }

        public string CalibrationIdHexString {
            get{
                return _calibrationIdHexString;
            }
            private set{
                _calibrationIdHexString = value;
                _calibrationIdString = value.ConvertHexToString(memoryModel.encoding);
                _calibrationIdHexBytes = value.ToByteArray();
            }
        }

        public Byte[] CalibrationIdHexBytes {
            get{
                return _calibrationIdHexBytes;
            }
            private set{
                _calibrationIdHexBytes = value;
                _calibrationIdHexString = value.ConvertBytesToHexString();
                _calibrationIdString = value.ConvertBytesToString(memoryModel.encoding);
            }
        }

        private string _ecuIdHexString;
        private string _ecuIdString;
        private Byte[] _ecuIdHexBytes;

        public string EcuIdString {
            get{
                return _ecuIdString;}
             private set{
                 _ecuIdString = value;
                 _ecuIdHexString = value.ConvertStringToHex(memoryModel.encoding);
                 _ecuIdHexBytes = value.ConvertStringToBytes(memoryModel.encoding);
             }
        }

        public string EcuIdHexString {
            get{
                return _ecuIdHexString;
            }
            private set{
                _ecuIdHexString = value;
                _ecuIdString = value.ConvertHexToString(memoryModel.encoding);
                _ecuIdHexBytes = value.ToByteArray();
            }
        }

        public Byte[] EcuIdHexBytes {
            get{
                return _ecuIdHexBytes;
            }
            private set{
                _ecuIdHexBytes = value;
                _ecuIdHexString = value.ConvertBytesToHexString();
                _ecuIdString = value.ConvertBytesToString(memoryModel.encoding);
            }
        }

        public string year {get; private set;}
        public string market {get; private set;}
        public string make {get; private set;}
        public string model {get; private set;}
        public string submodel {get; private set;}
        public string transmission {get; private set;}

        public string include {get; set;} //todo make private and internalize operations for undefined roms.
   
        public IMemoryModel memoryModel {get; private set;}
        private void setMemoryModel(string mm){
            memoryModel = MemoryModels.GetMemoryModel(mm);
        }
   
        public IFlashMethod flashMethod {get; private set;}
        private void setFlashMethod(string fm){
            flashMethod = FlashMethods.GetFlashMethod(fm);
        }

        public IChecksumModule checksumModule {get; private set;}
        private void setChecksumModule(string csm){
            checksumModule = ChecksumModules.GetChecksumModule(csm);
        }

        public void UpdateFromMod(RomMod.Mod mod)
        {
            include = mod.InitialCalibrationId;
            CalibrationIdAddress = mod.ModIdentAddress;
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
                    switch (element.Name.ToString())
                    {
                        case "xmlid":
                            xmlid = v;
                            break;

                        case "internalidaddress":
                            setCalibrationIdAddressFromHex(v);
                            break;

                        case "internalidstring":
                            CalibrationIdString = v;
                            break;

                        case "internalidhex":
                            CalibrationIdHexString = v;
                            break;

                        case "ecuid":
                            EcuIdHexString = v; //TODO: move these to memorymodels??? in subaru this is HEX, but this is inconsistent with calibrationid :(
                            break;

                        case "caseid":
                            caseid = v;
                            break;

                        case "checksummodule":
                            setChecksumModule(v);
                            break;

                        case "flashmethod":
                            setFlashMethod(v);
                            break;

                        case "include":
                            include = v;
                            break;

                        case "year":
                            year = v;
                            break;

                        case "market":
                            market = v;
                            break;

                        case "make":
                            make = v;
                            break;

                        case "model":
                            model = v;
                            break;

                        case "submodel":
                            submodel = v;
                            break;

                        case "transmission":
                            transmission = v;
                            break;

                        default:
                            break;
                    }
                }
            }
            isReady = SanityCheck();
        }

        public DefinitionMetaData()
        {
            isReady = false;
        }

       
        /// <summary>
        /// Holds all XElements pulled from XML for ROM tables
        /// Includes inherited XML
        /// </summary>
        public XElement EcuFlashXml {
            get
            {
                XElement x = new XElement("romid");
                if(xmlid != null)
                    x.Add(new XElement("xmlid", xmlid));
                if(caseid != null)
                    x.Add(new XElement("caseid", caseid));
                if (filesize != null)
                    x.Add(new XElement("filesize", filesize));
                if (memoryModel != null && memoryModel.name != null)
                    x.Add(new XElement("memmodel", memoryModel.name));
                if(flashMethod != null && flashMethod.name != null)
                    x.Add(new XElement("flashmethod", flashMethod.name));
                if(checksumModule!= null && checksumModule.Name != null)
                    x.Add(new XElement("checksummodule", checksumModule.Name));
                if(CalibrationIdAddress != null)
                    x.Add(new XElement("internalidaddress", CalibrationIdAddress.ConvertLongToHexString()));
                if(CalibrationIdString != null)
                    x.Add(new XElement("internalidstring", CalibrationIdString));//TODO: conditionally output hex??
                if(EcuIdHexString != null)
                    x.Add(new XElement("ecuid", EcuIdHexString));
                if(year != null)
                    x.Add(new XElement("year", year));
                if(market != null)
                    x.Add(new XElement("market", market));
                if(make != null)
                    x.Add(new XElement("make", make));
                if(model != null)
                    x.Add(new XElement("model", model));
                if(submodel != null)
                    x.Add(new XElement("submodel", submodel));
                if(transmission != null)
                    x.Add(new XElement("transmission", transmission));
                return x;
            }            
        }


        public XElement ExportRRMetaData()
        {
            XElement xe = new XElement("rom");
            xe.SetAttributeValue("base",this.include);
            XElement xeromid = EcuFlashXml;
            xe.Add(xeromid);
            return xe;
        }
    }
}
