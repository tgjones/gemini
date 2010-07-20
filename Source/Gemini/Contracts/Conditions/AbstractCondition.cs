using System;

namespace Gemini.Contracts.Conditions
{
	public abstract class AbstractCondition : ICondition
	{
		public event EventHandler ConditionChanged = delegate { };
		public bool Condition
		{
			get
			{
				return m_Condition;
			}
			protected set
			{
				if (m_Condition != value)
				{
					m_Condition = value;
					ConditionChanged(this, new EventArgs());
				}
			}
		}
		private bool m_Condition = false;
	}
}