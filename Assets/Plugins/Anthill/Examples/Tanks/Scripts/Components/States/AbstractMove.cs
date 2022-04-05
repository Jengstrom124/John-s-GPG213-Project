namespace Tanks
{
	using UnityEngine;
	using Anthill.AI;
	using Anthill.Utils;

	/// <summary>
	/// This is abstract class of all movement states.
	/// </summary>
	public abstract class AbstractMove : AntAIState
	{
		#region Variables
		
		protected TankControl _control;
		protected TankBlackboard _blackboard;
		
		#endregion
		#region Public Methods

		/// <summary>
		/// This function called when our object with AI just created.
		/// </summary>
		public override void Create(GameObject aGameObject)
		{
			_control = aGameObject.GetComponent<TankControl>();
			_blackboard = aGameObject.GetComponent<TankBlackboard>();
		}

		/// <summary>
		/// This function called before this state will be inactive.
		/// </summary>
		public override void Exit()
		{
			_control.EventArrived -= ArrivedHandler;
			_control.EventWayNotFound -= WayNotFoundHandler;
		}
		
		/// <summary>
		/// Moves tank to the specified point.
		/// </summary>
		/// <param name="aPoint">Point of target position.</param>
		public void MoveTo(Vector2 aPoint)
		{
			// Subscribe to the TankControl events.
			_control.EventArrived += ArrivedHandler;
			_control.EventWayNotFound += WayNotFoundHandler;

			// Move to our target.
			_control.MoveTo(aPoint);
		}

		/// <summary>
		/// This function finds some info about specified item in tank memory,
		/// and if we found info, then moving to the founded position.
		/// </summary>
		/// <param name="aKind">Kind of the item that we want to find.</param>
		public void SearchItem(ItemKind aKind)
		{
			var targetPoint = Vector2.zero;
			if (_blackboard.GetSpawner(aKind, out targetPoint))
			{
				// If we found in our memory where is item spawner,
				// then move to this point.
				if (AntMath.Distance(_control.Position, targetPoint) > 0.8f)
				{
					MoveTo(targetPoint);
					return;
				}
			}
			
			// Otherwise just move to the random point and
			// hoping for the success!
			MoveTo(GetRandomPoint());
		}

		/// <summary>
		/// Returns random point on the map. Useful if we don’t know 
		/// where to send the tank, send it to a random point.
		/// </summary>
		/// <returns>Random point on the map.</returns>
		public Vector2 GetRandomPoint()
		{
			// Select random node in the NavMesh for random movement.
			int index = AntMath.RandomRangeInt(0, Game.NavMesh.nodes.Count - 1);

			// Get the center of node as our target.
			return Game.NavMesh.GetNodeCenter(index);
		}
		
		#endregion
		#region Event Handlers

		private void ArrivedHandler(TankControl aControl)
		{
			// We have reach our target!

			// Don't forget to finish this state, 
			// that is means that AIAgent can select new state.
			Finish();
		}

		private void WayNotFoundHandler(TankControl aControl)
		{
			// We can't reach our target...
			// But no problem, just finish this state
			// and AIAgent decide that we will do next...
			// Maybe select another point? ;)
			Finish();
		}

		#endregion
	}
}