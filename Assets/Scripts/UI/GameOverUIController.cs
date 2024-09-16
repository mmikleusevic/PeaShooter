using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private CollisionDamageSystem collisionDamageSystem;
    private VisualElement gameOverScreen;
    private Button playAgainButton;
    private Button mainMenuButton;

    private void Start()
    {
        VisualElement uiVisualELement = GetComponent<UIDocument>().rootVisualElement;

        playAgainButton = uiVisualELement.Q<Button>("play-again");
        mainMenuButton = uiVisualELement.Q<Button>("main-menu");
        gameOverScreen = uiVisualELement.Q<VisualElement>("ui");

        collisionDamageSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<CollisionDamageSystem>();

        if (collisionDamageSystem != null)
        {
            collisionDamageSystem.OnPlayerDied += OnPlayerDied;
        }

        if (playAgainButton != null) playAgainButton.clicked += PlayAgainPressed;
        if (mainMenuButton != null) mainMenuButton.clicked += MainMenuPressed;
    }

    private void OnDestroy()
    {
        if (collisionDamageSystem != null)
        {
            collisionDamageSystem.OnPlayerDied -= OnPlayerDied;
        }

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