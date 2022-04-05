namespace Anthill.Animation
{
	using UnityEngine;
	using Anthill.Utils;

	/// <summary>
	/// Simple implementation of the sprite animation.
	/// </summary>
	[RequireComponent(typeof(SpriteRenderer))]
	[AddComponentMenu("Anthill/AntActor")]
	public class AntActor : MonoBehaviour
	{
		[HideInInspector]
		public string initialAnimation = string.Empty;

		[HideInInspector]
		public AntAnimation[] animations = new AntAnimation[0];

		[Tooltip("Speed of the animation.")]
		public float timeScale = 1.0f;

		[Tooltip("Plays the animation in the opposite direction.")]
		public bool reverse;

		[Tooltip("Starts playing animation from random frame.")]
		public bool playRandomFrame;

		[Tooltip("Animation is looped.")]
		public bool loop = true;

		[Tooltip("Delay between loops.")]
		public float loopDelay;

		public delegate void AnimationCompleteDelegate(AntActor aActor, string aAnimationName);
		public event AnimationCompleteDelegate EventAnimationComplete;

		private SpriteRenderer _sprite;
		private AntAnimation _currentAnimation;

		private const float ANIMATION_SPEED = 29.0f;

		private bool _isAnimInitialized;
		private float _currentFrame;
		private bool _isPlaying;
		private bool _isPaused;
		private int _prevFrame;
		private int _complete;
		private float _delay;

	#region Unity Calls

		protected virtual void Awake()
		{
			_sprite = GetComponent<SpriteRenderer>();
			_currentFrame = 1.0f;
			_isPlaying = true;
			_isPaused = false;
			_prevFrame = -1;
			_delay = 0.0f;

			if (initialAnimation != null)
			{
				SwitchAnimation(initialAnimation);
				if (playRandomFrame)
				{
					PlayRandomFrame();
				}
			}
		}

		protected virtual void Update()
		{
			if (_isPlaying)
			{
				if (_complete == 2)
				{
					_complete = 0;
				}
				else if (_complete == 1)
				{
					_complete = 2;
				}

				if (reverse)
				{
					PrevFrame();
					if (loop && _currentFrame < 0.0f)
					{
						AnimationComplete();
						_currentFrame = (float)(TotalFrames - 1);
						SetFrame(_currentFrame);
					}
				}
				else
				{
					NextFrame();
					if (loop && _currentFrame > (float)TotalFrames - 1)
					{
						AnimationComplete();
						_currentFrame = 0.0f;
						SetFrame(_currentFrame);
					}
				}
			}
			else if (_isPaused)
			{
				_delay -= Time.deltaTime;
				if (_delay <= 0.0f)
				{
					_delay = 0.0f;
					Play();
				}
			}
		}

	#endregion

	#region Public Methods

		/// <summary>
		/// Change the current animation.
		/// </summary>
		/// <param name="aAnimationKey">Animation name.</param>
		public void SwitchAnimation(string aAnimationKey)
		{
			if (_currentAnimation == null || !string.Equals(_currentAnimation.key, aAnimationKey))
			{
				int index = System.Array.FindIndex(animations, x => x.key.Equals(aAnimationKey));
				if (index >= 0 && index < animations.Length)
				{
					_isAnimInitialized = true;
					_currentAnimation = animations[index];
					_currentFrame = 1.0f;
					_prevFrame = -1;
				}
				else
				{
					A.Warning($"Animation `{aAnimationKey}` not found for `{name}` actor.");
				}
			}
		}

		/// <summary>
		/// Starts playing of the current animation.
		/// </summary>
		public void Play()
		{
			_isPlaying = true;
			_delay = 0.0f;
		}

		/// <summary>
		/// Stops playing of the current animation.
		/// </summary>
		public void Stop()
		{
			_isPlaying = false;
		}

		/// <summary>
		/// Changes current frame and stops playing current animation.
		/// </summary>
		/// <param name="aFrame">Frame index.</param>
		public void GotoAndStop(int aFrame)
		{
			_currentFrame = (float)aFrame - 1.0f;
			_currentFrame = (_currentFrame < 0.0f) 
				? 0.0f 
				: (_currentFrame >= TotalFrames) 
					? (float)(TotalFrames - 1) 
					: _currentFrame;
			SetFrame(_currentFrame);
			Stop();
		}

		/// <summary>
		/// Starts playing current animation from specified frame.
		/// </summary>
		/// <param name="aFrame">Frame index.</param>
		public void GotoAndPlay(int aFrame)
		{
			_currentFrame = (float)aFrame - 1.0f;
			_currentFrame = (_currentFrame < 0.0f) 
				? 0.0f 
				: (_currentFrame >= TotalFrames) 
					? (float)(TotalFrames - 1) 
					: _currentFrame;
			SetFrame(_currentFrame);
			Play();
		}

		/// <summary>
		/// Starts playing current animation from the random frame.
		/// </summary>
		public void PlayRandomFrame()
		{
			if (_isAnimInitialized)
			{
				GotoAndPlay(AntMath.RandomRangeInt(1, TotalFrames));
			}
		}

		/// <summary>
		/// Moves to the next frame.
		/// </summary>
		public void NextFrame()
		{
			_currentFrame += (ANIMATION_SPEED * Time.deltaTime * Time.timeScale) * timeScale;
			SetFrame(_currentFrame);
		}

		/// <summary>
		/// Moves to the previous frame.
		/// </summary>
		public void PrevFrame()
		{
			_currentFrame -= (ANIMATION_SPEED * Time.deltaTime * Time.timeScale) * timeScale;
			SetFrame(_currentFrame);
		}

	#endregion

	#region Private Methods

		private void AnimationComplete()
		{
			if (loop && loopDelay > 0.0f)
			{
				_delay = loopDelay;
				_isPlaying = false;
				_isPaused = true;
			}

			_complete = 1;
			if (EventAnimationComplete != null)
			{
				EventAnimationComplete(this, _currentAnimation.key);
			}
		}

		private void SetFrame(float aFrame)
		{
			if (_isAnimInitialized && _currentAnimation.frames != null)
			{
				int i = RoundFrame(aFrame);
				if (_prevFrame != i)
				{
					_sprite.sprite = _currentAnimation.frames[i];
					_prevFrame = i;
				}
			}
		}

		private int RoundFrame(float aFrame)
		{
			int i = Mathf.RoundToInt(aFrame);
			return (i <= 0) 
				? 0 
				: (i >= TotalFrames - 1) 
					? TotalFrames - 1 
					: i;
		}

	#endregion

	#region Getters / Setters

		/// <summary>
		/// Returns true if current animation is playing.
		/// </summary>
		public bool IsPlaying { get => _isPlaying; }

		/// <summary>
		/// Returns true if current animation just stopped.
		/// </summary>
		public bool JustFinished { get => (_complete == 1); }

		/// <summary>
		/// Returns current animation name.
		/// </summary>
		public string AnimationName
		{
			get => (_isAnimInitialized)
				? _currentAnimation.key
				: "<Undefined>";
			set => SwitchAnimation(value);
		}

		/// <summary>
		/// Returns count of the frames for current animation.
		/// </summary>
		public int TotalFrames
		{
			get => (_isAnimInitialized && _currentAnimation.frames != null) 
				? _currentAnimation.frames.Length 
				: 0;
		}

		/// <summary>
		/// Returns current frame of current animation.
		/// </summary>
		public int CurrentFrame { get => RoundFrame(_currentFrame); }

	#endregion
	}
}