namespace Anthill.AI
{
	using UnityEngine;
	using UnityEditor;

	using Anthill.Extensions;

	/// <summary>
	/// This is custom editor for the AIAgent.
	/// </summary>
	[CustomEditor(typeof(AntAIAgent))]
	public class AntAIAgentEditor : Editor
	{
	#region Private Variables
		
		private AntAIAgent _self;
		private bool _isGoal;
		private bool _isWorldState;
		private bool _isPlan;
		
	#endregion
	
	#region Unity Calls
		
		private void OnEnable()
		{
			_self = (AntAIAgent) target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (Application.isPlaying)
			{
				var c = GUI.color;

				// 1. Show our active goal.
				// ------------------------
#if UNITY_2018_3_OR_NEWER
				_isGoal = EditorGUILayout.BeginFoldoutHeaderGroup(_isGoal, "Goal");
#else
				_isGoal = EditorGUILayout.Foldout(_isGoal, "Goal");
#endif
				if (_isGoal)
				{
					for (int i = 0; i < AntAIPlanner.MAX_ATOMS; i++)
					{
						if (_self.Goal.GetMask(i))
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.Space(4.0f);
								GUI.color = c * ((_self.Goal.GetValue(i)) 
									? AntAIEditorStyle.Green
									: AntAIEditorStyle.Red
								);
								GUILayout.Button(_self.Goal.GetValue(i).ToStr(), "CN CountBadge", GUILayout.MaxWidth(20.0f), GUILayout.MaxHeight(20.0f));
								EditorGUILayout.LabelField(_self.planner.atoms[i]);
							}
							EditorGUILayout.EndHorizontal();
						}
					}
				}
#if UNITY_2018_3_OR_NEWER
				EditorGUILayout.EndFoldoutHeaderGroup();
#endif
				GUI.color = c;

				// 2. Show current world state.
				// ----------------------------
#if UNITY_2018_3_OR_NEWER
				_isWorldState = EditorGUILayout.BeginFoldoutHeaderGroup(_isWorldState, "World State");
#else
				_isWorldState = EditorGUILayout.Foldout(_isWorldState, "World State");
#endif
				if (_isWorldState)
				{
					for (int i = 0; i < AntAIPlanner.MAX_ATOMS; i++)
					{
						if (_self.worldState.GetMask(i))
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.Space(4.0f);
								GUI.color = c * ((_self.worldState.GetValue(i)) 
									? AntAIEditorStyle.Green
									: AntAIEditorStyle.Red
								);
								GUILayout.Button(_self.worldState.GetValue(i).ToStr(), "CN CountBadge", GUILayout.MaxWidth(20.0f), GUILayout.MaxHeight(20.0f));
								EditorGUILayout.LabelField(string.Concat(_self.planner.atoms[i]));
							}
							EditorGUILayout.EndHorizontal();
						}
					}
				}
#if UNITY_2018_3_OR_NEWER
				EditorGUILayout.EndFoldoutHeaderGroup();
#endif
				GUI.color = c;

				// 3. Show current plan.
				// ---------------------
				// GUI.color = c * ((_self.currentPlan.isSuccess) 
				// 	? new Color(0.5f, 1.0f, 0.5f) // green
				// 	: new Color(1.0f, 0.5f, 0.5f) // red
				// );

				var planSuccess = (_self.currentPlan.isSuccess)
					? "Plan (Status: Success)"
					: "Plan (Status: Failed)";

#if UNITY_2018_3_OR_NEWER
				_isPlan = EditorGUILayout.BeginFoldoutHeaderGroup(_isPlan, planSuccess);
#else
				_isPlan = EditorGUILayout.Foldout(_isPlan, planSuccess);
#endif
				if (_isPlan)
				{
					string value = string.Empty;
					for (int i = 0; i < _self.currentPlan.Count; i++)
					{
						if (_self.worldState.GetMask(i))
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.Space(4.0f);
								value = string.Concat((i + 1).ToString(), ". ", _self.currentPlan[i]);
								if (i == 0)
								{
									EditorGUILayout.LabelField(value, EditorStyles.boldLabel);
								}
								else
								{
									EditorGUILayout.LabelField(value);
								}
							}
							EditorGUILayout.EndHorizontal();
						}
					}
				}
#if UNITY_2018_3_OR_NEWER
				EditorGUILayout.EndFoldoutHeaderGroup();
#endif
				GUI.color = c;
			}
		}
		
	#endregion
	}
}