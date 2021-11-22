using InventorCode.Plugin;
using System.ComponentModel.Composition;
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
            MessageBox.Show("EmbeddedPluginDemo Loaded.");
        }

        public void Deactivate()
        {
            _inventorApplication = null;
        }

        public void Execute()
        {
        }
    }
}