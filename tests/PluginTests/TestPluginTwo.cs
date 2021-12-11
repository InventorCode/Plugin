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
    internal class TestPluginTwo : IPlugin
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

        #region IPlugin Properties
        public string Name { get => Assembly.GetExecutingAssembly().GetName().Name; }
        public string Version { get => Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        public CommandControl ExecuteSettings { get; set; }
        #endregion
    }
}
