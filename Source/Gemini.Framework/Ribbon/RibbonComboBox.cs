using System;
using System.Collections;
using System.Collections.Generic;
using Caliburn.PresentationFramework;

namespace Gemini.Framework.Ribbon
{
	public class RibbonComboBox : RibbonButtonBase<RibbonComboBox>
	{
		private readonly Func<object, IEnumerable<IResult>> _execute;
		private readonly IEnumerable _items;
		private object _selectedItem;

		public object SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				NotifyOfPropertyChange(() => SelectedItem);
				if (_execute != null)
					new SequentialResult(_execute.Invoke(_selectedItem)).Execute(null, null);
			}
		}

		public IEnumerable Items
		{
			get { return _items; }
		}

		public RibbonComboBox(string text)
			: base(text)
		{
			
		}

		public RibbonComboBox(string text, Func<object, IEnumerable<IResult>> execute)
			: base(text)
		{
			_execute = execute;
		}

		public RibbonComboBox(string text, Func<object, IEnumerable<IResult>> execute, Func<bool> canExecute, IEnumerable items)
			: base(text, canExecute)
		{
			_execute = execute;
			_items = items;
		}
	}
}