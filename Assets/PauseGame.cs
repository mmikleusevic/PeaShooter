using Unity.Entities;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    private CollisionDamageSystem collisionDamageSystem;

    private void Start()
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
    }

    private void OnPlayerDied()
    {
        StopTheGame();
    }

    private void StopTheGame()
    {
        Time.timeScale = 0;
    }
}