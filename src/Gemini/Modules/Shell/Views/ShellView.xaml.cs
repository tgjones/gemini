using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using AvalonDock;
using Caliburn.Micro;
using Gemini.Framework.Services;

namespace Gemini.Modules.Shell.Views
{
	/// <summary>
	/// Interaction logic for ShellView.xaml
	/// </summary>
	public partial class ShellView : Window, IShellView
	{
		public event EventHandler ActiveDocumentChanged;

		private readonly Dictionary<PaneLocation, DockablePane> _panes;

		public ShellView()
		{
			InitializeComponent();

			_panes = new Dictionary<PaneLocation, DockablePane>
			{
				{ PaneLocation.Left, LeftPane },
				{ PaneLocation.Right, RightPane },
				{ PaneLocation.Bottom, BottomPane }
			};
		}

		public void ShowTool(PaneLocation pane, IScreen model)
		{
			var view = ViewLocator.LocateForModel(model, null, null);

			if (view is DockableContent)
			{
				DockableContent dockableContentView = (DockableContent)view;
				dockableContentView.Show(
					Manager,
					(AnchorStyle)Enum.Parse(typeof(AnchorStyle), pane.ToString()));
			}
			else
			{
				var content = CreateContent<DockableContent>(view, model);
				Bind.SetModel(content, model);

				var host = _panes[pane];
				host.Items.Add(content);

				Refresh();
			}
		}

		public void OpenDocument(IScreen model)
		{
			var view = ViewLocator.LocateForModel(model, null, null);

			if (view is DocumentContent)
			{
				((DocumentContent) view).Show(Manager);
				((DocumentContent) view).Activate();
			}
			else
			{
				var content = CreateContent<DocumentContent>(view, model);
				Bind.SetModel(content, model);

				content.PropertyChanged += HostPropertyChanged;
				content.Closing += (s, e) => HostClosing(model, e);

				DocumentPane.Items.Add(content);

				model.Deactivated += (sender, args) =>
				{
					if (args.WasClosed)
						DocumentPane.Items.Remove(content);
				};

				//content.SetAsActive();
				Manager.ActiveDocument = content;
			}
		}

		private T CreateContent<T>(object view, IScreen model)
				where T : ManagedContent, new()
		{
			var contentControl = new T { Content = view };

			contentControl.SetBinding(ManagedContent.TitleProperty, "DisplayName");

			ViewModelBinder.Bind(model, contentControl, null);
			foreach (var binding in InputBindings)
				contentControl.InputBindings.Add((InputBinding) binding);
			foreach (var binding in CommandBindings)
				contentControl.CommandBindings.Add((CommandBinding) binding);

			return contentControl;
		}

		private void Refresh()
		{
			//HACK: For some reason, the layout doesn't update unless I change the size...
			SetValue(WidthProperty, ActualWidth + 1);
			SetValue(WidthProperty, ActualWidth - 1);
		}

		private void HostPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != "IsActiveDocument") return;

			var host = (ManagedContent)sender;

			if (host.IsActiveDocument)
				((IShell)DataContext).ActivateDocument((IScreen)host.DataContext);
		}

		private void HostClosing(IScreen rootModel, CancelEventArgs e)
		{
			((IShell)DataContext).CloseDocument(rootModel);
		}
	}
}
