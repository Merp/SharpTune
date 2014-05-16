using SharpTune.RomMod;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SharpTune.GUI
{
    public partial class MapToDef : Form
    {
        private readonly SharpTuner sharpTuner;
        public OpenFileDialog ofd = new OpenFileDialog();

        public MapToDef(SharpTuner st)
        {
            sharpTuner = st;
            InitializeComponent();
        }

        private void buttonMap_Click(object sender, EventArgs e)
        {
            string ecuid;
            if (sharpTuner.activeImage != null && sharpTuner.activeImage.Definition.EcuId != null)
                ecuid = sharpTuner.activeImage.Definition.EcuId.ToString();
            else
                ecuid = SimplePrompt.ShowDialog("Enter ECU Identifier (logger identifier)", "Enter EcuId");
            ofd.Filter = "MAP Files (*.map)|*.map";
            DialogResult res = Utils.STAShowOFDialog(ofd);
            if (res == DialogResult.OK)
            {
                //try
                //{
                sharpTuner.activeImage.Definition.ImportMapFile(ofd.FileName, sharpTuner.activeImage);//TODO: clean up creation of XML whitespace sucks ass.
                sharpTuner.activeImage.Definition.ExportEcuFlashXML();
                ModDefinition md = new ModDefinition(sharpTuner.AvailableDevices, null); //TODO: major KLUDGE
                   md.DefineRRLogEcuFromMap(ofd.FileName, ecuid);//TODO: import RR stuff to definnition class and deprecate this??
                    MessageBox.Show("Success!");
                    this.Close();
                    return;
                //catch (Exception err)
               // {
               //     MessageBox.Show(err.Message);
               // }
            }
            this.Close();
            return;
        }

        private void MapToDef_Load(object sender, EventArgs e)
        {

        }

        private void buttonText_Click(object sender, EventArgs e)
        {
            string ecuid;
            if (sharpTuner.activeImage != null && sharpTuner.activeImage.Definition.EcuId != null)
                ecuid = sharpTuner.activeImage.Definition.EcuId;
            else
                ecuid = SimplePrompt.ShowDialog("Enter ECU Identifier (logger identifier)", "Enter EcuId");
            try
            {
                sharpTuner.activeImage.Definition.ImportMapText(textBox1.Text, sharpTuner.activeImage);//TODO: clean up creation of XML whitespace sucks ass.
                sharpTuner.activeImage.Definition.ExportEcuFlashXML();
                ModDefinition md = new ModDefinition(sharpTuner.AvailableDevices, null);
                md.DefineRRLogEcuFromText(textBox1.Text, ecuid);
                MessageBox.Show("Success!");
                this.Close();
                return;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            this.Close();
            return;
        }
    }
}
