# Gemini

## What is this?

Gemini is a WPF framework designed specifically for building IDE-like applications. It builds on some excellent libraries:

* [AvalonDock](http://avalondock.codeplex.com)
* [Caliburn Micro](http://caliburnmicro.codeplex.com/)
* [MEF](http://msdn.microsoft.com/en-us/library/dd460648.aspx)

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-everything.png)

## Getting started

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
* [Gemini.Modules.Inspector.Xna](http://nuget.org/packages/Gemini.Modules.Inspector.Xna/)
* [Gemini.Modules.Output](http://nuget.org/packages/Gemini.Modules.Output/)
* [Gemini.Modules.PropertyGrid](http://nuget.org/packages/Gemini.Modules.PropertyGrid/)
* [Gemini.Modules.Xna](http://nuget.org/packages/Gemini.Modules.Xna/)

## What does it do?

Gemini allows you to build your WPF application by composing separate modules. This provides a nice
way of separating out the code for each part of your application. For example, here is the (very simple)
module used in the demo program:

```csharp
[Export(typeof(IModule))]
public class Module : ModuleBase
{
	[Import]
	private IPropertyGrid _propertyGrid;

	public override void Initialize()
	{
		MainMenu.All
			.First(x => x.Name == "View")
			.Add(new MenuItem("Home", OpenHome));

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

For full details, look at the [demo program](https://raw.github.com/tgjones/gemini/master/src/Gemini.Demo).

## What modules are built-in?

Gemini itself is built out of six core modules:

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
* Inspector.Xna
* Output
* PropertyGrid
* Xna

For more information about these modules, see below. In general, each module adds some combination
of menu items, tool window, document types and services.

### Shell module

TODO

### MainMenu module

TODO

### StatusBar module

TODO

### ToolBars module

TODO

### Toolbox module

TODO

### UndoRedo module

TODO

### CodeCompiler module

TODO

### CodeEditor module

TODO

### ErrorList module

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-module-errorlist.png)

Reproduces the error list tool window in Visual Studio. Can be used to show errors, warning, or information.

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

TODO

### Inspector module

TODO

### Inspector.Xna module

TODO

### Output module

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-module-output.png)

Much like the output tool window in Visual Studio.

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

### Xna module

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-module-xna.png)

Provides a number of utilities and controls for working with XNA content in WPF. In the screenshot above,
the document on the left uses `DrawingSurface`, and the tool window on the right uses `GraphicsDeviceControl`.
Note that the `GraphicsDeviceControl` is clipped correctly against its parent `ScrollViewer` bounds.

#### Provides

* `GraphicsDeviceService` service that implements XNA's `IGraphicsDeviceService`
* `ClippingHwndHost` control that clips hosted Win32 content to a WPF control's bounds

The Xna module includes 2 alternatives for hosting XNA content in WPF:

* `DrawingSurface` control that uses `D3DImage` as described
  [here](http://blog.bozalina.com/2010/11/xna-40-and-wpf.html).
* `GraphicsDeviceControl` control that implements Nick Gravelyn's technique for hosting
  WPF content using an HwndHost, described [here](http://blogs.msdn.com/b/nicgrave/archive/2011/03/25/wpf-hosting-for-xna-game-studio-4-0.aspx)

#### NuGet package

* [Gemini.Modules.Xna](http://nuget.org/packages/Gemini.Modules.Xna/)

#### Dependencies

* [XNA 4.0](http://www.microsoft.com/en-us/download/details.aspx?id=23714)

#### Usage

Both `DrawingSurface` and `GraphicsDeviceControl` provide similar APIs, but they are
subtly different. `DrawingSurface` works seamlessly with WPF mouse and keyboard input,
but `GraphicsDeviceControl` routes mouse input through its own set of methods
(`RaiseHwndLButtonDown` etc.).

```csharp
public class MyDrawingSurface : DrawingSurface
{
    protected override RaiseDraw(DrawEventArgs args)
    {
        args.GraphicsDevice.Clear(Color.LightGreen);
        base.RaiseDraw(args);
    }
}
```

```csharp
public class MyGraphicsDeviceControl : GraphicsDeviceControl
{
    protected override void RaiseRenderXna(GraphicsDeviceEventArgs args)
    {
        args.GraphicsDevice.Clear(Color.LightGreen);
        base.RaiseRenderXna(args);
    }
}
```

## Sample applications

Gemini includes three sample applications:

### Gemini.Demo

Showcases many of the available modules.

* [Source code](https://raw.github.com/tgjones/gemini/master/src/Gemini.Demo)

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-demo.png)

### Gemini.Demo.FilterDesigner

Showcases the GraphEditor, Inspector and Toolbox modules.

* [Source code](https://raw.github.com/tgjones/gemini/master/src/Gemini.Demo.FilterDesigner)

![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-demo-filter-designer.png)

### Gemini.Demo.Xna

Showcases the Xna module.

* [Source code](https://raw.github.com/tgjones/gemini/master/src/Gemini.Demo.Xna)
  
![Screenshot](https://raw.github.com/tgjones/gemini/master/doc/gemini-demo-xna.png)

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