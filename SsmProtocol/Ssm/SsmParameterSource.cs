///////////////////////////////////////////////////////////////////////////////
// Copyright (c) Nate Waddoups
// SsmParameterSource.cs
///////////////////////////////////////////////////////////////////////////////
//#define XPath
//#define XmlReader

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;

namespace NateW.Ssm
{
#if PocketPC

    public class InvalidDataException : Exception
    {
        public InvalidDataException(string message)
            : base(message)
        {
        }
    }

#endif

    /// <summary>
    /// Interprets the logger definition XML
    /// </summary>
    public class SsmParameterSource : ParameterSource
    {
        private static PropertyDefinition DatabaseEcuIdentifier = new PropertyDefinition("EcuIdentifier");

        /// <summary>
        /// Simple predicate to indicate when to stop advancing the XmlReader (see ReadUntil method below)
        /// </summary>
        private delegate bool SimplePredicate();
        
        /// <summary>
        /// Indicates which 'standard' parameters are supported by the current ECU
        /// </summary>
        private IList<byte> compatibilityMap;

        /// <summary>
        /// The ECU that this database was built with
        /// </summary>
        public string EcuIdentifier
        {
            [DebuggerStepThrough()]
            get { return this.PropertyBag[DatabaseEcuIdentifier] as string; }
        }
        
        /// <summary>
        /// Private constructor - use factory instead
        /// </summary>
        private SsmParameterSource(string directory, string ecuIdentifier, IList<byte> compatibilityMap) : base ("SSM")
        {
            this.PropertyBag[DatabaseEcuIdentifier] = ecuIdentifier;
            this.compatibilityMap = compatibilityMap;
            this.Load(directory);
        }

        /// <summary>
        /// Internal constructor, for test use only
        /// </summary>
        /// <remarks>
        /// This one does NOT parse the XML, so the different parsing methods
        /// can be tested independently.
        /// </remarks>
        internal SsmParameterSource(string ecuIdentifier, IList<byte> compatibilityMap) : base("SSM")
        {
            this.PropertyBag[DatabaseEcuIdentifier] = ecuIdentifier;
            this.compatibilityMap = compatibilityMap;
        }

        /// <summary>
        /// Factory
        /// </summary>
        public static SsmParameterSource GetInstance(string directory, string ecuIdentifier, IList<byte> compatibilityMap)
        {
            return new SsmParameterSource(directory, ecuIdentifier, compatibilityMap);
        }

        /// <summary>
        /// For debugging only, do not display in UI
        /// </summary>
        public override string ToString()
        {
            return this.PropertyBag.ToString() + " (" + this.Parameters.Count + " parameters)";
        }

        /// <summary>
        /// Load parameters from the logger definition XML
        /// </summary>
        private void Load(string directory)
        {
            string file = Path.Combine(directory, "logger.xml");
            using (Stream inputStream = File.OpenRead(file))
            {
#if !PocketPC
                string currentDirectory = Environment.CurrentDirectory;
                XPathDocument document;
                try
                {
                    Environment.CurrentDirectory = directory;
                    document = CreateDocument(inputStream);
                }
                finally
                {
                    Environment.CurrentDirectory = currentDirectory;
                }

                this.LoadStandardParameters(document);
                this.LoadExtendedParameters(document);
                this.LoadSwitches(document);
#else
                XmlReader reader = XmlReader.Create(inputStream);
                this.LoadStandardParameters(reader);

                // TODO: make this stream rewind unnecessary
                inputStream.Position = 0;
                reader = XmlReader.Create(inputStream);

                this.LoadExtendedParameters(reader);
#endif
            }
        }

#if !PocketPC

        #region XPath parsing methods

        /// <summary>
        /// Load the standard parameters
        /// </summary>
        internal void LoadStandardParameters(XPathDocument document)
        {
            XPathNavigator navigator = document.CreateNavigator();
            foreach (XPathNavigator node in navigator.Select(
                "/logger/protocols/protocol/parameters/parameter"))
            {
                string id = node.GetAttribute("id", string.Empty);
                string name = node.GetAttribute("name", string.Empty);

                int byteIndex = SsmParameterSource.GetIndexFromAttribute(node, "ecubyteindex");
                int bitIndex = SsmParameterSource.GetIndexFromAttribute(node, "ecubit");

                XPathNodeIterator children = node.Select("address");
                children.MoveNext();
                int address = SsmParameterSource.GetMemoryAddress(children.Current);
                int length = SsmParameterSource.GetMemoryLength(children.Current);

                children.MoveNext();
                children = node.Select("conversions/conversion");
                List<Conversion> conversions = SsmParameterSource.GetConversions(children);

                children = node.Select("depends/ref");
                ReadOnlyCollection<Parameter> dependencies = this.GetDependencies(children);

                if (this.EcuSupports(byteIndex, bitIndex))
                {
                    SsmParameter parameter = new SsmParameter(
                        this,
                        id,
                        name,
                        address,
                        length,
                        conversions.AsReadOnly(),
                        byteIndex,
                        bitIndex,
                        dependencies);

                    this.AddParameter(parameter);
                }
                else
                {
                    Trace.WriteLine("Skipping parameter " + id + " / " + name);
                }
            }
        }

        private bool EcuSupports(int byteIndex, int bitIndex)
        {
            // Calculated parameters have byteIndex 0
            if (byteIndex == 0)
            {
                return true;
            }

            // All other parameters have byteIndex of 8 or greater
            byteIndex -= 8;
            if (byteIndex < 0)
            {
                return false;
            }
            if (byteIndex >= this.compatibilityMap.Count)
            {
                return false;
            }

            byte b = this.compatibilityMap[byteIndex];
            int mask = 1 << bitIndex;
            if ((b & mask) == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Load the extended parameters 
        /// </summary>
        internal void LoadExtendedParameters(XPathDocument document)
        {   
            XPathNavigator navigator = document.CreateNavigator();
            foreach (XPathNavigator node in navigator.Select(
                "/logger/protocols/protocol/ecuparams/ecuparam/ecu[@id='" + this.EcuIdentifier + "']"))
            {
                node.MoveToFirstChild();
                int address = SsmParameterSource.GetMemoryAddress(node);
                int length = SsmParameterSource.GetMemoryLength(node);
                node.MoveToParent();
                node.MoveToParent();

                string name = node.GetAttribute("name", "");
                string id = node.GetAttribute("id", "");

                XPathNodeIterator iterator = node.Select("conversions/conversion");
                List<Conversion> conversions = SsmParameterSource.GetConversions(iterator);

                SsmParameter parameter = new SsmParameter(
                    this,
                    id,
                    name,
                    address,
                    length,
                    conversions.AsReadOnly());

                // TODO: remove this when the duplicates in logger.xml are cleaned up
                if (!this.Parameters.Contains(parameter))
                {
                    this.AddParameter(parameter);
                }
            }
        }

        /// <summary>
        /// Use XPathDocument to load switches
        /// </summary>
        internal void LoadSwitches(XPathDocument document)
        {
            XPathNavigator navigator = document.CreateNavigator();
            foreach (XPathNavigator node in navigator.Select(
                "/logger/protocols/protocol/switches/switch"))
            {
                string id = node.GetAttribute("id", string.Empty);
                string name = node.GetAttribute("name", string.Empty);
                string addressString = node.GetAttribute("byte", string.Empty);
                string bitString = node.GetAttribute("bit", string.Empty);

                int address = SsmParameterSource.GetAddressFromString(addressString);
                //int bit = (int) SsmParameterSource.GetAddressFromString(bitString);

                string expression = "x&(2^" + bitString + ")";
                Conversion conversion = Conversion.GetInstance(Conversion.Boolean, expression, "");

                /*
                SsmSwitch s = SsmSwitch.GetInstance(
                    id,
                    name,
                    address,
                    bit);
                this.switches.Add(s);
                 * */
                SsmParameter parameter = new SsmParameter(
                    this,
                    id,
                    name,
                    address,
                    1,
                    new List<Conversion>(new Conversion[] { conversion }).AsReadOnly());

                this.AddParameter(parameter);
            }
        }

        #endregion
#endif

        #region XmlReader parsing methods

        /// <summary>
        /// Load standard parameters with the XmlReader method
        /// </summary>
        /// <remarks>
        /// TODO: stop at the end of the parameters element, so the stream rewind won't be necessary
        /// </remarks>
        /// <exception cref="IOException">Unable to read from source stream</exception>
        /// <exception cref="InvalidDataException">Source document is corrupt</exception>
        internal void LoadStandardParameters(XmlReader reader)
        {
            while (!reader.EOF)
            {
                if (!reader.ReadToFollowing("parameter"))
                {
                    return;
                }
                
                //string elementName = reader.LocalName;
                string id = reader.GetAttribute("id");
                string name = reader.GetAttribute("name");
                string byteString = reader.GetAttribute("ecubyteindex");
                string bitString = reader.GetAttribute("ecubit");
                int byteIndex = GetInteger(byteString, 0);
                int bitIndex = GetInteger(bitString, 0);
                int address = 0;
                int length = 0;
                
                ReadOnlyCollection<Parameter> dependencies = null;

                reader.Read();
                SsmParameterSource.ReadUntil(
                    reader,
                    delegate
                    {
                        return reader.IsStartElement();
                    },
                    "Expected address element after parameter element");

                if (reader.IsStartElement("address"))
                {
                    SsmParameterSource.GetAddress(reader, ref address, ref length);
                }
                else if (reader.IsStartElement("depends"))
                {
                    List<Parameter> dependencyList = new List<Parameter>();
                    if (reader.ReadToDescendant("ref"))
                    {
                        while (reader.IsStartElement("ref"))
                        {
                            string dependencyId = reader.GetAttribute("parameter");
                            foreach (Parameter candidate in this.Parameters)
                            {
                                if (candidate.Id == dependencyId)
                                {
                                    dependencyList.Add(candidate);
                                    break;
                                }
                            }

                            if (!reader.ReadToNextSibling("ref"))
                            {
                                break;
                            }
                        }
                        if (dependencyList.Count > 0)
                        {
                            dependencies = dependencyList.AsReadOnly();
                        }
                        else
                        {
                            dependencies = null;
                        }
                    }
                    else
                    {
                        throw new InvalidDataException("XML schema doesn't match expectations. Parameter \"" + name + "\" has depends but not ref");
                    }
                }
                else
                {
                    throw new InvalidDataException("XML schema doesn't match expectations. Parameter \"" + name + "\" not followed by address or depends");
                }

                if (!reader.ReadToFollowing("conversion"))
                {
                    throw new InvalidDataException("XML schema doesn't match expectations (expected conversions node after address node, found " + reader.Name + ")");
                }

                List<Conversion> conversions = new List<Conversion>();
                SsmParameterSource.GetConversions(reader, conversions);

                SsmParameter parameter = new SsmParameter(
                    this,
                    id, 
                    name, 
                    address, 
                    length, 
                    conversions.AsReadOnly(), 
                    byteIndex, 
                    bitIndex, 
                    dependencies.Count == 0 ? null : dependencies);
                this.AddParameter(parameter);
                reader.Read();
            }
        }

        /// <summary>
        /// Use XmlReader to load extended parameters
        /// </summary>
        /// <exception cref="IOException">Unable to read from source stream</exception>
        /// <exception cref="InvalidDataException">Source document is corrupt</exception>
        internal void LoadExtendedParameters(XmlReader reader)
        {
            while (!reader.EOF)
            {
                if (!reader.ReadToFollowing("ecuparam"))
                {
                    return;
                }

                string id = reader.GetAttribute("id");
                string name = reader.GetAttribute("name");

                int depth = reader.Depth;
                reader.Read();
                for (; ; )
                {
                    if (reader.EOF)
                    {
                        return;
                    }
                    if (reader.Depth < depth)
                    {
                        break;
                    }
                    if (reader.IsStartElement("ecu"))
                    {
                        depth = reader.Depth;
                        string ecuId = reader.GetAttribute("id");
                        int address = 0;
                        int length = 0;

                        if (ecuId == this.EcuIdentifier)
                        {
                            reader.Read();
                            SsmParameterSource.ReadUntil(
                                reader,
                                delegate
                                {
                                    return reader.IsStartElement();
                                },
                                "Expected address element after ecu element");

                            SsmParameterSource.GetAddress(reader, ref address, ref length);
                            
                            if (!reader.ReadToFollowing("conversion"))
                            {
                                throw new InvalidDataException("Invalid XML: expected conversion element after ecuparam element");
                            }

                            List<Conversion> conversions = new List<Conversion>();
                            SsmParameterSource.GetConversions(reader, conversions);

                            SsmParameter parameter = new SsmParameter(
                                this,
                                id, 
                                name, 
                                address, 
                                length, 
                                conversions.AsReadOnly());
                            this.AddParameter(parameter);
                            break;
                        }
                    }
                    reader.Read();
                }
            }
        }

        /// <summary>
        /// Use XmlReader to load switches
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "reader")]
        internal void LoadSwitches(XmlReader reader)
        {
            throw new NotImplementedException("SsmParameterSource.LoadSwitches(XmlReader)");
        }

        #endregion

#if !PocketPC
        #region XPath helper methods

        /// <summary>
        /// Create an XPathDocument from the given stream
        /// </summary>
        internal static XPathDocument CreateDocument(Stream inputStream)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            //settings.ProhibitDtd = false;
            settings.DtdProcessing = DtdProcessing.Prohibit;

            //XmlParserContext context = new XmlParserContext(
            XmlReader reader = XmlReader.Create(inputStream, settings);
            System.Xml.XPath.XPathDocument document = new XPathDocument(reader);
            return document;
        }

        /// <summary>
        /// Get the current parameter's address 
        /// </summary>
        private static int GetMemoryAddress(XPathNavigator node)
        {
            string rawValue = node.Value;
            int address = GetAddressFromString(rawValue);
            return address;
        }

        /// <summary>
        /// Get an address from a string
        /// </summary>
        private static int GetAddressFromString(string addressString)
        {
            int address = 0;
            if (!string.IsNullOrEmpty(addressString))
            {
                try
                {
                    if (addressString.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        address = int.Parse(addressString.Substring(2), System.Globalization.NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        address = int.Parse(addressString, CultureInfo.InvariantCulture);
                    }
                }
                catch (FormatException)
                {
                }
            }
            return address;
        }

        /// <summary>
        /// Get the byte index into the ECU's capability vector
        /// </summary>
        private static int GetIndexFromAttribute(XPathNavigator node, string attribute)
        {
            string rawValue = node.GetAttribute(attribute, string.Empty);
            int byteIndex = 0;
            try
            {
                if (!string.IsNullOrEmpty(rawValue))
                {
                    byteIndex = Convert.ToInt32(rawValue, CultureInfo.InvariantCulture);
                }
            }
            catch (FormatException)
            {
                Debug.WriteLine("SsmParameterSource: GetIndexFromNodeAttribute: FormatException: parameter: " + attribute + ", byteIndex: " + rawValue);
            }

            return byteIndex;
        }

        /// <summary>
        /// Get the conversions for the current parameter
        /// </summary>
        private static List<Conversion> GetConversions(XPathNodeIterator iterator)
        {
            List<Conversion> result = new List<Conversion>();
            foreach (XPathNavigator node in iterator)
            {
                string units = node.GetAttribute("units", "");
                string expression = node.GetAttribute("expr", "");
                string format = node.GetAttribute("format", "");
                Conversion conversion = Conversion.GetInstance(units, expression, format);
                result.Add(conversion);
            }
            return result;
        }

        /// <summary>
        /// Get the dependencies for the current parameter
        /// </summary>
        private ReadOnlyCollection<Parameter> GetDependencies(XPathNodeIterator iterator)
        {
            List<Parameter> dependencies = new List<Parameter>();
            foreach (XPathNavigator node in iterator)
            {
                // TODO: consider storing parameters in a dictionary indexed by Id
                string id = node.GetAttribute("parameter", "");
                foreach (Parameter candidate in this.Parameters)
                {
                    if (candidate.Id == id)
                    {
                        dependencies.Add(candidate);
                        break;
                    }
                }
            }
            return dependencies.Count == 0 ? null : dependencies.AsReadOnly();
        }

        /// <summary>
        /// Get the number of memory bytes to read for the current parameter
        /// </summary>
        private static int GetMemoryLength(XPathNavigator navigator)
        {
            string lengthString = navigator.GetAttribute("length", "");
            int length = 1;
            if (!string.IsNullOrEmpty(lengthString))
            {
                length = Convert.ToInt32(lengthString, CultureInfo.InvariantCulture);
            }
            return length;
        }

        #endregion
#endif
        #region XmlReader helper methods

        /// <summary>
        /// Get the address & length from an address element
        /// </summary>
        /// <exception cref="IOException">Unable to read from source stream</exception>
        /// <exception cref="InvalidDataException">Source document is corrupt</exception>
        private static void GetAddress(XmlReader reader, ref int address, ref int length)
        {
            reader.Read();
            string addressString = reader.Value;
            address = SsmParameterSource.GetAddress(addressString);
            string lengthString = reader.GetAttribute("length");
            length = GetInteger(lengthString, 1);

            int depth = reader.Depth;

            SsmParameterSource.ReadUntil(
                reader,
                delegate
                {
                    return reader.Depth >= depth;
                },
                "Did not expect EOF after address element");
        }

        /// <summary>
        /// Get the conversions for a parameters
        /// </summary>
        /// <exception cref="IOException">Unable to read from source stream</exception>
        private static void GetConversions(XmlReader reader, List<Conversion> conversions)
        {
            while (reader.IsStartElement("conversion"))
            {
                string units = reader.GetAttribute("units");
                string expression = reader.GetAttribute("expr");
                string format = reader.GetAttribute("format");
                Conversion conversion = Conversion.GetInstance(units, expression, format);
                conversions.Add(conversion);
                if (!reader.ReadToNextSibling("conversion"))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Advance the XmlReader until the given predicate is satisfied
        /// </summary>
        /// <exception cref="IOException">Unable to read from source stream</exception>
        /// <exception cref="InvalidDataException">Source document is corrupt</exception>
        private static void ReadUntil(XmlReader reader, SimplePredicate predicate, string eofMessage)
        {
            while (!predicate())
            {
                if (reader.EOF)
                {
                    throw new InvalidDataException("Invalid XML: " + eofMessage);
                }
                reader.Read();
            }
        }

        /// <summary>
        /// Get an integer from a string
        /// </summary>
        private static int GetInteger(string s, int defaultValue)
        {
            if (string.IsNullOrEmpty(s))
            {
                return defaultValue;
            }

#if PocketPC
            try
            {
                return int.Parse(s);
            }
            catch (FormatException)
            {
            }
#else
            int result;
            if (int.TryParse(s, out result))
            {
                return result;
            }
#endif
            return defaultValue;
        }

        /// <summary>
        /// Get an int from a string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static int GetAddress(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return 0;
            }

#if PocketPC
            try
            {
                int.Parse(s.Substring(2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            catch (FormatException)
            {
            }
            return 0;
#else
            int result;
            if (int.TryParse(s.Substring(2), System.Globalization.NumberStyles.AllowHexSpecifier, null, out result))
            {
                return result;
            }
            return 0;
#endif
        }
        #endregion
    }
}
