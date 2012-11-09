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
using NateW.Ssm;
using RomModCore;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Threading;
using System.Diagnostics;
using SharpTune.Tables;
using SharpTune.GUI;
using System.Xml.Linq;
using System.Xml;




namespace SharpTune
{
    public partial class MainWindow : Form
    {
        TextWriter _writer = null;

        OpenFileDialog ofd = new OpenFileDialog();

        public MainWindow()
        {
            InitializeComponent();       
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            // Instantiate the writer
            _writer = new TextBoxStreamWriter(txtConsole);
            // Redirect the out Console stream
            Console.SetOut(_writer);

            loadDevices();
        }

        public void loadDevices()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (senderr, ee) =>
            {
                SharpTuner.populateAvailableDevices();

                //backgroundWorker1.ReportProgress(prog);

            };
            bw.RunWorkerAsync();

            SharpTuner.imageList = new List<DeviceImage>();
            refreshPorts();
        }
      
        private void refreshPorts()
        {
            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();

            Console.WriteLine("The following serial ports were found:");

            // Display each port name to the console.
            foreach (string port in ports)
            {
                Console.WriteLine(port);
            }

            comboBoxPorts.DataSource = ports;

        }

        public void button3_Click(object sender, EventArgs e)
        {

        }

        private void textBoxDump_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonPortRefresh_Click(object sender, EventArgs e)
        {
            refreshPorts();
        }

        public void buttonDump_Click(object sender, EventArgs e)
        {

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
                modUtilityToolStripMenuItem.Enabled = true;
                obfuscateCALIDToolStripMenuItem.Enabled = true;
                SharpTuner.AddImage(newImage);

                SharpTuner.activeImage = newImage;
                this.openDeviceListBox.Items.Add(SharpTuner.activeImage.FileName);
                
                // this is useful: SharpTuner.imageList.FindIndex(f => f.FileName == newImage.FileName);

                

                ImageTreeRefresh();
                Console.WriteLine("Successfully opened " + SharpTuner.activeImage.CalId + " filename: " + SharpTuner.activeImage.FileName);


                //Update GUI Info
                //textBoxCurrentRom.Text = currentImage.FileName;
                //textBoxCalId.Text = currentImage.CalId;
                //StatusLabel.Text = "ROM CALID: " + currentImage.CalId;
                //textBoxCalIdOffset.Text = String.Format("0x{0:X}", currentImage.CalIdOffset);


        }

        public void ImageTreeRefresh()
        {
            this.imageTreeView.Nodes.Clear();
            if(SharpTuner.activeImage != null)
            {
                this.imageTreeView.Nodes.Add(SharpTuner.activeImage.imageTree.Tree);
            }
            this.imageTreeView.Refresh();
                

        }

        private void imageTreeView_DoubleClick(object sender, EventArgs e)
        {
            if (imageTreeView.SelectedNode != null)
            {
                if (imageTreeView.SelectedNode.Tag != null)
                {
                    if (imageTreeView.SelectedNode.Tag.ToString().Contains(".table"))
                    {
                        spawnTableView(imageTreeView.SelectedNode.Tag.ToString());
                    }
                }
            }
        }

        private void imageTreeView_Click(object sender, EventArgs e)
        {
            
        }



        public static void debugCheck()
        {
            if (Debugger.IsAttached)
            {


                // This gives you time to examine the output before the console window closes.
                Debugger.Break();
            }
        }

       

        private void buttonXML_Click(object sender, EventArgs e)
        {

            Backupx86(@"\OpenECU\ECUFlash\rommetadata", "ERROR: COULD NOT FIND ECUFLASH DIRECTORY","");
            Backupx86(@"\RomRaider", "ERROR: COULD NOT FIND ROMRAIDER LOGGER XML","logger.xml");

        }

        public void Backupx86(string subDir, string errorMessage, string file)
        {
            string eDir = ResourceUtil.ProgramFilesx86();

            FileIOPermission f = new FileIOPermission(FileIOPermissionAccess.Read, eDir);

            eDir += subDir;

            if (System.IO.Directory.Exists(eDir))
            {
                string eDirBak = eDir + "_backup0";
                int i = 1;
                while (System.IO.Directory.Exists(eDirBak))
                {
                    int l = eDirBak.Length - 1;
                    eDirBak = eDirBak.Substring(0, l);
                    eDirBak += i.ToString();
                    i++;
                }

                SetAccessRule(eDir);

                if(file == null)
                {
                    CopyFolder(eDir, eDirBak);
                }
                else
                {
                    File.Copy(eDir, eDirBak);
                }

                StatusLabel.Text = "XML Backup Complete";
            }

            else
            {
                MessageBox.Show(errorMessage);
                return;
            }





        }


        public static void SetAccessRule(string directory)
        {
            System.Security.AccessControl.DirectorySecurity sec = System.IO.Directory.GetAccessControl(directory);
            FileSystemAccessRule accRule = new FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName, FileSystemRights.FullControl, AccessControlType.Allow);
            sec.AddAccessRule(accRule);
        }



        static public void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void buttonTestPatch_Click(object sender, EventArgs e)
        {

        }

        private void buttonPatchRom_Click(object sender, EventArgs e)
        {
           
        }

        private void linkDonate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.romraider.com/forum/viewtopic.php?f=37&t=8143");
        }

        private void spawnTableView(string tableTag)
        {
            BackgroundWorker bw = new BackgroundWorker();

            Table table = SharpTuner.activeImage.tableList.Find(t => t.Tag == tableTag);

            bw.DoWork += (senderr, ee) =>
            {
                //Application.Run(new TableView(ref table, SharpTuner));
            };

            bw.RunWorkerCompleted += (senderr, ee) =>
                {
                    if (SharpTuner.fileQueued == true)
                    {
                        openDeviceImage(SharpTuner.QueuedFilePath);
                        SharpTuner.fileQueued = false;

                    }
                };


            bw.RunWorkerAsync();
        }
        // Declare our worker thread
        private Thread workerThread = null;

        // Boolean flag used to stop the
        //private bool stopProcess = false;

        private void modUtilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModUtility modUtil = new ModUtility(SharpTuner.activeImage, this);
            modUtil.ShowDialog();
            //BackgroundWorker bw = new BackgroundWorker();

            //bw.DoWork += (senderr, ee) =>
            //{
            //    Application.Run(new ModUtility(SharpTuner.activeImage, this));
            //};

            //bw.RunWorkerCompleted += (senderr, ee) =>
            //    {
            //        if (SharpTuner.fileQueued == true)
            //        {
            //            openDeviceImage(SharpTuner.QueuedFilePath);
            //            SharpTuner.fileQueued = false;

            //        }
            //    };


            //bw.RunWorkerAsync();

            ////this.stopProcess = false;

            ////// Initialise and start worker thread
            ////this.workerThread = new Thread(new ThreadStart(this.SpawnModUtility));
            ////this.workerThread.Start();

            
        }

        //[STAThread]
        //private void SpawnModUtility()
        //{
        //    Application.Run(new ModUtility(SharpTuner.activeImage,this));
        //}

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

            if(SharpTuner.imageList.Count == 0)
            {
                closeDeviceImageToolStripMenuItem.Enabled = false;
                modUtilityToolStripMenuItem.Enabled = false;
                obfuscateCALIDToolStripMenuItem.Enabled = false;
                return;
            }

            int index = SharpTuner.imageList.FindIndex(f=> f.FilePath == SharpTuner.activeImage.FilePath);
            SharpTuner.imageList.RemoveAt(index);
            this.openDeviceListBox.Items.RemoveAt(index);
                //this.imageTreeView.Nodes.Remove(n => n.Tag = SharpTuner.activeImage.FileName);
                foreach (TreeNode node in this.imageTreeView.Nodes)
                {
                    if (node != null && node.Tag != null && node.Tag.ToString() == SharpTuner.activeImage.FilePath)
                    {
                        node.Remove();
                    }
                }
                if (SharpTuner.imageList.Count != 0)
                {
                    SharpTuner.activeImage = SharpTuner.imageList[0];
                    ImageTreeRefresh();
                }
                else
                {
                    closeDeviceImageToolStripMenuItem.Enabled = false;
                    modUtilityToolStripMenuItem.Enabled = false;
                    obfuscateCALIDToolStripMenuItem.Enabled = false;
                }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Application.Run(new ModUtility(SharpTuner.activeImage, this));
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Application.Run(new ModUtility(SharpTuner.activeImage, this));
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
                int index = SharpTuner.imageList.FindIndex(i => i.FileName == this.openDeviceListBox.SelectedItem.ToString());
                SharpTuner.activeImage = SharpTuner.imageList[index];
            }
            ImageTreeRefresh();
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            
            
            Console.WriteLine("Opening serial port {0}.", comboBoxPorts.SelectedText);
            SerialPort port = new SerialPort(comboBoxPorts.SelectedValue.ToString(), 4800, Parity.None, 8);
            SharpTuner.Port = port;
            try
            {
                SharpTuner.Port.Open();
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine("Error opening port",excpt.Message);
            }
            if (SharpTuner.Port.IsOpen)
            {
                SsmInterface ecu = SsmInterface.GetInstance(port.BaseStream);


                Console.WriteLine("Opened");
                Console.WriteLine("Getting ECU identifier... ");
                try
                {
                    IAsyncResult result = ecu.BeginGetEcuIdentifier(null, null);
                    result.AsyncWaitHandle.WaitOne();
                    ecu.EndGetEcuIdentifier(result);
                }
                catch (System.Exception excpt)
                {
                    Console.WriteLine("Error sending init", excpt.Message );
                }
                SharpTuner.setSsmInterface(ecu);
                Console.WriteLine(ecu.EcuIdentifier);
                MessageBox.Show("Port opened! Connected to " + ecu.EcuIdentifier);
            }
            else
            {
                SharpTuner.Port.Close();
                return;
            }
        }

        private void comboBoxPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            SharpTuner.activePort = comboBoxPorts.SelectedValue.ToString();
            this.button1.Enabled = true;
        }

        private void sSMTestAppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SharpTuner.ssmInterface == null || SharpTuner.ssmInterface.EcuIdentifier == null)
            {
                MessageBox.Show("Not connected to an ECU device");
                return;
            }

            //Spawn new window here
            SSMTestApp testapp = new SSMTestApp(this);
            testapp.ShowDialog();

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

            foreach (XElement xbs in xblobscalings)
            {
                blobscalings.Add(xbs.Attribute("name").Value);
            }

            Definition.ConvertXML ("rommetadata\\bases\\32BITBASE.xml", ref blobscalings, ref t3d, ref t2d, ref t1d, imap, true);
			Definition.ConvertXML ("rommetadata\\bases\\16BITBASE.xml", ref blobscalings, ref t3d, ref t2d, ref t1d, imap, true);
			
            foreach (String deffile in SharpTuner.availableDevices.IdentifierMap.Keys) {
				Definition.ConvertXML (deffile, ref blobscalings, ref t3d, ref t2d, ref t1d, imap, false);
			}
			
	
            string filename = "rommetadata\\scaling.xml";

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
      
    }

}
