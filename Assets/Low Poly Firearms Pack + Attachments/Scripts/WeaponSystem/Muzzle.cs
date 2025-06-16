using UnityEngine;
namespace LowPolyFirearms.WeaponSystem
{
	public class Muzzle : MonoBehaviour
	{
		[Range(0.1f, 1f)]
		public float recoilMultiplier = 0.8f;
		public float maxFlashEffect = 0.5f;
		public bool isSilence = false;
		public float GetRecoilMultiplier()
		{
			return recoilMultiplier;
		}
		public float GetFlashEffect()
		{
			return maxFlashEffect;
		}
	}
}