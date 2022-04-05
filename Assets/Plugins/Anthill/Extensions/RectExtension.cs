namespace Anthill.Extensions
{
	using UnityEngine;

	public static class RectExtension
	{
		public static Rect ScaleSizeBy(this Rect aRect, float aScale) 
		{ 
			return aRect.ScaleSizeBy(aScale, aRect.center); 
		}
		
		public static Rect ScaleSizeBy(this Rect aRect, float aScale, Vector2 aPivotPoint)
		{
			var result = aRect;

			//"translate" the top left to something like an origin
			result.x -= aPivotPoint.x;
			result.y -= aPivotPoint.y;

			//Scale the rect
			result.xMin *= aScale;
			result.yMin *= aScale;
			result.xMax *= aScale;
			result.yMax *= aScale;

			//"translate" the top left back to its original position
			result.x += aPivotPoint.x;
			result.y += aPivotPoint.y;

			return result;
		}
	}
}
