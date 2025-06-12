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

		// ����� ���� � ������� �����������
		Vector3 worldCenterOfMass = rb.worldCenterOfMass;

		// ������ ����� � ������ ����
		Gizmos.DrawSphere(worldCenterOfMass, gizmoSize);

		// (�������������) ����� �� ������� �� ������ ����
		Gizmos.DrawLine(transform.position, worldCenterOfMass);
	}
}
