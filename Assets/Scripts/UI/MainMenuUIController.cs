using Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;


namespace UI
{
    public class MainMenuUIController : MonoBehaviour
    {
        private VisualElement mainMenuUI;
        private Button optionsButton;
        private Button playButton;
        private Button quitButton;

        private void Start()
        {
            mainMenuUI = GetComponent<UIDocument>().rootVisualElement.Q("mainMenuUI");
            playButton = mainMenuUI.Q<Button>("play");
            optionsButton = mainMenuUI.Q<Button>("options");
            quitButton = mainMenuUI.Q<Button>("quit");

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
            mainMenuUI.style.visibility = Visibility.Hidden;
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
            mainMenuUI.style.visibility = Visibility.Visible;
            mainMenuUI.schedule.Execute(() => playButton.Focus())
                .Until(() => mainMenuUI.focusController.focusedElement == playButton);
        }
    }
}