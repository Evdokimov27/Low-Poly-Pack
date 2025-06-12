using UnityEngine;

public class Bullet : MonoBehaviour
{
	[Header("Ballistic settings")]
	[Tooltip("X - distance, Y - angle of the bullet's flight")]
	public AnimationCurve trajectoryCurve;
	[Tooltip("X - distance, Y - speed at this distance")]
	public AnimationCurve speedCurve;
	public float maxRangeBullet = 500f;

	[Header("Bullet settings")]
	public int pelletCount = 1;

	[Header("Spread settings")]
	public float maxSpreadAngle = 5f;
	public float speedVariationPercent = 0.1f;
	public float spreadStartDistance = 0f;

	private Vector3 direction;
	private Vector3 startPos;
	private float traveled;

	private float randomYaw;
	private float randomPitch;
	private float randomSpeedMultiplier;


	void Reset()
	{
		trajectoryCurve = new AnimationCurve(
			new Keyframe(0f, 0f),
			new Keyframe(50f, 2.5f),
			new Keyframe(150f, 0f),
			new Keyframe(300f, -10f),
			new Keyframe(500f, -50f)
		);

		speedCurve = new AnimationCurve(
			new Keyframe(0f, 100f),
			new Keyframe(50f, 150f),
			new Keyframe(150f, 100f),
			new Keyframe(300f, 75f),
			new Keyframe(500f, 20f)
		);
	}
	void Start()
	{
		startPos = transform.position;
		randomYaw = Random.Range(-1f, 1f);
		randomPitch = Random.Range(-1f, 1f);
		randomSpeedMultiplier = 1f + Random.Range(-speedVariationPercent, speedVariationPercent);
	}

	public void InitializeDirection(Vector3 shootDirection)
	{
		direction = shootDirection.normalized;
	}

	void Update()
	{
		traveled = Vector3.Distance(startPos, transform.position);

		float spreadProgress = Mathf.Clamp01((traveled - spreadStartDistance) / (maxRangeBullet - spreadStartDistance));
		float currentSpread = Mathf.Lerp(0f, maxSpreadAngle, spreadProgress);

		float angle = trajectoryCurve.Evaluate(traveled);
		Quaternion trajectoryRot = Quaternion.AngleAxis(angle, Vector3.Cross(direction, Vector3.down));
		Vector3 adjustedDirection = trajectoryRot * direction;

		Quaternion spreadRot = Quaternion.Euler(randomPitch * currentSpread, randomYaw * currentSpread, 0f);
		adjustedDirection = spreadRot * adjustedDirection;

		float currentSpeed = speedCurve.Evaluate(traveled) * randomSpeedMultiplier;

		transform.position += adjustedDirection * -currentSpeed * Time.deltaTime;
		transform.rotation = Quaternion.LookRotation(adjustedDirection);

		if (traveled >= maxRangeBullet)
			Destroy(gameObject);
	}
}
