using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

public class PlacePrefabsOnSelection : EditorWindow
{
	private List<GameObject> objectsToPlace = new List<GameObject>();
	private ReorderableList reorderableList;

	[MenuItem("Tools/Place Prefabs on Selected")]
	public static void ShowWindow()
	{
		GetWindow<PlacePrefabsOnSelection>("Place Prefabs");
	}

	private void OnEnable()
	{
		reorderableList = new ReorderableList(objectsToPlace, typeof(GameObject), true, true, true, true);

		reorderableList.drawHeaderCallback = (Rect rect) =>
		{
			EditorGUI.LabelField(rect, "Объекты для размещения (можно перетаскивать сразу несколько)");
		};

		reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
		{
			objectsToPlace[index] = (GameObject)EditorGUI.ObjectField(
				new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
				objectsToPlace[index],
				typeof(GameObject),
				false
			);
		};

		reorderableList.onAddCallback = (ReorderableList list) =>
		{
			objectsToPlace.Add(null);
		};
	}

	private void OnGUI()
	{
		HandleDragAndDrop(); // нужно перед DoLayoutList

		reorderableList.DoLayoutList();

		if (GUILayout.Button("Разместить объекты"))
		{
			PlacePrefabs();
		}
		if (GUILayout.Button("Отключить все дочерние объекты выделенных"))
		{
			DisableAllChildren();
		}
		if (GUILayout.Button("Переименовать выделенные объекты как 'Magazine <PrefabName>'"))
		{
			RenameSelectedObjectsAsMagazine();
		}
		if (GUILayout.Button("Создать объект в родителе на позиции выделенных"))
		{
			CreateObjectAtSelectedPositionsInParent();
		}

	}
	private void CreateObjectAtSelectedPositionsInParent()
	{
		GameObject[] selectedObjects = Selection.gameObjects;

		if (selectedObjects.Length == 0)
		{
			Debug.LogWarning("Нет выделенных объектов.");
			return;
		}

		foreach (GameObject obj in selectedObjects)
		{
			Transform parent = obj.transform.parent;

			if (parent == null)
			{
				Debug.LogWarning($"Объект '{obj.name}' не имеет родителя — пропущен.");
				continue;
			}

			GameObject newObject = new GameObject();

			GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);
			newObject.name = "_сb_magazine";
			newObject.transform.SetParent(parent);
			newObject.transform.position = obj.transform.position;
			newObject.transform.rotation = obj.transform.rotation;
			newObject.transform.localScale = Vector3.one;

			Undo.RegisterCreatedObjectUndo(newObject, "Create Magazine Object");
		}

		Debug.Log("Созданы объекты в родителях на позициях выделенных.");
	}

	private void RenameSelectedObjectsAsMagazine()
	{
		GameObject[] selectedObjects = Selection.gameObjects;

		if (selectedObjects.Length == 0)
		{
			Debug.LogWarning("Нет выделенных объектов для переименования.");
			return;
		}

		foreach (GameObject obj in selectedObjects)
		{
			GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);

			if (prefab != null)
			{
				string newName = "Magazine " + prefab.transform.parent.name;
				Undo.RecordObject(obj, "Rename Object");
				obj.name = newName;
			}
			else
			{
				Debug.LogWarning($"Объект '{obj.name}' не связан с префабом — пропущен.");
			}
		}

		Debug.Log("Переименование завершено.");
	}

	private void HandleDragAndDrop()
	{
		Event evt = Event.current;
		Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
		GUI.Box(dropArea, "Перетащи префабы сюда", EditorStyles.helpBox);

		if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
		{
			if (!dropArea.Contains(evt.mousePosition))
				return;

			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

			if (evt.type == EventType.DragPerform)
			{
				DragAndDrop.AcceptDrag();

				foreach (Object draggedObj in DragAndDrop.objectReferences)
				{
					if (draggedObj is GameObject go && PrefabUtility.GetPrefabAssetType(go) != PrefabAssetType.NotAPrefab)
					{
						if (!objectsToPlace.Contains(go))
							objectsToPlace.Add(go);
					}
				}

				evt.Use();
			}
		}
	}
	private void DisableAllChildren()
	{
		GameObject[] selectedObjects = Selection.gameObjects;

		if (selectedObjects.Length == 0)
		{
			Debug.LogWarning("Нет выделенных объектов.");
			return;
		}

		foreach (GameObject obj in selectedObjects)
		{
			foreach (Transform child in obj.transform)
			{
				Undo.RecordObject(child.gameObject, "Disable Child");
				child.gameObject.SetActive(false);
			}
		}

		Debug.Log("Все дочерние объекты отключены.");
	}
	private void PlacePrefabs()
	{
		GameObject[] selectedObjects = Selection.gameObjects;

		if (objectsToPlace.Count == 0 || selectedObjects.Length == 0)
		{
			Debug.LogWarning("Выберите объекты в сцене и добавьте хотя бы один префаб.");
			return;
		}

		foreach (GameObject target in selectedObjects)
		{
			foreach (GameObject prefab in objectsToPlace)
			{
				GameObject placed = (GameObject)PrefabUtility.InstantiatePrefab(prefab, target.scene);
				placed.transform.SetParent(target.transform);
				placed.transform.localPosition = Vector3.zero;
				Undo.RegisterCreatedObjectUndo(placed, "Place Prefab");
			}
		}

		Debug.Log("Объекты успешно размещены.");
	}
}
