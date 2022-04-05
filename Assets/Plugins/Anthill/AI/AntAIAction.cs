namespace Anthill.AI
{
	using UnityEngine;

	public class AntAIAction
	{
	#region Public Variables

		public int cost;            // Cost of the action.
		public string name;         // Name of the action.
		public GameObject state;    // Reference to the AntAIState.
		public AntAICondition pre;  // Previous world state.
		public AntAICondition post; // Current world state.

	#endregion
	
	#region Public Methods

		public AntAIAction(string aName, int aCost = 1)
		{
			cost = aCost;
			name = aName;
			state = null;
			pre = new AntAICondition();
			post = new AntAICondition();
		}

	#endregion
	}
}