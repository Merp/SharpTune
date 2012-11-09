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

namespace SharpTune
{
    public partial class ModUtility : Form
    {
        private DeviceImage currentImage { get; set; }

        private int selectedIndex { get; set; }

        private MainWindow currentMainWindow { get; set; }

        public ModUtility(DeviceImage image, MainWindow window)
        {
            this.currentImage = image;
            this.currentMainWindow = window;
            selectedIndex = 0;
            InitializeComponent();
        }

        private void ModUtility_Load(object sender, EventArgs e)
        {

            LoadPatches(Directory.GetCurrentDirectory());
          
        }

        [STAThread]
        private void patchFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open dialog to change patch file location
            //Then refresh patches

            FolderBrowserDialog d = new FolderBrowserDialog();
            //d.RootFolder = Environment.SpecialFolder.MyComputer;
            string path = this.currentImage.FileDirectory.ToString();
            d.SelectedPath = path;

             //d.ShowDialog();
            DialogResult ret = STAShowFDialog(d);

            if (ret == DialogResult.OK)
            {
                LoadPatches(d.SelectedPath);
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

        private void LoadPatches(string path)
        {
            treeView1.Nodes.Clear();

            if (!this.currentImage.getValidMods(path))
            {
                Console.WriteLine("NO VALID MODS FOR THIS ROM: {0}", this.currentImage.FileName);
                return;
            }

            treeView1.Nodes.Add("Compatible MODs for " + this.currentImage.FileName);

            // update treenode
            if (this.currentImage.ModList != null)
            {
                foreach (ModInfo mod in this.currentImage.ModList)
                {
                    Console.WriteLine("Loaded Patch: " + mod.FileName);
                    TreeNode patchTree = new TreeNode(mod.direction + ": " + mod.FileName);
                    patchTree.Tag = mod.FilePath;
                    
                    treeView1.Nodes.Add(patchTree);
                }
            }
           
      }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
        }

        private void treeView1_Click(object sender, EventArgs e)
        {
            
        
        }

        private void buttonTestPatch_Click(object sender, EventArgs e)
        {

            /// string[] command = new string[] { "test", this.currentImage.ModList[comboBoxPatches.SelectedIndex].FilePath , this.currentImage.FilePath };

            // Thread t = new Thread( () => RomPatchThread (command) );

            // t.Start();
            // need to fix console output to work with threads!
            //

            //if (RomModCore.Program.Main(command) == 1)
            if (!RomModCore.Program.ModTest(this.currentImage.ModList[selectedIndex].FilePath, this.currentImage.FilePath, this.currentImage.ModList[selectedIndex].isApplied))
            {
                MessageBox.Show("INVALID Patch File!" + System.Environment.NewLine + "See Log for details!", "RomMod", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonPatchRom.Enabled = false;

            }
            else
            {
                MessageBox.Show("MOD TEST SUCCESS", "RomMod", MessageBoxButtons.OK, MessageBoxIcon.Information);
                buttonPatchRom.Enabled = true;
                
            }
             }

        private void buttonPatchRom_Click(object sender, EventArgs e)
        {
            ModInfo currentmod = this.currentImage.ModList[selectedIndex];
            SaveFileDialog d = new SaveFileDialog();
            d.InitialDirectory = this.currentImage.FilePath;
            d.Filter = "Binary/Hex files (*.bin; *.hex)|*.bin;*.hex";
            //d.ShowDialog();
            DialogResult ret = STAShowSADialog(d);

            if (ret == DialogResult.OK && d.FileName != null)
            {
                try
                {
                    if (this.currentImage.FilePath != d.FileName)
                    {
                        System.IO.File.Copy(this.currentImage.FilePath, d.FileName, true);
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
            

            //string[] command = new string[] { "apply", this.currentImage.ModList[comboBoxPatches.SelectedIndex].FilePath, this.currentImage.FilePath };

            //if (RomModCore.Program.Main(command) == 1)
            if (!RomModCore.Program.TryApply(currentmod.FilePath, d.FileName, !currentmod.isApplied, true))
            {
                MessageBox.Show("MOD FAILED!" + System.Environment.NewLine + "See Log for details!", "RomMod", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("MOD SUCCESSFULLY APPLIED!", "RomMod", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SharpTuner.fileQueued = true;
                SharpTuner.QueuedFilePath = d.FileName;
            }

            this.Close();
            

        }

        private void treeView1_DockChanged(object sender, EventArgs e)
        {
            
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if ((treeView1.SelectedNode != null) && (treeView1.SelectedNode.Tag != null) && (treeView1.SelectedNode.Tag.ToString().Contains(".patch")))
            {
                buttonPatchRom.Enabled = true;
                buttonTestPatch.Enabled = true;
                selectedIndex = this.currentImage.ModList.FindIndex(m => m.FilePath == treeView1.SelectedNode.Tag.ToString());
                selectedModTextBox.Text = this.currentImage.ModList[selectedIndex].FileName;
            }
            else
            {
                buttonPatchRom.Enabled = false;
                buttonTestPatch.Enabled = false;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }



    }


}
