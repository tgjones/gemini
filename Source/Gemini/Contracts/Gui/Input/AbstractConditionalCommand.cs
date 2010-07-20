using System;
using System.ComponentModel;
using System.Windows.Input;
using Gemini.Contracts.Conditions;
using Gemini.Contracts.Utilities;

namespace Gemini.Contracts.Gui.Input
{
	public abstract class AbstractConditionalCommand : ICommand
	{
		public event EventHandler CanExecuteChanged = delegate { };

		public virtual bool CanExecute(object parameter)
		{
			return EnableCondition.Condition;
		}

		public virtual void Execute(object parameter)
		{
			Run();
		}

		/// <summary>
		/// This method is called when the command is executed.
		/// Override this in the derived class to actually do something.
		/// </summary>
		protected virtual void Run() { }

		#region " EnableCondition "
		/// <summary>
		/// Defaults to AlwaysTrueCondition.
		/// Set this to any ISoapBoxCondition object, and it will control
		/// the CanExecute property from the ICommand interface, and 
		/// will raise the CanExecuteChanged event when appropriate.
		/// </summary>
		public ICondition EnableCondition
		{
			get
			{
				if (m_EnableCondition == null)
				{
					// Lazy initialize this property.
					// We could do this in the constructor, but 
					// I like having it all contained in one
					// section of code.
					EnableCondition = new AlwaysTrueCondition();
				}
				return m_EnableCondition;
			}
			protected set
			{
				if (value == null)
				{
					throw new ArgumentNullException(m_EnableConditionName);
				}
				if (m_EnableCondition != value)
				{
					if (m_EnableCondition != null)
					{
						//remove the old event handler
						m_EnableCondition.ConditionChanged -= OnEnableConditionChanged;
					}
					m_EnableCondition = value;
					//add the new event handler
					m_EnableCondition.ConditionChanged += OnEnableConditionChanged;
					CanExecuteChanged(this, new EventArgs());

					//NotifyPropertyChanged(m_EnableConditionArgs);
				}
			}
		}
		private ICondition m_EnableCondition = null;
		static readonly PropertyChangedEventArgs m_EnableConditionArgs =
				NotifyPropertyChangedHelper.CreateArgs<AbstractConditionalCommand>(o => o.EnableCondition);
		static readonly string m_EnableConditionName =
				NotifyPropertyChangedHelper.GetPropertyName<AbstractConditionalCommand>(o => o.EnableCondition);
		private void OnEnableConditionChanged(object sender, EventArgs e)
		{
			CanExecuteChanged(sender, e);
		}
		#endregion


	}
}