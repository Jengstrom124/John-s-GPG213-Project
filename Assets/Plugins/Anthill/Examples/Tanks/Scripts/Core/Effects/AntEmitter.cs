namespace Anthill.Effects
{
	using System;
	using System.Reflection;
	using System.Collections.Generic;

	using Anthill.Pool;
	using Anthill.Utils;

	public class AntEmitter
	{
		#region Variables

		// :: Public Variables ::

		public int ID { get; private set; }
		public List<AntPool> emissionList;
		public IComponent[] components;

		public bool IsActive { get; private set; }

		// Переменная для SpawnPositionComponent.
		public int SpawnCount { get; set; } 

		// :: Private Variables ::

		internal static int AVAIL_ID;
		private ActorComponent _actorComponent;
		private bool _hasActor;
		private IEmission[] _emissionComponents;
		private ISource _sourceComponent;

		#endregion
		#region Initialization

		public AntEmitter(AntEmitterPreset aPreset)
		{
			ID = AVAIL_ID++;
			IsActive = false;

			emissionList = new List<AntPool>();

			var availComponents = AntEffectEngine.AvailableComponents;
			components = new IComponent[availComponents.Length];
			for (int i = 0, n = availComponents.Length; i < n; i++)
			{
				ConstructorInfo constructor = availComponents[i].GetConstructor(Type.EmptyTypes);
				var comp = (IComponent) constructor.Invoke(null);
				if (comp != null)
				{
					comp.Preset = aPreset;
					components[i] = comp;
				}
			}
		}

		#endregion
		#region Public Methods

		public void Reset()
		{
			emissionList.Clear();

			_actorComponent = FindComponent<ActorComponent>(true);
			_sourceComponent = FindComponent<ISource>();
			_emissionComponents = new IEmission[0];
			FindComponents<IEmission>(ref _emissionComponents);
			
			IsActive = (_emissionComponents.Length > 0 && _sourceComponent != null);
			
			if (IsActive)
			{
				_sourceComponent.Reset(this);
				for (int i = 0, n = _emissionComponents.Length; i < n; i++)
				{
					_emissionComponents[i].Reset(this);
				}
			}
		}

		public void Update(float aDeltaTime, float aTimeScale)
		{
			int finishedCount = 0;
			IEmission emission;
			for (int i = 0, n = _emissionComponents.Length; i < n; i++)
			{
				emission = _emissionComponents[i];
				if (emission.IsActive)
				{
					emission.Update(aDeltaTime, aTimeScale);
					if (emission.NewParticleIsReady())
					{
						// Добавляем количество частиц в спавн лист.
						for (int j = 0, jn = emission.Count; j < jn; j++)
						{
							emissionList.Add(_sourceComponent.Pool);
						}

						if (!emission.IsActive)
						{
							// Если источник активен,
							// работа эффекта еще не закончена.
							finishedCount++;
						}
					}
				}
			}

			// Отключаем работу эффекта если все частицы заспавнились.
			if (finishedCount == _emissionComponents.Length)
			{
				IsActive = false;
			}
		}

		#endregion
		#region Private Methods

		private T FindComponent<T>(bool aIgnoreExist = false)
		{
			for (int i = 0, n = components.Length; i < n; i++)
			{
				if (components[i] is T && (components[i].IsExists || aIgnoreExist))
				{
					return (T) components[i];
				}
			}
			return default(T);
		}

		private void FindComponents<T>(ref T[] aTargetList)
		{
			for (int i = 0, n = components.Length; i < n; i++)
			{
				if (components[i] is T && components[i].IsExists)
				{
					AntArray.Add<T>(ref aTargetList, (T) components[i]);
				}
			}
		}

		#endregion
		#region Getters / Settes

		public bool EmissionIsReady
		{
			get { return emissionList.Count > 0; }
		}

		public bool IsAllowSorting
		{
			get { return (_actorComponent.Preset.actor.isExists && _actorComponent.Preset.actor.sorting); }
		}

		public ActorComponent Actor
		{
			get { return _actorComponent; }
		}

		#endregion
	}
}