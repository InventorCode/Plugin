using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace jordanrobot.IPlugin
{
    public class PluginHost
    {
        [ImportMany(typeof(IPlugin))]
        private List<IPlugin> plugins = new List<IPlugin> { };

        public List<IPlugin> Plugins
        { get { return plugins; } }

        public string PluginsPath { get; set; }
        public CompositionContainer Container { set; get; }

        public PluginHost()
        {
        }

        public void ComposePlugins()
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
        }

        private static string ReturnDefaultPluginPath()
        {
            var fullExecutingAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var pluginPath = System.IO.Path.GetDirectoryName(fullExecutingAssemblyPath);
            return System.IO.Path.Combine(pluginPath, "Plugins");
        }

        public void ActivateAll(Inventor.Application invApp, string guid, bool firstTime = true)
        {
            foreach (var item in plugins)
            {
                item.Activate(invApp, guid);
            }
        }

        public void DeactivateAll()
        {
            foreach (var item in plugins)
            {
                item.Deactivate();
            }

            Container.Dispose();
        }
    }
}