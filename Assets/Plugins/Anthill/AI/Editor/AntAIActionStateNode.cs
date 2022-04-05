namespace Anthill.AI
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using Anthill.Extensions;

	/// <summary>
	/// This class implements a card of the plan that can be builded from the world state card
	/// in the AIWorkbench.
	/// </summary>
	public class AntAIActionStateNode : AntAIBaseNode
	{
	#region Private Variables

		private struct Item
		{
			public string name;
			public bool value;
			public bool isChanged;
		}

		private const float lineHeight = 21.0f;
		private List<Item> _items;
		private GUIStyle _badgeStyle;
		private GUIStyle _labelStyle;
		private GUIStyle _boldLabelStyle;

	#endregion

	#region Public Methods

		public AntAIActionStateNode(Vector2 aPosition, float aWidth, float aHeight,
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

			_boldLabelStyle = new GUIStyle(EditorStyles.boldLabel);
			m = _boldLabelStyle.margin;
			m.top = 0;
			_boldLabelStyle.margin = m;
		}

		public void BindData(string aTitle, AntAIPlanner aPlanner, AntAICondition aCur, AntAICondition aPre)
		{
			title = string.Concat("▶ ", aTitle);
			
			_items = new List<Item>();
			bool v;
			for (int i = 0, n = AntAIPlanner.MAX_ATOMS; i < n; i++)
			{
				if (aCur.GetMask(i))
				{
					v = aCur.GetValue(i);
					_items.Add(new Item
					{
						name = aPlanner.atoms[i],
						value = v,
						isChanged = (v != aPre.GetValue(i))
					});
				}
			}
		}

		public override void Draw()
		{
			rect.height = (_items.Count > 0)
				? lineHeight * _items.Count
				: lineHeight;
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

			var c = GUI.color;
			GUILayout.BeginArea(content);
			if (_items.Count > 0)
			{
				GUILayout.BeginVertical();

				for (int i = 0, n = _items.Count; i < n; i++)
				{
					GUILayout.BeginHorizontal(EditorStyles.toolbar);
						
					GUI.color = c * ((_items[i].value) 
						? AntAIEditorStyle.Green
						: AntAIEditorStyle.Red
					);
					
					GUILayout.Button(_items[i].value.ToStr(), _badgeStyle, GUILayout.MaxWidth(18.0f));
					GUI.color = c;
					
					if (_items[i].isChanged)
					{
						GUILayout.BeginVertical();
						{
							GUILayout.Space(3.0f);
							GUILayout.Label((_items[i].value) ? "YES" : "NO", GUI.skin.FindStyle("AssetLabel"));
						}
						GUILayout.EndVertical();
						
						EditorGUILayout.LabelField(string.Concat("►  ", _items[i].name), EditorStyles.miniBoldLabel);
					}
					else
					{
						EditorGUILayout.LabelField(_items[i].name, EditorStyles.miniBoldLabel);
					}
					
					GUILayout.EndHorizontal();
				}

				GUILayout.EndVertical();
			}
			else
			{
				GUILayout.Label("No Coditions", EditorStyles.centeredGreyMiniLabel);
			}
			GUILayout.EndArea();
		}

	#endregion
	}
}