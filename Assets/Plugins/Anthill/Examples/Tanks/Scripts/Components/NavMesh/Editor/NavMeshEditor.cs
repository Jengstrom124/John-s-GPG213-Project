namespace Tanks
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using Anthill.Utils;

	[CustomEditor(typeof(NavMesh))]
	public class NavMeshEditor : Editor
	{
		#region Variables
		
		private NavMesh _self;
		private List<Vector2> _from;
		private List<Vector2> _to;
		
		#endregion
		#region Unity Calls

		private void OnEnable()
		{
			_self = (NavMesh) target;
			_from = new List<Vector2>();
			_to = new List<Vector2>();
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			{
				_self.editMesh = EditorGUILayout.BeginToggleGroup("Edit NavMesh", _self.editMesh);
				if (_self.editMesh)
				{
					EditorGUILayout.HelpBox("1. Hold SHIFT key to add new triangles;\n2. Hold CTRL key to remove triangles;\n3. Hold CMD key to link different triangles (click and drag yellow rect);", MessageType.Info);
					GUI.enabled = !Application.isPlaying;
					if (GUILayout.Button("Clear NavMesh"))
					{
						if (EditorUtility.DisplayDialog("Resetting", "Reset the all nodes?", "Yes", "No"))
						{
							Clear();

							var vertices = new int[3];
							vertices[0] = AddVertex(0.0f, 0.5f);
							vertices[1] = AddVertex(-0.5f, -0.5f);
							vertices[2] = AddVertex(0.5f, -0.5f);

							var edges = new int[3];
							edges[0] = AddEdge(vertices[0], vertices[1]);
							edges[1] = AddEdge(vertices[1], vertices[2]);
							edges[2] = AddEdge(vertices[2], vertices[0]);

							AddNode(edges[0], edges[1], edges[2]);

							EditorUtility.SetDirty(target);
						}
					}
				}
				EditorGUILayout.EndToggleGroup();

			}
			EditorGUILayout.EndVertical();

			GUILayout.Space(10.0f);
			EditorGUILayout.LabelField("Debug Section", EditorStyles.boldLabel);
			_self.fromPoint = EditorGUILayout.Vector2Field("Start Point", _self.fromPoint);
			_self.toPoint = EditorGUILayout.Vector2Field("End Point", _self.toPoint);

			GUI.enabled = Application.isPlaying;
			if (GUILayout.Button("Find Way"))
			{
				var res = new List<Vector2>();
				_self.FindWay((Vector2) _self.fromPoint, (Vector2) _self.toPoint, ref res);
			}
			GUI.enabled = true;
		}

		private int FindNearVertex(int aVertexIndex, float aRadius)
		{
			float dist;
			for (int i = 0, n = _self.vertices.Count; i < n; i++)
			{
				if (aVertexIndex != i)
				{
					dist = AntMath.Distance(_self.vertices[aVertexIndex], _self.vertices[i]);
					if (dist < aRadius)
					{
						return i;
					}
				}
			}
			return -1;
		}

		private void OnSceneGUI()
		{
			bool isDirty = false;
			// bool isRemovePressed = ((Event.current.modifiers & (EventModifiers.Command | EventModifiers.Control)) != 0);
			bool isLinkPressed = ((Event.current.modifiers & EventModifiers.Command) != 0);
			bool isRemovePressed = ((Event.current.modifiers & EventModifiers.Control) != 0);
			bool isAddPressed = ((Event.current.modifiers & EventModifiers.Shift) != 0);

			// DEBUG POINTS.
			// -------------

			// From Point.
			Handles.color = Color.blue;
			var delta_d = DotHandle(_self.fromPoint) - _self.fromPoint;
			if (delta_d != Vector2.zero)
			{
				_self.fromPoint += delta_d;
			}

			// To Point.
			Handles.color = Color.red;
			delta_d = DotHandle(_self.toPoint) - _self.toPoint;
			if (delta_d != Vector2.zero)
			{
				_self.toPoint += delta_d;
			}
			
			// Merge nodes.
			// ------------
			if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
			{
				MergeNodes();
			}

			Handles.matrix = _self.transform.localToWorldMatrix;
			DrawMesh();

			if (!_self.editMesh) return;

			// Is pressed hotkey for linking nodes (command key).
			// --------------------------------------------------
			if (isLinkPressed)
			{
				if (_from.Count != _self.nodes.Count)
				{
					_from.Clear();
					_to.Clear();
					for (int i = 0, n = _self.nodes.Count; i < n; i++)
					{
						_from.Add(_self.GetNodeCenter(i));
						_to.Add(_from[_from.Count - 1]);
					}
				}

				Handles.color = Color.yellow;
				for (int i = 0, n = _to.Count; i < n; i++)
				{
					var p = _to[i];
					var delta = DotHandle(p) - p;
					if (delta != Vector2.zero)
					{
						_to[i] = p + delta;
					}

					if (!AntMath.Equal(_to[i], _from[i]))
					{
						Handles.DrawLine(_from[i], _to[i]);
						int nodeIndex = _self.FindNodeByPoint(_to[i]);
						if (nodeIndex > -1)
						{
							var edges = new int[2];
							var center = _self.GetNodeCenter(nodeIndex);
							DrawLinks(nodeIndex);
							if (IsPossibleMergeTriangles(_from[i], center, ref edges))
							{
								Handles.color = Color.green;
								var a = _self.vertices[_self.edges[edges[0]].a];
								var b = _self.vertices[_self.edges[edges[0]].b];
								var c = _self.vertices[_self.edges[edges[1]].a];
								var d = _self.vertices[_self.edges[edges[1]].b];
								if (!DrawEdgesIfNotCrossing(a, c, b, d))
								{
									if (!DrawEdgesIfNotCrossing(b, c, a, d))
									{
										if (!DrawEdgesIfNotCrossing(b, d, a, c))
										{
											DrawEdgesIfNotCrossing(a, d, b, c);
										}
									}
								}
								Handles.color = Color.yellow;
							}
						}
					}
				}
			}

			// If pressed hotkey for removing nodes (triangles).
			// -------------------------------------------------
			else if (isRemovePressed)
			{
				Handles.color = Color.red;
				Vector2 center;
				for (int i = 0, n = _self.nodes.Count; i < n; i++)
				{
					center = _self.GetNodeCenter(i);
					if (Handles.Button(center, Quaternion.identity, 0.05f, 0.05f, Handles.DotHandleCap))
					{
						RemoveNode(i);
						isDirty = true;
						break;
					}
				}
			}

			// Pressed hotkey for adding new nodes (triangles).
			// ------------------------------------------------
			else if (isAddPressed)
			{
				var mouse = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
				var p = _self.transform.InverseTransformPoint(mouse);
				// var pos = (Vector2) _self.transform.position;

				Handles.color = Color.white;
				int nearEdge = _self.GetNearEdge(p);
				if (nearEdge > -1)
				{
					Handles.DrawDottedLine(_self.vertices[_self.edges[nearEdge].a], p, 1.0f);
					Handles.DrawDottedLine(_self.vertices[_self.edges[nearEdge].b], p, 1.0f);

					if (Handles.Button(p, Quaternion.identity, 0.05f, 0.05f, Handles.DotHandleCap))
					{
						var newVertex = AddVertex(p.x, p.y);
						
						var edges = new int[2];
						edges[0] = AddEdge(_self.edges[nearEdge].a, newVertex);
						edges[1] = AddEdge(newVertex, _self.edges[nearEdge].b);

						var newNode = AddNode(nearEdge, edges[0], edges[1]);
						var e = _self.edges[nearEdge];
						e.AddNeighbor(newNode);
						_self.edges[nearEdge] = e;

						isDirty = true;
					}

					// Handles.color = Color.white;
					// float s = HandleUtility.GetHandleSize(p) * 0.05f;
					// for (int i = 0, n = _self.vertices.Count; i < n; i++)
					// {
					// 	Handles.DotHandleCap(0, _self.vertices[i], Quaternion.identity, s, EventType.DragUpdated);
					// }
				}
			}

			// Simple editing the NavMesh.
			// ---------------------------
			else
			{
				Handles.color = Color.white;
				for (int i = 0, n = _self.vertices.Count; i < n; i++)
				{
					var p = _self.vertices[i];
					var delta = DotHandle(p) - p;
					if (delta != Vector2.zero)
					{
						_self.vertices[i] = p + delta;
						isDirty = true;
					}
				}

				// float dist;
				// float ang;
				// Vector2 pa, pb;
				// Vector2 center = Vector2.zero;
				// Handles.color = Color.green;
				// for (int i = 0, n = _self.edges.Count; i < n; i++)
				// {
				// 	// if (_self.edges[i].hasNeigbors)
				// 	// 	continue;

				// 	pa = _self.vertices[_self.edges[i].a];
				// 	pb = _self.vertices[_self.edges[i].b];
				// 	dist = AntMath.Distance(pa, pb) * 0.5f;
				// 	ang = AntMath.AngleRad(pa, pb) - 90.0f * Mathf.Deg2Rad;
				// 	center = new Vector2((pa.x + pb.x) * 0.5f, (pa.y + pb.y) * 0.5f);
				// 	if (Handles.Button(center, Quaternion.identity, 0.05f, 0.05f, Handles.DotHandleCap))
				// 	{
				// 		// AntGeo.ExpandSegment(pa, pb, ref _c, ref _d, 100.0f);
				// 		// AntLog.Trace(_self.edges[i].nodeA, _self.edges[i].nodeB, _self.edges[i].HasNeigbors);
				// 		var newVertex = _self.AddVertex(
				// 			center.x + dist * Mathf.Cos(ang), 
				// 			center.y + dist * Mathf.Sin(ang)
				// 		);
						
				// 		var edges = new int[2];
				// 		edges[0] = _self.AddEdge(_self.edges[i].a, newVertex);
				// 		edges[1] = _self.AddEdge(newVertex, _self.edges[i].b);

				// 		var newNode = _self.AddNode(i, edges[0], edges[1]);
				// 		var e = _self.edges[i];
				// 		e.AddNeighbor(newNode);
				// 		_self.edges[i] = e;

				// 		// var edge = _self.edges[i];
				// 		// _self.edges[i] = edge;
				// 		isDirty = true;
				// 	}
				// }
			}

			if (isDirty)
			{
				EditorUtility.SetDirty(target);
			}
		}

		#endregion
		#region Private Methods
		
		private void DrawMesh()
		{
			for (int i = 0, n = _self.edges.Count; i < n; i++)
			{
				Handles.color = (_self.edges[i].HasNeigbors)
					? Color.grey
					: Color.green;

				Handles.DrawLine(
					_self.vertices[_self.edges[i].a], 
					_self.vertices[_self.edges[i].b]
				);
			}

			// Handles.color = Color.yellow;
			// Handles.DrawDottedLine(_c, _d, 2.0f);

			// for (int i = 0, n = _self.nodes.Count; i < n; i++)
			// {
			// 	for (int j = 0, nj = _self.nodes[i].edges.Length; j < nj; j++)
			// 	{
			// 		var a = _self.vertices[_self.edges[_self.nodes[i].edges[j]].a];
			// 		var b = _self.vertices[_self.edges[_self.nodes[i].edges[j]].b];
			// 		Handles.DrawLine(a, b);
			// 	}
			// }

			// Handles.color = Color.red;
			// if (_notFoundEdges.Count == 0) return;
			// for (int i = 0, n = _notFoundEdges.Count; i < n; i++)
			// {
			// 	Handles.DrawLine(_notFoundEdges[i].a, _notFoundEdges[i].b);
			// }
		}

		private void DrawLinks(int aNodeIndex)
		{
			var p = _self.GetNodeCenter(aNodeIndex);
			int neighbor;
			for (int i = 0, n = _self.nodes[aNodeIndex].edges.Length; i < n; i++)
			{
				neighbor = _self.edges[_self.nodes[aNodeIndex].edges[i]].nodeA;
				if (neighbor > -1 && neighbor != aNodeIndex)
				{
					Handles.DrawDottedLine(p, _self.GetNodeCenter(neighbor), 2.0f);
				}

				neighbor = _self.edges[_self.nodes[aNodeIndex].edges[i]].nodeB;
				if (neighbor > -1 && neighbor != aNodeIndex)
				{
					Handles.DrawDottedLine(p, _self.GetNodeCenter(neighbor), 2.0f);
				}
			}

			for (int i = 0, n = _self.nodes[aNodeIndex].edges.Length; i < n; i++)
			{
				Handles.color = Color.white;
				Handles.DrawLine(
					_self.vertices[_self.edges[_self.nodes[aNodeIndex].edges[i]].a], 
					_self.vertices[_self.edges[_self.nodes[aNodeIndex].edges[i]].b]
				);
			}
		}

		private void DrawCircle(Vector2 aPoint, float aRadius, int aVertices)
		{
			var v = (aVertices >= 3) ? aVertices : 3;
			float dx = aRadius;
			const float dy = 0.0f;

			float angle = 0.0f;
			var first = new Vector2(
				aPoint.x + Mathf.Cos(angle) * dx - Mathf.Sin(angle) * dy,
				aPoint.y - (Mathf.Sin(angle) * dx + Mathf.Cos(angle) * dy)
			);

			var prev = first;
			Vector2 cur = Vector2.zero;
			for (int i = 0; i < v; i++)
			{
				angle = ((i / (float) v) * 360.0f) * Mathf.Deg2Rad;
				cur.x = aPoint.x + Mathf.Cos(angle) * dx - Mathf.Sin(angle) * dy;
				cur.y = aPoint.y - (Mathf.Sin(angle) * dx + Mathf.Cos(angle) * dy);
				Handles.DrawLine(prev, cur);
				prev = cur;
			}

			Handles.DrawLine(prev, first);
		}

		private bool DrawEdgesIfNotCrossing(Vector2 aA, Vector2 aB, Vector2 aC, Vector2 aD)
		{
			if (!AntGeo.LinesCross(aA, aB, aC, aD))
			{
				Handles.DrawDottedLine(aA, aB, 1.0f);
				Handles.DrawDottedLine(aC, aD, 1.0f);
				return true;
			}
			return false;
		}

		private void MergeNodes()
		{
			// _notFoundEdges.Clear();
			for (int i = 0, n = _from.Count; i < n; i++)
			{
				if (!AntMath.Equal(_to[i], _from[i]))
				{
					int fromNode = _self.FindNodeByPoint(_from[i]);
					int toNode = _self.FindNodeByPoint(_to[i]);
					if (toNode > -1)
					{
						var edges = new int[2];
						var center = _self.GetNodeCenter(toNode);
						if (IsPossibleMergeTriangles(_from[i], center, ref edges))
						{
							var a = _self.vertices[_self.edges[edges[0]].a];
							var b = _self.vertices[_self.edges[edges[0]].b];
							var c = _self.vertices[_self.edges[edges[1]].a];
							var d = _self.vertices[_self.edges[edges[1]].b];
							if (!MergeEdgesIfNotCrossing(a, c, b, d, fromNode, toNode))
							{
								if (!MergeEdgesIfNotCrossing(b, c, a, d, fromNode, toNode))
								{
									if (!MergeEdgesIfNotCrossing(b, d, a, c, fromNode, toNode))
									{
										MergeEdgesIfNotCrossing(a, d, b, c, fromNode, toNode);
									}
								}
							}
						}
					}
				}

				_to[i] = _from[i];
			}
		}

		private bool MergeEdgesIfNotCrossing(Vector2 aA, Vector2 aB, Vector2 aC, Vector2 aD, int aFrom, int aTo)
		{
			// Link two nodes only. (?)
			// if ((AntMath.Equal(aA, aC) && AntMath.Equal(aB, aD)) ||
			// 	(AntMath.Equal(aA, aD) && AntMath.Equal(aB, aC)))
			// {
			// 	AntLog.Trace("Merge two edges!");
			// 	return true;
			// }

			// Create one new node.
			// --------------------
			var v = new Vector2[] { aA, aB, aC, aD };
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					if (i != j && AntMath.Equal(v[i], v[j]))
					{
						// Remove shared vertex.
						AntArray.RemoveAt<Vector2>(ref v, i);

						var edges = new int[3];
						edges[0] = GetEdge(v[0], v[1]);
						edges[1] = GetEdge(v[1], v[2]);
						edges[2] = GetEdge(v[2], v[0]);

						var node = AddNode(edges[0], edges[1], edges[2]);
						LinkNodes(aFrom, node);
						LinkNodes(node, aTo);

						var center = _self.GetNodeCenter(node);
						var testNode = _self.FindNodeByPoint(center);
						if (testNode != node)
						{
							// The triangle turned inside out, recreate.
							RemoveNode(node);

							edges[0] = GetEdge(v[0], v[2]);
							edges[1] = GetEdge(v[2], v[1]);
							edges[2] = GetEdge(v[1], v[0]);

							node = AddNode(edges[0], edges[1], edges[2]);
							LinkNodes(aFrom, node);
							LinkNodes(node, aTo);

							center = _self.GetNodeCenter(node);
							testNode = _self.FindNodeByPoint(center);
							if (testNode != node)
							{
								// The triangle turned inside out, recreate.
								RemoveNode(node);

								edges[0] = GetEdge(v[1], v[2]);
								edges[1] = GetEdge(v[2], v[0]);
								edges[2] = GetEdge(v[0], v[1]);

								node = AddNode(edges[0], edges[1], edges[2]);
								LinkNodes(aFrom, node);
								LinkNodes(node, aTo);

								center = _self.GetNodeCenter(node);
								testNode = _self.FindNodeByPoint(center);
								if (testNode != node)
								{
									A.Warning("Wrong triangle! ({0})", node);
								}
							}
						}

						return true;
					}
				}
			}

			// Create two new nodes.
			// ---------------------
			if (!AntGeo.LinesCross(aA, aB, aC, aD))
			{
				var edges = new int[5];
				edges[0] = GetEdge(aA, aB);
				edges[1] = GetEdge(aB, aD);
				edges[2] = GetEdge(aD, aA);
				edges[3] = GetEdge(aA, aC);
				edges[4] = GetEdge(aC, aD);

				var nodeA = AddNode(edges[0], edges[1], edges[2]);
				var nodeB = AddNode(edges[3], edges[4], edges[2]);
				LinkNodes(aFrom, nodeA);
				LinkNodes(nodeA, nodeB);
				LinkNodes(nodeB, aTo);
				return true;
			}

			return false;
		}

		private void LinkNodes(int aFrom, int aTo)
		{
			Vector2 c, d;
			var a = _self.GetNodeCenter(aFrom);
			var b = _self.GetNodeCenter(aTo);
			for (int i = 0, n = _self.edges.Count; i < n; i++)
			{
				c = _self.vertices[_self.edges[i].a];
				d = _self.vertices[_self.edges[i].b];
				if (AntGeo.LinesCross(a, b, c, d))
				{
					var e = _self.edges[i];
					e.AddNeighbor(aFrom);
					e.AddNeighbor(aTo);
					_self.edges[i] = e;
					return;
				}
			}
		}

		private void Clear()
		{
			_self.vertices.Clear();
			_self.edges.Clear();
			_self.nodes.Clear();
		}

		private int AddNode(int aEdgeA, int aEdgeB, int aEdgeC)
		{
			_self.nodes.Add(new NavMesh.Node(aEdgeA, aEdgeB, aEdgeC));
			int index = _self.nodes.Count - 1;
			var e = _self.edges[aEdgeA];
			e.AddNeighbor(index);
			_self.edges[aEdgeA] = e;

			e = _self.edges[aEdgeB];
			e.AddNeighbor(index);
			_self.edges[aEdgeB] = e;

			e = _self.edges[aEdgeC];
			e.AddNeighbor(index);
			_self.edges[aEdgeC] = e;
			return index;
		}

		private void RemoveNode(int aNodeIndex)
		{
			var delEdges = new List<int>();
			for (int i = 0, n = _self.nodes[aNodeIndex].edges.Length; i < n; i++)
			{
				delEdges.Add(_self.nodes[aNodeIndex].edges[i]);
				var e = _self.edges[_self.nodes[aNodeIndex].edges[i]];
				e.RemoveNeighbor(aNodeIndex);
				_self.edges[_self.nodes[aNodeIndex].edges[i]] = e;
			}

			_self.nodes.RemoveAt(aNodeIndex);

			// Fix links to the nodes in edges.
			for (int i = 0, n = _self.edges.Count; i < n; i++)
			{
				var e = _self.edges[i];
				if (e.nodeA > aNodeIndex)
				{
					e.nodeA--;
				}
			
				if (e.nodeB > aNodeIndex)
				{
					e.nodeB--;
				}
				_self.edges[i] = e;
			}

			delEdges.Sort();
			for (int i = delEdges.Count - 1; i >= 0; i--)
			{
				RemoveEdge(delEdges[i]);
			}
		}

		private int GetEdge(Vector2 aA, Vector2 aB)
		{
			int index = FindEdge(aA, aB);
			
			if (index == -1)
			{
				index = FindEdge(aB, aA);
			}

			return (index == -1)
				? AddEdge(GetVertex(aA), GetVertex(aB))
				: index;
		}

		public int FindEdge(Vector2 aA, Vector2 aB)
		{
			for (int i = 0, n = _self.edges.Count; i < n; i++)
			{
				if (AntMath.Equal(_self.vertices[_self.edges[i].a], aA) && 
					AntMath.Equal(_self.vertices[_self.edges[i].b], aB))
				{
					return i;
				}
			}
			return -1;
		}

		private int AddEdge(int aPointA, int aPointB)
		{
			_self.edges.Add(new NavMesh.Edge(aPointA, aPointB));
			return _self.edges.Count - 1;
		}

		public void RemoveEdge(int aEdgeIndex)
		{
			for (int i = 0, n = _self.nodes.Count; i < n; i++)
			{
				if (_self.nodes[i].HasEdge(aEdgeIndex))
				{
					return;
				}
			}

			var e = _self.edges[aEdgeIndex];
			_self.edges.RemoveAt(aEdgeIndex);
			for (int i = 0, n = _self.nodes.Count; i < n; i++)
			{
				for (int j = 0, nj = _self.nodes[i].edges.Length; j < nj; j++)
				{
					if (_self.nodes[i].edges[j] > aEdgeIndex)
					{
						_self.nodes[i].edges[j]--;
					}
				}
			}
			
			if (e.a > e.b)
			{
				RemoveVertex(e.a);
				RemoveVertex(e.b);
			}
			else if (e.a < e.b)
			{
				RemoveVertex(e.b);
				RemoveVertex(e.a);
			}
		}

		private int GetVertex(Vector2 aA)
		{
			int index = FindVertex(aA);
			return (index == -1)
				? AddVertex(aA.x, aA.y) 
				: index;
		}

		private int FindVertex(Vector2 aPoint)
		{
			for (int i = 0, n = _self.vertices.Count; i < n; i++)
			{
				if (AntMath.Equal(_self.vertices[i], aPoint)) 
				{
					return i;
				}
			}
			return -1;
		}

		private int AddVertex(float aX, float aY)
		{
			_self.vertices.Add(new Vector2(aX, aY));
			return _self.vertices.Count - 1;
		}

		private void RemoveVertex(int aVertexIndex)
		{
			for (int i = 0, n = _self.edges.Count; i < n; i++)
			{
				if (_self.edges[i].a == aVertexIndex || _self.edges[i].b == aVertexIndex)
				{
					return;
				}
			}

			_self.vertices.RemoveAt(aVertexIndex);
			NavMesh.Edge e;
			for (int i = 0, n = _self.edges.Count; i < n; i++)
			{
				e = _self.edges[i];
				if (e.a > aVertexIndex) e.a--;
				if (e.b > aVertexIndex) e.b--;
				_self.edges[i] = e;
			}
		}

		private bool IsPossibleMergeTriangles(Vector2 aA, Vector2 aB, ref int[] aCrossedEdges)
		{
			Vector2 c, d;
			int count = 0;
			for (int i = 0, n = _self.edges.Count; i < n; i++)
			{
				c = _self.vertices[_self.edges[i].a];
				d = _self.vertices[_self.edges[i].b];
				if (AntGeo.LinesCross(aA, aB, c, d))
				{
					// Write crossed edge to the result array.
					if (count < aCrossedEdges.Length)
					{
						aCrossedEdges[count] = i;
					}
					count++;

					if (_self.edges[i].HasNeigbors)
					{
						// Crossed line with neighbors, can't to connect.
						return false;
					}
				}
			}
			return (count == 2);
		}

		private Vector2 DotHandle(Vector2 aPosition, float aSize = 0.05f)
		{
			float s = HandleUtility.GetHandleSize(aPosition) * aSize;
			return Handles.FreeMoveHandle(aPosition, Quaternion.identity, s, Vector3.zero, Handles.DotHandleCap);
		}
		
		#endregion
	}
}