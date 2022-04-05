namespace Anthill.Effects
{
	public interface IEmission
	{
		void Reset(AntEmitter aEmitter);
		void Update(float aDeltaTime, float aTimeScale);
		bool NewParticleIsReady();
		bool IsActive { get; }
		int Count { get; }
	}
}