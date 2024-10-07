using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOverUIController : MonoBehaviour
{
    private PlayerHealthSystem playerHealthSystem;
    private VisualElement gameOverScreen;
    private Button playAgainButton;
    private Button mainMenuButton;

    private void Start()
    {
        VisualElement uiVisualELement = GetComponent<UIDocument>().rootVisualElement;

        playAgainButton = uiVisualELement.Q<Button>("play-again");
        mainMenuButton = uiVisualELement.Q<Button>("main-menu");
        gameOverScreen = uiVisualELement.Q<VisualElement>("ui");

        playerHealthSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerHealthSystem>();

        if (playerHealthSystem != null) playerHealthSystem.OnPlayerDied += OnPlayerDied;
        if (playAgainButton != null) playAgainButton.clicked += PlayAgainPressed;
        if (mainMenuButton != null) mainMenuButton.clicked += MainMenuPressed;
    }

    private void OnDestroy()
    {
        if (playerHealthSystem != null) playerHealthSystem.OnPlayerDied -= OnPlayerDied;
        if (playAgainButton != null) playAgainButton.clicked -= PlayAgainPressed;
        if (mainMenuButton != null) mainMenuButton.clicked -= MainMenuPressed;
    }

    private void OnPlayerDied()
    {
        ShowUI();
    }

    private void PlayAgainPressed()
    {
        LevelManager.Instance.LoadGameScene();
        HideUI();
    }

    private void MainMenuPressed()
    {
        LevelManager.Instance.LoadMainMenu();
        HideUI();
    }

    private void ShowUI()
    {
        gameOverScreen.visible = true;
    }

    private void HideUI()
    {
        gameOverScreen.visible = false;
    }
}