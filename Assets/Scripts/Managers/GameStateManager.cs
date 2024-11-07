using Unity.Entities;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    private bool isDead;
    private bool isPausedForLevelUp;

    private PlayerHealthSystem playerHealthSystem;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerHealthSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerHealthSystem>();

        if (playerHealthSystem != null) playerHealthSystem.OnPlayerDied += OnPlayerDied;

        ResumeGame();
    }

    private void OnDestroy()
    {
        if (playerHealthSystem != null) playerHealthSystem.OnPlayerDied -= OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        isDead = true;

        PauseGame();
    }

    public void PauseOnLevelUp()
    {
        isPausedForLevelUp = true;

        PauseGame();
    }

    public void OnAbilityChosen()
    {
        isPausedForLevelUp = false;

        ResumeGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        if (isDead || isPausedForLevelUp)
        {
            PauseGame();
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}