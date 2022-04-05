namespace Anthill.Effects
{
	using UnityEngine;
	using Anthill.Animation;
	using Anthill.Utils;

	/// <summary>
	/// Данный компонент применяет настройки к AntActor.
	/// </summary>
	public class ColourComponent : IComponent
	{
		#region Variables

		private IParticle _particle;
		private SpriteRenderer _sprite;
		private SpriteRenderer _childSprite;

		private Color _curColor;
		private Color _endColor;
		private Gradient _gradient;
		private AnimationCurve _curve;
		private bool _useChild;
		private bool _gradientColor;
		private float _delta;

		#endregion
		#region IComponent Implementation

		public void Initialize(IParticle aParticle)
		{
			_particle = aParticle;
			_childSprite = _particle.GameObject.GetComponentInChildren<SpriteRenderer>();
			_sprite = _particle.GameObject.GetComponent<SpriteRenderer>();
		}

		public void Reset(AntEmitter aEmitter)
		{
			_curColor = Preset.colour.startColor;
			_endColor = Preset.colour.endColor;
			_gradient = Preset.colour.gradient;
			_curve = Preset.colour.curveColor;
			_useChild = Preset.colour.useChildSprite;
			_gradientColor = Preset.colour.gradientColor;
			IsActive = (Preset.colour.animateColor || Preset.colour.gradientColor);

			if (Preset.colour.effectLifeTime)
			{
				_delta = _particle.Effect.Duration / _particle.Effect.TotalDuration;
				_delta = AntMath.TrimToRange(_delta, 0.0f, 1.0f);
				_curColor.r = AntMath.Lerp(_curColor.r, _endColor.r, _curve.Evaluate(_delta));
				_curColor.g = AntMath.Lerp(_curColor.g, _endColor.g, _curve.Evaluate(_delta));
				_curColor.b = AntMath.Lerp(_curColor.b, _endColor.b, _curve.Evaluate(_delta));
				_curColor.a = AntMath.Lerp(_curColor.a, _endColor.a, _curve.Evaluate(_delta));
			}

			if (_useChild)
			{
				_childSprite.color = _curColor;
			}
			else
			{
				_sprite.color = _curColor;
			}
		}

		public void Update(float aDeltaTime, float aTimeScale)
		{
			_delta = _particle.CurrentLifeTime / _particle.LifeTime;
			if (Preset.colour.animateColor)
			{
				_curColor = (_useChild) ? _childSprite.color : _sprite.color;
				_curColor.r = AntMath.Lerp(_curColor.r, _endColor.r, _curve.Evaluate(_delta));
				_curColor.g = AntMath.Lerp(_curColor.g, _endColor.g, _curve.Evaluate(_delta));
				_curColor.b = AntMath.Lerp(_curColor.b, _endColor.b, _curve.Evaluate(_delta));
				_curColor.a = AntMath.Lerp(_curColor.a, _endColor.a, _curve.Evaluate(_delta));

				if (_useChild)
				{
					_childSprite.color = _curColor;
				}
				else
				{
					_sprite.color = _curColor;
				}
			}
			else if (_gradientColor)
			{
				if (_useChild)
				{
					_childSprite.color = _gradient.Evaluate(_delta);
				}
				else
				{
					_sprite.color = _gradient.Evaluate(_delta);
				}
			}
		}

		public void Destroy()
		{
			_sprite = null;
			_childSprite = null;
			_particle = null;
			_gradient = null;
			_curve = null;
		}

		public AntEmitterPreset Preset { get; set; }
		public bool IsActive { get; private set; }
		public bool IsExists
		{
			get { return Preset.colour.isExists; }
		}

		#endregion
	}
}