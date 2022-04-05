namespace Tanks
{
	/// <summary>
	/// There is we want to get ammo.
	/// </summary>
	public class PickupBombState : AbstractPickup
	{
		public override void Enter()
		{
			Pickup(ItemKind.Bomb);
		}
	}
}