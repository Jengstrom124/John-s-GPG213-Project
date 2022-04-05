namespace SimpleExample
{
	using UnityEngine;
	using Anthill.AI;

	/// <summary>
	/// This is example of the custom implementation of the AI.
	/// </summary>
	public class SimpleExample : MonoBehaviour
	{
		public AntAIScenario scenario;

		private void Start()
		{
			// 1. Create the AI planner.
			// -------------------------
			var planner = new AntAIPlanner();

			// Load scenario for planner.
			A.Assert(scenario == null, "Scenario not selected!");
			planner.LoadScenario(scenario);

			// 2. Create world state.
			// ----------------------
			var worldState = new AntAICondition();
			worldState.BeginUpdate(planner);
			worldState.Set("Is Cargo Delivered", false);
			worldState.Set("See Cargo", false);
			worldState.Set("Has Cargo", false);
			worldState.Set("See Base", false);
			worldState.Set("Near Base", false);
			worldState.EndUpdate();

			// 3. Build plan.
			// --------------
			var plan = new AntAIPlan();
			planner.MakePlan(ref plan, worldState, planner.GetDefaultGoal());

			// Output plan.
			Debug.Log("<b>Plan:</b>");
			for (int i = 0; i < plan.Count; i++)
			{
				Debug.Log((i + 1) + ". " + plan[i]);
			}

			// 4. Change world state and rebuild plan.
			// ---------------------------------------
			worldState.BeginUpdate(planner);
			worldState.Set("Has Cargo", true);
			worldState.EndUpdate();

			planner.MakePlan(ref plan, worldState, planner.FindGoal("Delivery"));

			// Output plan.
			Debug.Log("<b>Plan:</b>");
			for (int i = 0; i < plan.Count; i++)
			{
				Debug.Log((i + 1) + ". " + plan[i]);
			}
		}
	}
}