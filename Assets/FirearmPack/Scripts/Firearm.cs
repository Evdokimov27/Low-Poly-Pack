using UnityEngine;

public class Weapon : MonoBehaviour
{
	[Header("Optional modules")]
	public Magazine magazine;
	public Sight sight;
	public Grip grip;
	public Muzzle muzzle;

	[Header("Weapon stats")]
	public float baseRecoil = 5f; 
	public float totalRecoil = 5f; 
	public float recoilReturnSpeed = 5f;

	[Header("Required field")]
	public Camera playerCamera;
	private Transform weaponTransform; 
	public Transform firePoint;

	private Quaternion originalRotation;
	private Quaternion currentRecoilRotation;
	private bool isRecoiling = false;

	void Start()
	{
		if (weaponTransform == null)
			weaponTransform = this.transform;

		originalRotation = weaponTransform.localRotation;
		currentRecoilRotation = originalRotation;
	}

	void Update()
	{
		if (Input.GetButtonDown("Fire1") && (magazine == null || magazine.HasAmmo()))
		{
			Fire();
		}
		if (Input.GetButton("Fire2") && sight)
		{
			sight.ToggleAiming();
		}
	}

	void LateUpdate()
	{
		if (!isRecoiling) return;

		currentRecoilRotation = Quaternion.Slerp(currentRecoilRotation, originalRotation, Time.deltaTime * recoilReturnSpeed);
		weaponTransform.localRotation = currentRecoilRotation;

		if (Quaternion.Angle(currentRecoilRotation, originalRotation) < 0.1f)
		{
			currentRecoilRotation = originalRotation;
			isRecoiling = false;
		}
	}

	void Fire()
	{
		totalRecoil = baseRecoil;
		totalRecoil *= grip != null ? grip.GetRecoilMultiplier() : 1f;
		totalRecoil *= muzzle != null ? muzzle.GetRecoilMultiplier() : 1f;
		if (magazine != null && magazine.ConsumeAmmo())
		{
			ApplyRecoil(totalRecoil);
		}
	}

	void ApplyRecoil(float recoil)
	{
		Quaternion recoilRot = Quaternion.Euler(recoil, 0f, 0f);
		currentRecoilRotation = weaponTransform.localRotation * recoilRot;
		weaponTransform.localRotation = currentRecoilRotation;
		isRecoiling = true;
	}
}
