namespace Anthill.Effects
{
	using UnityEngine;
	using System.Collections.Generic;

	using Anthill.Utils;
	using Anthill.Pool;

	public class AntEffect : AntPoolObject
	{
		#region Variables

		private enum State
		{
			Idle,
			Playing,
			Waiting,
			Completed
		}

		// :: Initial Settings ::

		[HideInInspector] public float startDelay;
		[HideInInspector] public float lifeTime;
		[HideInInspector] public bool isLooping = true;
		[HideInInspector] public bool isAutoPlay = true;
		[HideInInspector] public bool isAutoReturnToPool;
		[HideInInspector] public bool isAutoRepeat;

		[HideInInspector]
		public AntEmitterPreset[] emitters = new AntEmitterPreset[0];

		// :: Events ::

		public delegate void EventEmitterDelegate(AntEffect aEffect);
		public event EventEmitterDelegate EventPlay;
		public event EventEmitterDelegate EventStop;
		public event EventEmitterDelegate EventComplete;

		// :: Private variables ::

		private List<IParticle> _particles = new List<IParticle>();
		private AntSorter<IParticle> _sorter = new AntSorter<IParticle>();

		private float _duration;
		private float _startDelay;
		private State _state;

		private AntEmitter[] _emitters;

		#endregion
		#region Unity Calls

		protected override void Initialize()
		{
			// Инициализация эммитеров.
			_emitters = new AntEmitter[emitters.Length];
			for (int i = 0, n = _emitters.Length; i < n; i++)
			{
				A.Assert(emitters[i] == null, "AntEmitter `{0}` has missed preset.", gameObject.name);
				_emitters[i] = new AntEmitter(emitters[i]);
			}

			if (isAutoReturnToPool)
			{
				EventComplete += PlaceToPoolHandler;
			}

			EventExtractedFromPool += ExtractedFromPoolHandler;
		}

		#endregion
		#region Unity Calls

		private void Update()
		{
			float dt = Time.deltaTime;
			float ts = Time.timeScale;
			switch (_state)
			{
				case State.Playing :
					_duration += dt;
					UpdateEffects(dt, ts);
					if (!isLooping && _duration >= lifeTime)
					{
						Complete();
					}
					break;

				case State.Waiting :
					_startDelay -= dt;
					if (_startDelay <= 0.0f)
					{
						_state = State.Playing;
						_startDelay = 0.0f;
					}
					break;

				case State.Completed :
					if (isAutoRepeat)
					{
						Play();
					}
					break;
			}
		}

		#endregion
		#region Public Functions
		
		public void Reset()
		{
			_duration = 0.0f;
			_state = State.Idle;
			_startDelay = startDelay;

			for (int i = 0, n = _emitters.Length; i < n; i++)
			{
				_emitters[i].Reset();
			}
		}

		public void Play()
		{
			if (_state != State.Playing)
			{
				Reset();
				_state = (_startDelay > 0.0f) 
					? State.Waiting 
					: State.Playing; 
				if (EventPlay != null)
				{
					EventPlay(this);
				}
			}
		}

		public void Stop()
		{
			if (_state == State.Playing ||
				_state == State.Waiting)
			{
				_state = State.Idle;
				if (EventStop != null)
				{
					EventStop(this);
				}
			}
		}

		public void Complete()
		{
			if (_state == State.Playing)
			{
				_state = State.Completed;
				if (EventComplete != null)
				{
					EventComplete(this);
				}
			}
		}
		
		#endregion
		#region Private Methods

		private void UpdateEffects(float aDeltaTime, float aTimeScale)
		{
			bool isFinished = false;
			AntEmitter emitter;
			for (int i = 0, n = _emitters.Length; i < n; i++)
			{
				emitter = _emitters[i];
				if (emitter.IsActive)
				{
					isFinished = false;
					emitter.Update(aDeltaTime, aTimeScale);
					if (emitter.EmissionIsReady)
					{
						Emission(emitter);
					}
				}
			}

			if (!isLooping && isFinished)
			{
				Complete();
			}
		}

		private void Emission(AntEmitter aEmitter)
		{
			IParticle particle;
			for (int i = 0, n = aEmitter.emissionList.Count; i < n; i++)
			{
				particle = aEmitter.emissionList[i].GetObject<IParticle>();
				particle.Activate(this, aEmitter);
				particle.EffectID = aEmitter.ID;
				particle.BirthTime = _duration;

				if (aEmitter.IsAllowSorting)
				{
					AddParticle(particle);
				}
			}

			aEmitter.emissionList.Clear();
			UpdateSorting(aEmitter);
		}

		private void UpdateSorting(AntEmitter aEmitter)
		{
			if (!aEmitter.IsAllowSorting) { return; }

			IParticle particle;
			var actor = aEmitter.Actor.Preset.actor;
			switch (actor.sortingMode)
			{
				case ESortingMode.None :
					for (int i = 0, n = _particles.Count; i < n; i++)
					{
						particle = _particles[i];
						if (particle != null)
						{
							if (particle.IsActive)
							{
								if (particle != null && particle.EffectID == aEmitter.ID)
								{
									particle.SortingLayer = actor.sortingLayer;
									particle.SortingOrder = actor.sortingOrder;
								}
							}
							else
							{
								_particles[i] = null;
							}
						}
					}
					break;

				case ESortingMode.OldestInFront :
				case ESortingMode.YoungestInFront :
					_sorter.Clear();
					for (int i = 0, n = _particles.Count; i < n; i++)
					{
						particle = _particles[i];
						if (particle != null)
						{
							if (particle.IsActive)
							{
								if (particle.EffectID == aEmitter.ID)
								{
									_sorter.Add(particle, particle.BirthTime);
								}
							}
							else
							{
								_particles[i] = null;
							}
						}
					}

					_sorter.Sort((actor.sortingMode == ESortingMode.OldestInFront) 
						? AntSorterOrder.DESC 
						: AntSorterOrder.ASC
					);

					for (int i = 0, n = _sorter.Count; i < n; i++)
					{
						_sorter[i].SortingLayer = actor.sortingLayer;
						_sorter[i].SortingOrder = actor.sortingOrder + i;
					}
					break;
			}
		}

		private void AddParticle(IParticle aParticle)
		{
			for (int i = 0, n = _particles.Count; i < n; i++)
			{
				if (_particles[i] == null)
				{
					_particles[i] = aParticle;
					return;
				}
			}

			_particles.Add(aParticle);
		}
		
		#endregion
		#region Event Handlers

		private void PlaceToPoolHandler(AntEffect aEffect)
		{
			ReturnToPool();
		}

		private void ExtractedFromPoolHandler()
		{
			if (isAutoPlay)
			{
				Play();
			}
		}

		#endregion
		#region Getters / Setters

		public float TotalDuration
		{
			get { return lifeTime; }
		}

		public float Duration
		{
			get { return _duration; }
		}

		public bool IsPlaying
		{
			get { return (_state != State.Idle && _state != State.Completed); }
		}

		public bool IsCompleted
		{
			get { return (_state == State.Completed); }
		}

		public float Angle
		{
			get { return _transform.rotation.eulerAngles.z; }
			set { _transform.rotation = Quaternion.AngleAxis(value, Vector3.forward); }
		}

		#endregion

	}
}