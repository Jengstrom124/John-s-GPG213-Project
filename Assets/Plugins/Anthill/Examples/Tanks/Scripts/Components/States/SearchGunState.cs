namespace Tanks
{
	/// <summary>
	/// In this state we trying to find gun.
	/// </summary>
	public class SearchGunState : AbstractMove
	{
		public override void Enter()
		{
			SearchItem(ItemKind.Gun);
		}
	}
}