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
            if (SharpTuner.activeImage != null && SharpTuner.activeImage.Definition.carInfo.ContainsKey("ecuid"))
                ecuid = SharpTuner.activeImage.Definition.carInfo["ecuid"].ToString();
            else
                ecuid = SimplePrompt.ShowDialog("Enter ECU Identifier (logger identifier)", "Enter EcuId");
            ofd.Filter = "MAP Files (*.map)|*.map";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DefCreator.DefineRRLogEcuFromMap(ofd.FileName, ecuid);
                    MessageBox.Show("Success!");
                    this.Close();
                    return;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
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
          if (SharpTuner.activeImage != null && SharpTuner.activeImage.Definition.carInfo.ContainsKey("ecuid"))
                ecuid = SharpTuner.activeImage.Definition.carInfo["ecuid"].ToString();
            else
                ecuid = SimplePrompt.ShowDialog("Enter ECU Identifier (logger identifier)", "Enter EcuId");
            try
            {
                DefCreator.DefineRRLogEcuFromText(textBox1.Text, ecuid);
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
