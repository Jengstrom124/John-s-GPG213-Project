namespace Tanks
{
	using System.Collections.Generic;
	using UnityEngine;
	using Anthill.Pool;

	/// <summary>
	/// This is main class of the AI demo.
	/// </summary>
	public class Game : MonoBehaviour
	{
		// Struct for items list.
		[System.Serializable]
		public struct AvailItem
		{
			public ItemKind kind;
			public GameObject prefab;
		}

		// Struct for tanks list.
		[System.Serializable]
		public struct AvailTank
		{
			public TankTeam team;
			public GameObject prefab;
		}

		#region Variables

		public static NavMesh NavMesh { get; private set; }
		public static Game Instance { get; private set; }

		public GameObject bulletPrefab;
		public GameObject healthBar;

		[Tooltip("List of CollectableItems that can be spawned on the map.")]
		public AvailItem[] availableItems = new AvailItem[0];

		[Tooltip("List of Tanks that can be spawned on the map.")]
		public AvailTank[] availableTanks = new AvailTank[0];

		// List of original objects for spawning of collectable items.
		[System.NonSerialized]
		public List<CollectableItem> collectableItems;

		// List of original objects for spawning of tanks.
		[System.NonSerialized]
		public List<TankControl> tanks;
		
		#endregion
		#region Unity Calls

		private void Awake()
		{
			Instance = this;

			// Creating lists of original objects for the items
			// and tanks.
			collectableItems = new List<CollectableItem>();
			tanks = new List<TankControl>();

			// Get the reference on the Navigation Mesh.
			var go = GameObject.Find("NavMesh");
			A.Assert(go == null, "Can't find the NavMesh.");
			NavMesh = go.GetComponent<NavMesh>();

			var poolLoader = GetComponent<AntPoolLoader>();
			poolLoader.StartLoading();
		}

		#endregion
		#region Public Methods
		
		/// <summary>
		/// Spawn specified collectable item on the map.
		/// </summary>
		/// <param name="aPosition">Position of the item.</param>
		/// <param name="aKind">Kind of the item.</param>
		/// <param name="aParent">Parent transfrom.</param>
		/// <returns>CollectableItem or null.</returns>
		public CollectableItem SpawnItem(Vector2 aPosition, ItemKind aKind, Transform aParent = null)
		{
			int index = System.Array.FindIndex(availableItems, x => x.kind == aKind);
			if (index >= 0 && index < availableItems.Length)
			{
				// Create new item on the map.
				A.Assert(availableItems[index].prefab == null, $"Prefab for `{aKind}` collectable item is missed.");
				var go = GameObject.Instantiate(availableItems[index].prefab);
				go.transform.position = aPosition;
				if (aParent != null)
				{
					go.transform.SetParent(aParent, false);
				}
				
				// Add the item to the game and subscribe to collect event 
				// to know when need to remove the item from list.
				var item = go.GetComponent<CollectableItem>();
				item.EventCollected += ItemCollectHandler;
				collectableItems.Add(item);
				return item;
			}
			
			A.Warning($"Can't create `{aKind}` collectable item!");
			return null;
		}

		/// <summary>
		/// Spawn specified tank on the map.
		/// </summary>
		/// <param name="aPosition">Position of the tank.</param>
		/// <param name="aTeam">Tank Team (color).</param>
		/// <param name="aParent">Parent transform.</param>
		/// <returns>New tank or null.</returns>
		public TankControl SpawnTank(Vector2 aPosition, TankTeam aTeam, Transform aParent = null)
		{
			int index = System.Array.FindIndex(availableTanks, x => x.team == aTeam);
			if (index >= 0 && index < availableTanks.Length)
			{
				// Create new tank on the map.
				A.Assert(availableTanks[index].prefab == null, $"Prefab for `{aTeam}` tank is missed.");
				var go = GameObject.Instantiate(availableTanks[index].prefab);
				go.transform.position = aPosition;
				if (aParent != null)
				{
					go.transform.SetParent(aParent, false);
				}

				// Add the tank to the game and subscribe to destroy event
				// to know when need to remove the tank from list.
				var tank = go.GetComponent<TankControl>();
				tank.EventDestroyed += TankDestroyedHandler;
				tanks.Add(tank);

				// Create the health bar for the tank.
				A.Assert(healthBar == null, "Can't find template prefab for the HealthBar!");
				go = GameObject.Instantiate(healthBar);
				var hp = go.GetComponent<HealthBar>();
				hp.SetTarget(tank);
				aPosition.x += hp.offset.x;
				aPosition.y += hp.offset.y;
				go.transform.position = aPosition;
				if (aParent != null)
				{
					go.transform.SetParent(aParent, false);
				}

				return tank;
			}

			A.Warning($"Can't create `{aTeam}` tank!");
			return null;
		}

		/// <summary>
		/// Spawn tank bullet on the map.
		/// </summary>
		/// <param name="aPosition">Position of the bullet.</param>
		/// <param name="aParent">Parent transform.</param>
		/// <returns></returns>
		public Bullet SpawnBullet(Vector2 aPosition, Transform aParent)
		{
			A.Assert(bulletPrefab == null, "Can't find template prefab for the bullet!");
			var go = GameObject.Instantiate(bulletPrefab);
			go.transform.position = aPosition;
			if (aParent != null)
			{
				go.transform.SetParent(aParent, false);
			}

			return go.GetComponent<Bullet>();
		}
		
		#endregion
		#region Event Handlers
		
		private void ItemCollectHandler(CollectableItem aItem)
		{
			aItem.EventCollected -= ItemCollectHandler;
			collectableItems.Remove(aItem);
		}

		private void TankDestroyedHandler(TankControl aTank)
		{
			aTank.EventDestroyed -= TankDestroyedHandler;
			tanks.Remove(aTank);
		}
		
		#endregion
	}
}