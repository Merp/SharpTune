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
using System.Windows.Forms;
using SharpTune.RomMod;
using SharpTune;
using System.Resources;
using SharpTune.Properties;
using System.Collections;
using SharpTune.GUI;
using System.Diagnostics;


namespace SharpTuneCore
{
    /// <summary>
    /// Describes a ROM Image opened by the patching utility.
    /// </summary>
    /// 
    public class ECU
    {
        private readonly SharpTuner sharpTuner;

        public long FileSize { get; private set; }

        public string FileName { get; private set; }

        public string FilePath { get; private set; }

        public string FileDirectory { get; private set; }

        public string CalId { get; private set; }

        public int CalIdOffset { get; private set; }

        public bool isChanged { get; set; }

        public ECUMetaData Definition { get; private set; }

        public List<TableMetaData> tableList { get; set; }

        public TreeNode imageTree { get; set; }

        public Stream imageStream;

        public List<Mod> ModList { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// 
        public ECU(SharpTuner st, string fPath)
        {
            sharpTuner = st;
            FileInfo f = new FileInfo(fPath);
            this.FileSize = f.Length;
            this.FileName = f.Name;
            this.FilePath = fPath;
            this.FileDirectory = fPath.Replace(f.Name, "");
            
            TryOpenRom(fPath);

            if (this.CalId == null)
            {
                UndefinedWindow uw = new UndefinedWindow(sharpTuner, fPath);
                uw.ShowDialog();
                TryOpenRom(fPath);
            }
        }

        public void Refresh()
        {
            this.ModList.Clear();
            TryOpenRom(FilePath);
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
                Trace.WriteLine(String.Format("Successfully saved image to {0}", this.FilePath));
            }
            catch (System.Exception excpt)
            {
                MessageBox.Show("Error accessing file! It is locked!", "SharpTune", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Trace.WriteLine("Error accessing file! It is locked!");
                Trace.WriteLine(excpt.Message);
                return;
            }
        }

        private bool TryOpenRom(string f)
        {
            using (FileStream fileStream = File.OpenRead(f))
            {
                MemoryStream memStream = new MemoryStream();
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
                this.imageStream = memStream;
            }

            foreach (KeyValuePair<string, ECUMetaData> device in sharpTuner.AvailableDevices.DefDictionary)
            {
                this.imageStream.Seek(device.Value.calibrationIdAddress, SeekOrigin.Begin);

                byte[] b = new byte[device.Value.calibrationlId.Length];
                this.imageStream.Read(b, 0, device.Value.calibrationlId.Length);

                if (device.Value.calibrationlId.ToString() == System.Text.Encoding.UTF8.GetString(b))
                {
                    CalIdOffset = device.Value.calibrationIdAddress;
                    CalId = device.Key.ToString();
                    Definition = device.Value;
                    Definition.Populate();
                    //this.tableList = new List<Table>();

                    //foreach (XElement table in this.Definition.xRomTableList.Values)
                    //{
                    //    this.tableList.Add(TableFactory.CreateTable(table, this));
                    //}
                    //this.imageTree = new TableTree(this);
                    this.imageTree = new TreeNode("(" + this.CalId + ") " + this.FileName);
                    ModList = sharpTuner.GetValidMods(this); //FAN OUT
                    return true;
                }
            }
            Trace.TraceWarning(String.Format("Error opening Rom at {0}, Definition not found!",f));
            MessageBox.Show("Error opening Rom!, Definition not found!");
            return false;
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
                    Trace.WriteLine(String.Format("Successfully saved image to {0}", d.FileName));
                }
                catch (System.Exception excpt)
                {
                    MessageBox.Show("Error accessing file! It is locked!", "SharpTune", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Trace.WriteLine("Error accessing file! It is locked!");
                    Trace.WriteLine(excpt.Message);
                    return;
                }
            }
            else
            {
                MessageBox.Show("No output file specified! Try again!", "SharpTune", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        public void Dispose()
        {

        }

        public void FindInAvailableDevices(Dictionary<string,KeyValuePair<int, string>> availableDevices, Stream imageStream)
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
