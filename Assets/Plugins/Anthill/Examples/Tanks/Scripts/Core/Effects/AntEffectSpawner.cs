namespace Anthill.Effects
{
	using System;
	using System.Collections.Generic;
	
	using UnityEngine;

	using Anthill.Pool;

	public class AntEffectSpawner : MonoBehaviour
	{
		public enum EKind
		{
			Basic,
			Physic
		}

		[System.Serializable]
		public struct Settings
		{
			public string name;                  // Имя эффекта.
			public GameObject prefab;            // Указатель на префаб эффекта.
			public EKind kind;                   // Тип эффекта.
			public bool isAutoPlay;              // Автоматически запускать эффект.
			public bool isUpdatePosition;        // Обновлять постоянно положение эффекта (для looping эффектов только).
			public bool isInheritAngle;          // Обновлять угол эффекта исходя из угла объкта.
			public bool isDisableOnInpact;       // Выключить эффект при физическом контакте.
			public bool isPlaceToImpactPosition; // Для спавна эффекта необходимо использовать точку столкновения.
			public bool isUseImpactAngle;
			public bool isImpactOnce;            // Воспроизводить эффект один раз при столкновении.
			public LayerMask impactLayerMask;    // Маска столкновения для отсеивания коллизий.

			public bool customDelay;
			public float delay;                  // Задержка перед началом проигрывания эффекта.
			public Vector2 rndToDelay;           // Случайное значение к задержке в заданном диапазоне.

			public bool customLifeTime;
			public float lifeTime;               // Время жизни эффекта.
			public Vector2 rndToLifeTime;        // Случайное значение к времени жизни эффекта.

			public float angle;
			public Vector2 offset;               // Смещение относительно объекта.

			public Color dotColor;               // Цвет точки для настройки эффекта.
			public bool isOpened;                // Открыты настройки эффекта или нет.
		}

		#region Variables

		// :: Public Variables ::

		[HideInInspector]
		public Settings[] effects = new Settings[0];

		// :: Private Variables ::

		private List<AntEffectMediator> _mediators;
		private Transform _transform;
		private AntPoolObject _poolObject;
		private bool _isActive;

		#endregion
		#region Unity Calls

		private void Awake()
		{
			_isActive = false;
			_transform = GetComponent<Transform>();
			_poolObject = GetComponent<AntPoolObject>();
			if (_poolObject != null)
			{
				_poolObject.EventExtractedFromPool += ExtractedFromPoolHandler;
				_poolObject.EventReturnedToPool += ReturnToPoolHandler;
			}
			else
			{
				ExtractedFromPoolHandler();
			}
		}

		private void Update()
		{
			if (_isActive)
			{
				float dt = Time.deltaTime;
				for (int i = 0, n = _mediators.Count; i < n; i++)
				{
					_mediators[i].Process(dt);
				}
			}
		}

		#endregion
		#region Public Methods

		public void SetOffset(string aEffectName, Vector2 aNewOffset)
		{
			for (int i = 0, n = effects.Length; i < n; i++)
			{
				if (effects[i].name.Equals(aEffectName))
				{
					effects[i].offset = aNewOffset;
				}
			}
		}

		public AntEffectMediator Play(string aEffectName)
		{
			_isActive = true;
			var mediator = GetMediator(aEffectName);
			mediator.Reset();
			mediator.Play();
			return mediator;
		}

		public AntEffectMediator Play(string aEffectName, Vector2 aPositionOffset)
		{
			_isActive = true;
			var mediator = GetMediator(aEffectName);
			mediator.Offset = aPositionOffset;
			mediator.Reset();
			mediator.Play();
			return mediator;
		}

		public AntEffectMediator Play(string aEffectName, Vector2 aPositionOffset, float aAngleDeg)
		{
			_isActive = true;
			var mediator = GetMediator(aEffectName);
			mediator.Offset = aPositionOffset;
			mediator.Angle = aAngleDeg;
			mediator.Reset();
			mediator.Play();
			return mediator;
		}

		public bool HasEffect(string aEffectName)
		{
			int index = Array.FindIndex(effects, x => x.name.Equals(aEffectName));
			return (index >= 0 && index < effects.Length);
		}

		public void Stop(string aEffectName)
		{
			for (int i = 0, n = _mediators.Count; i < n; i++)
			{
				if (_mediators[i].IsPlaying && _mediators[i].Name.Equals(aEffectName))
				{
					_mediators[i].Stop();
				}
			}
		}

		public void AutoPlay()
		{
			ExtractedFromPoolHandler();
		}

		public void StopAll()
		{
			_isActive = false;
			for (int i = 0, n = _mediators.Count; i < n; i++)
			{
				_mediators[i].Stop();
			}
		}

		public string[] GetEffectNames()
		{
			var result = new string[effects.Length];
			for (int i = 0, n = result.Length; i < n; i++)
			{
				result[i] = effects[i].name;
			}
			return result;
		}

		#endregion
		#region Private Methods

		private AntEffectMediator GetMediator(string aName)
		{
			int index = _mediators.FindIndex(x => (!x.IsPlaying && x.Name.Equals(aName)));
			if (index >= 0 && index < _mediators.Count)
			{
				int effIndex = Array.FindIndex(effects, x => x.name.Equals(aName));
				var oldMediator = _mediators[index];
				oldMediator.Offset = effects[effIndex].offset;
				oldMediator.Angle = effects[effIndex].angle;
				return oldMediator;
			}

			index = Array.FindIndex(effects, x => x.name.Equals(aName));
			if (index >= 0 && index < effects.Length)
			{
				var effect = effects[index];
				var newMediator = new AntEffectMediator(aName, _transform, effect);
				newMediator.Offset = effect.offset;
				newMediator.Angle = effect.angle;
				_mediators.Add(newMediator);
				return newMediator;
			}
			
			A.Assert(true, string.Format("Can't find the `{0}` effect for `{1}` object!", aName, gameObject.name), true);
			return null;
		}

		#endregion
		#region Event Handlers

		private void ExtractedFromPoolHandler()
		{
			_isActive = true;
			
			if (_mediators == null)
			{
				_mediators = new List<AntEffectMediator>();
			}

			Settings curItem;
			AntEffectMediator effect;
			for (int i = 0, n = effects.Length; i < n; i++)
			{
				curItem = effects[i];
				if (curItem.kind == EKind.Basic && curItem.isAutoPlay)
				{
					effect = GetMediator(curItem.name);
					effect.Reset();
					effect.Play();
				}
				else if (curItem.kind == EKind.Physic)
				{
					effect = GetMediator(curItem.name);
					effect.Reset();
				}
			}
		}

		private void ReturnToPoolHandler()
		{
			StopAll();
		}

		#endregion
	}
}