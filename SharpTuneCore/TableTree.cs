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
using SharpTune.Tables;
using System.Windows.Forms;

namespace SharpTune
{

    //TODO: Change to inherit a treenode?
    public class TableTree
    {
        public TreeNode Tree { get; set; }

        public List<TreeNode> treeCollection { get; set; }

        /// <summary>
        /// Constructor from definition
        /// </summary>
        /// <param name="definition"></param>
        public TableTree(DeviceImage rom)
        {

            Definition definition = rom.Definition;

            Tree = new TreeNode(rom.FileName);
            Tree.Tag = rom.FilePath;
            treeCollection = new List<TreeNode>();

            foreach (Table table in rom.tableList)
            {

                //TODO: Get rid of this garbage
                //TODO: add Image for each tabel type
                //TODO: Add checkboxes for map copy
                string tablesubcat = null;
                string tablecategory = table.properties["category"].ToString();

                //if (tablecategory.Contains(" - "))
                //{
                //    int splitter = table.properties["category"].ToString().IndexOf('-');
                //    tablecategory = table.properties["category"].ToString().Substring(0, splitter);
                //    tablesubcat = table.properties["category"].ToString().Substring(splitter + 1, table.properties["category"].ToString().Length - splitter - 1);
                //}

                //if (tablesubcat != null)
                //{
                //    if (!treeCollection.Exists(tn => tn.Tag.ToString().Contains(table.properties["category"].ToString())))
                //    {
                //        //subcategory doesn't exist
                //        if (!treeCollection.Exists(tn => tn.Tag.ToString().Contains(tablecategory) && tn.Tag.ToString().Contains("1t1o1p1")))
                //        {
                //            //main category doesn't exist, create it
                //            TreeNode temp = new TreeNode(tablecategory);
                //            temp.Tag = "1t1o1p1 " + tablecategory;

                //            this.treeCollection.Add(temp);
                //        }
                //        //now create subcategory
                //        TreeNode temp2 = new TreeNode(tablesubcat);
                //        temp2.Tag = table.properties["category"];
                //        TreeNode temptable = new TreeNode(table.properties["name"]);
                //        temptable.Tag = table.properties["name"] + ".table";
                //        foreach (KeyValuePair<string, string> property in table.properties)
                //        {
                //            temptable.Nodes.Add(property.Key + ": " + property.Value);
                //        }
                //        temp2.Nodes.Add(temptable);
                //        this.treeCollection.Add(temp2);
                //    }
                //    else
                //    {

                //        //subcat already exists!
                //        TreeNode temptable1 = new TreeNode(table.properties["name"]);
                //        temptable1.Tag = table.properties["name"] + ".table";
                //        foreach (KeyValuePair<string, string> property in table.properties)
                //        {
                //            temptable1.Nodes.Add(property.Key + ": " + property.Value);
                //        }
                //        this.treeCollection.Find(tn => tn.Tag.ToString().Contains(table.properties["category"].ToString()) && !tn.Tag.ToString().Contains("1t1o1p1")).Nodes.Add(temptable1);
                //    }
                //}
                //else
                //{
                    ///subcat is null, working direct in top category!
                    ///
                    if (!treeCollection.Exists(tn => tn.Tag.ToString().Contains(tablecategory)))
                    {
                        //tree doesn't exist yet
                        TreeNode temp = new TreeNode(tablecategory);
                        temp.Tag = tablecategory; // "1t1o1p1" + tablecategory;
                        TreeNode temptable = new TreeNode(table.properties["name"]);
                        temptable.Tag = table.properties["name"] + ".table";
                        foreach (KeyValuePair<string, string> property in table.properties)
                        {
                            temptable.Nodes.Add(property.Key + ": " + property.Value);
                        }
                        temp.Nodes.Add(temptable);
                        this.treeCollection.Add(temp);
                    }
                    else
                    {
                        //tree exists already
                        TreeNode temptable1 = new TreeNode(table.properties["name"]);
                        temptable1.Tag = table.properties["name"] + ".table";
                        foreach (KeyValuePair<string, string> property in table.properties)
                        {
                            temptable1.Nodes.Add(property.Key + ": " + property.Value);
                        }
                        //TODO: ADD CHECKBOXES HERE!
                        this.treeCollection.Find(tn => tn.Tag.ToString().Contains(tablecategory)).Nodes.Add(temptable1);
                    }
                //}
            }

            //Finished parsing through every table in the list!

            //foreach (TreeNode tree in treeCollection)
            //{
            //    if (!tree.Tag.ToString().Contains("1t1o1p1"))
            //    {
            //        string temp = tree.Tag.ToString();
            //        int splitter = temp.IndexOf('-');
            //        temp = temp.Substring(0, splitter);
            //        //merge subcats in staging tree collection, find the parent tree
            //        this.treeCollection.Find(tn => tn.Text.ToString().Contains(temp) && tn.Tag.ToString().Contains("1t1o1p1")).Nodes.Add(tree);
            //    }
            //}

            foreach (TreeNode tree in treeCollection)
            {
                //if (tree.Tag.ToString().Contains("1t1o1p1"))
                //{
                    //add top categories to final tree
                    Tree.Nodes.Add(tree);
                //}
            }
        }
    }
}
