namespace Tanks
{
	/// <summary>
	/// There is we want to get ammo.
	/// </summary>
	public class PickupAmmoState : AbstractPickup
	{
		public override void Enter()
		{
			Pickup(ItemKind.Ammo);
		}
	}
}