namespace Tanks
{
	using UnityEngine;
	using Anthill.AI;
	using Anthill.Utils;

	/// <summary>
	/// Implementation of the bomb detonation.
	/// </summary>
	public class DetonateState : AntAIState
	{
		#region Variables

		private TankControl _control;

		#endregion
		#region Public Methods
		
		public override void Create(GameObject aGameObject)
		{
			_control = aGameObject.GetComponent<TankControl>();
		}

		public override void Enter()
		{
			// Explode all tanks around.
			var G = Game.Instance;
			var pos = _control.Position;
			float dist = 0.0f;
			for (int i = G.tanks.Count - 1; i >= 0; i--)
			{
				dist = AntMath.Distance(pos, G.tanks[i].Position);
				if (dist <= 1.0f)
				{
					// Kill all tanks around.
					G.tanks[i].Health = 0.0f;
				}
			}

			// TODO: Make explosion effect.
		}
		
		#endregion
	}
}