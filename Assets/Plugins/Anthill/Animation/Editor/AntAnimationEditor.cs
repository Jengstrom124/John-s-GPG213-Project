namespace Anthill.Animation
{
	using UnityEditor;
	using UnityEngine;

	using Anthill.Utils;

	[CustomEditor(typeof(AntAnimation))]
	public class AntAnimationEditor : Editor
	{
		private AntAnimation _self;

		private void OnEnable()
		{
			_self = (AntAnimation) target;
		}

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();

			var prevKey = _self.key;
			_self.key = EditorGUILayout.TextField("Animation Key", _self.key);

			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				Sprite newSprite = null;
				newSprite = (Sprite) EditorGUILayout.ObjectField("Add Frame", newSprite, typeof(Sprite), false, GUILayout.MaxHeight(16.0f));
				if (newSprite != null)
				{
					if (_self.frames == null)
					{
						_self.frames = new Sprite[0];
					}

					AntArray.Add<Sprite>(ref _self.frames, newSprite);
				}

				if (_self.frames != null)
				{
					int delIndex = -1;
					Sprite prev;
					for (int i = 0, n = _self.frames.Length; i < n; i++)
					{
						GUILayout.BeginHorizontal();
						{
							prev = _self.frames[i];
							_self.frames[i] = (Sprite) EditorGUILayout.ObjectField(_self.frames[i], typeof(Sprite), false);
							if (GUILayout.Button("", "OL Minus", GUILayout.MaxWidth(20.0f), GUILayout.MaxHeight(20.0f)))
							{
								delIndex = i;
							}
						}
						GUILayout.EndHorizontal();
					}

					if (delIndex > -1)
					{
						AntArray.RemoveAt<Sprite>(ref _self.frames, delIndex);
					}
				}
			}
			GUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(target);
			}
		}
	}
}