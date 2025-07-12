using System.Collections;
using UnityEngine;
namespace LowPolyFirearms.WeaponSystem
{
	public class TestHit : MonoBehaviour
	{
		public string bulletTag = "Bullet";
		public bool destroyOnHit = true;

		private Renderer targetRenderer;
		private Color originalColor;
		private Coroutine colorCoroutine;

		private void Start()
		{
			targetRenderer = GetComponent<Renderer>();
			originalColor = targetRenderer.material.color;
		}
		private void OnCollisionEnter(Collision collision)
		{
			if (collision.gameObject.CompareTag(bulletTag))
			{
				if (destroyOnHit)
				{
					Destroy(collision.gameObject);
				}
				if (colorCoroutine != null)
					StopCoroutine(colorCoroutine);

				colorCoroutine = StartCoroutine(FlashRed());
				OnHit();
			}
		}

		private void OnHit()
		{
			Debug.Log($"{gameObject.name} hit!");
		}

		private IEnumerator FlashRed()
		{
			targetRenderer.material.color = Color.red;
			yield return new WaitForSeconds(1f);
			targetRenderer.material.color = originalColor;
		}
	}
}