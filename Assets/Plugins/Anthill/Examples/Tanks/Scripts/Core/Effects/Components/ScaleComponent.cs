namespace Anthill.Effects
{
	using UnityEngine;
	using Anthill.Utils;

	/// <summary>
	/// ..
	/// </summary>
	public class ScaleComponent : IComponent
	{
		#region Variables

		private IParticle _particle;
		private Transform _transform;
		private Transform _childTransform;
		private Vector2 _startScale;
		private Vector2 _endScale;
		private AnimationCurve _curveX;
		private AnimationCurve _curveY;
		private float _delta;
		private float _dx;
		private float _dy;
		private bool _useChild;
		private bool _proportional;

		#endregion
		#region IComponent Implementation

		public void Initialize(IParticle aParticle)
		{
			_particle = aParticle;
			_transform = aParticle.GameObject.GetComponent<Transform>();
			_childTransform = aParticle.GameObject.GetComponentInChildren<Transform>();
		}

		public void Reset(AntEmitter aEmitter)
		{
			IsActive = Preset.scale.animateScale;
			_endScale = Preset.scale.endScale;
			_startScale = Vector2.zero;
			_startScale.x = Preset.scale.startScale.x + AntMath.RandomRangeFloat(Preset.scale.rndToScaleX);
			_startScale.y = Preset.scale.startScale.y + AntMath.RandomRangeFloat(Preset.scale.rndToScaleY);
			_curveX = Preset.scale.scaleCurveX;
			_curveY = Preset.scale.scaleCurveY;
			_useChild = Preset.scale.useChildSprite;
			_proportional = Preset.scale.proportional;

			if (Preset.scale.effectLifeTime && !AntMath.Equal(_particle.Effect.lifeTime, 0.0f))
			{
				_delta = _particle.Effect.Duration / _particle.Effect.TotalDuration;
				_delta = AntMath.TrimToRange(_delta, 0.0f, 1.0f);
				_startScale.x = AntMath.Lerp(_startScale.x, Preset.scale.endScale.x, _curveX.Evaluate(_delta));
				_startScale.y = AntMath.Lerp(_startScale.y, Preset.scale.endScale.y, _curveY.Evaluate(_delta));	
			}

			if (Preset.scale.useChildSprite)
			{
				_childTransform.localScale = new Vector3(
					_startScale.x, 
					(Preset.scale.proportional) 
						? _startScale.x 
						: _startScale.y, 
					1.0f
				);
			}
			else
			{
				_transform.localScale = new Vector3(
					_startScale.x, 
					(Preset.scale.proportional) 
						? _startScale.x 
						: _startScale.y, 
					1.0f
				);
			}
		}

		public void Update(float aDeltaTime, float aTimeScale)
		{
			_delta = _particle.CurrentLifeTime / _particle.LifeTime;
			_dx = AntMath.Lerp(_startScale.x, _endScale.x, _curveX.Evaluate(_delta));
			_dy = AntMath.Lerp(_startScale.y, _endScale.y, _curveY.Evaluate(_delta));

			if (_useChild)
			{
				_childTransform.localScale = new Vector3(
					_dx, 
					(_proportional)
						? _dx 
						: _dy, 
					1.0f
				);
			}
			else
			{
				_transform.localScale = new Vector3(
					_dx, 
					(_proportional) 
						? _dx 
						: _dy, 
					1.0f
				);
			}
		}

		public void Destroy()
		{
			_particle = null;
			_transform = null;
			_childTransform = null;
			_curveX = null;
			_curveY = null;
		}

		public AntEmitterPreset Preset { get; set; }
		public bool IsActive { get; private set; }
		public bool IsExists
		{
			get { return Preset.scale.isExists; }
		}

		#endregion
	}
}