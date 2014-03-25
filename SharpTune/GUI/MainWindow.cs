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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ConsoleRedirection;
using DumpXML;
using System.IO.Ports;
using SharpTune.RomMod;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Threading;
using System.Diagnostics;
using SharpTuneCore;
using SharpTune.GUI;
using System.Xml.Linq;
using System.Xml;
using System.Reflection;
using System.Text.RegularExpressions;
using SharpTune;
using System.Resources;
using System.Collections;
using SharpTune.Properties;



namespace SharpTune
{
    public partial class MainWindow : Form
    {
        TextWriter _writer = null;

        private int selectedModIndex;

        public OpenFileDialog ofd = new OpenFileDialog();

        private Thread workerThread = null;

        public MainWindow()
        {
            InitializeComponent();
            SharpTuner.Window = this;
        }

        public static void debugCheck()
        {
            if (Debugger.IsAttached)
            {
                // This gives you time to examine the output before the console window closes.
                Debugger.Break();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel1.Text = "SharpTune Version " + SharpTuner.Version;
            MessageBox.Show(@"
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    " + @"THIS SOFTWARE IS PROVIDED 'AS IS' AND WITHOUT ANY WARRANTY
    OF ANY KIND, WHETHER ORAL, WRITTEN, EXPRESS, IMPLIED OR STATUTORY, 
    INCLUDING BUT NOT LIMITED TO WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE, AND NONINFRINGEMENT. FOR OFF-ROAD USE ONLY.
    
    " + @"Source code for this program and all included software, 
    patches, data, and information can be found at:
    http://github.com/Merp/SharpTune
    http://github.com/Merp/MerpMod
    http://github.com/Merp/SubaruDefs." , "SharpTune");
            // Instantiate the writer
            //TODO switch to traces instead of console redirection.
            _writer = new TextBoxStreamWriter(txtConsole);
            // Redirect the out Console stream
            Console.SetOut(_writer);
            SharpTuner.Init();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (SharpTuner.fileQueued == true)
            {
                openDeviceImage(SharpTuner.QueuedFilePath);
                SharpTuner.fileQueued = false;

            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (SharpTuner.fileQueued == true)
            {
                openDeviceImage(SharpTuner.QueuedFilePath);
                SharpTuner.fileQueued = false;

            }
        }

        public static void SetAccessRule(string directory)
        {
            System.Security.AccessControl.DirectorySecurity sec = System.IO.Directory.GetAccessControl(directory);
            FileSystemAccessRule accRule = new FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, FileSystemRights.FullControl, AccessControlType.Allow);
            sec.AddAccessRule(accRule);
        }

        public void openDeviceImage(string filename)
        {
            //Construct new romimage
            DeviceImage newImage = new DeviceImage(filename);
            if (newImage.CalId == null)
            {
                Trace.TraceWarning(String.Format("Unable to identify rom at {0}", newImage.FilePath.ToString()));
                MessageBox.Show("Unable to idenfity rom at " + newImage.FilePath.ToString());
                return;
            }
            foreach (DeviceImage image in SharpTuner.ImageList)
            {
                if (image.FilePath == filename)
                {
                    Console.Write("Rom is already open!");
                    return;
                }
            }
            this.closeDeviceImageToolStripMenuItem.Enabled = true;
            obfuscateCALIDToolStripMenuItem.Enabled = true;
            SharpTuner.AddImage(newImage);
            this.openDeviceListBox.Items.Add(SharpTuner.ActiveImage.FileName);
            Trace.WriteLine("Successfully opened " + SharpTuner.ActiveImage.CalId + " filename: " + SharpTuner.ActiveImage.FileName);
            Refresh();
        }

        private void openDeviceImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofd.Filter = "Binary/Hex files (*.bin; *.hex)|*.bin;*.hex";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                openDeviceImage(ofd.FileName);
            }
        }

        private void obfuscateCALIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Initialise and start worker thread
            this.workerThread = new Thread(new ThreadStart(this.SpawnCalidUtility));
            this.workerThread.Start();
        }

        private void SpawnCalidUtility()
        {
            Application.Run(new CalidUtility(this));
        }

        private void closeDeviceImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Prompt to save file!
            if (SharpTuner.ActiveImage.isChanged)
            {
                if (MessageBox.Show("File is changed", "Would you like to save it?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SharpTuner.ActiveImage.SaveAs();
                    // a 'DialogResult.Yes' value was returned from the MessageBox
                    // proceed with your deletion
                }
            }

            if (SharpTuner.ImageList.Count == 0)
            {
                closeDeviceImageToolStripMenuItem.Enabled = false;
                //modUtilityToolStripMenuItem.Enabled = false;
                obfuscateCALIDToolStripMenuItem.Enabled = false;
            }
            else
            {
                int index = SharpTuner.ImageList.FindIndex(f => f.FilePath == SharpTuner.ActiveImage.FilePath);
                SharpTuner.ImageList.RemoveAt(index);
                this.openDeviceListBox.Items.RemoveAt(index);
                //this.imageTreeView.Nodes.Remove(n => n.Tag = SharpTuner.activeImage.FileName);
                //foreach (TreeNode node in this.imageTreeView.Nodes)
                //{
                //    if (node != null && node.Tag != null && node.Tag.ToString() == SharpTuner.activeImage.FilePath)
                //    {
                //        node.Remove();
                //    }
                //}
                if (SharpTuner.ImageList.Count != 0)
                {
                    SharpTuner.ActiveImage = SharpTuner.ImageList[0];
                    //ImageTreeRefresh();
                }
                else
                {
                    closeDeviceImageToolStripMenuItem.Enabled = false;
                    SharpTuner.ActiveImage = null;
                    obfuscateCALIDToolStripMenuItem.Enabled = false;
                }
            }
            Refresh();
        }

        private void saveDeviceImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SharpTuner.ActiveImage.Save();
        }

        private void saveDeviceImageAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SharpTuner.ActiveImage.SaveAs();
        }

        private void openDeviceListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (this.openDeviceListBox.SelectedItem != null)
            {
                string s = @" CALID: ";
                string sim = this.openDeviceListBox.SelectedItem.ToString();
                string[] im = Regex.Split(sim, s);
                int index = SharpTuner.ImageList.FindIndex(i => i.FileName == im[0]);
                SharpTuner.ActiveImage = SharpTuner.ImageList[index];
            }
            Refresh();
        }

        public override void Refresh()
        {
            RefreshImageInfo();
            RefreshModTree();
            RefreshModInfo();
            base.Refresh();
        }

        private void RefreshImageInfo()
        {
            if (SharpTuner.ActiveImage == null)
                return;

            object io = null;
            foreach (var i in openDeviceListBox.Items)
            {
                if (i.ToString() == SharpTuner.ActiveImage.FileName.ToString())
                {
                    io = i;
                    break;
                }
            }
            if (io != null)
                openDeviceListBox.SelectedItem = io;
        }

        public void RefreshModInfo()
        {
            try
            {
                Mod m = SharpTuner.ActiveImage.ModList[selectedModIndex];
                
                if (!SharpTuner.ActiveImage.ModList[selectedModIndex].isApplied)
                    buttonPatchRom.Text = "Apply Mod";
                else
                    buttonPatchRom.Text = "Remove Mod";

                selectedModTextBox.Text = "FileName: " +
                    m.FileName +
                    Environment.NewLine;
                selectedModTextBox.AppendText(
                    "Mod Identifier: " +
                    m.ModIdent +
                    Environment.NewLine);
                selectedModTextBox.AppendText(
                    "Version: " +
                    m.ModBuild +
                    Environment.NewLine);
                selectedModTextBox.AppendText(
                    "Author: " +
                    m.ModAuthor +
                    Environment.NewLine);
               selectedModTextBox.AppendText(
                    "Description: " + 
                    m.ModInfo + 
                    Environment.NewLine);
            }
            catch (System.Exception excpt)
            {
                string derp = excpt.Message;
                selectedModTextBox.Clear();
                buttonPatchRom.Enabled = false;
                buttonPatchRom.Text = "Select a patch";
            }
            
        }

        public void RefreshModTree()
        {
            treeView1.Nodes.Clear();
            if (SharpTuner.ActiveImage == null)
            {
                treeView1.Nodes.Add("");
                return;
            }

            if (SharpTuner.ActiveImage.ModList.Count > 0)
            {
                treeView1.Nodes.Add("Compatible MODs for " + SharpTuner.ActiveImage.FileName);
                foreach (Mod mod in SharpTuner.ActiveImage.ModList)
                {
                    TreeNode patchTree = new TreeNode(mod.direction + ": " + mod.FileName);
                    patchTree.Tag = mod.FilePath;

                    treeView1.Nodes.Add(patchTree);
                }
            }
            else
                treeView1.Nodes.Add("No Mods Found for " + SharpTuner.ActiveImage.CalId.ToString());
        }

        private void buttonPatchRom_Click(object sender, EventArgs e)
        {
            Mod currentmod = SharpTuner.ActiveImage.ModList[selectedModIndex];
            SaveFileDialog d = new SaveFileDialog();
            d.InitialDirectory = SharpTuner.ActiveImage.FilePath;
            d.Filter = "Binary/Hex files (*.bin; *.hex)|*.bin;*.hex";
            d.FileName = currentmod.ModIdent.ToString() + "_" + SharpTuner.ActiveImage.FileName;
            //d.ShowDialog();
            DialogResult ret = Utils.STAShowSADialog(d);

            if (ret == DialogResult.OK && d.FileName != null)
            {
                try
                {
                    if (SharpTuner.ActiveImage.FilePath != d.FileName)
                    {
                        System.IO.File.Copy(SharpTuner.ActiveImage.FilePath, d.FileName, true);
                    }
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
            if (!currentmod.isApplied && currentmod.TryCheckApplyMod(SharpTuner.ActiveImage.FilePath, d.FileName, true, true))
            {
                MessageBox.Show("MOD SUCCESSFULLY APPLIED!", "SharpTune", MessageBoxButtons.OK, MessageBoxIcon.Information);

                SharpTuner.fileQueued = true;
                SharpTuner.QueuedFilePath = d.FileName;
            }
            else if(currentmod.isApplied && currentmod.TryCheckApplyMod(SharpTuner.ActiveImage.FilePath, d.FileName, false, true))
            {
                MessageBox.Show("MOD SUCCESSFULLY REMOVED!", "SharpTune", MessageBoxButtons.OK, MessageBoxIcon.Information);

                SharpTuner.fileQueued = true;
                SharpTuner.QueuedFilePath = d.FileName;
            }              
            else
            {
                MessageBox.Show("MOD FAILED!" + System.Environment.NewLine + "See Log for details!", "SharpTune", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Refresh();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if ((treeView1.SelectedNode != null) && (treeView1.SelectedNode.Tag != null) && (treeView1.SelectedNode.Tag.ToString().Contains(".patch")))
            {
                buttonPatchRom.Enabled = true;
                //buttonTestPatch.Enabled = true;
                selectedModIndex = SharpTuner.ActiveImage.ModList.FindIndex(m => m.FilePath == treeView1.SelectedNode.Tag.ToString());
                
                RefreshModInfo();
            }
            else
            {
                buttonPatchRom.Enabled = false;
                buttonPatchRom.Text = "Select a patch";
            }
        }

        private void romRaiderIRCChannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://webchat.freenode.net/?channels=romraider&uio=d4");
        }

        private void romRaiderForumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.romraider.com/forum/viewforum.php?f=37");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutWindow aboutwindow = new AboutWindow();
            aboutwindow.ShowDialog();
        }

        private void licensingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LicenseWindow licensewindow = new LicenseWindow();
            licensewindow.ShowDialog();
        }

        private void convertEFXMLRRv2ToolStripMenuItem_Click (object sender, EventArgs e)
		{
            //TODO: this is a big fucking mess
			List<XElement> xscalings = new List<XElement> ();
            List<XElement> xblobscalings = new List<XElement>();
			List<String> blobscalings = new List<string>();
            Dictionary<String, List<String>> t3d = new Dictionary<String, List<String>>();
            Dictionary<String, List<String>> t2d = new Dictionary<String, List<String>>();
            Dictionary<String, List<String>> t1d = new Dictionary<String, List<String>>();
            t3d.Add("32BITBASE",new List<String>());
            t2d.Add("32BITBASE",new List<String>());
            t1d.Add("32BITBASE",new List<String>());
            t3d.Add("16BITBASE",new List<String>());
            t2d.Add("16BITBASE", new List<String>());
            t1d.Add("16BITBASE", new List<String>());

            Dictionary<String,String> imap = SharpTuner.AvailableDevices.BuildInheritanceMap();
            
			foreach (String deffile in SharpTuner.AvailableDevices.DefDictionary.Keys) {
				Definition.pullScalings (deffile, ref xblobscalings, ref xscalings);
			}
			Definition.pullScalings("rommetadata\\bases\\32BITBASE.xml", ref xblobscalings, ref xscalings);
			Definition.pullScalings("rommetadata\\bases\\16BITBASE.xml", ref xblobscalings, ref xscalings);
            foreach (String deffile in SharpTuner.AvailableDevices.DefDictionary.Keys)
            {
                Definition.pullScalings(deffile, ref xblobscalings, ref xscalings);
            }

            foreach (XElement xbs in xblobscalings)
            {
                blobscalings.Add(xbs.Attribute("name").Value);
            }

            Definition.ConvertXML ("rommetadata\\bases\\32BITBASE.xml", ref blobscalings, ref t3d, ref t2d, ref t1d, imap, true);
			Definition.ConvertXML ("rommetadata\\bases\\16BITBASE.xml", ref blobscalings, ref t3d, ref t2d, ref t1d, imap, true);
			
            foreach (String deffile in SharpTuner.AvailableDevices.DefDictionary.Keys) {
				Definition.ConvertXML (deffile, ref blobscalings, ref t3d, ref t2d, ref t1d, imap, false);
			}
			
            string filename = "rommetadata\\Scalings\\";//scalings.xml";
            Directory.CreateDirectory(filename);
            filename = filename + "scalings.xml";
            
	    using (XmlTextWriter xmlWriter = new XmlTextWriter(filename, new UTF8Encoding(false)))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.Indentation = 4;
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("scalings");
                foreach (XElement xel in xscalings)
                {
                    xel.WriteTo(xmlWriter);
                }
                foreach (XElement xel in xblobscalings)
                {
                    xel.Name = "blobscaling";
                    xel.WriteTo(xmlWriter);
                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }
        }

        private void definitionLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            if (SharpTuner.ActiveImage != null)
            {
                string path = SharpTuner.ActiveImage.ToString();
                d.SelectedPath = path;
            }
            DialogResult ret = Utils.STAShowFDialog(d);

            if (ret == DialogResult.OK)
            {
                Settings.Default.SubaruDefsRepoPath = d.SelectedPath.ToString();
                Settings.Default.Save();
                Trace.WriteLine("Definition Repo Path Changed to: " + Settings.Default.SubaruDefsRepoPath);
                SharpTuner.Init();
                SharpTuner.RefreshImages();
            } 
        }

        private void xMLToIDCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ForceOpenRom())
            {
                // Initialise and start worker thread
                this.workerThread = new Thread(new ThreadStart(this.SpawnXMLToIDC));
                this.workerThread.Start();
            }
        }

        private bool ForceOpenRom()
        {
            if (SharpTuner.ActiveImage == null)
            {
                MessageBox.Show("No ROM selected! Please open and select a ROM first!");
                ofd.Filter = "Binary/Hex files (*.bin; *.hex)|*.bin;*.hex";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    openDeviceImage(ofd.FileName);
                }
                if (SharpTuner.ActiveImage == null)
                {
                    MessageBox.Show("OPEN A ROM, DUMMY!");
                    return false;
                }
            }
            return true;
        }

        private void SpawnXMLToIDC()
        {
            Application.Run(new XMLtoIDC());
        }

        private void iDAToHEWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.workerThread = new Thread(new ThreadStart(this.SpawnIDAtoHEW));
            this.workerThread.Start();
        }

        private void SpawnIDAtoHEW()
        {
            Application.Run(new IDAtoHEW());
        }

        private void manuallySelectPatchToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //Open dialog to change patch file location
            //Then refresh patches

            FolderBrowserDialog d = new FolderBrowserDialog();
            //d.RootFolder = Environment.SpecialFolder.MyComputer;
            string path = Settings.Default.PatchPath;
            d.SelectedPath = path;

            //d.ShowDialog();
            DialogResult ret = Utils.STAShowFDialog(d);

            if (ret == DialogResult.OK)
            {
                Settings.Default.PatchPath = d.SelectedPath;
                SharpTuner.LoadMods();
                SharpTuner.InitSettings();
                SharpTuner.RefreshImages();
            }
        }

        private void mAPToRRLoggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ForceOpenRom())
            {
                this.workerThread = new Thread(new ThreadStart(this.SpawnMapToDef));
                this.workerThread.Start();
            }
        }

        private void SpawnMapToDef()
        {
            Application.Run(new MapToDef());
        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start(SharpTuner.DonateUrl);
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void sharpTuningForumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(SharpTuner.ForumUrl);
        }

        private void definitionEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ForceOpenRom())
            {
                this.workerThread = new Thread(new ThreadStart(this.SpawnDefEditor));
                this.workerThread.Start();
            }
        }

        private void SpawnDefEditor()
        {
            Application.Run(new DefinitionEditor());
        }

        private void xMLToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
