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
using NateW.Ssm;
using Merp;

namespace SharpTune.GUI
{
    public partial class SSMTestApp : Form
    {
        public MainWindow currentWindow { get; set; }
        public int address { get; set; }
        public int length { get; set; }

        public SSMTestApp(MainWindow window)
        {
            this.currentWindow = window;
            InitializeComponent();
        }

        private void SSMTestApp_Load(object sender, EventArgs e)
        {
            // List info about the connected device
            // Display fields/buttons to send SSM read/write commands
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void modeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Check for proper values!
            switch (this.modeBox.SelectedIndex)
            {
                case 0:
                    //call init routine
                    ssmInit();
                    break;

                case 1:
                    //call read routine
                    ssmReadSSM();
                    break;

                case 2:
                    ssmReadBlock();
                    break;


                case 3:
                    //call write routine
                    ssmWriteBlock();
                    break;

                default:
                    break;

            }

        }
        private void ssmInit()
        {

            SsmInterface ecu = SharpTuner.ssmInterface;
            try
            {
                IAsyncResult result = ecu.BeginGetEcuIdentifier(null, null);
                result.AsyncWaitHandle.WaitOne();
                ecu.EndGetEcuIdentifier(result);
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine("Error sending init", excpt.Message);
            }

            Console.WriteLine(ecu.EcuIdentifier);
            MessageBox.Show("Init received from " + ecu.EcuIdentifier);
        }

        private void ssmReadSSM()
        {
            SsmInterface ecu = SharpTuner.ssmInterface;
            this.valueBox.AppendText("Reading SSM Param " + this.address.ToString());
            this.valueBox.AppendText(Environment.NewLine);
            List<int> addresses = new List<int>();
            addresses.Add(this.address);
            IAsyncResult result = ecu.BeginMultipleRead(addresses, null, null);
            result.AsyncWaitHandle.WaitOne();
            byte[] readbytes = ecu.EndMultipleRead(result);
            this.valueBox.AppendText(BitConverter.ToString(readbytes));
            
        }

        private void ssmReadBlock()
        {
            SsmInterface ecu = SharpTuner.ssmInterface;
            this.valueBox.AppendText("Reading " + this.length + " bytes from address " + this.address);
            this.valueBox.AppendText(Environment.NewLine);
            IAsyncResult result = ecu.BeginBlockRead(this.address,this.length, null, null);
            result.AsyncWaitHandle.WaitOne();
            byte[] readbytes = ecu.EndBlockRead(result);
            this.valueBox.AppendText(BitConverter.ToString(readbytes));
            this.valueBox.AppendText(Environment.NewLine);

        }

        private void ssmWriteBlock()
        {
            byte[] bytes = this.valueBox.Text.ToString().ToByteArray();
            SsmInterface ecu = SharpTuner.ssmInterface;
            this.valueBox.AppendText("Writing " + this.length + " bytes from address " + this.address);
            this.valueBox.AppendText(Environment.NewLine);
            IAsyncResult result = ecu.BeginBlockWrite(this.address, bytes, null, null);
            result.AsyncWaitHandle.WaitOne();
            byte[] readbytes = ecu.EndBlockWrite(result);
            this.valueBox.AppendText(BitConverter.ToString(readbytes));
            this.valueBox.AppendText(Environment.NewLine);
        }


        private void addressBox_TextChanged(object sender, EventArgs e)
        {
            if(addressBox.Text.Length == 6)
            {
                try
                {
                    this.address = System.Int32.Parse(this.addressBox.Text.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
                }
                catch
                {
                    MessageBox.Show("Bad address!");
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.length = int.Parse(textBox1.Text.ToString());
            }
            catch
            {
                MessageBox.Show("Problem with length!");
            }
        }
    }
}
