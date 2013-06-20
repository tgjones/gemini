# Gemini

## What is this?

Gemini is a WPF framework designed specifically for building IDE-like applications. It builds on some excellent libraries:

* [AvalonDock](http://avalondock.codeplex.com)
* [Caliburn Micro](http://caliburnmicro.codeplex.com/)
* [MEF](http://msdn.microsoft.com/en-us/library/dd460648.aspx)

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

For full details, look at the [demo program](https://github.com/roastedamoeba/gemini/tree/master/src/Gemini.Demo).

## What modules are built-in?

Gemini itself is built out of six core modules:

* [Shell](#module-shell)
* [MainMenu](#module-mainmenu)
* [StatusBar](#module-statusbar)
* [ToolBars](#module-toolbars)
* [Toolbox](#module-toolbox)
* [UndoRedo](#module-undoredo)

Several more modules ship with Gemini, and are available as 
[NuGet packages](http://nuget.org/packages?q=Gemini.Modules) as described above:

* [CodeCompiler](#module-codecompiler)
* [CodeEditor](#module-codeeditor)
* [ErrorList](#module-errorlist)
* [GraphEditor](#module-grapheditor)
* [Inspector](#module-inspector)
* [Inspector.Xna](#module-inspector-xna)
* [Output](#module-output)
* [PropertyGrid](#module-propertygrid)
* [Xna](#module-xna)

### <a id="module-shell"></a>Shell module

TODO

### <a id="module-mainmenu"></a>MainMenu module

TODO

### <a id="module-statusbar"></a>StatusBar module

TODO

### <a id="module-toolbars"></a>ToolBars module

TODO

### <a id="module-toolbox"></a>Toolbox module

TODO

### <a id="module-undoredo"></a>UndoRedo module

TODO

### <a id="module-codecompiler"></a>CodeCompiler module

TODO

### <a id="module-codeeditor"></a>CodeEditor module

TODO

### <a id="module-errorlist"></a>ErrorList module

TODO

### <a id="module-grapheditor"></a>GraphEditor module

TODO

### <a id="module-inspector"></a>Inspector module

TODO

### <a id="module-inspector-xna"></a>Inspector.Xna module

TODO

### <a id="module-output"></a>Output module

TODO

### <a id="module-propertygrid"></a>PropertyGrid module

TODO

### <a id="module-xna"></a>Xna module

TODO

## Sample applications

Gemini's source code includes three sample applications:

* [Gemini.Demo](https://github.com/tgjones/gemini/tree/master/src/Gemini.Demo) - 
  showcases many of the available modules.
  ![Screenshot](doc/gemini-demo.png)
* [Gemini.Demo.FilterDesigner](https://github.com/tgjones/gemini/tree/master/src/Gemini.Demo.FilterDesigner) - 
  showcases the GraphEditor, Inspector and Toolbox modules.
  ![Screenshot](doc/gemini-demo-filter-designer.png)
* [Gemini.Demo.Xna](https://github.com/tgjones/gemini/tree/master/src/Gemini.Demo.Xna) - 
  showcases the Xna module.
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