namespace Anthill.Effects
{
	using System;
	using UnityEngine;
	using Anthill.Pool;
	
	public class AntEffectEngine
	{
		private static Type[] _components = 
		{
			typeof(BurstComponent),
			typeof(EmissionComponent),
			typeof(SourceComponent),
			typeof(PositionComponent),
			typeof(LifeTimeComponent),
			typeof(ActorComponent),
			typeof(ColourComponent),
			typeof(ScaleComponent),
			typeof(RotationComponent),
			typeof(MovementComponent),
			typeof(TrailComponent)
		};

		public static Type[] AvailableComponents
		{
			get { return _components; }
		}

		public static AntEffect GetEffect(string aEffectName, Vector3 aPosition, float aAngle = 0.0f)
		{
			AntEffect emitter = AntPoolManager.GetObject<AntEffect>(aEffectName);
			if (emitter != null)
			{
				emitter.Position = aPosition;
				emitter.Angle = aAngle;
			}
			return emitter;
		}
	}
}