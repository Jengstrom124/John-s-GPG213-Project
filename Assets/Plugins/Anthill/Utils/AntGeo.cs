namespace Anthill.Utils
{
	using UnityEngine;

	public static class AntGeo
	{
		/// <summary>
		/// Векторное произведение.
		/// </summary>
		/// <param name="aA"></param>
		/// <param name="aB"></param>
		/// <param name="aC"></param>
		/// <returns></returns>
		public static float CrossProd(Vector2 aA, Vector2 aB, Vector2 aC)
		{
			float x1 = aB.x - aA.x;
			float y1 = aB.y - aA.y;
			float x2 = aC.x - aA.x;
			float y2 = aC.y - aA.y;
			return (x1 * y2 - x2 * y1);
		}

		/// <summary>
		/// Скальярное произведение.
		/// </summary>
		/// <param name="aA"></param>
		/// <param name="aB"></param>
		/// <param name="aC"></param>
		/// <returns></returns>
		public static float ScalarProd(Vector2 aA, Vector2 aB, Vector2 aC)
		{
			float x1 = aB.x - aA.x;
			float y1 = aB.y - aA.y;
			float x2 = aC.x - aA.x;
			float y2 = aC.y - aA.y;
			return (x1 * x2 + y1 * y2);
		}

		/// <summary>
		/// Проверяет точка C лежит на ребре AB, если она лежит на одной прямой с AB (векторное
		/// произведение = и C лежит между A и B (скалярное произведение <= 0; = 0, 
		/// когда C совпадает с A или с B).
		/// </summary>
		/// <param name="aA">Первая вершина ребра.</param>
		/// <param name="aB">Вторая вершина ребра.</param>
		/// <param name="aC">Проверяемая точка.</param>
		/// <returns>Возвращает true если точка лежит на ребре.</returns>
		public static bool IsPointOnEdge(Vector2 aA, Vector2 aB, Vector2 aC)
		{
			float prod1 = CrossProd(aA, aB, aC);
			float prod2 = ScalarProd(aC, aA, aB);
			return (prod1 == 0 && prod2 <= 0);
		}

		/// <summary>
		/// Проверяет пересекаются ли отрезки AB и CD.
		/// </summary>
		/// <param name="aA">Вершина A первого отрезка.</param>
		/// <param name="aB">Вершина B первого отрезка.</param>
		/// <param name="aC">Вершина A второго отрезка.</param>
		/// <param name="aD">Вершина B второго отрезка.</param>
		/// <returns>Возвращает TRUE если отрезки пересекаются.</returns>
		public static bool IsEdgeCross(Vector2 aA, Vector2 aB, Vector2 aC, Vector2 aD)
		{
			float prod1 = CrossProd(aA, aB, aC);
			float prod2 = CrossProd(aA, aB, aD);
			float prod3 = CrossProd(aD, aC, aA);
			float prod4 = CrossProd(aD, aC, aB);

			// Отрезки AB и CD пересекаются, если C и D лежат по разные стороны от AB,
			// A и B лежат по разные стороны от CD.
			bool isCross = (prod1 * prod2 < 0 && prod3 * prod4 < 0);

			// Надо проверить, не принадлежат ли концы одного отрезка к другому,
			// в этом случае одно или несколько векторных произведений обратятся в ноль.
			bool cond1 = IsPointOnEdge(aA, aB, aC);
			bool cond2 = IsPointOnEdge(aA, aB, aD);
			bool cond3 = IsPointOnEdge(aC, aD, aA);
			bool cond4 = IsPointOnEdge(aC, aD, aB);

			// Проверка, что только один конец одного из отрезков принадлежит другому
			// таким образом, из рассмотрения исключаются ребра, лежащие на луче.
			bool cond1A = (cond1 && !(cond2 || cond3 || cond4));
			bool cond2A = (cond2 && !(cond1 || cond3 || cond4));
			bool cond3A = (cond3 && !(cond1 || cond2 || cond4));
			bool cond4A = (cond4 && !(cond1 || cond2 || cond3));

			bool isOneInsideOther = (cond1A || cond2A || cond3A || cond4A);

			return (isCross || isOneInsideOther);
		}

		/// <summary>
		/// Определяет находится ли точка внутри многоугольника.
		/// </summary>
		/// <param name="aPolygon">Вершины многоугольника.</param>
		/// <param name="aPoint">Проверяемая точка.</param>
		/// <param name="aRayLength">Длина луча для трассировки.</param>
		/// <returns>Возвращает true если точка находится в пределах многоугольника.</returns>
		public static bool IsPointInPolygon(ref Vector2[] aPolygon, Vector2 aPoint, int aRayLength = 1024)
		{
			// Вершины многоугольника.
			var first = Vector2.zero;
			var second = Vector2.zero;

			// Получение второй точки, лежащей на луче.
			var end = new Vector2(aRayLength, aPoint.y);

			// Кол-во пересечений с лучом.
			int count = 0;

			// Не передано вершин.
			if (aPolygon.Length == 0)
				return false;
			
			// Передана одна вершина (проверяем соотвествие вершины с указанной точкой).
			if (aPolygon.Length == 1)
				return (Mathf.Abs(aPolygon[0].x - aPoint.x) <= 0.00001f && Mathf.Abs(aPolygon[0].y - aPoint.y) <= 0.00001f);

			// Передан отрезок (проверяем лежит ли точка на заданном отрезке).
			if (aPolygon.Length == 2)
			{
				first.x = aPolygon[0].x;
				first.y = aPolygon[0].y;
				second.x = aPolygon[1].x;
				second.y = aPolygon[1].y;
				return IsPointOnEdge(first, second, aPoint);
			}

			// Передан как минимум треугольник.
			for (int i = 1, n = aPolygon.Length; i <= n; i++)
			{
				if (i < n)
				{
					first.x = aPolygon[i - 1].x;
					first.y = aPolygon[i - 1].y;
					second.x = aPolygon[i].x;
					second.y = aPolygon[i].y;
				}
				else
				{
					// Обрабатываем последнее ребро (последняя и первая вершины).
					second.x = aPolygon[0].x;
					second.y = aPolygon[0].y;
					first.x = aPolygon[aPolygon.Length - 1].x;
					first.y = aPolygon[aPolygon.Length - 1].y;
				}

				// Принадлежит ли точка ребру.
				if (IsPointOnEdge(first, second, aPoint))
				{
					// Точка на границе.
					return true; 
				}
				
				// Пересекается ли ребро с лучом (при этом исключаются ребра, лежащие на луче)
				if (IsEdgeCross(first, second, aPoint, end))
				{
					// Определяем нижнюю вершину по Y.
					// Ребро не лежит на одной прямой с горизонтальным лучом (из предыдущего условия)
					// Если выбрать не горизонтальный луч, надо дополнительно рассмотреть горизонтальные ребра.
					if ((first.y > second.y && second.y < aPoint.y) ||
						(first.y < second.y && first.y < aPoint.y))
					{
						// Нижний конец ребра, ниже луча.
						count++;
					}
				}
			}

			if (count % 2 > 0)
			{
				// Нечетное кол-во пересечений с горизонтальным лучом.
				return true;
			}
			return false;
		}

		/// <summary>
		/// Определяет находится ли точка внутри многоугольника (только выпуклые многоугольники).
		/// </summary>
		/// <param name="aPolygon"></param>
		/// <param name="aPoint"></param>
		/// <returns></returns>
		public static bool IsPointInConvexPolygon(ref Vector2[] aPolygon, Vector2 aPoint)
		{
			int n = aPolygon.Length;
			bool result = false;
			for (int i = 0, j = n - 1; i < n; j = i++)
			{
				if (((aPolygon[i].y > aPoint.y) != (aPolygon[j].y > aPoint.y)) &&
					(aPoint.x < (aPolygon[j].x - aPolygon[i].x) * (aPoint.y - aPolygon[i].y) / (aPolygon[j].y - aPolygon[i].y) + aPolygon[i].x))
				{
					result = !result;
				}
			}
			return result;
		}

		/// <summary>
		/// Возвращает центральную точку для грани.
		/// </summary>
		/// <param name="aA">Точка A.</param>
		/// <param name="aB">Точка B.</param>
		/// <returns>Возвращает центр между A и B.</returns>
		public static Vector2 HalfPointOfEdge(Vector2 aA, Vector2 aB)
		{
			return new Vector2((aA.x + aB.x) * 0.5f, (aA.y + aB.y) * 0.5f);
		}

		// public static bool LinesCross(float aAB_x1, float aAB_y1, float aAB_x2, float aAB_y2, float aCD_x1, float aCD_y1, float aCD_x2, float aCD_y2)
		// {
		// 	float d = (aAB_x2 - aAB_x1) * (aCD_y1 - aCD_y2) - (aCD_x1 - aCD_x2) * (aAB_y2 - aAB_y1);

		//  // Отрезки паралельны.
		// 	if (Equal(d, 0.0f)) return false;

		// 	float d1 = (aCD_x1 - aAB_x1) * (aCD_y1 - aCD_y2) - (aCD_x1 - aCD_x2) * (aCD_y1 - aAB_y1);
		// 	float d2 = (aAB_x2 - aAB_x1) * (aCD_y1 - aAB_y1) - (aCD_x1 - aAB_x1) * (aAB_y2 - aAB_y1);
		// 	float t1 = d1 / d;
		// 	float t2 = d2 / d;

		// 	return (t1 >= 0.0f && t1 <= 1.0f && t2 >= 0.0f && t2 <= 1.0f);
		// }

		// ---
		// public static bool SimpleLinesCross(Vector2 aA, Vector2 aB, Vector2 aC, Vector2 aD)
		// {
		// 	if (AntMath.Equal(aA, aC) || AntMath.Equal(aA, aD) ||
		// 		AntMath.Equal(aB, aC) || AntMath.Equal(aB, aD))
		// 	{
		// 		return false;
		// 	}

		// 	return (Intersect(aA.x, aB.x, aC.x, aD.x) && 
		// 		Intersect(aA.y, aB.y, aC.y, aD.y) &&
		// 		Area(aA, aB, aC) * Area(aA, aB, aD) <= 0.0f &&
		// 		Area(aC, aD, aA) * Area(aC, aD, aB) <= 0.0f);
		// }

		// private static float Area(Vector2 aA, Vector2 aB, Vector2 aC)
		// {
		// 	return (aB.x - aA.x) * (aC.y - aA.y) - (aB.y - aA.y) * (aC.x - aA.x);
		// }

		// private static bool Intersect(float aA, float aB, float aC, float aD)
		// {
		// 	if (aA > aB) Swap(ref aA, ref aB);
		// 	if (aC > aD) Swap(ref aC, ref aD);
		// 	return AntMath.Max(aA, aC) <= AntMath.Min(aB, aD);
		// }

		// private static void Swap(ref float aA, ref float aB)
		// {
		// 	float t = aA;
		// 	aA = aB;
		// 	aB = t;
		// }
		// ---

		/// <summary>
		/// Определяет пересечение двух отрезков.
		/// </summary>
		/// <param name="aA">Точка A отрезка AB.</param>
		/// <param name="aB">Точка B отрезка AB.</param>
		/// <param name="aC">Точка C отрезка CD.</param>
		/// <param name="aD">Точка D отрезка CD.</param>
		/// <param name="aIgnoreEqualPoints">Проверка, если какие-либо из вершин совпадают, то линии не пересекаются.</param>
		/// <returns>Возвращает true если отрезки пересекаются.</returns>
		public static bool LinesCross(Vector2 aA, Vector2 aB, Vector2 aC, Vector2 aD, bool aIgnoreEqualPoints = false)
		{
			if (aIgnoreEqualPoints && 
				(AntMath.Equal(aA, aC) || AntMath.Equal(aA, aD) ||
				AntMath.Equal(aB, aC) || AntMath.Equal(aB, aD)))
			{
				return false;
			}

			float d = (aD.y - aC.y) * (aB.x - aA.x) - (aD.x - aC.x) * (aB.y - aA.y);

			// Отрезки паралельны.
			if (Equal(d, 0.0f)) return false;

			float na = (aD.x - aC.x) * (aA.y - aC.y) - (aD.y - aC.y) * (aA.x - aC.x);
			float nb = (aB.x - aA.x) * (aA.y - aC.y) - (aB.y - aA.y) * (aA.x - aC.x);
			float ua = na / d;
			float ub = nb / d;

			return (ua >= 0.0f && ua <= 1.0f && ub >= 0.0f && ub <= 1.0f);
		}

		/// <summary>
		/// Определяет пересечение двух отрезков с вычислением точки пересечения.
		/// </summary>
		/// <param name="aA">Точка A отрезка AB.</param>
		/// <param name="aB">Точка B отрезка AB.</param>
		/// <param name="aC">Точка C отрезка CD.</param>
		/// <param name="aD">Точка D отрезка CD.</param>
		/// <param name="aCrossPoint">Точка в которой пересекаются два отрезка.</param>
		/// <returns>Возвращает true если отрезки пересекаются.</returns>
		public static bool LinesCross(Vector2 aA, Vector2 aB, Vector2 aC, Vector2 aD, out Vector2 aCrossPoint)
		{
			aCrossPoint = Vector2.zero;
			bool isIntersect = false;
			float d = (aD.y - aC.y) * (aB.x - aA.x) - (aD.x - aC.x) * (aB.y - aA.y);

			// Отрезки паралельны.
			if (Equal(d, 0.0f)) return isIntersect;

			float na = (aD.x - aC.x) * (aA.y - aC.y) - (aD.y - aC.y) * (aA.x - aC.x);
			float nb = (aB.x - aA.x) * (aA.y - aC.y) - (aB.y - aA.y) * (aA.x - aC.x);
			float ua = na / d;
			float ub = nb / d;

			if (ua >= 0.0f && ua <= 1.0f && ub >= 0.0f && ub <= 1.0f)
			{
				aCrossPoint.x = aA.x + (ua * (aB.x - aA.x));
				aCrossPoint.y = aA.y + (ua * (aB.y - aA.y));	
				isIntersect = true;
			}
			
			return isIntersect;
		}

		/// <summary>
		/// Сравнивает значения с заданной погрешностью.
		/// </summary>
		/// <param name="aValueA">Первое значение.</param>
		/// <param name="aValueB">Второе значение.</param>
		/// <param name="aDiff">Погрешность.</param>
		/// <returns>Возвращает true если значения равны.</returns>
		public static bool Equal(float aValueA, float aValueB, float aDiff = 0.00001f)
		{
			return (Mathf.Abs(aValueA - aValueB) <= aDiff);
		}
		
		/// <summary>
		/// Находит ближайшую точку сегмента к указанной точке.
		/// </summary>
		/// <param name="aPoint">Точка ближайшую к которой необходимо найти.</param>
		/// <param name="aA">Первая точка отрезка.</param>
		/// <param name="aB">Вторая точка отрезка.</param>
		/// <returns>Возвращает ближайшую точку из отрезка к указанной.</returns>
		// public static Vector2 GetNearestPointFromSegment(Vector2 aPoint, Vector2 aA, Vector2 aB)
		// {
		// 	var a = aPoint.x - aA.x;
		// 	var b = aPoint.y - aA.y;
		// 	var c = aB.x - aA.x;
		// 	var d = aB.y - aA.y;

		// 	var dot = a * c + b * d;
		// 	var lenSq = c * c + d * d;
		// 	var param = -1.0f;
		// 	if (!AntMath.Equal(lenSq, 0.0f)) 
		// 	{
		// 		// In case of zero length line.
		// 		param = dot / lenSq;
		// 	}

		// 	var result = Vector2.zero;
		// 	if (param < 0.0f)
		// 	{
		// 		result.x = aA.x;
		// 		result.y = aA.y;
		// 	}
		// 	else if (param > 1.0f)
		// 	{
		// 		result.x = aB.x;
		// 		result.y = aB.y;
		// 	}
		// 	else
		// 	{
		// 		result.x = aA.x + param * c;
		// 		result.y = aA.x + param * d;
		// 	}

		// 	return result;
		// }

		public static Vector2 GetNearestPointFromSegment(Vector2 aPoint, Vector2 aA, Vector2 aB)
		{
			return (AntMath.Distance(aPoint, aA) < AntMath.Distance(aPoint, aB)) ? aA : aB;
		}

		/// <summary>
		/// Разворачивает сегмент в обе стороны до указанной длины.
		/// </summary>
		/// <param name="aA">Первая точка отрезка.</param>
		/// <param name="aB">Вторая точка отрезка.</param>
		/// <param name="aC">Первая точка нового отрезка.</param>
		/// <param name="aD">Вторая точка нового отрезка.</param>
		/// <param name="aLenght">Длина на которую необходимо увеличить сегмент.</param>
		public static void ExpandSegment(Vector2 aA, Vector2 aB, out Vector2 aC, out Vector2 aD, float aLenght)
		{
			var lenAB = AntMath.Distance(aA, aB);
			aC.x = aB.x + (aB.x - aA.x) / lenAB * aLenght;
			aC.y = aB.y + (aB.y - aA.y) / lenAB * aLenght;
			aD.x = aA.x + (aA.x - aB.x) / lenAB * aLenght;
			aD.y = aA.y + (aA.y - aB.y) / lenAB * aLenght;
		}

		public static Vector2 ExpandSegment(Vector2 aA, Vector2 aB, float aLenght)
		{
			var lenAB = AntMath.Distance(aA, aB);
			return new Vector2(
				aB.x + (aB.x - aA.x) / lenAB * aLenght,
				aB.y + (aB.y - aA.y) / lenAB * aLenght
			);
		}

		// public static float ShortDistanceBetweenPointAndSegment(Vector2 aPoint, Vector2 aA, Vector2 aB)
		// {
		// 	var a = aPoint.x - aA.x;
		// 	var b = aPoint.y - aA.y;
		// 	var c = aB.x - aA.x;
		// 	var d = aB.y - aA.y;

		// 	var dot = a * c + b * d;
		// 	var lenSq = c * c + d * d;
		// 	var param = -1.0f;
		// 	if (!AntMath.Equal(lenSq, 0.0f)) // In case of zero length line.
		// 		param = dot / lenSq;

		// 	float xx, yy;
		// 	if (param < 0.0f)
		// 	{
		// 		xx = aA.x;
		// 		yy = aA.y;
		// 	}
		// 	else if (param > 1.0f)
		// 	{
		// 		xx = aB.x;
		// 		yy = aB.y;
		// 	}
		// 	else
		// 	{
		// 		xx = aA.x + param * c;
		// 		yy = aA.x + param * d;
		// 	}

		// 	var dx = aPoint.x - xx;
		// 	var dy = aPoint.y - yy;
		// 	return Mathf.Sqrt(dx * dx + dy * dy);
		// }
	}
}