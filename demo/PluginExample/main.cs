using Inventor;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using jordanrobot.IPlugin;
using System.ComponentModel.Composition;

namespace PluginDemo
{
    [Export(typeof(IPlugin))]
    public class Main : IPlugin
    {
        private Inventor.Application _inventorApplication;
        private string _clientId;

        private DockableWindow dockableWindow;
        public void Activate(Inventor.Application InventorApplication, string ClientId, bool firstTime = true)
        {
            _inventorApplication = InventorApplication;
            _clientId = ClientId;

            //Create dock able window
            dockableWindow = _inventorApplication.UserInterfaceManager.DockableWindows.Add(ClientId,
                "docable_window.StandardAddInServer.dockableWindow", "IPluginDemo - Plugin sample");
            dockableWindow.ShowVisibilityCheckBox = true;
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
