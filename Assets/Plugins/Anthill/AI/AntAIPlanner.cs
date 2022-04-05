namespace Anthill.AI
{
	using System.Collections.Generic;

	/// <summary>
	/// The main class that builds plans.
	/// </summary>
	public class AntAIPlanner
	{
	#region Variables
		
		public const int MAX_ATOMS = 32;   // Max of the conditions.
		public const int MAX_ACTIONS = 32; // Max of the actions.

		public delegate void PlanUpdatedDelegate(AntAIPlan aNewPlan);
		public event PlanUpdatedDelegate EventPlanUpdated; // Called when plan is updated.

		public List<string> atoms = new List<string>();
		public List<AntAIAction> actions = new List<AntAIAction>();
		public List<AntAICondition> goals = new List<AntAICondition>();

#if UNITY_EDITOR
		public AntAICondition DebugConditions { get; private set; }
		// public bool DebugMode { get; set; }
#endif

		private string _scenarioName;
		private int _defActionIndex = -1;
		private int _defGoalIndex = -1;

	#endregion

	#region Public Methods

		public void LoadScenario(AntAIScenario aScenario)
		{
			int atomIndex;
			_scenarioName = aScenario.name;

			// 0. Register all conditions.
			// ---------------------------
			for (int i = 0, n = aScenario.conditions.list.Length; i < n; i++)
			{
				GetAtomIndex(aScenario.conditions.list[i].name);
			}

			// 1. Read Action List.
			// --------------------
			// string template = string.Empty;
			// if (DebugMode)
			// {
			// 	A.Log("<b>Action List</b>");
			// }

			AntAIAction action;
			AntAIScenarioAction scenarioAction;
			for (int i = 0, n = aScenario.actions.Length; i < n; i++)
			{
				scenarioAction = aScenario.actions[i];
				action = GetAction(scenarioAction.name);
				action.state = scenarioAction.state;
				action.cost = scenarioAction.cost;

				// if (DebugMode)
				// {
				// 	A.Log("<b>Action:</b> `{0}`", action.name);
				// 	A.Log("  Pre:");
				// }

				// Read Pre Conditions.
				for (int j = 0, nj = scenarioAction.pre.Length; j < nj; j++)
				{
					atomIndex = GetAtomIndex(aScenario.conditions.GetName(scenarioAction.pre[j].id));
					action.pre.Set(atomIndex, scenarioAction.pre[j].value);

					// if (DebugMode)
					// {
					// 	template = (scenarioAction.pre[j].value)
					// 		? "    `<color=green>{0} = {1}</color>`"
					// 		: "    `<color=red>{0} = {1}</color>`";

					// 	A.Log(template,
					// 		aScenario.conditions.GetName(scenarioAction.pre[j].id),
					// 		scenarioAction.pre[j].value);
					// }
				}

				// if (DebugMode)
				// {
				// 	A.Log("  Post:");
				// }

				// Read Post Conditions.
				for (int j = 0, nj = scenarioAction.post.Length; j < nj; j++)
				{
					atomIndex = GetAtomIndex(aScenario.conditions.GetName(scenarioAction.post[j].id));
					action.post.Set(atomIndex, scenarioAction.post[j].value);

					// if (DebugMode)
					// {
					// 	template = (scenarioAction.post[j].value)
					// 		? "    `<color=green>{0} = {1}</color>`"
					// 		: "    `<color=red>{0} = {1}</color>`";

					// 	A.Log(template,
					// 		aScenario.conditions.GetName(scenarioAction.post[j].id),
					// 		scenarioAction.post[j].value);
					// }
				}

				if (scenarioAction.isDefault)
				{
					_defActionIndex = actions.Count - 1;
				}
			}

			// 2. Read Goal List
			// -----------------

			// if (DebugMode)
			// {
			// 	A.Log("<b>Goal List</b>");
			// }

			AntAICondition goal;
			AntAIScenarioGoal scenarioGoal;
			for (int i = 0, n = aScenario.goals.Length; i < n; i++)
			{
				scenarioGoal = aScenario.goals[i];
				goal = GetGoal(scenarioGoal.name);

				// if (DebugMode)
				// {
				// 	A.Log("<b>Goal:</b> {0}", goal.name);
				// 	A.Log("  Cond:");
				// }

				// Read Conditions.
				for (int j = 0, nj = scenarioGoal.conditions.Length; j < nj; j++)
				{
					goal.Set(this, aScenario.conditions.GetName(scenarioGoal.conditions[j].id),
						scenarioGoal.conditions[j].value);

					// if (DebugMode)
					// {
					// 	template = (scenarioGoal.conditions[j].value)
					// 		? "    `<color=green>{0} = {1}</color>`"
					// 		: "    `<color=red>{0} = {1}</color>`";

					// 	A.Log(template,
					// 		aScenario.conditions.GetName(scenarioGoal.conditions[j].id),
					// 		scenarioGoal.conditions[j].value);
					// }
				}

				if (scenarioGoal.isDefault)
				{
					_defGoalIndex = goals.Count - 1;
				}
			}
		}

		public bool Pre(string aActionName, string aAtomName, bool aValue)
		{
			var action = GetAction(aActionName);
			int atomId = GetAtomIndex(aAtomName);
			return (action == null || atomId == -1)
				? false
				: action.pre.Set(atomId, aValue);
		}

		public bool Post(string aActionName, string aAtomName, bool aValue)
		{
			var action = GetAction(aActionName);
			int atomId = GetAtomIndex(aAtomName);
			return (action == null || atomId == -1)
				? false
				: action.post.Set(atomId, aValue);
		}

		/// <summary>
		/// Sets cost for the action.
		/// </summary>
		/// <param name="aActionName">Action name.</param>
		/// <param name="aCost">New cost value.</param>
		/// <returns>True if cost is setted.</returns>
		public bool SetCost(string aActionName, int aCost)
		{
			var action = GetAction(aActionName);
			if (action != null)
			{
				action.cost = aCost;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Clears all Conditions and Actions.
		/// </summary>
		public void Clear()
		{
			atoms.Clear();
			actions.Clear();
		}

		/// <summary>
		/// Extracts Condition index by Condition name.
		/// </summary>
		/// <param name="aAtomName">Name of the Condition.</param>
		/// <returns>Returns index of the Condition in the list. If Condition not exists, then it will be added to the Conditions list.</returns>
		public int GetAtomIndex(string aAtomName)
		{
			int index = atoms.IndexOf(aAtomName);
			if (index == -1 && atoms.Count < MAX_ATOMS)
			{
				atoms.Add(aAtomName);
				index = atoms.Count - 1;
			}
			return index;
		}

		/// <summary>
		/// Gets goal by goal name.
		/// </summary>
		/// <param name="aGoalName">Goal name.</param>
		/// <returns>Returns goal by name. If goal not found, will be created new one.</returns>
		public AntAICondition GetGoal(string aGoalName)
		{
			var goal = FindGoal(aGoalName);
			if (goal == null)
			{
				goal = new AntAICondition { name = aGoalName };
				goals.Add(goal);
			}
			return goal;
		}

		/// <summary>
		/// Finds the Goal by name.
		/// </summary>
		/// <param name="aGoalName">Name of the Goal for search.</param>
		/// <returns>Returns founded Goal or `null`.</returns>
		public AntAICondition FindGoal(string aGoalName)
		{
			return goals.Find(x => x.name.Equals(aGoalName));
		}

		/// <summary>
		/// Returns default goal.
		/// </summary>
		/// <returns>Default goal or null if default goal not found.</returns>
		public AntAICondition GetDefaultGoal()
		{
			return (_defGoalIndex >= 0 && _defGoalIndex < goals.Count)
				? goals[_defGoalIndex]
				: null;
		}

		/// <summary>
		/// Gets action by name.
		/// </summary>
		/// <param name="aActionName">Action name.</param>
		/// <returns>Returns action by name. If action not found, will be created new one.</returns>
		public AntAIAction GetAction(string aActionName)
		{
			var action = FindAction(aActionName);
			if (action == null && actions.Count < MAX_ACTIONS)
			{
				action = new AntAIAction(aActionName);
				actions.Add(action);
			}
			return action;
		}

		/// <summary>
		/// Find action by name.
		/// </summary>
		/// <param name="aActionName">Action name.</param>
		/// <returns>Returns action or null if action not found.</returns>
		public AntAIAction FindAction(string aActionName)
		{
			return actions.Find(x => (x.name != null && x.name.Equals(aActionName)));
		}

		/// <summary>
		/// Gets default action.
		/// </summary>
		/// <returns>Returns default action or null if default action not found.</returns>
		public AntAIAction GetDefaultAction()
		{
			return (_defActionIndex >= 0 && _defActionIndex < actions.Count)
				? actions[_defActionIndex]
				: null;
		}

		/// <summary>
		/// Builds the plan.
		/// </summary>
		/// <param name="aPlan">Reference to the result Plan.</param>
		/// <param name="aCurrent">Current World State.</param>
		/// <param name="aGoal">Goal World State that we want reach.</param>
		public void MakePlan(ref AntAIPlan aPlan, AntAICondition aCurrent, AntAICondition aGoal)
		{
#if UNITY_EDITOR
			DebugConditions = aCurrent.Clone();
#endif
			var opened = new List<AntAINode>();
			var closed = new List<AntAINode>();

			opened.Add(new AntAINode
			{
				world = aCurrent,
				parent = null,
				cost = 0,
				heuristic = aCurrent.Heuristic(aGoal),
				sum = aCurrent.Heuristic(aGoal),
				action = string.Empty
			});

			AntAINode current = opened[0];
			while (opened.Count > 0)
			{
				// Find lowest rank.
				current = opened[0];
				for (int i = 1, n = opened.Count; i < n; i++)
				{
					if (opened[i].sum < current.sum)
					{
						current = opened[i];
					}
				}

				opened.Remove(current);

				if (current.world.Match(aGoal))
				{
					// Plan is found!
					ReconstructPlan(ref aPlan, closed, current);
					aPlan.isSuccess = true;
					if (EventPlanUpdated != null)
					{
						EventPlanUpdated(aPlan);
					}

					return;
				}

				closed.Add(current);

				// Get neighbors.
				List<AntAIAction> neighbors = GetPossibleTransitions(current.world);
				AntAICondition neighbor;
				int openedIndex = -1;
				int closedIndex = -1;
				int cost = -1;
				for (int i = 0, n = neighbors.Count; i < n; i++)
				{
					cost = current.cost + neighbors[i].cost;
					
					neighbor = current.world.Clone();
					neighbor.Act(neighbors[i].post);

					openedIndex = FindEqual(opened, neighbor);
					closedIndex = FindEqual(closed, neighbor);

					if (openedIndex > -1 && cost < opened[openedIndex].cost)
					{
						opened.RemoveAt(openedIndex);
						openedIndex = -1;
					}

					if (closedIndex > -1 && cost < closed[closedIndex].cost)
					{
						closed.RemoveAt(closedIndex);
						closedIndex = -1;
					}

					if (openedIndex == -1 && closedIndex == -1)
					{
						opened.Add(new AntAINode
						{
							world = neighbor,
							cost = cost,
							heuristic = neighbor.Heuristic(aGoal),
							sum = cost + neighbor.Heuristic(aGoal),
							action = neighbors[i].name,
							parent = current.world
						});
					}
				}
			}

			// Failed plan.
			ReconstructPlan(ref aPlan, closed, current);
			aPlan.isSuccess = false;

			if (EventPlanUpdated != null)
			{
				EventPlanUpdated(aPlan);
			}
		}

	#endregion

	#region Private Methods

		private List<AntAIAction> GetPossibleTransitions(AntAICondition aCurrent)
		{
			var possible = new List<AntAIAction>();
			for (int i = 0, n = actions.Count; i < n; i++)
			{
				if (actions[i].pre.Match(aCurrent))
				{
					possible.Add(actions[i]);
				}
			}
			return possible;
		}

		private int FindEqual(List<AntAINode> aList, AntAICondition aCondition)
		{
			for (int i = 0, n = aList.Count; i < n; i++)
			{
				if (aList[i].world.Equals(aCondition))
				{
					return i;
				}
			}
			return -1;
		}

		private void ReconstructPlan(ref AntAIPlan aPlan, List<AntAINode> aClosed, AntAINode aGoal)
		{
			aPlan.Reset();
			int index;
			AntAINode current = aGoal;
			while (current != null && current.parent != null)
			{
				aPlan.Insert(current.action);
				index = FindEqual(aClosed, current.parent);
				current = (index == -1) ? aClosed[0] : aClosed[index];
			}
		}

	#endregion
	}
}