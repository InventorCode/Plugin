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
    }
}