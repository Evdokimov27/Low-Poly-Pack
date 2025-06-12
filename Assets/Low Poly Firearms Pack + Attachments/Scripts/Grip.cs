using UnityEngine;

public class Grip : MonoBehaviour
{
	[Range(0.1f, 1f)]
	public float recoilMultiplier = 0.7f;

	public float GetRecoilMultiplier()
	{
		return recoilMultiplier;
	}
}
