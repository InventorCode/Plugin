using Inventor;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using jordanrobot.IPlugin;
using System.Collections.Generic;

namespace IPluginDemo
{
    [ProgId("IPluginDemo.StandardAddinServer")]
    [GuidAttribute("7F39964A-C0DC-4BC7-948E-4B0A060256D1")]
    public class StandardAddInServer : Inventor.ApplicationAddInServer
    {
        private Inventor.Application m_inventorApplication;

        // Create an instance of the PluginHost class in the package.  This contains
        // the code to wire up the plugins dynamically.
        private PluginHost pluginHost = new PluginHost();
        
        // Added to store the list of plugins that we will be generating.  We'll use
        // this later to activate and deactivate the plugins.
        private List<IPlugin> plugins = new List<IPlugin> { };

        public StandardAddInServer()
        {
            // Here we will actually wire up the plugins by collecting classes with advertised
            // IPlugin interfaces into our PluginHost.  We return that list of plugins below...

            plugins = pluginHost.ComposePlugins();
        }


        public void Activate(Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            m_inventorApplication = addInSiteObject.Application;

            // Here we'll activate the individual plugins...
            foreach (var item in plugins)
            {
                item.Activate(m_inventorApplication, "7F39964A-C0DC-4BC7-948E-4B0A060256D1");
            }
        }

        public void Deactivate()
        {
            // And here we'll deactivate the individual plugins...
            foreach (var item in plugins)
            {
                item.Deactivate();
            }

            pluginHost.Dispose();

            // Release objects.
            m_inventorApplication = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ExecuteCommand(int commandID)
        {
        }

        public object Automation
        {
            get
            {
                return null;
            }
        }
    }
}
