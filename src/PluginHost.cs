using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using jordanrobot.IPlugin;

namespace IPluginDemo
{
    class PluginHost
    {
        [ImportMany(typeof(IPlugin))]
        List<IPlugin> plugins = new List<IPlugin> { };

        public string PluginsPath { get; set; }
        public CompositionContainer Container { set; get; }

        public PluginHost()
        {
        }

        public List<IPlugin> ComposePlugins()
        {
            //Wire up MEF parts
            if (string.IsNullOrEmpty(PluginsPath))
            {
                PluginsPath = ReturnDefaultPluginPath();
            }

            var assemblyCatalog = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
            var directoryCatalog = new DirectoryCatalog(PluginsPath);
            var catalog = new AggregateCatalog(directoryCatalog, assemblyCatalog);

            Container = new CompositionContainer(catalog);
            Container.SatisfyImportsOnce(this);

            return plugins;
        }

        private static string ReturnDefaultPluginPath()
        {
            var fullExecutingAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var pluginPath = System.IO.Path.GetDirectoryName(fullExecutingAssemblyPath);
            return System.IO.Path.Combine(pluginPath, "Plugins");
        }

        public void Dispose()
        {
            Container.Dispose();
        }
    }
}
