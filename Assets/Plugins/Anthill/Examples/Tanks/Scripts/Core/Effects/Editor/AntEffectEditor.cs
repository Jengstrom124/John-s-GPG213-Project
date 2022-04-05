namespace Anthill.Effects
{
	using UnityEngine;
	using UnityEditor;

	// using Anthill.Core;
	using Anthill.Utils;

	[CustomEditor(typeof(AntEffect))]
	public class AntEffect2Editor : Editor
	{
		private AntEffect _self;

		private void OnEnable()
		{
			_self = (AntEffect) target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUI.BeginChangeCheck();
			_self.startDelay = EditorGUILayout.FloatField("Start Delay", _self.startDelay);

			GUI.enabled = !_self.isLooping;
			_self.lifeTime = EditorGUILayout.FloatField("LifeTime", _self.lifeTime);
			GUI.enabled = true;

			_self.isLooping = EditorGUILayout.Toggle("Looping", _self.isLooping);
			_self.isAutoPlay = EditorGUILayout.Toggle("Auto Play", _self.isAutoPlay);
			_self.isAutoReturnToPool = EditorGUILayout.Toggle("Auto Return To Pool", _self.isAutoReturnToPool);
			_self.isAutoRepeat = EditorGUILayout.Toggle("Auto Repeat", _self.isAutoRepeat);
			EditorGUILayout.Space();

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			{
				AntEmitterPreset newObject = null;
				newObject = EditorGUILayout.ObjectField("Drop to Add", newObject, typeof(AntEmitterPreset), false) as AntEmitterPreset;
				if (newObject != null)
				{
					AntArray.Add<AntEmitterPreset>(ref _self.emitters, newObject);
				}

				int delIndex = -1;
				for (int i = 0, n = _self.emitters.Length; i < n; i++)
				{
					EditorGUILayout.BeginHorizontal();
					{
						_self.emitters[i] = EditorGUILayout.ObjectField(_self.emitters[i], typeof(AntEmitterPreset), false) as AntEmitterPreset;
						if (GUILayout.Button("x", GUILayout.Width(16.0f), GUILayout.Height(16.0f)))
						{
							delIndex = i;
						}
					}
					EditorGUILayout.EndHorizontal();
				}

				if (delIndex >= 0)
				{
					AntArray.RemoveAt<AntEmitterPreset>(ref _self.emitters, delIndex);
				}
			}
			EditorGUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(target);
			}

			// MARK: Эффекты больше нельзя стартануть напрямую, т.к. они теперь работают через системы.
			// if (Application.isPlaying)
			// {
			// 	if (_self.IsPlaying && (_self.isLooping || _self.isAutoRepeat))
			// 	{
			// 		if (GUILayout.Button("Stop"))
			// 		{
			// 			_self.Stop();
			// 		}
			// 	}
			// 	else
			// 	{
			// 		if (GUILayout.Button("Play"))
			// 		{
			// 			_self.Play();
			// 		}
			// 	}
			// }
		}
	}
}