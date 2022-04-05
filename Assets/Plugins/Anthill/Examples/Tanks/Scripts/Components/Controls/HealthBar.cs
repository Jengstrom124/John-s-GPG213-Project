namespace Tanks
{
	using UnityEngine;
	using Anthill.Animation;

	/// <summary>
	/// Implementation of the health bar for the unit.
	/// </summary>
	public class HealthBar : MonoBehaviour
	{
		#region Variables

		[Tooltip("Offset of the health bar.")]
		public Vector2 offset = new Vector2(0.0f, -0.75f);
		
		private Transform _t;
		private SpriteRenderer _sprite;
		private AntActor _actor;
		private TankControl _tank;
		private bool _hasTarget;
		
		#endregion
		#region Unity Calls
		
		private void Awake()
		{
			_t = GetComponent<Transform>();
			_actor = GetComponent<AntActor>();
			_sprite = GetComponent<SpriteRenderer>();
		}

		private void Update()
		{
			if (_hasTarget)
			{
				// Updating position.
				var p = _t.position;
				p.x = _tank.Position.x + offset.x;
				p.y = _tank.Position.y + offset.y;
				_t.position = p;
			}
		}
		
		#endregion
		#region Public Methods
		
		/// <summary>
		/// Sets target unit for the health bar for observing of the health.
		/// </summary>
		/// <param name="aTank">Target tank.</param>
		public void SetTarget(TankControl aTank)
		{
			_tank = aTank;
			_tank.EventChangeHealth += TankChangeHealthHandler;
			_tank.EventDestroyed += TankDestroyedHandler;
			_hasTarget = true;
			UpdateVisual();
		}

		#endregion
		#region Private Methods

		private void UpdateVisual()
		{
			var percent = _tank.Health / _tank.maxHealth;
			var frame = Mathf.RoundToInt(_actor.TotalFrames * percent) + 1;
			frame = (frame > _actor.TotalFrames) ? _actor.TotalFrames : frame;
			_actor.GotoAndStop(frame);
			_sprite.enabled = (frame != _actor.TotalFrames);
		}
		
		#endregion
		#region Event Handlers
		
		private void TankDestroyedHandler(TankControl aTank)
		{
			// If target is destroyed, destroy the health bar too.
			aTank.EventDestroyed -= TankDestroyedHandler;
			aTank.EventChangeHealth -= TankChangeHealthHandler;
			_tank = null;
			_hasTarget = false;
			GameObject.Destroy(gameObject);
		}

		private void TankChangeHealthHandler(TankControl aTank)
		{
			// If target changed health value, update the health bar too.
			UpdateVisual();
		}
		
		#endregion
	}
}