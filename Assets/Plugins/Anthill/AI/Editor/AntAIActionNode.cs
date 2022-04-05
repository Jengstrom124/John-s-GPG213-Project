namespace Anthill.AI
{
	using UnityEngine;
	using UnityEditor;

	using Anthill.Utils;
	using Anthill.Extensions;

	/// <summary>
	/// This class implements a cards of actions in AIWorkbench.
	/// </summary>
	public class AntAIActionNode : AntAIBaseNode
	{
		private enum ConditionKind
		{
			Pre,
			Post
		}

		public delegate void ActionNodeDelegate(AntAIActionNode aNode);
		public event ActionNodeDelegate EventDelete;
		public event ActionNodeDelegate EventAsDefault;
	
	#region Private Variables

		/// <summary>
		/// Height of base EditorGUILayout component.
		/// </summary>
		private const float baseHeight = 20.0f;

		/// <summary>
		/// Height of the base toolbar component.
		/// </summary>
		private const float conditionHeight = 21.0f;

		private AntAIScenario _scenario;
		private AntAIScenarioAction _action;

		private GUIStyle _defaultStyle;
		private GUIStyle _activeDefaultStyle;
		private GUIStyle _badgeStyle;

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

		public AntAIScenarioAction Action
		{
			get => _action;
			set
			{
				_action = value;
				rect.position = _action.position;
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
			get => string.Format(title, _action.name);
		}

	#endregion

	#region Public Methods

		public AntAIActionNode(Vector2 aPosition, float aWidth, float aHeight,
			GUIStyle aDefaultStyle, GUIStyle aSelectedStyle) : base(aPosition, aWidth, aHeight, aDefaultStyle, aSelectedStyle)
		{
			_defaultStyle = CreateNodeStyle("node1.png");
			_activeDefaultStyle = CreateNodeStyle("node1 on.png");
			_badgeStyle = new GUIStyle(EditorStyles.toolbarButton);
 			_badgeStyle.normal.textColor = Color.white;
			_badgeStyle.active.textColor = Color.white;
			_badgeStyle.focused.textColor = Color.white;
			_badgeStyle.hover.textColor = Color.white;
		}

		public override void Drag(Vector2 aDelta)
		{
			var oldPos = rect.position;
			base.Drag(aDelta);
			if (!AntMath.Equal(oldPos.x, rect.position.x) || 
				!AntMath.Equal(oldPos.y, rect.position.y))
			{
				_action.position = rect.position;
				EditorUtility.SetDirty(_scenario);
			}
		}

		public override void Draw()
		{
			UpdateStyle();
			
			rect.height = (baseHeight * 3 + conditionHeight * (_action.pre.Length + _action.post.Length + 2)) + 1;
			rect.height += (_action.pre.Length == 0) ? conditionHeight : 0.0f;
			rect.height += (_action.post.Length == 0) ? conditionHeight : 0.0f;
			rect.height += 52.0f;

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
			content.height = rect.height - 52.0f;
			GUI.Box(content, "", _bodyStyle);

			EditorGUI.BeginChangeCheck();
			GUILayout.BeginArea(content);
			{
				EditorGUIUtility.labelWidth = 40.0f;

				GUILayout.Space(2.0f);

				// Action name.
				GUILayout.BeginHorizontal();
				{
					GUILayout.Space(4.0f);
					_action.name = EditorGUILayout.TextField("Name", _action.name);
				}
				GUILayout.EndHorizontal();

				// State reference.
				GUILayout.BeginHorizontal();
				{
					GUILayout.Space(4.0f);
					var prevState = _action.state;
					_action.state = (GameObject) EditorGUILayout.ObjectField(
						"State", _action.state, typeof(GameObject), false
					);
					if (!System.Object.ReferenceEquals(_action.state, prevState) && _action.state != null)
					{
						var state = _action.state.GetComponent<AntAIState>();
						if (state == null)
						{
							EditorUtility.DisplayDialog(
								"Incorrect prefab", 
								$"Prefab `{_action.state.name}` don't have the AntAIState script.", 
								"Ok"
							);
							_action.state = prevState;
						}
					}
				}
				GUILayout.EndHorizontal();

				// Cost value.
				GUILayout.BeginHorizontal();
				{
					GUILayout.Space(4.0f);
					_action.cost = EditorGUILayout.IntField("Cost", _action.cost);

					if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(18.0f), GUILayout.Height(18.0f)))
					{
						_action.cost += 1;
					}

					if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(18.0f), GUILayout.Height(18.0f)))
					{
						_action.cost -= 1;
					}
				}
				GUILayout.EndHorizontal();
				
				DrawConditionList("Pre Conditions", ref _action.pre, ConditionKind.Pre);
				DrawConditionList("Post Conditions", ref _action.post, ConditionKind.Post);
			}
			GUILayout.EndArea();

			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(_scenario);
			}
		}

		public override bool ProcessEvents(Event aEvent, AntAIWorkbench aWorkbench)
		{
			var result = false;
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
						_action.position = rect.position;
						EditorUtility.SetDirty(_scenario);
					}
					isDragged = false;
					break;

				case EventType.MouseDrag :
					if (aEvent.button == 0 && isDragged)
					{
						Drag(aEvent.delta);
						aEvent.Use();
						result = true;
					}
					break;
			}
			return result;
		}

	#endregion

	#region Private Methods

		private void UpdateStyle()
		{
			if (IsSelected)
			{
				currentStyle = (_action.isDefault)
					? _activeDefaultStyle
					: selectedStyle;
			}
			else
			{
				currentStyle = (_action.isDefault)
					? _defaultStyle
					: normalStyle;
			}

			Color = (_action.isDefault)
				? AntAIEditorStyle.CardBlue
				: AntAIEditorStyle.CardWhite;
		}

		protected override void ProcessContextMenu()
		{
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("Set as Default"), _action.isDefault, SetAsDefaultHandler);
			menu.AddSeparator("");
			menu.AddItem(new GUIContent($"Delete `{_action.name}`"), false, DeleteHandler);
			menu.ShowAsContext();
		}

		private void DrawConditionList(string aLabel, 
			ref AntAIScenarioItem[] aConditions, ConditionKind aConditionKind)
		{
			GUILayout.BeginVertical();
			{
				var c = GUI.color;
				GUILayout.BeginHorizontal(EditorStyles.toolbar);
				{
					EditorGUILayout.LabelField(aLabel, EditorStyles.boldLabel);
					GUILayout.FlexibleSpace();
					EditorGUILayout.BeginVertical();
					GUILayout.Space(2.0f);
					if (GUILayout.Button("", "OL Plus"))
					{
						var menu = new GenericMenu();
						for (int i = 0, n = _scenario.conditions.list.Length; i < n; i++)
						{
							switch (aConditionKind)
							{
								case ConditionKind.Pre :
									menu.AddItem(new GUIContent(
										_scenario.conditions.list[i].name), 
										false, 
										OnAddPreCondition, 
										_scenario.conditions.list[i].name
									);
									break;

								case ConditionKind.Post :
									menu.AddItem(new GUIContent(
										_scenario.conditions.list[i].name), 
										false, 
										OnAddPostCondition, 
										_scenario.conditions.list[i].name
									);
									break;
							}
						}
						menu.ShowAsContext();
					}
					EditorGUILayout.EndVertical();
				}
				GUILayout.EndHorizontal();

				if (aConditions.Length == 0)
				{
					GUILayout.BeginHorizontal(EditorStyles.toolbar);
					EditorGUILayout.LabelField("No Conditions", EditorStyles.centeredGreyMiniLabel);
					GUILayout.EndHorizontal();
				}
				else
				{
					int delIndex = -1;
					for (int i = 0, n = aConditions.Length; i < n; i++)
					{
						GUILayout.BeginHorizontal(EditorStyles.toolbar);
						{
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
			_action.isDefault = !_action.isDefault;
			for (int i = 0, n = _scenario.actions.Length; i < n; i++)
			{
				if (!System.Object.ReferenceEquals(_scenario.actions[i], _action))
				{
					_scenario.actions[i].isDefault = false;
				}
			}

			EditorUtility.SetDirty(Scenario);
			EventAsDefault?.Invoke(this);
		}

		private void DeleteHandler()
		{
			if (EditorUtility.DisplayDialog("Delete", $"Remove the `{_action.name}` action?", "Yes", "No"))
			{
				EventDelete?.Invoke(this);
			}
		}

		private void OnAddPreCondition(object aValue)
		{
			var item = new AntAIScenarioItem
			{
				id = _scenario.conditions.GetIndex(aValue.ToString()),
				value = true
			};
			AntArray.Add(ref _action.pre, item);
			EditorUtility.SetDirty(Scenario);
		}

		private void OnAddPostCondition(object aValue)
		{
			var item = new AntAIScenarioItem
			{
				id = _scenario.conditions.GetIndex(aValue.ToString()),
				value = true
			};
			AntArray.Add(ref _action.post, item);
			EditorUtility.SetDirty(Scenario);
		}

	#endregion
	}
}