using UnityEngine;
using System.IO;

public class ScreenshotCapture : MonoBehaviour
{
	public KeyCode screenshotKey = KeyCode.K;
	public int resolutionMultiplier = 1; // 1 = текущий размер экрана

	private void Update()
	{
		if (Input.GetKeyDown(screenshotKey))
		{
			TakeScreenshot();
		}
	}

	void TakeScreenshot()
	{
		string folderPath = Path.Combine(Application.dataPath, "../Assets/Screenshots");
		if (!Directory.Exists(folderPath))
			Directory.CreateDirectory(folderPath);

		string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
		string filename = $"screenshot_{timestamp}.png";
		string fullPath = Path.Combine(folderPath, filename);

		ScreenCapture.CaptureScreenshot(fullPath, resolutionMultiplier);
		Debug.Log($"Скриншот сохранён: {fullPath}");
	}
}
