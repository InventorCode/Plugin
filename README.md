# IPlugin

A simple package to manage plugins for custom Autodesk Inventor Addins.

## Purpose

This package allows the user to quickly create a plugin architecture within an Autodesk Inventor addin.  The mechanism for this is Microsoft's MEF (Managed Extensibility Framework), a .Net Framework 4.x library that allows for plugin-like decoupling between components (MEF refers to these as "parts").  It sounds a bit dumb at first -- after all addins are plugins for Inventor.  But when faced with providing multiple tools as part of a single addin, the developer is faced with several problems, chief among them is organization of the tools.  The goal of this package is to:

1. Simplify bundling multiple related or unrelated tools/plugins into a single add-in.
1. Effectively eliminate the coupling between these tools/plugins.
1. Enable the developer to manage and store addins and tools/plugins in seperate locations
1. Enables development of tools/plugins and the main addin by separate parties/teams.

## Installation

Install this [nuget package](https://www.nuget.org/packages/jordanrobot.IPlugin/) by installing `jordanrobot.IPlugin`.  Alternatively you can head over to the [project repo](https://github.com/jordanrobot/IPlugin) and copy the classes you need from the `src/` directory.

## How It Works: An Overview

If you want to release an Inventor Addin named "Bob's Better Toolbox" that combines twenty different drawing and modeling tools, the typical path involves adding the code for each of the tools within the same addin codebase.  While organization methods vary, typically tools would be split into seperate namespaces.  This helps to seperate the unrelated portions of the code from each other; however these tools still reside within the same project, and it's very easy to introduce unintended dependencies. Changes to the tools may affect each other and cause bugs to creep into the code base.  It also becomes harder for multiple people to work on different tools at the same time.  For most teams, this will most likely become a mess after a period of time.

To help remedy this, we can decouple the tools/plugins from each other entirely.  They can reside in seperate namespaces, projects, solutions, or even repos. We can utilize MEF (Managed Extensibility Framework) to manage this; and it's not opinionated about where you put the source code for each plugin.  You're free to organize your project however best suits your organization or team.  The IPlugin project creates a wrapper around the MEF and tailors it to work with Autodesk Inventor's Addin API. This may sound complicated, but it's actually quite simple: the IPlugin project only contains one class and one interface.  These are the parts of the system:


1. The Addin has a typical `StandardAddInServer` class that directly interacts with Inventor; it implements the `Inventor.ApplicationAddInServer` interface that Inventor uses to load and manage the add-in lifecycle.  This class should be largely familiar for those who have written Inventor Addins in the past.  What we'll do is manage our `PluginHost` class instance here.

1. The IPlugin package provides a `PluginHost` class. This class will dynamically wire-up the various plugins to the Addin and send commands to Activate and Deactivate those plugins at Addin start. Members:

    - New(): returns a new PluginHost object
    - ActivateAll(Inventor.Application invApp, string guid, bool firstStart = true): activates each IPlugin loaded by MEF
    - DeactivateAll(): deactivates each IPlugin loaded by MEF
    - Plugins: returns a List<IPlugin> for each plugin dynamically loaded via MEF
    - PluginsPath: user settable string that controls where external assemblies are loaded from

1. The package also will provide an `IPlugin` interface. This interface is implemented in each plugin you create; we're essentially creating our own mini-addins.  This interface has a very similar structure to the `ApplicationAddInServer` interface.

    - Execute(): Executes whatever you want; accessible from the calling assembly.
    - Activate(Inventor.Application inventorApplication, string ClientId, bool firstTime = true)
    - Deactivate(): Deactivates the plugin


In the demo project below, when the addin is started:

1. The `StandardAddinServer` is loaded into Inventor. The `PluginHost` object is created.
1. The `PluginHost` class will look for each of the plugins implementing our `IPlugin` interface and
1. create a list of them.
1. The loaded plugins are then activated, and are ready for use.
1. When the addin is deactivated, the plugins are likewise deactivated.

## Plugin Assembly Location

These plugins can be located in the Addin assembly (dll file), or in seperate assemblies.  The default location for seperate addin assemblies is in a Plugin/ folder in the location of the addin assembly.  This default file structure looks something like:

- Addin/
    - Addin.addin
    - Addin.dll
    - Addin.manifest
    - Plugins/
        - Plugin.dll

You can change this default behavior by setting `PluginHost.PluginPath(string str)` to a folder path of your choice.  Note that there is no error checking or validation on user set locations.  After that, call the `pluginHost.ComposePlugins()` method and the new directory location will by dynamically searched for qualifying assemblies.

## Demo Project

This project contains a [Demo solution](https://github.com/jordanrobot/IPlugin/tree/master/demo) that demonstrates a bare-bones implementation of this system.

### AddinDemo

This is the addin class that implements the `Inventor.ApplicationAddInServer`.  

```C#
using Inventor;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using jordanrobot.IPlugin;
using System.Collections.Generic;

namespace AddinDemo
{
    [ProgId("AddinDemo.StandardAddinServer")]
    [GuidAttribute("7F39964A-C0DC-4BC7-948E-4B0A060256D1")]
    public class StandardAddInServer : Inventor.ApplicationAddInServer
    {
        private Inventor.Application m_inventorApplication;

        // Create an instance of the PluginHost class in the package.  This contains
        // the code to wire up the plugins dynamically.
        private PluginHost pluginHost = new PluginHost();
        
        public StandardAddInServer()
        {
            // Here we will actually wire up the plugins by collecting classes with advertised
            // IPlugin interfaces into our PluginHost object.  If you need granular control, you
            // can manually compose these using MEF. See the IPluginHost class to get you started.
            pluginHost.ComposePlugins();
        }


        public void Activate(Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            m_inventorApplication = addInSiteObject.Application;

            // Here we'll activate the individual plugins...
            pluginHost.ActivateAll(m_inventorApplication, "7F39964A-C0DC-4BC7-948E-4B0A060256D1");
        }

        public void Deactivate()
        {
            // And here we'll deactivate the individual plugins...
            pluginHost.DeactivateAll();

            // Release objects.
            m_inventorApplication = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ExecuteCommand(int commandID) { }

        public object Automation { get => null; }
    }
}
```

---

### PluginExample

As for the individual tools/plugins... the demo code for a single plugin is included below.  Think of this as a mini-addin; you can do just about anything here that you would in a full fledged addin: add event handlers, buttons, commands, ribbon interfaces, etc.

1. For this to work wou will need to add an assembly reference `System.ComponentModel.Composition`, and
1. install the `jordanrobot.IPlugin` nuget package

```C#
using Inventor;
using Microsoft.Win32;
using System;
using System.Runtime.Interop;
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

        private DockableWindow dockableWindow;
        public void Activate(Inventor.Application InventorApplication, string ClientId, bool firstTime = true)
        {
            _inventorApplication = InventorApplication;
            _clientId = ClientId;

            //Create dockable window
            dockableWindow = _inventorApplication.UserInterfaceManager.DockableWindows.Add(ClientId,
                "dockable_window.StandardAddInServer.dockableWindow", "IPluginDemo - Plugin sample");
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
