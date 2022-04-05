namespace Tanks
{
	using UnityEngine;
	using Anthill.AI;

	/// <summary>
	/// Implementation of the approach when tank unity has a bomb.
	/// </summary>
	public class ApproachState : AntAIState
	{
		#region Variables

		private TankControl _control;
		private TankBlackboard _blackboard;
		private float _reAimInterval;

		#endregion
		#region Public Methods

		public override void Create(GameObject aGameObject)
		{
			_control = aGameObject.GetComponent<TankControl>();
			_blackboard = aGameObject.GetComponent<TankBlackboard>();
		}

		public override void Enter()
		{
			_control.EventDestroyed += tankDestroyedHandler;
			// Move to the enemy tank.
			_control.MoveTo(_blackboard.EnemyTank.Position);
			_reAimInterval = 1.5f;
		}

		public override void Execute(float aDeltaTime, float aTimeScale)
		{
			// Update our target because target tank not waiting for our approuching ;)
			_reAimInterval -= aDeltaTime;
			if (_reAimInterval <= 0.0f)
			{
				_control.MoveTo(_blackboard.EnemyTank.Position);
				_reAimInterval = 1.5f;
			}
		}

		#endregion
		#region Event Handlers

		private void tankDestroyedHandler(TankControl aTank)
		{
			aTank.EventDestroyed -= tankDestroyedHandler;
			Finish();
		}

		#endregion
	}
}