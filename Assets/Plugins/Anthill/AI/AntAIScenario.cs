namespace Anthill.AI
{
	using System;
	using UnityEngine;
	using Anthill.Utils;

	/// <summary>
	/// Scenario it's a collection of the conditions, actions and goals that need to know for
	/// building the plan and complete goals.
	/// </summary>
	[CreateAssetMenuAttribute(fileName = "Scenario", menuName = "Anthill/AI Scenario", order = 1)]
	public class AntAIScenario : ScriptableObject
	{
		/// <summary>
		/// List of conditions for description of the World State.
		/// </summary>
		[HideInInspector]
		public AntAIScenarioCondition conditions = new AntAIScenarioCondition();

		/// <summary>
		/// List of conditions for building action plan.
		/// </summary>
		[HideInInspector]
		public AntAIScenarioAction[] actions = new AntAIScenarioAction[0];

		/// <summary>
		/// List of available goals.
		/// </summary>
		[HideInInspector]
		public AntAIScenarioGoal[] goals = new AntAIScenarioGoal[0];

		/// <summary>
		/// List of world states, uses only for the testing of the scenarios.
		/// </summary>
		[HideInInspector]
		public AntAIWorldState[] worldStates = new AntAIWorldState[0];
	
	#region Public Methods

		public void AddCondition(string aName)
		{
			AntArray.Add(ref conditions.list, new AntAIScenarioConditionItem
			{
				id = conditions.list.Length,
				name = aName
			});
		}

		public void RemoveConditionAt(int aIndex)
		{
			if (aIndex >= 0 && aIndex < conditions.list.Length)
			{
				UpdateConditionIndexes(aIndex);
				UpdateActionIndexes(aIndex);
				UpdateGoalIndexes(aIndex);
				UpdateWorldStates(aIndex);

				AntArray.RemoveAt(ref conditions.list, aIndex);
			}
		}

	#endregion

	#region Private Methods

		private void UpdateConditionIndexes(int aDelIndex)
		{
			for (int i = conditions.list.Length - 1; i >= 0; i--)
			{
				if (conditions.list[i].id > aDelIndex)
				{
					conditions.list[i].id -= 1;
				}
			}
		}

		private void UpdateActionIndexes(int aDelIndex)
		{
			for (int i = 0, n = actions.Length; i < n; i++)
			{
				var action = actions[i];

				// Update pre conditions for actions.
				for (int j = action.pre.Length - 1; j >= 0; j--)
				{
					if (action.pre[j].id == aDelIndex)
					{
						AntArray.RemoveAt(ref action.pre, j);
					}
					else
					{
						if (action.pre[j].id > aDelIndex)
						{
							action.pre[j].id -= 1;
						}
					}
				}

				// Update post conditions for actions.
				for (int j = action.post.Length - 1; j >= 0; j--)
				{
					if (action.post[j].id == aDelIndex)
					{
						AntArray.RemoveAt(ref action.post, j);
					}
					else
					{
						if (action.post[j].id > aDelIndex)
						{
							action.post[j].id -= 1;
						}
					}
				}
			}
		}

		private void UpdateGoalIndexes(int aDelIndex)
		{
			for (int i = 0, n = goals.Length; i < n; i++)
			{
				var goal = goals[i];

				// Update goal conditions.
				for (int j = goal.conditions.Length - 1; j >= 0; j--)
				{
					if (goal.conditions[j].id == aDelIndex)
					{
						AntArray.RemoveAt(ref goal.conditions, j);
					}
					else
					{
						if (goal.conditions[j].id > aDelIndex)
						{
							goal.conditions[j].id -= 1;
						}
					}
				}
			}
		}

		private void UpdateWorldStates(int aDelIndex)
		{
			for (int i = 0, n = worldStates.Length; i < n; i++)
			{
				var state = worldStates[i];

				// Update world state conditions.
				for (int j = state.list.Length - 1; j >= 0; j--)
				{
					if (state.list[j].id == aDelIndex)
					{
						AntArray.RemoveAt(ref state.list, j);
					}
					else
					{
						if (state.list[j].id > aDelIndex)
						{
							state.list[j].id -= 1;
						}
					}
				}
			}
		}

	#endregion
	}

	/// <summary>
	/// A class describing the World State that used for the
	/// debugging of the scenario in the editor mode.
	/// </summary>
	[Serializable]
	public class AntAIWorldState
	{
		public Vector2 position;
		public bool isAutoUpdate;
		public AntAIScenarioItem[] list;
		
		public AntAIWorldState()
		{
			list = new AntAIScenarioItem[0];
		}
	}

	/// <summary>
	/// A class describing the Goal.
	/// </summary>
	[Serializable]
	public class AntAIScenarioGoal
	{
		public string name;                        // Name of the goal.
		public bool isDefault;                     // This goal will be active by default.
		public Vector2 position;                   // Position of the node in the editor workplace.
		public AntAIScenarioItem[] conditions;     // World state what we want to reach.

		public AntAIScenarioGoal()
		{
			name = "<Unnamed>";
			position = Vector2.zero;
			conditions = new AntAIScenarioItem[0];
		}
	}

	/// <summary>
	/// A class describing a specific action.
	/// </summary>
	[Serializable]
	public class AntAIScenarioAction
	{
		public string name;                        // Name of the action.
		public bool isDefault;                     // This action will be as default state.
		public GameObject state;                   // Reference to the prefab with custom AntAIState script.
		public int cost;                           // Cost of the action (чем выше, тем действие дороже).
		public Vector2 position;                   // Position of the node in the editor workplace.
		public AntAIScenarioItem[] pre;            // Conditions before this action will be called.
		public AntAIScenarioItem[] post;           // Conditions after this action will be called.

		public AntAIScenarioAction()
		{
			name = "<Unnamed>";
			isDefault = false;
			state = null;
			cost = 0;
			position = Vector2.zero;
			pre = new AntAIScenarioItem[0];
			post = new AntAIScenarioItem[0];
		}
	}

	/// <summary>
	/// A structure describing the value of a condition.
	/// </summary>
	[Serializable]
	public struct AntAIScenarioItem
	{
		public int id;
		public bool value;
	}

	/// <summary>
	/// A class that implements a list of conditions used in the script to describe the world.
	/// </summary>
	[Serializable]
	public class AntAIScenarioCondition
	{
		// public Vector2 position;
		public AntAIScenarioConditionItem[] list = new AntAIScenarioConditionItem[0];
		
		public AntAIScenarioCondition Clone()
		{
			var clone = new AntAIScenarioCondition();
			clone.list = new AntAIScenarioConditionItem[list.Length];
			for (int i = 0, n = list.Length; i < n; i++)
			{
				clone.list[i] = list[i];
			}
			return clone;
		}

		public string GetName(int aIndex)
		{
			int index = Array.FindIndex(list, x => x.id == aIndex);
			return (index >= 0 && index < list.Length)
				? list[index].name
				: null;
		}

		public int GetIndex(string aConditionName)
		{
			int index = Array.FindIndex(list, x => x.name.Equals(aConditionName));
			return (index >= 0 && index < list.Length)
				? list[index].id
				: -1;
		}

		public string this[int aIndex]
		{
			get => (aIndex >= 0 && aIndex < list.Length)
					? list[aIndex].name
					: null; 
		}

		public int Count
		{
			get => list.Length;
		}
	}

	/// <summary>
	/// The structure describing the condition.
	/// </summary>
	[Serializable]
	public struct AntAIScenarioConditionItem
	{
		public int id;
		public string name;
	}
}