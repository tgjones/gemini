using System;

namespace Gemini.Contracts.Conditions
{
	public interface ICondition
	{
		event EventHandler ConditionChanged;
		bool Condition { get; }
	}
}