namespace Anthill.Effects
{
	using System;
	using UnityEngine;
	using UnityEditor;

	using Anthill.Editor;
	using Anthill.Utils;

	[CustomEditor(typeof(AntEmitterPreset))]
	public class AntEmitterPresetEditor : Editor
	{
		#region Variables

		// :: Private Variables ::

		private AntEmitterPreset _self;
		private Color _green = new Color(212.0f / 255.0f, 1.0f, 134 / 255.0f, 1.0f);
		private bool _isDirty;

		private bool _flipSettings;

		#endregion
		#region Unity Calls

		private void OnEnable()
		{
			_self = (AntEmitterPreset) target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			
			DrawBurstComponent("Burst");
			DrawEmissionComponent("Emission");
			DrawSourceComponent("Prefabs");
			DrawPositionComponent("Position");
			DrawLifeTimeComponent("Life Time");
			DrawActorComponent("Actor");
			DrawColourComponent("Colour");
			DrawScaleComponent("Scale");
			DrawRotationComponent("Rotation");
			DrawMovementComponent("Movement");
			DrawPhysicSettingsComponent("Physic Settings");
			DrawPhysicImpulseComponent("Physic Impulse");
			DrawPhysicExplosionComponent("Physic Explosion");
			DrawSFLightingComponent("SF Lighting");
			DrawTrailComponent("Trail");

			if (_isDirty)
			{
				EditorUtility.SetDirty(target);
				// AntLog.Trace("Changed!");
				_isDirty = false;
			}
		}

		#endregion
		#region Private Methods

		private void DrawBurstComponent(string aTitle)
		{
			var color = GUI.color;
			GUI.color = (_self.burst.isExists) ? _green : color;
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				GUI.color = color;
				GUILayout.BeginHorizontal();
				{
					EditorGUI.indentLevel++;
					_self.burst.isOpen = EditorGUILayout.Foldout(_self.burst.isOpen, aTitle, true);
					_self.burst.isExists = EditorGUILayout.Toggle(_self.burst.isExists, GUILayout.MaxWidth(30.0f));
					EditorGUI.indentLevel--;
					
				}
				GUILayout.EndHorizontal();

				if (_self.burst.isOpen)
				{
					EditorGUILayout.Space();
					EditorGUI.indentLevel++;

					// List Title.
					GUILayout.BeginHorizontal();
					{
						EditorGUILayout.LabelField("Time", EditorStyles.boldLabel, GUILayout.MaxWidth(80.0f));
						EditorGUILayout.LabelField("Min", EditorStyles.boldLabel, GUILayout.MaxWidth(80.0f));
						EditorGUILayout.LabelField("Max", EditorStyles.boldLabel, GUILayout.MaxWidth(80.0f));
						if (GUILayout.Button("+", GUILayout.MaxWidth(16.0f), GUILayout.MaxHeight(16.0f)))
						{
							AntArray.Add<BurstItem>(ref _self.burst.list, new BurstItem
							{
								time = 0.0f,
								min = 1,
								max = 1
							});
						}
					}
					GUILayout.EndHorizontal();

					// List Contents.
					if (_self.burst.list.Length == 0)
					{
						EditorGUILayout.LabelField("<List is Empty>", EditorStyles.boldLabel);
					}

					int delIndex = -1;
					BurstItem item;
					for (int i = 0, n = _self.burst.list.Length; i < n; i++)
					{
						GUILayout.BeginHorizontal();
						item = _self.burst.list[i];
						item.time = EditorGUILayout.FloatField(item.time, GUILayout.MaxWidth(80.0f));
						item.min = EditorGUILayout.IntField(item.min, GUILayout.MaxWidth(80.0f));
						item.max = EditorGUILayout.IntField(item.max, GUILayout.MaxWidth(80.0f));
						_self.burst.list[i] = item;
						
						if (GUILayout.Button("x", GUILayout.MaxWidth(16.0f), GUILayout.MaxHeight(16.0f)))
						{
							delIndex = i;
						}
						GUILayout.EndHorizontal();
					}

					if (delIndex >= 0)
					{
						AntArray.RemoveAt<BurstItem>(ref _self.burst.list, delIndex);
					}

					EditorGUILayout.Space();
					EditorGUI.indentLevel--;
				}
			}
			GUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				_isDirty = true;
			}
		}

		private void DrawEmissionComponent(string aTitle)
		{
			var color = GUI.color;
			GUI.color = (_self.emission.isExists) ? _green : color;
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				GUI.color = color;
				GUILayout.BeginHorizontal();
				{
					EditorGUI.indentLevel++;
					_self.emission.isOpen = EditorGUILayout.Foldout(_self.emission.isOpen, aTitle, true);
					_self.emission.isExists = EditorGUILayout.Toggle(_self.emission.isExists, GUILayout.MaxWidth(30.0f));
					EditorGUI.indentLevel--;
					
				}
				GUILayout.EndHorizontal();

				if (_self.emission.isOpen)
				{
					EditorGUILayout.Space();
					EditorGUI.indentLevel++;

					_self.emission.spawnInterval = EditorGUILayout.Slider("Spawn Interval", _self.emission.spawnInterval, 0.0f, 10.0f);
					_self.emission.rndToSpawnInterval = EditorGUILayout.Vector2Field("Rnd To Interval", _self.emission.rndToSpawnInterval);
					_self.emission.numParticles = EditorGUILayout.IntSlider("Particles Count", _self.emission.numParticles, 0, 50);
					_self.emission.rndToNumParticles = EditorGUILayout.Vector2Field("Rnd To Particles", _self.emission.rndToNumParticles);

					EditorGUILayout.Space();
					EditorGUI.indentLevel--;
				}
			}
			GUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				_isDirty = true;
			}
		}

		private void DrawSourceComponent(string aTitle)
		{
			var color = GUI.color;
			GUI.color = (_self.source.isExists) ? _green : color;
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				GUI.color = color;
				GUILayout.BeginHorizontal();
				{
					EditorGUI.indentLevel++;
					_self.source.isOpen = EditorGUILayout.Foldout(_self.source.isOpen, aTitle, true);
					_self.source.isExists = EditorGUILayout.Toggle(_self.source.isExists, GUILayout.MaxWidth(30.0f));
					EditorGUI.indentLevel--;
					
				}
				GUILayout.EndHorizontal();

				if (_self.source.isOpen)
				{
					EditorGUILayout.Space();
					EditorGUI.indentLevel++;

					GameObject newObject = null;
					newObject = EditorGUILayout.ObjectField("Drop to Add", newObject, typeof(GameObject), false) as GameObject;
					if (newObject != null)
					{
						if (!IsParticleAlreadyAdded(newObject))
						{
							var particle = newObject.GetComponent<AntParticle>();
							if (particle != null)
							{
								AntArray.Add<GameObject>(ref _self.source.prefabs, newObject);
							}
							else
							{
								EditorUtility.DisplayDialog("Oops!", string.Format("Prefab `{0}` does not have AntParticle component!", newObject.name), "Ok");
							}
						}
						else
						{
							EditorUtility.DisplayDialog("Oops!", string.Format("Prefab `{0}` already added to the spawn list!", newObject.name), "Ok");
						}
					}

					if (_self.source.prefabs.Length > 0)
					{
						int delIndex = -1;
						for (int i = 0, n = _self.source.prefabs.Length; i < n; i++)
						{
							GUILayout.BeginHorizontal();
							{
								newObject = EditorGUILayout.ObjectField(_self.source.prefabs[i], typeof(GameObject), true) as GameObject;
								if (GUILayout.Button("x", GUILayout.MaxWidth(16.0f), GUILayout.MaxHeight(16.0f)))
								{
									delIndex = i;
								}
							}
							GUILayout.EndHorizontal();
							if (!System.Object.ReferenceEquals(newObject, _self.source.prefabs[i]))
							{
								if (!IsParticleAlreadyAdded(newObject))
								{
									_self.source.prefabs[i] = newObject;
								}
								else
								{
									EditorUtility.DisplayDialog("Oops!", string.Format("Prefab `{0}` already added to the spawn list!", newObject.name), "Ok");
								}
							}
						}

						if (delIndex > -1)
						{
							AntArray.RemoveAt<GameObject>(ref _self.source.prefabs, delIndex);
						}

						EditorGUILayout.Space();
						_self.source.selectRandom = EditorGUILayout.Toggle("Select Random", _self.source.selectRandom);
					}
					else
					{
						EditorGUILayout.LabelField("<Prefab List is Empty>", EditorStyles.boldLabel);
					}

					EditorGUILayout.Space();
					EditorGUI.indentLevel--;
				}
			}
			GUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				_isDirty = true;
			}
		}

		private void DrawLifeTimeComponent(string aTitle)
		{
			var color = GUI.color;
			GUI.color = (_self.lifeTime.isExists) ? _green : color;
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				GUI.color = color;
				GUILayout.BeginHorizontal();
				{
					EditorGUI.indentLevel++;
					_self.lifeTime.isOpen = EditorGUILayout.Foldout(_self.lifeTime.isOpen, aTitle, true);
					_self.lifeTime.isExists = EditorGUILayout.Toggle(_self.lifeTime.isExists, GUILayout.MaxWidth(30.0f));
					EditorGUI.indentLevel--;
					
				}
				GUILayout.EndHorizontal();

				if (_self.lifeTime.isOpen)
				{
					EditorGUILayout.Space();
					EditorGUI.indentLevel++;
					
					_self.lifeTime.duration = EditorGUILayout.Slider("Duration", _self.lifeTime.duration, 0.0f, 20.0f);
					_self.lifeTime.rndToDuration = EditorGUILayout.Vector2Field("Rnd To Duration", _self.lifeTime.rndToDuration);

					EditorGUILayout.Space();
					EditorGUI.indentLevel--;
				}
			}
			GUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				_isDirty = true;
			}
		}

		private void DrawPositionComponent(string aTitle)
		{
			var color = GUI.color;
			GUI.color = (_self.position.isExists) ? _green : color;
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				GUI.color = color;
				GUILayout.BeginHorizontal();
				{
					EditorGUI.indentLevel++;
					_self.position.isOpen = EditorGUILayout.Foldout(_self.position.isOpen, aTitle, true);
					_self.position.isExists = EditorGUILayout.Toggle(_self.position.isExists, GUILayout.MaxWidth(30.0f));
					EditorGUI.indentLevel--;
					
				}
				GUILayout.EndHorizontal();

				if (_self.position.isOpen)
				{
					EditorGUILayout.Space();
					EditorGUI.indentLevel++;

					_self.position.position = EditorGUILayout.Vector2Field("Position", _self.position.position);
					_self.position.rndToPositionX = EditorGUILayout.Vector2Field("Rnd to X Pos", _self.position.rndToPositionX);
					_self.position.rndToPositionY = EditorGUILayout.Vector2Field("Rnd to Y Pos", _self.position.rndToPositionY);
					EditorGUILayout.Space();

					_self.position.distance = EditorGUILayout.FloatField("Distance", _self.position.distance);
					_self.position.rndToDistance = EditorGUILayout.Vector2Field("Rnd to Distance", _self.position.rndToDistance);
					EditorGUILayout.Space();

					_self.position.initialAngle = EditorGUILayout.Slider("Initial Angle", _self.position.initialAngle, -180.0f, 180.0f);
					_self.position.lowerAngle = EditorGUILayout.Slider("Lower Area", _self.position.lowerAngle, -180.0f, 0.0f);
					_self.position.upperAngle = EditorGUILayout.Slider("Upper Area", _self.position.upperAngle, 0.0f, 180.0f);
					EditorGUILayout.Space();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.position.strongOrder = EditorGUILayout.BeginToggleGroup("Strong Order", _self.position.strongOrder);
						EditorGUI.indentLevel++;
						if (_self.position.strongOrder)
						{
							EditorGUILayout.Space();
							_self.position.countParticles = EditorGUILayout.IntField("Count Particles", _self.position.countParticles);
							EditorGUILayout.Space();
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();
					
					EditorGUILayout.Space();
					_self.position.rotate = EditorGUILayout.Toggle("Rotate Prefab", _self.position.rotate);
					_self.position.inheritRotation = EditorGUILayout.Toggle("Inherit Rotation", _self.position.inheritRotation);
					EditorGUILayout.Space();
					
					EditorGUI.indentLevel--;
				}

				// EditorGUILayout.EndFoldoutHeaderGroup();
			}
			GUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				_isDirty = true;
			}
		}

		private void DrawActorComponent(string aTitle)
		{
			var color = GUI.color;
			GUI.color = (_self.actor.isExists) ? _green : color;
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				GUI.color = color;
				GUILayout.BeginHorizontal();
				{
					EditorGUI.indentLevel++;
					_self.actor.isOpen = EditorGUILayout.Foldout(_self.actor.isOpen, aTitle, true);
					_self.actor.isExists = EditorGUILayout.Toggle(_self.actor.isExists, GUILayout.MaxWidth(30.0f));
					EditorGUI.indentLevel--;
					
				}
				GUILayout.EndHorizontal();

				if (_self.actor.isOpen)
				{
					EditorGUILayout.Space();
					EditorGUI.indentLevel++;

					GUI.enabled = !_self.actor.selectRandomFrame;
					_self.actor.startFrame = EditorGUILayout.IntField("Start Frame", _self.actor.startFrame);
					GUI.enabled = true;
					_self.actor.selectRandomFrame = EditorGUILayout.Toggle("Random Frame", _self.actor.selectRandomFrame);
					EditorGUILayout.Space();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.actor.playAnimation = EditorGUILayout.BeginToggleGroup("Play", _self.actor.playAnimation);
						EditorGUI.indentLevel++;
						if (_self.actor.playAnimation)
						{
							EditorGUILayout.Space();
							_self.actor.reverse = EditorGUILayout.Toggle("Reverse", _self.actor.reverse);
							_self.actor.timeScale = EditorGUILayout.FloatField("Time Scale", _self.actor.timeScale);
							_self.actor.rndToTimeScale = EditorGUILayout.Vector2Field("Rnd to Time Scale", _self.actor.rndToTimeScale);
							EditorGUILayout.Space();
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.actor.loop = EditorGUILayout.BeginToggleGroup("Loop", _self.actor.loop);
						EditorGUI.indentLevel++;
						if (_self.actor.loop)
						{
							EditorGUILayout.Space();
							_self.actor.loopDelay = EditorGUILayout.FloatField("Loop Delay", _self.actor.loopDelay);
							_self.actor.rndToLoopDelay = EditorGUILayout.Vector2Field("Rnd to Loop Delay", _self.actor.rndToLoopDelay);
							EditorGUILayout.Space();
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.actor.sorting = EditorGUILayout.BeginToggleGroup("Sorting", _self.actor.sorting);
						EditorGUI.indentLevel++;
						if (_self.actor.sorting)
						{
							EditorGUILayout.Space();
							_self.actor.sortingMode = (ESortingMode) EditorGUILayout.EnumPopup("Mode", _self.actor.sortingMode);
							var layers = AntEditorHelpers.GetSortingLayerNames();
							if (_self.actor.sortingLayer == "")
							{
								_self.actor.sortingLayer = "Default";
							}

							int index = Array.IndexOf(layers, _self.actor.sortingLayer);
							index = EditorGUILayout.Popup("Layer", index, layers);
							_self.actor.sortingLayer = (index >= 0 && index < layers.Length)
								? layers[index]
								: layers[0];
							_self.actor.sortingOrder = EditorGUILayout.IntField("Order", _self.actor.sortingOrder);
							EditorGUILayout.Space();
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();

					EditorGUILayout.Space();
					GUILayout.BeginHorizontal();
					{
						GUI.enabled = !_self.actor.rndFlipX;
						_self.actor.flipX = EditorGUILayout.Toggle("Flip X", _self.actor.flipX);
						GUI.enabled = !_self.actor.rndFlipY;
						_self.actor.flipY = EditorGUILayout.Toggle("Flip Y", _self.actor.flipY);
						
						GUI.enabled = true;
					}
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					{
						_self.actor.rndFlipX = EditorGUILayout.Toggle("Rnd Flip X", _self.actor.rndFlipX);
						_self.actor.rndFlipY = EditorGUILayout.Toggle("Rnd Flip Y", _self.actor.rndFlipY);
					}
					GUILayout.EndHorizontal();
					EditorGUILayout.Space();

					_self.actor.useChildActor = EditorGUILayout.Toggle("Use Child Actor", _self.actor.useChildActor);
					EditorGUILayout.Space();
					EditorGUI.indentLevel--;
				}
			}
			GUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				_isDirty = true;
			}
		}

		private void DrawColourComponent(string aTitle)
		{
			var color = GUI.color;
			GUI.color = (_self.colour.isExists) ? _green : color;
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				GUI.color = color;
				GUILayout.BeginHorizontal();
				{
					EditorGUI.indentLevel++;
					_self.colour.isOpen = EditorGUILayout.Foldout(_self.colour.isOpen, aTitle, true);
					_self.colour.isExists = EditorGUILayout.Toggle(_self.colour.isExists, GUILayout.MaxWidth(30.0f));
					EditorGUI.indentLevel--;
					
				}
				GUILayout.EndHorizontal();

				if (_self.colour.isOpen)
				{
					EditorGUILayout.Space();
					EditorGUI.indentLevel++;

					_self.colour.startColor = EditorGUILayout.ColorField("Start Color", _self.colour.startColor);
					_self.colour.useChildSprite = EditorGUILayout.Toggle("Use Child Sprite", _self.colour.useChildSprite);

					EditorGUILayout.Space();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.colour.gradientColor = EditorGUILayout.BeginToggleGroup("Gradient Color", _self.colour.gradientColor);
						EditorGUI.indentLevel++;
						if (_self.colour.gradientColor)
						{
#if UNITY_2018_3_OR_NEWER
							EditorGUILayout.Space();
							_self.colour.gradient = EditorGUILayout.GradientField("Gradient Color", _self.colour.gradient);
							EditorGUILayout.Space();
#else
							EditorGUILayout.HelpBox("Gradient editor not supported in the Unity version lower than 2018.3.", MessageType.Warning);
#endif
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.colour.animateColor = EditorGUILayout.BeginToggleGroup("Animate Color", _self.colour.animateColor);
						EditorGUI.indentLevel++;
						if (_self.colour.animateColor)
						{
							EditorGUILayout.Space();
							_self.colour.endColor = EditorGUILayout.ColorField("End Color", _self.colour.endColor);
							_self.colour.curveColor = EditorGUILayout.CurveField("Curve", _self.colour.curveColor);
							EditorGUILayout.Space();
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();

					EditorGUI.indentLevel--;
				}
			}
			GUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				_isDirty = true;
			}
		}

		private void DrawScaleComponent(string aTitle)
		{
			var color = GUI.color;
			GUI.color = (_self.scale.isExists) ? _green : color;
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				GUI.color = color;
				GUILayout.BeginHorizontal();
				{
					EditorGUI.indentLevel++;
					_self.scale.isOpen = EditorGUILayout.Foldout(_self.scale.isOpen, aTitle, true);
					_self.scale.isExists = EditorGUILayout.Toggle(_self.scale.isExists, GUILayout.MaxWidth(30.0f));
					EditorGUI.indentLevel--;
					
				}
				GUILayout.EndHorizontal();

				if (_self.scale.isOpen)
				{
					EditorGUILayout.Space();
					EditorGUI.indentLevel++;

					_self.scale.startScale = EditorGUILayout.Vector2Field("Start Scale", _self.scale.startScale);
					_self.scale.rndToScaleX = EditorGUILayout.Vector2Field("Rnd to Scale X", _self.scale.rndToScaleX);
					_self.scale.rndToScaleY = EditorGUILayout.Vector2Field("Rnd to Scale Y", _self.scale.rndToScaleY);
					EditorGUILayout.Space();

					_self.scale.proportional = EditorGUILayout.Toggle("Proportional", _self.scale.proportional);
					_self.scale.useChildSprite = EditorGUILayout.Toggle("Use Child Sprite", _self.scale.useChildSprite);
					EditorGUILayout.Space();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.scale.animateScale = EditorGUILayout.BeginToggleGroup("Animate Scale", _self.scale.animateScale);
						EditorGUI.indentLevel++;
						if (_self.scale.animateScale)
						{
							EditorGUILayout.Space();
							_self.scale.effectLifeTime = EditorGUILayout.Toggle("Life Time of Effect", _self.scale.effectLifeTime);
							_self.scale.endScale = EditorGUILayout.Vector2Field("End Scale", _self.scale.endScale);
							_self.scale.scaleCurveX = EditorGUILayout.CurveField("Curve X", _self.scale.scaleCurveX);
							_self.scale.scaleCurveY = EditorGUILayout.CurveField("Curve Y", _self.scale.scaleCurveY);
							EditorGUILayout.Space();
							
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();

					EditorGUI.indentLevel--;
				}
			}
			GUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				_isDirty = true;
			}
		}

		private void DrawRotationComponent(string aTitle)
		{
			var color = GUI.color;
			GUI.color = (_self.rotation.isExists) ? _green : color;
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				GUI.color = color;
				GUILayout.BeginHorizontal();
				{
					EditorGUI.indentLevel++;
					_self.rotation.isOpen = EditorGUILayout.Foldout(_self.rotation.isOpen, aTitle, true);
					_self.rotation.isExists = EditorGUILayout.Toggle(_self.rotation.isExists, GUILayout.MaxWidth(30.0f));
					EditorGUI.indentLevel--;
					
				}
				GUILayout.EndHorizontal();

				if (_self.rotation.isOpen)
				{
					EditorGUILayout.Space();
					EditorGUI.indentLevel++;

					_self.rotation.startAngle = EditorGUILayout.Slider("Start Angle", _self.rotation.startAngle, -180.0f, 180.0f);
					_self.rotation.rndToAngle = EditorGUILayout.Vector2Field("Rnd to Angle", _self.rotation.rndToAngle);
					_self.rotation.inheritRotation = EditorGUILayout.Toggle("Inherit Rotation", _self.rotation.inheritRotation);

					EditorGUILayout.Space();
					GUI.enabled = !_self.rotation.animateAngle;
					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.rotation.enableRotation = EditorGUILayout.BeginToggleGroup("Animate Rotation", _self.rotation.enableRotation);
						EditorGUI.indentLevel++;
						if (_self.rotation.enableRotation)
						{
							EditorGUILayout.Space();
							_self.rotation.angularSpeed = EditorGUILayout.FloatField("Angular Speed", _self.rotation.angularSpeed);
							_self.rotation.rndToAngularSpeed = EditorGUILayout.Vector2Field("Rnd to Speed", _self.rotation.rndToAngularSpeed);
							_self.rotation.accel = EditorGUILayout.FloatField("Accel", _self.rotation.accel);
							_self.rotation.drag = EditorGUILayout.FloatField("Drag", _self.rotation.drag);
							EditorGUILayout.Space();
							
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();
					
					GUI.enabled = !_self.rotation.enableRotation;
					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.rotation.animateAngle = EditorGUILayout.BeginToggleGroup("Animate Angle", _self.rotation.animateAngle);
						EditorGUI.indentLevel++;
						if (_self.rotation.animateAngle)
						{
							EditorGUILayout.Space();
							_self.rotation.endAngle = EditorGUILayout.FloatField("End Angle", _self.rotation.endAngle);
							_self.rotation.curveAngle = EditorGUILayout.CurveField("Curve", _self.rotation.curveAngle);
							EditorGUILayout.Space();
							
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();
					GUI.enabled = true;

					EditorGUI.indentLevel--;
				}

				if ((_self.rotation.enableRotation || _self.rotation.animateAngle) && 
					(_self.movement.gravity && _self.movement.rotate))
				{
					EditorGUILayout.HelpBox("Animation for rotation will not work as it will be overwritten by the `Movement` component. Remove the checkbox to `Rotate Particle` so that the angle animation is available.", MessageType.Warning);
				}

				if (_self.physicImpulse.isExists && _self.rotation.isExists)
				{
					EditorGUILayout.HelpBox("To avoid unnecessary physical recalculations, avoid rotation settings in this component, use the angle settings in the `Physic Impulse` component.", MessageType.Warning);
				}
			}
			GUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				_isDirty = true;
			}
		}

		private void DrawMovementComponent(string aTitle)
		{
			var color = GUI.color;
			GUI.color = (_self.movement.isExists) ? _green : color;
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				GUI.color = color;
				GUILayout.BeginHorizontal();
				{
					EditorGUI.indentLevel++;
					_self.movement.isOpen = EditorGUILayout.Foldout(_self.movement.isOpen, aTitle, true);
					_self.movement.isExists = EditorGUILayout.Toggle(_self.movement.isExists, GUILayout.MaxWidth(30.0f));
					EditorGUI.indentLevel--;
					
				}
				GUILayout.EndHorizontal();

				if (_self.movement.isOpen)
				{
					EditorGUILayout.Space();
					EditorGUI.indentLevel++;

					_self.movement.speed = EditorGUILayout.FloatField("Speed", _self.movement.speed);
					_self.movement.rndToSpeed = EditorGUILayout.Vector2Field("Rnd to Speed", _self.movement.rndToSpeed);

					GUI.enabled = !_self.movement.gravity && !_self.movement.animateSpeed;
					_self.movement.accel = EditorGUILayout.FloatField("Accel", _self.movement.accel);
					_self.movement.drag = EditorGUILayout.FloatField("Drag", _self.movement.drag);
					GUI.enabled = true;
					EditorGUILayout.Space();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.movement.animateSpeed = EditorGUILayout.BeginToggleGroup("Animate Speed", _self.movement.animateSpeed);
						EditorGUI.indentLevel++;
						if (_self.movement.animateSpeed)
						{
							EditorGUILayout.Space();
							_self.movement.endSpeed = EditorGUILayout.FloatField("End Angle", _self.movement.endSpeed);
							_self.movement.speedCurve = EditorGUILayout.CurveField("Curve", _self.movement.speedCurve);
							EditorGUILayout.Space();
							
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.movement.gravity = EditorGUILayout.BeginToggleGroup("Gravity", _self.movement.gravity);
						EditorGUI.indentLevel++;
						if (_self.movement.gravity)
						{
							EditorGUILayout.Space();
							_self.movement.gravityFactor = EditorGUILayout.Vector2Field("Gravity Factor", _self.movement.gravityFactor);
							_self.movement.rotate = EditorGUILayout.Toggle("Rotate Particle", _self.movement.rotate);
							EditorGUILayout.Space();
							
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();

					if (_self.movement.gravity && _self.movement.rotate)
					{
						EditorGUI.indentLevel--;
						EditorGUILayout.HelpBox("Rotation of the Particle will be depends from the velocity direction.", MessageType.Info);
						EditorGUI.indentLevel++;
					}

					EditorGUI.indentLevel--;
				}
			}
			GUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				_isDirty = true;
			}
		}

		private void DrawPhysicSettingsComponent(string aTitle)
		{
			var color = GUI.color;
			GUI.color = (_self.physicSettings.isExists) ? _green : color;
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				GUI.color = color;
				GUILayout.BeginHorizontal();
				{
					EditorGUI.indentLevel++;
					_self.physicSettings.isOpen = EditorGUILayout.Foldout(_self.physicSettings.isOpen, aTitle, true);
					_self.physicSettings.isExists = EditorGUILayout.Toggle(_self.physicSettings.isExists, GUILayout.MaxWidth(30.0f));
					EditorGUI.indentLevel--;
					
				}
				GUILayout.EndHorizontal();

				if (_self.physicSettings.isOpen)
				{
					EditorGUILayout.Space();
					EditorGUI.indentLevel++;

					_self.physicSettings.freezeRotation = EditorGUILayout.Toggle("Freeze Rotation", _self.physicSettings.freezeRotation);

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.physicSettings.customMass = EditorGUILayout.BeginToggleGroup("Custom Mass", _self.physicSettings.customMass);
						EditorGUI.indentLevel++;
						if (_self.physicSettings.customMass)
						{
							EditorGUILayout.Space();
							_self.physicSettings.autoMass = EditorGUILayout.Toggle("Auto Mass", _self.physicSettings.autoMass);
							GUI.enabled = !_self.physicSettings.autoMass;
							_self.physicSettings.mass = EditorGUILayout.FloatField("Mass", _self.physicSettings.mass);
							_self.physicSettings.rndToMass = EditorGUILayout.Vector2Field("Rnd to Mass", _self.physicSettings.rndToMass);
							GUI.enabled = true;
							EditorGUILayout.Space();
							
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.physicSettings.customLinearDrag = EditorGUILayout.BeginToggleGroup("Custom Linear Drag", _self.physicSettings.customLinearDrag);
						EditorGUI.indentLevel++;
						if (_self.physicSettings.customLinearDrag)
						{
							EditorGUILayout.Space();
							_self.physicSettings.linearDrag = EditorGUILayout.FloatField("Linear Drag", _self.physicSettings.linearDrag);
							_self.physicSettings.rndToLinearDrag = EditorGUILayout.Vector2Field("Rnd to Linear Drag", _self.physicSettings.rndToLinearDrag);
							EditorGUILayout.Space();
							
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.physicSettings.customAngularDrag = EditorGUILayout.BeginToggleGroup("Custom Angular Drag", _self.physicSettings.customAngularDrag);
						EditorGUI.indentLevel++;
						if (_self.physicSettings.customAngularDrag)
						{
							EditorGUILayout.Space();
							_self.physicSettings.angularDrag = EditorGUILayout.FloatField("Angular Drag", _self.physicSettings.angularDrag);
							_self.physicSettings.rndToAngularDrag = EditorGUILayout.Vector2Field("Rnd to Angular Drag", _self.physicSettings.rndToAngularDrag);
							EditorGUILayout.Space();
							
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.physicSettings.customGravityScale = EditorGUILayout.BeginToggleGroup("Custom Gravity Scale", _self.physicSettings.customGravityScale);
						EditorGUI.indentLevel++;
						if (_self.physicSettings.customGravityScale)
						{
							EditorGUILayout.Space();
							_self.physicSettings.gravityScale = EditorGUILayout.FloatField("Gravity Scale", _self.physicSettings.gravityScale);
							_self.physicSettings.rndToGravityScale = EditorGUILayout.Vector2Field("Rnd to Gravity Scale", _self.physicSettings.rndToGravityScale);
							EditorGUILayout.Space();
							
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.physicSettings.lifeTimeLoss = EditorGUILayout.BeginToggleGroup("Life Time Loss", _self.physicSettings.lifeTimeLoss);
						EditorGUI.indentLevel++;
						if (_self.physicSettings.lifeTimeLoss)
						{
							EditorGUILayout.Space();
							_self.physicSettings.amountTimeLosing = EditorGUILayout.FloatField("Time Losing", _self.physicSettings.amountTimeLosing);
							EditorGUILayout.Space();
							
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();

					EditorGUI.indentLevel--;
				}
			}
			GUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				_isDirty = true;
			}
		}

		private void DrawPhysicImpulseComponent(string aTitle)
		{
			var color = GUI.color;
			GUI.color = (_self.physicImpulse.isExists) ? _green : color;
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				GUI.color = color;
				GUILayout.BeginHorizontal();
				{
					EditorGUI.indentLevel++;
					_self.physicImpulse.isOpen = EditorGUILayout.Foldout(_self.physicImpulse.isOpen, aTitle, true);
					_self.physicImpulse.isExists = EditorGUILayout.Toggle(_self.physicImpulse.isExists, GUILayout.MaxWidth(30.0f));
					EditorGUI.indentLevel--;
					
				}
				GUILayout.EndHorizontal();

				if (_self.physicImpulse.isOpen)
				{
					EditorGUILayout.Space();
					EditorGUI.indentLevel++;

					_self.physicImpulse.force = EditorGUILayout.FloatField("Force", _self.physicImpulse.force);
					_self.physicImpulse.rndToForce = EditorGUILayout.Vector2Field("Rnd to Force", _self.physicImpulse.rndToForce);
					EditorGUILayout.Space();

					GUI.enabled = !_self.physicImpulse.useParticleAngle;
					_self.physicImpulse.startAngle = EditorGUILayout.Slider("Start Angle", _self.physicImpulse.startAngle, -180.0f, 180.0f);
					GUI.enabled = true;
					_self.physicImpulse.rndToAngleLower = EditorGUILayout.Slider("Rnd to Lower", _self.physicImpulse.rndToAngleLower, -180.0f, 0.0f);
					_self.physicImpulse.rndToAngleUpper = EditorGUILayout.Slider("Rnd to Upper", _self.physicImpulse.rndToAngleUpper, 0.0f, 180.0f);
					_self.physicImpulse.useParticleAngle = EditorGUILayout.Toggle("Use Particle Angle", _self.physicImpulse.useParticleAngle);
					if (_self.physicImpulse.useParticleAngle)
					{
						EditorGUILayout.HelpBox("Warning, if you will set angle via `Rotation Component`, it's may makes additional physic calculations. Use the in internal angle settings.", MessageType.Warning);
					}
					_self.physicImpulse.inheritRotation = EditorGUILayout.Toggle("Inherit Rotation", _self.physicImpulse.inheritRotation);
					EditorGUILayout.Space();

					_self.physicImpulse.rotate = EditorGUILayout.Toggle("Rotate Particle", _self.physicImpulse.rotate);
					GUI.enabled = _self.physicImpulse.rotate;
					_self.physicImpulse.rotateChild = EditorGUILayout.Toggle("Rotate Child", _self.physicImpulse.rotateChild);
					GUI.enabled = true;
					EditorGUILayout.Space();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.physicImpulse.applyToPosition = EditorGUILayout.BeginToggleGroup("Apply to Position", _self.physicImpulse.applyToPosition);
						EditorGUI.indentLevel++;
						if (_self.physicImpulse.applyToPosition)
						{
							EditorGUILayout.Space();
							_self.physicImpulse.position = EditorGUILayout.Vector2Field("Position", _self.physicImpulse.position);
							_self.physicImpulse.rndToPositionX = EditorGUILayout.Vector2Field("Rnd to Pos X", _self.physicImpulse.rndToPositionX);
							_self.physicImpulse.rndToPositionY = EditorGUILayout.Vector2Field("Rnd to Pos Y", _self.physicImpulse.rndToPositionY);
							EditorGUILayout.Space();
							
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.physicImpulse.addTorque = EditorGUILayout.BeginToggleGroup("Add Torque", _self.physicImpulse.addTorque);
						EditorGUI.indentLevel++;
						if (_self.physicImpulse.addTorque)
						{
							EditorGUILayout.Space();
							_self.physicImpulse.torqueForce = EditorGUILayout.FloatField("Torque Force", _self.physicImpulse.torqueForce);
							_self.physicImpulse.rndToTorqueForce = EditorGUILayout.Vector2Field("Rnd to Torque", _self.physicImpulse.rndToTorqueForce);
							EditorGUILayout.Space();
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();

					EditorGUI.indentLevel--;
				}
			}
			GUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				_isDirty = true;
			}
		}

		private void DrawPhysicExplosionComponent(string aTitle)
		{
			var color = GUI.color;
			GUI.color = (_self.physicExplosion.isExists) ? _green : color;
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				GUI.color = color;
				GUILayout.BeginHorizontal();
				{
					EditorGUI.indentLevel++;
					_self.physicExplosion.isOpen = EditorGUILayout.Foldout(_self.physicExplosion.isOpen, aTitle, true);
					_self.physicExplosion.isExists = EditorGUILayout.Toggle(_self.physicExplosion.isExists, GUILayout.MaxWidth(30.0f));
					EditorGUI.indentLevel--;
					
				}
				GUILayout.EndHorizontal();

				if (_self.physicExplosion.isOpen)
				{
					EditorGUILayout.Space();
					EditorGUI.indentLevel++;

					_self.physicExplosion.radius = EditorGUILayout.FloatField("Radius", _self.physicExplosion.radius);
					_self.physicExplosion.force = EditorGUILayout.FloatField("Force", _self.physicExplosion.force);
					_self.physicExplosion.numRays = EditorGUILayout.IntField("Amount of Rays", _self.physicExplosion.numRays);
					_self.physicExplosion.mask = AntLayerMask.LayerMaskField("Mask", (LayerMask) _self.physicExplosion.mask);
					EditorGUILayout.Space();

					_self.physicExplosion.startDecay = EditorGUILayout.FloatField("Start Decay", _self.physicExplosion.startDecay);
					_self.physicExplosion.pixelsPerUnit = EditorGUILayout.FloatField("Pixels Per Unit", _self.physicExplosion.pixelsPerUnit);
					_self.physicExplosion.minDistance = EditorGUILayout.FloatField("Min Distance", _self.physicExplosion.minDistance);

					EditorGUILayout.Space();
					EditorGUI.indentLevel--;
				}
			}
			GUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				_isDirty = true;
			}
		}

		private void DrawSFLightingComponent(string aTitle)
		{
			var color = GUI.color;
			GUI.color = (_self.sfLighting.isExists) ? _green : color;
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				GUI.color = color;
				GUILayout.BeginHorizontal();
				{
					EditorGUI.indentLevel++;
					_self.sfLighting.isOpen = EditorGUILayout.Foldout(_self.sfLighting.isOpen, aTitle, true);
					_self.sfLighting.isExists = EditorGUILayout.Toggle(_self.sfLighting.isExists, GUILayout.MaxWidth(30.0f));
					EditorGUI.indentLevel--;
					
				}
				GUILayout.EndHorizontal();

				if (_self.sfLighting.isOpen)
				{
					EditorGUILayout.Space();
					EditorGUI.indentLevel++;

					_self.sfLighting.startRadius = EditorGUILayout.FloatField("Start Radius", _self.sfLighting.startRadius);
					_self.sfLighting.rndToStartRadius = EditorGUILayout.Vector2Field("Rnd to Radius", _self.sfLighting.rndToStartRadius);
					EditorGUILayout.Space();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.sfLighting.animateRadius = EditorGUILayout.BeginToggleGroup("Animate Radius", _self.sfLighting.animateRadius);
						EditorGUI.indentLevel++;
						if (_self.sfLighting.animateRadius)
						{
							EditorGUILayout.Space();
							_self.sfLighting.endRadius = EditorGUILayout.FloatField("End Radius", _self.sfLighting.endRadius);
							_self.sfLighting.radiusCurve = EditorGUILayout.CurveField("Curve", _self.sfLighting.radiusCurve);
							EditorGUILayout.Space();
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();
					EditorGUILayout.Space();

					_self.sfLighting.startIntensity = EditorGUILayout.FloatField("Start Intensity", _self.sfLighting.startIntensity);
					_self.sfLighting.rndToStartIntensity = EditorGUILayout.Vector2Field("Rnd to Intensity", _self.sfLighting.rndToStartIntensity);
					EditorGUILayout.Space();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.sfLighting.animateIntensity = EditorGUILayout.BeginToggleGroup("Animate Intensity", _self.sfLighting.animateIntensity);
						EditorGUI.indentLevel++;
						if (_self.sfLighting.animateIntensity)
						{
							EditorGUILayout.Space();
							_self.sfLighting.endIntensity = EditorGUILayout.FloatField("End Intensity", _self.sfLighting.endIntensity);
							_self.sfLighting.intensityCurve = EditorGUILayout.CurveField("Curve", _self.sfLighting.intensityCurve);
							EditorGUILayout.Space();
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();
					EditorGUILayout.Space();

					_self.sfLighting.startColor = EditorGUILayout.ColorField("Start Color", _self.sfLighting.startColor);
					EditorGUILayout.Space();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.sfLighting.animateColor = EditorGUILayout.BeginToggleGroup("Animate Intensity", _self.sfLighting.animateColor);
						EditorGUI.indentLevel++;
						if (_self.sfLighting.animateColor)
						{
#if UNITY_2018_3_OR_NEWER
							EditorGUILayout.Space();
							_self.sfLighting.gradient = EditorGUILayout.GradientField("Gradient", _self.sfLighting.gradient);
							EditorGUILayout.Space();
#else
							EditorGUILayout.HelpBox("Gradient editor not supported in the Unity version lower than 2018.3.", MessageType.Warning);
#endif
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();

					EditorGUI.indentLevel--;
				}
			}
			GUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				_isDirty = true;
			}
		}

		private void DrawTrailComponent(string aTitle)
		{
			var color = GUI.color;
			GUI.color = (_self.trail.isExists) ? _green : color;
			EditorGUI.BeginChangeCheck();
			GUILayout.BeginVertical(EditorStyles.helpBox);
			{
				GUI.color = color;
				GUILayout.BeginHorizontal();
				{
					EditorGUI.indentLevel++;
					_self.trail.isOpen = EditorGUILayout.Foldout(_self.trail.isOpen, aTitle, true);
					_self.trail.isExists = EditorGUILayout.Toggle(_self.trail.isExists, GUILayout.MaxWidth(30.0f));
					EditorGUI.indentLevel--;
					
				}
				GUILayout.EndHorizontal();

				if (_self.trail.isOpen)
				{
					EditorGUILayout.Space();
					EditorGUI.indentLevel++;

					_self.trail.startTime = EditorGUILayout.FloatField("Start Time", _self.trail.startTime);
					_self.trail.rndToStartTime = EditorGUILayout.Vector2Field("Rnd to Time", _self.trail.rndToStartTime);
					EditorGUILayout.Space();

					GUILayout.BeginVertical(EditorStyles.helpBox);
					{
						EditorGUI.indentLevel--;
						_self.trail.animateTime = EditorGUILayout.BeginToggleGroup("Animate Time", _self.trail.animateTime);
						EditorGUI.indentLevel++;
						if (_self.trail.animateTime)
						{
							EditorGUILayout.Space();
							_self.trail.endTime = EditorGUILayout.FloatField("End Time", _self.trail.endTime);
							_self.trail.timeCurve = EditorGUILayout.CurveField("Curve", _self.trail.timeCurve);
							EditorGUILayout.Space();
						}
						EditorGUILayout.EndToggleGroup();
					}
					GUILayout.EndVertical();
					EditorGUILayout.Space();

					_self.trail.startWidth = EditorGUILayout.FloatField("Start Width", _self.trail.startWidth);
					_self.trail.rndToStartWidth = EditorGUILayout.Vector2Field("Rnd to Width", _self.trail.rndToStartWidth);
					_self.trail.widthCurve = EditorGUILayout.CurveField("Curve Width", _self.trail.widthCurve);
					EditorGUILayout.Space();
					
#if UNITY_2018_3_OR_NEWER
					_self.trail.gradient = EditorGUILayout.GradientField("Gradient", _self.trail.gradient);
#else
							EditorGUILayout.HelpBox("Gradient editor not supported in the Unity version lower than 2018.3.", MessageType.Warning);
#endif
					EditorGUILayout.Space();

					EditorGUI.indentLevel--;
				}
			}
			GUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				_isDirty = true;
			}
		}

		private bool IsParticleAlreadyAdded(GameObject aObject)
		{
			int index = System.Array.FindIndex(_self.source.prefabs, x => System.Object.ReferenceEquals(x, aObject));
			return (index >= 0 && index < _self.source.prefabs.Length);
		}

		#endregion
	}
}