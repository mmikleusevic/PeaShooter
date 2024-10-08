using UnityEngine;
using UnityEngine.UIElements;

public class PauseUIController : MonoBehaviour
{
    private VisualElement pauseElementUI;
    private VisualElement pauseUI;
    private Button pauseButton;
    private Button resumeButton;
    private Button restartButton;
    private Button optionsButton;
    private Button mainMenuButton;

    private void Start()
    {
        VisualElement uiVisualELement = GetComponent<UIDocument>().rootVisualElement;
        pauseElementUI = uiVisualELement.Q<VisualElement>("pauseElementUI");
        pauseUI = uiVisualELement.Q<VisualElement>("pauseUI");
        pauseButton = pauseElementUI.Q<Button>("pause");
        resumeButton = pauseUI.Q<Button>("resume");
        restartButton = pauseUI.Q<Button>("restart");
        optionsButton = pauseUI.Q<Button>("options");
        mainMenuButton = pauseUI.Q<Button>("main-menu");

        if (pauseButton != null) pauseButton.clicked += OnPause;
        if (resumeButton != null) resumeButton.clicked += OnResume;
        if (restartButton != null) restartButton.clicked += OnRestart;
        if (optionsButton != null) optionsButton.clicked += OnOptions;
        if (mainMenuButton != null) mainMenuButton.clicked += OnMainMenu;
    }

    private void OnDestroy()
    {
        if (pauseButton != null) pauseButton.clicked -= OnPause;
        if (resumeButton != null) resumeButton.clicked -= OnResume;
        if (restartButton != null) restartButton.clicked -= OnRestart;
        if (optionsButton != null) optionsButton.clicked -= OnOptions;
        if (mainMenuButton != null) mainMenuButton.clicked -= OnMainMenu;
    }

    private void OnPause()
    {
        GameStateManager.Instance.PauseGame();

        pauseElementUI.visible = false;
        pauseUI.visible = true;
    }

    private void OnResume()
    {
        GameStateManager.Instance.ResumeGame();

        pauseElementUI.visible = true;
        pauseUI.visible = false;
    }

    private void OnRestart()
    {
        HideAll();
        LevelManager.Instance.LoadGameScene();
    }

    private void OnOptions()
    {
        OptionsUIController.Instance.Show();
    }

    private void OnMainMenu()
    {
        HideAll();
        LevelManager.Instance.LoadMainMenu();
    }

    private void HideAll()
    {
        pauseElementUI.visible = false;
        pauseUI.visible = false;
    }
}