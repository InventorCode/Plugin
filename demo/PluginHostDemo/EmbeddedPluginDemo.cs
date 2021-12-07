using InventorCode.Plugin;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Windows.Forms;

namespace PluginHostDemo
{
    [Export(typeof(IPlugin))]
    internal class EmbeddedPluginDemo : IPlugin
    {
        private Inventor.Application _inventorApplication;
        private string _clientId;

        public void Activate(Inventor.Application InventorApplication, string ClientId, bool firstTime = true)
        {
            _inventorApplication = InventorApplication;
            _clientId = ClientId;
            MessageBox.Show("EmbeddedPluginDemo Loaded with " + Name + " ver: " + Version) ;
        }

        public void Deactivate()
        {
            _inventorApplication = null;
        }

        public void Execute()
        {
        }

        public Inventor.CommandControl ExecuteSettings { get => null; }

        public string Name { get => Assembly.GetExecutingAssembly().GetName().FullName; }

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