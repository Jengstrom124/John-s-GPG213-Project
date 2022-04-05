namespace Anthill.AI
{
	using System;
	using System.Collections.Generic;

	using UnityEngine;
	using UnityEditor;
	
	using Anthill.Editor;
	using Anthill.Utils;
	using Anthill.Extensions;

	/// <summary>
	/// Main class of the AIWorkbench.
	/// </summary>
	public class AntAIWorkbench : EditorWindow
	{
		private struct LabelData
		{
			public string caption;
			public Rect rect;
			public GUIStyle style;
		}

		private struct Connection
		{
			public Vector2 from;
			public Vector2 to;
			public Vector2 center;
			public Vector2 arrowA;
			public Vector2 arrowB;
			public AntAIBaseNode fromNode;
			public AntAIBaseNode toNode;
		}

		private struct Deletion
		{
			public int index;
			public bool confirm;

			public static Deletion Empty
			{
				get => new Deletion
				{
					index = -1,
					confirm = false
				};
			}
		}

	#region Variables

		// Nodes
		private List<AntAIBaseNode> _nodes;

		// Styles
		private GUIStyle _titleStyle;
		private GUIStyle _labelStyle;
		private GUIStyle _smallLabelStyle;
		private GUIStyle _nodeStyle;
		private GUIStyle _activeNodeStyle;
		private GUIStyle _actionStyle;
		private GUIStyle _activeActionStyle;
		private GUIStyle _failStyle;
		private GUIStyle _activeFailStyle;
		private GUIStyle _successStyle;
		private GUIStyle _activeSuccessStyle;

		// Helpers
		private Vector2 _spawnPosition;
		private bool _alignToGrid;
		private bool _isReloaded;
		private bool _isConditionsFoldout;
		private bool _isCardsFoldout;
		private Deletion _delCondition;
		private Deletion _delNode;
		private Vector2 _sideBarScrollPos;

		// Scrolling window
		private Rect _scrollViewRect;
		private Vector2 _scrollPos = Vector2.zero;
		private Vector2 _scrollStartMousePos;
		private bool _isWindowScrollActive = false;

		// Mouse
		public static Vector2 mousePosition;
		public static Vector2 zoomScrollCorrectedMousePosition;
		public static Vector2 zoomScrollCorrectedMenuPosition;

		// Lerping Cam Position
		private bool _isLerpToPos = false;
		private Vector2 _targetPosition;
		private float _counter = 0.0f;

		// Zoom
		public static float zoomScale = 1f;
		private static float _zoomScaleLowerLimit = 0.6f;
		private static float _zoomScaleUpperLimit = 1.4f;

		// Sizings
    	private float _topPanelHeight = 22.0f;
    	private static float _leftPaneWidth = 200.0f;

		// Data
		private AntAIScenario _current;
		private AntAIScenario[] _scenarios;
		private List<Connection> _connections;

	#endregion

	#region Getters / Setters

		public bool IsAlignToGrid
		{
			get => _alignToGrid;
		}

		public Vector2 Offset
		{
			get => Vector2.zero;
		}

	#endregion

	#region Initialize Window

		// [MenuItem("Tools/Anthill/AI Workbench")]
		public static AntAIWorkbench ShowWindow()
		{
			var window = (AntAIWorkbench) EditorWindow.GetWindow(typeof(AntAIWorkbench), false, "AI Workbench");
			window.autoRepaintOnSceneChange = true;
			return window;
		}

		public static void OpenScenario(string aName)
		{
			ShowWindow().SelectPresetHandler(aName);
		}

		public static T[] GetAllInstances<T>() where T : ScriptableObject
		{
			string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
			T[] a = new T[guids.Length];
			for(int i = 0, n = guids.Length; i < n; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
			}

			return a;
		}

	#endregion

	#region Unit Calls

		private void OnEnable()
		{
			_titleStyle = new GUIStyle();
			_titleStyle.fontSize = 22;
			_titleStyle.normal.textColor = Color.gray;

			_labelStyle = new GUIStyle();
			_labelStyle.fontSize = 14;
			_labelStyle.normal.textColor = Color.gray;

			_smallLabelStyle = new GUIStyle();
			_smallLabelStyle.normal.textColor = Color.gray;

			_nodeStyle = CreateNodeStyle("node0.png");
			_activeNodeStyle = CreateNodeStyle("node0 on.png");
			_actionStyle = CreateNodeStyle("node5.png");
			_activeActionStyle = CreateNodeStyle("node5 on.png");
			_successStyle = CreateNodeStyle("node3.png");
			_activeSuccessStyle = CreateNodeStyle("node3 on.png");
			_failStyle = CreateNodeStyle("node6.png");
			_activeFailStyle = CreateNodeStyle("node6 on.png");

			_nodes = new List<AntAIBaseNode>();
			_connections = new List<Connection>();
			_scenarios = GetAllInstances<AntAIScenario>();
			
			_isReloaded = true;
			_delCondition = Deletion.Empty;
			_delNode = Deletion.Empty;
		}

		private void Update()
		{
			if (_isLerpToPos)
			{
				if (_counter > 1.0f)
				{
					_isLerpToPos = false;
					_counter = 0f;
				}
				else
				{
					Repaint();
					_scrollPos = Vector2.Lerp(_scrollPos, _targetPosition, _counter);
					_counter += 0.002f;
				}
			}
		}

		private void OnInspectorUpdate()
		{
			if (Selection.objects.Length == 1 && Selection.objects[0] is AntAIScenario)
			{
				var tmp = (AntAIScenario) Selection.objects[0];
				if (tmp == null)
				{
					Repaint();
				}
				else if (!System.Object.ReferenceEquals(tmp, _current))
				{
					Repaint();
				}
			}
		}

		private void OnGUI()
		{
			if (_isReloaded)
			{
				SetCurrentScenario(_current);
				_isReloaded = false;
			}

			var currentEvent = Event.current;
			mousePosition = currentEvent.mousePosition;
			zoomScrollCorrectedMousePosition = (new Vector2(mousePosition.x - 200.0f, mousePosition.y - 21.0f) / zoomScale) + _scrollPos + new Vector2(200.0f, 21.0f);
			zoomScrollCorrectedMenuPosition = ((new Vector2(mousePosition.x - 200.0f, mousePosition.y - 40.0f) + _scrollPos) / zoomScale) + new Vector2(200.0f, 40.0f);

			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical(GUILayout.Width(_leftPaneWidth));
				{
					GUILayout.BeginVertical(GUILayout.Width(_leftPaneWidth));
					{
						_sideBarScrollPos = GUILayout.BeginScrollView(_sideBarScrollPos);
						DrawPreset();
						DrawConditions();
						DrawCards();
						GUILayout.EndScrollView();
					}
					GUILayout.EndVertical();
				}
				GUILayout.EndVertical();

				AntGuiDrawer.DrawLine(
					new Vector2(_leftPaneWidth, 0.0f), 
					new Vector2(_leftPaneWidth, position.height),
					Color.black,
					1.0f
				);
				
				// Toolbar.
				GUILayout.BeginHorizontal(GUILayout.Height(_topPanelHeight));
				{
					DrawToolbar();
				}
				GUILayout.EndHorizontal();

				// AntGuiDrawer.DrawLine(
				// 	new Vector2(_leftPaneWidth, _topPanelHeight), 
				// 	new Vector2(position.width, _topPanelHeight),
				// 	Color.black,
				// 	1.0f
				// );

				// Scrollable window area.
				if (!(position.width < _leftPaneWidth + 50.0f))
				{
					DrawScrollAreaGUI(currentEvent);
				}
			}
			GUILayout.EndHorizontal();
		}

	#endregion

	#region Private Methods

		private void DrawToolbar()
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
			{
				GUILayout.FlexibleSpace();

				GUILayout.Label("Zoom");
				zoomScale = EditorGUILayout.Slider(zoomScale, _zoomScaleLowerLimit, _zoomScaleUpperLimit, GUILayout.MaxWidth(160.0f));
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DrawScrollAreaGUI(Event aEvent)
		{
			_scrollViewRect = new Rect(_leftPaneWidth, _topPanelHeight, position.width - _leftPaneWidth, position.height - _topPanelHeight - 3);
			_scrollViewRect = AntZoomArea.Begin(zoomScale, _scrollViewRect);

			using (var scope = new GUI.ScrollViewScope(_scrollViewRect, _scrollPos, new Rect(_leftPaneWidth, _topPanelHeight, 10000.0f, 10000.0f)))
			{
				scope.handleScrollWheel = false;
				_scrollPos = scope.scrollPosition;

				Handles.DrawSolidRectangleWithOutline(
					new Rect(_leftPaneWidth + 2, _topPanelHeight, 10000.0f, 10000.0f), 
					AntAIEditorStyle.Gray,
					AntAIEditorStyle.Gray
				);

				AntGuiDrawer.DrawBackgroundGrid(_scrollViewRect, _scrollPos, 20, Color.gray, 1.0f);
				AntGuiDrawer.DrawBackgroundGrid(_scrollViewRect, _scrollPos, 100, Color.gray, 1.0f);

				if (_current != null)
				{
					DrawLinks();
					DrawNodes(_nodes);
					ScrollViewGUIEvents(aEvent);
				}
				else
				{
					if (aEvent.type == EventType.Repaint)
					{
						_scrollPos = Vector2.zero;
						DrawHelp();
					}
				}
			}
			AntZoomArea.End();
		}

		public void ScrollViewGUIEvents(Event aEvent)
		{
			for (int i = _nodes.Count - 1; i >= 0; i--)
			{
				if (_nodes[i].ProcessEvents(aEvent, this))
				{
					return;
				}
			}

			// Right Click
			// -----------
			if (aEvent.button == 1)
			{
				if (aEvent.type == EventType.MouseDown)
				{
					if (_current != null)
					{
						var menu = new GenericMenu();
						menu.AddItem(new GUIContent("● Add Action"), false, AddActionHandler);
						menu.AddItem(new GUIContent("★ Add Goal"), false, AddGoalHandler);
						menu.AddItem(new GUIContent("Add World State"), false, AddWorldStateHandler);
						menu.AddSeparator("");
						menu.AddItem(new GUIContent("Align To Grid"), _alignToGrid, AlignToGridHandler);
						_spawnPosition = aEvent.mousePosition;
						menu.ShowAsContext();
					}
				}
			}

			// Left Click
			// ----------
			else if (aEvent.button == 0)
			{
				if (aEvent.type == EventType.MouseDown)
				{
					if (_scrollViewRect.Contains(mousePosition))
					{
						if (GUI.GetNameOfFocusedControl() != "nothing")
						{
							GUI.FocusControl("nothing");
						}

						var rect = new Rect(_scrollViewRect.x, _scrollViewRect.y + 14f, _scrollViewRect.width, _scrollViewRect.height - 14f);
						if (!_isWindowScrollActive && rect.Contains(mousePosition))
						{
							_isWindowScrollActive = true;
						}

						_scrollStartMousePos = mousePosition;
					}
				}
				else if (aEvent.type == EventType.MouseDrag)
				{
					if (_isWindowScrollActive)
					{
						if (_isLerpToPos)
						{
							_isLerpToPos = false;
						}
						
						Vector2 mouseMovementDifference = (mousePosition - _scrollStartMousePos);
						_scrollPos -= new Vector2(mouseMovementDifference.x / zoomScale, mouseMovementDifference.y / zoomScale);
						_scrollStartMousePos = mousePosition;
						aEvent.Use();

						GUI.changed = true;
					}
				}
				else if (aEvent.type == EventType.MouseUp || aEvent.type == EventType.DragExited)
				{
					if (_isWindowScrollActive)
					{
						_isWindowScrollActive = false;
					}

					if (GUIUtility.hotControl != 0)
					{
						GUIUtility.hotControl = 0;
					}

					aEvent.Use();
				}
			}

			// Mouse Wheel
			// -----------
			if (aEvent.type == EventType.ScrollWheel && aEvent.control)
			{
				zoomScale += -(aEvent.delta.y / 100f);
				zoomScale = Mathf.Clamp(zoomScale, _zoomScaleLowerLimit, _zoomScaleUpperLimit);            
				aEvent.Use();
			}

			// if (aEvent.modifiers == EventModifiers.Control && aEvent.keyCode == KeyCode.S)
			// {
			// 	Debug.Log("CTRL+S Keys Pressed");
			// }
		}

		private void DrawHelp()
		{
			GUI.Label(
				new Rect(210.0f, 30.0f, 200.0f, 50.0f), 
				"AI scenario not selected.",
				_titleStyle
			);

			GUI.Label(
				new Rect(220.0f, 70.0f, 200.0f, 50.0f),
				"Create new scenario:",
				_labelStyle
			);

			GUI.Label(
				new Rect(230.0f, 100.0f, 200.0f, 50.0f),
				"1. Open context menu in the Project window.",
				_smallLabelStyle
			);

			GUI.Label(
				new Rect(230.0f, 120.0f, 200.0f, 50.0f),
				"2. Select `Create ► Anthill ► AI Scenario`.",
				_smallLabelStyle
			);

			GUI.Label(
				new Rect(230.0f, 140.0f, 200.0f, 50.0f),
				"3. Enter the name and setup new AI behaviour.",
				_smallLabelStyle
			);
		}

		private void DrawLinks()
		{
			AntAIBaseNode node;
			AntAIBaseNode toNode;
			Vector2 outPosition;
			Vector2 inPosition;

			_connections.Clear();
			for (int i = 0, n = _nodes.Count; i < n; i++)
			{
				node = _nodes[i];
				for (int j = 0, nj = node.links.Count; j < nj; j++)
				{
					toNode = node.links[j];
					outPosition = node.GetOutputPoint(toNode);
					inPosition = toNode.GetInputPoint(node);

					float dist = AntMath.Distance(outPosition, inPosition);
					float ang = AntMath.AngleRad(outPosition, inPosition);
					var pos = new Vector2(
						outPosition.x + dist * 0.5f * Mathf.Cos(ang),
						outPosition.y + dist * 0.5f * Mathf.Sin(ang));
					var posA = new Vector2(
						pos.x - 10f * Mathf.Cos(ang + 35.0f * Mathf.Deg2Rad),
						pos.y - 10f * Mathf.Sin(ang + 35.0f * Mathf.Deg2Rad));
					var posB = new Vector2(
						pos.x - 10f * Mathf.Cos(ang - 35.0f * Mathf.Deg2Rad),
						pos.y - 10f * Mathf.Sin(ang - 35.0f * Mathf.Deg2Rad));

					_connections.Add(
						new Connection 
						{
							from = outPosition,
							to = inPosition,
							center = pos,
							arrowA = posA,
							arrowB = posB,
							fromNode = node,
							toNode = toNode
						}
					);
				}
			}

			Connection con;
			Color c = Color.gray;
			for (int i = 0, n = _connections.Count; i < n; i++)
			{
				con = _connections[i];
				AntGuiDrawer.DrawLine(con.from, con.to, c, 3.0f);
				AntGuiDrawer.DrawLine(con.center, con.arrowA, c, 3.0f);
				AntGuiDrawer.DrawLine(con.center, con.arrowB, c, 3.0f);
			}
		}

		private void DrawPreset()
		{
			EditorGUILayout.BeginHorizontal();
			{
				var prev = _current;
				_current = (AntAIScenario) EditorGUILayout.ObjectField(_current, typeof(AntAIScenario), false);
				if (!System.Object.ReferenceEquals(_current, prev))
				{
					SetCurrentScenario(_current);
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DrawConditions()
		{
			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			{
				var caption = (_isConditionsFoldout)
					? $"▼ Conditions"
					: $"► Conditions";

				var style = new GUIStyle(EditorStyles.toolbarButton);
				style.alignment = TextAnchor.MiddleLeft;
				_isConditionsFoldout = GUILayout.Toggle(_isConditionsFoldout, caption, style);

				if (GUILayout.Button("+ Add", EditorStyles.toolbarButton, GUILayout.Width(46)))
				{
					_isConditionsFoldout = true;
					if (_current != null)
					{
						_current.AddCondition("<Unnamed>");
						EditorUtility.SetDirty(_current);
						GUI.changed = true;
					}
				}
			}
			GUILayout.EndHorizontal();

			if (!_isConditionsFoldout)
			{
				return;
			}

			if (_current != null && _current.conditions.list.Length > 0)
			{
				for (int i = 0, n = _current.conditions.list.Length; i < n; i++)
				{
					GUILayout.BeginHorizontal(EditorStyles.toolbar);
					_current.conditions.list[i].name = EditorGUILayout.TextField(
						_current.conditions.list[i].name, 
						EditorStyles.toolbarTextField, 
						GUILayout.MaxWidth(200.0f)
					);
					
					GUILayout.FlexibleSpace();

					if (_delCondition.index == i)
					{
						var c = GUI.color;
						GUI.color = AntAIEditorStyle.Red;
						if (GUILayout.Button("Delete?", EditorStyles.toolbarButton, GUILayout.MaxWidth(70.0f)))
						{
							_delCondition.confirm = true;
						}

						GUI.color = AntAIEditorStyle.Green;
						if (GUILayout.Button("×", EditorStyles.toolbarButton, GUILayout.MaxWidth(18.0f)))
						{
							_delCondition.index = -1;
						}
						GUI.color = c;
					}
					else
					{
						if (GUILayout.Button("×", EditorStyles.toolbarButton, GUILayout.MaxWidth(18.0f)))
						{
							_delCondition.index = i;
						}
					}
					GUILayout.EndHorizontal();
				}

				if (_delCondition.index > -1 && _delCondition.confirm)
				{
					_current.RemoveConditionAt(_delCondition.index);
					EditorUtility.SetDirty(_current);
					_delCondition.index = -1;
					_delCondition.confirm = false;
				}

				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
				{
					if (GUILayout.Button("Copy Conditions as Enum", EditorStyles.toolbarButton))
					{
						CopyConditionsAsEnum();
					}
				}
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				GUILayout.Label("No Coditions", EditorStyles.centeredGreyMiniLabel);
			}
		}

		private void DrawCards()
		{
			var style = new GUIStyle(EditorStyles.toolbarButton);
			style.alignment = TextAnchor.MiddleLeft;

			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			{
				var caption = (_isCardsFoldout)
					? $"▼ List of Nodes"
					: $"► List of Nodes";

				_isCardsFoldout = GUILayout.Toggle(_isCardsFoldout, caption, style);
			}
			GUILayout.EndHorizontal();

			if (!_isCardsFoldout)
			{
				return;
			}
			
			if (_nodes.Count > 0)
			{
				var c = GUI.color;
				for (int i = 0, n = _nodes.Count; i < n; i++)
				{
					GUILayout.BeginHorizontal(EditorStyles.toolbar);
					{
						GUI.color = _nodes[i].Color;

						int limit = (_delNode.index == i) ? 18 : AntAIEditorStyle.CardTitleLimit;
						var title = _nodes[i].Title;
						if (title.Length > limit)
						{
							title = $"{title.Substring(0, limit - 3)}...";
						}
						
						if (GUILayout.Button(title, style))
						{
							for (int j = 0, nj = _nodes.Count; j < nj; j++)
							{
								_nodes[j].IsSelected = false;
							}

							_nodes[i].IsSelected = true;
							LerpToNode(i);
						}

						GUI.color = c;
						if (_nodes[i] is AntAIWorldStateNode)
						{
							var node = _nodes[i] as AntAIWorldStateNode;
							node.WorldState.isAutoUpdate = GUILayout.Toggle(node.WorldState.isAutoUpdate, "UPD", EditorStyles.toolbarButton, GUILayout.MaxWidth(40.0f));
						}

						if (!(_nodes[i] is AntAIActionStateNode))
						{
							if (_delNode.index == i)
							{
								GUI.color = AntAIEditorStyle.Red;
								if (GUILayout.Button("Delete?", EditorStyles.toolbarButton, GUILayout.MaxWidth(70.0f)))
								{
									_delNode.confirm = true;
								}

								GUI.color = AntAIEditorStyle.Green;
								if (GUILayout.Button("×", EditorStyles.toolbarButton, GUILayout.MaxWidth(18.0f)))
								{
									_delNode.index = -1;
								}
								GUI.color = c;
							}
							else
							{
								if (GUILayout.Button("×", EditorStyles.toolbarButton, GUILayout.MaxWidth(18.0f)))
								{
									_delNode.index = i;
								}
							}
						}
					}
					GUILayout.EndHorizontal();
				}

				if (_delNode.index > -1 && _delNode.confirm)
				{
					DeleteNodeAt(_delNode.index);
					_delNode.index = -1;
					_delNode.confirm = false;
				}
			}
		}

		private void LerpToNode(int aIndex)
		{
			if (_current != null && aIndex >= 0 && aIndex < _nodes.Count)
			{
				_isLerpToPos = true;
				_counter = 0.0f;

				AntAIBaseNode selectedNode = _nodes[aIndex];
				Vector2 offset = new Vector2(position.width * 0.5f, position.height * 0.5f);
				_targetPosition = selectedNode.rect.position - offset;
			}
		}

		private GUIStyle CreateNodeStyle(string aTextureName)
		{
			var style = new GUIStyle();
			style.normal.background = (EditorGUIUtility.isProSkin)
				? (Texture2D) EditorGUIUtility.Load($"builtin skins/darkskin/images/{aTextureName}")
				: (Texture2D) EditorGUIUtility.Load($"builtin skins/lightskin/images/{aTextureName}");
			style.border = new RectOffset(12, 12, 12, 12);
			style.richText = true;
			style.fontStyle = FontStyle.Bold;
			style.padding = new RectOffset(12, 0, 10, 0);
			style.normal.textColor = new Color(0.639f, 0.65f, 0.678f);
			return style;
		}

		private void SetCurrentScenario(AntAIScenario aScenario)
		{
			_current = aScenario;
			ClearNodes();

			if (_current == null)
			{
				return;
			}
			
			FixPositionHelper.Fix(_current);

			// Add All Actions.
			for (int i = 0, n = _current.actions.Length; i < n; i++)
			{
				_nodes.Add(CreateActionNode(_current, _current.actions[i]));
			}

			// Add All Goals.
			for (int i = 0, n = _current.goals.Length; i < n; i++)
			{
				_nodes.Add(CreateGoalNode(_current, _current.goals[i]));
			}

			// Add All World States.
			for (int i = 0, n = _current.worldStates.Length; i < n; i++)
			{
				_nodes.Add(CreateWorldStateNode(_current, _current.worldStates[i]));
			}
		}

		private void ClearNodes()
		{
			for (int i = 0, n = _nodes.Count; i < n; i++)
			{
				if (_nodes[i] is AntAIActionNode)
				{
					(_nodes[i] as AntAIActionNode).EventDelete -= DeleteActionHandler;
				}
				else if (_nodes[i] is AntAIGoalNode)
				{
					(_nodes[i] as AntAIGoalNode).EventDelete -= DeleteGoalHandler;	
				}
			}

			_nodes.Clear();
		}

		private AntAIWorldStateNode CreateWorldStateNode(AntAIScenario aScenario, AntAIWorldState aWorldState)
		{
			var node = new AntAIWorldStateNode(aWorldState.position, 200.0f, 300.0f, _actionStyle, _activeActionStyle);
			node.EventDelete += DeleteWorldStateHandler;
			node.EventBuildPlan += BuildPlanHandler;
			node.EventClearPlan += ClearPlanHandler;
			node.title = "WORLD STATE";
			node.Scenario = aScenario;
			node.WorldState = aWorldState;
			return node;
		}

		private AntAIGoalNode CreateGoalNode(AntAIScenario aScenario, AntAIScenarioGoal aGoal)
		{
			var node = new AntAIGoalNode(aGoal.position, 200.0f, 300.0f, _nodeStyle, _activeNodeStyle);
			node.EventDelete += DeleteGoalHandler;
			node.title = "★ {0}";
			node.Scenario = aScenario;
			node.Goal = aGoal;
			return node;
		}

		private AntAIActionNode CreateActionNode(AntAIScenario aScenario, AntAIScenarioAction aAction)
		{
			var node = new AntAIActionNode(aAction.position, 200.0f, 300.0f, _nodeStyle, _activeNodeStyle);
			node.EventDelete += DeleteActionHandler;
			node.title = "● {0}";
			node.Scenario = aScenario;
			node.Action = aAction;
			return node;
		}

		private void DrawNodes(List<AntAIBaseNode> aNodes)
		{
			for (int i = aNodes.Count - 1; i >= 0; i--)
			{
				aNodes[i].Draw();
			}
		}

		private void ClearPlan()
		{
			for (int i = _nodes.Count - 1; i >= 0; i--)
			{
				if (_nodes[i] is AntAIActionStateNode)
				{
					_nodes[i].links.Clear();
					_nodes.RemoveAt(i);
				}
				else if (_nodes[i] is AntAIWorldStateNode)
				{
					_nodes[i].links.Clear();
				}
			}
		}

	#endregion

	#region Event Handlers

		public void SelectPresetHandler(object aPresetName)
		{
			AntAIScenario selectedPreset = System.Array.Find(_scenarios, x => x.name.Equals(aPresetName.ToString()));
			_current = selectedPreset;
			SetCurrentScenario(selectedPreset);
		}

		private void AddActionHandler()
		{
			var action = new AntAIScenarioAction();
			action.position = _spawnPosition;
			AntArray.Add(ref _current.actions, action);
			_nodes.Add(CreateActionNode(_current, _current.actions[_current.actions.Length - 1]));
			EditorUtility.SetDirty(_current);
		}

		private void AddGoalHandler()
		{
			var goal = new AntAIScenarioGoal();
			goal.position = _spawnPosition;
			AntArray.Add(ref _current.goals, goal);
			_nodes.Add(CreateGoalNode(_current, _current.goals[_current.goals.Length - 1]));
			EditorUtility.SetDirty(_current);
		}

		private void AddWorldStateHandler()
		{
			var worldState = new AntAIWorldState();
			worldState.position = _spawnPosition;
			AntArray.Add(ref _current.worldStates, worldState);
			_nodes.Add(CreateWorldStateNode(_current, _current.worldStates[_current.worldStates.Length - 1]));
			EditorUtility.SetDirty(_current);
		}

		private void DeleteNodeAt(int aIndex)
		{
			if (_nodes[_delNode.index] is AntAIActionNode)
			{
				DeleteActionHandler(_nodes[_delNode.index] as AntAIActionNode);
			}
			else if (_nodes[_delNode.index] is AntAIGoalNode)
			{
				DeleteGoalHandler(_nodes[_delNode.index] as AntAIGoalNode);
			}
			else if (_nodes[_delNode.index] is AntAIWorldStateNode)
			{
				DeleteWorldStateHandler(_nodes[_delNode.index] as AntAIWorldStateNode);
			}
		}

		private void DeleteWorldStateHandler(AntAIWorldStateNode aNode)
		{
			ClearPlan();

			int index = Array.IndexOf(_current.worldStates, aNode.WorldState);
			if (index >= 0 && index < _current.worldStates.Length)
			{
				AntArray.RemoveAt(ref _current.worldStates, index);
			}

			aNode.EventDelete -= DeleteWorldStateHandler;
			aNode.EventBuildPlan -= BuildPlanHandler;
			_nodes.Remove(aNode);
			EditorUtility.SetDirty(_current);
		}

		private void DeleteGoalHandler(AntAIGoalNode aNode)
		{
			int index = Array.IndexOf(_current.goals, aNode.Goal);
			if (index >= 0 && index < _current.goals.Length)
			{
				AntArray.RemoveAt(ref _current.goals, index);
			}

			aNode.EventDelete -= DeleteGoalHandler;
			_nodes.Remove(aNode);
			EditorUtility.SetDirty(_current);
		}

		private void DeleteActionHandler(AntAIActionNode aNode)
		{
			int index = Array.IndexOf(_current.actions, aNode.Action);
			if (index >= 0 && index < _current.actions.Length)
			{
				AntArray.RemoveAt(ref _current.actions, index);
			}

			aNode.EventDelete -= DeleteActionHandler;
			_nodes.Remove(aNode);
			EditorUtility.SetDirty(_current);
		}

		private void ClearPlanHandler(AntAIWorldStateNode aNode)
		{
			ClearPlan();
		}

		private void BuildPlanHandler(AntAIWorldStateNode aNode, AntAIPlan aPlan, AntAIPlanner aPlanner)
		{
			ClearPlan();

			AntAIAction action;
			AntAICondition curConditions = aPlanner.DebugConditions;
			AntAICondition preConditions;
			AntAIActionStateNode node;
			Vector2 pos = aNode.Position;

			aNode.links.Clear();
			AntAIBaseNode prevNode = aNode;
			for (int i = 0, n = aPlan.Count; i < n; i++)
			{
				action = aPlanner.GetAction(aPlan[i]);
				preConditions = curConditions.Clone();
				curConditions.Act(action.post);

				pos.x += 220.0f;
				if (i + 1 == aPlan.Count)
				{
					if (aPlan.isSuccess)
					{
						node = new AntAIActionStateNode(pos, 200.0f, 300.0f, _successStyle, _activeSuccessStyle);
						node.Color = AntAIEditorStyle.CardGreen;
					}
					else
					{
						node = new AntAIActionStateNode(pos, 200.0f, 300.0f, _failStyle, _activeFailStyle);
						node.Color = AntAIEditorStyle.CardRed;
					}
				}
				else
				{
					node = new AntAIActionStateNode(pos, 200.0f, 300.0f, _nodeStyle, _activeNodeStyle);
				}

				node.BindData(string.Concat((i + 1).ToString(), ". ", action.name), aPlanner, curConditions, preConditions);
				_nodes.Add(node);

				prevNode.links.Add(_nodes[_nodes.Count - 1]);
				prevNode = _nodes[_nodes.Count - 1];
			}
		}

		private void AlignToGridHandler()
		{
			_alignToGrid = !_alignToGrid;
		}

		private void CopyConditionsAsEnum()
		{
			var str = string.Format("public enum {0}", (_current != null) ? _current.name : "AICondition");
			str = string.Concat(str, "\n{\n");
			string conditionName = string.Empty;
			for (int i = 0, n = _current.conditions.list.Length; i < n; i++)
			{
				conditionName = _current.conditions.list[i].name.RemoveSpaces();
				str = string.Concat(str, $"\t{conditionName} = {_current.conditions.list[i].id}");
				str = (i + 1 == _current.conditions.list.Length)
					? string.Concat(str, "\n}")
					: string.Concat(str, ",\n");
			}

			var te = new TextEditor();
			te.text = str;
			te.SelectAll();
			te.Copy();

			EditorUtility.DisplayDialog("Copied!", "All conditions copied to the clipboard as Enum.", "Ok");
		}

	#endregion
	}
}