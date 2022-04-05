namespace Anthill.Effects
{
	using UnityEngine;
	using Anthill.Animation;
	using Anthill.Utils;

	/// <summary>
	/// Данный компонент применяет настройки к AntActor.
	/// </summary>
	public class ActorComponent : IComponent
	{
		#region Variables

		private IParticle _particle;
		private AntActor _actor;
		private SpriteRenderer _sprite;

		#endregion
		#region IComponent Implementation

		public void Initialize(IParticle aParticle)
		{
			_particle = aParticle;
		}

		public void Reset(AntEmitter aEmitter)
		{
			_sprite = (Preset.actor.useChildActor)
				? _particle.GameObject.GetComponentInChildren<SpriteRenderer>()
				: _particle.GameObject.GetComponent<SpriteRenderer>();

			_actor = (Preset.actor.useChildActor)
				? _particle.GameObject.GetComponentInChildren<AntActor>()
				: _particle.GameObject.GetComponent<AntActor>();

			_actor.timeScale = Preset.actor.timeScale + AntMath.RandomRangeFloat(Preset.actor.rndToTimeScale);
			_sprite.flipX = (Preset.actor.rndFlipX)
				? (AntMath.RandomRangeInt(0, 1) == 1)
				: Preset.actor.flipX;

			_sprite.flipY = (Preset.actor.rndFlipY)
				? (AntMath.RandomRangeInt(0, 1) == 1)
				: Preset.actor.flipY;

			_actor.reverse = Preset.actor.reverse;
			_actor.loop = Preset.actor.loop;
			_actor.loopDelay = Preset.actor.loopDelay + AntMath.RandomRangeFloat(Preset.actor.rndToLoopDelay);

			if (Preset.actor.playAnimation)
			{
				if (Preset.actor.selectRandomFrame)
				{
					_actor.PlayRandomFrame();
				}
				else
				{
					_actor.GotoAndPlay(Preset.actor.startFrame);
				}
			}
			else
			{
				if (Preset.actor.selectRandomFrame)
				{
					_actor.GotoAndStop(AntMath.RandomRangeInt(1, _actor.TotalFrames));
				}
				else
				{
					_actor.GotoAndStop(Preset.actor.startFrame);
				}
			}

			IsActive = false; // Выкл. обработку компонента.
		}

		public void Update(float aDeltaTime, float aTimeScale)
		{
			// ..
		}

		public void Destroy()
		{
			_actor = null;
			_sprite = null;
		}

		public AntEmitterPreset Preset { get; set; }
		public bool IsActive { get; private set; }
		public bool IsExists
		{
			get { return Preset.actor.isExists; }
		}

		#endregion
	}
}