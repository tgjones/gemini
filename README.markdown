# Gemini

### What is this?

Gemini is a WPF framework designed specifically for building IDE-like applications. It is built on:

* [AvalonDock](http://avalondock.codeplex.com)
* [Caliburn Micro](http://caliburnmicro.codeplex.com/)
* MEF

![Screenshot](https://github.com/roastedamoeba/gemini/raw/master/doc/screenshot.PNG)

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

### Acknowledgements

Much of the original ideas and code came from [Rob Eisenberg](http://www.bluespire.com/), creator of the [Caliburn Micro](http://caliburnmicro.codeplex.com/) framework. I have extended and modified his code to integrate better with AvalonDock 2.0, which natively supports MVVM-style binding.