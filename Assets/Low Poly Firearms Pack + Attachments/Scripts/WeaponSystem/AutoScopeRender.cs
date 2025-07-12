using UnityEngine;
using UnityEngine.UI;

namespace LowPolyFirearms.WeaponSystem
{
	public class AutoScopeRender : MonoBehaviour
	{
		[Header("Scope Camera Setup")]
		public Camera scopeCamera;
		public RawImage rawImage;
		public int textureWidth = 128;
		public int textureHeight = 128;
		public RenderTextureFormat textureFormat = RenderTextureFormat.ARGB32;

		[Header("Zoom")]
		[Tooltip("FOV at x1 (no zoom)")]
		public float baseFOV = 60f;

		[Tooltip("Current level zoom (x-zoom)")]
		[Range(1f, 18f)] public float zoomLevel = 1f;

		private void Start()
		{
			if (scopeCamera == null) return;

			RenderTexture rt = new RenderTexture(textureWidth, textureHeight, 16, textureFormat);
			rt.name = "ScopeRT_" + gameObject.name;
			rt.Create();

			scopeCamera.targetTexture = rt;
			rawImage.texture = rt;

			ApplyZoom();
		}

		private void OnValidate()
		{
			ApplyZoom();
		}

		public void ApplyZoom()
		{
			if (scopeCamera != null && zoomLevel >= 1f)
			{
				float fov = baseFOV / zoomLevel;
				scopeCamera.fieldOfView = fov;
			}
		}
	}
}
