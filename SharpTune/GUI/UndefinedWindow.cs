using SharpTune.Properties;
using SharpTuneCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SharpTune.GUI
{
    public partial class UndefinedWindow : Form
    {
        private readonly SharpTuner sharpTuner;

        private ECUMetaData def;
        private string filePath;
        private List<string> defList;

        public UndefinedWindow(SharpTuner st, string f)
        {
            sharpTuner = st;
            InitializeComponent();
            filePath = f;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void UndefinedWindow_Load(object sender, EventArgs e)
        {
            defList = new List<string>(sharpTuner.AvailableDevices.IdentList.OrderBy(x => x.ToString()).ToList());
            def = new ECUMetaData(sharpTuner.AvailableDevices);
            List<string> dss = new List<string>();
            dss.Add("DEFAULT");
            dss.AddRange(defList);
            comboBoxCopyDef.DataSource = dss;

            List<string> iss = new List<string>();
            iss.AddRange(defList);
            comboBoxIncludeDef.DataSource = iss;

            textBoxDefXml.Text = defaultShortDef;
        }

        private string defaultShortDef = @"<romid>
               <xmlid></xmlid>
               <internalidaddress></internalidaddress>
               <internalidstring></internalidstring>
               <ecuid></ecuid>
               <year></year>
               <market></market>
               <make></make>
               <model></model>
               <submodel></submodel>
               <transmission></transmission>
               <memmodel></memmodel>
               <flashmethod></flashmethod>
               <checksummodule></checksummodule>
              </romid>";

        private void comboBoxCopyDef_SelectedValueChanged(object sender, EventArgs e)
        {
            updateXml();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //TODO put this in definition class!
            try
            {
                XElement xe = XElement.Parse(textBoxDefXml.Text);
                def.ident.ParseEcuFlashXml(xe,def.include);
            }
            catch (Exception er)
            {
                MessageBox.Show("Error: " + er.Message);
                return;
            }

            using (FileStream fileStream = File.OpenRead(this.filePath))
            {
                MemoryStream memStream = new MemoryStream();
                memStream.SetLength(fileStream.Length);
                fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);

                memStream.Seek(def.calibrationIdAddress, SeekOrigin.Begin);

                byte[] b = new byte[8];
                memStream.Read(b, 0, 8);
                string id = System.Text.Encoding.UTF8.GetString(b);
                DialogResult dialogResult = MessageBox.Show("Found Identifier: " + id +". Use this??", "Identifier", MessageBoxButtons.YesNo);
                if(dialogResult == DialogResult.Yes)
                {
                    def.ident.setIdForUndefined(id);
                    textBoxDefXml.Text = def.ident.EcuFlashXml_SH705x.ToString();

                }
                else if (dialogResult == DialogResult.No)
                {
                    return;
                }
            } 
        }

        private void comboBoxIncludeDef_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sharpTuner.AvailableDevices.DefDictionary.ContainsKey(comboBoxIncludeDef.SelectedItem.ToString()))
                def.ident.include = comboBoxIncludeDef.SelectedItem.ToString();
            else
                def.ident.include = null;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        { 
            if (checkBox1.Checked && sharpTuner.AvailableDevices.DefDictionary.ContainsKey(comboBoxCopyDef.SelectedItem.ToString()))
            {
                sharpTuner.AvailableDevices.DefDictionary[comboBoxCopyDef.SelectedItem.ToString()].Populate();
                def.CopyTables(sharpTuner.AvailableDevices.DefDictionary[comboBoxCopyDef.SelectedItem.ToString()]);//copy tables
            }
            //Save the definition XML
            try
            {
                XElement xe = XElement.Parse(textBoxDefXml.Text);
                def.ident.ParseEcuFlashXml(xe,comboBoxIncludeDef.SelectedValue.ToString());
            
            StringBuilder path = new StringBuilder();
            path.Append(Settings.Default.EcuFlashDefRepoPath + "/");
            if (def.ident.model != null)
            {
                path.Append(def.ident.model.ToString());
                if (def.ident.submodel != null)
                {
                    string s = " " + def.ident.submodel;
                    path.Append(s);
                }
                path.Append("/");
            }
            string dirpath = path.ToString();
            path.Append(def.calibrationlId.ToString() + ".xml");
            if (!Directory.Exists(dirpath))
            {
                Directory.CreateDirectory(dirpath);
            }
            else if (File.Exists(path.ToString()))
            {
                DialogResult dialogResult = MessageBox.Show("Definition already exists at " + path.ToString() + System.Environment.NewLine + "Overwrite it??", "Warning", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                {
                    MessageBox.Show("Save definition aborted");
                    return;
                }
            }
            def.filePath = path.ToString();
            def.ExportEcuFlashXML();
            MessageBox.Show("Successfully saved definition to " + def.filePath);
            sharpTuner.PopulateAvailableDevices();
            this.Dispose();
            }
            catch (Exception er)
            {
                MessageBox.Show("Error: " + er.Message);
                return;
            }
        }

        private void comboBoxCopyDef_TextChanged(object sender, EventArgs e)
        {
            //TODO: this is bullshit, fix findDef();
        }

        private void findDef()
        {
            string s = null;
            if (defList.ContainsCI(comboBoxCopyDef.Text.ToString()))
            {
                s = defList.Find(x => x.ToString().ContainsCI(comboBoxCopyDef.Text.ToString()));
                if (s != null && s != comboBoxCopyDef.Text)
                {
                    comboBoxCopyDef.Text = s;
                }
                comboBoxCopyDef.SelectedValue = s;
            }
            updateXml();
        }

        private void updateXml()
        {
            if (sharpTuner.AvailableDevices.DefDictionary.ContainsKey(comboBoxCopyDef.SelectedItem.ToString()))
            {
                textBoxDefXml.Text = sharpTuner.AvailableDevices.DefDictionary[comboBoxCopyDef.SelectedItem.ToString()].ident.EcuFlashXml_SH705x.ToString();
                comboBoxIncludeDef.SelectedItem = comboBoxCopyDef.SelectedItem;
            }
            else
                textBoxDefXml.Text = defaultShortDef;
        }
    }
}
