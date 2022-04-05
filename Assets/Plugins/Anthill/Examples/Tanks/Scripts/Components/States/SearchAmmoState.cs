namespace Tanks
{
	/// <summary>
	/// In this state we trying to find ammo.
	/// </summary>
	public class SearchAmmoState : AbstractMove
	{
		public override void Enter()
		{
			SearchItem(ItemKind.Ammo);
		}
	}
}