using System.Windows;
using System.Windows.Controls;
using Gemini.Framework;

namespace Gemini.Modules.Shell.Controls
{
	public class PanesStyleSelector : StyleSelector
	{
		public Style ToolStyle
		{
			get;
			set;
		}

		public Style DocumentStyle
		{
			get;
			set;
		}

		public override Style SelectStyle(object item, DependencyObject container)
		{
			if (item is ITool)
				return ToolStyle;

			if (item is IDocument)
				return DocumentStyle;

			return base.SelectStyle(item, container);
		}
	}
}