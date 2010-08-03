using System;
using System.Windows;
using Caliburn.Core;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Gemini.Framework;
using Gemini.Framework.Questions;
using Gemini.Framework.Services;
using Microsoft.Practices.ServiceLocation;

namespace Gemini
{
	public class GeminiApplication : CaliburnApplication
	{
		protected override IServiceLocator CreateContainer()
		{
			return new Registry().CreateContainer();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;Component/Themes/Office2010/Blue.xaml") });
			Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Gemini;Component/Resources/Primitives.xaml") });
			Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Gemini;Component/Resources/Styles.xaml") });

			var binder = (DefaultBinder) Container.GetInstance<IBinder>();
			binder.EnableMessageConventions();

			var modules = Container.GetAllInstances<IModule>();
			modules.Apply(x => x.Configure((IContainer) Container));

			var shell = Container.GetInstance<IShell>();

			shell.AfterViewLoaded += delegate
			{
				modules.Apply(x => x.Start());
				shell.OnModulesInitialized(EventArgs.Empty);
			};

			var routedMessageController = Container.GetInstance<IRoutedMessageController>();
			routedMessageController.SetupDefaults(
				new GenericInteractionDefaults<Fluent.Button>(
					"Click",
					(b, v) => b.DataContext = v,
					b => b.DataContext));

			base.OnStartup(e);
		}

		protected override object CreateRootModel()
		{
			return Container.GetInstance<IShell>();
		}

		protected override void ExecuteShutdownModel(ISubordinate model, Action completed)
		{
			model.Execute(completed);
		}
	}
}