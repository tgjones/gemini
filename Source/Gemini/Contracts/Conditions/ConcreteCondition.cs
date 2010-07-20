namespace Gemini.Contracts.Conditions
{
	/// <summary>
	/// Helper class to allow us to create conditions 
	/// and control them.
	/// </summary>
	public class ConcreteCondition : AbstractCondition
	{
		public ConcreteCondition()
		{
		}

		public ConcreteCondition(bool condition)
		{
			Condition = condition;
		}

		public void ToggleCondition()
		{
			Condition = !Condition;
		}

		public void SetCondition(bool value)
		{
			Condition = value;
		}
	}
}