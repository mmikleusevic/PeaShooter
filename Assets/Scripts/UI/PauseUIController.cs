using System;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UIElements;

namespace UI
{
    public class PauseUIController : MonoBehaviour
    {
        private Button mainMenuButton;
        private Button optionsButton;
        private Button pauseButton;
        private VisualElement pauseElementUI;
        private VisualElement pauseUI;
        private InputActionAsset playerInputAsset;
        private Button restartButton;
        private Button resumeButton;

        private void Awake()
        {
            playerInputAsset = FindFirstObjectByType<EventSystem>().GetComponent<InputSystemUIInputModule>()
                .actionsAsset;
            playerInputAsset.FindActionMap("UI").FindAction("Pause").performed += TogglePauseUI;
        }

        private void Start()
        {
            VisualElement uiVisualELement = GetComponent<UIDocument>().rootVisualElement;
            pauseElementUI = uiVisualELement.Q<VisualElement>("pauseElementUI");
            pauseUI = uiVisualELement.Q<VisualElement>("pauseUI");
            pauseButton = pauseElementUI.Q<Button>("pause");
            resumeButton = pauseUI.Q<Button>("resume");
            restartButton = pauseUI.Q<Button>("restart");
            optionsButton = pauseUI.Q<Button>("options");
            mainMenuButton = pauseUI.Q<Button>("mainMenu");

            if (OptionsUIController.Instance) OptionsUIController.Instance.OnOptionsClosed += OnOptionsClosed;
            if (pauseButton != null) pauseButton.clicked += OnPause;
            if (resumeButton != null) resumeButton.clicked += OnResume;
            if (restartButton != null) restartButton.clicked += OnRestart;
            if (optionsButton != null) optionsButton.clicked += OnOptions;
            if (mainMenuButton != null) mainMenuButton.clicked += OnMainMenu;
        }

        private void OnDestroy()
        {
            playerInputAsset.FindActionMap("UI").FindAction("Pause").performed -= TogglePauseUI;
            if (OptionsUIController.Instance) OptionsUIController.Instance.OnOptionsClosed -= OnOptionsClosed;
            if (pauseButton != null) pauseButton.clicked -= OnPause;
            if (resumeButton != null) resumeButton.clicked -= OnResume;
            if (restartButton != null) restartButton.clicked -= OnRestart;
            if (optionsButton != null) optionsButton.clicked -= OnOptions;
            if (mainMenuButton != null) mainMenuButton.clicked -= OnMainMenu;
        }

        public event Action OnPauseUIOpened;
        public event Action OnPauseUIClosed;

        private void TogglePauseUI(InputAction.CallbackContext obj)
        {
            if (!pauseUI.visible)
            {
                GameStateManager.Instance.PauseGame();
                Show();
            }
            else
            {
                GameStateManager.Instance.ResumeGame();
                Hide();
            }
        }

        private void OnOptionsClosed()
        {
            Show();
        }

        private void OnPause()
        {
            GameStateManager.Instance.PauseGame();

            Show();
        }

        private void OnResume()
        {
            GameStateManager.Instance.ResumeGame();

            Hide();
        }

        private void OnRestart()
        {
            HideAll();
            LevelManager.Instance.LoadGameScene();
        }

        private void OnOptions()
        {
            pauseUI.style.visibility = Visibility.Hidden;
            OptionsUIController.Instance.Show();
        }

        private void OnMainMenu()
        {
            HideAll();
            LevelManager.Instance.LoadMainMenu();
        }

        private void Show()
        {
            OnPauseUIOpened?.Invoke();

            pauseElementUI.style.visibility = Visibility.Hidden;
            pauseUI.style.visibility = Visibility.Visible;

            pauseUI.schedule.Execute(() => resumeButton.Focus())
                .Until(() => pauseUI.focusController.focusedElement == resumeButton);
        }

        private void Hide()
        {
            OnPauseUIClosed?.Invoke();

            pauseElementUI.style.visibility = Visibility.Visible;
            pauseUI.style.visibility = Visibility.Hidden;
        }

        private void HideAll()
        {
            pauseElementUI.style.visibility = Visibility.Hidden;
            pauseUI.style.visibility = Visibility.Visible;
        }
    }
}