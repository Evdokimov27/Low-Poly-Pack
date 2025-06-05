using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProBuilderMergeShortcut
{
	[MenuItem("Tools/ProBuilder/Force Merge Selected %#m")] // Ctrl+Shift+M
	private static void MergeSelectedProBuilderObjects()
	{
		bool success = EditorApplication.ExecuteMenuItem("Tools/ProBuilder/Object/Merge Objects");

		if (!success)
			Debug.LogWarning("�� ������� ��������� ������� �������. �������, ��� ����������� ProBuilder � ������� ������� ProBuilder.");
	}

	[MenuItem("Tools/ProBuilder/Force Builderize Selected %#k")] // Ctrl+Shift+K
	public static void MergeProBuilderObjects()
	{
		bool success = EditorApplication.ExecuteMenuItem("Tools/ProBuilder/Object/Pro Builderize");

		if (!success)
			Debug.LogWarning("�� ������� ��������� ������� �������.");
	}

	[MenuItem("Tools/ProBuilder/Export Selected Hierarchy %#p")] // Ctrl+Shift+P
	public static void ExportHierarchyAsObj()
	{
		GameObject active = Selection.activeGameObject;

		if (active == null)
		{
			Debug.LogWarning("�� ������ �������� ������ ��� ��������.");
			return;
		}

		List<GameObject> selection = new List<GameObject> { active };
		foreach (Transform child in active.GetComponentsInChildren<Transform>(true))
		{
			if (child != active.transform)
				selection.Add(child.gameObject);
		}

		Selection.objects = selection.ToArray();

		bool success = EditorApplication.ExecuteMenuItem("Tools/ProBuilder/Export/Export Obj");

		if (!success)
			Debug.LogWarning("�� ������� ��������� �������. �������, ��� ���������� ProBuilder.");
	}

	[MenuItem("Tools/ProBuilder/Create Folders For Selected %#]")]
	public static void CreateFoldersForSelected()
	{
		GameObject[] selectedObjects = Selection.gameObjects;

		if (selectedObjects.Length == 0)
		{
			Debug.LogWarning("��� ���������� ��������.");
			return;
		}

		Undo.IncrementCurrentGroup();
		Undo.SetCurrentGroupName("�������� ����� � ����������� ��������");

		foreach (GameObject obj in selectedObjects)
		{
			GameObject folder = new GameObject(obj.name + "_Folder");

			Undo.RegisterCreatedObjectUndo(folder, "�������� �����");

			// ���������� ����� �� ��� �� �������, ��� � ������
			folder.transform.SetParent(obj.transform.parent);
			folder.transform.SetSiblingIndex(obj.transform.GetSiblingIndex());

			Undo.SetTransformParent(obj.transform, folder.transform, "����������� ������� � �����");
		}

		Debug.Log("����� ������� � ������� ����������.");
	}
}
