namespace Anthill.AI
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using Anthill.Utils;

	/// <summary>
	/// This is base class for all cards in the AIWorkbench.
	/// </summary>
	public abstract class AntAIBaseNode
	{
	#region Public Variables

		public Rect rect;
		public Rect content;
		public string title;
		public string value;
		public bool isDragged;
		public bool isActive;
		public List<AntAIBaseNode> links;

		public GUIStyle currentStyle;
		public GUIStyle normalStyle;
		public GUIStyle selectedStyle;

	#endregion

	#region Private Variables

		protected GUIStyle _bodyStyle;
		protected GUIStyle _titleStyle;

		protected bool _isSelected;
		protected Color _color = AntAIEditorStyle.CardWhite;

	#endregion

	#region Getters / Setters
		
		public Vector2 Position
		{
			get => rect.position;
			set => rect.position = value;
		}

		public virtual bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				currentStyle = (_isSelected)
					? selectedStyle
					: normalStyle;
			}
		}

		public virtual Color Color
		{
			get => _color;
			set => _color = value;
		}

		public virtual string Title
		{
			get => title;
		}
		
	#endregion

	#region Public Methods

		public AntAIBaseNode(Vector2 aPosition, float aWidth, float aHeight,
			GUIStyle aDefaultStyle, GUIStyle aSelectedStyle)
		{
			rect = new Rect(aPosition.x, aPosition.y, aWidth, aHeight);
			content = new Rect(aPosition.x + 7, aPosition.y + 30.0f, aWidth - 14.0f, aHeight - 40.0f);
			currentStyle = aDefaultStyle;
			normalStyle = aDefaultStyle;
			selectedStyle = aSelectedStyle;

			_bodyStyle = CreateStyle("IN BigTitle");

			_titleStyle = new GUIStyle();
			_titleStyle.fontStyle = FontStyle.Bold;
			if (EditorGUIUtility.isProSkin)
			{
				_titleStyle.normal.textColor = Color.white;
			}
			
			links = new List<AntAIBaseNode>();
		}

		public virtual void Drag(Vector2 aDelta)
		{
			rect.position += aDelta;
		}

		public virtual void Draw()
		{
			// Node
			GUI.Box(rect, "", currentStyle);
			
			// Title
			GUI.Label(
				new Rect(rect.x + 12.0f, rect.y + 12.0f, rect.y + 12.0f, rect.width - 24.0f), 
				title, 
				_titleStyle
			);

			content.x = rect.x + 7.0f;
			content.y = rect.y + 30.0f;
			content.width = rect.width - 14.0f;
			content.height = rect.height - 50.0f;
			GUI.Box(content, "", _bodyStyle);
		}

		public virtual bool ProcessEvents(Event aEvent, AntAIWorkbench aWorkbench)
		{
			bool result = false;
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
							result = true;
						}
						else
						{
							GUI.changed = true;
							IsSelected = false;
							currentStyle = normalStyle;
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

		public Vector2 GetOutputPoint(Vector2 aToPosition)
		{
			var pos = Position;
			float ang = AntMath.AngleDeg(pos, aToPosition);
			return new Vector2(
				pos.x + 10.0f * Mathf.Cos((ang + 90.0f) * Mathf.Deg2Rad),
				pos.y + 10.0f * Mathf.Sin((ang + 90.0f) * Mathf.Deg2Rad)
			);
		}

		public Vector2 GetOutputPoint(AntAIBaseNode aToNode)
		{
			var pos = Position;
			pos.x += rect.width * 0.5f;
			pos.y += rect.height * 0.5f;
			float ang = AntMath.AngleDeg(pos, aToNode.Position);
			return new Vector2(
				pos.x + 10.0f * Mathf.Cos((ang + 90.0f) * Mathf.Deg2Rad),
				pos.y + 10.0f * Mathf.Sin((ang + 90.0f) * Mathf.Deg2Rad)
			);
		}

		public Vector2 GetInputPoint(AntAIBaseNode aFromNode)
		{
			var pos = Position;
			pos.x += rect.width * 0.5f;
			pos.y += rect.height * 0.5f;
			float ang = AntMath.AngleDeg(pos, aFromNode.Position);
			return new Vector2(
				pos.x - 10.0f * Mathf.Cos((ang + 90.0f) * Mathf.Deg2Rad),
				pos.y - 10.0f * Mathf.Sin((ang + 90.0f) * Mathf.Deg2Rad)
			);
		}

	#endregion

	#region Private Methods

		protected virtual void ProcessContextMenu()
		{
			// ..
		}

		protected GUIStyle CreateStyle(string aTextureName)
		{
			var style = new GUIStyle(aTextureName);
			style.border = new RectOffset(12, 12, 12, 12);
			style.padding = new RectOffset(12, 0, 10, 0);
			return style;
		}

		protected GUIStyle CreateNodeStyle(string aTextureName)
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

	#endregion
	}
}