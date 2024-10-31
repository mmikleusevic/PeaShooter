using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUIController : MonoBehaviour
{
    private Button optionsButton;
    private Button playButton;
    private Button quitButton;

    private void Start()
    {
        VisualElement uiVisualELement = GetComponent<UIDocument>().rootVisualElement;

        playButton = uiVisualELement.Q<Button>("play");
        optionsButton = uiVisualELement.Q<Button>("options");
        quitButton = uiVisualELement.Q<Button>("quit");

        if (playButton != null) playButton.clicked += PlayPressed;
        if (playButton != null) optionsButton.clicked += OptionsPressed;
        if (quitButton != null) quitButton.clicked += QuitPressed;
    }

    private void OnDestroy()
    {
        if (playButton != null) playButton.clicked -= PlayPressed;
        if (playButton != null) optionsButton.clicked -= OptionsPressed;
        if (quitButton != null) quitButton.clicked -= QuitPressed;
    }

    private void PlayPressed()
    {
        LevelManager.Instance.LoadGameScene();
    }

    private void OptionsPressed()
    {
        OptionsUIController.Instance.Show();
    }

    private void QuitPressed()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}