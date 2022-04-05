namespace Anthill.AI
{
	using System.Collections.Generic;
	using UnityEngine;
	using Anthill.Utils;

	/// <summary>
	/// Default implementation of the AntAI.
	/// </summary>
	[AddComponentMenu("Anthill/AntAIAgent")]
	public class AntAIAgent : MonoBehaviour
	{
	#region Public Variables

		[Tooltip("Enable this if you don't want use the standart Update() method.")]
		public bool manualUpdate;

		[Tooltip("Delay between updating the world state and plan builds.")]
		public float thinkInterval = 0.1f;

		[Tooltip("Reference to the AI scenario.")]
		public AntAIScenario scenario;

		public AntAIPlanner planner = new AntAIPlanner();
		public AntAICondition worldState = new AntAICondition();
		public AntAIPlan currentPlan = new AntAIPlan();
	
	#endregion

	#region Private Variables

		private List<AntAIState> _states;
		private AntAIState _currentState;
		private AntAICondition _currentGoal;
		private ISense[] _sensors;

		private float _thinkInterval;

	#endregion

	#region Getters / Setters
		
		/// <summary>
		/// Returns active goal.
		/// </summary>
		public AntAICondition Goal { get => _currentGoal; }

		/// <summary>
		/// Returns current state.
		/// </summary>
		public AntAIState State { get => _currentState; }
		
	#endregion

	#region Unity Calls

		private void Awake()
		{
			// We set a random interval so that when spawning many agents, they think at different times.
			// This is trick should improve the perfomace.
			_thinkInterval = AntMath.RandomRangeFloat(0.0f, thinkInterval);

			// Get list of sensors.
			_sensors = GetComponents<ISense>();
			if (_sensors.Length == 0)
			{
				A.Warning($"AIAgent `{gameObject.name}` have no sensors for collection conditions of the world state.");
			}
			
			// Loading the scenario.
			A.Assert(scenario == null, $"Have no specified AI Scenario for `{name}` object.");
			planner.LoadScenario(scenario);

			// Initialize States.
			_states = new List<AntAIState>();
			for (int i = 0, n = scenario.actions.Length; i < n; i++)
			{
				A.Assert(
					scenario.actions[i].state == null, 
					$"AI Scenario of `{name}` object have no state prefab for `{scenario.actions[i].name}` action."
				);

				// Skipping states with the same name. Do not create duplicates.
				if (ContainsState(scenario.actions[i].state.name))
				{
					continue;
				}

				// Creating the state prefab as child of the AIAgent.
				var go = GameObject.Instantiate(scenario.actions[i].state);
				go.transform.SetParent(gameObject.transform, false);
				go.name = scenario.actions[i].state.name;
				
				var state = go.GetComponent<AntAIState>();
				
				A.Assert(
					state == null, 
					$"GameObject `{go.name}` have no `AntAIState` script`!"
				);

				state.Create(gameObject);
				state.gameObject.SetActive(false);
				_states.Add(state);
			}
		}

		private void Start()
		{
			SetDefaultState();
			SetDefaultGoal();
			Think();
		}

		private void Update()
		{
			if (!manualUpdate)
			{
				Execute(Time.deltaTime, Time.timeScale);
			}
		}

	#endregion

	#region Public Methods

		/// <summary>
		/// Calling this function to update the AI decisions.
		/// </summary>
		/// <param name="aDeltaTime">Delta Time.</param>
		/// <param name="aTimeScale">Time Scale.</param>
		public void Execute(float aDeltaTime, float aTimeScale)
		{
			// Update current action.
			_currentState.Execute(aDeltaTime, aTimeScale);

			// Delay before next think.
			_thinkInterval -= aDeltaTime;
			if (_thinkInterval < 0.0f)
			{
				_thinkInterval = thinkInterval;
				Think();
			}
		}

		/// <summary>
		/// Calling this function when need to update our world state and 
		/// select new actions, if needed.
		/// </summary>
		public void Think()
		{
			// Update world state.
			for (int i = 0, n = _sensors.Length; i < n; i++)
			{
				_sensors[i].CollectConditions(this, worldState);
			}

			// Check the current action.
			if (_currentState != null)
			{
				if (_currentState.IsFinished(this, worldState))
				{
					// If current action is finished or interrupted, then select new one
					// and start it with the `Force` flag.
					SetState(SelectNewState(worldState), true);
				}
				else if (_currentState.AllowInterrupting)
				{
					// If the current action is still active (it was not interrupted or completed), 
					// then we update the plan based on the current world situation and change 
					// the action only if the action from the updated plan differs from the current one.
					SetState(SelectNewState(worldState));
				}
			}
			else
			{
				// Set default action.
				SetDefaultState();
			}
		}

		/// <summary>
		/// Sets new goal for AI.
		/// </summary>
		/// <param name="aGoalName">Name of the goal that exists in the AI Scenario.</param>
		public void SetGoal(string aGoalName)
		{
			_currentGoal = planner.FindGoal(aGoalName);
			A.Assert(_currentGoal == null, $"Can't find goal `{aGoalName}` for `{gameObject.name}` AI Agent.");
		}

		/// <summary>
		/// Sets goal by default (default goal from the AI Scenario).
		/// </summary>
		public void SetDefaultGoal()
		{
			_currentGoal = planner.GetDefaultGoal();
			A.Assert(_currentGoal == null, $"Can't find default goal for `{gameObject.name}` AI Agent.");
		}

		/// <summary>
		/// Sets action by default (default action from the AI Scenario).
		/// </summary>
		public void SetDefaultState()
		{
			var defAction = planner.GetDefaultAction();
			A.Assert(defAction == null, $"Can't find default action for `{gameObject.name}` AI Agent.");
			SetState(defAction.state.name, true);
		}

	#endregion

	#region Private Methods

		private bool ContainsState(string aName)
		{
			var index = _states.FindIndex(x => x.name.Equals(aName));
			return (index >= 0 && index < _states.Count);
		}
		
		private string SelectNewState(AntAICondition aWorldState)
		{
			var defAction = planner.GetDefaultAction();
			var stateName = (defAction != null) ? defAction.state.name : string.Empty;
			planner.MakePlan(ref currentPlan, aWorldState, _currentGoal);
			if (currentPlan.isSuccess || currentPlan.Count > 0)
			{
				var action = planner.FindAction(currentPlan[0]);
				if (action != null && action.state != null)
				{
					stateName = action.state.name;
				}
			}

			return stateName;
		}

		private void SetState(string aStateName, bool aForce = false)
		{
			// Leave from current state.
			if (_currentState != null)
			{
				if (!aForce && string.Equals(_currentState.name, aStateName))
				{
					// We are in this state and we have no reason to enter it again.
					// Just skip...
					return;
				}

				_currentState.Exit();
				_currentState.gameObject.SetActive(false);
				_currentState = null;
			}

			// Set new state.
			int index = _states.FindIndex(x => x.name.Equals(aStateName));
			if (index >= 0 && index < _states.Count)
			{
				_currentState = _states[index];
				_currentState.gameObject.SetActive(true);
				_currentState.Reset();
				_currentState.Enter();
			}
			else
			{
				A.Warning($"Can't find state `{aStateName}` for `{gameObject.name}` AI Agent.");
			}
		}
		
	#endregion
	}
}