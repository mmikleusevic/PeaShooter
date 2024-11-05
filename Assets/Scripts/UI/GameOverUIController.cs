using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOverUIController : MonoBehaviour
{
    private VisualElement gameOverScreen;
    private Button mainMenuButton;
    private Button playAgainButton;
    private PlayerHealthSystem playerHealthSystem;

    private void Start()
    {
        VisualElement uiVisualELement = GetComponent<UIDocument>().rootVisualElement;

        gameOverScreen = uiVisualELement.Q<VisualElement>("ui");
        playAgainButton = uiVisualELement.Q<Button>("play-again");
        mainMenuButton = uiVisualELement.Q<Button>("main-menu");

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
        gameOverScreen.style.visibility = Visibility.Visible;

        gameOverScreen.schedule.Execute(() => playAgainButton.Focus())
            .Until(() => gameOverScreen.focusController.focusedElement == playAgainButton);
    }

    private void HideUI()
    {
        gameOverScreen.style.visibility = Visibility.Hidden;
    }
}