namespace Tanks
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using Anthill.Utils;

	public class NavMesh : MonoBehaviour
	{
		[Serializable]
		public struct Node
		{
			public int[] edges;
			
			[NonSerialized] 
			public Vector2 position;

			[NonSerialized] 
			public int[] neighbors;

			[NonSerialized]
			public int parent;

			[NonSerialized]
			public float heuristic;

			[NonSerialized]
			public float cost;

			[NonSerialized]
			public float sum;

			public Node(int aEdgeA, int aEdgeB, int aEdgeC)
			{
				edges = new [] { aEdgeA, aEdgeB, aEdgeC };
				position = Vector2.zero;
				neighbors = new int[] { -1, -1, -1 };
				parent = -1;
				heuristic = 0.0f;
				cost = 0.0f;
				sum = 0.0f;
			}

			public void AddNeighbor(int aNodeIndex)
			{
				if (neighbors == null)
				{
					neighbors = new int[] { -1, -1, -1 };
				}

				for (int i = 0; i < 3; i++)
				{
					if (neighbors[i] == -1 || neighbors[i] == aNodeIndex)
					{
						neighbors[i] = aNodeIndex;
						break;
					}
				}
			}

			public void RemoveNeighbor(int aNodeIndex)
			{
				for (int i = 0; i < 3; i++)
				{
					if (neighbors[i] == aNodeIndex)
					{
						neighbors[i] = -1;
						break;
					}
				}
			}

			public bool HasEdge(int aEdgeIndex)
			{
				return (System.Array.IndexOf(edges, aEdgeIndex) > -1);
			}

			public void ReplaceEdge(int aOldEdgeIndex, int aNewEdgeIndex)
			{
				for (int i = 0, n = edges.Length; i < n; i++)
				{
					if (edges[i] == aOldEdgeIndex)
					{
						edges[i] = aNewEdgeIndex;
					}
				}
			}

			public float Heuristic(Vector2 aGoal)
			{
				// By Distance but it's not working so good as we want.
				// float dx = aGoal.x - position.x;
				// float dy = aGoal.y - position.y;
				// return Mathf.Sqrt(dx * dx + dy * dy);

				// MARK: I have no idea how to calc heuristic for the nodes,
				//       so patfinder preffer big nodes beacuse it will be look
				//       as short way. Therefore try to make triangles / nodes of 
				//       approximately the same size and do not allow too small 
				//       or too large.
				return 1.0f;
			}
		}

		[Serializable]
		public struct Edge
		{
			public int a;
			public int b;
			public int nodeA;
			public int nodeB;

			public Edge(int aPointA, int aPointB)
			{
				a = aPointA;
				b = aPointB;
				nodeA = -1;
				nodeB = -1;
			}

			public void AddNeighbor(int aNodeIndex)
			{
				if (nodeA == -1 || nodeA == aNodeIndex)
				{
					nodeA = aNodeIndex;
				}
				else if (nodeB == -1 || nodeB == aNodeIndex)
				{
					nodeB = aNodeIndex;
				}
			}

			public void RemoveNeighbor(int aNodeIndex)
			{
				if (nodeA == aNodeIndex)
				{
					nodeA = -1;
				}
				else if (nodeB == aNodeIndex)
				{
					nodeB = -1;
				}
			}

			public bool HasNeigbors
			{
				get { return (nodeA > -1 && nodeB > -1); }
			}
		}

		#region Variables

		public static NavMesh Current { get; private set; }

		[HideInInspector]
		// [Tooltip("Enables editor mode for the navigation mesh.")]
		public bool editMesh = false;

		[HideInInspector]
		// [Tooltip("Starting point for debug search.")]
		public Vector2 fromPoint = Vector2.zero;

		[HideInInspector]
		// [Tooltip("End point for debug search.")]
		public Vector2 toPoint = Vector2.zero;

		[HideInInspector]
		public List<Vector2> vertices = new List<Vector2>();

		[HideInInspector]
		public List<Edge> edges = new List<Edge>();

		[HideInInspector]
		public List<Node> nodes = new List<Node>();

		private List<Vector2> _foundedWay = new List<Vector2>();
		private List<Vector2> _routeA = new List<Vector2>();
		private List<Vector2> _routeB = new List<Vector2>();

		#endregion
		#region Unity Calls
		
		private void Awake()
		{
			Current = this;
			// Initialize additional metadata for nodes.
			for (int i = 0, n = nodes.Count; i < n; i++)
			{
				var node = nodes[i];
				node.position = GetNodeCenter(i);
				for (int j = 0; j < 3; j++)
				{
					if (edges[node.edges[j]].nodeA != i)
					{
						node.AddNeighbor(edges[node.edges[j]].nodeA);
					}
					else if (edges[node.edges[j]].nodeB != i)
					{
						node.AddNeighbor(edges[node.edges[j]].nodeB);
					}
				}
				nodes[i] = node;
			}
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (Application.isPlaying)
			{
				// Draw all edges (nodes).
				// -----------------
				Gizmos.matrix = transform.localToWorldMatrix;
				for (int i = 0, n = edges.Count; i < n; i++)
				{
					Gizmos.color = (edges[i].HasNeigbors)
						? Color.grey
						: Color.green;

					Gizmos.DrawLine(
						vertices[edges[i].a], 
						vertices[edges[i].b]
					);
				}

				// Draw links between nodes.
				// -------------------------
				// Gizmos.color = Color.blue;
				// for (int i = 0, n = nodes.Count; i < n; i++)
				// {
				// 	for (int j = 0; j < 3; j++)
				// 	{
				// 		if (nodes[i].neighbors[j] != -1)
				// 		{
				// 			Gizmos.DrawLine(nodes[i].position, nodes[nodes[i].neighbors[j]].position);
				// 		}
				// 	}
				// }

				// Draw founded way.
				// -----------------
				if (_foundedWay.Count > 0)
				{
					Gizmos.color = Color.red;
					Vector2 prev = _foundedWay[0];
					for (int i = 1, n = _foundedWay.Count; i < n; i++)
					{
						Gizmos.DrawLine(prev, _foundedWay[i]);
						prev = _foundedWay[i];
					}
				}

				// Draw route.
				// -----------
				if (_routeA.Count > 0)
				{
					Gizmos.color = Color.yellow;
					Vector2 prev = _routeA[0];
					for (int i = 1, n = _routeA.Count; i < n; i++)
					{
						Gizmos.DrawLine(prev, _routeA[i]);
						prev = _routeA[i];
					}
				}

				if (_routeB.Count > 0)
				{
					Gizmos.color = Color.white;
					Vector2 prev = _routeB[0];
					for (int i = 1, n = _routeB.Count; i < n; i++)
					{
						DrawCircle(_routeB[i], 0.1f, 4);
						Gizmos.DrawLine(prev, _routeB[i]);
						prev = _routeB[i];
					}
				}
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
				Gizmos.DrawLine(prev, cur);
				prev = cur;
			}

			Gizmos.DrawLine(prev, first);
		}
#endif
		
		#endregion
		#region Public Methods

		public bool FindWay(Vector2 aFromPoint, Vector2 aToPoint, ref List<Vector2> aRoute)
		{
			_foundedWay.Clear();
			int fromNode = FindNodeByPoint(aFromPoint - (Vector2) transform.position);
			int toNode = FindNodeByPoint(aToPoint - (Vector2) transform.position);

			fromNode = (fromNode == -1)
				? FindNearNode(aFromPoint)
				: fromNode;

			toNode = (toNode == -1)
				? FindNearNode(aToPoint)
				: toNode;

			if (fromNode == -1 || toNode == -1)
			{
				return false;
			}

			var way = FindWay(fromNode, toNode);
			if (way != null)
			{
				BuildRoute(aFromPoint, aToPoint, way, ref aRoute);

				// Copy route to debug draw array.
				_routeB.Clear();
				for (int i = 0, n = aRoute.Count; i < n; i++)
				{
					_routeB.Add(aRoute[i]);
				}
				return true;
			}
			
			return false;
		}

		public List<int> FindWay(int aFromNode, int aToNode)
		{
			var opened = new List<int>();
			var closed = new List<int>();
			
			int currentIndex = aFromNode;
			var current = nodes[currentIndex];
			current.parent = -1;
			current.cost = 0.0f;
			current.heuristic = current.Heuristic(nodes[aToNode].position);
			current.sum = current.heuristic;
			nodes[currentIndex] = current;
			opened.Add(currentIndex);

			while (opened.Count > 0)
			{
				currentIndex = opened[0];
				current = nodes[currentIndex];
				for (int i = 0, n = opened.Count; i < n; i++)
				{
					if (nodes[opened[i]].sum < current.sum)
					{
						currentIndex = opened[i];
						current = nodes[currentIndex];
					}
				}

				opened.Remove(currentIndex);

				if (currentIndex == aToNode)
				{
					return BuildWay(currentIndex);
				}

				closed.Add(currentIndex);

				Node neighbor;
				int neighborIndex = -1;
				int openedIndex = -1;
				int closedIndex = -1;
				float cost = 0.0f;
				for (int i = 0, n = current.neighbors.Length; i < n; i++)
				{
					neighborIndex = current.neighbors[i];
					if (neighborIndex == -1) continue;

					neighbor = nodes[neighborIndex];
					cost = current.cost + neighbor.cost;

					openedIndex = opened.IndexOf(neighborIndex);
					closedIndex = closed.IndexOf(neighborIndex);

					if (openedIndex > -1 && cost < nodes[opened[openedIndex]].cost)
					{
						opened.RemoveAt(openedIndex);
						openedIndex = -1;
					}

					if (closedIndex > -1 && cost < nodes[closed[closedIndex]].cost)
					{
						closed.RemoveAt(closedIndex);
						closedIndex = -1;
					}

					if (openedIndex == -1 && closedIndex == -1)
					{
						neighbor.cost = cost;
						neighbor.heuristic = neighbor.Heuristic(nodes[aToNode].position);
						neighbor.sum = cost + neighbor.heuristic;
						neighbor.parent = currentIndex;
						nodes[neighborIndex] = neighbor;
						opened.Add(neighborIndex);
					}
				}
			}
			return null;
		}

		private List<int> BuildWay(int aFromNodeIndex)
		{
			int currentIndex = aFromNodeIndex;
			var result = new List<int>();
			result.Add(currentIndex);
			while (nodes[currentIndex].parent != -1)
			{
				currentIndex = nodes[currentIndex].parent;
				result.Add(currentIndex);
			}

			// TODO: Rework search that no need to reverse the result way.
			result.Reverse();
			return result;
		}
		
		private void BuildRoute(Vector2 aFrom, Vector2 aTo, List<int> aWay, ref List<Vector2> aRoute)
		{
			aRoute.Clear();
			aRoute.Add(aFrom);

			var currentPoint = aFrom;
			var nextPoint = Vector2.zero;
			Vector2 a, b, c, d, to;
			Node node;
			for (int i = 0, n = aWay.Count; i < n; i++)
			{
				node = nodes[aWay[i]];
				for (int j = 0; j < 3; j++)
				{
					// If edge has links to neighbors.
					if (edges[node.edges[j]].HasNeigbors)
					{
						// Extract edge points.
						a = vertices[edges[node.edges[j]].a];
						b = vertices[edges[node.edges[j]].b];

						// Expand the ray to the target.
						to = AntGeo.ExpandSegment(currentPoint, aTo, 100.0f);

						// Check crossing ray from current point to the target.
						if (AntGeo.LinesCross(currentPoint, to, a, b, out nextPoint))
						{
							AddPointToList(ref aRoute, nextPoint);
							currentPoint = nextPoint;
							break;
						}
						else
						{
							// If not found crossing, then expand edge to two sides.
							AntGeo.ExpandSegment(a, b, out c, out d, 50.0f);

							// Check ray to the target with expanded edge.
							if (AntGeo.LinesCross(currentPoint, to, c, d, out nextPoint))
							{
								// If found crossing point with expanded edge, find the near point of the edge to cross point.
								AntGeo.ExpandSegment(a, b, out c, out d, -0.1f);
								nextPoint = AntGeo.GetNearestPointFromSegment(nextPoint, c, d);

								AddPointToList(ref aRoute, nextPoint);
								currentPoint = nextPoint;
								break;
							}
							else
							{
								// If no found crossing, then try to cast ray from the center of current node.
								currentPoint = node.position;
								if (AntGeo.LinesCross(currentPoint, to, c, d, out nextPoint))
								{
									AntGeo.ExpandSegment(a, b, out c, out d, -0.1f);
									nextPoint = AntGeo.GetNearestPointFromSegment(nextPoint, c, d);

									AddPointToList(ref aRoute, nextPoint);
									currentPoint = nextPoint;
									break;
								}
							}
						}
					}
				}
			}

			// Add last point.
			aRoute.Add(aTo);

			// Debug route.
			_routeA.Clear();
			for (int i = 0, n = aRoute.Count; i < n; i++)
			{
				_routeA.Add(aRoute[i]);
			}
			
			// Optimize path to remove some points.
			int co = 3;
			while (co != 0)
			{
				co--;
				var delList = new List<int>();
				int currentIndex = 0;
				int nextIndex = 2;
				
				while (true)
				{
					if (nextIndex >= aRoute.Count)
					{
						break;
					}

					if (IsCrossAnyWall(aRoute[currentIndex], aRoute[nextIndex]))
					{
						currentIndex++;
						nextIndex++;
					}
					else
					{
						aRoute.RemoveAt(nextIndex - 1);
					}
				}
			}
		}

		private bool IsCrossAnyWall(Vector2 aA, Vector2 aB)
		{
			int count = 0;
			for (int i = 0, n = edges.Count; i < n; i++)
			{
				if (!edges[i].HasNeigbors)
				{
					if (AntGeo.LinesCross(aA, aB, vertices[edges[i].a], vertices[edges[i].b], true))
					{
						return true;
					}
				}
				else if (AntGeo.LinesCross(aA, aB, vertices[edges[i].a], vertices[edges[i].b], true))
				{
					count++;
				}
			}
			return !(count > 0);
		}

		private void AddPointToList(ref List<Vector2> aList, Vector2 aPoint)
		{
			for (int i = 0, n = aList.Count; i < n; i++)
			{
				if (AntMath.Equal(aList[i], aPoint))
				{
					// This point already exists in the list, skip.
					return;
				}
			}

			aList.Add(aPoint);
		}

		public int FindNodeByPoint(Vector2 aPoint)
		{
			var v = new Vector2[3];
			for (int i = 0, n = nodes.Count; i < n; i++)
			{
				v[0] = vertices[edges[nodes[i].edges[0]].a];
				v[1] = vertices[edges[nodes[i].edges[0]].b];
				v[2] = vertices[edges[nodes[i].edges[2]].a];
				if (AntGeo.IsPointInConvexPolygon(ref v, aPoint))
				{
					return i;
				}
			}
			return -1;
		}

		public int FindNearNode(Vector2 aPoint)
		{
			float dist = 0.0f;
			float minDistance = float.MaxValue;
			int minIndex = -1;
			for (int i = 0, n = nodes.Count; i < n; i++)
			{
				dist = AntMath.Distance(aPoint, nodes[i].position);
				if (dist < minDistance)
				{
					minDistance = dist;
					minIndex = i;
				}
			}
			return minIndex;
		}

		public int GetNearEdge(Vector2 aPosition)
		{
			float dist;
			float minValue = float.MaxValue;
			int minIndex = -1;
			Vector2 edgePos = Vector2.zero;
			Vector2 a, b;
			for (int i = 0, n = edges.Count; i < n; i++)
			{
				if (!edges[i].HasNeigbors)
				{
					a = vertices[edges[i].a];
					b = vertices[edges[i].b];
					edgePos.x = (a.x + b.x) * 0.5f;
					edgePos.y = (a.y + b.y) * 0.5f;
					dist = AntMath.Distance(aPosition, edgePos);
					if (dist < minValue)
					{
						minValue = dist;
						minIndex = i;
					}
				}
			}
			return minIndex;
		}

		public Vector2 GetNodeCenter(int aNodeIndex)
		{
			var a = vertices[edges[nodes[aNodeIndex].edges[0]].a];
			var b = vertices[edges[nodes[aNodeIndex].edges[0]].b];
			var toA = new Vector2((a.x + b.x) * 0.5f, (a.y + b.y) * 0.5f);

			a = vertices[edges[nodes[aNodeIndex].edges[2]].a];
			b = vertices[edges[nodes[aNodeIndex].edges[2]].b];
			var toB = new Vector2((a.x + b.x) * 0.5f, (a.y + b.y) * 0.5f);

			a = vertices[edges[nodes[aNodeIndex].edges[1]].a];
			b = vertices[edges[nodes[aNodeIndex].edges[1]].b];

			var result = Vector2.zero;
			if (!AntGeo.LinesCross(b, toA, a, toB, out result))
			{
				AntGeo.LinesCross(a, toA, b, toB, out result);
			}
			return result;
		}
		
		#endregion
	}
}