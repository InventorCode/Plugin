using Inventor;
using InventorCode.Plugin;
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

        public CommandControl ExecuteSettings { get => null; }

        public string Name { get => Assembly.GetExecutingAssembly().GetName().Name; }

        public string Version { get => Assembly.GetExecutingAssembly().GetName().Version.ToString(); }

        public string Description
        {
            get
            {
                var assembly = typeof(IPlugin).Assembly;
                object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);

                AssemblyProductAttribute attribute = null;
                if (attributes.Length > 0)
                {
                    attribute = attributes[0] as AssemblyProductAttribute;
                    return attribute.ToString();
                }
                return null;
            }
        }
    }
}