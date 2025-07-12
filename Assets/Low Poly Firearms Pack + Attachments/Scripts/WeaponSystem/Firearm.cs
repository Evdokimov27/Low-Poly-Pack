using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LowPolyFirearms.WeaponSystem
{
	[RequireComponent(typeof(RecoilHandler))]
	[RequireComponent(typeof(AudioSource))]
	public class Weapon : MonoBehaviour
	{
		public enum FireMode { Single, Burst, Auto, Bolt }

		[Header("Can Shoot?")]
		public bool isSelected = false;
		[Header("Draw a bullet trajectory?")]
		public bool drawTrajectory = false;
		public Color colorTrajectory = Color.blue;
		[Header("Draw a bullet trail after shoot?")]
		[SerializeField] private bool showTrail = false;
		[SerializeField] private Color colorTrail = Color.white;
		[Header("Modules")]
		public Magazine magazine;
		public Magazine additionalMagazine;
		public Sight sight;
		public Grip grip;
		public Muzzle muzzle;

		[Header("Audio")]
		public AudioClip shotSound;
		private AudioSource audioSource;

		[Header("Fire mode")]
		public FireMode fireMode = FireMode.Single;
		public List<FireMode> supportedFireModes = new List<FireMode> { FireMode.Single };
		public int burstCount = 3;
		public float fireRate = 0.1f;


		private int burstShotsRemaining = 0;
		private bool isFiring = false;
		private float nextFireTime = 0f;

		private Magazine prevMag;
		private Magazine prevAdditionalMag;
		private RecoilHandler recoilHandler;
		private Transform firePoint;
		private ParticleSystem shootParticle;
		private bool isChambering = false;

		public float totalRecoil;
		void Start()
		{
			audioSource = GetComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.clip = shotSound;

			recoilHandler = GetComponent<RecoilHandler>();
			if (recoilHandler != null)
				recoilHandler.Initialize(transform);

			Reload();
		}
		[ContextMenu("Test GetMag")]
		public void Reload()
		{
			if(magazine == null) magazine = HasMagazine();
		}
		public Magazine HasMagazine()
		{
			var magazineParent = transform.Find("_сb_magazine_1");
			if (magazineParent.transform.childCount > 0)
			{
				return magazineParent.GetComponentInChildren<Magazine>();
			}
			return null;
		}
		void Update()
		{
			if (!isSelected || IsUIClick()) return;

			if (Input.GetKeyDown(KeyCode.V))
				SwitchFireMode();

			if (Input.GetButton("Fire2"))
			{
				sight?.ToggleAiming();
				recoilHandler.SetAiming(true);
			}
			else
				recoilHandler.SetAiming(false);


			switch (fireMode)
			{
				case FireMode.Single:
					if (Input.GetButtonDown("Fire1")) TryFire();
					break;

				case FireMode.Burst:
					if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
					{
						burstShotsRemaining = burstCount;
						isFiring = true;
					}
					break;

				case FireMode.Auto:
					if (Input.GetButton("Fire1")) TryFire();
					break;
				case FireMode.Bolt:

					if (Input.GetButtonDown("Fire1"))
					{
						if (isChambering)
						{
							isChambering = false;
						}
						else TryFire();
					}
					break;
			}

			if (fireMode == FireMode.Burst && isFiring && burstShotsRemaining > 0)
			{
				if (Time.time >= nextFireTime)
				{
					if (TryFire())
					{
						burstShotsRemaining--;
						nextFireTime = Time.time + fireRate;
					}
					else
					{
						burstShotsRemaining = 0;
					}
				}
			}

			if (drawTrajectory)
				DrawBulletTrajectoryGizmo();
		}



		private bool TryFire()
		{
			if (Time.time < nextFireTime) return false;

			bool hasFired = false;
			Bullet bulletToFire = null;

			if (magazine != null && magazine.ConsumeAmmo())
			{
				hasFired = true;
				bulletToFire = magazine.bullet;
			}
			else if (additionalMagazine != null && additionalMagazine.ConsumeAmmo())
			{
				hasFired = true;
				bulletToFire = additionalMagazine.bullet;
			}

			if (hasFired && bulletToFire != null)
			{
				Shoot(bulletToFire);
				if (hasFired && fireMode == FireMode.Bolt)
				{
					isChambering = true;
				}
			}
			return hasFired;
		}

		void Shoot(Bullet bulletToFire)
		{
			firePoint = muzzle ? muzzle.transform.Find("_cb_firepoint") : transform.Find("_cb_firepoint");

			int pelletCount = bulletToFire.pelletCount;

			for (int i = 0; i < pelletCount; i++)
			{
				Vector3 direction = firePoint.forward;
				GameObject bulletGO = Instantiate(bulletToFire.gameObject, firePoint.position, Quaternion.LookRotation(direction));
				ApplyBulletTrail(bulletGO);
				if (bulletGO.TryGetComponent<Bullet>(out var bullet))
					bullet.InitializeDirection(direction);
				Destroy(bulletGO, 15f);
			}

			nextFireTime = Time.time + fireRate;

			totalRecoil = recoilHandler.recoilIntensity;
			totalRecoil *= grip ? grip.GetRecoilMultiplier() : 1f;
			totalRecoil *= muzzle ? muzzle.GetRecoilMultiplier() : 1f;
			totalRecoil *= recoilHandler.isAiming ? recoilHandler.aimMultiplier : 1f;
			recoilHandler?.TriggerRecoil(totalRecoil);

			shootParticle = firePoint.Find("ShootParticle").GetComponent<ParticleSystem>();
			var main = shootParticle.main;
			main.startSizeMultiplier = muzzle ? muzzle.GetFlashEffect() : 2f;
			shootParticle.Play();

			PlayFireSound();
		}

		private void ApplyBulletTrail(GameObject bulletGO)
		{
			if (!showTrail || bulletGO == null) return;

			var trail = bulletGO.AddComponent<TrailRenderer>();
			trail.time = 0.2f;
			trail.startWidth = 0.05f;
			trail.endWidth = 0.01f;
			trail.material = new Material(Shader.Find("Sprites/Default"));
			trail.startColor = colorTrail;
			trail.endColor = new Color(1f, 1f, 1f, 0f);
		}
		private void SwitchFireMode()
		{
			if (supportedFireModes == null || supportedFireModes.Count == 0) return;

			int currentIndex = supportedFireModes.IndexOf(fireMode);
			int nextIndex = (currentIndex + 1) % supportedFireModes.Count;
			fireMode = supportedFireModes[nextIndex];

			Debug.Log("Switched fire mode to: " + fireMode);
		}

		private void PlayFireSound()
		{
			if (shotSound == null || audioSource == null) return;

			audioSource.volume = muzzle != null && muzzle.isSilence ? 0.5f : 1f;
			audioSource.pitch = muzzle != null && muzzle.isSilence ? 1.9f : 1f;
			audioSource.spatialBlend = 1f;
			audioSource.dopplerLevel = 0f;

			audioSource.PlayOneShot(shotSound);
		}



		public bool IsUIClick()
		{
			return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
		}

		[ContextMenu("Random attachment")]
		public void ActivateRandomAttachments()
		{
			EnableRandomChild("_cb_canted_1", ref sight);
			EnableRandomChild("_cb_grip_1", ref grip);
			EnableRandomChild("_cb_suppressor_1", ref muzzle);
			EnableRandomChild("_сb_magazine_1", ref magazine);
			EnableRandomChild("_сb_magazine_add", ref additionalMagazine);
		}
		private void EnableRandomChild<T>(string parentName, ref T result, bool disableOthers = true) where T : Component
		{
			Transform parent = transform.Find(parentName);
			if (parent == null)
			{
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
		void LateUpdate()
		{
			recoilHandler?.ProcessRecoil();
		}

		private void DrawBulletTrajectoryGizmo()
		{
			if (!magazine || !magazine.bullet || firePoint == null) return;

			Bullet bullet = magazine.bullet;

			Vector3 position = firePoint.position;
			Vector3 direction = -firePoint.forward.normalized;

			float distance = 0f;
			float maxDistance = bullet.maxRangeBullet;
			float stepTime = Time.fixedDeltaTime;

			float yaw = 0f;
			float pitch = 0f;

			float spreadProgress = Mathf.Clamp01((distance - bullet.spreadStartDistance) / (maxDistance - bullet.spreadStartDistance));
			float currentSpread = Mathf.Lerp(0f, bullet.maxSpreadAngle, spreadProgress);
			yaw = bullet.speedVariationPercent != 0 ? 0.5f : 0f;
			pitch = bullet.speedVariationPercent != 0 ? 0.5f : 0f;

			Vector3 previousPos = position;

			for (int i = 0; i < bullet.maxRangeBullet; i++)
			{
				float angle = -bullet.trajectoryCurve.Evaluate(distance);
				Quaternion rot = Quaternion.AngleAxis(angle, Vector3.Cross(direction, Vector3.down));
				Vector3 adjustedDir = rot * direction;

				spreadProgress = Mathf.Clamp01((distance - bullet.spreadStartDistance) / (maxDistance - bullet.spreadStartDistance));
				currentSpread = Mathf.Lerp(0f, bullet.maxSpreadAngle, spreadProgress);
				Quaternion spreadRot = Quaternion.Euler(pitch * currentSpread, yaw * currentSpread, 0f);
				adjustedDir = spreadRot * adjustedDir;

				float speed = bullet.speedCurve.Evaluate(distance);
				Vector3 delta = adjustedDir * speed * stepTime;
				position += delta;
				distance += delta.magnitude;

				Debug.DrawLine(previousPos, position, colorTrajectory);
				previousPos = position;

				if (distance >= maxDistance)
					break;
			}
		}
	}
}