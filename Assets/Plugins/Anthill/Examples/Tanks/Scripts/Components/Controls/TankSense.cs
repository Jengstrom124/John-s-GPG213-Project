namespace Tanks
{
	using System.Collections.Generic;
	using UnityEngine;
	using Anthill.AI;
	using Anthill.Utils;

	/// <summary>
	/// This class implements Tank sensitivity like a vision and touch sensors.
	/// </summary>
	public class TankSense : MonoBehaviour, ISense
	{
		#region Variables

		[Tooltip("Enable debug draw of the Tank vision.")]
		public bool showVision;
		
		private Transform _t;                // Reference to the Transform component.
		private TankControl _control;        // Reference to the Tank control.
		private TankBlackboard _blackboard;  // Bot memory.
		private CollectableItem[] _seeItems; // List of items that Tank see at the current moment.
		private TankControl[] _seeTanks;     // List of enemy Tanks that Bot see at the current moment.
		private TankControl[] _touchTanks;   // List of enemy Tanks that that we touch at the current moment.

		private const int SEE_ITEMS_COUNT = 4;
		private const int TANKS_COUNT = 3;

		// Helpers for debug drawing of vision.
		private List<Vector2> _seeFrom;
		private List<Vector2> _seeTo;
		
		#endregion
		#region ISense Implementation
		
		public void CollectConditions(AntAIAgent aAgent, AntAICondition aWorldState)
		{
			// 1. Remove all stored data from previous ticks.
			// ----------------------------------------------
			ClearSensors();

			// 2. Get fresh Vision and Touch data.
			// -----------------------------
			UpdateVision();
			UpdateTouch();

			// 3. Update World State.
			// ----------------------
			aWorldState.Set(TankScenario.Alive, true);
			aWorldState.Set(TankScenario.Injured, (_control.Health < _control.maxHealth));
			aWorldState.Set(TankScenario.EnemyAlive, true); // All time is true ;)

			aWorldState.Set(TankScenario.HasGun, _control.Tower.IsHasGun);
			aWorldState.Set(TankScenario.HasAmmo, _control.Tower.IsHasAmmo);
			aWorldState.Set(TankScenario.HasBomb, _control.Tower.IsHasBomb);

			// Reset items visibility.
			aWorldState.Set(TankScenario.SeeGun, false);
			aWorldState.Set(TankScenario.SeeAmmo, false);
			aWorldState.Set(TankScenario.SeeBomb, false);
			aWorldState.Set(TankScenario.SeeRepair, false);

			// Update vision for items.
			for (int i = 0; i < SEE_ITEMS_COUNT; i++)
			{
				if (_seeItems[i] != null)
				{
					// Adds founded spawner and item to the our memory.
					// This data maybe will be required by bot in the near future.
					_blackboard.TrackSpawner(_seeItems[i].Position, _seeItems[i].kind);
					_blackboard.TrackItem(_seeItems[i], _seeItems[i].kind);

					// Update the world state.
					switch (_seeItems[i].kind)
					{
						case ItemKind.Gun :
							aWorldState.Set(TankScenario.SeeGun, true);
							break;
						case ItemKind.Ammo :
							aWorldState.Set(TankScenario.SeeAmmo, true);
							break;
						case ItemKind.Bomb :
							aWorldState.Set(TankScenario.SeeBomb, true);
							break;
						case ItemKind.Repair :
							aWorldState.Set(TankScenario.SeeRepair, true);
							break;
					}
				}
			}
			
			// Update vision for enemies.
			aWorldState.Set(TankScenario.SeeEnemy, false);

			for (int i = 0; i < TANKS_COUNT; i++)
			{
				if (_seeTanks[i] != null)
				{
					_blackboard.TrackTank(_seeTanks[i]);
					aWorldState.Set(TankScenario.SeeEnemy, true);
				}
			}

			// Check the enemy on line of the fire.
			aWorldState.Set(TankScenario.OnLineofFire, false);

			if (_blackboard.HasEnemy)
			{
				float angle = AntMath.AngleDeg(_control.Position, _blackboard.EnemyTank.Position);
				if (AntMath.Equal(AntMath.Angle(angle), AntMath.Angle(_control.Tower.Angle), 10.0f))
				{
					aWorldState.Set(TankScenario.OnLineofFire, true);
				}
			}

			// Check the touching of enemy tanks.
			aWorldState.Set(TankScenario.NearEnemy, false);

			for (int i = 0; i < TANKS_COUNT; i++)
			{
				if (_touchTanks[i] != null)
				{
					aWorldState.Set(TankScenario.NearEnemy, true);
					break;
				}
			}
		}
		
		#endregion
		#region Unity Calls
		
		private void Awake()
		{
			_t = GetComponent<Transform>();
			_control = GetComponent<TankControl>();
			_blackboard = GetComponent<TankBlackboard>();
			_seeItems = new CollectableItem[SEE_ITEMS_COUNT];
			_seeTanks = new TankControl[TANKS_COUNT];
			_touchTanks = new TankControl[TANKS_COUNT];

			_seeFrom = new List<Vector2>();
			_seeTo = new List<Vector2>();
		}

		private void OnDrawGizmos()
		{
			if (Application.isPlaying && showVision)
			{
				Gizmos.color = Color.yellow;
				for (int i = 0, n = _seeFrom.Count; i < n; i++)
				{
					Gizmos.DrawLine(_seeFrom[i], _seeTo[i]);
				}
			}
		}
		
		#endregion
		#region Private Methods
		
		/// <summary>
		/// Updates vision sensor.
		/// </summary>
		private void UpdateVision()
		{
			var G = Game.Instance;

			// 1. See on the items.
			// --------------------
			float dist;
			float angle;
			Vector2 seeTo;
			Vector2 seeFrom;
			for (int i = G.collectableItems.Count - 1; i >= 0; i--)
			{
				// Calc distance to the item.
				dist = AntMath.Distance(_t.position, G.collectableItems[i].Position);
				if (dist < _control.visionRadius)
				{
					// If distance in our vision field.
					// Try to make raycast to it.
					angle = AntMath.AngleRad(_t.position, G.collectableItems[i].Position);
					seeFrom = new Vector2(
						_t.position.x + 0.4f * Mathf.Cos(angle),
						_t.position.y + 0.4f * Mathf.Sin(angle)
					);

					seeTo = new Vector2(
						dist * Mathf.Cos(angle), 
						dist * Mathf.Sin(angle)
					);

					var hit = Physics2D.Raycast(seeFrom, seeTo);
					if (hit.collider != null)
					{
						var ci = hit.collider.GetComponent<CollectableItem>();
						if (ci != null)
						{
							// Yay! We actually can see some item.
							// Just add it to our see list.
							AddItem(ci);

							// Save vision points for debug drawing.
							if (showVision)
							{
								seeTo += (Vector2) G.collectableItems[i].Position;
								_seeFrom.Add(seeFrom);
								_seeTo.Add(G.collectableItems[i].Position);
							}
						}
					}
				}
			}

			// 2. See on the enemies.
			// ----------------------
			for (int i = G.tanks.Count - 1; i >= 0; i--)
			{
				if (G.tanks[i].team == _control.team)
				{
					// Friendly tank. Skip...
					continue;
				}

				// Calc distance to the enemy tank.
				dist = AntMath.Distance(_t.position, G.tanks[i].Position);
				if (dist < _control.visionRadius)
				{
					// If distance in our vision field.
					// Try to make raycast to it.
					angle = AntMath.AngleRad(_t.position, G.tanks[i].Position);
					seeFrom = new Vector2(
						_t.position.x + 0.4f * Mathf.Cos(angle),
						_t.position.y + 0.4f * Mathf.Sin(angle)
					);

					seeTo = new Vector2(
						dist * Mathf.Cos(angle), 
						dist * Mathf.Sin(angle)
					);

					var hit = Physics2D.Raycast(seeFrom, seeTo);
					if (hit.collider != null)
					{
						var tc = hit.collider.GetComponent<TankControl>();
						if (tc != null && tc.team != _control.team)
						{
							// We actually can see enemy tank.
							// Add it to our see list.
							AddTank(tc);

							// Save vision points for debug drawing.
							if (showVision)
							{
								seeTo += (Vector2) G.tanks[i].Position;
								_seeFrom.Add(seeFrom);
								_seeTo.Add(G.tanks[i].Position);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Updates touch sensors.
		/// </summary>
		private void UpdateTouch()
		{
			// Check touch sensors if our tank really has a bomb.
			if (_control.Tower.IsHasBomb)
			{
				CheckTouch((_t.rotation.eulerAngles.z + 25.0f) * AntMath.RADIANS);
				CheckTouch(_t.rotation.eulerAngles.z * AntMath.RADIANS);
				CheckTouch((_t.rotation.eulerAngles.z - 25.0f) * AntMath.RADIANS);
			}

			// MARK: In this case, we could just check the OnEnterCollision2D with the tank, 
			//       but we are not looking for simple ways! ;)
			//       In fact, I was thinking about using this sensor to detect obstacles in the way.
		}

		/// <summary>
		/// Implementation for touch sensor.
		/// </summary>
		/// <param name="aAngleRad">Angle of touch sensor.</param>
		private void CheckTouch(float aAngleRad)
		{
			var senseFrom = new Vector2(
				_t.position.x + 0.4f * Mathf.Cos(aAngleRad),
				_t.position.y + 0.4f * Mathf.Sin(aAngleRad)
			);

			var senseTo = new Vector2(
				_t.position.x + 0.6f * Mathf.Cos(aAngleRad),
				_t.position.y + 0.6f * Mathf.Sin(aAngleRad)
			);

			var hit = Physics2D.Raycast(senseFrom, senseTo, 0.4f);
			if (hit.collider != null)
			{
				var tc = hit.collider.GetComponent<TankControl>();
				if (tc != null && tc.team != _control.team)
				{
					AddTouchTank(tc);
				}
			}
			
			if (showVision)
			{
				_seeFrom.Add(senseFrom);
				_seeTo.Add(senseTo);
			}
		}

		private void ClearSensors()
		{
			if (showVision)
			{
				_seeFrom.Clear();
				_seeTo.Clear();
			}

			for (int i = 0; i < SEE_ITEMS_COUNT; i++)
			{
				_seeItems[i] = null;
			}

			for (int i = 0; i < TANKS_COUNT; i++)
			{
				_seeTanks[i] = null;
				_touchTanks[i] = null;
			}
		}

		private void AddItem(CollectableItem aItem)
		{
			for (int i = 0; i < SEE_ITEMS_COUNT; i++)
			{
				if (_seeItems[i] == null)
				{
					_seeItems[i] = aItem;
					break;
				}
			}
		}

		private void AddTank(TankControl aTank)
		{
			for (int i = 0; i < TANKS_COUNT; i++)
			{
				if (_seeTanks[i] == null)
				{
					_seeTanks[i] = aTank;
					break;
				}
			}
		}

		private void AddTouchTank(TankControl aTank)
		{
			for (int i = 0; i < TANKS_COUNT; i++)
			{
				if (_touchTanks[i] == null)
				{
					_touchTanks[i] = aTank;
				}
			}
		}
		
		#endregion
	}
}