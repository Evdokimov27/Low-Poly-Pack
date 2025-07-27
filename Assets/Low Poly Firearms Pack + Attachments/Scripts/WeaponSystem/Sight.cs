using UnityEngine;

namespace LowPolyFirearms.WeaponSystem
{
	public class Sight : MonoBehaviour
	{
		[Header("Zoom Settings")]
		[Tooltip("Min level zoom")]
		public float minZoom = 4f;

		[Tooltip("Max level zoom")]
		public float maxZoom = 16f;

		[Tooltip("The step of changing the zoom (for example, 0.25 = 4.25, 4.5...)")]
		public float zoomStep = 0.25f;

		private float zoomLevel = 4f;

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
				zoomLevel = Mathf.Clamp(zoomLevel, minZoom, maxZoom);
			}

			if (zoom != null)
			{
				zoom.zoomLevel = zoomLevel;
				zoom.ApplyZoom();
			}
		}
	}
}
