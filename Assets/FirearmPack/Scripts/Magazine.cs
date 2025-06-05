using UnityEngine;

public class Magazine : MonoBehaviour
{
	public int maxAmmo = 30;
	public int currentAmmo;

	void Start()
	{
		currentAmmo = maxAmmo;
	}

	public bool HasAmmo()
	{
		return currentAmmo > 0;
	}

	public bool ConsumeAmmo()
	{
		if (currentAmmo > 0)
		{
			currentAmmo--;
			return true;
		}
		else
			return false;
	}

	public void Reload()
	{
		currentAmmo = maxAmmo;
	}

	public int GetAmmo()
	{
		return currentAmmo;
	}
}
