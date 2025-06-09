using UnityEngine;

public enum MagazineType
{
	AK,
	M4,
	Glock,
	MP5,
	UMP45,
	MP7,
	TMP,
	M9,
	P30L,
	P90,
	C14,
	GM6,
	L96A1,
	XM29,
	LMAG,
	M249,
	M1911,
	MK21,
	MK23,
	SWAM,
	AT4,
	M32,
	AA12,
	Javelin,
	None
}


public class Magazine : MonoBehaviour
{
	public MagazineType magazineType;

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
