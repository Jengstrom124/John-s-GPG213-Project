namespace Anthill.Effects
{
	using UnityEngine;
	using Anthill.Utils;

	/// <summary>
	/// ...
	/// </summary>
	public class TrailComponent : IComponent
	{
		#region Variables

		private Trail _preset;
		private IParticle _particle;
		private TrailRenderer _trail;

		private float _startTime;
		private float _delta;
		private bool _needToInit;

		#endregion
		#region IComponent Implementation

		public void Initialize(IParticle aParticle)
		{
			_particle = aParticle;
			_trail = aParticle.GameObject.GetComponent<TrailRenderer>();
		}

		public void Reset(AntEmitter aEmitter)
		{
			IsActive = true;
			_preset = Preset.trail;
			_startTime = _preset.startTime + AntMath.RandomRangeFloat(_preset.rndToStartTime);
			_trail.widthMultiplier = _preset.startWidth + AntMath.RandomRangeFloat(_preset.rndToStartWidth);
			_trail.widthCurve = _preset.widthCurve;
			_trail.colorGradient = _preset.gradient;
			_trail.Clear();
			_needToInit = true;
		}

		public void Update(float aDeltaTime, float aTimeScale)
		{
			if (_needToInit)
			{
				_needToInit = false;
				_trail.sortingLayerName = _particle.SortingLayer;
    			_trail.sortingOrder = _particle.SortingOrder;
				IsActive = _preset.animateTime;
				_needToInit = false;
			}

			if (_preset.animateTime)
			{
				_delta = _particle.CurrentLifeTime / _particle.LifeTime;
				_trail.time = AntMath.Lerp(_startTime, _preset.endTime, _preset.timeCurve.Evaluate(_delta));
			}
		}

		public void Destroy()
		{
			_particle = null;
			_trail = null;
		}

		public AntEmitterPreset Preset { get; set; }
		public bool IsActive { get; private set; }
		public bool IsExists
		{
			get { return Preset.trail.isExists; }
		}

		#endregion
	}
}