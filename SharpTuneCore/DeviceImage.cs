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

#define ADMIN

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using SharpTune.Tables;
using System.Windows.Forms;


namespace SharpTune
{
    /// <summary>
    /// Describes a ROM Image opened by the patching utility.
    /// </summary>
    /// 
    public class DeviceImage
    {
        public long FileSize { get; private set; }

        public string FileName { get; private set; }

        public string FilePath { get; private set; }

        public string FileDirectory { get; private set; }

        public string CalId { get; private set; }

        public int CalIdOffset { get; private set; }

        public bool isChanged { get; set; }

        public List<ModInfo> ModList { get; private set; }

        public Definition Definition { get; private set; }

        //public List<Table> tableList { get; set; }

        public TreeNode imageTree { get; set; }

        public Stream imageStream;

        

        /// <summary>
        /// Constructor
        /// </summary>
        /// 
        public DeviceImage(string fPath)
        {
            FileInfo f = new FileInfo(fPath);
            this.FileSize = f.Length;
            this.FileName = f.Name;
            this.FilePath = fPath;
            this.FileDirectory = fPath.Replace(f.Name, "");
            //f.Delete();

            using (FileStream fileStream = File.OpenRead(fPath))
            {
                MemoryStream memStream = new MemoryStream();
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
                this.imageStream = memStream;
            }
            //this.imageStream = File.Open(this.FilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

                foreach (KeyValuePair<string, Pair<int, string>> device in SharpTuner.availableDevices.IdentifierMap)
                {
                    this.imageStream.Seek(device.Value.First, SeekOrigin.Begin);

                    byte[] b = new byte[device.Value.Second.Length];
                    this.imageStream.Read(b, 0, device.Value.Second.Length);

                    if (device.Value.Second.ToString() == System.Text.Encoding.UTF8.GetString(b))
                    {
                        this.CalIdOffset = device.Value.First;
                        this.CalId = device.Value.Second.ToString();
                        this.Definition = new Definition(device.Key.ToString());


                        //this.tableList = new List<Table>();

                        //foreach (XElement table in this.Definition.xRomTableList.Values)
                        //{
                        //    this.tableList.Add(TableFactory.CreateTable(table, this));
                        //}
                       //this.imageTree = new TableTree(this);
                        this.imageTree = new TreeNode("(" + this.CalId + ") " + this.FileName);
                    }
                }

//TODO: Figure something better out here
//#if ADMIN
//                if (this.CalId == null)
//                {
//                    string inputcalid = SimplePrompt.ShowDialog("Definition Not Found", "Enter Definition CALID");
//                    foreach (KeyValuePair<string, Pair<int, string>> device in this.parent.availableDevices.IdentifierMap)
//                    {
//                        if (device.Value.Second.ToString() == inputcalid)
//                        {
//                            this.CalIdOffset = device.Value.First;
//                            this.CalId = device.Value.Second.ToString();
//                            this.Definition = new Definition(device.Key.ToString(), this);


//                            this.tableList = new List<Table>();

//                            foreach (XElement table in this.Definition.xRomTableList.Values)
//                            {
//                                this.tableList.Add(TableFactory.CreateTable(table, this));
//                            }
//                            this.imageTree = new TableTree(this);
//                        }
//                    }
                
//            }
//#endif



            this.ModList = new List<ModInfo>();
            
        }

        /// <summary>
        /// Memory Stream "Save" -> disk
        /// </summary>
        public void Save()
        {
            try
            {
                using (FileStream fileStream = File.OpenWrite(this.FilePath))
                {
                    this.imageStream.Seek(0, SeekOrigin.Begin);
                    this.imageStream.CopyTo(fileStream);
                }
                Console.WriteLine("Successfully saved image to {0}", this.FilePath);
            }
            catch (System.Exception excpt)
            {
                MessageBox.Show("Error accessing file! It is locked!", "RomMod", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("Error accessing file! It is locked!");
                Console.WriteLine(excpt.Message);
                return;
            }
        }

        /// <summary>
        /// Memory Stream "Save AS" -> disk
        /// </summary>
        /// <param name="fPath"></param>
        public void SaveAs()
        {
            SaveFileDialog d = new SaveFileDialog();
            d.Filter = "Binary/Hex files (*.bin; *.hex)|*.bin;*.hex";
            d.ShowDialog();
            

            if (d.FileName != null)
            {
                try
                {
                    using (FileStream fileStream = File.OpenWrite(d.FileName))
                    {
                        this.imageStream.Seek(0, SeekOrigin.Begin);
                        this.imageStream.CopyTo(fileStream);
                    }
                    Console.WriteLine("Successfully saved image to {0}", d.FileName);
                }
                catch (System.Exception excpt)
                {
                    MessageBox.Show("Error accessing file! It is locked!", "RomMod", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine("Error accessing file! It is locked!");
                    Console.WriteLine(excpt.Message);
                    return;
                }
            }
            else
            {
                MessageBox.Show("No output file specified! Try again!", "RomMod", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void Dispose()
        {

        }

        public void FindInAvailableDevices(Dictionary<string, Pair<int, string>> availableDevices, Stream imageStream)
        {
            
        }
        

        public bool PredicateFileName(string filename)
        {

            if (this.FileName == "filename")
            {
                return true;
            }
            {
                return false;
            }

        }

    }

}
