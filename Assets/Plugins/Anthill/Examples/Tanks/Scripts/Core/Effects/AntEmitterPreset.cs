namespace Anthill.Effects
{
	using UnityEngine;

	[CreateAssetMenuAttribute(fileName = "EmitterPreset", menuName = "Anthill/Emitter Preset", order = 2)]
	public class AntEmitterPreset : ScriptableObject
	{
		[HideInInspector] public Burst burst;
		[HideInInspector] public Emission emission;
		[HideInInspector] public Source source;
		[HideInInspector] public LifeTime lifeTime;
		[HideInInspector] public Position position;
		[HideInInspector] public Actor actor;
		[HideInInspector] public Colour colour;
		[HideInInspector] public Scale scale;
		[HideInInspector] public Rotation rotation;
		[HideInInspector] public Movement movement;
		[HideInInspector] public PhysicSettings physicSettings;
		[HideInInspector] public PhysicImpulse physicImpulse;
		[HideInInspector] public PhysicExplosion physicExplosion;
		[HideInInspector] public SFLighting sfLighting;
		[HideInInspector] public Trail trail;

		public AntEmitterPreset()
		{
			// Brust defaults. //
			burst.list = new BurstItem[0];

			// Emission defaults.
			emission.isExists = true;
			emission.spawnInterval = 1.0f;
			emission.numParticles = 1;

			// Source defaults.
			source.isExists = true;
			source.prefabs = new GameObject[0];

			// LifeTime defaults.
			lifeTime.isExists = true;
			lifeTime.duration = 1.0f;

			// Spawn Area defaults.
			position.isExists = true;

			// Actor defaults.
			actor.startFrame = 1;
			actor.timeScale = 1.0f;

			// Colour defaults.
			colour.startColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			colour.curveColor = new AnimationCurve();
			colour.gradient = new Gradient();

			// Scale defaults.
			scale.isExists = true;
			scale.startScale = Vector2.one;
			scale.scaleCurveX = new AnimationCurve();
			scale.scaleCurveY = new AnimationCurve();

			// Rotation defaults.
			rotation.drag = 1.0f;

			// Movement defaults.
			movement.drag = 1.0f;

			// Physic Settings defaults.
			physicSettings.autoMass = true;
			physicSettings.mass = 1.0f;
			physicSettings.angularDrag = 0.05f;
			physicSettings.gravityScale = 1.0f;

			// Physic Explosion defaults.
			physicExplosion.radius = 0.5f;
			physicExplosion.force = 20.0f;
			physicExplosion.numRays = 12;
			physicExplosion.mask = (LayerMask) 1;
			physicExplosion.startDecay = 0.0f;
			physicExplosion.pixelsPerUnit = 100.0f;
			physicExplosion.minDistance = 0.01f;

			// SF Lighting defaults.
			sfLighting.startRadius = 0.5f;
			sfLighting.startIntensity = 0.5f;
			sfLighting.startColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

			// Trail defaults.
			trail.startTime = 0.5f;
			trail.endTime = 0.5f;
			trail.startWidth = 1.0f;
			trail.timeCurve = new AnimationCurve();
			trail.widthCurve = new AnimationCurve();
			trail.gradient = new Gradient();
		}
	}

	// :: Brust Component ::

	[System.Serializable]
	public struct Burst
	{
		public bool isOpen;
		public bool isExists;
		public BurstItem[] list;
	}

	[System.Serializable]
	public struct BurstItem
	{
		public float time;
		public int min;
		public int max;
	}

	// :: Emission Component ::

	[System.Serializable]
	public struct Emission
	{
		public bool isOpen;
		public bool isExists;

		public float spawnInterval;
		public Vector2 rndToSpawnInterval;
		public int numParticles;
		public Vector2 rndToNumParticles;
	}

	// :: Source Component ::

	[System.Serializable]
	public struct Source
	{
		public bool isOpen;
		public bool isExists;
		public GameObject[] prefabs;
		public bool selectRandom;
	}

	// :: Life Time Component ::

	[System.Serializable]
	public struct LifeTime
	{
		public bool isOpen;
		public bool isExists;
		public float duration;
		public Vector2 rndToDuration;
	}

	// :: Spawn Position Component ::

	[System.Serializable]
	public struct Position
	{
		public bool isOpen;
		public bool isExists;

		public Vector2 position;
		public Vector2 rndToPositionX;
		public Vector2 rndToPositionY;

		public bool rotate;
		public bool inheritRotation;

		public float distance;
		public Vector2 rndToDistance;

		public float initialAngle;
		public float lowerAngle;
		public float upperAngle;

		public bool strongOrder;
		public int countParticles;
	}

	// :: Actor Component ::

	[System.Serializable]
	public struct Actor
	{
		public bool isOpen;
		public bool isExists;

		public int startFrame;
		public bool selectRandomFrame;
		public bool useChildActor;
		
		public bool playAnimation;
		public bool reverse;
		public float timeScale;
		public Vector2 rndToTimeScale;

		public bool loop;
		public float loopDelay;
		public Vector2 rndToLoopDelay;

		public bool flipX;
		public bool flipY;
		public bool rndFlipX;
		public bool rndFlipY;

		public bool sorting;
		public ESortingMode sortingMode;
		public string sortingLayer;
		public int sortingOrder;
	}

	// :: Colour Component ::

	[System.Serializable]
	public struct Colour
	{
		public bool isOpen;
		public bool isExists;
		public Color startColor;

		public bool useChildSprite;
		
		public bool animateColor;
		public bool effectLifeTime;
		public Color endColor;
		public AnimationCurve curveColor;

		public bool gradientColor;
		public Gradient gradient;
	}

	// :: Scale Component ::

	[System.Serializable]
	public struct Scale
	{
		public bool isOpen;
		public bool isExists;

		public Vector2 startScale;
		public Vector2 rndToScaleX;
		public Vector2 rndToScaleY;

		public bool proportional;
		public bool useChildSprite;

		public bool animateScale;
		public bool effectLifeTime;
		public Vector2 endScale;
		public AnimationCurve scaleCurveX;
		public AnimationCurve scaleCurveY;
	}

	// :: Rotation Component ::

	[System.Serializable]
	public struct Rotation
	{
		public bool isOpen;
		public bool isExists;
		
		public float startAngle;
		public Vector2 rndToAngle;

		public bool enableRotation;
		public float angularSpeed;
		public Vector2 rndToAngularSpeed;
		public float accel;
		public float drag;

		public bool animateAngle;
		public float endAngle;
		public AnimationCurve curveAngle;

		public bool inheritRotation;
	}

	// :: Movement Component ::

	[System.Serializable]
	public struct Movement
	{
		public bool isOpen;
		public bool isExists;

		public float speed;
		public Vector2 rndToSpeed;
		public float accel;
		public float drag;

		public bool animateSpeed;
		public float endSpeed;
		public AnimationCurve speedCurve;

		public bool gravity;
		public Vector2 gravityFactor;
		public bool rotate;
	}

	// :: Physic Settings Component ::

	[System.Serializable]
	public struct PhysicSettings
	{
		public bool isOpen;
		public bool isExists;

		public bool customMass;
		public bool autoMass;
		public float mass;
		public Vector2 rndToMass;

		public bool customLinearDrag;
		public float linearDrag;
		public Vector2 rndToLinearDrag;

		public bool customAngularDrag;
		public float angularDrag;
		public Vector2 rndToAngularDrag;

		public bool customGravityScale;
		public float gravityScale;
		public Vector2 rndToGravityScale;

		public bool freezeRotation;

		public bool lifeTimeLoss;
		public float amountTimeLosing;
	}

	// :: Physic Impulse Component ::

	[System.Serializable]
	public struct PhysicImpulse
	{
		public bool isOpen;
		public bool isExists;

		public float force;
		public Vector2 rndToForce;

		public float startAngle;
		public float rndToAngleLower;
		public float rndToAngleUpper;
		public bool useParticleAngle;
		public bool inheritRotation;

		public bool rotate;
		public bool rotateChild;

		public bool applyToPosition;
		public Vector2 position;
		public Vector2 rndToPositionX;
		public Vector2 rndToPositionY;

		public bool addTorque;
		public float torqueForce;
		public Vector2 rndToTorqueForce;
	}

	// :: Physic Explosion Component ::

	[System.Serializable]
	public struct PhysicExplosion
	{
		public bool isOpen;
		public bool isExists;

		public float radius;
		public float force;
		public int numRays;

		public LayerMask mask;

		public float startDecay;
		public float pixelsPerUnit;
		public float minDistance;
	}

	// :: SFLightning Component ::

	[System.Serializable]
	public struct SFLighting
	{
		public bool isOpen;
		public bool isExists;

		public float startRadius;
		public Vector2 rndToStartRadius;

		public bool animateRadius;
		public float endRadius;
		public AnimationCurve radiusCurve;

		public float startIntensity;
		public Vector2 rndToStartIntensity;

		public bool animateIntensity;
		public float endIntensity;
		public AnimationCurve intensityCurve;

		public Color startColor;

		public bool animateColor;
		public Gradient gradient;
	}

	// :: Trail Component ::
	
	[System.Serializable]
	public struct Trail
	{
		public bool isOpen;
		public bool isExists;

		public float startTime;
		public Vector2 rndToStartTime;

		public bool animateTime;
		public float endTime;
		public AnimationCurve timeCurve;

		public float startWidth;
		public Vector2 rndToStartWidth;
		public AnimationCurve widthCurve;

		public Gradient gradient;
	}
}