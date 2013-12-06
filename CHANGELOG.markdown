# Overview

## 0.4.0 - 2013-09-05

### Major Changes

* Added `MainWindowViewModel` as the new top-level view model. It contains a single instance
  of `ShellViewModel`. This allows modules to change the window implementation (i.e. to a 
  Metro-style window) without changing the AvalonDock-related code.

### New Features

* \#10 Added support for custom themes (Jonathan Lima)

* \#11 Added support for opening windows (Luuk Sommers)

* \#13 Enhanced layout serialization - it now supports documents and layout item state serialization (Roman Novitsky)

* \#17 Support more file types in CodeEditor (Roman Novitsky)

* Added Metro module - uses MahApps.Metro and AvalonDock's Metro theme.

* \#19 Added width and height properties to MainWindowViewModel

### Resolved Issues

* \#12 Opening tool if already loaded on startup causes multiple tools

* \#16 StatusBar items do not draw to the correct column

* \#21 Tools are not highlighted when opened programmatically

## 0.3.0 - 2013-06-21

### Major Changes

* Added CodeCompiler module

* Added CodeEditor module

* Added ErrorList module

* Added GraphEditor module

* Added Inspector module

* Added Inspector.Xna module

* Added Xna module

## 0.2.0 - 2013-05-07

### Major Changes

* The Output and PropertyGrid modules are now separate projects, and separate NuGet packages. Gemini itself
  now depends only on AvalonDock and Caliburn.Micro.

### Resolved Issues

* \#5 Provide a way to override ShellViewModel, and other built-in exports (rumata28).

* \#6 Fixed adding two panels on the same side during initialization (luuksommers).