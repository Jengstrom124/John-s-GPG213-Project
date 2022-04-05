namespace Anthill.Pool
{
	using UnityEngine;
	
	public class AntPoolObject : MonoBehaviour
	{
		public delegate void PoolActionDelegate();

		// Событие возникает после извлечения объекта из пула.
		public PoolActionDelegate EventExtractedFromPool;

		// Событие возникает когда объект попал в пул.
		public PoolActionDelegate EventPlacedToPool;

		// Событие возникает когда объект вернулся в пул после жизненного цикла.
		public PoolActionDelegate EventReturnedToPool;

		public AntPool Pool { get; set; }
		protected Transform _transform;

		#region Unity Calls

		private void Awake()
		{
			_transform = GetComponent<Transform>();
			Initialize();
		}

		private void OnDestroy()
		{
			Pool = null;
		}

		#endregion
		#region Public Methods

		public void ReturnToPool()
		{
			if (Pool != null)
			{
				//AntLog.Assert(Pool == null, string.Format("Can't return the \"{0}\" because object not poolled!", name), true);
				Pool.AddObject(this);

				if (EventReturnedToPool != null)
				{
					EventReturnedToPool();
				}
			}
		}

		#endregion
		#region Protected Methods

		protected virtual void Initialize()
		{
			// При наследовании класс от AntPoolObject, перекройте
			// данный метод для инициализации.
		}

		internal void ExtractedFromPool()
		{
			gameObject.SetActive(true);
			if (EventExtractedFromPool != null)
			{
				EventExtractedFromPool();
			}
		}

		internal void PlacedToPool()
		{
			gameObject.SetActive(false);
			if (EventPlacedToPool != null)
			{
				EventPlacedToPool();
			}
		}

		#endregion
		#region Getters / Setters

		public Transform Transform
		{
			get { return _transform; }
		}

		public virtual Vector2 Position
		{
			get { return _transform.position; }
			set
			{
				Vector3 p = _transform.position;
				p.x = value.x;
				p.y = value.y;
				_transform.position = p;
			}
		}

		public virtual Vector3 Position3D
		{
			get { return _transform.position; }
			set { _transform.position = value; }
		}

		public virtual Quaternion Rotation
		{
			get { return _transform.rotation; }
			set { _transform.rotation = value; }
		}

		#endregion
	}
}