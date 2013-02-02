/*
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using EB.Math;
using System.Collections;
using System.Globalization;
using Merp;
using System.Windows.Forms;

namespace SharpTuneCore
{
    public class ScalingFactory
    {
        public static Scaling CreateScaling(XElement xel)
        {
            switch (xel.Attribute("storagetype").Value.ToString())
            {
                case "bloblist":
                    return new BlobScaling(xel);
                case "float":
                    return new FloatScaling(xel);
                case "uint8":
                    return new Uint8Scaling(xel);
                case "uint16":
                    return new Uint16Scaling(xel);
                case "uint32":
                    return new Uint32Scaling(xel);
                case "int8":
                    return new Int8Scaling(xel);
                case "int16":
                    return new Int16Scaling(xel);
                case "int32":
                    return new Int32Scaling(xel);
                default:
                    return null;

            }
        }
    }

    public class Scaling
    {
        public string name { get; set; }
        public string units { get; private set; }
        public string toexpr { get; private set; }
        public string frexpr { get; private set; }
        public string format { get; private set; }
        public Single min { get; private set; }
        public Single max { get; private set; }
        public Single inc { get; private set; }
        public string endian { get; set; }
        public int storageSize { get; set; }
        public bool signed { get; set; }
        public Dictionary<string, string> properties { get; set; }
        public Function readFunction { get; set; }
        public Function writeFunction { get; set; }


        public Scaling()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Scaling(XElement xel)
        {
            this.properties = new Dictionary<string, string>();

            foreach (XAttribute attribute in xel.Attributes())
            {
                this.properties.Add(attribute.Name.ToString(), attribute.Value.ToString());
            }

            this.name = this.properties["name"];
            this.signed = this.properties["storagetype"].Contains("u");

            foreach (KeyValuePair<string, string> property in this.properties)
            {
                switch (property.Key.ToString())
                {
                    case "name":
                        this.name = property.Value.ToString();
                        continue;

                    case "endian":
                        this.endian = property.Value.ToString();
                        continue;

                    case "units":
                        this.units = property.Value.ToString();
                        continue;

                    case "toexpr":
                        this.toexpr = property.Value.ToString();
                        continue;

                    case "frexpr":
                        this.frexpr = property.Value.ToString();
                        continue;

                    case "format":
                        this.format = property.Value.ToString();
                        continue;

                    case "min":
                        this.min = System.Single.Parse(property.Value.ToString(), System.Globalization.NumberStyles.Float);
                        continue;

                    case "max":
                        this.max = System.Single.Parse(property.Value.ToString(), System.Globalization.NumberStyles.Float);
                        continue;

                    case "inc":
                        this.inc = System.Single.Parse(property.Value.ToString(), System.Globalization.NumberStyles.Float);
                        continue;

                    default:
                        continue;
                }

            }

            if (this.frexpr != null)
            {

            }
        }

        public virtual string toDisplay(byte[] bytes) { return ""; }

        public virtual byte[] fromDisplay(string str) { return null; }

        public void Convert(double value, out string valueAsString, out double valueAsDouble, bool read)
        {
            Function function = new Function();
            if (read)
            {
                if (this.readFunction == null)
                {
                    this.readFunction = new Function();
                    //TODO: Do inverse function in code
                    this.readFunction.Parse(this.toexpr);
                    readFunction.Infix2Postfix();
                }
                function = this.readFunction;
            }
            else
            {
                if (this.writeFunction == null)
                {
                    this.writeFunction = new Function();
                    writeFunction.Parse(this.frexpr);
                    writeFunction.Infix2Postfix();
                }
                function = this.writeFunction;

            }




            ArrayList var = function.Variables;
            Symbol symbol = (Symbol)var[0];
            symbol.m_value = value;
            var[0] = symbol;
            function.Variables = var;
            function.EvaluatePostfix();
            valueAsDouble = function.Result;

            //if (this.units == Conversion.Boolean)
            //{
            //    if (valueAsDouble == 0)
            //    {
            //        valueAsString = Conversion.BooleanFalse;
            //    }
            //    else
            //    {
            //        valueAsString = Conversion.BooleanTrue;
            //        valueAsDouble = 1;
            //    }
            //}
            //else
            //{
            //this.format = "{0:" + format + "}";

            //TODO:FIX FORMATTING
            valueAsString = string.Format(CultureInfo.InvariantCulture, "{0:0.0000}", valueAsDouble);
            //}
        }

    }

    public sealed class FloatScaling : Scaling
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FloatScaling(XElement xel)
            : base(xel)
        {
            this.signed = false;
            this.storageSize = 4;
        }

        public override string toDisplay(byte[] bytes)
        {
            string dstring;
            double ddouble;
            Convert((double)BitConverter.ToSingle(bytes, 0), out dstring, out ddouble, true);
            return String.Format("{0:0.00000000000}", dstring);
        }

        public override byte[] fromDisplay(string str)
        {
            string dstring;
            double ddouble;
            Convert((double)Double.Parse(str), out dstring, out ddouble, false);
            return BitConverter.GetBytes((Single)ddouble);
        }

    }

    public sealed class Uint8Scaling : Scaling
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Uint8Scaling(XElement xel)
            : base(xel)
        {
            this.signed = false;
            this.storageSize = 1;
        }

        public override string toDisplay(byte[] bytes)
        {
            string dstring;
            double ddouble;
            Convert((double)(sbyte)bytes[0], out dstring, out ddouble, true);
            return String.Format("{0:0.0000}", dstring);
        }

        public override byte[] fromDisplay(string str)
        {
            string dstring;
            double ddouble;
            Convert((double)Double.Parse(str), out dstring, out ddouble, false);
            return BitConverter.GetBytes((byte)ddouble);
        }

    }

    public sealed class Int8Scaling : Scaling
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Int8Scaling(XElement xel)
            : base(xel)
        {
            this.signed = true;
            this.storageSize = 1;
        }

        public override string toDisplay(byte[] bytes)
        {
            string dstring;
            double ddouble;
            Convert((double)bytes[0], out dstring, out ddouble, true);
            return String.Format("{0:0.0000}", dstring);
        }

        public override byte[] fromDisplay(string str)
        {
            string dstring;
            double ddouble;
            Convert((double)Double.Parse(str), out dstring, out ddouble, false);
            return BitConverter.GetBytes((sbyte)ddouble);
        }

    }

    public sealed class Uint16Scaling : Scaling
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Uint16Scaling(XElement xel)
            : base(xel)
        {
            this.signed = false;
            this.storageSize = 2;
        }

        public override string toDisplay(byte[] bytes)
        {
            string dstring;
            double ddouble;
            Convert((double)BitConverter.ToUInt16(bytes, 0), out dstring, out ddouble, true);
            return String.Format("{0:0.0000}", dstring);
        }

        public override byte[] fromDisplay(string str)
        {
            string dstring;
            double ddouble;
            double strd = Double.Parse(str);
            Convert(strd, out dstring, out ddouble, false);
            UInt16 uuuu = (UInt16)ddouble;
            return BitConverter.GetBytes((UInt16)ddouble);
        }

    }

    public sealed class Int16Scaling : Scaling
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Int16Scaling(XElement xel)
            : base(xel)
        {
            this.signed = true;
            this.storageSize = 2;
        }

        public override string toDisplay(byte[] bytes)
        {
            string dstring;
            double ddouble;
            Convert((double)BitConverter.ToInt16(bytes, 0), out dstring, out ddouble, true);
            return String.Format("{0:0.0000}", dstring);
        }

        public override byte[] fromDisplay(string str)
        {
            string dstring;
            double ddouble;
            Convert((double)Double.Parse(str), out dstring, out ddouble, false);
            return BitConverter.GetBytes((Int16)ddouble);
        }

    }

    public sealed class Uint32Scaling : Scaling
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Uint32Scaling(XElement xel)
            : base(xel)
        {
            this.signed = false;
            this.storageSize = 4;
        }

        public override string toDisplay(byte[] bytes)
        {
            string dstring;
            double ddouble;
            Convert((double)BitConverter.ToUInt32(bytes, 0), out dstring, out ddouble, true);
            return String.Format("{0:0.0000}", dstring);
        }

        public override byte[] fromDisplay(string str)
        {
            string dstring;
            double ddouble;
            Convert((double)Double.Parse(str), out dstring, out ddouble, false);
            return BitConverter.GetBytes((UInt32)ddouble);
        }

    }

    public sealed class Int32Scaling : Scaling
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Int32Scaling(XElement xel)
            : base(xel)
        {
            this.signed = true;
            this.storageSize = 4;
        }

        public override string toDisplay(byte[] bytes)
        {
            string dstring;
            double ddouble;
            Convert((double)BitConverter.ToInt32(bytes, 0), out dstring, out ddouble, true);
            return String.Format("{0:0.0000}", dstring);
        }

        public override byte[] fromDisplay(string str)
        {
            string dstring;
            double ddouble;
            Convert((double)Double.Parse(str), out dstring, out ddouble, false);
            return BitConverter.GetBytes((Int32)ddouble);
        }

    }

    public sealed class BlobScaling : Scaling
    {
        //convert this to BLOBS, not int!
        public Dictionary<string, byte[]> validBlobs { get; private set; }
        public bool locked { get; private set; }
        public int blobLength { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public BlobScaling(XElement xel)
            : base(xel)
        {
            this.validBlobs = new Dictionary<string, byte[]>();
            this.locked = false;

            foreach (XElement child in xel.Elements())
            {
                string name = child.Name.ToString();

                switch (name)
                {
                    case "data":
                        //Will also have to convert this to blobs
                        this.validBlobs.Add(child.Attribute("name").Value.ToString(), child.Attribute("value").Value.ToString().ToByteArray());
                        this.blobLength = child.Attribute("value").Value.ToString().ToByteArray().Length;
                        break;

                    default:
                        Console.WriteLine("Unknown child encountered in Scaling {0}", xel.Attribute("name").Value.ToString());
                        break;
                }
            }

            foreach (KeyValuePair<string, byte[]> blob in this.validBlobs)
            {
                if (blob.Value.Length != this.blobLength)
                {
                    MessageBox.Show("Error parsing bloblist XML!!!");
                }
            }

            this.storageSize = this.blobLength;
        }

        public override string toDisplay(byte[] bytes)
        {
            foreach (KeyValuePair<string, byte[]> blob in this.validBlobs)
            {
                if (bytes.SequenceEqual(blob.Value))
                {
                    return blob.Key.ToString();
                }
            }
            this.locked = true;
            return "No Matches Found!!";
        }

        public override byte[] fromDisplay(string str)
        {
            foreach (KeyValuePair<string, byte[]> blob in this.validBlobs)
            {
                if (str == blob.Key)
                {
                    return blob.Value;
                }
            }
            return null;
        }

    }





    static class Extensions
    {

        public static Scaling PopulateProperties(this Scaling scaling)
        {
            foreach (KeyValuePair<string, string> property in scaling.properties)
            {
                switch (property.Key.ToString())
                {
                    case "name":
                        continue;

                    case "storagetype":
                        continue;

                    default:
                        continue;
                }

            }
            return scaling;
        }

        public static Scaling AddBase(this Scaling child, Scaling baseScaling)
        {
            foreach (KeyValuePair<string, string> property in baseScaling.properties)
            {
                //If property doesn't exist in the child, add it from the base!
                if (!child.properties.ContainsKey(property.Key))
                {
                    child.properties.Add(property.Key, property.Value);
                }
            }
            return child;
        }
    }


}
