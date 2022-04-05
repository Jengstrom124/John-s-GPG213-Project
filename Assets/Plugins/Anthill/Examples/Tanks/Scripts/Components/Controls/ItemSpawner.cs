namespace Tanks
{
	using UnityEngine;
	using Anthill.Animation;

	/// <summary>
	/// This script implements the work of spawners that creates various items on the map.
	/// </summary>
	public class ItemSpawner : MonoBehaviour
	{
		[Tooltip("Kind of the item that will be spawned.")]
		public ItemKind kind;

		[Tooltip("Time delay before a new thing is created.")]
		public float spawnInterval = 4.0f;

		private Transform _t;
		private AntActor _actor;
		private float _currentInterval;
		private bool _isActive;

		#region Unity Calls

		private void Awake()
		{
			_t = GetComponent<Transform>();
			_actor = GetComponent<AntActor>();
		}

		private void Start()
		{
			_isActive = true;
		}

		private void Update()
		{
			if (_isActive)
			{
				// Showing the time to next spawn.
				var p = _currentInterval / spawnInterval;
				_actor.GotoAndStop(Mathf.RoundToInt(p * _actor.TotalFrames));

				// Spawn new item.
				_currentInterval += Time.deltaTime;
				if (_currentInterval >= spawnInterval)
				{
					_currentInterval = 0.0f;
					SpawnBonus();
				}
			}
		}

		#endregion
		#region Private Methods

		private void SpawnBonus()
		{
			// Disable the spawn timer.
			_isActive = false;
			_actor.GotoAndStop(1);

			// Create the item.
			var item = Game.Instance.SpawnItem(transform.position, kind, _t.parent.transform);
			item.EventCollected += ItemCollectHandler;
		}

		private void ItemCollectHandler(CollectableItem aItem)
		{
			// If item of this spawner is collected, so enable spawner to spawn new item.
			aItem.EventCollected -= ItemCollectHandler;
			_isActive = true;
		}

		#endregion
	}
}