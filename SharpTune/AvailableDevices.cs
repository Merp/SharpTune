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
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpTune;
using System.Diagnostics;
using System.Threading;
using System.Reflection;

namespace SharpTuneCore
{
    public class AvailableDevices
    {
     
        public Dictionary<string, Definition> DefDictionary {get; private set;}

        public List<string> IdentList { get; private set; }

        public int DeviceCount { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AvailableDevices()
        {
            DefDictionary = new Dictionary<string,Definition>();
            IdentList = new List<string>();
            this.DeviceCount = 0;
        }

        public void Populate(string xmldir)
        {
            try
            {
                //wtf is this TODO
                List<string> ts = ResourceUtil.directorySearchRecursiveDir(xmldir,null);
                if(!GetDevices(ts[0]))
                {
                    Trace.WriteLine("XML initialize failed");
                    DialogResult deferr = MessageBox.Show(
                        "Error initializing definitions! Please download the appropriate definitions and point the settings to the git repo base directory!" + Environment.NewLine +
                        "Would you like to download the latest definitions??",
                        "Error loading definitions",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1,
                        0);

                    if (deferr == DialogResult.Yes)
                        Process.Start(SharpTuner.GitHelpUrl);
                        
                }
                else
                {
                    //MessageBox.Show("finished populating available devices");
                    Trace.WriteLine("Successfully loaded " + DeviceCount + "XML definitions!");
                }
                
            }
            catch (System.Exception excpt)
            {
                Trace.WriteLine(excpt.Message);
            }
            
        }

        //TODO, populate entire map instead of just finding base??? use lists?
        public Dictionary<String, String> BuildInheritanceMap()
        {
            Dictionary<String, String> imap = new Dictionary<String, String>();

            foreach (KeyValuePair<String, Definition> pair in this.DefDictionary)
            {
                imap.Add(pair.Value.defPath, findInherit(pair.Key));
            }
            return imap;
        }

        public String findInherit(String xmlid)
        {
            String fetchpath = getDefPath(xmlid);
            XDocument xmlDoc = XDocument.Load(fetchpath, LoadOptions.PreserveWhitespace);
            XElement inc = xmlDoc.XPathSelectElement("/rom/include");
            if (inc != null && inc.Value.ToString().Contains("BASE"))
                    return inc.Value.ToString();
            else
                try
                {
                    return findInherit(inc.Value.ToString());
                }catch(System.Exception e){
                    Trace.WriteLine(e.Message);
                    return null;
                }
        }  

        public bool GetDevices(string directory)
        {
            try
            {
                string[] files = Directory.GetFiles(directory);
                Parallel.ForEach(
                    files, f =>
                    {
                        try
                        {
                            Definition d = new Definition(f);
                            if (d.isBase)
                                d.LoadRomId();
                            lock(DefDictionary)
                            {
                                if (DefDictionary.ContainsKey(d.internalId))
                                {
                                    Trace.WriteLine("Duplicate definition found for: " + d.internalId + " in file: " + f + " Check the definitions!!");
                                    Trace.WriteLine("Definition was previously found in file: " + DefDictionary[d.internalId].defPath);
                                }
                                else
                                {
                                    DefDictionary.Add(d.internalId, d);

                                    lock (IdentList)
                                    {
                                        IdentList.Add(d.internalId);
                                        DeviceCount++;
                                    }
                                }
                            }
                        }
                        catch (System.Exception excpt)
                        {
                            Trace.WriteLine("Error reading XML file " + f);
                            Trace.WriteLine(excpt.Message);
                        }
                    });

                List<string> directories = Directory.GetDirectories(directory).ToList();
                
                Parallel.ForEach(
                    directories, d =>
                    {
                        if (!GetDevices(d))
                        {
                            return;
                        }
                    });

                return true;
            }
            catch (System.Exception excpt)
            {
                Trace.WriteLine(excpt.Message);
            }

            return false;
        }

        public string getDefPath(string id)
        {
            if (DefDictionary.ContainsKey(id) && DefDictionary[id].internalId != null)
                return DefDictionary[id].defPath;
            return null;
        }

        public Definition getDef(string id)
        {
            if (DefDictionary.ContainsKey(id) && DefDictionary[id].internalId != null)
            {
                DefDictionary[id].Populate();
                return DefDictionary[id];
            }
            return null;
        }
    }   
}
