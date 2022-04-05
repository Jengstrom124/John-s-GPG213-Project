namespace Anthill.Effects
{
	using Anthill.Pool;

	/// <summary>
	/// Данный интерфейс определяет компоненты которые
	/// являются источниками частиц в системе эффектов.
	/// </summary>
	public interface ISource
	{
		void Reset(AntEmitter aEmitter);
		AntPool Pool { get; }
	}
}