# IPlugin

A simple package to manage plugins for custom Autodesk Inventor Addins.

## Purpose

This package allows the user to quickly create a plugin architecture within an Autodesk Inventor addin.  The mechanism for this is Microsoft's MEF (Managed Extensibility Framework), a .Net Framework 4.x library that allows for plugin-like decoupling between components (MEF refers to these as "parts").  It sounds a bit dumb at first -- after all addins are plugins for Inventor.  But when faced with providing multiple tools as part of a single addin, the developer is faced with several problems, chief among them is organization of the tools.  The goal of this package is to:

1. Simplify bundling multiple related or unrelated tools into a single add-in.
1. Effectively eliminate the coupling between these tools.
1. Enable the developer to manage and store addins and tools in seperate locations
1. Enables development of tools and the main addin by separate parties/teams.

## Installation

Install this [nuget package](https://www.nuget.org/packages/jordanrobot.IPlugin/) by installing `jordanrobot.IPlugin`.  Alternatively you can head over to the [project repo](https://github.com/jordanrobot/IPlugin) and copy the classes you need from the `src/` directory.

## How It Works: An Overview

If you want to release an Inventor Addin named "Bob's Better Toolbox" that combines twenty different tools, the typical path involves adding the code for each of the tools within the same addin codebase.  You can organize the code into the relevant sections, say a namespace per tool.  This helps to seperate the unrelated portions of the code from each other; however these tools still reside within the same project. Changes to the tools may affect each other and there is no clear delineation between the tools.  For most teams, this will quickly become a mess.

1. In the plugin-style decoupled structure, the Addin has the typical `StandardAddInServer` class that directly interacts with Inventor; it implements the `Inventor.ApplicationAddInServer` interface that Inventor uses to load and manage the add-in lifecycle.
1. The IPlugin package provides a `PluginHost` class. This class will wire-up the various tools to the Addin.  If is usually accessed directly from the `StandardAddInServer` class.
1. The package also will provide an `IPlugin` interface. This interface is implemented in each tool; we're essentially creating our own mini-addins.

When the addin is started, the `PluginHost` class will look for each of the tools (classes implementing our `IPlugin` interface) and wire them up dynamically.  These tools can be located in the Addin assembly (dll file), or in seperate assemblies on the user's disc.

## Demo Project

This project contains a [Demo solution](https://github.com/jordanrobot/IPlugin/tree/master/demo) that demonstrates a bare-bones implementation of this system.

### IPluginDemo

This is the addin class that implements the `Inventor.ApplicationAddInServer`.  

```C#

using Inventor;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using jordanrobot.IPlugin;
using System.Collections.Generic;

namespace IPluginDemo
{
    [ProgId("IPluginDemo.StandardAddinServer")]
    [GuidAttribute("7F39964A-C0DC-4BC7-948E-4B0A060256D1")]
    public class StandardAddInServer : Inventor.ApplicationAddInServer
    {
        private Inventor.Application m_inventorApplication;

        // Create an instance of the PluginHost class in the package.  This contains
        // the code to wire up the plugins dynamically.
        private PluginHost pluginHost = new PluginHost();

        // Added to store the list of plugins that we will be generating.  We'll use
        // this later to activate and deactivate the plugins.
        private List<IPlugin> plugins = new List<IPlugin> { };

        public StandardAddInServer()
        {
            // Here we will actually wire up the plugins by collecting classes with advertised
            // IPlugin interfaces into our PluginHost.  We return that list of plugins below...

            plugins = pluginHost.ComposePlugins();
        }

        public void Activate(Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            m_inventorApplication = addInSiteObject.Application;

            // Here we'll activate the individual plugins...
            foreach (var item in plugins)
            {
                item.Activate(m_inventorApplication, "7F39964A-C0DC-4BC7-948E-4B0A060256D1");
            }
        }

        public void Deactivate()
        {

            // And here we'll deactivate the individual plugins...
            foreach (var item in plugins)
            {
                item.Deactivate();
            }
            pluginHost.Dispose();


            // Release objects.
            m_inventorApplication = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        
        public void ExecuteCommand(int commandID) { }

        public object Automation { get => null;}

    }
}
```

### Plugin Directory Location

By default, the plugins directory is located in: /Addin Directory/Plugins/
If you want to specify a plugin directory from which to load the plugin dlls, you can do so with the following call:
`pluginHost.PluginsPath = "C:/Path/To/Plugin/Dll/Files/"`
After that, call the `pluginHost.ComposePlugins()` method and the new directory location will by dynamically searched for qualifying assemblies.


### PluginExample

As for the individual tools/plugins... the demo code for a single plugin is included below.  Think of this as a mini-addin; you can do just about anything here that you would in a full fledged addin: add event handlers, buttons, commands, ribbon interfaces, etc.

1. For this to work wou will need to add an assembly reference `System.ComponentModel.Composition`, and
1. install the `jordanrobot.IPlugin` nuget package

```C#
using Inventor;
using Microsoft.Win32;
using System;
using System.Runtime.Interop
using System.Windows.Forms;
using jordanrobot.IPlugin;
using System.ComponentModel.Composition;

namespace PluginDemo
{
    //This attribute is required!  It is what PluginHost uses to find this plugin.
    [Export(typeof(IPlugin))]
    // Implement the IPlugin Interface
    public class Main : IPlugin
    {
        private Inventor.Application _inventorApplication;
        private string _clientId;

        // we'll create a dockable window for the purposes of this test...
        private DockableWindow dockableWindow;

        public void Activate(Inventor.Application InventorApplication, string ClientId, bool firstTime = true)
        {
            _inventorApplication = InventorApplication;
            _clientId = ClientId;

            //Create dockable window
            dockableWindow = _inventorApplication.UserInterfaceManager.DockableWindows.Add(ClientId,
                "docable_window.StandardAddInServer.dockableWindow", "IPluginDemo - Plugin sample");
            dockableWindow.ShowVisibilityCheckBox = true;
        }

        public void Deactivate()
        {
            _inventorApplication = null;
        }

        public void Execute()
        {
        }
    }
}
```
