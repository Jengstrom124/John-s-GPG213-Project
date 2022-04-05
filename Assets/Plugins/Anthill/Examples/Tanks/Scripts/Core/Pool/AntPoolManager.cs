namespace Anthill.Pool
{
	using System.Collections.Generic;
	using UnityEngine;

	public static class AntPoolManager
	{
		private static Transform _parent;
		private readonly static List<AntPool> _pools = new List<AntPool>();

		#region Public Methods

		public static void SetParent(Transform aParent)
		{
			_parent = aParent;
		}

		public static void CreatePool(string aResourcePath, int aCapacity)
		{
			Object obj = Resources.Load(aResourcePath);
			if (obj != null)
			{
				if (obj is GameObject)
				{
					AddPool(obj, aCapacity);
				}
				else
				{
					A.Warning("Object `{0}` [{1}] is not a GameObject!", aResourcePath, obj);
				}
			}
			else
			{
				A.Warning("Can't load the object `{0}`!", aResourcePath);
			}
		}

		public static void AddPool(Object aSamplePrefab, int aCapacity)
		{
			var go = new GameObject();
			go.name = aSamplePrefab.name;
			if (_parent != null)
			{
				go.transform.SetParent(_parent);
			}
			
			_pools.Add(new AntPool(aSamplePrefab, aCapacity, go.transform));
		}

		public static List<string> GetPoolNames()
		{
			var result = new List<string>();
			for (int i = 0, n = _pools.Count; i < n; i++)
			{
				result.Add(_pools[i].name);
			}
			return result;
		}

		public static List<string> GetPoolNames<T>()
		{
			var result = new List<string>();
			for (int i = 0, n = _pools.Count; i < n; i++)
			{
				if (_pools[i].HasComponent<T>())
				{
					result.Add(_pools[i].name);
				}
			}
			return result;
		}

		public static T GetObject<T>(string aName, Vector2 aPosition, Quaternion aRotation)
		{
			T result = default(T);
			AntPoolObject po = GetObject(aName, aPosition, aRotation);
			if (po != null)
			{
				result = po.GetComponent<T>();
			}
			return result;
		}

		public static T GetObject<T>(string aName, Vector2 aPosition)
		{
			T result = default(T);
			AntPoolObject po = GetObject(aName, aPosition);
			if (po != null)
			{
				result = po.GetComponent<T>();
			}
			return result;
		}

		public static T GetObject<T>(string aName)
		{
			T result = default(T);
			AntPoolObject po = GetObject(aName);
			if (po != null)
			{
				result = po.GetComponent<T>();
			}
			return result;
		}

		public static AntPoolObject GetObject(string aName, Vector3 aPosition, Quaternion aRotation)
		{
			AntPoolObject result = GetObject(aName);
			if (result != null)
			{
				result.Position = aPosition;
				result.Rotation = aRotation;
			}
			return result;
		}

		public static AntPoolObject GetObject(string aName, Vector3 aPosition)
		{
			AntPoolObject result = GetObject(aName);
			if (result != null)
			{
				result.Position = aPosition;
			}
			return result;
		}

		public static AntPoolObject GetObject(string aName)
		{
			AntPool pool;
			for (int i = 0, n = _pools.Count; i < n; i++)
			{
				pool = _pools[i];
				if (pool.name.Equals(aName))
				{
					return pool.GetObject();
				}
			}
			
			A.Warning("Can't find the `{0}` in the pool!", aName);
			return null;
		}

		public static AntPool GetPool(string aName)
		{
			AntPool pool;
			for (int i = 0, n = _pools.Count; i < n; i++)
			{
				pool = _pools[i];
				if (pool.name.Equals(aName))
				{
					return pool;
				}
			}
			return null;
		}

		public static void DebugInfo()
		{
			for (int i = 0, n = _pools.Count; i < n; i++)
			{
				if (_pools[i].GrowCount > 0)
				{
					A.Log("Pool `{0}` grew by {1}", _pools[i].name, _pools[i].GrowCount);
				}
			}
		}

		#endregion
	}
}