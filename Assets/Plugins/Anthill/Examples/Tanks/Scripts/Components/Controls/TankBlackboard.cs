namespace Tanks
{
	using System.Collections.Generic;
	using UnityEngine;
	using Anthill.Utils;

	/// <summary>
	/// This class implements the memory of the tank where it stores all the key information, 
	/// for example: 
	/// - Info about the points found where various bonuses appear. 
	/// - Info about the items seen to collect them.
	/// </summary>
	public class TankBlackboard : MonoBehaviour
	{
		private struct Spawner
		{
			public ItemKind kind;
			public Vector2 poisition;
		}

		private struct Item
		{
			public ItemKind kind;
			public CollectableItem obj;
		}

		private Transform _t;
		private TankControl _enemyTank;
		private float _enemyTankDist;
		private bool _hasEnemy;

		private List<Spawner> _spawners;
		private List<Item> _items;

		#region Unity Calls

		private void Awake()
		{
			_t = GetComponent<Transform>();
			_spawners = new List<Spawner>();
			_items = new List<Item>();
		}

		#endregion
		#region Public Methods

		/// <summary>
		/// Adds a point with a spawner to the tank’s memory 
		/// so that the next time you can find this point without 
		/// studying the map of the area.
		/// </summary>
		/// <param name="aPoint">Point.</param>
		/// <param name="aKind">Kind of the item.</param>
		public void TrackSpawner(Vector2 aPoint, ItemKind aKind)
		{
			int index = _spawners.FindIndex(x => x.kind == aKind);
			if (index >= 0 && index < _spawners.Count)
			{
				var item = _spawners[index];
				item.poisition = aPoint;
				_spawners[index] = item;
				return;
			}

			_spawners.Add(new Spawner
			{
				kind = aKind,
				poisition = aPoint
			});
		}

		/// <summary>
		/// Extracts from the memory a point with a spawner if you need 
		/// to find the corresponding object on the map.
		/// </summary>
		/// <param name="aKind">Kind of the required item.</param>
		/// <param name="aPosition">Result position.</param>
		/// <returns>True if position in memory is found.</returns>
		public bool GetSpawner(ItemKind aKind, out Vector2 aPosition)
		{
			aPosition = Vector2.zero;
			int index = _spawners.FindIndex(x => x.kind == aKind);
			if (index >= 0 && index < _spawners.Count)
			{
				aPosition = _spawners[index].poisition;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Adds founded collectable item to the memory.
		/// </summary>
		/// <param name="aObject">Item object.</param>
		/// <param name="aKind">Kind of the item.</param>
		public void TrackItem(CollectableItem aObject, ItemKind aKind)
		{
			// Subscribe for collection event of new item.
			aObject.EventCollected += CollectItemHandler;

			// Find item of this kind in the our memory.
			int index = _items.FindIndex(x => x.kind == aKind);
			if (index >= 0 && index < _items.Count)
			{
				// Found item with same kind in the memory,
				// then replace existing for new.
				var item = _items[index];
				
				if (item.obj != null)
				{
					// Unsubscribe from collection event of the existing item.
					item.obj.EventCollected -= CollectItemHandler;
				}

				item.obj = aObject;
				_items[index] = item;
				return;
			}

			// Just add item to our memory.
			_items.Add(new Item
			{
				kind = aKind,
				obj = aObject
			});
		}

		/// <summary>
		/// Gets item with specified kind from the bot memory.
		/// </summary>
		/// <param name="aKind">Kind of the required item.</param>
		/// <returns>Item object or null if item not existing in the memory.</returns>
		public CollectableItem GetItem(ItemKind aKind)
		{
			int index = _items.FindIndex(x => x.kind == aKind);
			if (index >= 0 && index < _items.Count)
			{
				return _items[index].obj;
			}

			return null;
		}

		/// <summary>
		/// Sets enemy tank as current target.
		/// </summary>
		/// <param name="aEnemyTank">Reference to the enemy tank.</param>
		public void TrackTank(TankControl aEnemyTank)
		{
			if (_enemyTank != null && !System.Object.ReferenceEquals(_enemyTank, aEnemyTank))
			{
				// If we already tracked other tank, then check what of them is close to us?
				float dist = AntMath.Distance(_t.position, aEnemyTank.Position);
				if (dist < _enemyTankDist)
				{
					// If new enemy closer, then replace old on the new one.
					_enemyTankDist = dist;
					_enemyTank.EventDestroyed -= TankDestroyedHandler;
					_enemyTank = aEnemyTank;
					_enemyTank.EventDestroyed += TankDestroyedHandler;
				}
			}
			else
			{
				// Just track the enemy.
				_enemyTankDist = AntMath.Distance(_t.position, aEnemyTank.Position);
				_enemyTank = aEnemyTank;
				_enemyTank.EventDestroyed += TankDestroyedHandler;
				_hasEnemy = true;
			}
		}

		#endregion
		#region Event Handlers

		private void CollectItemHandler(CollectableItem aItem)
		{
			aItem.EventCollected -= CollectItemHandler;

			// Remove collected item from our memory.
			int index = _items.FindIndex(x => x.kind == aItem.kind);
			if (index >= 0 && index < _items.Count)
			{
				_items.RemoveAt(index);
			}
		}

		private void TankDestroyedHandler(TankControl aTank)
		{
			aTank.EventDestroyed -= TankDestroyedHandler;
			_enemyTank = null;
			_hasEnemy = false;
		}

		#endregion
		#region Getters / Setters
		
		public bool HasEnemy { get => _hasEnemy; }
		public TankControl EnemyTank { get => _enemyTank; }
		
		#endregion
	}
}