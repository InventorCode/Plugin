using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace InventorCode.Plugin
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

        private bool DirectoryExists(string testString)
        {
            return System.IO.Directory.Exists(testString);
        }

        public void ComposePlugins()
        {
            //Wire up MEF parts
            if (string.IsNullOrEmpty(PluginsPath))
            {
                PluginsPath = ReturnDefaultPluginPath();
            }

            var assemblyCatalog = new AssemblyCatalog(System.Reflection.Assembly.GetCallingAssembly());
            
            if (DirectoryExists(PluginsPath))
            {
                var catalog = new AggregateCatalog(assemblyCatalog);
                RecursivedMefPluginLoader(catalog, PluginsPath);
                Container = new CompositionContainer(catalog);
                Container.SatisfyImportsOnce(this);
            }
            {
                Container = new CompositionContainer(assemblyCatalog);
                Container.SatisfyImportsOnce(this);
            }
        }

        private void RecursivedMefPluginLoader(AggregateCatalog catalog, string path)
        {
            Queue<string> directories = new Queue<string>();
            directories.Enqueue(path);
            while (directories.Count > 0)
            {
                var directory = directories.Dequeue();
                //Load plugins in this folder
                var directoryCatalog = new DirectoryCatalog(directory);
                catalog.Catalogs.Add(directoryCatalog);

                //Add subDirectories to the queue
                var subDirectories = System.IO.Directory.GetDirectories(directory);
                foreach (string subDirectory in subDirectories)
                {
                    directories.Enqueue(subDirectory);
                }
            }
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