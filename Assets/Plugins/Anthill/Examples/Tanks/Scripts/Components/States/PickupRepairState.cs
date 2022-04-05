namespace Tanks
{
	/// <summary>
	/// There is we want to get repair.
	/// </summary>
	public class PickupRepairState : AbstractPickup
	{
		public override void Enter()
		{
			Pickup(ItemKind.Repair);
		}
	}
}