using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[InitializeOnLoad]
public static class FullScreenRendererController
{
    static FullScreenRendererController()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state is PlayModeStateChange.EnteredPlayMode or PlayModeStateChange.ExitingPlayMode)
        {
            UpdateFullScreenPassRenderer(state == PlayModeStateChange.EnteredPlayMode);
        }
    }

    private static void UpdateFullScreenPassRenderer(bool isPlaying)
    {
        UniversalRenderPipelineAsset pipelineAsset =
            GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

        if (!pipelineAsset) return;

        foreach (var rendererData in pipelineAsset.rendererDataList)
        {
            if (!rendererData) continue;

            foreach (var feature in rendererData.rendererFeatures)
            {
                if (feature is FullScreenPassRendererFeature fullScreenFeature)
                {
                    fullScreenFeature.SetActive(isPlaying);
                }
            }
        }
    }
}