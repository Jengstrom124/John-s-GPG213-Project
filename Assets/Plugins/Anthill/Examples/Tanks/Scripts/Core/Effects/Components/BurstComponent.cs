namespace Anthill.Effects
{
	using Anthill.Utils;

	/// <summary>
	/// Данный компонент спавнит заданное кол-во частиц в заданное время
	/// после спавна эффекта.
	/// </summary>
	public class BurstComponent : IComponent, IEmission
	{
		#region Variables

		private Burst _preset;
		private int _currentIndex;
		private int _count;
		private float _delay;

		private bool _isResetted;
		private bool _isActive;

		#endregion
		#region IComponent Implementation

		public void Initialize(IParticle aParticle)
		{
			// ..
		}

		public void Reset(AntEmitter aEmitter)
		{
			if (_isResetted)
			{
				return;
			}
			
			_isResetted = true;
			_preset = Preset.burst;
			IsActive = (_preset.list.Length > 0);
			_currentIndex = 0;
			_count = 0;
			if (IsActive)
			{
				_delay = _preset.list[_currentIndex].time;
			}
		}

		public void Update(float aDeltaTime, float aTimeScale)
		{
			_delay -= aDeltaTime * aTimeScale;
		}

		public void Destroy()
		{
			// ..
		}

		public AntEmitterPreset Preset { get; set; }
		public bool IsActive
		{
			get { return _isActive; }
			private set
			{
				_isActive = value;
				if (!value)
				{
					_isResetted = false;
				}
			}
		}
			
		public bool IsExists
		{
			get { return Preset.burst.isExists; }
		}

		#endregion
		#region IEmission Implementation

		public bool NewParticleIsReady()
		{
			if (IsActive && _delay <= 0.0f)
			{
				_count = AntMath.RandomRangeInt(
					_preset.list[_currentIndex].min,
					_preset.list[_currentIndex].max
				);
				
				_currentIndex++;
				IsActive = (_currentIndex < _preset.list.Length);
				if (IsActive)
				{
					_delay = _preset.list[_currentIndex].time;
				}

				return true;
			}
			return false;
		}

		public int Count
		{
			get { return _count; }
		}

		#endregion
	}
}