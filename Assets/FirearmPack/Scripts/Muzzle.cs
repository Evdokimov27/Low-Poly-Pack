using UnityEngine;

public class Muzzle : MonoBehaviour
{
	[Range(0.1f, 1f)]
	public float recoilMultiplier = 0.8f;

	public float GetRecoilMultiplier()
	{
		return recoilMultiplier;
	}
}
