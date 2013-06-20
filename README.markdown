# Gemini

## What is this?

Gemini is a WPF framework designed specifically for building IDE-like applications. It builds on some excellent libraries:

* [AvalonDock](http://avalondock.codeplex.com)
* [Caliburn Micro](http://caliburnmicro.codeplex.com/)
* [MEF](http://msdn.microsoft.com/en-us/library/dd460648.aspx)

![Screenshot](doc/gemini-everything.png)

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

For full details, look at the [demo program](src/Gemini.Demo).

## What modules are built-in?

Gemini itself is built out of six core modules:

* [Shell](#shell-module)
* [MainMenu](#mainmenu-module)
* [StatusBar](#statusbar-module)
* [ToolBars](#toolbars-module)
* [Toolbox](#toolbox-module)
* [UndoRedo](#undoredo-module)

Several more modules ship with Gemini, and are available as 
[NuGet packages](http://nuget.org/packages?q=Gemini.Modules) as described above:

* [CodeCompiler](#codecompiler-module)
* [CodeEditor](#codeeditor-module)
* [ErrorList](#errorlist-module)
* [GraphEditor](#grapheditor-module)
* [Inspector](#inspector-module)
* [Inspector.Xna](#inspectorxna-module)
* [Output](#output-module)
* [PropertyGrid](#propertygrid-module)
* [Xna](#xna-module)

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

TODO

### GraphEditor module

TODO

### Inspector module

TODO

### Inspector.Xna module

TODO

### Output module

TODO

### PropertyGrid module

![Screenshot](doc/gemini-module-propertygrid.png)

#### Dependencies

* [Extended WPF Toolkit](http://wpftoolkit.codeplex.com/)

#### Usage

```csharp
var propertyGrid = IoC.Get<IPropertyGrid>();
propertyGrid.SelectedObject = myObject;
```

### Xna module

TODO

## Sample applications

Gemini includes three sample applications:

### Gemini.Demo

Showcases many of the available modules.

* [Source code](src/Gemini.Demo)

![Screenshot](doc/gemini-demo.png)

### Gemini.Demo.FilterDesigner

Showcases the GraphEditor, Inspector and Toolbox modules.

* [Source code](src/Gemini.Demo.FilterDesigner)

![Screenshot](doc/gemini-demo-filter-designer.png)

### Gemini.Demo.Xna

Showcases the Xna module.

* [Source code](src/Gemini.Demo.Xna)
  
![Screenshot](doc/gemini-demo-xna.png)

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