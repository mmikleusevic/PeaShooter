using Managers;
using Systems;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class GameOverUIController : MonoBehaviour
    {
        private VisualElement gameOverUI;
        private Button mainMenuButton;
        private Button playAgainButton;
        private PlayerHealthSystem playerHealthSystem;

        private void Start()
        {
            VisualElement uiVisualELement = GetComponent<UIDocument>().rootVisualElement;

            gameOverUI = uiVisualELement.Q<VisualElement>("gameOverUI");
            playAgainButton = uiVisualELement.Q<Button>("playAgain");
            mainMenuButton = uiVisualELement.Q<Button>("mainMenu");

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
            gameOverUI.style.visibility = Visibility.Visible;

            gameOverUI.schedule.Execute(() => playAgainButton.Focus())
                .Until(() => gameOverUI.focusController.focusedElement == playAgainButton);
        }

        private void HideUI()
        {
            gameOverUI.style.visibility = Visibility.Hidden;
        }
    }
}