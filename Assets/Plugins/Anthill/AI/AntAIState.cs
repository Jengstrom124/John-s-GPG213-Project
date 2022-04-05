namespace Anthill.AI
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Default implementation of the state for AIAgent.
	/// </summary>
	public abstract class AntAIState : MonoBehaviour
	{
	#region Private Variables
		
		private bool _isFinished = false;
		private bool _allowInterrupting = true;
		private List<string> _interruptions = new List<string>();
		
	#endregion

	#region Getters / Setters
		
		/// <summary>
		/// List of interruption conditions.
		/// </summary>
		public List<string> Interruptions { get => _interruptions; }

		/// <summary>
		/// Determines possobility to force interruption of the current state.
		/// </summary>
		public virtual bool AllowInterrupting
		{
			get => _allowInterrupting;
			set => _allowInterrupting = value;
		}
		
	#endregion

	#region Public Methods
		
		/// <summary>
		/// Calling when game object and state is created.
		/// </summary>
		/// <param name="aGameObject">Reference to the owner game object.</param>
		public virtual void Create(GameObject aGameObject)
		{
			// ...
		}

		/// <summary>
		/// Calling before destroying the game object.
		/// </summary>
		/// <param name="aGameObject">Reference to the owner game object.</param>
		public virtual void Destroy(GameObject aGameObject)
		{
			// ...
		}

		/// <summary>
		/// Calling before entering to state.
		/// </summary>
		public virtual void Enter()
		{
			// ...
		}

		/// <summary>
		/// Calling every frame when state is active.
		/// </summary>
		/// <param name="aDeltaTime">Delta time.</param>
		/// <param name="aTimeScale">Time scale.</param>
		public virtual void Execute(float aDeltaTime, float aTimeScale)
		{
			// ...
		}

		/// <summary>
		/// Calling before leaving from the state.
		/// </summary>
		public virtual void Exit()
		{
			// ...
		}

		/// <summary>
		/// Call this function when state is finished.
		/// </summary>
		public void Finish()
		{
			_isFinished = true;
		}

		/// <summary>
		/// This function calling automatic to reset finished status.
		/// </summary>
		public void Reset()
		{
			_isFinished = false;
		}

		/// <summary>
		/// Calling from the AntAIAgent for checking when state is finished.
		/// </summary>
		/// <param name="aAgent">Ref to the AntAIAgent.</param>
		/// <param name="aWorldState">Current world state.</param>
		/// <returns>True if state is finished.</returns>
		public virtual bool IsFinished(AntAIAgent aAgent, AntAICondition aWorldState)
		{
			return (_isFinished || OverlapInterrupts(aAgent, aWorldState));
		}

		/// <summary>
		/// Checking if current world state is equal interruption settings.
		/// </summary>
		/// <param name="aAgent">Ref to the AntAIAgent.</param>
		/// <param name="aWorldState">Current world state.</param>
		/// <returns></returns>
		public bool OverlapInterrupts(AntAIAgent aAgent, AntAICondition aWorldState)
		{
			int index = -1;
			for (int i = 0, n = _interruptions.Count; i < n; i++)
			{
				index = aAgent.planner.GetAtomIndex(_interruptions[i]);
				if (aWorldState.GetValue(index))
				{
					return true;
				}
			}
			return false;
		}

	#endregion
	}
}