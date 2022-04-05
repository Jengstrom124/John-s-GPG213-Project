namespace Anthill.Effects
{
	using UnityEngine;
	using Anthill.Utils;

	/// <summary>
	/// ..
	/// </summary>
	public class RotationComponent : IComponent
	{
		#region Variables

		private IParticle _particle;
		private Transform _transform;
		private float _startAngle;
		private float _endAngle;
		private float _angularSpeed;
		private bool _enableRotation;
		private bool _animateAngle;
		private float _accel;
		private float _drag;
		private AnimationCurve _curve;

		#endregion
		#region IComponent Implementation

		public void Initialize(IParticle aParticle)
		{
			_particle = aParticle;
			_transform = aParticle.GameObject.GetComponent<Transform>();
		}

		public void Reset(AntEmitter aEmitter)
		{
			IsActive = (Preset.rotation.enableRotation || Preset.rotation.animateAngle);
			_startAngle = Preset.rotation.startAngle + AntMath.RandomRangeFloat(Preset.rotation.rndToAngle);
			_endAngle = Preset.rotation.endAngle;
			_angularSpeed = Preset.rotation.angularSpeed + AntMath.RandomRangeFloat(Preset.rotation.rndToAngularSpeed);
			_enableRotation = Preset.rotation.enableRotation;
			_animateAngle = Preset.rotation.animateAngle;
			_accel = Preset.rotation.accel;
			_drag = Preset.rotation.drag;
			_curve = Preset.rotation.curveAngle;

			if (Preset.rotation.inheritRotation)
			{
				_startAngle += _particle.Effect.Angle;
				_endAngle += _particle.Effect.Angle;
			}

			Angle = _startAngle;
		}

		public void Update(float aDeltaTime, float aTimeScale)
		{
			if (_enableRotation)
			{
				Angle += _angularSpeed * aDeltaTime * aTimeScale;
				_angularSpeed += _accel * aDeltaTime * aTimeScale;
				_angularSpeed *= _drag;
			}
			else if (_animateAngle)
			{
				float delta = _particle.CurrentLifeTime / _particle.LifeTime;
				Angle = AntMath.LerpAngleDeg(_startAngle, _endAngle, _curve.Evaluate(delta));
			}
		}

		public void Destroy()
		{
			_particle = null;
			_transform = null;
		}

		public AntEmitterPreset Preset { get; set; }
		public bool IsActive { get; private set; }
		public bool IsExists
		{
			get { return Preset.rotation.isExists; }
		}

		#endregion
		#region Private Methods

		private float Angle
		{
			get { return _transform.rotation.eulerAngles.z; }
			set { _transform.rotation = Quaternion.AngleAxis(value, Vector3.forward); }
		}

		#endregion
	}
}