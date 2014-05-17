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
using System.Text.RegularExpressions;
using System.Diagnostics;
using EcuMapTools;
using SharpTuneCore;
using SharpTune.Properties;

//TODO: Add support for ECUFlash definitions to load ALPHA tables!!

namespace SharpTune.GUI
{
    public partial class XMLtoIDCGUI : Form
    {
        private readonly ECU deviceImage;
        private readonly XMLtoIDC xmlConv;

        public XMLtoIDCGUI(ECU di)
        {
            deviceImage = di;
            xmlConv = new XMLtoIDC(deviceImage);
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
            if (deviceImage != null)
            {
                string t = Path.GetDirectoryName(deviceImage.FilePath);
                d.SelectedPath = t;
            }
            DialogResult ret = SharpTune.Utils.STAShowFDialog(d);
            if (ret == DialogResult.OK)
            {
                if (romTablesCheckBox.Checked && ExtParamsCheckBox.Checked && ssmParamsCheckBox.Checked)
                {
                    if (checkBoxUseDef.Checked)
                        xmlConv.MakeAll(ssmBaseTextBox.Text, loggerdefs[comboBoxLoggerDef.SelectedIndex], loggerdtds[comboBoxLoggerDTD.SelectedIndex]);
                    else
                        xmlConv.MakeAll(ssmBaseTextBox.Text, ecudefs[comboBoxEcuDef.SelectedIndex], loggerdefs[comboBoxLoggerDef.SelectedIndex], loggerdtds[comboBoxLoggerDTD.SelectedIndex]);
                }
                else
                {
                    if (romTablesCheckBox.Checked)
                    {
                        if (checkBoxUseDef.Checked)
                            xmlConv.MakeRomTables();
                        else
                            xmlConv.MakeRomTables(ecudefs[comboBoxEcuDef.SelectedIndex]);
                    }
                    if (ExtParamsCheckBox.Checked)
                    {
                        xmlConv.MakeExtParams(loggerdefs[comboBoxLoggerDef.SelectedIndex], loggerdtds[comboBoxLoggerDTD.SelectedIndex]);
                        //NSFW.XMLtoIDC.GuiRun(new string[] { "extparam", "32", "ecu", SharpTuner.ActiveImage.Definition.CarInfo["ecuid"].ToString() }, spath, null, loggerdefs[comboBoxLoggerDef.SelectedIndex], loggerdtds[comboBoxLoggerDTD.SelectedIndex]);
                    }
                    if (ssmParamsCheckBox.Checked)
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(ssmBaseTextBox.Text, @"\A\b[0-9a-fA-F]+\b\Z")) //@"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"))//todo analyze ssm bse address
                        {
                            string spath = d.SelectedPath.ToString() + @"\" + deviceImage.CalId + @"_ssmparams.idc";
                            spath.deleteFile();
                            Trace.WriteLine("Writing SSM param IDC file to " + spath);
                            xmlConv.MakeStdParams(ssmBaseTextBox.Text, loggerdefs[comboBoxLoggerDef.SelectedIndex], loggerdtds[comboBoxLoggerDTD.SelectedIndex]);
                            //NSFW.XMLtoIDC.GuiRun(new string[] { "stdparam", "32", "ecu", SharpTuner.ActiveImage.CalId, ssmBaseTextBox.Text }, spath, null, loggerdefs[comboBoxLoggerDef.SelectedIndex], loggerdtds[comboBoxLoggerDTD.SelectedIndex]);
                        }
                        else
                        {
                            MessageBox.Show("Invalid SSM base address! IDC write aborted!");
                            Trace.WriteLine("Invalid SSM base address! IDC write aborted!");
                        }
                    }
                }
                MessageBox.Show("Finished Writing IDC Files!");
                Trace.WriteLine("Finished writing IDC Files!");
            } 
        }

        private void ssmParamsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ssmParamsCheckBox.Checked)
            {
                ssmBaseTextBox.Enabled = true;
                ssmBaseTextBox.Text = xmlConv.GetSSMBase();
            }
            else
            {
                ssmBaseTextBox.Enabled = false;
                ssmBaseTextBox.Text = "Enter SSM Base Address";
            }
        }

        private List<string> loggerdtds = new List<string>();
        private List<string> loggerdefs = new List<string>();
        private List<string> ecudefs = new List<string>();
        private List<string> loggerdtdfiles = new List<string>();
        private List<string> loggerdeffiles = new List<string>();
        private List<string> ecudeffiles = new List<string>();
        private string rominfo;

        private void XMLtoIDC_Load(object sender, EventArgs e)
        {
            //Populate rom info section
           
                try
                {
                    //rominfo = SharpTuner.activeImage.Definition.carInfo.ToString() + System.Environment.NewLine;

                    if (deviceImage.Definition != null)
                    {
                        checkBoxUseDef.Enabled = true;
                        checkBoxUseDef.Checked = true;
                        comboBoxEcuDef.Enabled = false;
                    }

                    rominfo += "FileName:  " + deviceImage.FileName + System.Environment.NewLine;
                    foreach (var s in deviceImage.Definition.ident.EcuFlashXml.Elements())//TODO: use a dictionary instead.
                    {
                        rominfo += s.Name.ToString() + ":  " + s.Value.ToString() + System.Environment.NewLine;
                    }
                }
                catch (Exception er)
                {
                    Console.Write(er.Message);
               
            }

            RomInfoTextBox.Text = rominfo;

            //search through defs for logger.dtd
            loggerdtds = Utils.DirSearchCI(Settings.Default.RomRaiderDefRepoPath, new List<string>() { "logger.dtd" });
            Utils.getFilePaths(loggerdtds, ref loggerdtdfiles);
            comboBoxLoggerDTD.DataSource = loggerdtdfiles;

            //search through defs for logger defs
            loggerdefs = Utils.DirSearchCI(Settings.Default.SubaruDefsRepoPath, new List<string>() { "logger" , ".xml" });
            Utils.getFilePaths(loggerdefs, ref loggerdeffiles);
            comboBoxLoggerDef.DataSource = loggerdeffiles;
            foreach (string d in loggerdeffiles)
            {
                if (d.ContainsCI("en") && d.ContainsCI("std"))
                    comboBoxLoggerDef.SelectedItem = d;
            }

            //search through defs for ecu defs
            ecudefs = Utils.DirSearchCI(Settings.Default.RomRaiderDefRepoPath, new List<string>() { ".xml" }, new List<string>() { "log" });
            Utils.getFilePaths(ecudefs, ref ecudeffiles);
            comboBoxEcuDef.DataSource = ecudeffiles;
            foreach (string d in ecudeffiles)
            {
                
                if(d.ContainsCI("ecu_defs")) ///TODO update RR repo and add search for "STD"
                    comboBoxEcuDef.SelectedItem = d;
                else if (d.ContainsCI(deviceImage.CalId))
                {
                    comboBoxEcuDef.SelectedItem = d;
                    break; //gives precedence to def with CALID
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUseDef.Checked == false)
                comboBoxEcuDef.Enabled = true;
            else
                comboBoxEcuDef.Enabled = false;
        }

    }
}
