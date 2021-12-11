using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventorCode.Plugin;
using Inventor;
using System.ComponentModel.Composition;
using System.Reflection;

namespace PluginTests
{
    [Export(typeof(IPlugin))]
    internal class TestPlugin : IPlugin
    {

        public void Activate(global::Inventor.Application inventorApplication, string ClientId, bool firstTime = true)
        {
            throw new NotImplementedException();
        }

        public void Deactivate()
        {
            throw new NotImplementedException();
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }

        public string Name { get => "This is a name."; }
        public string Version { get => "This is a version."; }
        CommandControl IPlugin.ExecuteSettings { get; set; }
    }
}
