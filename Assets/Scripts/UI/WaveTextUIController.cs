using Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class WaveTextUIController : MonoBehaviour
    {
        private Label waveLabel;

        private void Awake()
        {
            if (LevelManager.Instance != null) LevelManager.Instance.OnSubSceneLoaded += OnSubSceneLoaded;
        }

        private void Start()
        {
            VisualElement rootVisualELement = GetComponent<UIDocument>().rootVisualElement;
            waveLabel = rootVisualELement.Q<Label>("waveText");
        }

        private void OnDestroy()
        {
            if (LevelManager.Instance != null) LevelManager.Instance.OnSubSceneLoaded -= OnSubSceneLoaded;
        }

        private void OnSubSceneLoaded(int subSceneIndex)
        {
            waveLabel.text = $"Wave {subSceneIndex + 1}";
        }
    }
}