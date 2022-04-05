namespace Anthill.AI
{
	using UnityEngine;
	using UnityEditor;

	/// <summary>
	/// Custom editor for the AntAIScenario scriptable object.
	/// </summary>
	[CustomEditor(typeof(AntAIScenario))]
	public class AntAIScenarioEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (GUILayout.Button("Open AI Workbench", GUILayout.MinHeight(40.0f)))
			{
				AntAIWorkbench.OpenScenario(target.name);
			}
		}
	}
}