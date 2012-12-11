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
using System.Reflection;
using Merp;


namespace SharpTune
{
    public class ResourceUtil
    {
        /// <summary>
        /// Takes the full name of a resource and loads it in to a stream.
        /// </summary>
        /// <param name="resourceName">Assuming an embedded resource is a file
        /// called info.png and is located in a folder called Resources, it
        /// will be compiled in to the assembly with this fully qualified
        /// name: Full.Assembly.Name.Resources.info.png. That is the string
        /// that you should pass to this method.</param>
        /// <returns></returns>
        public static Stream GetEmbeddedResourceStream(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }

        ///// <summary>
        ///// Get the list of all emdedded resources in the assembly.
        ///// </summary>
        ///// <returns>An array of fully qualified resource names</returns>
        //public static List<string> GetPatchNames()
        //{
        //    string[] fulllist = Assembly.GetAssembly(typeof(RomModCore.Program)).GetManifestResourceNames();
        //    List<string> patchList = new List<String>();
            
        //    foreach (string item in fulllist)
        //    {
        //        if (item.Contains(".patch"))
        //        {
        //            patchList.Add(item);
        //        }
                                
        //    }

        //    return patchList;

        //}

        public static string ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }


        /// <summary>
        /// Returns a List of file paths
        /// Whos names contain the search term
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public static List<string> directorySearchRecursive(string directory, string[] terms)
        {
           List<string> filelist = new List<string>();
           try
           {

               foreach (string d in Directory.GetDirectories(directory))
               {
                   
                   foreach (string file in directorySearch(d, terms))
                   {
                       filelist.Add(file);
                   }

                   return filelist;
               }
               //dumps here if no subdirs
               return directorySearch(directory, terms);
           }
           catch (System.Exception excpt)
           {
               Console.WriteLine(excpt.Message);
           }

            return null;
        }

        public static List<string> directorySearchRecursiveDir(string directory, string[] terms)
        {
            List<string> dirlist = new List<string>();
            try
            {
                bool found = true;
                foreach (string t in terms)
                    {
                        if (!directory.ContainsCI(t))
                        {
                            found = false;
                        }
                    }
                if (found)
                {
                    dirlist.Add(directory);
                    return dirlist;
                }

                string[] dirs = Directory.GetDirectories(directory);
                if (dirs.Length < 1)
                {
                    return dirlist;
                }
                else
                {
                    foreach (string d in dirs)
                    {
                        foreach (string st in directorySearchRecursiveDir(d, terms))
                        {
                            dirlist.Add(st);
                        }
                    }
                }
            }
            catch (System.UnauthorizedAccessException excpt)
            {
                //do nothing
                //Console.WriteLine(excpt.Message);
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            return dirlist;
        }

        /// <summary>
        /// Returns a List of file paths
        /// Whos names contain the search term
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public static List<string> directorySearch(string directory, string[] terms)
        {
            List<string> filelist = new List<string>();
            try
            {
                    string[] files = Directory.GetFiles(directory);

                    foreach (string f in files)
                    {
                        if (termSearch(f, terms))
                        {
                            filelist.Add(f);
                        }
                    }
 
                    return filelist;

            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }

            return null;
        }

        /// <summary>
        /// Searches a string for all terms in the array
        /// </summary>
        /// <param name="content"></param>
        /// <param name="terms"></param>
        /// <returns></returns>
        public static bool termSearch(string content, string[] args)
        {
            foreach (string term in args)
                       {
                           if (!content.Contains(term))
                           {
                               return false;
                           }
                       }

            return true;
        }

    }

}
