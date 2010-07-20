using System;

namespace Gemini.Contracts.Conditions
{
	public sealed class AlwaysTrueCondition : ICondition
	{
		public event EventHandler ConditionChanged = delegate { };
		public bool Condition { get { return true; } }
	}
}