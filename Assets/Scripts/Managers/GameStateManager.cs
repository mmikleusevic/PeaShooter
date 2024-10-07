using Unity.Entities;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    [SerializeField] private AbilityPickerRNG abilityPickerRNG;
    private PlayerHealthSystem playerHealthSystem;
    private PlayerExperienceSystem playerExperienceSystem;
    private bool isPausedForLevelUp = false;
    private bool isDead = false;
    public bool IsDead => IsDead;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        playerHealthSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerHealthSystem>();
        playerExperienceSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerExperienceSystem>();

        if (playerHealthSystem != null) playerHealthSystem.OnPlayerDied += OnPlayerDied;
        if (playerExperienceSystem != null) playerExperienceSystem.OnLevelUp += OnLevelUp;
        if (abilityPickerRNG != null) abilityPickerRNG.OnAbilityChosen += OnAbilityChosen;

        ResumeGame();
    }

    private void OnDestroy()
    {
        if (playerHealthSystem != null) playerHealthSystem.OnPlayerDied -= OnPlayerDied;
        if (playerExperienceSystem != null) playerExperienceSystem.OnLevelUp -= OnLevelUp;
        if (abilityPickerRNG != null) abilityPickerRNG.OnAbilityChosen -= OnAbilityChosen;
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

    private void OnAbilityChosen()
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