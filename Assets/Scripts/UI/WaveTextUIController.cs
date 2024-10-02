using UnityEngine;
using UnityEngine.UIElements;

public class WaveTextUIController : MonoBehaviour
{
    private Label waveText;

    private void Awake()
    {
        if (LevelManager.Instance != null) LevelManager.Instance.OnSubSceneLoaded += OnSubSceneLoaded;
    }

    private void Start()
    {
        VisualElement uiVisualELement = GetComponent<UIDocument>().rootVisualElement;
        waveText = uiVisualELement.Q<Label>("waveText");
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null) LevelManager.Instance.OnSubSceneLoaded -= OnSubSceneLoaded;
    }

    private void OnSubSceneLoaded(int subSceneIndex)
    {
        waveText.text = $"Wave {subSceneIndex + 1}";
    }
}