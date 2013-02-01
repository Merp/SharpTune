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
//using NateW.Ssm;
using RomModCore;
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
using Merp;



namespace SharpTune
{
    public partial class MainWindow : Form
    {
        TextWriter _writer = null;

        private int selectedModIndex;

        OpenFileDialog ofd = new OpenFileDialog();

        private Thread workerThread = null;

        public MainWindow()
        {
            InitializeComponent();
            SharpTuner.mainWindow = this;
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
            // Instantiate the writer
            _writer = new TextBoxStreamWriter(txtConsole);
            // Redirect the out Console stream
            Console.SetOut(_writer);

            loadDevices();
            Assembly assembly = Assembly.GetExecutingAssembly();
            LoadMods(assembly);
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

        public void loadDevices()
        {
            //BackgroundWorker bw = new BackgroundWorker();
            // bw.DoWork += (senderr, ee) =>
            //{
            SharpTuner.populateAvailableDevices();
            //backgroundWorker1.ReportProgress(prog);
            // };
            // bw.RunWorkerAsync();
            SharpTuner.imageList = new List<DeviceImage>();
        }

        public void openDeviceImage(string filename)
        {
            //Construct new romimage
            DeviceImage newImage = new DeviceImage(filename);
            if (newImage.CalId == null)
            {
                Console.WriteLine("Unable to identify rom at {0}", newImage.FilePath.ToString());
                MessageBox.Show("Unable to idenfity rom at " + newImage.FilePath.ToString());
                return;
            }
            foreach (DeviceImage image in SharpTuner.imageList)
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
            SharpTuner.activeImage = newImage;
            this.openDeviceListBox.Items.Add(SharpTuner.activeImage.FileName + " CALID: "+SharpTuner.activeImage.CalId);
            Console.WriteLine("Successfully opened " + SharpTuner.activeImage.CalId + " filename: " + SharpTuner.activeImage.FileName);
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
            if (SharpTuner.activeImage.isChanged)
            {
                if (MessageBox.Show("File is changed", "Would you like to save it?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    SharpTuner.activeImage.SaveAs();
                    // a 'DialogResult.Yes' value was returned from the MessageBox
                    // proceed with your deletion
                }
            }

            if (SharpTuner.imageList.Count == 0)
            {
                closeDeviceImageToolStripMenuItem.Enabled = false;
                //modUtilityToolStripMenuItem.Enabled = false;
                obfuscateCALIDToolStripMenuItem.Enabled = false;
            }
            else
            {
                int index = SharpTuner.imageList.FindIndex(f => f.FilePath == SharpTuner.activeImage.FilePath);
                SharpTuner.imageList.RemoveAt(index);
                this.openDeviceListBox.Items.RemoveAt(index);
                //this.imageTreeView.Nodes.Remove(n => n.Tag = SharpTuner.activeImage.FileName);
                //foreach (TreeNode node in this.imageTreeView.Nodes)
                //{
                //    if (node != null && node.Tag != null && node.Tag.ToString() == SharpTuner.activeImage.FilePath)
                //    {
                //        node.Remove();
                //    }
                //}
                if (SharpTuner.imageList.Count != 0)
                {
                    SharpTuner.activeImage = SharpTuner.imageList[0];
                    //ImageTreeRefresh();
                }
                else
                {
                    closeDeviceImageToolStripMenuItem.Enabled = false;
                    //modUtilityToolStripMenuItem.Enabled = false;
                    obfuscateCALIDToolStripMenuItem.Enabled = false;
                }
            }
            RefreshModTree();
            RefreshModInfo();
        }

        private void saveDeviceImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SharpTuner.activeImage.Save();
        }

        private void saveDeviceImageAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SharpTuner.activeImage.SaveAs();
        }

        private void openDeviceListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (this.openDeviceListBox.SelectedItem != null)
            {
                string s = @" CALID: ";
                string sim = this.openDeviceListBox.SelectedItem.ToString();
                string[] im = Regex.Split(sim, s);
                int index = SharpTuner.imageList.FindIndex(i => i.FileName == im[0]);
                SharpTuner.activeImage = SharpTuner.imageList[index];
            }
            RefreshModTree();
            RefreshModInfo();
        }
        
        private void LoadMods(Assembly assembly)
        {
            int i = SharpTuner.activeImage.ModList.Count;
            SharpTuner.activeImage.ModList.Clear();
            string calid = SharpTuner.activeImage.CalId.ToString();
            string[] mods = assembly.GetManifestResourceNames();
            foreach (string modpath in mods)
            {
                if (modpath.ContainsCI(SharpTuner.activeImage.CalId.ToString()))
                {
                    Stream stream = assembly.GetManifestResourceStream(modpath);
                    Mod tempMod = new Mod(stream, modpath);
                    if (tempMod.TryCheckApplyMod(SharpTuner.activeImage.FilePath, SharpTuner.activeImage.FilePath + ".temp", false))
                        SharpTuner.activeImage.ModList.Add(tempMod);
                }
            }
            if (SharpTuner.activeImage.ModList == null || SharpTuner.activeImage.ModList.Count == i)
            {
                Console.WriteLine("NO VALID MODS FOR THIS ROM: {0}", SharpTuner.activeImage.FileName);
            }
            RefreshModTree();
            RefreshModInfo();
        }

        private void LoadMods(string path)
        {
            int i = SharpTuner.activeImage.ModList.Count;
            string calid = SharpTuner.activeImage.CalId.ToString();
            string[] terms = { ".patch" };
            List<string> searchresults = ResourceUtil.directorySearch(path, terms);
            if (searchresults == null)
            {
                Console.WriteLine("NO VALID MODS FOR THIS ROM: {0}", SharpTuner.activeImage.FileName);
            }
            else
            {
                foreach (string modpath in searchresults)
                {
                    Mod tempMod = new Mod(modpath);
                    if (tempMod.TryCheckApplyMod(SharpTuner.activeImage.FilePath, SharpTuner.activeImage.FilePath + ".temp", false))
                        SharpTuner.activeImage.ModList.Add(tempMod);
                }
                if (SharpTuner.activeImage.ModList == null || SharpTuner.activeImage.ModList.Count == i)
                {
                    Console.WriteLine("NO VALID MODS FOR THIS ROM: {0}", SharpTuner.activeImage.FileName);
                }
            }
            RefreshModTree();
            RefreshModInfo();
        }

        private void RefreshModInfo()
        {
            try
            {
                selectedModTextBox.Text = "FileName: " +
                    SharpTuner.activeImage.ModList[selectedModIndex].FileName +
                    Environment.NewLine;
                selectedModTextBox.AppendText(
                    "Mod Name: " +
                    SharpTuner.activeImage.ModList[selectedModIndex].ModName +
                    Environment.NewLine);
                selectedModTextBox.AppendText(
                    "Version: " +
                    SharpTuner.activeImage.ModList[selectedModIndex].ModVersion +
                    Environment.NewLine);
                selectedModTextBox.AppendText(
                    "Author: " +
                    SharpTuner.activeImage.ModList[selectedModIndex].ModAuthor +
                    Environment.NewLine);
                string t = Regex.Replace(SharpTuner.activeImage.ModList[selectedModIndex].ModInfo, @"""""""""", @"""""");
                t = Regex.Replace(t, @"""""", Environment.NewLine);
                t = Regex.Replace(t, @"""", "");
                selectedModTextBox.AppendText(
                    "Description: " + t);
            }
            catch (System.Exception excpt)
            {
                string derp = excpt.Message;
                selectedModTextBox.Clear();
            }
        }

        private void RefreshModTree()
        {
            treeView1.Nodes.Clear();
            if (SharpTuner.activeImage.ModList.Count > 0)
            {
                treeView1.Nodes.Add("Compatible MODs for " + SharpTuner.activeImage.FileName);
                foreach (Mod mod in SharpTuner.activeImage.ModList)
                {
                    Console.WriteLine("Loaded Patch: " + mod.FileName);
                    TreeNode patchTree = new TreeNode(mod.direction + ": " + mod.FileName);
                    patchTree.Tag = mod.FilePath;

                    treeView1.Nodes.Add(patchTree);
                }
            }
            else
                treeView1.Nodes.Add("No Mods Found for " + SharpTuner.activeImage.CalId.ToString());
        }

        private void buttonPatchRom_Click(object sender, EventArgs e)
        {
            Mod currentmod = SharpTuner.activeImage.ModList[selectedModIndex];
            SaveFileDialog d = new SaveFileDialog();
            d.InitialDirectory = SharpTuner.activeImage.FilePath;
            d.Filter = "Binary/Hex files (*.bin; *.hex)|*.bin;*.hex";
            //d.ShowDialog();
            DialogResult ret = STAShowSADialog(d);

            if (ret == DialogResult.OK && d.FileName != null)
            {
                try
                {
                    if (SharpTuner.activeImage.FilePath != d.FileName)
                    {
                        System.IO.File.Copy(SharpTuner.activeImage.FilePath, d.FileName, true);
                    }
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
            if (!currentmod.TryCheckApplyMod(SharpTuner.activeImage.FilePath, d.FileName, true))
            {
                MessageBox.Show("MOD FAILED!" + System.Environment.NewLine + "See Log for details!", "RomMod", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("MOD SUCCESSFULLY APPLIED!", "RomMod", MessageBoxButtons.OK, MessageBoxIcon.Information);

                SharpTuner.fileQueued = true;
                SharpTuner.QueuedFilePath = d.FileName;
            }
        }

        private void manuallySelectPatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open dialog to change patch file location
            //Then refresh patches

            FolderBrowserDialog d = new FolderBrowserDialog();
            //d.RootFolder = Environment.SpecialFolder.MyComputer;
            string path = SharpTuner.activeImage.FileDirectory.ToString();
            d.SelectedPath = path;

            //d.ShowDialog();
            DialogResult ret = STAShowFDialog(d);

            if (ret == DialogResult.OK)
            {
                LoadMods(d.SelectedPath);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if ((treeView1.SelectedNode != null) && (treeView1.SelectedNode.Tag != null) && (treeView1.SelectedNode.Tag.ToString().Contains(".patch")))
            {
                buttonPatchRom.Enabled = true;
                //buttonTestPatch.Enabled = true;
                selectedModIndex = SharpTuner.activeImage.ModList.FindIndex(m => m.FilePath == treeView1.SelectedNode.Tag.ToString());
                RefreshModInfo();
            }
            else
            {
                buttonPatchRom.Enabled = false;
                //buttonTestPatch.Enabled = false;
            }
        }

        private void patchFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            //d.RootFolder = Environment.SpecialFolder.MyComputer;
            string path = SharpTuner.activeImage.FileDirectory.ToString();
            d.SelectedPath = path;
            //d.ShowDialog();
            DialogResult ret = STAShowFDialog(d);
            if (ret == DialogResult.OK)
            {
                LoadMods(d.SelectedPath);
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

            Dictionary<String,String> imap = SharpTuner.availableDevices.BuildInheritanceMap();
            
			foreach (String deffile in SharpTuner.availableDevices.IdentifierMap.Keys) {
				Definition.pullScalings (deffile, ref xblobscalings, ref xscalings);
			}
			Definition.pullScalings("rommetadata\\bases\\32BITBASE.xml", ref xblobscalings, ref xscalings);
			Definition.pullScalings("rommetadata\\bases\\16BITBASE.xml", ref xblobscalings, ref xscalings);
            foreach (String deffile in SharpTuner.availableDevices.IdentifierMap.Keys)
            {
                Definition.pullScalings(deffile, ref xblobscalings, ref xscalings);
            }

            foreach (XElement xbs in xblobscalings)
            {
                blobscalings.Add(xbs.Attribute("name").Value);
            }

            Definition.ConvertXML ("rommetadata\\bases\\32BITBASE.xml", ref blobscalings, ref t3d, ref t2d, ref t1d, imap, true);
			Definition.ConvertXML ("rommetadata\\bases\\16BITBASE.xml", ref blobscalings, ref t3d, ref t2d, ref t1d, imap, true);
			
            foreach (String deffile in SharpTuner.availableDevices.IdentifierMap.Keys) {
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
            //Open dialog to change patch file location
            //Then refresh patches

            FolderBrowserDialog d = new FolderBrowserDialog();
            //d.RootFolder = Environment.SpecialFolder.MyComputer;
            if (SharpTuner.activeImage != null)
            {
                string path = SharpTuner.activeImage.ToString();
                d.SelectedPath = path;
            }
            //d.ShowDialog();
            DialogResult ret = STAShowFDialog(d);

            if (ret == DialogResult.OK)
            {
                SharpTuner.definitionPath = d.SelectedPath.ToString();
                SharpTuner.populateAvailableDevices();
                loadDevices();
            } 
        }

        public class FDialogState
        {
            public DialogResult result;
            public FolderBrowserDialog dialog;
            public void ThreadProcShowDialog()
            {
                result = dialog.ShowDialog();
            }
        }

        private DialogResult STAShowFDialog(FolderBrowserDialog dialog)
        {
            FDialogState state = new FDialogState();
            state.dialog = dialog;
            System.Threading.Thread t = new System.Threading.Thread(state.ThreadProcShowDialog);
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            t.Join();
            return state.result;
        }

        public class SADialogState
        {
            public DialogResult result;
            public SaveFileDialog dialog;

            public void ThreadProcShowDialog()
            {
                result = dialog.ShowDialog();
            }
        }

        private DialogResult STAShowSADialog(SaveFileDialog dialog)
        {
            SADialogState state = new SADialogState();
            state.dialog = dialog;
            System.Threading.Thread t = new System.Threading.Thread(state.ThreadProcShowDialog);
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            t.Join();
            return state.result;
        }
    }
}
