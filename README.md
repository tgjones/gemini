# Gemini [![Build status](https://github.com/tgjones/gemini/workflows/CI/badge.svg)](https://github.com/tgjones/gemini/actions) [![NuGet](https://img.shields.io/nuget/v/GeminiWpf.svg)](https://www.nuget.org/packages/GeminiWpf/) [![Join the chat at https://gitter.im/tgjones/gemini](https://badges.gitter.im/tgjones/gemini.svg)](https://gitter.im/tgjones/gemini?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

Gemini is a WPF framework designed specifically for building IDE-like applications. It builds on some excellent libraries:

* [AvalonDock](https://github.com/Dirkster99/AvalonDock) (specifically Dirkster99's fork, because it supports .NET Core)
* [Caliburn Micro](https://github.com/Caliburn-Micro/Caliburn.Micro)

Gemini ships with two themes: a Light theme and a Blue theme. There is also an in-development Dark theme.

![Screenshot - Light theme](https://raw.github.com/tgjones/gemini/master/doc/gemini-everything-light.png)

![Screenshot - Blue theme](https://raw.github.com/tgjones/gemini/master/doc/gemini-everything-blue.png)

## Getting started

If you are creating a new WPF application, follow these steps:

* Install the [Gemini](http://nuget.org/packages/GeminiWpf/) NuGet package.
* Delete `MainWindow.xaml` - you don't need it.
* Open `App.xaml` and delete the `StartupUri="MainWindow.xaml"` attribute.
* Add `xmlns:gemini="http://schemas.timjones.tw/gemini"` to `App.xaml`.
* Add `<gemini:AppBootstrapper x:Key="bootstrapper" />` to a `ResourceDictionary` within `<Application.Resources>`.

So the whole `App.xaml` should look something like this:

```xml
<Application x:Class="Gemini.Demo.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
             xmlns:gemini="http://schemas.timjones.tw/gemini">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary>
                    <gemini:AppBootstrapper x:Key="bootstrapper" />
                </ResourceDictionary>
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

Now hit F5 and see a very empty application!

By far the easiest way to get started with Gemini is to use the various NuGet packages.
First, install the base Gemini package (note that the package ID is `GeminiWpf`, to
distinguish it from another NuGet package with the same name):

* [Gemini](http://nuget.org/packages/GeminiWpf/)

Then add any other modules you are interested in (note that some modules have dependencies
on other modules, but this is taken care of by the NuGet package dependency system):

* [Gemini.Modules.CodeCompiler](http://nuget.org/packages/Gemini.Modules.CodeCompiler/)
* [Gemini.Modules.CodeEditor](http://nuget.org/packages/Gemini.Modules.CodeEditor/)
* [Gemini.Modules.ErrorList](http://nuget.org/packages/Gemini.Modules.ErrorList/)
* [Gemini.Modules.GraphEditor](http://nuget.org/packages/Gemini.Modules.GraphEditor/)
* [Gemini.Modules.Inspector](http://nuget.org/packages/Gemini.Modules.Inspector/)
* [Gemini.Modules.Output](http://nuget.org/packages/Gemini.Modules.Output/)
* [Gemini.Modules.PropertyGrid](http://nuget.org/packages/Gemini.Modules.PropertyGrid/)

## Continuous builds

We use AppVeyor to build Gemini after every commit to the master branch,
and also to generate pre-release NuGet packages so you can try out new features immediately.

To access the pre-release NuGet packages, you'll need to add a custom package source in Visual Studio,
pointing to this URL:

https://ci.appveyor.com/nuget/gemini-g84phgw340sm

Make sure you select "Include Prerelease" when searching for NuGet packages.

## What does it do?

Gemini allows you to build your WPF application by composing separate modules. This provides a nice
way of separating out the code for each part of your application. For example, here is a very simple
module:

```csharp
[Export(typeof(IModule))]
public class Module : ModuleBase
{
	[Import]
	private IPropertyGrid _propertyGrid;

    public override IEnumerable<Type> DefaultTools
    {
        get { yield return typeof(IInspectorTool); }
    }

	public override void Initialize()
	{
		var homeViewModel = IoC.Get<HomeViewModel>();
		Shell.OpenDocument(homeViewModel);

		_propertyGrid.SelectedObject = homeViewModel;
	}

	private IEnumerable<IResult> OpenHome()
	{
		yield return Show.Document<HomeViewModel>();
	}
}
```

### Documents

Documents are (usually) displayed in the main area in the middle of the window. To create a new document
type, simply inherit from the `Document` class:

```csharp
public class SceneViewModel : Document
{
	public override string DisplayName
	{
		get { return "3D Scene"; }
	}

	private Vector3 _position;
	public Vector3 Position
	{
        get { return _position; }
        set
        {
            _position = value;
            NotifyOfPropertyChange(() => Position);
        }
	}
}
```

To open a document, call `OpenDocument` on the shell (`Shell` is defined in `ModuleBase`, but you can also 
retrieve it from the IoC container with `IoC.Get<IShell>()`):

```csharp
Shell.OpenDocument(new SceneViewModel());
```

You can then create a `SceneView` view, and Caliburn Micro will use a convention-based lookup to find the correct view.

### Persisted documents

If you have a document that needs to be loaded from, and saved to, a file, you can use the `PersistedDocument`
base class, to remove a lot of the boilerplate code that you would usually have to write. You only need to
implement the `DoNew`, `DoLoad`, and `DoSave` methods.

``` csharp
public class EditorViewModel : PersistedDocument
{
	private EditorView _view;
	private string _originalText;

	protected override Task DoNew()
	{
		_originalText = string.Empty;
		ApplyOriginalText();
		return TaskUtility.Completed;
	}

	protected override Task DoLoad(string filePath)
	{
		_originalText = File.ReadAllText(filePath);
		ApplyOriginalText();
		return TaskUtility.Completed;
	}

	protected override Task DoSave(string filePath)
	{
		var newText = _view.textBox.Text;
		File.WriteAllText(filePath, newText);
		_originalText = newText;
		return TaskUtility.Completed;
	}

	private void ApplyOriginalText()
	{
		_view.textBox.Text = _originalText;

		_view.textBox.TextChanged += delegate
		{
			IsDirty = string.Compare(_originalText, _view.textBox.Text) != 0;
		};
	}

	protected override void OnViewLoaded(object view)
	{
		_view = (EditorView) view;
	}
}
```

### Tools

Tools are usually docked to the sides of the window, although they can also be dragged
free to become floating windows. Most of the modules (ErrorList, Output, Toolbox, etc.) primarily provide tools.
For example, here is the property grid tool class:

```csharp
[Export(typeof(IPropertyGrid))]
public class PropertyGridViewModel : Tool, IPropertyGrid
{
	public PropertyGridViewModel()
	{
		DisplayName = "Properties";
	}

	public override PaneLocation PreferredLocation
	{
		get { return PaneLocation.Right; }
	}

	private object _selectedObject;
	public object SelectedObject
	{
		get { return _selectedObject; }
		set
		{
			_selectedObject = value;
			NotifyOfPropertyChange(() => SelectedObject);
		}
	}
}
```

For more details on creating documents and tools, look at the 
[demo program](https://github.com/tgjones/gemini/tree/master/src/Gemini.Demo)
and the source code for the built-in modules.

### Commands

Commands are one of the core concepts in Gemini. Commands help you to avoid duplicating code
by letting you define command handlers in a single place, regardless of whether the command
is invoked through a menu item, toolbar item, or other trigger.
Gemini's commands are conceptually similar to WPF commands, but they are more powerful.

First, create a command definition. Here's Gemini [command definition for opening files](https://github.com/tgjones/gemini/blob/master/src/Gemini/Modules/Shell/Commands/OpenFileCommandDefinition.cs):

``` csharp
[CommandDefinition]
public class OpenFileCommandDefinition : CommandDefinition
{
	public const string CommandName = "File.OpenFile";

	public override string Name
	{
		get { return CommandName; }
	}

	public override string Text
	{
		get { return "_Open"; }
	}

	public override string ToolTip
	{
		get { return "Open"; }
	}

	public override Uri IconSource
	{
		get { return new Uri("pack://application:,,,/Gemini;component/Resources/Icons/Open.png"); }
	}
	
	[Export]
    public static CommandKeyboardShortcut KeyGesture = new CommandKeyboardShortcut<OpenFileCommandDefinition>(new KeyGesture(Key.O, ModifierKeys.Control));
}
```

Then, provide a command handler. You can do this in one of two ways. For global commands,
that don't depend on a document context, create a global handler:

``` csharp
[CommandHandler]
public class OpenFileCommandHandler : CommandHandlerBase<OpenFileCommandDefinition>
{
	public override void Update(Command command)
	{
		// You can enable / disable the command here with:
		// command.Enabled = true;
		
		// You can also modify the command text / icon, which will affect
		// any menu items or toolbar items bound to this command.
	}
	
	public override async Task Run(Command command)
	{
		// ... implement command handling here
	}
}
```

For commands that depend on a document context, and should be disabled when there is no active document
or the active document is not of the correct type, define the command in the document class:

``` csharp
public class MyDocument : Document, ICommandHandler<ClearTextCommandDefinition>
{
	void ICommandHandler<ClearTextCommandDefinition>.Update(Command command)
	{
		command.Enabled = this.Text.Any();
	}

	Task ICommandHandler<ClearTextCommandDefinition>.Run(Command command)
	{
		this.Text = string.Empty;
		return TaskUtility.Completed;
	}
}
```

To remove built-in keyboard shortcuts, you can exclude them declaratively:

``` csharp
[Export]
public static ExcludeCommandKeyboardShortcut ExcludeFileOpenShortcut = new ExcludeCommandKeyboardShortcut(OpenFileCommandDefinition.KeyGesture);
```

To find out how to bind commands to menus or toolbars, see the "MainMenu" and "ToolBars" modules below.

## What modules are built-in?

Gemini itself is built out of seven core modules:

* MainWindow
* Shell
* MainMenu
* StatusBar
* ToolBars
* Toolbox
* UndoRedo

Several more modules ship with Gemini, and are available as 
[NuGet packages](http://nuget.org/packages?q=Gemini.Modules) as described above:

* CodeCompiler
* CodeEditor
* ErrorList
* GraphEditor
* Inspector
* Output
* PropertyGrid

For more information about these modules, see below. In general, each module adds some combination
of menu items, tool window, document types and services.

### MainWindow module

The main window module:

* manages the overall window

#### Provides

* `IMainWindow` interface

#### NuGet package

* [Gemini](http://nuget.org/packages/GeminiWpf/)

#### Dependencies

* None

#### Usage

The `IMainWindow` interface exposes a number of useful properties to control
aspects of the main application window.

```csharp
public interface IMainWindow
{
    WindowState WindowState { get; set; }
    double Width { get; set; }
    double Height { get; set; }

    string Title { get; set; }
    ImageSource Icon { get; set; } 

    IShell Shell { get; }
}
```

### Shell module

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-module-shell.png)

The shell module:

* manages placement of the document and tool windows
* persists and loads the size and position of tool windows
* manages the links between AvalonDock and Caliburn.Micro

#### Provides

* `IShell` interface

#### NuGet package

* [Gemini](http://nuget.org/packages/GeminiWpf/)

#### Dependencies

* None

#### Usage

The `IShell` interface exposes a number of useful properties and methods. It is the main way
to control Gemini's behaviour.

```csharp
public interface IShell
{
    event EventHandler ActiveDocumentChanging;
    event EventHandler ActiveDocumentChanged;

    bool ShowFloatingWindowsInTaskbar { get; set; }
        
	IMenu MainMenu { get; }
    IToolBars ToolBars { get; }
	IStatusBar StatusBar { get; }

	IDocument ActiveItem { get; }

	IObservableCollection<IDocument> Documents { get; }
	IObservableCollection<ITool> Tools { get; }

    void ShowTool<TTool>() where TTool : ITool;
	void ShowTool(ITool model);

	void OpenDocument(IDocument model);
	void CloseDocument(IDocument document);

	void Close();
}
```

### MainMenu module

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-module-mainmenu.png)

Adds a main menu to the top of the window.

#### NuGet package

* [Gemini](http://nuget.org/packages/GeminiWpf/)

#### Dependencies

* None

#### Usage

First, create commands, as described above in the "Commands" section. Then declare menus, menu item groups,
and menu items. This is how the built-in File menu and menu items are declared; you can create your own
menus in the same way.

```csharp
public static class MenuDefinitions
{
    [Export]
    public static readonly MenuDefinition FileMenu = new MenuDefinition(MainMenuBar, 0, Resources.FileMenuText);
	
	[Export]
    public static readonly MenuItemGroupDefinition FileNewOpenMenuGroup = new MenuItemGroupDefinition(FileMenu, 0);
	
	[Export]
    public static readonly MenuItemDefinition FileNewMenuItem = new TextMenuItemDefinition(
        MenuDefinitions.FileNewOpenMenuGroup, 0, "_New");
}
```

You can either use an existing menu or menu item group as a parent for your menu items, or create your own.

To remove an existing menu item (such as a built-in menu item that you don't want), you can exclude it declaratively:

``` csharp
[Export]
public static readonly ExcludeMenuItemDefinition ExcludeOpenMenuItem = new ExcludeMenuItemDefinition(Gemini.Modules.Shell.MenuDefinitions.FileOpenMenuItem);

[Export]
public static readonly ExcludeMenuItemGroupDefinition ExcludeWindowMenuItemGroup = new ExcludeMenuItemGroupDefinition(Gemini.Modules.MainMenu.MenuDefinitions.ViewToolsMenuGroup);

[Export]
public static readonly ExcludeMenuDefinition ExcludeWindowMenuDefinition = new ExcludeMenuDefinition(Gemini.Modules.MainMenu.MenuDefinitions.WindowMenu);
```

### StatusBar module

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-module-statusbar.png)

Adds a status bar to the bottom of the window.

#### Provides

* `IStatusBar`
* `StatusBarItemViewModel` class

#### NuGet package

* [Gemini](http://nuget.org/packages/GeminiWpf/)

#### Dependencies

* None

#### Usage

```csharp
var statusBar = IoC.Get<IStatusBar>();
statusBar.AddItem("Hello world!", new GridLength(1, GridUnitType.Star));
statusBar.AddItem("Ln 44", new GridLength(100));
statusBar.AddItem("Col 79", new GridLength(100));
```

### ToolBars module

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-module-toolbars.png)

Adds a toolbar tray to the top of the window. By default, the toolbar tray is hidden - use
`Shell.ToolBars.Visible = true` to show it.

#### NuGet package

* [Gemini](http://nuget.org/packages/GeminiWpf/)

#### Dependencies

* None

#### Usage

First, create commands, as described above in the "Commands" section. Then declare toolbars, toolbar item groups,
and toolbar items. This is how the standard toolbar and toolbar items are declared; you can create your own
toolbars in the same way.

```csharp
internal static class ToolBarDefinitions
{
	[Export]
	public static ToolBarDefinition StandardToolBar = new ToolBarDefinition(0, "Standard");
	
	[Export]
    public static ToolBarItemGroupDefinition StandardOpenSaveToolBarGroup = new ToolBarItemGroupDefinition(
        ToolBars.ToolBarDefinitions.StandardToolBar, 8);
		
	[Export]
    public static ToolBarItemDefinition OpenFileToolBarItem = new CommandToolBarItemDefinition<OpenFileCommandDefinition>(
        StandardOpenSaveToolBarGroup, 0);
}

// ...

Shell.ToolBars.Visible = true;
```

### Toolbox module

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-module-toolbox.png)

Reproduces the toolbox tool window from Visual Studio. Use the `[ToolboxItem]` attribute to provide
available items for listing in the toolbox. You specify the document type for each toolbox item.
When the user switches to a different document, Gemini manages showing only the toolbox items that 
are supported for the active document type. Items are listed in categories. 
The toolbox supports drag and drop.

#### Provides

* `IToolbox` tool window
* `ToolboxItemAttribute` attribute
* `ToolboxDragDrop` utility class

#### NuGet package

* [Gemini](http://nuget.org/packages/GeminiWpf/)

#### Dependencies

* None

#### Usage

```csharp
[ToolboxItem(typeof(GraphViewModel), "Image Source", "Generators")]
public class ImageSource : ElementViewModel
{
	// ...
}
```

Handling dropping onto a document (this code is from [`GraphView.xaml.cs`](https://github.com/tgjones/gemini/blob/master/src/Gemini.Demo.FilterDesigner/Modules/FilterDesigner/Views/GraphView.xaml.cs)):

```csharp
private void OnGraphControlDragEnter(object sender, DragEventArgs e)
{
    if (!e.Data.GetDataPresent(ToolboxDragDrop.DataFormat))
        e.Effects = DragDropEffects.None;
}

private void OnGraphControlDrop(object sender, DragEventArgs e)
{
    if (e.Data.GetDataPresent(ToolboxDragDrop.DataFormat))
    {
        var mousePosition = e.GetPosition(GraphControl);

        var toolboxItem = (ToolboxItem) e.Data.GetData(ToolboxDragDrop.DataFormat);
        var element = (ElementViewModel) Activator.CreateInstance(toolboxItem.ItemType);
        element.X = mousePosition.X;
        element.Y = mousePosition.Y;

        ViewModel.Elements.Add(element);
    }
}
```

### UndoRedo module

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-module-undoredo.png)

Provides a framework for adding undo/redo support to your application. An undo/redo stack is maintained
separately for each document. The screenshot above shows the history tool window. You can drag the slider
to move forward or backward in the document's history.

#### Provides

* `IHistoryTool` tool window
* `IUndoableAction` interface
* `UndoRedoToolbarItems` utility class

#### NuGet package

* [Gemini](http://nuget.org/packages/GeminiWpf/)

#### Dependencies

* None

#### Usage

First, define an action. The action needs to implement `IUndoableAction`:

```csharp
public class MyAction : IUndoableAction
{
    public string Name
    {
        get { return "My Action"; }
    }

    public void Execute()
    {
        // Do something
    }

    public void Undo()
    {
        // Put it back
    }
}
```

Then execute the action:

```csharp
var undoRedoManager = IoC.Get<IShell>().ActiveItem.UndoRedoManager;
undoRedoManager.ExecuteAction(new MyAction());
```

Now the action will be shown in the history tool window. If you are using the Undo or Redo menu items or 
toolbar buttons, they will also react appropriately to the action.

### CodeCompiler module

Uses Roslyn to compile C# code. Currently, `ICodeCompiler` exposes a very simple interface:

```csharp
public interface ICodeCompiler
{
    Assembly Compile(
        IEnumerable<SyntaxTree> syntaxTrees, 
        IEnumerable<MetadataReference> references,
        string outputName);
}
```

An interesting feature, made possible by Roslyn, is that the compiled assemblies are garbage-collectible.
This means that you can compile C# source code, run the resulting assembly in the same `AppDomain` as 
your main application, and then unload the assembly from memory. This would be very useful, for example, in
a game editor where you want the game preview window to update as soon as the user modifies a script 
source file.

#### Provides

* `ICodeCompiler` service

#### NuGet package

* [Gemini.Modules.CodeCompiler](http://nuget.org/packages/Gemini.Modules.CodeCompiler/)

#### Dependencies

* [Roslyn](http://msdn.microsoft.com/en-us/vstudio/roslyn.aspx)

#### Usage

This example is from [HelixViewModel](https://github.com/tgjones/gemini/blob/master/src/Gemini.Demo/Modules/Home/ViewModels/HelixViewModel.cs) in one of the sample applications.

```csharp
var newAssembly = _codeCompiler.Compile(
    new[] { SyntaxTree.ParseText(_helixView.TextEditor.Text) },
    new[]
    {
        MetadataReference.CreateAssemblyReference("mscorlib"),
        MetadataReference.CreateAssemblyReference("System"),
        MetadataReference.CreateAssemblyReference("PresentationCore"),
        new MetadataFileReference(typeof(IResult).Assembly.Location),
        new MetadataFileReference(typeof(AppBootstrapper).Assembly.Location),
        new MetadataFileReference(GetType().Assembly.Location)
    },
    "GeminiDemoScript");
```

Once there are no references to `newAssembly`, it will be eligible for garbage collection.

### CodeEditor module

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-module-codeeditor.png)

Uses AvalonEdit to provide syntax highlighting and other features for editing C# source files.

#### Provides

* `EditorProvider` for C# source files
* `CodeEditor` control

#### NuGet package

* [Gemini.Modules.CodeEditor](http://nuget.org/packages/Gemini.Modules.CodeEditor/)

#### Dependencies

* [AvalonEdit](https://github.com/icsharpcode/SharpDevelop/wiki/AvalonEdit)

#### Usage

Opening a file with a `.cs` extension will automatically use the `CodeEditor` module to display
the document. You can also use the `CodeEditor` control in your own views:

```xml
<codeeditor:CodeEditor SyntaxHighlighting="C#" />
```

### ErrorList module

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-module-errorlist.png)

Reproduces the error list tool window from Visual Studio. Can be used to show errors, warning, or information.

#### Provides

* `IErrorList` tool window

#### NuGet package

* [Gemini.Modules.ErrorList](http://nuget.org/packages/Gemini.Modules.ErrorList/)

#### Dependencies

* None

#### Usage

```csharp
var errorList = IoC.Get<IErrorList>();
errorList.Clear();
errorList.AddItem(
	ErrorListItemType.Error,
	"Description of the error",
    @"C:\MyFile.txt",
    1,   // Line
    20); // Column
```

You can optionally provide a callback that will be executed when the user double-clicks on an item:

```csharp
errorList.AddItem(
	ErrorListItemType.Error,
	"Description of the error",
    @"C:\MyFile.txt",
    1, // Line
    20, // Character
    () =>
    {
        var openDocumentResult = new OpenDocumentResult(@"C:\MyFile.txt");
        IoC.BuildUp(openDocumentResult);
        openDocumentResult.Execute(null);
    });
```

### GraphEditor module

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-module-grapheditor.png)

Implements a general purpose graph / node editing UI. This module provides the UI controls - the logic
and view models are usually specific to your application, and are left to you. The FilterDesigner sample
application (in the screenshot above) is one example of how it can be used.

Although I implemented it slightly differently, I got a lot of inspiration and some ideas for the code 
from Ashley Davis's [CodeProject article](http://www.codeproject.com/Articles/182683/NetworkView-A-WPF-custom-control-for-visualizing-a).

#### Provides

* `GraphControl` control
* `ConnectorItem` control
* `BezierLine` control
* `ZoomAndPanControl` control from [this CodeProject article](http://www.codeproject.com/Articles/85603/A-WPF-custom-control-for-zooming-and-panning)

#### NuGet package

* [Gemini.Modules.GraphEditor](http://nuget.org/packages/Gemini.Modules.GraphEditor/)

#### Dependencies

* None

#### Usage

You'll need to create view models to represent:

* the graph itself
* elements
* connectors
* connections.

I suggest looking at the 
[FilterDesigner sample application](https://github.com/tgjones/gemini/tree/master/src/Gemini.Demo.FilterDesigner)
to get an idea of what's involved.

### Inspector module

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-module-inspector.png)

Similar in purpose to the property grid, but the Inspector module takes a more flexible approach.
Instead of the strict "two-column / property per row" layout used in the standard PropertyGrid,
the Inspector module allows each editor to customise its own view.

It comes with the following editors:

* BitmapSource
* CheckBox
* CollapsibleGroup
* Color (WPF)
* Enum
* Point3D (WPF)
* Range
* TextBox

#### Provides

* `IInspectorTool` tool window
* `InspectableObjectBuilder` class

#### NuGet package

* [Gemini.Modules.Inspector](http://nuget.org/packages/Gemini.Modules.Inspector/)

#### Dependencies

* [Extended WPF Toolkit](http://wpftoolkit.codeplex.com/) (for the colour picker)

#### Usage

You can build up the inspector for an object in two ways:

* Convention-based. The Inspector module can reflect over an object and create editors for the properties whose 
  types it recognises. It comes with built-in editors for `int`, `string`, `Enum`, etc.
* Manually. Use the fluent interface on `InspectableObjectBuilder` to create editors.

You can also mix and match these approaches.

```csharp
var inspectorTool = IoC.Get<IInspectorTool>();
inspectorTool.SelectedObject = new InspectableObjectBuilder()
	.WithCollapsibleGroup("My Group", b => b
		.WithColorEditor(myObject, x => x.Color))
	.WithObjectProperties(Shell.ActiveItem, pd => true) // Automatically adds browsable properties.
	.ToInspectableObject();
```

### Output module

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-module-output.png)

Much like the output tool window from Visual Studio.

#### Provides

* `IOutput` tool window

#### NuGet package

* [Gemini.Modules.Output](http://nuget.org/packages/Gemini.Modules.Output/)

#### Dependencies

* None

#### Usage

```csharp
var output = IoC.Get<IOutput>();
output.AppendLine("Started up");
```

### PropertyGrid module

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-module-propertygrid.png)

Pretty much does what it says on the tin. It uses the PropertyGrid control from the
Extended WPF Toolkit.

#### Provides

* `IPropertyGrid` tool window

#### NuGet package

* [Gemini.Modules.PropertyGrid](http://nuget.org/packages/Gemini.Modules.PropertyGrid/)

#### Dependencies

* [Extended WPF Toolkit](http://wpftoolkit.codeplex.com/)

#### Usage

```csharp
var propertyGrid = IoC.Get<IPropertyGrid>();
propertyGrid.SelectedObject = myObject;
```

## Sample application

Gemini.Demo showcases many of the available modules. The screenshot below shows the interactive script editor in action -
as you type, the code will be compiled in real-time into a dynamic assembly and then executed in the same AppDomain.

It also includes a very basic example of a filter designer, built on the GraphEditor module.

* [Source code](https://raw.github.com/tgjones/gemini/master/src/Gemini.Demo)

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-demo.png)

## What projects use Gemini?

I've used Gemini on several of my own projects:

* [Meshellator](http://github.com/tgjones/meshellator)
* [Rasterizr](http://github.com/tgjones/rasterizr)
* [SlimShader](http://github.com/tgjones/slimshader)
* coming soon...

## Acknowledgements

* Many of the original ideas, and much of the early code came from [Rob Eisenberg](http://www.bluespire.com/), 
  creator of the [Caliburn Micro](http://caliburnmicro.codeplex.com/) framework. I have extended and modified 
  his code to integrate better with AvalonDock 2.0, which natively supports MVVM-style binding.
* I used the VS2010 theme from [Edi](http://edi.codeplex.com/).

Gemini is not the only WPF framework for building IDE-like applications. Here are some others:

* [SoapBox Core](http://soapboxautomation.com/products/soapbox-core-2/) - source [here](http://svn.soapboxcore.com/svn/),
  but I think this project might be dead.
* [Wide](https://github.com/chandramouleswaran/Wide/) - looks promising, and has a 
  [CodeProject article](http://www.codeproject.com/Articles/551885/How-to-create-a-VS-2012-like-application-Wide-IDE).
