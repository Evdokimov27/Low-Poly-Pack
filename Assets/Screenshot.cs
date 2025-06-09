using UnityEngine;
using System.IO;
using System.Collections;

public class ScreenshotCapture : MonoBehaviour
{
	public GameObject canvas;
	public KeyCode screenshotKey = KeyCode.K;
	public int resolutionMultiplier = 1;

	private void Update()
	{
		if (Input.GetKeyDown(screenshotKey))
		{
			StartCoroutine(TakeScreenshotRoutine());
		}
	}

	private IEnumerator TakeScreenshotRoutine()
	{
		if (canvas != null)
			canvas.SetActive(false);

		yield return new WaitForEndOfFrame();

		string folderPath = Path.Combine(Application.dataPath, "../Assets/Screenshots");
		if (!Directory.Exists(folderPath))
			Directory.CreateDirectory(folderPath);

		string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
		string filename = $"screenshot_{timestamp}.png";
		string fullPath = Path.Combine(folderPath, filename);

		ScreenCapture.CaptureScreenshot(fullPath, resolutionMultiplier);

		yield return new WaitForEndOfFrame();

		if (canvas != null)
			canvas.SetActive(true);
	}
}
