using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModRom.Tables
{
    public class Category
    {
        public string name { get; private set; }

        public List<string> subcats { get; private set; }

        public Category(string n)
        {
            this.name = n;
            this.subcats = new List<string>();
        }
        
    }

    public class CategoryList : List<Category>
    {

        public bool Contains(string search)
        {
            foreach (Category cat in this)
	        {
		        if(cat.name.Contains(search)) return true;
	        }

            return false;
        }
 

    }

}
