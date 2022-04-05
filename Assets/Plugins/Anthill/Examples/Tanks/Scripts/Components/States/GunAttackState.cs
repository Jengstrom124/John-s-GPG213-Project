namespace Tanks
{
	using UnityEngine;
	using Anthill.AI;
	using Anthill.Utils;

	/// <summary>
	/// State of the tank where is we attacking with gun.
	/// </summary>
	public class GunAttackState : AntAIState
	{
		private TankControl _control;
		private TankBlackboard _blackboard;
		private float _delay;
		private float _targetAngle;

		public override void Create(GameObject aGameObject)
		{
			_control = aGameObject.GetComponent<TankControl>();
			_blackboard = aGameObject.GetComponent<TankBlackboard>();

			// This state can't be interrupted.
			AllowInterrupting = false;
		}

		public override void Enter()
		{
			_targetAngle = AntMath.AngleDeg(_control.Position, _blackboard.EnemyTank.Position);

			// Calc spawn point for the bullet.
			// This point should be outside the tank body, 
			// otherwise this bullet will damage our self.
			var ang = _control.Tower.Angle * AntMath.RADIANS;
			var p = _control.Position;
			p.x += 0.45f * Mathf.Cos(ang); 
			p.y += 0.45f * Mathf.Sin(ang);

			// Spawn and execute the bullet.
			var bullet = Game.Instance.SpawnBullet(p, _control.transform.parent);
			Quaternion q = Quaternion.AngleAxis(_control.Tower.Angle, Vector3.forward);
			bullet.transform.rotation = q;
			bullet.Execute(_control.Tower.Angle);

			_control.Tower.TakeAmmo(1);
			_delay = 0.75f;

			_control.Tower.ShotEffect();
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

			_delay -= aDeltaTime;
			if (_delay <= 0.0f)
			{
				Finish();
			}
		}
	}
}