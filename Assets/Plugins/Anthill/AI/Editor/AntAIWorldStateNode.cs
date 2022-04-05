namespace Anthill.AI
{
	using UnityEngine;
	using UnityEditor;

	using Anthill.Utils;
	using Anthill.Extensions;

	/// <summary>
	/// This is implementation world state card for debuging of the scenarios.
	/// </summary>
	public class AntAIWorldStateNode : AntAIBaseNode
	{
		public delegate void WorldStateNodeDelegate(AntAIWorldStateNode aNode);
		public event WorldStateNodeDelegate EventDelete;
		public event WorldStateNodeDelegate EventClearPlan;

		public delegate void BuildPlanDelegate(AntAIWorldStateNode aNode, AntAIPlan aPlan, AntAIPlanner aPlanner);
		public event BuildPlanDelegate EventBuildPlan;

	#region Private Variables

		private AntAIScenario _scenario;
		private AntAIWorldState _worldState;
		private GUIStyle _badgeStyle;
		private GUIStyle _labelStyle;

		private const float lineHeight = 21.0f;
		private string _currentGoal = string.Empty;

	#endregion

	#region Getters / Setters

		public AntAIScenario Scenario
		{
			get => _scenario;
			set
			{
				_scenario = value;
				rect.position = new Vector2(200.0f, 30.0f);
			}
		}

		public AntAIWorldState WorldState
		{
			get => _worldState;
			set
			{
				_worldState = value;
				rect.position = _worldState.position;
			}
		}

		public override Color Color
		{
			get => AntAIEditorStyle.CardOrange;
		}

	#endregion

	#region Public Methods

		public AntAIWorldStateNode(Vector2 aPosition, float aWidth, float aHeight,
			GUIStyle aDefaultStyle, GUIStyle aSelectedStyle) : base(aPosition, aWidth, aHeight, aDefaultStyle, aSelectedStyle)
		{
			_badgeStyle = new GUIStyle(EditorStyles.toolbarButton);
 			_badgeStyle.normal.textColor = Color.white;
			_badgeStyle.active.textColor = Color.white;
			_badgeStyle.focused.textColor = Color.white;
			_badgeStyle.hover.textColor = Color.white;

			_labelStyle = new GUIStyle(EditorStyles.label);
			var m = _labelStyle.margin;
			m.top = 0;
			_labelStyle.margin = m;
		}

		public override void Drag(Vector2 aDelta)
		{
			var oldPos = rect.position;
			base.Drag(aDelta);
			if (!AntMath.Equal(oldPos.x, rect.position.x) || 
				!AntMath.Equal(oldPos.y, rect.position.y))
			{
				_worldState.position = rect.position;
				EditorUtility.SetDirty(_scenario);
			}
		}

		public override void Draw()
		{
			if (_scenario == null)
			{
				return;
			}

			if (_worldState.isAutoUpdate && Event.current.type == EventType.Repaint)
			{
				CheckGoals();
			}

			rect.height = (_worldState.list.Length > 0)
				? lineHeight * _worldState.list.Length - 1.0f
				: lineHeight - 1.0f;
			rect.height += 52.0f;
			GUI.Box(rect, "", currentStyle);
			
			// Title
			GUI.Label(new Rect(rect.x + 12.0f, rect.y + 12.0f, rect.y + 12.0f, rect.width - 24.0f), title, _titleStyle);

			content.x = rect.x + 7.0f;
			content.y = rect.y + 30.0f;
			content.width = rect.width - 14.0f;
			content.height = rect.height - 52.0f;
			GUI.Box(content, "", _bodyStyle);

			var r = new Rect(rect.x + rect.width - 25.0f, rect.y + 11.0f, 20.0f, 44.0f);
			EditorGUI.BeginChangeCheck();

			GUILayout.BeginArea(r);
			if (GUILayout.Button("", "OL Plus", GUILayout.MaxWidth(16.0f), GUILayout.MaxHeight(16.0f)))
			{
				var menu = new GenericMenu();
				for (int i = 0, n = _scenario.conditions.list.Length; i < n; i++)
				{
					menu.AddItem(new GUIContent(_scenario.conditions.list[i].name), false, AddConditionHandler, _scenario.conditions.list[i].name);
				}

				menu.AddSeparator("");
				menu.AddItem(new GUIContent("Add All"), false, AddAllConditionsHandler, "Add All");
				menu.ShowAsContext();
			}
			GUILayout.EndArea();

			var c = GUI.color;
			GUILayout.BeginArea(content);
			{
				EditorGUIUtility.labelWidth = 80.0f;
				if (_worldState.list.Length > 0)
				{
					int delIndex = -1;
					GUILayout.BeginVertical();
					{
						for (int i = 0, n = _worldState.list.Length; i < n; i++)
						{
							GUILayout.BeginHorizontal(EditorStyles.toolbar);
							{
								GUI.color = c * ((_worldState.list[i].value) 
									? AntAIEditorStyle.Green
									: AntAIEditorStyle.Red
								);
								
								if (GUILayout.Button(_worldState.list[i].value.ToStr(), _badgeStyle, GUILayout.MaxWidth(20.0f)))
								{
									_worldState.list[i].value = !_worldState.list[i].value;
									if (_worldState.isAutoUpdate)
									{
										BuildPlanHandler();
									}
								}

								EditorGUILayout.LabelField(_scenario.conditions.GetName(_worldState.list[i].id), EditorStyles.miniBoldLabel);

								GUI.color = c;
								if (GUILayout.Button("×", EditorStyles.toolbarButton, GUILayout.MaxWidth(18.0f)))
								{
									delIndex = i;
								}
							}
							GUILayout.EndHorizontal();
						}
					}
					GUILayout.EndVertical();

					if (delIndex > -1)
					{
						AntArray.RemoveAt(ref _worldState.list, delIndex);
					}
				}
				else
				{
					GUILayout.Label("No Coditions", EditorStyles.centeredGreyMiniLabel);
				}
			}
			GUILayout.EndArea();
			
			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(_scenario);
			}
		}

		protected override void ProcessContextMenu()
		{
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("Build Plan"), false, BuildPlanHandler);
			menu.AddItem(new GUIContent("Clear Plan"), false, ClearPlanHandler);
			menu.AddSeparator("");
			menu.AddItem(new GUIContent("Auto Update"), _worldState.isAutoUpdate, AutoUpdateHandler);
			menu.AddItem(new GUIContent("Delete World State"), false, DeleteHandler);
			menu.ShowAsContext();
		}

		private void CheckGoals()
		{
			for (int i = 0, n = _scenario.goals.Length; i < n; i++)
			{
				if (_scenario.goals[i].isDefault && !_currentGoal.Equals(_scenario.goals[i].name))
				{
					_currentGoal = _scenario.goals[i].name;
					BuildPlanHandler();
				}
			}
		}
		
	#endregion

	#region Event Handlers
		
		public void BuildPlanHandler()
		{
			// Create planner.
			var planner = new AntAIPlanner();
			// planner.DebugMode = true;
			planner.LoadScenario(_scenario);

			// Create current world state and set current conditions.
			var current = new AntAICondition();
			current.BeginUpdate(planner);
			for (int i = 0, n = _worldState.list.Length; i < n; i++)
			{
				current.Set(_scenario.conditions.GetName(_worldState.list[i].id), _worldState.list[i].value);
			}
			current.EndUpdate();

			// Create our goal world state.
			AntAIScenarioGoal defaultGoal = null;
			for (int i = 0, n = _scenario.goals.Length; i < n; i++)
			{
				if (_scenario.goals[i].isDefault)
				{
					defaultGoal = _scenario.goals[i];
					break;
				}
			}

			A.Assert(defaultGoal == null, "Default goal not found!");
			if (defaultGoal == null) return;

			// Copy conditions from the scenario goal to the goal conditions.
			var goal = new AntAICondition();
			goal.BeginUpdate(planner);
			for (int i = 0, n = defaultGoal.conditions.Length; i < n; i++)
			{
				goal.Set(_scenario.conditions.GetName(defaultGoal.conditions[i].id), defaultGoal.conditions[i].value);
			}
			goal.EndUpdate();

			// Create and build the plan.
			var plan = new AntAIPlan();
			planner.MakePlan(ref plan, current, goal);

			// Call event when plan is ready.
			if (EventBuildPlan != null)
			{
				EventBuildPlan(this, plan, planner);
			}
		}

		private void ClearPlanHandler()
		{
			if (EventClearPlan != null)
			{
				EventClearPlan(this);
			}
		}

		private void AutoUpdateHandler()
		{
			_worldState.isAutoUpdate = !_worldState.isAutoUpdate;
		}

		private void DeleteHandler()
		{
			if (EditorUtility.DisplayDialog("Delete", "Remove the World State?", "Yes", "No"))
			{
				if (EventDelete != null)
				{
					EventDelete(this);
				}
			}
		}

		private void AddAllConditionsHandler(object aValue)
		{
			_worldState.list = new AntAIScenarioItem[0];
			for (int i = 0, n = _scenario.conditions.list.Length; i < n; i++)
			{
				AntArray.Add(ref _worldState.list, new AntAIScenarioItem
				{
					id = _scenario.conditions.GetIndex(_scenario.conditions.list[i].name),
					value = true	
				});
			}
			EditorUtility.SetDirty(_scenario);
		}

		private void AddConditionHandler(object aValue)
		{
			var item = new AntAIScenarioItem
			{
				id = _scenario.conditions.GetIndex(aValue.ToString()),
				value = true
			};
			AntArray.Add(ref _worldState.list, item);
			EditorUtility.SetDirty(_scenario);
		}

	#endregion
	}
}