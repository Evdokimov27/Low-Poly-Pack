using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CenterOfMassGizmo : MonoBehaviour
{
	public Color gizmoColor = Color.red;
	public float gizmoSize = 0.1f;

	private Rigidbody rb;

	void OnDrawGizmos()
	{
		if (rb == null)
			rb = GetComponent<Rigidbody>();

		Gizmos.color = gizmoColor;

		// Центр масс в мировых координатах
		Vector3 worldCenterOfMass = rb.worldCenterOfMass;

		// Рисуем сферу в центре масс
		Gizmos.DrawSphere(worldCenterOfMass, gizmoSize);

		// (Необязательно) Линия от объекта до центра масс
		Gizmos.DrawLine(transform.position, worldCenterOfMass);
	}
}
