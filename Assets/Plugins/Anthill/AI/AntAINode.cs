namespace Anthill.AI
{
	/// <summary>
	/// Node of the world state.
	/// </summary>
	public class AntAINode
	{
		public AntAICondition parent; // Parent world state from which we came.
		public AntAICondition world;  // Current world state.
		public string action;         // Action that led to this condition.
		public int heuristic;
		public int cost;              // Cost of the node.
		public int sum;               // Summ heruistic and cost.
	}
}