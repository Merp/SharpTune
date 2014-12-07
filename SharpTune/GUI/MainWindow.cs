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
        private readonly SharpTuner sharpTuner;

        TextWriter _writer = null;

        private KeyValuePair<Mod,ModDirection> selectedMod;

        public OpenFileDialog ofd = new OpenFileDialog();

        private Thread workerThread = null;

        public MainWindow(SharpTuner st)
        {
            sharpTuner = st;
            InitializeComponent();
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
            this.toolStripStatusLabel1.Text = "SharpTune Version " + sharpTuner.Version;
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
            sharpTuner.Init();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (sharpTuner.fileQueued == true)
            {
                openDeviceImage(sharpTuner.QueuedFilePath);
                sharpTuner.fileQueued = false;

            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (sharpTuner.fileQueued == true)
            {
                openDeviceImage(sharpTuner.QueuedFilePath);
                sharpTuner.fileQueued = false;

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
            //TODO move this, application logic shouldn't be in GUI class.
            ECU newImage = new ECU(sharpTuner, filename);
            if (newImage.CalId == null)
            {
                Trace.TraceWarning(String.Format("Unable to identify rom at {0}", newImage.FilePath.ToString()));
                MessageBox.Show("Unable to idenfity rom at " + newImage.FilePath.ToString());
                return;
            }
            foreach (ECU image in sharpTuner.ImageList)
            {
                if (image.FilePath == filename)
                {
                    Console.Write("Rom is already open!");
                    return;
                }
            }
            this.closeDeviceImageToolStripMenuItem.Enabled = true;
            obfuscateCALIDToolStripMenuItem.Enabled = true;
            sharpTuner.AddImage(newImage);
            this.openDeviceListBox.Items.Add(sharpTuner.activeImage.FileName);
            Trace.WriteLine("Successfully opened " + sharpTuner.activeImage.CalId + " filename: " + sharpTuner.activeImage.FileName);
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
            Application.Run(new CalidUtility(sharpTuner, this));
        }

        private void closeDeviceImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Prompt to save file!
            if (sharpTuner.activeImage.isChanged)
            {
                if (MessageBox.Show("File is changed", "Would you like to save it?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    sharpTuner.activeImage.SaveAs();
                    // a 'DialogResult.Yes' value was returned from the MessageBox
                    // proceed with your deletion
                }
            }

            if (sharpTuner.ImageList.Count == 0)
            {
                closeDeviceImageToolStripMenuItem.Enabled = false;
                //modUtilityToolStripMenuItem.Enabled = false;
                obfuscateCALIDToolStripMenuItem.Enabled = false;
            }
            else
            {
                int index = sharpTuner.ImageList.FindIndex(f => f.FilePath == sharpTuner.activeImage.FilePath);
                sharpTuner.ImageList.RemoveAt(index);
                this.openDeviceListBox.Items.RemoveAt(index);
                //this.imageTreeView.Nodes.Remove(n => n.Tag = sharpTuner.activeImage.FileName);
                //foreach (TreeNode node in this.imageTreeView.Nodes)
                //{
                //    if (node != null && node.Tag != null && node.Tag.ToString() == sharpTuner.activeImage.FilePath)
                //    {
                //        node.Remove();
                //    }
                //}
                if (sharpTuner.ImageList.Count != 0)
                {
                    sharpTuner.activeImage = sharpTuner.ImageList[0];
                    //ImageTreeRefresh();
                }
                else
                {
                    closeDeviceImageToolStripMenuItem.Enabled = false;
                    sharpTuner.activeImage = null;
                    obfuscateCALIDToolStripMenuItem.Enabled = false;
                }
            }
            Refresh();
        }

        private void saveDeviceImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sharpTuner.activeImage.Save();
        }

        private void saveDeviceImageAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sharpTuner.activeImage.SaveAs();
        }

        private void openDeviceListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (this.openDeviceListBox.SelectedItem != null)
            {
                string s = @" CALID: ";
                string sim = this.openDeviceListBox.SelectedItem.ToString();
                string[] im = Regex.Split(sim, s);
                int index = sharpTuner.ImageList.FindIndex(i => i.FileName == im[0]);
                sharpTuner.activeImage = sharpTuner.ImageList[index];
            }
            Refresh();
        }

        public override void Refresh()
        {
            RefreshImageInfo();
            RefreshTableTree();
            RefreshModTree();
            RefreshModInfo();
            base.Refresh();
        }

        private void RefreshImageInfo()
        {
            if (sharpTuner.activeImage == null)
                return;

            object io = null;
            foreach (var i in openDeviceListBox.Items)
            {
                if (i.ToString() == sharpTuner.activeImage.FileName.ToString())
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
                Mod m = selectedMod.Key;
                if (selectedMod.Value == ModDirection.Apply)
                    buttonPatchRom.Text = "Apply Mod";
                else if (selectedMod.Value == ModDirection.Remove)
                    buttonPatchRom.Text = "Remove Mod";
                else if (selectedMod.Value == ModDirection.Upgrade)
                    buttonPatchRom.Text = "Upgrade Mod";

                DataTextBox.Text = "FileName: " +
                    m.FileName +
                    Environment.NewLine;
                DataTextBox.AppendText(
                    "Mod Identifier: " +
                    m.ModIdent +
                    Environment.NewLine);
                DataTextBox.AppendText(
                    "Version: " +
                    m.ModBuild +
                    Environment.NewLine);
                DataTextBox.AppendText(
                    "Author: " +
                    m.ModAuthor +
                    Environment.NewLine);
               DataTextBox.AppendText(
                    "Description: " + 
                    m.ModInfo + 
                    Environment.NewLine);
            }
            catch (System.Exception excpt)
            {
                string derp = excpt.Message;
                DataTextBox.Clear();
                buttonPatchRom.Enabled = false;
                buttonPatchRom.Text = "Select a patch";
            }
            
        }

        public void RefreshTableTree()
        {
            TableTreeView.Nodes.Clear();
            if (sharpTuner.activeImage == null)
            {
                TableTreeView.Nodes.Add("");
                return;
            }

            if (sharpTuner.activeImage.romTableList != null && sharpTuner.activeImage.romTableList.Count > 0)
            {
                TableTreeView.Nodes.Add(sharpTuner.activeImage.FileName);

                TreeNode infoTree = new TreeNode("ROM Info");
                foreach(KeyValuePair<string,string> prop in sharpTuner.activeImage.Definition.ident.propertyBag)
                {
                    infoTree.Nodes.Add(prop.Key + ": " + prop.Value);
                }
               
                TableTreeView.Nodes.Add(infoTree);

                foreach (string category in sharpTuner.activeImage.romTableCategories)
                {
                    TreeNode catTree = new TreeNode(category);
                    catTree.Tag = category;

                    foreach(TableMetaData table in sharpTuner.activeImage.romTableList.Values)
                    {
                        if(category.EqualsCI(table.category))
                        {
                            TreeNode tableTree = new TreeNode(table.name);
                            tableTree.Tag = table.name;
                            catTree.Nodes.Add(tableTree);
                        }
                    }

                    TableTreeView.Nodes.Add(catTree);
                }
            }
            else
                TableTreeView.Nodes.Add("No Tables Found for " + sharpTuner.activeImage.FileName);
        }

        public void RefreshModTree()
        {
            ModTreeView.Nodes.Clear();
            if (sharpTuner.activeImage == null)
            {
                ModTreeView.Nodes.Add("");
                return;
            }

            if (sharpTuner.activeImage.ModList.Count > 0)
            {
                ModTreeView.Nodes.Add("Compatible MODs for " + sharpTuner.activeImage.FileName);
                foreach (KeyValuePair<Mod,ModDirection> mod in sharpTuner.activeImage.ModList)
                {
                    TreeNode patchTree = new TreeNode(mod.Value.ToString() + ": " + mod.Key.FileName);
                    patchTree.Tag = mod.Key.FilePath;

                    ModTreeView.Nodes.Add(patchTree);
                }
            }
            else
                ModTreeView.Nodes.Add("No Mods Found for " + sharpTuner.activeImage.FileName);
        }

        private void buttonPatchRom_Click(object sender, EventArgs e)
        {
            Mod currentmod = selectedMod.Key;
            SaveFileDialog d = new SaveFileDialog();
            d.InitialDirectory = sharpTuner.activeImage.FilePath;
            d.Filter = "Binary/Hex files (*.bin; *.hex)|*.bin;*.hex";
            d.FileName = currentmod.ModIdent.ToString() + "_" + sharpTuner.activeImage.FileName;
            //d.ShowDialog();
            DialogResult ret = Utils.STAShowSADialog(d);

            if (ret == DialogResult.OK && d.FileName != null)
            {
                try
                {
                    if (sharpTuner.activeImage.FilePath != d.FileName)
                    {
                        System.IO.File.Copy(sharpTuner.activeImage.FilePath, d.FileName, true);
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
            if (selectedMod.Value == ModDirection.Apply)
            {
                TryApplyMod(currentmod,sharpTuner.activeImage.FilePath,d.FileName);
            }
            else if(selectedMod.Value == ModDirection.Remove)
            {
                TryRemoveMod(currentmod,sharpTuner.activeImage.FilePath, d.FileName);
            }
            else if (selectedMod.Value == ModDirection.Upgrade)
            {
                TryUpgradeMod(currentmod, d.FileName);
            }
            else
            {
                MessageBox.Show("MOD FAILED!" + System.Environment.NewLine + "See Log for details!", "SharpTune", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Refresh();
        }

        private bool TryApplyMod(Mod m, string infile, string outfile)
        {
            if (m.TryCheckApplyMod(infile, outfile, true, true))
            {
                MessageBox.Show("MOD SUCCESSFULLY APPLIED!", "SharpTune", MessageBoxButtons.OK, MessageBoxIcon.Information);

                sharpTuner.fileQueued = true;
                sharpTuner.QueuedFilePath = outfile;
                return true;
            }
            return false;
        }

        private bool TryRemoveMod(Mod m, string infile, string outfile)
        {
            if (m.TryCheckApplyMod(infile, outfile, false, true))
            {
                MessageBox.Show("MOD SUCCESSFULLY REMOVED!", "SharpTune", MessageBoxButtons.OK, MessageBoxIcon.Information);

                sharpTuner.fileQueued = true;
                sharpTuner.QueuedFilePath = outfile;
                return true;
            }
            return false;
        }

        private bool TryUpgradeMod(Mod m, string outfile)
        {
            foreach (KeyValuePair<Mod,ModDirection> mkvp in sharpTuner.activeImage.ModList)
            {
                if (mkvp.Value == ModDirection.Remove)
                {
                    if (m.TryCheckApplyMod(sharpTuner.activeImage.FilePath, outfile, false, true))
                    {
                        if (m.TryCheckApplyMod(outfile, outfile, true, true))
                        {
                            MessageBox.Show("MOD SUCCESSFULLY UPGRADED!", "SharpTune", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            sharpTuner.fileQueued = true;
                            sharpTuner.QueuedFilePath = outfile;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if ((ModTreeView.SelectedNode != null) && (ModTreeView.SelectedNode.Tag != null) && (ModTreeView.SelectedNode.Tag.ToString().Contains(".patch")))
            {
                buttonPatchRom.Enabled = true;
                //buttonTestPatch.Enabled = true;
                selectedMod = sharpTuner.activeImage.ModList.Single(m => m.Key.FilePath == ModTreeView.SelectedNode.Tag.ToString());
                
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

            Dictionary<String,String> imap = sharpTuner.AvailableDevices.BuildInheritanceMap();
            
			foreach (String deffile in sharpTuner.AvailableDevices.DefDictionary.Keys) {
				ECUMetaData.pullScalings (deffile, ref xblobscalings, ref xscalings);
			}
			ECUMetaData.pullScalings("rommetadata\\bases\\32BITBASE.xml", ref xblobscalings, ref xscalings);
			ECUMetaData.pullScalings("rommetadata\\bases\\16BITBASE.xml", ref xblobscalings, ref xscalings);
            foreach (String deffile in sharpTuner.AvailableDevices.DefDictionary.Keys)
            {
                ECUMetaData.pullScalings(deffile, ref xblobscalings, ref xscalings);
            }

            foreach (XElement xbs in xblobscalings)
            {
                blobscalings.Add(xbs.Attribute("name").Value);
            }

            ECUMetaData.ConvertXML ("rommetadata\\bases\\32BITBASE.xml", ref blobscalings, ref t3d, ref t2d, ref t1d, imap, true);
			ECUMetaData.ConvertXML ("rommetadata\\bases\\16BITBASE.xml", ref blobscalings, ref t3d, ref t2d, ref t1d, imap, true);
			
            foreach (String deffile in sharpTuner.AvailableDevices.DefDictionary.Keys) {
				ECUMetaData.ConvertXML (deffile, ref blobscalings, ref t3d, ref t2d, ref t1d, imap, false);
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
            if (sharpTuner.activeImage != null)
            {
                string path = sharpTuner.activeImage.ToString();
                d.SelectedPath = path;
            }
            DialogResult ret = Utils.STAShowFDialog(d);

            if (ret == DialogResult.OK)
            {
                Settings.Default.SubaruDefsRepoPath = d.SelectedPath.ToString();
                Settings.Default.Save();
                Trace.WriteLine("Definition Repo Path Changed to: " + Settings.Default.SubaruDefsRepoPath);
                sharpTuner.Init();
                sharpTuner.RefreshImages();
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
            if (sharpTuner.activeImage == null)
            {
                MessageBox.Show("No ROM selected! Please open and select a ROM first!");
                ofd.Filter = "Binary/Hex files (*.bin; *.hex)|*.bin;*.hex";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    openDeviceImage(ofd.FileName);
                }
                if (sharpTuner.activeImage == null)
                {
                    MessageBox.Show("OPEN A ROM, DUMMY!");
                    return false;
                }
            }
            return true;
        }

        private void SpawnXMLToIDC()
        {
            Application.Run(new XMLtoIDCGUI(sharpTuner.activeImage));
        }

        private void iDAToHEWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.workerThread = new Thread(new ThreadStart(this.SpawnIDAtoHEW));
            this.workerThread.Start();
        }

        private void SpawnIDAtoHEW()
        {
            Application.Run(new IDAtoHEW(sharpTuner.AvailableDevices));
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
                sharpTuner.LoadMods();
                sharpTuner.InitSettings();
                sharpTuner.RefreshImages();
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
            Application.Run(new MapToDef(sharpTuner));
        }

        private void splitContainer2_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start(Settings.Default.DonateUrl);
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void sharpTuningForumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Settings.Default.ForumUrl);
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
            Application.Run(new DefinitionEditor(sharpTuner));
        }

        private void xMLToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
