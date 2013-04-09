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
        public OpenFileDialog ofd = new OpenFileDialog();

        public MapToDef()
        {
            InitializeComponent();
        }

        private void buttonMap_Click(object sender, EventArgs e)
        {
            string ecuid;
            if (SharpTuner.ActiveImage != null && SharpTuner.ActiveImage.Definition.CarInfo.ContainsKey("ecuid"))
                ecuid = SharpTuner.ActiveImage.Definition.CarInfo["ecuid"].ToString();
            else
                ecuid = SimplePrompt.ShowDialog("Enter ECU Identifier (logger identifier)", "Enter EcuId");
            ofd.Filter = "MAP Files (*.map)|*.map";
            DialogResult res = Utils.STAShowOFDialog(ofd);
            if (res == DialogResult.OK)
            {
                //try
                //{
                    SharpTuner.ActiveImage.Definition.ImportMapFile(ofd.FileName,SharpTuner.ActiveImage);//TODO: clean up creation of XML whitespace sucks ass.
                    SharpTuner.ActiveImage.Definition.ExportXML();
                    ModDefinition.DefineRRLogEcuFromMap(ofd.FileName, ecuid);//TODO: import RR stuff to definnition class and deprecate this??
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
          if (SharpTuner.ActiveImage != null && SharpTuner.ActiveImage.Definition.CarInfo.ContainsKey("ecuid"))
                ecuid = SharpTuner.ActiveImage.Definition.CarInfo["ecuid"].ToString();
            else
                ecuid = SimplePrompt.ShowDialog("Enter ECU Identifier (logger identifier)", "Enter EcuId");
            try
            {
                ModDefinition.DefineRRLogEcuFromText(textBox1.Text, ecuid);
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
