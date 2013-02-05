using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpTune;

namespace SharpTune.GUI
{
    public partial class XMLtoIDC : Form
    {
        public XMLtoIDC()
        {
            InitializeComponent();
        }

        private void checkedChecker()
        {
            if (romTablesCheckBox.Checked || ExtParamsCheckBox.Checked || ssmParamsCheckBox.Checked)
            {
                generateIdcButton.Enabled = true;
            }
            else
                generateIdcButton.Enabled = false;
        }

        private void generateIdcButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            if (SharpTuner.activeImage != null)
            {
                string path = SharpTuner.activeImage.ToString();
                d.SelectedPath = path;
            }
            DialogResult ret = SharpTune.Extensions.STAShowFDialog(d);
            if (ret == DialogResult.OK)
            {
                //TTODO add calls to XMLTOIDC
                if (romTablesCheckBox.Checked)
                {
                    string spath = d.SelectedPath.ToString() + @"/" + SharpTuner.activeImage.CalId + @"_romtables.idc";
                }
                if (ExtParamsCheckBox.Checked)
                {
                    string spath = d.SelectedPath.ToString() + @"/" + SharpTuner.activeImage.CalId + @"_extparams.idc";
                }
                if (ssmParamsCheckBox.Checked)
                {
                    if(true)//todo analyze ssm bse address
                    {
                        string spath = d.SelectedPath.ToString() + @"/" + SharpTuner.activeImage.CalId + @"_ssmparams.idc";
                    }
                }
                d.SelectedPath.ToString();
            } 
        }

        private void ssmParamsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //TOOD: attempt to autodetect SSM base address, if found, LOCK the text input
        }
    }
}
