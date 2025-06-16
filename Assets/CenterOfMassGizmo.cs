using UnityEngine;
using UnityEditor;
using System.IO;

public class RemoveLeadingUnderscores : EditorWindow
{
	private string rootPath = "Assets/";

	[MenuItem("Tools/Cleanup/Remove Leading Underscores")]
	public static void ShowWindow()
	{
		GetWindow<RemoveLeadingUnderscores>("Remove _ Prefix");
	}

	void OnGUI()
	{
		GUILayout.Label("Remove '_' prefix from all files and folders", EditorStyles.boldLabel);
		rootPath = EditorGUILayout.TextField("Root Folder", rootPath);

		if (GUILayout.Button("Start Cleanup"))
		{
			if (Directory.Exists(rootPath))
			{
				RenameAllRecursive(rootPath);
				AssetDatabase.Refresh();
				Debug.Log("✅ Cleanup completed.");
			}
			else
			{
				Debug.LogError("❌ Path not found: " + rootPath);
			}
		}
	}

	static void RenameAllRecursive(string path)
	{
		// Рекурсивно переходим по подпапкам
		foreach (string subDir in Directory.GetDirectories(path))
		{
			RenameAllRecursive(subDir);
		}

		// Переименование файлов
		foreach (string filePath in Directory.GetFiles(path))
		{
			string fileName = Path.GetFileName(filePath);
			if (fileName.StartsWith("_"))
			{
				string newFileName = fileName.TrimStart('_');
				string newPath = Path.Combine(Path.GetDirectoryName(filePath), newFileName);
				AssetDatabase.MoveAsset(filePath.Replace("\\", "/"), newPath.Replace("\\", "/"));
			}
		}

		// Переименование папок
		string folderName = Path.GetFileName(path);
		if (folderName.StartsWith("_"))
		{
			string parent = Path.GetDirectoryName(path);
			string newFolderName = folderName.TrimStart('_');
			string newFolderPath = Path.Combine(parent, newFolderName);
			AssetDatabase.MoveAsset(path.Replace("\\", "/"), newFolderPath.Replace("\\", "/"));
		}
	}
}
