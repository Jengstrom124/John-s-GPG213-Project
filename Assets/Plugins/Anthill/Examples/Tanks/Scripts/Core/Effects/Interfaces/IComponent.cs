namespace Anthill.Effects
{
	/// <summary>
	/// Интерфейс определяющий базовые возможности любого компонента.
	/// </summary>
	public interface IComponent
	{
		void Initialize(IParticle aParticle);
		void Reset(AntEmitter aEmitter);
		void Update(float aDeltaTime, float aTimeScale);
		void Destroy();

		AntEmitterPreset Preset { get; set; }
		bool IsActive { get; }
		bool IsExists { get; }
	}
}