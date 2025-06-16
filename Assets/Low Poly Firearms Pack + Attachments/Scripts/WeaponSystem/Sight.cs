using UnityEngine;
namespace LowPolyFirearms.WeaponSystem
{
	public class Sight : MonoBehaviour
	{
		[Header("Setting Zoom")]
		public float maxZoom = 100f;
		public float minZoom = 10f;

		[Range(0f, 1f)]
		public float currentZoom = 1f;

		public float zoomSpeed = 0.1f;

		private AutoScopeRender zoom;

		private void Start()
		{
			zoom = GetComponentInChildren<AutoScopeRender>();
			if (zoom != null)
			{
				currentZoom = 1;
				zoom.maxZoom = maxZoom;
				zoom.minZoom = minZoom;
			}
		}

		public void ToggleAiming()
		{
			float scroll = Input.GetAxis("Mouse ScrollWheel");
			if (Mathf.Abs(scroll) > 0.01f)
			{
				currentZoom -= scroll * zoomSpeed;
				currentZoom = Mathf.Clamp01(currentZoom);
			}

			if (zoom != null)
			{
				zoom.zoomLerp = currentZoom;
				zoom.ApplyZoom();
			}
		}
	}
}