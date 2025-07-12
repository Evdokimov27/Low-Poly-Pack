using UnityEngine;
namespace LowPolyFirearms.WeaponSystem
{
	public class TestAimingShoot : MonoBehaviour
	{
		public float rotationSpeed = 100f;
		public bool rotateX = true;
		public bool rotateY = true;

		private bool isRotating = false;
		private float rotX = 0f;
		private float rotY = 0f;

		void Update()
		{
			if (Input.GetMouseButtonDown(1))
				isRotating = true;
			else if (Input.GetMouseButtonUp(1))
				isRotating = false;

			if (isRotating)
			{
				float mouseX = Input.GetAxis("Mouse X");
				float mouseY = Input.GetAxis("Mouse Y");

				if (rotateX)
					rotY += mouseX * rotationSpeed * Time.deltaTime;
				if (rotateY)
					rotX += mouseY * rotationSpeed * Time.deltaTime;

				transform.rotation = Quaternion.Euler(rotX, rotY, 0f);
			}
		}
	}
}