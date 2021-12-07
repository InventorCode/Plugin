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
        public string Version { get => "This is a version."; }
        CommandControl IPlugin.ExecuteSettings { get => null; set => throw new NotImplementedException(); }
    }
}
