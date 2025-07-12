using UnityEngine;

namespace LowPolyFirearms.WeaponSystem
{
	public class Sight : MonoBehaviour
	{
		[Header("Zoom Settings")]
		[Tooltip("Max level zoom")]
		public float maxZoom = 4f;

		[Tooltip("The step of changing the zoom (for example, 0.25 = 1.25, x1.5...)")]
		public float zoomStep = 0.25f;

		[Tooltip("Current level zoom (x1 — no zoom)")]
		public float zoomLevel = 1f;

		private AutoScopeRender zoom;

		private void Start()
		{
			zoom = GetComponentInChildren<AutoScopeRender>();
			if (zoom != null)
			{
				zoom.zoomLevel = zoomLevel;
				zoom.ApplyZoom();
			}
		}

		public void ToggleAiming()
		{
			float scroll = Input.GetAxis("Mouse ScrollWheel");

			if (Mathf.Abs(scroll) > 0.01f)
			{
				zoomLevel += scroll > 0 ? zoomStep : -zoomStep;
				zoomLevel = Mathf.Clamp(zoomLevel, 1f, maxZoom);
			}

			if (zoom != null)
			{
				zoom.zoomLevel = zoomLevel;
				zoom.ApplyZoom();
			}
		}
	}
}
