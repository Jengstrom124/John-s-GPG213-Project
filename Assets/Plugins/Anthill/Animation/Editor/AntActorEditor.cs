namespace Anthill.Animation
{
	using UnityEngine;
	using UnityEditor;

	using Anthill.Utils;

	[CustomEditor(typeof(AntActor))]
	public class AntActorEditor : Editor
	{
		private AntActor _self;

		private void OnEnable()
		{
			_self = (AntActor) target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUI.BeginChangeCheck();

			string[] anims = new string[_self.animations.Length];
			for (int i = 0, n = _self.animations.Length; i < n; i++)
			{
				anims[i] = (_self.animations[i] != null)
					? _self.animations[i].key
					: "<Missed Animation>";
			}

			if (anims.Length == 0)
			{
				AntArray.Add(ref anims, "<None>");
			}
		
			int index = 0;
			index = System.Array.FindIndex(anims, x => string.Equals(x, _self.initialAnimation));
			index = (index < 0) ? 0 : index;
			index = EditorGUILayout.Popup("Initial Animation", index, anims);

			if (anims.Length > 0 && !_self.initialAnimation.Equals(anims[index]))
			{
				_self.initialAnimation = anims[index];
			}
			
			if (_self.animations.Length == 0)
			{
				EditorGUILayout.HelpBox("You must add at least one animation.", MessageType.Warning);
			}

			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				AntAnimation newAnim = null;
				newAnim = (AntAnimation) EditorGUILayout.ObjectField("Add Animation", newAnim, typeof(AntAnimation), false, GUILayout.MaxHeight(16.0f));
				if (newAnim != null)
				{
					var foundIndex = System.Array.FindIndex(_self.animations, x => System.Object.ReferenceEquals(x, newAnim));
					if (foundIndex == -1)
					{
						foundIndex = System.Array.FindIndex(_self.animations, x => x.key.Equals(newAnim.key));
						if (foundIndex == -1)
						{
							AntArray.Add<AntAnimation>(ref _self.animations, newAnim);
						}
						else
						{
							A.Warning($"Animation with `{newAnim.key}` name already exists! Required unique animation names.");
						}
					}
					else
					{
						A.Warning($"Animation `{newAnim.name}` already added!");
					}
				}

				int delIndex = -1;
				AntAnimation prevAnim = null;
				for (int i = 0, n = _self.animations.Length; i < n; i++)
				{
					GUILayout.BeginHorizontal();
					{
						prevAnim = _self.animations[i];
						_self.animations[i] = (AntAnimation) EditorGUILayout.ObjectField(_self.animations[i], typeof(AntAnimation), false);
						if (GUILayout.Button("", "OL Minus", GUILayout.MaxWidth(20.0f), GUILayout.MaxHeight(20.0f)))
						{
							delIndex = i;
						}
					}
					GUILayout.EndHorizontal();
				}

				if (delIndex > -1)
				{
					AntArray.RemoveAt<AntAnimation>(ref _self.animations, delIndex);
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