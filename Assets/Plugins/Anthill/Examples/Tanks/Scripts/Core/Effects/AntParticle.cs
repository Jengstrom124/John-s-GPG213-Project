namespace Anthill.Effects
{
	using System;
	using System.Reflection;

	using UnityEngine;
	using UnityEngine.Rendering;

	using Anthill.Pool;

	[RequireComponent(typeof(AntPoolObject))]
	public class AntParticle : MonoBehaviour, IParticle
	{
		#region Variables

		// :: Public Variables ::

		public bool IsHasCollision { get; set; }

		// :: Private Variables ::

		private AntPoolObject _poolObject;
		private IComponent[] _components;

		private SortingGroup _sortingGroup;
		private SpriteRenderer _spriteRenderer;
		private bool _hasSortingGroup;
		private bool _hasSpriteRenderer;
		private int _componentsCount;

		#endregion
		#region Unity Calls

		private void Awake()
		{
			_poolObject = GetComponent<AntPoolObject>();
			var availComponents = AntEffectEngine.AvailableComponents;
			_components = new IComponent[availComponents.Length];
			_componentsCount = _components.Length;
			for (int i = 0, n = availComponents.Length; i < n; i++)
			{
				ConstructorInfo constructor = availComponents[i].GetConstructor(Type.EmptyTypes);
				var comp = (IComponent) constructor.Invoke(null);
				if (comp != null)
				{
					comp.Initialize(this);
					_components[i] = comp;
				}
			}

			_sortingGroup = GetComponent<SortingGroup>();
			_hasSortingGroup = (_sortingGroup != null);
			if (!_hasSortingGroup)
			{
				_spriteRenderer = GetComponent<SpriteRenderer>();
				_hasSpriteRenderer = (_spriteRenderer != null);
			}
		}

		private void OnCollisionEnter2D(Collision2D aCollider)
		{
			IsHasCollision = true;
		}

		#endregion
		#region Public Methods

		private void Update()
		{
			if (IsActive)
			{
				IComponent component;
				for (int i = 0; i < _componentsCount; i++)
				{
					component = _components[i];
					if (component.IsExists && component.IsActive)
					{
						component.Update(Time.deltaTime, Time.timeScale);
					}
				}

				CurrentLifeTime += Time.deltaTime;
			}
		}

		#endregion
		#region IParticle Implementation

		public void Activate(AntEffect aEffect, AntEmitter aEmitter)
		{
			Effect = aEffect;
			CurrentLifeTime = 0.0f;
			IsActive = true;

			for (int i = 0; i < _componentsCount; i++)
			{
				_components[i].Preset = aEmitter.components[i].Preset;
				if (aEmitter.components[i].IsExists)
				{
					_components[i].Reset(aEmitter);
				}
			}
		}

		public void Kill()
		{
			Effect = null;
			IsActive = false;
			_poolObject.ReturnToPool();
		}

		public int EffectID { get; set; }
		public float BirthTime { get; set; }

		public string SortingLayer
		{
			get
			{
				return (_hasSortingGroup) 
					? _sortingGroup.sortingLayerName 
					: (_hasSpriteRenderer)
						? _spriteRenderer.sortingLayerName
						: null;
			}
			set
			{
				if (_hasSortingGroup)
				{
					_sortingGroup.sortingLayerName = value;
				}
				else if (_hasSpriteRenderer)
				{
					_spriteRenderer.sortingLayerName = value;
				}
			}
		}

		public int SortingOrder
		{ 
			get
			{
				return (_hasSortingGroup)
					? _sortingGroup.sortingOrder
					: (_hasSpriteRenderer)
						? _spriteRenderer.sortingOrder
						: 0;
			}
			set
			{
				if (_hasSortingGroup)
				{
					_sortingGroup.sortingOrder = value;
				}
				else if (_hasSpriteRenderer)
				{
					_spriteRenderer.sortingOrder = value;
				}
			}
		}

		public GameObject GameObject
		{
			get { return this.gameObject; }
		}

		public AntEffect Effect { get; private set; }
		public float LifeTime { get; set; }
		public float CurrentLifeTime { get; set; }
		public bool IsActive { get; set; }

		#endregion
	}
}