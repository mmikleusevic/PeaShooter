using Unity.Entities;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;
    private bool isDead;
    private bool isPausedForLevelUp;
    private PlayerExperienceSystem playerExperienceSystem;

    private PlayerHealthSystem playerHealthSystem;
    public bool IsDead => IsDead;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        playerHealthSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerHealthSystem>();
        playerExperienceSystem =
            World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerExperienceSystem>();

        if (playerHealthSystem != null) playerHealthSystem.OnPlayerDied += OnPlayerDied;
        if (playerExperienceSystem != null) playerExperienceSystem.OnLevelUp += OnLevelUp;

        ResumeGame();
    }

    private void OnDestroy()
    {
        if (playerHealthSystem != null) playerHealthSystem.OnPlayerDied -= OnPlayerDied;
        if (playerExperienceSystem != null) playerExperienceSystem.OnLevelUp -= OnLevelUp;
    }

    private void OnPlayerDied()
    {
        isDead = true;

        PauseGame();
    }

    private void OnLevelUp()
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
            PauseGame();
        else
            Time.timeScale = 1;
    }
}