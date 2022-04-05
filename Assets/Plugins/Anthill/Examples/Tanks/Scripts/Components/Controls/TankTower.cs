namespace Tanks
{
	using UnityEngine;
	using Anthill.Animation;
	using Anthill.Effects;

	/// <summary>
	/// Controller for working with tank tower.
	/// </summary>
	public class TankTower : MonoBehaviour
	{
		#region Variables

		private bool _isBomb;
		private bool _isGun;
		private int _ammoCount;
		private AntActor _actor;
		private AntEffectSpawner _effect;
		private Transform _t;

		#endregion
		#region Unity Calls

		private void Awake()
		{
			_t = GetComponent<Transform>();
			_actor = GetComponent<AntActor>();
			_effect = GetComponent<AntEffectSpawner>();
		}

		#endregion
		#region Public Methods

		/// <summary>
		/// Gives to tank some ammo, for example, when Tank take ammo pack on the map.
		/// </summary>
		/// <param name="aAmount">Count of ammo to give.</param>
		public void GiveAmmo(int aAmount)
		{
			_ammoCount += aAmount;
			if (_ammoCount > 3)
			{
				_ammoCount = 3;
			}

			UpdateVisual();
		}

		/// <summary>
		/// Takes from tank some ammo, for example, when Tank is shot to the enemy.
		/// </summary>
		/// <param name="aAmount">Count of ammo to take.</param>
		public void TakeAmmo(int aAmount)
		{
			_ammoCount -= aAmount;
			if (_ammoCount < 0)
			{
				_ammoCount = 0;
			}

			UpdateVisual();
		}

		public void ShotEffect()
		{
			_effect.Play("eff_Shot");
		}

		#endregion
		#region Private Methods
		
		private void UpdateVisual()
		{
			if (_isGun)
			{
				_actor.SwitchAnimation("GunTower");
				_actor.GotoAndStop(_ammoCount + 1);
			}
			else if (_isBomb)
			{
				_actor.SwitchAnimation("BombTower");
				_actor.GotoAndStop(1);
			}
			else
			{
				_actor.SwitchAnimation("NoTower");
				_actor.GotoAndStop(1);
			}
		}
		
		#endregion
		#region Getters / Setters
		
		/// <summary>
		/// Returns true if Tank has a GUN.
		/// </summary>
		public bool IsHasGun
		{
			get => _isGun;
			set
			{
				_isGun = value;
				if (_isGun)
				{
					_isBomb = false;
				}
				UpdateVisual();
			}
		}

		/// <summary>
		/// Returns true if Tank has a BOMB.
		/// </summary>
		public bool IsHasBomb
		{
			get => _isBomb;
			set
			{
				_isBomb = value;
				if (_isBomb)
				{
					_isGun = false;
				}
				UpdateVisual();
			}
		}

		/// <summary>
		/// Returns true if Tank has AMMO for GUN.
		/// </summary>
		public bool IsHasAmmo { get => (_ammoCount > 0); }

		/// <summary>
		/// Returns Tower angle in degrees.
		/// </summary>
		public float Angle { get => _t.rotation.eulerAngles.z; }

		/// <summary>
		/// Returns Tower rotation.
		/// </summary>
		public Quaternion Rotation
		{
			get => _t.rotation;
			set => _t.rotation = value;
		}

		#endregion
	}
}