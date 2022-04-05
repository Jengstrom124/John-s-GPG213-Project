namespace Anthill.AI
{
	using UnityEngine;
	using UnityEditor;

	using Anthill.Utils;
	using Anthill.Extensions;

	/// <summary>
	/// This is implementation of the Goal card in the AIWorkbench.
	/// </summary>
	public class AntAIGoalNode : AntAIBaseNode
	{
		public delegate void GoalNodeDelegate(AntAIGoalNode aNode);
		public event GoalNodeDelegate EventDelete;
		public event GoalNodeDelegate EventAsDefault;

	#region Private Variables

		private float _baseHeight = 20.0f;
		private const float _lineHeight = 21.0f;
		private AntAIScenario _scenario;
		private AntAIScenarioGoal _goal;

		private GUIStyle _defaultStyle;
		private GUIStyle _activeDefaultStyle;
		private GUIStyle _badgeStyle;
		private GUIStyle _labelStyle;

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

		public AntAIScenarioGoal Goal
		{
			get => _goal;
			set
			{
				_goal = value;
				rect.position = _goal.position;
			}
		}

		public override bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				UpdateStyle();
			}
		}

		public override string Title
		{
			get => string.Format(title, _goal.name);
		}

	#endregion

	#region Public Methods

		public AntAIGoalNode(Vector2 aPosition, float aWidth, float aHeight,
			GUIStyle aDefaultStyle, GUIStyle aSelectedStyle) : base(aPosition, aWidth, aHeight, aDefaultStyle, aSelectedStyle)
		{
			_defaultStyle = CreateNodeStyle("node1.png");
			_activeDefaultStyle = CreateNodeStyle("node1 on.png");
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
				_goal.position = rect.position;
				EditorUtility.SetDirty(_scenario);
			}
		}

		public override void Draw()
		{
			UpdateStyle();

			rect.height = (_baseHeight + _lineHeight * (_goal.conditions.Length + 1)) + 1.0f;
			rect.height += (_goal.conditions.Length == 0) ? _lineHeight : 0.0f;
			rect.height += 54.0f;

			GUI.Box(rect, "", currentStyle);

			var title = Title;
			if (title.Length > AntAIEditorStyle.CardTitleLimit)
			{
				title = $"{title.Substring(0, AntAIEditorStyle.CardTitleLimit - 3)}...";
			}
			
			// Title
			GUI.Label(
				new Rect(rect.x + 12.0f, rect.y + 12.0f, rect.y + 12.0f, rect.width - 24.0f), 
				title, 
				_titleStyle
			);

			content.x = rect.x + 7.0f;
			content.y = rect.y + 30.0f;
			content.width = rect.width - 14.0f;
			content.height = rect.height - 54.0f;
			GUI.Box(content, "", _bodyStyle);

			EditorGUI.BeginChangeCheck();
			GUILayout.BeginArea(content);
			{
				EditorGUIUtility.labelWidth = 40.0f;

				GUILayout.Space(2.0f);
				GUILayout.BeginHorizontal();
				{
					GUILayout.Space(4.0f);
					_goal.name = EditorGUILayout.TextField("Name", _goal.name);
				}
				GUILayout.EndHorizontal();

				DrawConditionList("Goal Conditions", ref _goal.conditions);
			}
			GUILayout.EndArea();
			
			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(_scenario);
			}
		}

		public override bool ProcessEvents(Event aEvent, AntAIWorkbench aWorkbench)
		{
			switch (aEvent.type)
			{
				case EventType.MouseDown :
					if (aEvent.button == 0)
					{
						if (rect.Contains(aEvent.mousePosition))
						{
							isDragged = true;
							GUI.changed = true;
							IsSelected = true;
						}
						else
						{
							GUI.changed = true;
							IsSelected = false;
						}
					}

					if (aEvent.button == 1 && IsSelected &&
						rect.Contains(aEvent.mousePosition))
					{
						ProcessContextMenu();
						aEvent.Use();
					}
					break;

				case EventType.MouseUp :
					if (isDragged && aWorkbench.IsAlignToGrid)
					{
						var dx = Mathf.RoundToInt((rect.x - aWorkbench.Offset.x) / 20.0f);
						var dy = Mathf.RoundToInt((rect.y - aWorkbench.Offset.y) / 20.0f);
						rect.x = aWorkbench.Offset.x + dx * 20.0f;
						rect.y = aWorkbench.Offset.y + dy * 20.0f;
						aWorkbench.Repaint();
						_goal.position = rect.position;
						EditorUtility.SetDirty(_scenario);
					}
					isDragged = false;
					break;

				case EventType.MouseDrag :
					if (aEvent.button == 0 && isDragged)
					{
						Drag(aEvent.delta);
						aEvent.Use();
						return true;
					}
					break;
			}
			return false;
		}

	#endregion

	#region Private Methods

		private void UpdateStyle()
		{
			if (IsSelected)
			{
				currentStyle = (_goal.isDefault) 
					? _activeDefaultStyle
					: selectedStyle;
			}
			else
			{
				currentStyle = (_goal.isDefault)
					? _defaultStyle
					: normalStyle;
			}

			Color = (_goal.isDefault)
				? AntAIEditorStyle.CardBlue
				: AntAIEditorStyle.CardWhite;
		}

		protected override void ProcessContextMenu()
		{
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("Set as Default"), _goal.isDefault, SetAsDefaultHandler);
			menu.AddSeparator("");
			menu.AddItem(new GUIContent($"Delete `{_goal.name}`"), false, DeleteHandler);
			menu.ShowAsContext();
		}

		private void DrawConditionList(string aLabel, ref AntAIScenarioItem[] aConditions)
		{
			GUILayout.BeginVertical();
			{
				var c = GUI.color;
				GUILayout.BeginHorizontal(EditorStyles.toolbar);
				{
					EditorGUILayout.LabelField(aLabel, EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();

					GUILayout.BeginVertical();
					GUILayout.Space(2.0f);
					if (GUILayout.Button("", "OL Plus"))
					{
						var menu = new GenericMenu();
						for (int i = 0, n = _scenario.conditions.list.Length; i < n; i++)
						{
							menu.AddItem(
								new GUIContent(_scenario.conditions.list[i].name), 
								false, 
								OnAddCondition, 
								_scenario.conditions.list[i].name
							);
						}
						menu.ShowAsContext();
					}
					GUILayout.EndVertical();
				}
				GUILayout.EndHorizontal();

				if (aConditions.Length == 0)
				{
					EditorGUILayout.LabelField("No Conditions", EditorStyles.centeredGreyMiniLabel);
				}
				else
				{
					int delIndex = -1;
					for (int i = 0, n = aConditions.Length; i < n; i++)
					{
						GUILayout.BeginHorizontal(EditorStyles.toolbar);
						GUI.color = c * ((aConditions[i].value) 
							? AntAIEditorStyle.Green
							: AntAIEditorStyle.Red
						);

						if (GUILayout.Button(aConditions[i].value.ToStr(), _badgeStyle, GUILayout.MaxWidth(20.0f)))
						{
							aConditions[i].value = !aConditions[i].value;
						}

						EditorGUILayout.LabelField(_scenario.conditions.GetName(aConditions[i].id), EditorStyles.miniBoldLabel);

						GUI.color = c;
						if (GUILayout.Button("×", EditorStyles.toolbarButton, GUILayout.MaxWidth(18.0f)))
						{
							delIndex = i;
						}
						GUILayout.EndHorizontal();
					}
					
					if (delIndex > -1)
					{
						AntArray.RemoveAt(ref aConditions, delIndex);
					}
				}
			}
			GUILayout.EndVertical();
		}

	#endregion

	#region Event Handlers

		private void SetAsDefaultHandler()
		{
			_goal.isDefault = !_goal.isDefault;
			for (int i = 0, n = _scenario.goals.Length; i < n; i++)
			{
				if (!System.Object.ReferenceEquals(_scenario.goals[i], _goal))
				{
					_scenario.goals[i].isDefault = false;
				}
			}

			EditorUtility.SetDirty(Scenario);
			EventAsDefault?.Invoke(this);
		}

		private void DeleteHandler()
		{
			if (EditorUtility.DisplayDialog("Delete", $"Remove the `{_goal.name}` goal?", "Yes", "No"))
			{
				EventDelete?.Invoke(this);
			}
		}

		private void OnAddCondition(object aValue)
		{
			var item = new AntAIScenarioItem
			{
				id = _scenario.conditions.GetIndex(aValue.ToString()),
				value = true
			};
			AntArray.Add(ref _goal.conditions, item);
		}

	#endregion
	}
}