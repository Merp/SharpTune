using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SharpTune.Core
{
    public interface IPluginHost
    {
        bool Register(IPlugin ipi);
    }

    public interface IPlugin
    {
        string Name { get; set; }
        IPluginHost Host { get; set; }
        void Run();
    }
    
    class PluginContainer : IPluginHost
    {

        private IPlugin[] ipis;

        private List<string> SupportedPlugins = new List<string>(){"SharpTune Vin Auth"};

        public bool Register(IPlugin ipi)
        {
            //MenuItem mn = new MenuItem(ipi.Name, new EventHandler(NewLoad));
            Trace.WriteLine("Registered: " + ipi.Name);
            //menuItem1.MenuItems.Add(mn);
            return true;
        }  

        private void NewLoad(object sender, System.EventArgs e)
        {
            //MenuItem mn = (MenuItem)sender; 
            for (int i = 0; i < ipis.Length; i++)
            {
                foreach (string strType in SupportedPlugins)
                {
                    if (ipis[i] != null)
                    {
                        if (ipis[i].Name == strType)
                        {
                            ipis[i].Run();
                            break;
                        }
                    }
                }
            }
        }    
    }
}
