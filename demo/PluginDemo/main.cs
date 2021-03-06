using Inventor;
using InventorCode.Plugin;
using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Windows.Forms;

namespace PluginDemo
{
    /// <summary>
    /// This is a plugin that we'll create to work with the IPlugin Demo.  This is effectively
    /// a mini-addin that knows nothing about the "host-addin" that will be loading it.
    /// All button creation, command creation, etc relevant to this mini-addin will be contained
    /// here.
    /// </summary>

    //This attribute is required!  It is what PluginHost uses to find this plugin.
    [Export(typeof(IPlugin))]
    public class Main : IPlugin
    {
        // Implement the IPlugin Interface

        private Inventor.Application _inventorApplication;
        private string _clientId;

        public void Activate(Inventor.Application InventorApplication, string ClientId, bool firstTime = true)
        {
            _inventorApplication = InventorApplication;
            _clientId = ClientId;
            MessageBox.Show("External PluginDemo Loaded, with " + Name + " ver: " + Version);
        }

        public void Deactivate()
        {
            _inventorApplication = null;
        }

        public void Execute()
        {
        }

        #region Properties

        //Provides a place to implement a settings command from the PluginHost
        public CommandControl ExecuteSettings { get; set; }

        // Provides the name of your plugin.  Typically this would be set to return the
        // assembly name as shown below...
        public string Name { get => Assembly.GetExecutingAssembly().GetName().Name; }

        // Provides the version of your plugin.  Typically this would be set to return the
        // assembly version as shown below...
        public string Version { get => Assembly.GetExecutingAssembly().GetName().Version.ToString(); }

        #endregion Properties
    }
}