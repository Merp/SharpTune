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
using System.Xml;
using System.IO;
using ConsoleRedirection;

namespace DumpXML
{
    class DumpXml
    {
        StringBuilder output = new StringBuilder();

        public List<string> rangeNameList = new List<string>();

        public List<int> rangeStartList = new List<int>();

        public List<int> rangeLengthList = new List<int>();

        public void readXml()
        {
        
            using (XmlTextReader reader = new XmlTextReader("dump.xml"))
            {
                reader.ReadToFollowing("range");
                while (reader.EOF == false)
                {


                    reader.MoveToFirstAttribute();
                    rangeNameList.Add(reader.Value);
                    reader.MoveToNextAttribute();
                    string temp = (reader.Value);
                    rangeNameList.Add(temp);
                    rangeStartList.Add(Int32.Parse(reader.Value, System.Globalization.NumberStyles.HexNumber));
                    reader.MoveToNextAttribute();
                    temp = (reader.Value);
                    rangeNameList.Add(temp);
                    int temp1 = (Int32.Parse(reader.Value, System.Globalization.NumberStyles.HexNumber));
                    int temp2 = rangeStartList[rangeStartList.Count-1];
                    rangeLengthList.Add(temp1-temp2);

                    reader.ReadToFollowing("range");
                }

            }

        }

    }
}
