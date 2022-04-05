namespace Anthill.Editor
{
	using UnityEngine;
	using System.Collections.Generic;
	using Anthill.Extensions;

	/// <summary>
	/// A simple class providing static access to functions that will provide a 
	/// zoomable area similar to Unity's built in BeginVertical and BeginArea
	/// Systems. Code based off of article found at:
	/// http://martinecker.com/martincodes/unity-editor-window-zooming/
	///  
	/// (Site may be down)
	/// </summary>
	public class AntZoomArea
	{
		private static Stack<Matrix4x4> previousMatrices = new Stack<Matrix4x4>();
		public static Rect Begin(float aZoomScale, Rect aScreenCoordsArea)
		{
			GUI.EndGroup();

			Rect clippedArea = aScreenCoordsArea.ScaleSizeBy(1.0f / aZoomScale, aScreenCoordsArea.min);
			clippedArea.y += -3.0f;

			GUI.BeginGroup(new Rect(0.0f, 21.0f / aZoomScale, clippedArea.width + clippedArea.x, clippedArea.height + clippedArea.y));

			previousMatrices.Push(GUI.matrix);
			Matrix4x4 translation = Matrix4x4.TRS(aScreenCoordsArea.min, Quaternion.identity, Vector3.one);
			Matrix4x4 scale = Matrix4x4.Scale(new Vector3(aZoomScale, aZoomScale, 1.0f));
			GUI.matrix = translation * scale * translation.inverse;

			return clippedArea;
		}

		/// <summary>
		/// Ends the zoom area.
		/// </summary>
		public static void End()
		{
			GUI.matrix = previousMatrices.Pop();
			GUI.EndGroup();
			GUI.BeginGroup(new Rect(0.0f, 21.0f, Screen.width, Screen.height));
		}
	}
}