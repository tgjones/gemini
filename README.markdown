# Gemini

### What is this?

![Screenshot](https://raw.github.com/roastedamoeba/nexus/master/doc/screenshot.png)

Gemini is a WPF framework designed specifically for building IDE-like applications. It is built on:

* AvalonDock
* Caliburn Micro
* MEF

### What does it do?

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

### What modules are built-in?

Gemini comes with three modules:

* An output pane
* A property grid pane
* A status bar

### What projects use Gemini?

I've used Gemini on several of my own projects:

* [Meshellator](http://github.com/roastedamoeba/meshellator)
* [Rasterizr](http://github.com/roastedamoeba/rasterizr)
* coming soon...