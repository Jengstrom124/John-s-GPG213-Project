namespace Anthill.AI
{
	using UnityEngine;

	/// <summary>
	/// Helper class for correction of card positions when moved from version
	/// Ant AI 1.2.0 to 1.3.0.
	/// </summary>
	public static class FixPositionHelper
	{
		public static void Fix(AntAIScenario aAiScenario)
		{
			var offset = Vector2.zero;
			for (int i = 0, n = aAiScenario.actions.Length; i < n; i++)
			{
				if (aAiScenario.actions[i].position.x < 200.0f)
				{
					offset.x = 200.0f;
				}

				if (aAiScenario.actions[i].position.y < 30.0f)
				{
					offset.y = 30.0f;
				}
			}

			for (int i = 0, n = aAiScenario.goals.Length; i < n; i++)
			{
				if (aAiScenario.goals[i].position.x < 200.0f)
				{
					offset.x = 200.0f;
				}

				if (aAiScenario.goals[i].position.y < 30.0f)
				{
					offset.y = 30.0f;
				}
			}

			for (int i = 0, n = aAiScenario.worldStates.Length; i < n; i++)
			{
				if (aAiScenario.worldStates[i].position.x < 200.0f)
				{
					offset.x = 200.0f;
				}

				if (aAiScenario.worldStates[i].position.y < 30.0f)
				{
					offset.y = 30.0f;
				}
			}

			// Correction.
			// -----------

			for (int i = 0, n = aAiScenario.actions.Length; i < n; i++)
			{
				aAiScenario.actions[i].position += offset;
			}

			for (int i = 0, n = aAiScenario.goals.Length; i < n; i++)
			{
				aAiScenario.goals[i].position += offset;
			}

			for (int i = 0, n = aAiScenario.worldStates.Length; i < n; i++)
			{
				aAiScenario.worldStates[i].position += offset;
			}
		}
	}
}