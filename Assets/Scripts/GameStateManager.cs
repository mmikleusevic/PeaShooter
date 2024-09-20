using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;
    public event Action OnPlayerDied;

    private EntityQuery playerDeadEntityQuery;

    private bool isDead = false;
    public bool IsDead => IsDead;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        playerDeadEntityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<PlayerDeadComponent>()
            .Build(entityManager);

        ResumeTheGame();
    }

    private void Update()
    {
        if (playerDeadEntityQuery.CalculateEntityCount() > 0)
        {
            if (isDead) return;

            PlayerDied();
        }
    }

    private void PlayerDied()
    {
        OnPlayerDied?.Invoke();
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