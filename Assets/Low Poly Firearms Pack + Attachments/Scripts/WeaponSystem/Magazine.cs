using UnityEngine;

namespace LowPolyFirearms.WeaponSystem
{
	public class Magazine : MonoBehaviour
	{
		public Bullet bullet;
		public int maxAmmo = 30;
		public int currentAmmo;

		void Start()
		{
			currentAmmo = maxAmmo;
		}
		public bool HasAmmo() => currentAmmo > 0;
		public bool ConsumeAmmo()
		{
			if (currentAmmo > 0)
			{
				currentAmmo--;
				return true;
			}
			return false;
		}
		public void Reload() => currentAmmo = maxAmmo;
		public int GetAmmo() => currentAmmo;
	}
}