namespace Anthill.Effects
{
	using UnityEngine;
	using Anthill.Utils;

	/// <summary>
	/// Данный компонент определяет появление частиц в заданной квадратной области.
	/// </summary>
	public class PositionComponent : IComponent
	{
		#region Variables

		private IParticle _particle;
		private Transform _transform;

		#endregion
		#region ICompoment Implementation

		public void Initialize(IParticle aParticle)
		{
			_particle = aParticle;
			_transform = aParticle.GameObject.GetComponent<Transform>();
		}

		public void Reset(AntEmitter aEmitter)
		{
			var center = _transform.position;
			var spawn = Preset.position;
			center.x = spawn.position.x + AntMath.RandomRangeFloat(spawn.rndToPositionX);
			center.y = spawn.position.y + AntMath.RandomRangeFloat(spawn.rndToPositionY);

			float lowerAngle = spawn.initialAngle + spawn.lowerAngle;
			float upperAngle = spawn.initialAngle + spawn.upperAngle;
			if (spawn.inheritRotation)
			{
				center = AntMath.RotatePointDeg(center, Vector2.zero, _particle.Effect.Angle);
				lowerAngle += _particle.Effect.Angle;
				upperAngle += _particle.Effect.Angle;
			}

			center.x += _particle.Effect.Position.x;
			center.y += _particle.Effect.Position.y;

			var pos = Vector2.zero;
			float curAngle;
			float dist = spawn.distance + AntMath.RandomRangeFloat(spawn.rndToDistance);
			if (spawn.strongOrder && spawn.countParticles > 0)
			{
				aEmitter.SpawnCount++;
				float step = Mathf.DeltaAngle(lowerAngle, upperAngle) / (spawn.countParticles - 1);
				curAngle = (lowerAngle + (step * aEmitter.SpawnCount)) * AntMath.RADIANS;
				pos.x = center.x + dist * Mathf.Cos(curAngle);
				pos.y = center.y + dist * Mathf.Sin(curAngle);

				if (aEmitter.SpawnCount >= spawn.countParticles)
				{
					aEmitter.SpawnCount = 0;
				}
			}
			else
			{
				curAngle = AntMath.RandomRangeFloat(lowerAngle, upperAngle) * AntMath.RADIANS;
				pos.x = center.x + dist * Mathf.Cos(curAngle);
				pos.y = center.y + dist * Mathf.Sin(curAngle);
			}

			if (spawn.rotate)
			{
				Angle = curAngle * AntMath.DEGREES;
			}
			_transform.position = pos;

			IsActive = false; // Выкл. обработку компонента.
		}

		public void Update(float aDeltaTime, float aTimeScale)
		{
			// ...
		}

		public void Destroy()
		{
			_transform = null;
			_particle = null;
		}

		public AntEmitterPreset Preset { get; set; }
		public bool IsActive { get; private set; }
		public bool IsExists { get { return Preset.position.isExists; } }

		#endregion
		#region Private Methods

		private float Angle
		{
			get { return _transform.rotation.eulerAngles.z; }
			set { _transform.rotation = Quaternion.AngleAxis(value, Vector3.forward); }
		}

		#endregion
	}
}