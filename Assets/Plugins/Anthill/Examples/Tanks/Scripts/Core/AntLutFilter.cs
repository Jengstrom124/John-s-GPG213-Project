namespace Anthill.Filters
{
	using UnityEngine;

	#if UNITY_EDITOR
	using UnityEditor;
	#endif

	[ExecuteInEditMode]
	public class AntLutFilter : MonoBehaviour
	{
		public Shader shader;
		public Texture2D lutTexture;
		[Range(0.0f, 1.0f)]
		public float blend = 1.0f;
		[Range(0.0f, 3.0f)]
		public float originalIntensity = 1.0f;
		[Range(-1.0f, 1.0f)]
		public float resultIntensity;
		[Range(-1.0f, 1.0f)]
		public float finalIntensity;

		private const string SHADER_NAME = "Anthill/LutFilter";
		private float _time;
		private Material _material;
		private Texture3D _3Dlut;
		private string _memoPath;
		private bool _isInitialized;

		#region Unity Calls

		private void Start()
		{
			shader = Shader.Find(SHADER_NAME);
			if (shader != null)
			{
				_material = new Material(shader);
				_material.hideFlags = HideFlags.HideAndDontSave;
				_isInitialized = true;
			}
			else
			{
				A.Warning($"Shader `{SHADER_NAME}` not found!");
				_isInitialized = false;
				enabled = false;
			}
		}

		#if UNITY_EDITOR
		private void Update()
		{
			if (!Application.isPlaying)
			{
				shader = Shader.Find(SHADER_NAME);
			}
		}

		private void OnValidate()
		{
			string path = AssetDatabase.GetAssetPath(lutTexture);
			if (_memoPath != path)
			{
				Convert(lutTexture);
			}
		}
		#endif

		private void OnRenderImage(RenderTexture aSourceTexture, RenderTexture aTargetTexture)
		{
			if (_isInitialized)
			{
				_time += Time.deltaTime;
				if (_time > 100.0f)
				{
					_time = 0.0f;
				}

				if (_3Dlut == null)
				{
					Convert(lutTexture);
				}

				// int lutSize = _3Dlut.width;
				_3Dlut.wrapMode = TextureWrapMode.Clamp;
				_material.SetTexture("_LutTex", _3Dlut);
				_material.SetFloat("_Blend", blend);
				_material.SetFloat("_Intensity", originalIntensity);
				_material.SetFloat("_Extra", resultIntensity);
				_material.SetFloat("_Extra2", finalIntensity);
				Graphics.Blit(aSourceTexture, aTargetTexture, _material, QualitySettings.activeColorSpace == ColorSpace.Linear ? 1 : 0);
			}
			else
			{
				Graphics.Blit(aSourceTexture, aTargetTexture);
			}
		}
		
		#endregion

		private void SetIdentityLut()
		{
			const int dim = 16;
			var newC = new Color[dim * dim * dim];
			const float oneOverDim = 1.0f / (1.0f * dim - 1.0f);

			for (int i = 0; i < dim; i++)
			{
				for (int j = 0; j < dim; j++)
				{
					for (int k = 0; k < dim; k++)
					{
						newC[i + (j * dim) + (k * dim * dim)] = new Color((i * 1.0f) * oneOverDim, (j * 1.0f) * oneOverDim, (k * 1.0f) * oneOverDim, 1.0f);
					}
				}
			}

			if (_3Dlut)
			{
				DestroyImmediate(_3Dlut);
			}

			_3Dlut = new Texture3D(dim, dim, dim, TextureFormat.ARGB32, false);
			_3Dlut.SetPixels(newC);
			_3Dlut.Apply();
		}

		private bool ValidDimensions(Texture2D aTexture2D)
		{
			return (aTexture2D != null)
				? (aTexture2D.height == Mathf.FloorToInt(Mathf.Sqrt(aTexture2D.width)))
				: false;
		}

		private void Convert(Texture2D aTexture2D)
		{
			if (aTexture2D != null)
			{
				int dim = aTexture2D.height;
				if (!ValidDimensions(aTexture2D))
				{
					A.Warning($"The given 2D texture `{aTexture2D.name}` cannot be used as a 3D LUT.");
					return;
				}

				#if UNITY_EDITOR
				if (Application.isPlaying)
				{
					string path = AssetDatabase.GetAssetPath(lutTexture);
					_memoPath = path;
					var textureImporter = (TextureImporter) AssetImporter.GetAtPath(path);
					if (!textureImporter.isReadable)
					{
						textureImporter.isReadable = true;
						textureImporter.mipmapEnabled = false;
						textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
						AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
					}
				}
				#endif

				var c = aTexture2D.GetPixels();
				var newC = new Color[c.Length];
				for (int i = 0; i < dim; i++)
				{
					for (int j = 0; j < dim; j++)
					{
						for (int k = 0; k < dim; k++)
						{
							int j_ = dim - j - 1;
							newC[i + (j * dim) + (k * dim * dim)] = c[k * dim + i + j_ * dim * dim];
						}
					}
				}

				if (_3Dlut)
				{
					DestroyImmediate(_3Dlut);
				}
				_3Dlut = new Texture3D(dim, dim, dim, TextureFormat.ARGB32, false);
				_3Dlut.SetPixels(newC);
				_3Dlut.Apply();
			}
			else
			{
				SetIdentityLut();
			}
		}
	}
}