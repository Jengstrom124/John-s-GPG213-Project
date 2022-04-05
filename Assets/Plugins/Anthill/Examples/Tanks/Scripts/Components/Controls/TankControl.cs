namespace Tanks
{
	using System.Collections.Generic;
	using UnityEngine;
	using Anthill.Utils;
	using Anthill.Animation;
	using Anthill.Effects;

	public enum TankTeam
	{
		RedTeam,
		GreenTeam
	}
	
	public class TankControl : MonoBehaviour
	{
		public delegate void TankControlDelegate(TankControl aControl);
		public event TankControlDelegate EventArrived;
		public event TankControlDelegate EventWayNotFound;
		public event TankControlDelegate EventDestroyed;
		public event TankControlDelegate EventChangeHealth;

		[Header("Parts")]
		[Tooltip("Reference to the tower object.")]
		public TankTower towerRef;

		[Header("Settings")]
		public TankTeam team;

		[Tooltip("Health.")]
		public float maxHealth = 1.0f;
		
		[Tooltip("Radius of the vision for tank.")]
		public float visionRadius = 2.0f;

		[Tooltip("Radius of the magnet to getting the items.")]
		public float magnetRadius = 1.0f;

		[Tooltip("The radius at which the attracted item will be collected.")]
		public float collectRadius = 0.2f;

		[Header("Movement")]

		[Tooltip("Tank may moved by mouse clicking on the map.")]
		public bool mouseControl;

		[Tooltip("Speed rotation of the tank tower.")]
		public float towerRotationSpeed = 1.0f;

		[Tooltip("Amount of the time that need to rotate for 360 degrees.")]
		public float totalSteeringTime;

		[Tooltip("Steering curve for smooth rotation.")]
		public AnimationCurve steeringCurve;

		public float totalAccelTime;
		public AnimationCurve accelCurve;
		public AnimationCurve breakCurve;
		public float moveSpeed = 10.0f;

		private Transform _t;
		private Rigidbody2D _body;
		private AntActor _actor;
		private AntEffectSpawner _effect;

		private bool _isMove = false;
		private bool _isBrake = false;
		private List<Vector2> _route = new List<Vector2>();
		private int _pointIndex;
		private Vector2 _currentPoint;

		private bool _isSteering = true;
		private float _steeringTime = 0.0f;
		private float _totalSteeringTime = 0.0f;
		private float _speed = 0.0f;
		private float _accelerationTime = 0.0f;
		private float _health;

		private bool _isSmoke;
		private float _debugTargetAngle;

		#region Unity Calls

		private void OnEnable()
		{
			Health = maxHealth;
		}

		private void Awake()
		{
			_t = GetComponent<Transform>();
			_body = GetComponent<Rigidbody2D>();
			_effect = GetComponent<AntEffectSpawner>();
			_actor = GetComponent<AntActor>();
			_actor.Stop();
		}

		private void Update()
		{
			// Debug controlling of the any Unit on the map if enabled mousControl.
			if (mouseControl && Input.GetMouseButtonDown(0))
			{
				MoveTo(AntMath.GetMouseWorldPosition());
			}

			if (_isMove)
			{
				UpdateMovement();
			}
			else
			{
				Velocity = Vector2.zero;
				StopMovementEffects();
			}
		}

		private void OnDrawGizmos()
		{
			if (Application.isPlaying)
			{
				float currentAngle = Angle * AntMath.RADIANS;
				var from = (Vector2) _t.position;
				var to = new Vector2(from.x + 0.5f * Mathf.Cos(currentAngle), from.y + 0.5f * Mathf.Sin(currentAngle));
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(from, to);

				to = new Vector2(from.x + 0.5f * Mathf.Cos(_debugTargetAngle), from.y + 0.5f * Mathf.Sin(_debugTargetAngle));
				Gizmos.color = Color.red;
				Gizmos.DrawLine(from, to);

				Gizmos.color = Color.yellow;
				DrawCircle(_t.position, visionRadius, 24);
			}
		}

		private void DrawCircle(Vector2 aPoint, float aRadius, int aVertices)
		{
			var v = (aVertices >= 3) ? aVertices : 3;
			float dx = aRadius;
			const float dy = 0.0f;

			float angle = 0.0f;
			var first = new Vector2(
				aPoint.x + Mathf.Cos(angle) * dx - Mathf.Sin(angle) * dy,
				aPoint.y - (Mathf.Sin(angle) * dx + Mathf.Cos(angle) * dy)
			);

			var prev = first;
			Vector2 cur = Vector2.zero;
			for (int i = 0; i < v; i++)
			{
				angle = ((i / (float) v) * 360.0f) * Mathf.Deg2Rad;
				cur.x = aPoint.x + Mathf.Cos(angle) * dx - Mathf.Sin(angle) * dy;
				cur.y = aPoint.y - (Mathf.Sin(angle) * dx + Mathf.Cos(angle) * dy);
				Gizmos.DrawLine(prev, cur);
				prev = cur;
			}

			Gizmos.DrawLine(prev, first);
		}

		#endregion
		#region Public Methods

		/// <summary>
		/// Rotates the tank tower.
		/// </summary>
		/// <param name="aDir">Direction of rotation.</param>
		/// <param name="aDeltaTime">Delta.</param>
		public void RotateTower(float aDir, float aDeltaTime)
		{
			float angle = towerRef.Rotation.eulerAngles.z + towerRotationSpeed * aDir;
			Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
 			towerRef.Rotation = Quaternion.Slerp(towerRef.Rotation, q, aDeltaTime);
		}

		/// <summary>
		/// Start moving of the tank to specified point.
		/// </summary>
		/// <param name="aPoint">Point where tank should to go.</param>
		public void MoveTo(Vector2 aPoint)
		{
			if (Game.NavMesh.FindWay(_t.position, aPoint, ref _route))
			{
				// If we found the way.
				StartMove();
			}
			else
			{
				// Way not found.
				StopMove();

				if (EventWayNotFound != null)
				{
					EventWayNotFound(this);
				}
			}
		}

		/// <summary>
		/// Starts tank movement.
		/// </summary>
		public void StartMove()
		{
			_isMove = true;
			_isBrake = false;
			_accelerationTime = 0.0f;
			_currentPoint = _route[0];
			_pointIndex = 0;
			_actor.Play();
			StopMovementEffects();
			PlayMovementEffects();
		}

		/// <summary>
		/// Stops tank movement.
		/// </summary>
		public void StopMove()
		{
			_isMove = false;
			_isBrake = false;
			_speed = 0.0f;
			_accelerationTime = 0.0f;
			Velocity = Vector2.zero;
			_actor.Stop();
			StopMovementEffects();
		}

		/// <summary>
		/// Starts playing of the movement effects.
		/// </summary>
		public void PlayMovementEffects()
		{
			_effect.Play("eff_TankTracks1");
			_effect.Play("eff_TankTracks2");
			_effect.Play("eff_Dust1");
			_effect.Play("eff_Dust2");
		}

		/// <summary>
		/// Stops playing of the movement effects.
		/// </summary>
		public void StopMovementEffects()
		{
			_effect.Stop("eff_TankTracks1");
			_effect.Stop("eff_TankTracks2");
			_effect.Stop("eff_Dust1");
			_effect.Stop("eff_Dust2");
		}

		#endregion
		#region Private Methods

		private void UpdateMovement()
		{
			if (AntMath.Distance(_currentPoint, (Vector2) _t.position) < 0.5f)
			{
				// Arrived to the current way point.
				_pointIndex++;
				if (_pointIndex < _route.Count)
				{
					// Move to next one.
					_currentPoint = _route[_pointIndex];
				}
				else
				{
					// This is end of the way.
					float dist = AntMath.Distance(_currentPoint, (Vector2) _t.position);
					if (dist < 0.5f)
					{
						// Enable break.
						_speed = AntMath.Lerp(_speed, 0.0f, 1.0f - breakCurve.Evaluate(dist / 0.5f));
						_isBrake = true;
					}

					if (AntMath.Equal(_speed, 0.0f, 0.1f))
					{
						// Absolutely arrived.
						StopMove();
						if (EventArrived != null)
						{
							EventArrived(this);
						}
					}
				}

				_steeringTime = 0.0f;
				_isSteering = false;
			}

			float targetAngle = AntMath.Angle(AntMath.AngleDeg(_t.position, _currentPoint));
			_debugTargetAngle = targetAngle * AntMath.RADIANS;
			float angleDiff = AntMath.AngleDifference(Angle, targetAngle) * AntMath.DEGREES;

			// If our direction incorrect.
			if (!AntMath.Equal(angleDiff, 0.0f, 0.01f) && !_isSteering)
			{
				// Correct our angle to the current way point.
				_isSteering = true;
				_steeringTime = 0.0f;
				_totalSteeringTime = totalSteeringTime * (1.0f - Mathf.Abs(angleDiff / 360.0f));
			}

			// Acceleration!
			if (!_isBrake && _accelerationTime < totalAccelTime)
			{
				_accelerationTime += Time.deltaTime;
				_speed = AntMath.Lerp(_speed, moveSpeed, accelCurve.Evaluate(_accelerationTime / totalAccelTime));
			}

			// Correction of the angle.
			if (_isSteering)
			{
				_steeringTime += Time.deltaTime;
				Angle = AntMath.LerpAngleDeg(Angle, targetAngle, steeringCurve.Evaluate(_steeringTime / _totalSteeringTime));
				if (AntMath.Equal(angleDiff, 0.0f, 0.01f))
				{
					_isSteering = false;
					_steeringTime = 0.0f;
				}
			}

			// Movement.
			float ang = Angle * AntMath.RADIANS;
			Velocity = new Vector2(_speed * Mathf.Cos(ang), _speed * Mathf.Sin(ang));
		}

		private void Kill()
		{
			AntEffectEngine.GetEffect("eff_Explosion", transform.position);

			// Killing of the tank unit.
			if (EventDestroyed != null)
			{
				EventDestroyed(this);
			}

			_effect.StopAll();
			GameObject.Destroy(gameObject);
		}

		#endregion
		#region Getters / Setters

		/// <summary>
		/// Tank velocity.
		/// </summary>
		public Vector2 Velocity
		{
			get => _body.velocity;
			set => _body.velocity = value;
		}
		
		/// <summary>
		/// Tank position.
		/// </summary>
		public Vector3 Position
		{
			get => _t.position;
			set => _t.position = value;
		}

		/// <summary>
		/// Tank angle.
		/// </summary>
		public float Angle
		{
			get => _body.rotation;
			set => _body.rotation = value;
		}

		/// <summary>
		/// Reference to the Tower control script.
		/// </summary>
		public TankTower Tower { get => towerRef; }

		/// <summary>
		/// Tank health.
		/// </summary>
		public float Health
		{
			get => _health;
			set
			{
				// Update health value.
				_health = value;
				_health = (_health > maxHealth)
					? maxHealth
					: _health;

				if (EventChangeHealth != null)
				{
					// Health is changed!
					EventChangeHealth(this);
				}

				if (!_isSmoke && _health / maxHealth < 0.45f)
				{
					// Health is lower than 45%, enable the smoke effect.
					_effect.Play("eff_Smoke");
					_isSmoke = true;
				}
				else if (_isSmoke && _health / maxHealth > 0.5f)
				{
					// Health is upper than 50%, disable the smoke effect.
					_effect.Stop("eff_Smoke");
					_isSmoke = false;
				}

				// Health is out ;(
				if (_health <= 0.0f)
				{
					Kill();
				}
			}
		}

		#endregion
	}
}