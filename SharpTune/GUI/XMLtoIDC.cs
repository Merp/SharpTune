using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpTune;
using System.IO;

namespace SharpTune.GUI
{
    public partial class XMLtoIDC : Form
    {
        public XMLtoIDC()
        {
            InitializeComponent();
        }

        private void checkedChecker(object sender, EventArgs e)
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
                    //comboBoxEcuDef.SelectedItem
                }
                if (ExtParamsCheckBox.Checked)
                {
                    string spath = d.SelectedPath.ToString() + @"/" + SharpTuner.activeImage.CalId + @"_extparams.idc";
                    //comboBoxLoggerDef.SelectedItem
                    //comboBoxLoggerDTD.SelectedItem
                }
                if (ssmParamsCheckBox.Checked)
                {
                    if(true)//todo analyze ssm bse address
                    {
                        string spath = d.SelectedPath.ToString() + @"/" + SharpTuner.activeImage.CalId + @"_ssmparams.idc";
                        //comboBoxLoggerDef.SelectedItem
                        //comboBoxLoggerDTD.SelectedItem
                    }
                }
                d.SelectedPath.ToString();
            } 
        }

        private void ssmParamsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //TOOD: attempt to autodetect SSM base address, if found, LOCK the text input
        }

        private List<string> loggerdtds = new List<string>();
        private List<string> loggerdefs = new List<string>();
        private List<string> ecudefs = new List<string>();
        private List<string> loggerdtdfiles = new List<string>();
        private List<string> loggerdeffiles = new List<string>();
        private List<string> ecudeffiles = new List<string>();

        private void XMLtoIDC_Load(object sender, EventArgs e)
        {
            //search through defs for logger.dtd
            loggerdtds = Extensions.DirSearchCI(SharpTune.SharpTuner.RRDefRepoPath, new List<string>() { "logger.dtd" });
            Extensions.getFilePaths(loggerdtds, ref loggerdtdfiles);
            comboBoxLoggerDTD.DataSource = loggerdtdfiles;

            //search through defs for logger defs
            loggerdefs = Extensions.DirSearchCI(SharpTune.SharpTuner.RRDefRepoPath, new List<string>() { "logger" , ".xml" });
            Extensions.getFilePaths(loggerdefs, ref loggerdeffiles);
            comboBoxLoggerDef.DataSource = loggerdeffiles;

            //search through defs for ecu defs
            ecudefs = Extensions.DirSearchCI(SharpTune.SharpTuner.RRDefRepoPath, new List<string>() { ".xml" }, new List<string>() { "log" });
            Extensions.getFilePaths(ecudefs, ref ecudeffiles);
            comboBoxEcuDef.DataSource = ecudeffiles;
           
        }

    }
}
