using Unity.Entities;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    private CollisionDamageSystem collisionDamageSystem;

    private bool isDead = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        collisionDamageSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<CollisionDamageSystem>();

        if (collisionDamageSystem != null)
        {
            collisionDamageSystem.OnPlayerDied += OnPlayerDied;
        }

        ResumeTheGame();
    }

    private void OnDestroy()
    {
        if (collisionDamageSystem != null)
        {
            collisionDamageSystem.OnPlayerDied -= OnPlayerDied;
        }
    }

    private void OnPlayerDied()
    {
        isDead = true;
        PauseTheGame();
    }

    public void PauseTheGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeTheGame()
    {
        if (isDead)
        {
            PauseTheGame();
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}