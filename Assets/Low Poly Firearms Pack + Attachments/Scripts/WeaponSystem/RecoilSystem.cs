using UnityEngine;

namespace LowPolyFirearms.WeaponSystem
{
	public class RecoilHandler : MonoBehaviour
	{
		[Header("Recoil Amount")]
		public float recoilPerShotVerticalMin = -3f;
		public float recoilPerShotVerticalMax = 3f;
		public float recoilPerShotHorizontalMin = -1f;
		public float recoilPerShotHorizontalMax = 1f;

		[Header("Clamping")]
		public float maxAccumulatedVerticalRecoil = 5f;
		public float maxAccumulatedHorizontalRecoil = 5f;


		[Header("Timing")]
		public float attackTime = 0.1f;
		public float returnTime = 0.4f;

		[Header("Modifiers")]
		[Range(0f, 5f)] public float recoilIntensity = 1f;
		public bool isAiming = false;
		[Range(0f, 1f)] public float aimMultiplier = 0.5f;

		private Transform recoilTarget;
		private Quaternion originalRotation;

		private Vector2 accumulatedRecoil = Vector2.zero;
		private Vector2 currentRecoil = Vector2.zero;

		private float attackTimer = 0f;
		private float returnTimer = 0f;
		private bool isRecoiling = false;
		private bool isReturning = false;

		public void Initialize(Transform target)
		{
			recoilTarget = target;
			originalRotation = target.localRotation;
		}

		public void SetAiming(bool aiming)
		{
			isAiming = aiming;
		}

		public void TriggerRecoil(float scale)
		{

			float vertical = Random.Range(recoilPerShotVerticalMin, recoilPerShotVerticalMax) * scale;
			float horizontal = Random.Range(recoilPerShotVerticalMin, recoilPerShotVerticalMax) * scale;

			Vector2 newRecoil = new Vector2(vertical, horizontal);
			accumulatedRecoil += newRecoil;

			accumulatedRecoil.x = Mathf.Clamp(accumulatedRecoil.x, -maxAccumulatedVerticalRecoil, maxAccumulatedVerticalRecoil);
			accumulatedRecoil.y = Mathf.Clamp(accumulatedRecoil.y, -maxAccumulatedHorizontalRecoil, maxAccumulatedHorizontalRecoil);

			attackTimer = 0f;
			returnTimer = 0f;
			isRecoiling = true;
			isReturning = false;
		}


		public void ProcessRecoil()
		{
			if (recoilTarget == null) return;

			if (isRecoiling)
			{
				attackTimer += Time.deltaTime;
				float t = Mathf.Clamp01(attackTimer / attackTime);
				currentRecoil = Vector2.Lerp(currentRecoil, accumulatedRecoil, t);

				if (t >= 1f)
				{
					isRecoiling = false;
					isReturning = true;
					attackTimer = 0f;
				}
			}
			else if (isReturning)
			{
				returnTimer += Time.deltaTime;
				float t = Mathf.Clamp01(returnTimer / returnTime);
				currentRecoil = Vector2.Lerp(currentRecoil, Vector2.zero, t);

				if (t >= 1f)
				{
					isReturning = false;
					accumulatedRecoil = Vector2.zero;
					currentRecoil = Vector2.zero;
				}
			}

			Quaternion rot = Quaternion.Euler(currentRecoil.x, currentRecoil.y, 0f);
			recoilTarget.localRotation = originalRotation * rot;
		}
	}
}