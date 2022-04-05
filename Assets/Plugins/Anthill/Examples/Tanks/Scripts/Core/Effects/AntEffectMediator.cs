namespace Anthill.Effects
{
	using UnityEngine;
	using Anthill.Utils;

	public class AntEffectMediator
	{
		private AntEffectSpawner.Settings _settings;
		private Transform _transform;

		private AntEffect _effect;
		private Vector2 _positionOffset;
		private Vector2 _zeroPivot;
		private Vector2 _offset;
		private float _angle;
		private readonly string _name;
		private bool _isPlaying;

		private ContactPoint2D[] _contacts;

		public AntEffectMediator(string aName, Transform aOwnerTransform, 
			AntEffectSpawner.Settings aSettings)
		{
			_name = aName;
			_settings = aSettings;
			_transform = aOwnerTransform;
			_zeroPivot = Vector2.zero;
			_offset = Vector2.zero;
			_contacts = new ContactPoint2D[1];
		}

		public void Reset()
		{
			_isPlaying = false;
			_effect = null;
		}

		public void Play()
		{
			_isPlaying = true;
			
			var pos = new Vector3(
				_transform.position.x + _positionOffset.x,
				_transform.position.y + _positionOffset.y, 
				_transform.position.z
			);

			_effect = AntEffectEngine.GetEffect(_settings.prefab.name, pos);
			_effect.EventComplete += EffectCompleteHandler;
			_effect.Angle = (_settings.isInheritAngle)
				? _angle + _transform.rotation.eulerAngles.z
				: _angle;

			if (_settings.customDelay)
			{
				_effect.startDelay = _settings.delay + AntMath.RandomRangeFloat(_settings.rndToDelay);
			}

			if (_settings.customLifeTime)
			{
				_effect.isLooping = false;
				_effect.lifeTime = _settings.lifeTime + AntMath.RandomRangeFloat(_settings.rndToLifeTime);
			}

			_effect.Reset();
			_effect.Play();
		}

		public void Stop()
		{
			if (_isPlaying)
			{
				_isPlaying = false;
				_effect.EventComplete -= EffectCompleteHandler;
				_effect.Complete();
				_effect = null;
			}
		}

		public void Process(float aDeltaTime)
		{
			if (_isPlaying)
			{
				if (_settings.isUpdatePosition)
				{
					_offset = _positionOffset;
					if (_positionOffset != Vector2.zero)
					{
						_offset = AntMath.RotatePointDeg(_positionOffset, _zeroPivot, _transform.rotation.eulerAngles.z);
					}
					
					_offset.x += _transform.position.x;
					_offset.y += _transform.position.y;
					_effect.Position = _offset;
				}

				if (_settings.isInheritAngle)
				{
					_effect.Angle = _transform.rotation.eulerAngles.z + _angle;
				}
			}
		}

		private void EffectCompleteHandler(AntEffect aEffect)
		{
			Stop();
		}

		public Vector2 Offset
		{
			get { return _positionOffset; }
			set 
			{
				_positionOffset = value;
				if (_isPlaying)
				{
					_effect.Position = new Vector2(
						_transform.position.x + _positionOffset.x,
						_transform.position.y + _positionOffset.y
					);
				}
			}
		}

		public float Angle
		{
			get { return _angle; }
			set
			{
				_angle = value;
				if (_isPlaying)
				{
					_effect.Angle = _angle;
				}
			}
		}

		public bool IsPlaying
		{
			get { return _isPlaying; }
		}

		public string Name
		{
			get { return _name; }
		}
	}
}