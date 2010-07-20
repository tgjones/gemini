using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Gemini.Contracts;
using Gemini.Contracts.Gui.Layout;
using Gemini.Contracts.Utilities;

namespace Gemini.Pads.Output
{
	[Export(ContractNames.ExtensionPoints.Workbench.Pads, typeof(IPad))]
	[Export(ContractNames.CompositionPoints.Workbench.Pads.Output, typeof(OutputPad))]
	[Pad(Name = PadName)]
	public class OutputPad : AbstractPad
	{
		public event EventHandler TextChanged;

		public const string PadName = "Output";

		public OutputPad()
		{
			Name = PadName;
			Title = "Output";
		}

		#region Text property

		/// <summary>
		/// Used to uniquely identify the layout item.
		/// </summary>
		public string Text
		{
			get { return _text; }
			protected set
			{
				if (value == null)
					throw new ArgumentNullException();
				if (_text != value)
				{
					_text = value;
					NotifyPropertyChanged(TextArgs);
					if (TextChanged != null)
						TextChanged(this, EventArgs.Empty);
				}
			}
		}
		private string _text = string.Empty;
		private static readonly PropertyChangedEventArgs TextArgs = NotifyPropertyChangedHelper.CreateArgs<OutputPad>(o => o.Text);

		#endregion

		public void ClearText()
		{
			Text = string.Empty;
		}

		public void AppendText(string text)
		{
			Text += text + Environment.NewLine;
		}
	}
}