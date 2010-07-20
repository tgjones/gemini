namespace Gemini.Contracts
{
	public static class ContractNames
	{
		public static class CompositionPoints
		{
			public static class Host
			{
				public const string MainWindow = "CompositionPoints.Host.MainWindow";
			}

			public static class Workbench
			{
				public const string ViewModel = "CompositionPoints.Workbench";

				public static class Pads
				{
					public const string PropertyGrid = "CompositionPoints.Workbench.Pads.PropertyGrid";
					public const string Output = "CompositionPoints.Workbench.Pads.Output";
					public const string Toolbox = "CompositionPoints.Workbench.Pads.Toolbox";
				}
			}

			public static class Options
			{
				public const string OptionsDialog = "CompositionPoints.Options.OptionsDialog";
			}
		}

		public static class Services
		{
			public static class Host
			{
				public const string ExtensionService = "Services.Host.ExtensionService";
			}

			public static class Logging
			{
				public const string LoggingService = "Services.Logging.LoggingService";
			}

			public static class Layout
			{
				public const string LayoutManager = "Services.Layout.LayoutManager";
			}

			public static class FileDialog
			{
				public const string FileDialogService = "Services.FileDialog.FileDialogService";
			}

			public static class Messaging
			{
				public const string MessagingService = "Services.Messaging.MessagingService";
			}

			public static class Output
			{
				public const string OutputService = "Services.Output.OutputService";
			}

			public static class PropertyGrid
			{
				public const string PropertyGridService = "Services.PropertyGrid.PropertyGridService";
			}
		}

		public static class Extensions
		{
			public static class Workbench
			{
				public static class MainMenu
				{
					public const string File = "File";
					public const string Edit = "Edit";
					public const string View = "View";
					public const string Tools = "Tools";
					public const string Window = "Window";
					public const string Help = "Help";

					public static class FileMenu
					{
						public const string Exit = "Exit";
					}

					public static class ViewMenu
					{
						public const string ToolBars = "ToolBars";
					}

					public static class ToolsMenu
					{
						public const string Options = "Options";
					}
				}
			}
		}

		public static class ExtensionPoints
		{
			public static class Host
			{
				public const string Styles = "ExtensionPoints.Host.Styles";
				public const string Views = "ExtensionPoints.Host.Views";
				public const string StartupCommands = "ExtensionPoints.Host.StartupCommands";
				public const string ShutdownCommands = "ExtensionPoints.Host.ShutdownCommands";
				public const string Void = "ExtensionPoints.Host.Void";
			}

			public static class Workbench
			{
				public const string StatusBar = "ExtensionPoints.Workbench.StatusBar";
				public const string Pads = "ExtensionPoints.Workbench.Pads";
				public const string Documents = "ExtensionPoints.Workbench.Documents";

				public static class Ribbon
				{
					public const string Backstage = "ExtensionPoints.Workbench.Ribbon.Backstage";
					public const string Tabs = "ExtensionPoints.Workbench.Ribbon.Tabs";
					public const string HomeTab = "ExtensionPoints.Workbench.Ribbon.HomeTab";
					public const string ViewTab = "ExtensionPoints.Workbench.Ribbon.ViewTab";

					public static class HomeTabGroups
					{
						public const string ClipboardGroup = "ExtensionPoints.Workbench.Ribbon.HomeTabGroups.ClipboardGroup";
					}
				}
			}
		}
	}
}