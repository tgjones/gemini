using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using Gemini.Contracts;
using Gemini.Contracts.Application.Startup;
using Gemini.Contracts.Services.ExtensionService;
using Gemini.Contracts.Services.LoggingService;

namespace Gemini.Host
{
	public abstract class GeminiApplication : Application, IPartImportsSatisfiedNotification
	{
		private List<ResourceDictionary> _addedResourceDictionaries = new List<ResourceDictionary>();
		private CompositionContainer _container;

		public CompositionContainer Container
		{
			get { return _container; }
		}

		/// <summary>
		/// Main WPF startup window for the application
		/// </summary>
		[Import(ContractNames.CompositionPoints.Host.MainWindow, typeof(Window))]
		public new Window MainWindow
		{
			get { return base.MainWindow; }
			set { base.MainWindow = value; }
		}

		/// <summary>
		/// This imports resource dictionaries for Styles so they're
		/// all added to the application resources.
		/// These get imported before the Views.
		/// </summary>
		[ImportMany(ContractNames.ExtensionPoints.Host.Styles, typeof(ResourceDictionary), AllowRecomposition = true)]
		private IEnumerable<ResourceDictionary> Styles { get; set; }

		/// <summary>
		/// This imports resource dictionaries for Views so they're
		/// all added to the application resources.
		/// In general these should be full of DataTemplates for 
		/// displaying ViewModel classes.
		/// </summary>
		[ImportMany(ContractNames.ExtensionPoints.Host.Views, typeof(ResourceDictionary), AllowRecomposition = true)]
		private IEnumerable<ResourceDictionary> Views { get; set; }

		/// <summary>
		/// Hosts a logging service
		/// </summary>
		[Import(ContractNames.Services.Logging.LoggingService, typeof(ILoggingService))]
		public ILoggingService logger { get; set; }

		/// <summary>
		/// This imports any commands that are supposed to run when
		/// the application starts.
		/// </summary>
		[ImportMany(ContractNames.ExtensionPoints.Host.StartupCommands, typeof(IExecutableCommand), AllowRecomposition = true)]
		private IEnumerable<IExecutableCommand> StartupCommands { get; set; }

		/// <summary>
		/// This imports any commands that are supposed to run when
		/// the application is shutdown.
		/// </summary>
		[ImportMany(ContractNames.ExtensionPoints.Host.ShutdownCommands, typeof(IExecutableCommand), AllowRecomposition = true)]
		private IEnumerable<IExecutableCommand> ShutdownCommands { get; set; }

		/// <summary>
		/// This imports things that just want to be part of the composition.
		/// </summary>
		[ImportMany(ContractNames.ExtensionPoints.Host.Void, typeof(Object), AllowRecomposition = true)]
		private IEnumerable<Object> VoidObjects { get; set; }

		/// <summary>
		/// We need this to sort ordered extentions, like the startup commands
		/// </summary>
		[Import(ContractNames.Services.Host.ExtensionService)]
		private IExtensionService ExtensionService { get; set; }

		protected override void OnStartup(StartupEventArgs e)
		{
			// DON'T USE LOGGER HERE.  It's not composed yet.
			base.OnStartup(e);

			// Add global resources.
			//Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("/AvalonDock.Themes;component/themes/ExpressionDark.xaml") });
			Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/AvalonDock.Themes;component/themes/dev2010.xaml") });
			//Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("Resources/ExpressionDarkTheme.xaml") });

			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			if (Compose())
			{
				stopWatch.Stop();

				// Now we can use logger
				logger.InfoWithFormat("Composition complete...({0} milliseconds)", stopWatch.ElapsedMilliseconds);

				logger.Info("Showing Main Window...");
				MainWindow.Show();
			}
			else
			{
				Shutdown();
			}
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			// Run all the shutdown commands
			IList<IExecutableCommand> commands = ExtensionService.Sort(ShutdownCommands);
			foreach (IExecutableCommand cmd in commands)
			{
				logger.Info("Running shutdown command " + cmd.ID + "...");
				try
				{
					cmd.Run();
				}
				catch (Exception ex)
				{
					logger.Error("Exception while running command " + cmd.ID, ex);
				}
				logger.Info("Shutdown command " + cmd.ID + " completed.");
			}

			Thread.Sleep(250); // Give threads and other parts of the app a bit of time to
			// end gracefully.

			if (_container != null)
			{
				_container.Dispose();
			}
		}

		/// <summary>
		/// Don't use logger in here!  It's not composed yet.
		/// </summary>
		/// <returns>True if successful</returns>
		private bool Compose()
		{
			var catalog = new AggregateCatalog();
			catalog.Catalogs.Add(new DirectoryCatalog(".", "*.dll"));
			catalog.Catalogs.Add(new DirectoryCatalog(".", "*.exe"));

			_container = new CompositionContainer(catalog);

			try
			{
				_container.ComposeParts(this);
			}
			catch (CompositionException compositionException)
			{
				MessageBox.Show(compositionException.ToString());
				return false;
			}
			return true;
		}

		private bool m_startupCommandsRun = false;

		public void OnImportsSatisfied()
		{
			// Add the imported resource dictionaries
			// to the application resources
			if (_addedResourceDictionaries.Count > 0)
			{
				// in case of recompose
				foreach (ResourceDictionary rd in _addedResourceDictionaries)
					Resources.MergedDictionaries.Remove(rd);
				_addedResourceDictionaries.Clear();
			}
			logger.Info("Importing Styles...");
			foreach (ResourceDictionary r in Styles)
			{
				_addedResourceDictionaries.Add(r);
				this.Resources.MergedDictionaries.Add(r);
			}
			logger.Info("Importing Views...");
			foreach (ResourceDictionary r in Views)
			{
				_addedResourceDictionaries.Add(r);
				this.Resources.MergedDictionaries.Add(r);
			}

			if (!m_startupCommandsRun) // Don't run on recomposition
			{
				m_startupCommandsRun = true;
				// Run all the startup commands
				IList<IExecutableCommand> commands = ExtensionService.Sort(StartupCommands);
				foreach (IExecutableCommand cmd in commands)
				{
					logger.Info("Running startup command " + cmd.ID + "...");
					try
					{
						cmd.Run();
					}
					catch (Exception ex)
					{
						logger.Error("Exception while running command " + cmd.ID, ex);
					}
					logger.Info("Startup command " + cmd.ID + " completed.");
				}
			}
		}
	}
}