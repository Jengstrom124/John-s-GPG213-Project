namespace Tanks
{
	/// <summary>
	/// This is patrol state.
	/// </summary>
	public class ScoutState : AbstractMove
	{
		public override void Enter()
		{
			// Moving to random point on the map.
			MoveTo(GetRandomPoint());
		}
	}
}