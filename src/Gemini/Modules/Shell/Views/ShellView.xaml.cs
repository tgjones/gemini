using System;
using System.IO;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using Gemini.Framework;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Gemini.Modules.Shell.Views
{
    /// <summary>
	/// Interaction logic for ShellView.xaml
	/// </summary>
	public partial class ShellView : Window, IShellView
    {
	    public ShellView()
		{
			InitializeComponent();
		}

	    private void OnWindowUnloaded(object sender, RoutedEventArgs e)
	    {
	        SaveLayout();
	    }

        public void LoadLayout()
        {
            var layoutSerializer = new XmlLayoutSerializer(Manager);
            layoutSerializer.LayoutSerializationCallback += (s, e) =>
            {
                var anchorable = e.Model as LayoutAnchorable;

                if (anchorable != null)
                {
                    var toolType = Type.GetType(e.Model.ContentId);
                    if (toolType != null)
                    {
                        var tool = IoC.GetInstance(toolType, null) as ITool;
                        if (tool != null)
                        {
                            e.Content = tool;
                            tool.IsVisible = anchorable.IsVisible;
                            if (anchorable.IsActive)
                                tool.Activate();
                            tool.IsSelected = anchorable.IsSelected;
                        }
                    }
                }
            };
            try
            {
                layoutSerializer.Deserialize(LayoutUtility.LayoutFile);
            }
            catch
            {
            }
        }

        private void SaveLayout()
        {
            var layoutSerializer = new XmlLayoutSerializer(Manager);
            layoutSerializer.Serialize(LayoutUtility.LayoutFile);
        }
	}
}
