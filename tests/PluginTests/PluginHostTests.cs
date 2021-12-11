using InventorCode.Plugin;
using NUnit.Framework;
using System;
using System.Linq;

namespace PluginTests
{
    [TestFixture]
    public class PluginHostTests
    {
        [Test]
        public void PluginHostConstructor_ActivatesPlugin()
        {
            var host = new PluginHost();
            host.ComposePlugins();

            Action test = () => host.ActivateAll(null, "test");

            Assert.Throws<NotImplementedException>(() => test());
        }

        [Test]
        public void PluginHost_FindsTwoInternalPlugins()
        {
            var host = new PluginHost();
            host.ComposePlugins();
            var plugins = host.Plugins;

            Assert.AreEqual(2, plugins.Count());
        }

        [Test]
        public void PluginHost_CanAccessName()
        {
            var host = new PluginHost();
            host.ComposePlugins();
            var plugins = host.Plugins;

            var answer = from IPlugin plugin in plugins
                         where plugin.Name == "This is a name."
                         select plugin.Name;

            Assert.AreEqual(answer.Single(), "This is a name.");
        }

        [Test]
        public void PluginHost_CanAccessVersion()
        {
            var host = new PluginHost();
            host.ComposePlugins();
            var plugins = host.Plugins;

            var answer = from IPlugin plugin in plugins
                         where plugin.Version == "This is a version."
                         select plugin.Version;

            Assert.AreEqual(answer.Single(), "This is a version.");
        }

        [Test]
        public void PluginHost_ExecuteSettingsReturnsNull()
        {
            var host = new PluginHost();
            host.ComposePlugins();
            var plugins = host.Plugins;
            var plugin = plugins[0];
            var answer = plugin.ExecuteSettings;

            Assert.IsNull(answer);
        }

    }
}