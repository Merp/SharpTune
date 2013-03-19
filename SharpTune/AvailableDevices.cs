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
        public AvailableDevices(string xmldir)
        {
            DefDictionary = new Dictionary<string,Definition>();
            IdentList = new List<string>();
            this.DeviceCount = 0;

            try
            {
                List<string> ts = ResourceUtil.directorySearchRecursiveDir(SharpTuner.EcuFlashDefRepoPath,null);
                if(!GetDevices(ts[0]))
                {
                    Console.WriteLine("XML initialize failed");
                }
                else
                {
                    //MessageBox.Show("finished populating available devices");
                    Console.WriteLine("Successfully loaded {0} XML definitions!", DeviceCount);
                }
                
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
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
                    Console.WriteLine(e.Message);
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
                                DefDictionary.Add(d.internalId, d);
                            lock(IdentList)
                                IdentList.Add(d.internalId);
                            DeviceCount++;
                        }
                        catch (System.Exception excpt)
                        {
                            Console.WriteLine("Error reading XML file " + f);
                            Console.WriteLine(excpt.Message);
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
                Console.WriteLine(excpt.Message);
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

        public bool LoadFullDef(string id)
        {
            if (DefDictionary.ContainsKey(id) && DefDictionary[id].internalId != null)
                DefDictionary[id].Populate();
            return true;
        }
    }   
}
