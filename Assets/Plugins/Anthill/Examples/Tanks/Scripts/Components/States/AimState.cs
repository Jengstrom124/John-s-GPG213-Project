namespace Tanks
{
	using UnityEngine;
	using Anthill.AI;
	using Anthill.Utils;

	/// <summary>
	/// Implementation of the aiming.
	/// </summary>
	public class AimState : AntAIState
	{
		private TankControl _control;
		private TankBlackboard _blackboard;
		private float _targetAngle;

		public override void Create(GameObject aGameObject)
		{
			_control = aGameObject.GetComponent<TankControl>();
			_blackboard = aGameObject.GetComponent<TankBlackboard>();
		}

		public override void Enter()
		{
			// Setup target angle.
			_targetAngle = AntMath.AngleDeg(_control.Position, _blackboard.EnemyTank.Position);
			_control.StopMove();
		}

		public override void Execute(float aDeltaTime, float aTimeScale)
		{
			// Aim on the target.
			if (!AntMath.Equal(AntMath.Angle(_control.towerRef.Angle), AntMath.Angle(_targetAngle), 1.0f))
			{
				float curAng = AntMath.Angle(_control.towerRef.Angle);
				float tarAng = AntMath.Angle(_targetAngle);
				if (Mathf.Abs(curAng - tarAng) > 180.0f)
				{
					if (curAng > tarAng)
					{
						tarAng += 360.0f;
					}
					else
					{
						tarAng -= 360.0f;
					}
				}

				if (curAng < tarAng)
				{
					_control.RotateTower(1.0f, aDeltaTime);
				}
				else if (curAng > tarAng)
				{
					_control.RotateTower(-1.0f, aDeltaTime);
				}
			}
			else
			{
				Finish();
			}
		}
	}
}