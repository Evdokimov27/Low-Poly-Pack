using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.HighDefinition;

public class RenderPipelineSwitcher : EditorWindow
{
	private RenderPipelineType selectedPipeline = RenderPipelineType.BuiltIn;

	[MenuItem("Tools/Render Pipeline Switcher")]
	public static void ShowWindow()
	{
		GetWindow<RenderPipelineSwitcher>("Pipeline Switcher");
	}

	private void OnGUI()
	{
		GUILayout.Label("Render Pipeline Converter", EditorStyles.boldLabel);
		selectedPipeline = (RenderPipelineType)EditorGUILayout.EnumPopup("Select Pipeline", selectedPipeline);

		if (GUILayout.Button("Switch Pipeline"))
		{
			SwitchToPipeline(selectedPipeline);
		}
	}

	private static void SwitchToPipeline(RenderPipelineType pipelineType)
	{
		switch (pipelineType)
		{
			case RenderPipelineType.BuiltIn:
				GraphicsSettings.renderPipelineAsset = null;
				QualitySettings.renderPipeline = null;
				Debug.Log("✅ Switched to Built-in Pipeline.");
				break;

			case RenderPipelineType.URP:
				string urpPath = "Assets/Settings/URP_Asset.asset";
				var urpAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(urpPath);

				if (urpAsset == null)
				{
					urpAsset = CreateInstance<UniversalRenderPipelineAsset>();
					AssetDatabase.CreateAsset(urpAsset, urpPath);
					Debug.LogWarning("Created default URP asset.");
				}

				GraphicsSettings.renderPipelineAsset = urpAsset;
				QualitySettings.renderPipeline = urpAsset;
				Debug.Log("✅ Switched to URP.");
				break;

			case RenderPipelineType.HDRP:
				string hdrpPath = "Assets/Settings/HDRP_Asset.asset";
				var hdrpAsset = AssetDatabase.LoadAssetAtPath<HDRenderPipelineAsset>(hdrpPath);

				if (hdrpAsset == null)
				{
					hdrpAsset = CreateInstance<HDRenderPipelineAsset>();
					AssetDatabase.CreateAsset(hdrpAsset, hdrpPath);
					Debug.LogWarning("Created default HDRP asset.");
				}

				GraphicsSettings.renderPipelineAsset = hdrpAsset;
				QualitySettings.renderPipeline = hdrpAsset;
				Debug.Log("✅ Switched to HDRP.");
				break;
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	private enum RenderPipelineType
	{
		BuiltIn,
		URP,
		HDRP
	}
}
