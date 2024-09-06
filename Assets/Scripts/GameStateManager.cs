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
    }

    private void OnDisable()
    {
        if (collisionDamageSystem != null)
        {
            collisionDamageSystem.OnPlayerDied -= OnPlayerDied;
        }

        StartTheGame();
    }

    private void OnPlayerDied()
    {
        StopTheGame();
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