using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUIController : MonoBehaviour
{
    private Button playButton;
    private Button quitButton;

    private void OnEnable()
    {
        VisualElement uiVisualELement = GetComponent<UIDocument>().rootVisualElement;

        playButton = uiVisualELement.Q<Button>("play");
        quitButton = uiVisualELement.Q<Button>("quit");

        playButton.clicked += PlayPressed;
        quitButton.clicked += QuitPressed;
    }

    private void OnDisable()
    {
        playButton.clicked -= PlayPressed;
        quitButton.clicked -= QuitPressed;
    }

    private void PlayPressed()
    {
        LevelManager.Instance.LoadStartingScene();
    }

    private void QuitPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
