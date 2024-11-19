using Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace UI
{
    public class MainMenuUIController : MonoBehaviour
    {
        private VisualElement mainMenuElement;
        private Button optionsButton;
        private Button playButton;
        private Button quitButton;

        private void Start()
        {
            mainMenuElement = GetComponent<UIDocument>().rootVisualElement.Q("mainMenu");
            playButton = mainMenuElement.Q<Button>("play");
            optionsButton = mainMenuElement.Q<Button>("options");
            quitButton = mainMenuElement.Q<Button>("quit");

            if (playButton != null) playButton.clicked += PlayPressed;
            if (optionsButton != null) optionsButton.clicked += OptionsPressed;
            if (quitButton != null) quitButton.clicked += QuitPressed;
            if (OptionsUIController.Instance) OptionsUIController.Instance.OnOptionsClosed += OnOptionsClosed;

            playButton?.Focus();
        }

        private void OnDestroy()
        {
            if (playButton != null) playButton.clicked -= PlayPressed;
            if (optionsButton != null) optionsButton.clicked -= OptionsPressed;
            if (quitButton != null) quitButton.clicked -= QuitPressed;
            if (OptionsUIController.Instance) OptionsUIController.Instance.OnOptionsClosed -= OnOptionsClosed;
        }

        private void PlayPressed()
        {
            LevelManager.Instance.LoadGameScene();
        }

        private void OptionsPressed()
        {
            OptionsUIController.Instance.Show();
            mainMenuElement.style.visibility = Visibility.Hidden;
        }

        private void QuitPressed()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        private void OnOptionsClosed()
        {
            mainMenuElement.style.visibility = Visibility.Visible;
            mainMenuElement.schedule.Execute(() => playButton.Focus())
                .Until(() => mainMenuElement.focusController.focusedElement == playButton);
        }
    }
}