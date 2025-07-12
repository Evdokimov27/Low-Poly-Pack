using System.Collections.Generic;
using UnityEngine;
namespace LowPolyFirearms.WeaponSystem
{
	public class TestGun : MonoBehaviour
	{
		public Camera aimCamera;
		public List<Camera> cameras = new List<Camera>();
		public List<Weapon> weapons = new List<Weapon>();

		private void Start()
		{
			ActivateCamera(0);
		}

		public void ActivateCamera(int index)
		{
			
			for (int i = 0; i < cameras.Count; i++)
			{
				if (index != 5)
				{
					if (cameras[i] != null)
					{
						cameras[i].enabled = (i == index);
					}
					aimCamera.enabled = false;

				}
				else
				{
					aimCamera.enabled = true;
				}
			
			}
		}

		public void RandomizeAttachment()
		{
			foreach (var weapon in weapons)
			{
				if (weapon != null)
				{
					weapon.ActivateRandomAttachments();
				}
			}
		}
	}
}