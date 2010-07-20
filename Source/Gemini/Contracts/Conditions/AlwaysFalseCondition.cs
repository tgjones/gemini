using System;

namespace Gemini.Contracts.Conditions
{
	public sealed class AlwaysFalseCondition : ICondition
	{
		public event EventHandler ConditionChanged = delegate { };
		public bool Condition { get { return false; } }
	}
}