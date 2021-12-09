namespace InventorCode.Plugin
{
    public interface IPlugin
    {
        /// <summary>
        /// Executes code that the user chooses.
        /// </summary>
        void Execute();

        /// <summary>
        /// Activates the plugin, acts as the plugin's entry point.  Synonymous with the
        /// Inventor.StandardAddinServer.Activate() method.
        /// </summary>
        /// <param name="inventorApplication">Inventor.Application object</param>
        /// <param name="ClientId">The addin's string GUID.</param>
        /// <param name="firstTime">Optional boolean value. Not currently used.</param>
        void Activate(Inventor.Application inventorApplication, string ClientId, bool firstTime = true);

        /// <summary>
        /// Deactivates the plugin, acts as the plugin's finalize/cleanup method.
        /// Synonymous with the Inventor.StandardAddinServer.Activate() method.
        /// </summary>
        void Deactivate();

        /// <summary>
        /// Provides the ability to add a settings ui accessible from the PluginHost.
        /// </summary>
        Inventor.CommandControl ExecuteSettings { get; set; }

        /// <summary>
        /// Provides the name of the plugin.
        /// </summary>
        string Name { get;}

        /// <summary>
        /// Provides the version number of the plugin.
        /// </summary>
        string Version { get; }
    }
} 