namespace Anthill.Effects
{
	using UnityEngine;
	using Anthill.Utils;

	/// <summary>
	/// Данный компонент спавнит заданное количество частиц с заданным 
	/// временным интервалом.
	/// </summary>
	public class EmissionComponent : IComponent, IEmission
	{
		#region Variables

		private float _spawnInterval;
		private Emission _preset;

		#endregion
		#region IComponent Implementation

		public void Initialize(IParticle aParticle)
		{
			// ..
		}

		public void Reset(AntEmitter aEmitter)
		{
			IsActive = true;
			_preset = Preset.emission;
			_spawnInterval = _preset.spawnInterval + AntMath.RandomRangeFloat(_preset.rndToSpawnInterval);
		}

		public void Update(float aDeltaTime, float aTimeScale)
		{
			_spawnInterval -= aDeltaTime * aTimeScale;
		}

		public void Destroy()
		{
			// ..
		}

		public AntEmitterPreset Preset { get; set; }
		public bool IsActive { get; private set; }
		public bool IsExists
		{
			get { return Preset.emission.isExists; }
		}

		#endregion
		#region IEmission Implementation

		public bool NewParticleIsReady()
		{
			if (_spawnInterval <= 0.0f)
			{
				_spawnInterval = _preset.spawnInterval + AntMath.RandomRangeFloat(_preset.rndToSpawnInterval);
				return true;
			}

			return false;
		}

		public int Count
		{
			get
			{
				var rnd = AntMath.RandomRangeInt(
					Mathf.RoundToInt(_preset.rndToNumParticles.x),
					Mathf.RoundToInt(_preset.rndToNumParticles.y)
				);
				return _preset.numParticles + rnd;
			}
		}

		#endregion
	}
}