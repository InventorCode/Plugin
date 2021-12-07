namespace InventorCode.Plugin
{
    public interface IPlugin
    {
        void Execute();

        void Activate(Inventor.Application inventorApplication, string ClientId, bool firstTime = true);

        void Deactivate();

        Inventor.CommandControl ExecuteSettings { get; set; }

        string Name { get;}

        string Description { get; }

        string Version { get; }
    }
}