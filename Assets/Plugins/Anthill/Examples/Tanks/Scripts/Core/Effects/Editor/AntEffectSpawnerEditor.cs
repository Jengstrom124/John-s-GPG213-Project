namespace Anthill.Effects
{
	using UnityEngine;
	using UnityEditor;
	using Anthill.Utils;

	[CustomEditor(typeof(AntEffectSpawner))]
	public class AntEffectSpawnerEditor : Editor
	{
		private AntEffectSpawner _self;

		private void OnEnable()
		{
			_self = (AntEffectSpawner) target;
		}

		private void OnSceneGUI()
		{
			Color c = Handles.color;
			Vector3 pos;
			AntEffectSpawner.Settings curEffect;
			for (int i = 0, n = _self.effects.Length; i < n; i++)
			{
				if (_self.effects[i].isOpened)
				{
					curEffect = _self.effects[i];
					Handles.color = curEffect.dotColor;
					pos = new Vector3(
						_self.transform.position.x + curEffect.offset.x, 
						_self.transform.position.y + curEffect.offset.y, 
						0.0f
					);

					pos = DotGizmo(pos);
					curEffect.offset = new Vector2(
						pos.x - _self.transform.position.x,
						pos.y - _self.transform.position.y
					);
						
					_self.effects[i] = curEffect;
				}
			}
			Handles.color = c;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.Separator();
			EditorGUI.BeginChangeCheck();
			GameObject newEffect = null;

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("List of Effects", EditorStyles.boldLabel);
				if (GUILayout.Button("Collapse", EditorStyles.miniButtonLeft))
				{
					for (int i = 0, n = _self.effects.Length; i < n; i++)
					{
						_self.effects[i].isOpened = false;
					}
				}

				if (GUILayout.Button("Expand", EditorStyles.miniButtonRight))
				{
					for (int i = 0, n = _self.effects.Length; i < n; i++)
					{
						_self.effects[i].isOpened = true;
					}
				}
			}
			EditorGUILayout.EndHorizontal();

			newEffect = (GameObject) EditorGUILayout.ObjectField("Add Effect", newEffect, typeof(GameObject), false);
			if (newEffect != null)
			{
				if (newEffect.GetComponent<AntEffect>() == null)
				{
					A.Warning("Can't add the effect! Object `{0}` doesn't have a AntEmitterEffect.", newEffect.name);
				}
				else
				{
					AntArray.Add<AntEffectSpawner.Settings>(ref _self.effects, new AntEffectSpawner.Settings
					{
						name = newEffect.name,
						prefab = newEffect,
						kind = AntEffectSpawner.EKind.Basic,
						isAutoPlay = true,
						isUpdatePosition = false,
						isInheritAngle = false,
						isDisableOnInpact = false,
						isPlaceToImpactPosition = false,
						isUseImpactAngle = false,
						isImpactOnce = false,
						impactLayerMask = new LayerMask(),
						delay = 0.0f,
						rndToDelay = Vector2.zero,
						lifeTime = 1.0f,
						rndToLifeTime = Vector2.zero,
						angle = 0.0f,
						offset = Vector2.zero,
						dotColor = Color.white,
						isOpened = true
					});
				}
			}
			
			if (_self.effects.Length > 0)
			{
				int delIndex = -1;
				Color c = GUI.color;
				AntEffectSpawner.Settings settings;
				for (int i = 0, n = _self.effects.Length; i < n; i++)
				{
					settings = _self.effects[i];
					GUI.color = GUI.color * settings.dotColor;
					GUILayout.BeginVertical(EditorStyles.helpBox);
					GUI.color = c;

					GUILayout.BeginHorizontal();
					{
						EditorGUI.indentLevel++;
						settings.isOpened = EditorGUILayout.Foldout(settings.isOpened, settings.name, true);
						EditorGUI.indentLevel--;
						
						// GUI.color = c * new Color(1.0f, 1.0f, 0.5f);
						if (GUILayout.Button("", "OL Minus", GUILayout.MaxWidth(16.0f), GUILayout.MaxHeight(16.0f)))
						{
							delIndex = i;
						}
						// GUI.color = c;
					}
					GUILayout.EndHorizontal();

					if (settings.isOpened)
					{
						EditorGUILayout.Space();
						EditorGUI.indentLevel++;
						EditorGUILayout.BeginHorizontal();
						settings.name = EditorGUILayout.TextField("Name", settings.name);
						settings.dotColor = EditorGUILayout.ColorField(settings.dotColor, GUILayout.MaxWidth(80.0f));
						EditorGUILayout.EndHorizontal();
						settings.prefab = EditorGUILayout.ObjectField("Effect", settings.prefab, typeof(GameObject), true) as GameObject;
						// settings.kind = (AntEffectSpawner.EKind) EditorGUILayout.EnumPopup("Kind", settings.kind);
						
						string tooltip;
						// if (settings.kind == AntEffectSpawner.EKind.Basic)
						// {
							tooltip = "Проигрывать эффект при создании объекта.";
							settings.isAutoPlay = EditorGUILayout.Toggle(new GUIContent("Auto Play", tooltip), settings.isAutoPlay);
							
							tooltip = "Остановить эффект при столкновении текущего объекта с другим.";
							settings.isDisableOnInpact = EditorGUILayout.Toggle(new GUIContent("Stop OnCollision", tooltip), settings.isDisableOnInpact);
							EditorGUILayout.Space();

							tooltip = "Смещение источника эффекта относительно владельца.";
							settings.offset = EditorGUILayout.Vector2Field(new GUIContent("Offset", tooltip), settings.offset);

							tooltip = "Угол источника эффекта относительно владельца.";
							settings.angle = EditorGUILayout.Slider(new GUIContent("Angle", tooltip), settings.angle, -180.0f, 180.0f);
							EditorGUILayout.Space();
							
							tooltip = "Применить угол объекта для эффекта.";
							settings.isInheritAngle =  EditorGUILayout.Toggle(new GUIContent("Inherit Angle", tooltip), settings.isInheritAngle);

							tooltip = "Применить позицию объекта для эффекта во время анимации.";
							settings.isUpdatePosition =  EditorGUILayout.Toggle(new GUIContent("Update Position", tooltip), settings.isUpdatePosition);
							EditorGUILayout.Space();
						// }
						// else if (settings.kind == AntEffectSpawner.EKind.Physic)
						// {
						// 	settings.impactLayerMask = AntLayerMask.LayerMaskField("Layer Mask", settings.impactLayerMask);

						// 	tooltip = "Эффект проиграется один раз только при первом столкновении.";
						// 	settings.isImpactOnce = EditorGUILayout.Toggle(new GUIContent("Play Once", tooltip), settings.isImpactOnce);

						// 	tooltip = "Установить эффект в точку столкновения.";
						// 	settings.isPlaceToImpactPosition = EditorGUILayout.Toggle(new GUIContent("Place to Impact Point", tooltip), settings.isPlaceToImpactPosition);

						// 	tooltip = "Применить угол объекта для эффекта.";
						// 	settings.isInheritAngle = EditorGUILayout.Toggle(new GUIContent("Inherit Angle", tooltip), settings.isInheritAngle);

						// 	tooltip = "Применить позицию объекта для эффекта во время анимации.";
						// 	settings.isUseImpactAngle = EditorGUILayout.Toggle(new GUIContent("Use Inpact Angle", tooltip), settings.isUseImpactAngle);

						// 	EditorGUILayout.Space();
						// }
						EditorGUI.indentLevel--;

						GUILayout.BeginVertical(EditorStyles.helpBox);
						{
							settings.customDelay = EditorGUILayout.BeginToggleGroup("Custom Delay", settings.customDelay);
							if (settings.customDelay)
							{
								EditorGUILayout.Space();
								settings.delay = EditorGUILayout.FloatField("Delay", settings.delay);
								settings.rndToDelay = EditorGUILayout.Vector2Field("Rnd to Delay", settings.rndToDelay);
								EditorGUILayout.Space();
								
							}
							EditorGUILayout.EndToggleGroup();
						}
						GUILayout.EndVertical();

						GUILayout.BeginVertical(EditorStyles.helpBox);
						{
							settings.customLifeTime = EditorGUILayout.BeginToggleGroup("Custom Life Time", settings.customLifeTime);
							if (settings.customLifeTime)
							{
								EditorGUILayout.Space();
								settings.lifeTime = EditorGUILayout.FloatField("Life Time", settings.lifeTime);
								settings.rndToLifeTime = EditorGUILayout.Vector2Field("Rnd to Life Time", settings.rndToLifeTime);
								EditorGUILayout.Space();
								
							}
							EditorGUILayout.EndToggleGroup();
						}
						GUILayout.EndVertical();
					}
					
					_self.effects[i] = settings;
					if (Application.isPlaying)
					{
						if (GUILayout.Button("Play"))
						{
							_self.Play(settings.name);
						}

						// EditorGUILayout.LabelField("Debug Settings");
						// EditorGUILayout.Toggle("Auto Spawn", false);
						// EditorGUILayout.FloatField("Spawn Delay", _self.)
					}

					GUILayout.EndVertical();
				}

				if (delIndex > -1)
				{
					AntArray.RemoveAt<AntEffectSpawner.Settings>(ref _self.effects, delIndex);
				}
			}
			else
			{
				EditorGUILayout.LabelField("List of Effects is Empty.", EditorStyles.centeredGreyMiniLabel);
			}

			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(target);
			}
		}

		private Vector2 DotGizmo(Vector3 aPosition, float aSize = 0.05f)
		{
			float s = HandleUtility.GetHandleSize(aPosition) * aSize;
			return Handles.FreeMoveHandle(aPosition, Quaternion.identity, s, Vector3.zero, Handles.DotHandleCap);
		}
	}
}