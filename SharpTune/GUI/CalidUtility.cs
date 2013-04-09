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
using System.Collections;
using SharpTune.RomMod;
using SharpTuneCore;

namespace SharpTune
{
    public partial class CalidUtility : Form
    {
        private DeviceImage currentImage { get; set; }
        private MainWindow mainWindow { get; set; }

        public CalidUtility(MainWindow window)
        {
            this.mainWindow = window;
            this.currentImage = SharpTuner.ActiveImage;
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void CalidUtility_Load(object sender, EventArgs e)
        {
            oldcalidbox.Text = currentImage.CalId;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string randomstring = RandomString(currentImage.CalId.Length);
            newcalidbox.Text = randomstring;
        }

        private static Random random = new Random((int)DateTime.Now.Ticks);//thanks to McAden
        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (newcalidbox.Text.Length != this.currentImage.CalId.Length)
            {
                MessageBox.Show("ID is not long enough!", "RomMod", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (SharpTuner.AvailableDevices.IdentList.ContainsCI(newcalidbox.Text.ToString()))
            {
                MessageBox.Show("ID is already defined!", "RomMod", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SaveFileDialog d = new SaveFileDialog();
            d.InitialDirectory = this.currentImage.FilePath;
            d.Filter = "Binary/Hex files (*.bin; *.hex)|*.bin;*.hex";
            DialogResult ret = STAShowSADialog(d);

                if (d.FileName != null)
                {
                    try
                    {
                        FileStream romStream = new FileStream(d.FileName, FileMode.OpenOrCreate, FileAccess.Write);

                        currentImage.imageStream.Seek(0, SeekOrigin.Begin);
                        currentImage.imageStream.CopyTo(romStream, 1024);
                        romStream.Seek(this.currentImage.CalIdOffset, SeekOrigin.Begin);
                        string newcalid = newcalidbox.Text.ToString();
                        //byte[] bytes = new byte[newcalid.Length * sizeof(char)];

                        byte[] bytes = Encoding.ASCII.GetBytes(newcalid);


                        romStream.Write(bytes, 0, bytes.Length);

                        romStream.Close();

                        //System.IO.File.Copy(this.currentImage.FilePath, d.FileName, true);
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
            this.Close();
        }
    }
}
