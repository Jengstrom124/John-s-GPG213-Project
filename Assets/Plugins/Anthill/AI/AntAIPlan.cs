namespace Anthill.AI
{
	using System.Collections.Generic;

	/// <summary>
	/// This class implements list of the actions as plan.
	/// </summary>
	public class AntAIPlan
	{
	#region Variables

		public bool isSuccess; // True if plan is successed.

		private readonly List<string> _actions;

	#endregion

	#region Getters / Setters

		/// <summary>
		/// Returns name of the action by index.
		/// </summary>
		public string this[int aIndex]
		{
			get => (aIndex >= 0 && aIndex < _actions.Count)
				? _actions[aIndex]
				: string.Empty;
		}

		/// <summary>
		/// Returns count of actions in the plan.
		/// </summary>
		public int Count { get => _actions.Count; }

	#endregion

	#region Public Methods

		public AntAIPlan()
		{
			_actions = new List<string>();
		}

		/// <summary>
		/// Removes all actions from the plan.
		/// </summary>
		public void Reset()
		{
			_actions.Clear();
		}

		/// <summary>
		/// Adds new action to the plan.
		/// </summary>
		/// <param name="aValue">Action name.</param>
		public void Insert(string aValue)
		{
			_actions.Insert(0, aValue);
		}

	#endregion
	}
}