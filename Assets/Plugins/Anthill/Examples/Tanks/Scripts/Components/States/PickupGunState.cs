namespace Tanks
{
	/// <summary>
	/// There is we pickup the gun.
	/// </summary>
	public class PickupGunState : AbstractPickup
	{
		public override void Enter()
		{
			Pickup(ItemKind.Gun);
		}
	}
}