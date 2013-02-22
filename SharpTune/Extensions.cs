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
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;

namespace SharpTune
{
    public class FDialogState
    {
        public DialogResult result;
        public FolderBrowserDialog dialog;
        public void ThreadProcShowDialog()
        {
            result = dialog.ShowDialog();
        }
    }

    public class OFDialogState
    {
        public DialogResult result;
        public OpenFileDialog dialog;
        public void ThreadProcShowDialog()
        {
            result = dialog.ShowDialog();
        }
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

    public static class Extensions
    {
        public static DialogResult STAShowFDialog(FolderBrowserDialog dialog)
        {
            FDialogState state = new FDialogState();
            state.dialog = dialog;
            System.Threading.Thread t = new System.Threading.Thread(state.ThreadProcShowDialog);
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            t.Join();
            return state.result;
        }

        public static DialogResult STAShowOFDialog(OpenFileDialog dialog)
        {
            OFDialogState state = new OFDialogState();
            state.dialog = dialog;
            System.Threading.Thread t = new System.Threading.Thread(state.ThreadProcShowDialog);
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            t.Join();
            return state.result;
        }

        

        public static DialogResult STAShowSADialog(SaveFileDialog dialog)
        {
            SADialogState state = new SADialogState();
            state.dialog = dialog;
            System.Threading.Thread t = new System.Threading.Thread(state.ThreadProcShowDialog);
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Start();
            t.Join();
            return state.result;
        }

        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static bool ContainsCI(this string source, string toCheck)
        {
            return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool EqualsCI(this string source, string toCheck)
        {
            return source.Equals(toCheck, StringComparison.OrdinalIgnoreCase);
        }

        public static byte[] ReverseBytes(this byte[] inArray)
        {
            byte temp;
            int highCtr = inArray.Length - 1;

            for (int ctr = 0; ctr < inArray.Length / 2; ctr++)
            {
                temp = inArray[ctr];
                inArray[ctr] = inArray[highCtr];
                inArray[highCtr] = temp;
                highCtr -= 1;
            }
            return inArray;
        }

        public static byte[] ToByteArray(this String hexString)
        {

            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }


            byte[] HexAsBytes = new byte[hexString.Length / 2];
            for (int index = 0; index < HexAsBytes.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                if (!VerifyHex(byteValue)) return HexAsBytes;

                HexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return HexAsBytes;
        }


        public static readonly Regex r = new Regex(@"^[0-9A-F]+$");
        public static bool VerifyHex(string _hex)
        {
            return r.Match(_hex).Success;
        }

        public static void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);
            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }
            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }

        public static bool MatchAllCI(string foo, List<string> sTerms)
        {
            foreach(string s in sTerms)
            {
                if (!foo.ContainsCI(s))
                    return false;
            }
            return true;
        }

        public static bool ExcludeAllCI(string foo, List<string> sTerms)
        {
            foreach (string s in sTerms)
            {
                if (foo.ContainsCI(s))
                    return false;
            }
            return true;
        }

        public static List<string> DirSearchCI(string sDir, List<string> sTerms)
        {
	        try	
	        {
                List<string> lstFilesFound = new List<string>();
                foreach (string f in Directory.GetFiles(sDir))
                {
                    if (MatchAllCI(Path.GetFileName(f), sTerms))
                        lstFilesFound.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    lstFilesFound.AddRange(DirSearchCI(d, sTerms));
                }
                return lstFilesFound;
	        }
	        catch (System.Exception excpt) 
	        {
		        Console.WriteLine(excpt.Message);
	        }
            return null;
        }

        public static List<string> DirSearchCI(string sDir, List<string> search, List<string> excludes)
        {
            try
            {
                List<string> lstFilesFound = new List<string>();
                foreach (string f in Directory.GetFiles(sDir))
                {
                    if (MatchAllCI(Path.GetFileName(f), search) && ExcludeAllCI(Path.GetFileName(f), excludes))
                        lstFilesFound.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    lstFilesFound.AddRange(DirSearchCI(d, search, excludes));
                }
                return lstFilesFound;
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            return null;
        }

        public static void getFilePaths(List<string> input, ref List<string> output)
        {
            output.Clear();
            foreach (string s in input)
            {
                string t = s.ToString();
                output.Add(Path.GetFileName(t));
            }
        }

        public static void deleteFile(this string path)
        {
            File.Delete(path);
        }

        public static string[] ShiftLeftTruncate(string[] args)
        {
            string[] targs = new string[args.Length - 1];
            for (int i = 1; i < args.Length; i++)
                targs[i - 1] = args[i];
            return targs;
        }
    }
}
