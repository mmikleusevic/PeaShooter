using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    private CollisionDamageSystem collisionDamageSystem;
    private Button playAgainButton;
    private Button mainMenuButton;
    private VisualElement gameOverScreen;

    private void OnEnable()
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

        playAgainButton.clicked += PlayAgainPressed;
        mainMenuButton.clicked += MainMenuPressed;
    }

    private void OnDisable()
    {
        if (collisionDamageSystem != null)
        {
            collisionDamageSystem.OnPlayerDied -= OnPlayerDied;
        }

        playAgainButton.clicked -= PlayAgainPressed;
        mainMenuButton.clicked -= MainMenuPressed;
    }

    private void OnPlayerDied()
    {
        ShowUI();
    }

    private void PlayAgainPressed()
    {
        StartCoroutine(LevelManager.Instance.LoadGameScene());
        HideUI();
    }

    private void MainMenuPressed()
    {
        StartCoroutine(LevelManager.Instance.LoadMainMenu());
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