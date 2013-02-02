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
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;

namespace ConsoleRedirection
{
    public class TextBoxStreamWriter : TextWriter
    {
        TextBox _output = null;
        private readonly object writerLock = new object();
        private string stringqueue { get; set; }


        public TextBoxStreamWriter(TextBox output)
        {
            this._output = output;
        }


        public override void Write(char value)
        {

             //base.Write(value);

            this.stringqueue += value.ToString();

            string newl = this.NewLine;

            if (this.stringqueue != null && this.stringqueue.ToString().Contains(this.NewLine.ToString()) )
            {

                BackgroundWorker backgroundwriter = new BackgroundWorker();

                string towrite = this.stringqueue;
                this.stringqueue = null;

                backgroundwriter.DoWork += (senderr, ee) =>
                {

                    if (this._output.InvokeRequired)
                    {
                        Action<TextBox, string> action = new Action<TextBox, string>((txb, str) =>
                            {
                                    txb.AppendText(str);
                            });

                        
                        lock (this._output)
                        {
                            this._output.BeginInvoke(action, new object[] { this._output, towrite });
                        }

                        //
                    }
                    else
                    {
                        try
                        {
                            this._output.AppendText(value.ToString()); // When character data is written, append it to the text box.
                        }
                        catch
                        {
                            //do nothing
                        }
                    }

                };

                backgroundwriter.RunWorkerAsync();
                
            }

           
        }


        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }



    }
}
