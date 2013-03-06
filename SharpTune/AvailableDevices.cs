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
    public sealed class Pair<TFirst, TSecond>
    : IEquatable<Pair<TFirst, TSecond>>
    {
        private readonly TFirst first;
        private readonly TSecond second;

        public Pair(TFirst first, TSecond second)
        {
            this.first = first;
            this.second = second;
        }

        public TFirst First
        {
            get { return first; }
        }

        public TSecond Second
        {
            get { return second; }
        }

        public bool Equals(Pair<TFirst, TSecond> other)
        {
            if (other == null)
            {
                return false;
            }
            return EqualityComparer<TFirst>.Default.Equals(this.First, other.First) &&
                   EqualityComparer<TSecond>.Default.Equals(this.Second, other.Second);
        }

        public override bool Equals(object o)
        {
            return Equals(o as Pair<TFirst, TSecond>);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<TFirst>.Default.GetHashCode(first) * 37 +
                   EqualityComparer<TSecond>.Default.GetHashCode(second);
        }
    }


    public class AvailableDevices
    {
        
        public Dictionary<string, Pair<int, string>> IdentifierMap {get; set;}

        public int DeviceCount { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AvailableDevices(string xmldir)
        {
            IdentifierMap = new Dictionary<string, Pair<int, string>>();

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

        public Dictionary<String, String> BuildInheritanceMap()
        {
            Dictionary<String, String> imap = new Dictionary<String, String>();

            foreach (KeyValuePair<String, Pair<int, String>> pair in this.IdentifierMap)
            {
                imap.Add(pair.Key, findInherit(pair.Value.Second));
            }
            return imap;
        }

        public String findDefinition(String xmlid)
        {
            foreach (KeyValuePair<string, Pair<int, String>> pair in this.IdentifierMap)
            {
                if (pair.Value.Second.Equals(xmlid))
                {
                    return pair.Key.ToString();
                }
            }
            return null;
        }

        public String findInherit(String xmlid)
        {
            String fetchpath = "";
            foreach(KeyValuePair<string,Pair<int,String>> pair in this.IdentifierMap){
                if(pair.Value.Second.Equals(xmlid)){
                    fetchpath = pair.Key.ToString();
                    break;
                }
            }
            XDocument xmlDoc = XDocument.Load(fetchpath);
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
                        string ident;
                        int identaddress;
                        try
                        {
                            if (!ReadIdent(f, out ident, out identaddress))
                            {
                                Console.WriteLine("no identifier found for " + f);
                            }
                            else
                            {
                                try
                                {
                                    Pair<int, string> temppair = new Pair<int, string>(identaddress, ident);
                                    lock (this.IdentifierMap)
                                    {
                                        this.IdentifierMap.Add(f, temppair);
                                    }
                                    //Console.WriteLine("Added Device {0} with identaddress {1}", ident, identaddress);
                                    DeviceCount++;
                                }
                                catch (System.Exception excpt)
                                {
                                    Console.WriteLine("Error reading XML file, adding identifier to map " + f);
                                    Console.WriteLine(excpt.Message);
                                }
                            }
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

        private bool ReadIdent(string fetchpath, out string ident, out int identaddress)
        {
            ident = null;
            identaddress = 0;
            if (fetchpath == null) return false;

            XDocument xmlDoc;
            XElement xmlEle;

            try
            {
                xmlDoc = XDocument.Load(fetchpath);
                xmlEle = XElement.Load(fetchpath);
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine("Error loading XML file " + fetchpath);
                Console.WriteLine(excpt.Message);
            }
            finally
            {
                xmlDoc = XDocument.Load(fetchpath);
                xmlEle = XElement.Load(fetchpath);
            }

            //Initial definition fetch, without inheritance

            if (xmlDoc.XPathSelectElement("//rom/romid/internalidstring") != null)
            {
                string internalidstring = xmlDoc.XPathSelectElement("//rom/romid/internalidstring").Value;
                ident = internalidstring;
            }
            else if (xmlDoc.XPathSelectElement("//rom/romid/internalidhex") != null)
            {
                string internalidhex = xmlDoc.XPathSelectElement("//rom/romid/internalidhex").Value;
                ident = internalidhex;
            }
            else
            {
                return false;
            }
            identaddress = int.Parse(xmlDoc.XPathSelectElement("//rom/romid/internalidaddress").Value, NumberStyles.AllowHexSpecifier);
            return true;

        }

    }
}
