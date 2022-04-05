namespace Tanks
{
	using UnityEngine;
	using Anthill.Utils;
	using Anthill.Effects;

	/// <summary>
	/// This class implements the Bullet logic.
	/// </summary>
	public class Bullet : MonoBehaviour
	{
		#region Variables
		
		public float lifeTime = 10.0f;
		public float executeForce = 5.0f;
		public float damage = 0.35f;

		private SpriteRenderer _sprite;
		private CircleCollider2D _collider;
		private Rigidbody2D _body; 
		private float _currentLifeTime;
		private bool _isDead = true;
		private float _destroyDelay = 2.0f;
		
		#endregion
		#region Unity Calls
		
		private void Awake()
		{
			_sprite = GetComponent<SpriteRenderer>();
			_body = GetComponent<Rigidbody2D>();
			_collider = GetComponent<CircleCollider2D>();
		}

		private void Update()
		{
			if (_isDead)
			{
				// If bullet is killed, we should wait some time
				// before we can destroy it, because we have Trail effect.
				// Wait until the Trail effect finishes its animation.
				_destroyDelay -= Time.deltaTime;
				if (_destroyDelay <= 0.0f)
				{
					GameObject.Destroy(gameObject);
				}
			}
			else
			{
				// If the bullet has not hit anywhere for a long time, kill it.
				_currentLifeTime += Time.deltaTime;
				if (_currentLifeTime > lifeTime)
				{
					Kill();
				}
			}
		}
		
		private	void OnCollisionEnter2D(Collision2D aCollision)
		{
			if (aCollision.collider != null)
			{
				var tank = aCollision.collider.GetComponent<TankControl>();
				if (tank != null)
				{
					// If has collision with other tank, apply damage to it.
					tank.Health -= damage;
				}
			}

			// Make hit effect.
			AntEffectEngine.GetEffect("eff_TankHit", transform.position);
			
			// Kill the bullet.
			Kill();
		}

		#endregion
		#region Public Methods
		
		/// <summary>
		/// Executes bullet to specified angle.
		/// </summary>
		/// <param name="aAngle">Angle to move.</param>
		public void Execute(float aAngle)
		{
			_body.rotation = aAngle;
			float ang = aAngle * AntMath.RADIANS;
			var force = new Vector2(
				executeForce * Mathf.Cos(ang),
				executeForce * Mathf.Sin(ang)
			);

			_body.AddForce(force, ForceMode2D.Impulse);
		}
		
		#endregion
		#region Private Methods
		
		private void Kill()
		{
			_isDead = true;
			_destroyDelay = 2.0f;
			_body.simulated = false;
			_collider.isTrigger = true;
			_sprite.enabled = false;
		}
		
		#endregion
	}
}