using System.IO;
using UnityEditor;
using UnityEngine;

public class RenameByMaterial : MonoBehaviour
{
	[MenuItem("Tools/Rename Direct Children to Prefab Asset Name (Real Instances Only)")]
	public static void Rename()
	{
		GameObject[] selectedObjects = Selection.gameObjects;

		if (selectedObjects.Length == 0)
		{
			Debug.LogWarning("Выделите хотя бы один объект.");
			return;
		}

		int count = 0;

		foreach (GameObject selected in selectedObjects)
		{
			foreach (Transform child in selected.transform)
			{
				if (child.childCount < 1) continue;

				if (PrefabUtility.GetPrefabInstanceStatus(child.gameObject) != PrefabInstanceStatus.Connected)
					continue;

				string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(child.gameObject);
				if (string.IsNullOrEmpty(path)) continue;

				GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
				if (prefabAsset == null) continue;

				Undo.RecordObject(child.gameObject, "Rename to Prefab Name");
				Debug.Log($"Переименовано {child.name} → \"{prefabAsset.name}\".");
				child.name = prefabAsset.name;
				count++;
			}
		}

		Debug.Log($"Итог: переименовано {count} объектов.");
	}
	[MenuItem("Tools/Delete Selected Objects Without Prefab")]
	private static void DeleteObjectsWithoutPrefab()
	{
		GameObject[] selectedObjects = Selection.gameObjects;

		int deletedCount = 0;

		foreach (GameObject obj in selectedObjects)
		{
			// Проверяем, есть ли префаб-оригинал
			GameObject prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(obj);

			if (prefabSource == null)
			{
				Undo.DestroyObjectImmediate(obj);
				deletedCount++;
			}
		}

		Debug.Log($"Удалено объектов без префабов: {deletedCount}");
	}
	[MenuItem("Tools/Sort Children Alphabetically")]
	private static void SortSelectedChildren()
	{
		GameObject selected = Selection.activeGameObject;

		if (selected == null)
		{
			Debug.LogWarning("Не выбран объект!");
			return;
		}

		int childCount = selected.transform.childCount;
		if (childCount == 0)
		{
			Debug.LogWarning("У объекта нет дочерних элементов.");
			return;
		}

		// Сохраняем дочерние объекты во временный массив
		Transform[] children = new Transform[childCount];
		for (int i = 0; i < childCount; i++)
		{
			children[i] = selected.transform.GetChild(i);
		}

		// Сортируем массив по имени
		System.Array.Sort(children, (a, b) => string.Compare(a.name, b.name, System.StringComparison.Ordinal));

		// Меняем порядок в иерархии
		for (int i = 0; i < childCount; i++)
		{
			children[i].SetSiblingIndex(i);
		}

		Debug.Log($"Отсортировано {childCount} дочерних объектов по алфавиту у объекта '{selected.name}'.");
	}
	[MenuItem("Tools/Rename Assets Inside Selected Folders")]
	private static void RenameAssetsInSelectedFolders()
	{
		Object[] selectedFolders = Selection.GetFiltered(typeof(DefaultAsset), SelectionMode.Assets);

		int totalRenamed = 0;

		foreach (Object folder in selectedFolders)
		{
			string folderPath = AssetDatabase.GetAssetPath(folder);
			if (!AssetDatabase.IsValidFolder(folderPath))
				continue;

			string folderName = Path.GetFileName(folderPath);
			string[] assetGUIDs = AssetDatabase.FindAssets("", new[] { folderPath });

			int index = 0;

			foreach (string guid in assetGUIDs)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(guid);

				// Пропускаем сами папки
				if (AssetDatabase.IsValidFolder(assetPath))
					continue;

				Object asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
				if (asset == null) continue;

				string newName = folderName + "_" + index;
				string error = AssetDatabase.RenameAsset(assetPath, newName);

				if (!string.IsNullOrEmpty(error))
				{
					Debug.LogWarning($"Ошибка при переименовании '{assetPath}': {error}");
				}
				else
				{
					totalRenamed++;
					index++;
				}
			}
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		Debug.Log($"Готово. Переименовано {totalRenamed} объектов.");
	}
	[MenuItem("Tools/Disable MeshRenderer and Clear Material")]
	private static void DisableMeshAndMaterial()
	{
		GameObject[] selectedObjects = Selection.gameObjects;

		foreach (GameObject obj in selectedObjects)
		{
			// Отключение MeshRenderer и очистка материала
			MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
			if (meshRenderer != null)
			{
				Undo.RecordObject(meshRenderer, "Disable MeshRenderer and Clear Material");
				meshRenderer.sharedMaterial = null;
				meshRenderer.enabled = false;
				Debug.Log($"Disabled MeshRenderer and cleared material on '{obj.name}'");
			}

			// MeshFilter оставляем как есть (он не имеет .enabled)
			MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
			if (meshFilter != null)
			{
				Debug.Log($"MeshFilter оставлен активным на '{obj.name}'");
			}
		}
	}
	private static Material FindFirstMaterial(Transform parent)
	{
		Renderer renderer = parent.GetComponent<Renderer>();
		if (renderer != null && renderer.sharedMaterial != null)
			return renderer.sharedMaterial;

		foreach (Transform child in parent)
		{
			if (child.gameObject.activeSelf && !child.name.Contains("_cb"))
			{
				Material mat = FindFirstMaterial(child);
				if (mat != null)
					return mat;
			}
		}
		return null;
	}
}
public class GridDistributeYZ : EditorWindow
{
	private float ySpacing = 10f;
	private float zSpacing = 10f;
	private int rowCount = 3; // Сколько объектов по Z до перехода по Y

	[MenuItem("Tools/Grid Distribute on YZ")]
	public static void ShowWindow()
	{
		GetWindow<GridDistributeYZ>("Grid Distribute YZ");
	}

	private void OnGUI()
	{
		GUILayout.Label("Распределение сеткой по Y-Z", EditorStyles.boldLabel);
		rowCount = EditorGUILayout.IntField("Количество по Z (в строке)", rowCount);
		ySpacing = EditorGUILayout.FloatField("Шаг по Y", ySpacing);
		zSpacing = EditorGUILayout.FloatField("Шаг по Z", zSpacing);

		if (GUILayout.Button("Распределить"))
		{
			DistributeGrid();
		}
	}

	private void DistributeGrid()
	{
		GameObject[] selected = Selection.gameObjects;

		if (selected.Length == 0)
		{
			Debug.LogWarning("Ничего не выбрано.");
			return;
		}

		// Сортируем для предсказуемости
		System.Array.Sort(selected, (a, b) => string.Compare(a.name, b.name));

		Vector3 startPos = selected[0].transform.position;

		for (int i = 0; i < selected.Length; i++)
		{
			int row = i % rowCount; // индекс по Z
			int col = i / rowCount; // индекс по Y

			float y = startPos.y + col * ySpacing;
			float z = startPos.z + row * zSpacing;
			float x = startPos.x;

			Undo.RecordObject(selected[i].transform, "Grid Distribute");
			selected[i].transform.position = new Vector3(x, y, z);
		}

		Debug.Log($"Распределено {selected.Length} объектов по сетке YZ ({rowCount} на строку).");
	}
}
