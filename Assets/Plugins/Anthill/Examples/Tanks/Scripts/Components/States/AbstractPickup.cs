namespace Tanks
{
	using UnityEngine;
	using Anthill.AI;
	using Anthill.Utils;

	/// <summary>
	/// This is a base class for all pickup actions.
	/// </summary>
	public abstract class AbstractPickup : AntAIState
	{
		private TankControl _control;
		private TankBlackboard _blackboard;
		private CollectableItem _targetItem;
		private ItemKind _itemKind;

		public override void Create(GameObject aGameObject)
		{
			_control = aGameObject.GetComponent<TankControl>();
			_blackboard = aGameObject.GetComponent<TankBlackboard>();
		}

		public override void Execute(float aDeltaTime, float aTimeScale)
		{
			if (_targetItem != null)
			{
				float dist = AntMath.Distance(_control.Position, _targetItem.Position);
				if (dist <= _control.magnetRadius)
				{
					// Magneting our loot!
					float angle = AntMath.AngleRad(_targetItem.Position, _control.Position);
					var force = new Vector2(
						0.1f * Mathf.Cos(angle),
						0.1f * Mathf.Sin(angle)
					);

					_targetItem.AddForce(force);
					if (dist < _control.collectRadius)
					{
						// Yay! We got the item!
						_targetItem.Collect();

						switch (_itemKind)
						{
							case ItemKind.Gun :
								if (_control.Tower.IsHasBomb)
								{
									// Drop bomb on the map.
									Game.Instance.SpawnItem(
										_control.Position, 
										ItemKind.Bomb, 
										_control.transform.parent.transform
									);
								}
								_control.Tower.IsHasGun = true;
								break;

							case ItemKind.Ammo :
								_control.Tower.GiveAmmo(3);
								break;

							case ItemKind.Bomb :
								if (_control.Tower.IsHasGun)
								{
									// Drop gun on the map.
									Game.Instance.SpawnItem(
										_control.Position,
										ItemKind.Gun,
										_control.transform.parent.transform
									);
								}
								_control.Tower.IsHasBomb = true;
								break;

							case ItemKind.Repair :
								_control.Health = _control.maxHealth;
								break;
						}

						Finish();
					}
				}
			}
			else
			{
				// Item is lost :(
				// Finish the action.
				Finish();
			}
		}

		public void Pickup(ItemKind aItem)
		{
			_itemKind = aItem;
			_targetItem = _blackboard.GetItem(_itemKind);
			if (_targetItem != null)
			{
				_control.MoveTo(_targetItem.Position);
			}
		}
	}
}