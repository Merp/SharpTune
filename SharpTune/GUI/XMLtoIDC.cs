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

//TODO: Add support for ECUFlash definitions to load ALPHA tables!!

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
                string t = Path.GetDirectoryName(SharpTuner.activeImage.FilePath);
                d.SelectedPath = t;
            }
            DialogResult ret = SharpTune.Extensions.STAShowFDialog(d);
            if (ret == DialogResult.OK)
            {
                //TODO ERROR HANDLING, CLEAR OUTPUT FILES FIRST??
                if (romTablesCheckBox.Checked && ExtParamsCheckBox.Checked && ssmParamsCheckBox.Checked)
                {
                    string spath = d.SelectedPath.ToString() + @"\" + SharpTuner.activeImage.CalId + @"_XmlToIdc.idc";
                    spath.deleteFile();
                    Console.WriteLine("Writing SSM param IDC file to " + spath);
                    NSFW.XMLtoIDC.GuiRun(new string[] { "makeall", "ecu", SharpTuner.activeImage.CalId, ssmBaseTextBox.Text }, spath, ecudefs[comboBoxEcuDef.SelectedIndex], loggerdefs[comboBoxLoggerDef.SelectedIndex], loggerdtds[comboBoxLoggerDTD.SelectedIndex]);
                }
                else
                {
                    if (romTablesCheckBox.Checked)
                    {
                        string spath = d.SelectedPath.ToString() + @"\" + SharpTuner.activeImage.CalId + @"_romtables.idc";
                        spath.deleteFile();
                        Console.WriteLine("Writing Rom Table IDC file to " + spath);
                        NSFW.XMLtoIDC.GuiRun(new string[] { "tables", SharpTuner.activeImage.CalId }, spath, ecudefs[comboBoxEcuDef.SelectedIndex], null, null);
                    }
                    if (ExtParamsCheckBox.Checked)
                    {
                        string spath = d.SelectedPath.ToString() + @"\" + SharpTuner.activeImage.CalId + @"_extparams.idc";
                        spath.deleteFile();
                        Console.WriteLine("Writing extended RAM param IDC file to " + spath);
                        NSFW.XMLtoIDC.GuiRun(new string[] { "extparam", "32", "ecu", SharpTuner.activeImage.Definition.carInfo["ecuid"].ToString() }, spath, null, loggerdefs[comboBoxLoggerDef.SelectedIndex], loggerdtds[comboBoxLoggerDTD.SelectedIndex]);
                    }
                    if (ssmParamsCheckBox.Checked)
                    {
                        if (System.Text.RegularExpressions.Regex.IsMatch(ssmBaseTextBox.Text, @"\A\b[0-9a-fA-F]+\b\Z")) //@"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"))//todo analyze ssm bse address
                        {
                            string spath = d.SelectedPath.ToString() + @"\" + SharpTuner.activeImage.CalId + @"_ssmparams.idc";
                            spath.deleteFile();
                            Console.WriteLine("Writing SSM param IDC file to " + spath);
                            NSFW.XMLtoIDC.GuiRun(new string[] { "stdparam", "32", "ecu", SharpTuner.activeImage.CalId, ssmBaseTextBox.Text }, spath, null, loggerdefs[comboBoxLoggerDef.SelectedIndex], loggerdtds[comboBoxLoggerDTD.SelectedIndex]);
                        }
                        else
                        {
                            MessageBox.Show("Invalid SSM base address! IDC write aborted!");
                            Console.Write("Invalid SSM base address! IDC write aborted!");
                        }
                    }
                }
                MessageBox.Show("Finished Writing IDC Files!");
                Console.WriteLine("Finished writing IDC Files!");
            } 
        }

        private void ssmParamsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ssmParamsCheckBox.Checked)
            {
                ssmBaseTextBox.Enabled = true;
                ssmBaseTextBox.Text = getSSMBase();
            }
            else
            {
                ssmBaseTextBox.Enabled = false;
                ssmBaseTextBox.Text = "Enter SSM Base Address";
            }
        }

        private string getSSMBase()
        {
            byte[] byc = new byte[4];
            long highlimit = 5000000;
            long lowlimit = 100000;
            long difflimit = 100000;
            if(SharpTuner.activeImage.imageStream.Length < highlimit)
                highlimit = SharpTuner.activeImage.imageStream.Length;
            for (long i = lowlimit; i < highlimit; i += 4)
            {
                long start = i;
                SharpTuner.activeImage.imageStream.Seek(i, SeekOrigin.Begin);
                if (SSMBaseRecursion(i, 0, 0, difflimit))
                    return start.ToString("X");
                else
                    continue;
            }
            difflimit += 40000;
            for (long i = lowlimit; i < highlimit; i += 4)
            {
                long start = i;
                SharpTuner.activeImage.imageStream.Seek(i, SeekOrigin.Begin);
                if (SSMBaseRecursion(i, 0, 0, difflimit))
                    return start.ToString("X");
                else
                    continue;
            }

            return "Enter SSM Base";
        }

        private bool SSMBaseRecursion(long currentoffset, int lastaddress, int recursionlevel, long min)
        {
            int addinc;
            if (recursionlevel < 6)
                addinc = 17;
            else
                addinc = 1000;
            byte[] byc = new byte[4];
            int bc = 0;
            SharpTuner.activeImage.imageStream.Read(byc, 0, 4);

            byc.ReverseBytes();
            bc = BitConverter.ToInt32(byc,0);
            if(recursionlevel == 0)
                lastaddress = bc;
            if(bc > 0 && Math.Abs(currentoffset - bc) < min && lastaddress + addinc > bc)
            {
                if (recursionlevel > 40)
                    return true;
                recursionlevel++;
                currentoffset += 4;
                return SSMBaseRecursion(currentoffset, bc, recursionlevel, min);
            }
            else
                return false;
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
                    rominfo += "FileName:  " + SharpTuner.activeImage.FileName + System.Environment.NewLine;
                    foreach (var s in SharpTuner.activeImage.Definition.carInfo)
                    {
                        rominfo += s.Key.ToString() + ":  " + s.Value.ToString() + System.Environment.NewLine;
                    }
                }
                catch (Exception er)
                {
                    Console.Write(er.Message);
               
            }

            RomInfoTextBox.Text = rominfo;

            //search through defs for logger.dtd
            loggerdtds = Extensions.DirSearchCI(SharpTune.SharpTuner.RRDefRepoPath, new List<string>() { "logger.dtd" });
            Extensions.getFilePaths(loggerdtds, ref loggerdtdfiles);
            comboBoxLoggerDTD.DataSource = loggerdtdfiles;

            //search through defs for logger defs
            loggerdefs = Extensions.DirSearchCI(SharpTune.SharpTuner.RRDefRepoPath, new List<string>() { "logger" , ".xml" });
            Extensions.getFilePaths(loggerdefs, ref loggerdeffiles);
            comboBoxLoggerDef.DataSource = loggerdeffiles;
            foreach (string d in loggerdeffiles)
            {
                if (d.ContainsCI("en") && d.ContainsCI("std"))
                    comboBoxLoggerDef.SelectedItem = d;
            }

            //search through defs for ecu defs
            ecudefs = Extensions.DirSearchCI(SharpTune.SharpTuner.RRDefRepoPath, new List<string>() { ".xml" }, new List<string>() { "log" });
            Extensions.getFilePaths(ecudefs, ref ecudeffiles);
            comboBoxEcuDef.DataSource = ecudeffiles;
            foreach (string d in ecudeffiles)
            {
                
                if(d.ContainsCI("ecu_defs")) ///TODO update RR repo and add search for "STD"
                    comboBoxEcuDef.SelectedItem = d;
                else if (d.ContainsCI(SharpTuner.activeImage.CalId))
                {
                    comboBoxEcuDef.SelectedItem = d;
                    break; //gives precedence to def with CALID
                }
            }
        }

    }
}
