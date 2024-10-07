using System;
using Unity.Entities;
using UnityEngine;

public class AbilityPickerRNG : MonoBehaviour
{
    public event Action OnAbilityChosen;

    private PlayerExperienceSystem playerExperienceSystem;

    private void Start()
    {
        playerExperienceSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerExperienceSystem>();

        if (playerExperienceSystem != null) playerExperienceSystem.OnLevelUp += OnLevelUp;
    }

    private void OnDestroy()
    {
        if (playerExperienceSystem != null) playerExperienceSystem.OnLevelUp -= OnLevelUp;
    }

    private void OnLevelUp()
    {
        // Instantiate 3-4 Random UI Cards of abilities so that player can pick a new ability or upgrade his old one
        // Make it scriptable objects probably with data to use for cards
        // After he picks invoke OnAbilityChosen to resume the game
        OnAbilityChosen?.Invoke();
    }
}