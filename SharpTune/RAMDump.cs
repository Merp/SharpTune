///*
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//*/

//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.IO.Ports;
//using System.Text;
//using NateW.Ssm;
//using ConsoleRedirection;
//using DumpXML;
//using SharpTune;

//namespace RamMod
//{
//    class RAMDump
//    {
//        /// <summary>
//        /// This is a map of unused RAM bytes
//        /// TODO: Populate with full list, and remove if values read are nonzero
//        /// </summary>
//        private List<long> UnusedRam { get; set; }

//        //private const string fileNamePrefix = "EcuImage_";
//        private const string fileNamePrefix = "EcuImage_Ram_";

//        public List<EcuImageRange> ranges = new List<EcuImageRange>();
//        //{
//        //    //new EcuImageRange(0x000, 64 * 1024),
//        //    //new EcuImageRange(0x20000, 64 * 1024),
//        //    new EcuImageRange(0xFF0000, 0x00FF),
//        //};

        

//        public SsmInterface ecu;


//        public bool InitDump(string inputport, List<int> starts, List<int> lengths)
//        {
//            try
//            {
//                string portName = "";
                
//                portName = inputport;


//                Trace.WriteLine(String.Format("Opening serial port {0}.", portName));
//                SerialPort port = new SerialPort(portName, 4800, Parity.None, 8);
//                port.Open();
//                ecu = SsmInterface.GetInstance(port.BaseStream);
//                //MockEcuStream stream = MockEcuStream.CreateInstance();
//                //SsmInterface ecu = SsmInterface.GetInstance(stream);
//                Trace.WriteLine("opened.");

//                Console.Write("Getting ECU identifier... ");
//                IAsyncResult result = ecu.BeginGetEcuIdentifier(null, null);
//                result.AsyncWaitHandle.WaitOne();
//                ecu.EndGetEcuIdentifier(result);
//                Trace.WriteLine(ecu.EcuIdentifier);
//                return true;

              
                
//            }
//            catch (Exception ex)
//            {
//                Trace.WriteLine(ex.ToString());
//            }

//            return false;
//        }

//        //public void InitDump(string[] args)
//        //{
//        //    try
//        //    {
//        //        string portName = "";
//        //        bool test = false;

//        //        if (args.Length > 0)
//        //        {
//        //            if (args[0] == "/?" || args[0] == "-?")
//        //            {
//        //                Usage();
//        //                return;
//        //            }

//        //            portName = args[0];
//        //        }
//        //        else
//        //        {
//        //            Usage();
//        //        }

//        //        if (args.Length > 1)
//        //        {
//        //            if (args[1] == "-t")
//        //            {
//        //                test = true;
//        //            }
//        //            else
//        //            {
//        //                Usage();
//        //                return;
//        //            }
//        //        }

//        //        Trace.WriteLine(String.Format("Opening serial port {0}.", portName));
//        //        SerialPort port = new SerialPort(portName, 4800, Parity.None, 8);
//        //        port.Open();
//        //        ecu = SsmInterface.GetInstance(port.BaseStream);
//        //        //MockEcuStream stream = MockEcuStream.CreateInstance();
//        //        //SsmInterface ecu = SsmInterface.GetInstance(stream);
//        //        Trace.WriteLine("opened.");

//        //        Console.Write("Getting ECU identifier... ");
//        //        IAsyncResult result = ecu.BeginGetEcuIdentifier(null, null);
//        //        result.AsyncWaitHandle.WaitOne();
//        //        ecu.EndGetEcuIdentifier(result);
//        //        Trace.WriteLine(ecu.EcuIdentifier);

//        //        form1.dumpEnable();

//        //        if (test)
//        //        {
//        //            int rows = 7;
//        //            int bytesPerRow = 16;
//        //            int length = bytesPerRow * rows;
//        //            byte[] values = ecu.SyncReadBlock(0, length);
//        //            Trace.WriteLine(String.Format("Requested {0} bytes, received {1}", length, values.Length));

//        //            StringBuilder builder = new StringBuilder(100);
//        //            for (int row = 0; row < rows; row++)
//        //            {
//        //                for (int i = 0; i < bytesPerRow; i++)
//        //                {
//        //                    builder.Append("0x");
//        //                    builder.Append(values[i].ToString("X2"));
//        //                    builder.Append(',');

//        //                    if (i % 16 == 15)
//        //                    {
//        //                        Trace.WriteLine(builder.ToString());
//        //                        builder = new StringBuilder(100);
//        //                    }
//        //                }
//        //            }
//        //            /*
//        //            0x04,0x2F,0x12,0x78,0x52,0x06,0x00,0xFF,0x59,0x94,0x7A,0xFF,0xFF,0x21,0x14,0xC5,
//        //            0x00,0x99,0x3D,0x02,0x88,0x0D,0xFF,0xFF,0x00,0x15,0xFF,0xFF,0xB4,0x4A,0x26,0xFF,
//        //            0x07,0xFF,0x80,0x66,0x3C,0xFF,0x7C,0xFF,0xFF,0x00,0x3A,0xFF,0xFF,0xFF,0xFF,0xFF,
//        //            0x00,0xFF,0x00,0xD7,0xD4,0x00,0xFF,0xFF,0xFF,0xFF,0x00,0x55,0x32,0x32,0x18,0x18,
//        //            0x02,0x02,0x80,0xFF,0x1F,0xFF,0x7F,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
//        //            0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
//        //            0xAA,0x00,0xC8,0x00,0xD0,0x01,0x00,0x00,0x02,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
//        //             */
//        //            return;
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Trace.WriteLine(ex.ToString());
//        //    }
//        //}

//        //public void ReadDump()
//        //{
//        //    try
//        //    {
//        //        Trace.WriteLine("Reading data.  This will take a while.");
//        //        string name = fileNamePrefix + ecu.EcuIdentifier;
//        //        using (Stream fileStream = File.Create(name + ".cs"))
//        //        {
//        //            foreach (EcuImageRange range in ranges)
//        //            {
//        //                range.Read(ecu);
//        //            }

//        //            using (TextWriter writer = new StreamWriter(fileStream))
//        //            {
//        //                writer.WriteLine("namespace NateW.Ssm {");
//        //                writer.WriteLine("  public class " + name + " : EcuImage {");
//        //                writer.WriteLine("      public " + name + "() {");
//        //                foreach (EcuImageRange range in ranges)
//        //                {
//        //                    range.Write(writer);
//        //                }
//        //                writer.WriteLine("        }");
//        //                writer.WriteLine("    }");
//        //                writer.WriteLine("}");
//        //            }
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Trace.WriteLine(ex.ToString());
//        //    }
//        //}


//        public void ReadDump(List<EcuImageRange> rangesinput)
//        {
//            ranges = rangesinput;

//            try
//            {
                
//                Trace.WriteLine("Reading data.  This will take a while.");
//                string name = fileNamePrefix + ecu.EcuIdentifier;
//                using (Stream fileStream = File.Create(name + ".cs"))
//                {
//                    foreach (EcuImageRange range in ranges)
//                    {
//                        range.Read(ecu);
//                    }

//                    using (TextWriter writer = new StreamWriter(fileStream))
//                    {
//                        writer.WriteLine("namespace NateW.Ssm {");
//                        writer.WriteLine("  public class " + name + " : EcuImage {");
//                        writer.WriteLine("      public " + name + "() {");
//                        foreach (EcuImageRange range in ranges)
//                        {
//                            range.Write(writer);
//                        }
//                        writer.WriteLine("        }");
//                        writer.WriteLine("    }");
//                        writer.WriteLine("}");
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Trace.WriteLine(ex.ToString());
//            }
//        }


//        private static void Usage()
//        {
//            Trace.WriteLine("Usage: EcuDump.exe [COMX]");
//            Trace.WriteLine("Where COMX is COM1, or COM2, or whatever.");
//        }

//        internal void InitDump()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
