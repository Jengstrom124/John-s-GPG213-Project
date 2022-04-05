namespace Anthill.Effects
{
	using UnityEngine;
	using Anthill.Utils;

	/// <summary>
	/// Данный компонент реализует движение частиц.
	/// </summary>
	public class MovementComponent : IComponent
	{
		#region Variables

		// :: Private Variables ::

		private Transform _transform;
		private IParticle _particle;
		private Vector3 _position;
		private float _speed;
		private float _startSpeed;
		private float _endSpeed;
		private Movement _movement;
		private Vector2 _velocity;
		private float _angle;
		private float _delta;

		#endregion
		#region IComponent Implementation

		public void Initialize(IParticle aParticle)
		{
			_transform = aParticle.GameObject.GetComponent<Transform>();
			_particle = aParticle;
		}

		public void Reset(AntEmitter aEmitter)
		{
			IsActive = true;

			_movement = Preset.movement;
			_speed = _movement.speed + AntMath.RandomRangeFloat(_movement.rndToSpeed);
			_startSpeed = _speed;
			_endSpeed = _movement.endSpeed;

			_angle = Angle * AntMath.RADIANS;
			_velocity.x = _speed * Mathf.Cos(_angle);
			_velocity.y = _speed * Mathf.Sin(_angle);
		}

		public void Update(float aDeltaTime, float aTimeScale)
		{
			if (_movement.gravity)
			{
				_velocity.x += _movement.gravityFactor.x * aDeltaTime * aTimeScale;
				_velocity.y += _movement.gravityFactor.y * aDeltaTime * aTimeScale;

				if (_movement.rotate)
				{
					Angle = AntMath.AngleDeg(_velocity);
				}
			}
			else
			{
				_delta = _particle.CurrentLifeTime / _particle.LifeTime;
				_angle = Angle * AntMath.RADIANS;
				_velocity.x = _speed * Mathf.Cos(_angle);
				_velocity.y = _speed * Mathf.Sin(_angle);

				if (_movement.animateSpeed)
				{
					_speed = AntMath.Lerp(_startSpeed, _endSpeed, _movement.speedCurve.Evaluate(_delta));
				}
				else
				{
					_speed += _movement.accel * aDeltaTime * aTimeScale;
					_speed *= _movement.drag;
				}
			}

			_position = _transform.position;
			_position.x += _velocity.x * aDeltaTime * aTimeScale;
			_position.y += _velocity.y * aDeltaTime * aTimeScale;
			_transform.position = _position;
		}

		public void Destroy()
		{
			_transform = null;
			_particle = null;
		}

		public AntEmitterPreset Preset { get; set; }
		public bool IsActive { get; private set; }
		public bool IsExists
		{
			get { return Preset.movement.isExists; }
		}

		#endregion
		#region Getters / Setters

		private float Angle
		{
			get { return _transform.rotation.eulerAngles.z; }
			set { _transform.rotation = Quaternion.AngleAxis(value, Vector3.forward); }
		}

		#endregion
	}
}