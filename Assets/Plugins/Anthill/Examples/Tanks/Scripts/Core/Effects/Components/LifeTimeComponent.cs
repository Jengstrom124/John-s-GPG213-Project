namespace Anthill.Effects
{
	using Anthill.Utils;
	
	/// <summary>
	/// Данный компонент определяет время жизни частиц.
	/// </summary>
	public class LifeTimeComponent : IComponent
	{
		#region Variables

		private float _duration;
		private IParticle _particle;

		#endregion
		#region IComponent Implementation

		public void Initialize(IParticle aParticle)
		{
			_particle = aParticle;
		}

		public void Reset(AntEmitter aEmitter)
		{
			_duration = Preset.lifeTime.duration;
			_duration += AntMath.RandomRangeFloat(Preset.lifeTime.rndToDuration);
			if (_duration <= 0.0f)
			{
				_duration = 0.1f;
			}

			_particle.LifeTime = _duration;
			IsActive = true;
		}

		public void Update(float aDeltaTime, float aTimeScale)
		{
			_duration -= aDeltaTime * aTimeScale;
			if (_duration <= 0.0f)
			{
				_duration = 0.0f;
				IsActive = false;
				_particle.Kill();
			}
		}

		public void Destroy()
		{
			_particle = null;
		}

		public AntEmitterPreset Preset { get; set; }
		public bool IsActive { get; private set; }
		public bool IsExists
		{
			get { return Preset.lifeTime.isExists; }
		}

		#endregion
	}
}