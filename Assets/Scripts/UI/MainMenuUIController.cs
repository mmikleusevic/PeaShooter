using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUIController : MonoBehaviour
{
    private Button playButton;
    private Button quitButton;

    private void Start()
    {
        VisualElement uiVisualELement = GetComponent<UIDocument>().rootVisualElement;

        playButton = uiVisualELement.Q<Button>("play");
        quitButton = uiVisualELement.Q<Button>("quit");

        if (playButton != null) playButton.clicked += PlayPressed;
        if (quitButton != null) quitButton.clicked += QuitPressed;
    }

    private void OnDestroy()
    {
        if (playButton != null) playButton.clicked -= PlayPressed;
        if (quitButton != null) quitButton.clicked -= QuitPressed;
    }

    private void PlayPressed()
    {
        StartCoroutine(LevelManager.Instance.LoadGameScene());
    }

    private void QuitPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}