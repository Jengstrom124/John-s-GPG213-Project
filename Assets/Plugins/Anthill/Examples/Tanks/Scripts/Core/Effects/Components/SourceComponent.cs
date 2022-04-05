namespace Anthill.Effects
{
	using Anthill.Pool;
	using Anthill.Utils;

	/// <summary>
	/// Данный компонент определяет какие именно игровые объекты будут
	/// использоваться в качестве частиц.
	/// </summary>
	public class SourceComponent : IComponent, ISource
	{
		#region Variables

		// :: Public Variables ::

		public AntPool[] pools;

		// :: Private Variables ::

		private int _prevIndex = -1;

		#endregion
		#region IComponent Implementation

		public void Initialize(IParticle aParticle)
		{
			// ...
		}

		public void Reset(AntEmitter aEmitter)
		{
			_prevIndex = -1;
			IsActive = false; // Выкл. обработку компонента.
		}

		public void Update(float aDeltaTime, float aTimeScale)
		{
			// ...
		}

		public void Destroy()
		{
			// ...
		}

		public AntEmitterPreset Preset { get; set; }
		public bool IsActive { get; private set; }
		public bool IsExists { get { return Preset.source.isExists; } }
		
		#endregion
		#region ISource Implementation

		public AntPool Pool 
		{
			get
			{
				_prevIndex++;
				if (Preset.source.selectRandom)
				{
					_prevIndex = AntMath.RandomRangeInt(0, Preset.source.prefabs.Length);
				}

				if (_prevIndex >= Preset.source.prefabs.Length)
				{
					_prevIndex = 0;
				}

				return AntPoolManager.GetPool(Preset.source.prefabs[_prevIndex].name);
			}
		}

		#endregion
	}
}