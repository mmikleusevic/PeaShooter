using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUIController : MonoBehaviour
{
    private Button playButton;
    private Button optionsButton;
    private Button quitButton;

    [SerializeField] private OptionsUIController optionsUI;

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
        optionsUI.Show();
    }

    private void QuitPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}