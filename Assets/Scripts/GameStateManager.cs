using Unity.Entities;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private CollisionDamageSystem collisionDamageSystem;

    private void OnEnable()
    {
        collisionDamageSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<CollisionDamageSystem>();

        if (collisionDamageSystem != null)
        {
            collisionDamageSystem.OnPlayerDied += OnPlayerDied;
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLoad += OnGameLoad;
        }
    }

    private void OnDisable()
    {
        if (collisionDamageSystem != null)
        {
            collisionDamageSystem.OnPlayerDied -= OnPlayerDied;
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLoad -= OnGameLoad;
        }
    }

    private void OnPlayerDied()
    {
        StopTheGame();
    }

    private void OnGameLoad()
    {
        StartTheGame();
    }

    private void StopTheGame()
    {
        Time.timeScale = 0;
    }

    private void StartTheGame()
    {
        Time.timeScale = 1;
    }
}