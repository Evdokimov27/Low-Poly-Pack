using UnityEngine;
using UnityEngine.EventSystems;

public class Weapon : MonoBehaviour
{
	[Header("Can Shoot?")]
	public bool isSelected = false;
	[Header("Optional modules")]
	public Magazine magazine;
	public Magazine additional_magazine;
	public Sight sight;
	public Grip grip;
	public Muzzle muzzle;

	[Header("Weapon stats")]
	public float baseRecoil = 5f; 
	public float totalRecoil = 5f; 
	public float recoilReturnSpeed = 5f;

	[Header("Required field")]
	public bool isActive = false;

	[Header("Required field")]
	public Camera playerCamera;
	private Transform weaponTransform; 
	public Transform firePoint;
	[Header("Supported magazine types")]
	public MagazineType[] supportedMagazineTypes;
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
	private bool IsMagazineCompatible(Magazine mag)
	{
		foreach (var type in supportedMagazineTypes)
		{
			if (mag.magazineType == type)
				return true;
		}
		return false;
	}
	void Update()
	{
		if (isSelected && !IsUIClick())
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
		if (magazine != null && !IsMagazineCompatible(magazine))
		{
			Debug.LogWarning("Incompatible magazine: " + magazine.name);
			magazine = null;
		}
		if (additional_magazine != null && !IsMagazineCompatible(additional_magazine))
		{
			Debug.LogWarning("Incompatible additional magazine: " + additional_magazine.name);
			additional_magazine = null;
		}
	}
	public bool IsUIClick()
	{
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return true;
		}
		return false;
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
		if (additional_magazine != null && additional_magazine.ConsumeAmmo())
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

	[ContextMenu("Random attachment")]
	public void ActivateRandomAttachments()
	{
		EnableRandomChild("_cb_canted_1", ref sight);
		EnableRandomChild("_cb_grip_1", ref grip);
		EnableRandomChild("_cb_suppressor_1", ref muzzle);
		EnableRandomChild("_ñb_magazine_1", ref magazine);
		EnableRandomChild("_ñb_magazine_add", ref additional_magazine);
	}

	private void EnableRandomChild<T>(string parentName, ref T result, bool disableOthers = true) where T : Component
	{
		Transform parent = transform.Find(parentName);
		if (parent == null)
		{
			Debug.LogWarning("Not found: " + parentName);
			result = null;
			return;
		}

		int count = parent.childCount;
		if (count == 0)
		{
			result = null;
			return;
		}

		if (disableOthers)
		{
			for (int i = 0; i < count; i++)
			{
				parent.GetChild(i).gameObject.SetActive(false);
			}
		}

		int randomIndex = Random.Range(-1, count); 

		if (randomIndex == -1)
		{
			result = null;
			return;
		}

		Transform selected = parent.GetChild(randomIndex);
		selected.gameObject.SetActive(true);

		result = selected.GetComponent<T>();
	}

}
