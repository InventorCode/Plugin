using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace InventorCode.Plugin
{
    /// <summary>
    /// PluginHost provides a simplified wrapper for hosting MEF components within
    /// Autodesk Inventor addins.
    /// </summary>
    public class PluginHost
    {
        [ImportMany(typeof(IPlugin))]
        private List<IPlugin> plugins = new List<IPlugin> { };

        /// <summary>
        /// List of IPlugin objects.  This list is populated after executing ComposePlugins()
        /// </summary>
        public List<IPlugin> Plugins
        { get { return plugins; } }

        /// <summary>
        /// A string path that specifies the plugin assembly directory.  This is an option property
        /// that may be omitted.  If it is omitted, the value of "Plugins/" will be used.
        /// </summary>
        public string PluginsPath { get; set; }

        /// <summary>
        /// The composition container for dynamically loaded MEF components. This is provided for
        /// advanced use.
        /// </summary>
        public CompositionContainer Container { set; get; }

        /// <summary>
        /// Creates a new PluginHost object.
        /// </summary>
        public PluginHost()
        {
        }

        private bool DirectoryExists(string testString)
        {
            return System.IO.Directory.Exists(testString);
        }

        /// <summary>
        /// Dynamically loads the MEF components from the current assembly and any assemblies in
        /// the PluginPath that implement the IPlugin interface.
        /// </summary>
        public void ComposePlugins()
        {
            //Wire up MEF parts
            if (string.IsNullOrEmpty(PluginsPath) || !System.IO.Directory.Exists(PluginsPath))
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
            } else
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

        /// <summary>
        /// Executes the Activate() method of each IPlugin object.
        /// </summary>
        /// <param name="invApp">Inventor.Application object.</param>
        /// <param name="guid">The addin's string GUID.</param>
        /// <param name="firstTime">Optional boolean value. Not currently used.</param>
        public void ActivateAll(Inventor.Application invApp, string guid, bool firstTime = true)
        {
            foreach (var item in plugins)
            {
                item.Activate(invApp, guid);
            }
        }

        /// <summary>
        /// Executes the Deactivate() method in each IPlugin object listed in the Container.
        /// </summary>
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