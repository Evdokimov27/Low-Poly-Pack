using UnityEngine;
using UnityEngine.UI;

namespace LowPolyFirearms.WeaponSystem
{
	public class AutoScopeRender : MonoBehaviour
	{
		public Camera scopeCamera;
		public RawImage rawImage;
		public int textureWidth = 128;
		public int textureHeight = 128;
		public RenderTextureFormat textureFormat = RenderTextureFormat.ARGB32;

		public float minZoom = 20f;
		public float maxZoom = 60f;
		[Range(0f, 1f)] public float zoomLerp = 1f;

		private void Start()
		{
			if (scopeCamera == null) return;
			zoomLerp = 1;
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
			if (scopeCamera != null)
			{
				float fov = Mathf.Lerp(minZoom, maxZoom, zoomLerp);
				scopeCamera.fieldOfView = fov;
			}
		}
	}
}
