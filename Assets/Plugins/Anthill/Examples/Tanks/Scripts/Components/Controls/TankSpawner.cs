namespace Tanks
{
	using UnityEngine;
	using Anthill.Animation;

	/// <summary>
	/// Implementation of the spawner for the Tank units.
	/// </summary>
	public class TankSpawner : MonoBehaviour
	{
		#region Variables
		
		public bool turnedOn;
		public TankTeam team;
		public float spawnInterval = 4.0f;

		private Transform _t;
		private AntActor _actor;
		private float _currentInterval;
		private bool _isActive;
		
		#endregion
		#region Unity Calls
		
		private void Awake()
		{
			_t = GetComponent<Transform>();
			_actor = GetComponent<AntActor>();
			_actor.GotoAndStop(1);
		}

		private void Start()
		{
			// Immediatly spawn new tank.
			if (turnedOn)
			{
				_currentInterval = spawnInterval - 0.25f;
				_isActive = true;
			}
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
					SpawnTank();
				}
			}
		}
		
		#endregion
		#region Private Methods
		
		private void SpawnTank()
		{
			// Disable the spawn timer.
			_isActive = false;
			_actor.GotoAndStop(1);

			// Create the tank.
			var item = Game.Instance.SpawnTank(transform.position, team, _t.parent.transform);
			item.EventDestroyed += TankDestroyedHandler;
		}

		private void TankDestroyedHandler(TankControl aTank)
		{
			// Tank of this spawner is destroyed, activating the spawner to spawn new Tank.
			aTank.EventDestroyed -= TankDestroyedHandler;
			_isActive = true;
		}
		
		#endregion
	}
}