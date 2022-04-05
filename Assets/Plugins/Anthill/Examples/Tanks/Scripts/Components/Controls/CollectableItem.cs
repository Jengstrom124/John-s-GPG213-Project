namespace Tanks
{
	using UnityEngine;
	using Anthill.Effects;

	public enum ItemKind
	{
		Gun,
		Ammo,
		Bomb,
		Repair
	}

	/// <summary>
	/// Implementation of the collectable items.
	/// </summary>
	public class CollectableItem : MonoBehaviour
	{
		public delegate void CollectableItemDelegate(CollectableItem aItem);
		public event CollectableItemDelegate EventCollected;

		#region Variables
		
		[Tooltip("Kind of the item.")]
		public ItemKind kind;

		[Tooltip("Ref to the visual transform, required for the wave animation.")]
		public Transform visual;

		[Tooltip("Time of the wave animation.")]
		public float waveAnimationTime;

		[Tooltip("Curve of the wave animation.")]
		public AnimationCurve waveAnimationCurve = new AnimationCurve();

		private Transform _t;
		private Vector2 _force;
		private float _animationTime;
		
		#endregion
		#region Unity Calls
		
		private void Awake()
		{
			_t = GetComponent<Transform>();
		}

		private void Update()
		{
			// Implementation of the wave animation.
			_animationTime += Time.deltaTime;
			if (_animationTime >= waveAnimationTime)
			{
				_animationTime = 0.0f;
			}
			var p = visual.localPosition;
			p.y = waveAnimationCurve.Evaluate(_animationTime / waveAnimationTime);
			visual.localPosition = p;

			// Updating of the item movement.
			p = _t.position;
			p += new Vector3(_force.x * Time.deltaTime, _force.y * Time.deltaTime, 0.0f);
			_t.position = p;

			_force.x *= 0.95f;
			_force.y *= 0.95f;
		}
		
		#endregion
		#region Public Methods

		/// <summary>
		/// Adds force to the movement for item.
		/// </summary>
		/// <param name="aForce">Force.</param>
		public void AddForce(Vector2 aForce)
		{
			_force += aForce;
		}
		
		/// <summary>
		/// Call this function when item is collected.
		/// </summary>
		public void Collect()
		{
			AntEffectEngine.GetEffect("eff_Collect", transform.position);
			EventCollected?.Invoke(this);
			GameObject.Destroy(gameObject);
		}
		
		#endregion
		#region Getters / Setters
		
		/// <summary>
		/// Position of the item.
		/// </summary>
		public Vector3 Position
		{
			get => _t.position;
			set => _t.position = value;
		}
		
		#endregion
	}
}