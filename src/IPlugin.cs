namespace PlugableAddin
{
    public interface IPlugin
    {
        void Execute();

        void Activate(Inventor.Application inventorApplication, string ClientId, bool firstTime = true);

        void Deactivate();
    }
}