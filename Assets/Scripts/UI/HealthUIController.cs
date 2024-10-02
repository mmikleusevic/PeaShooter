using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthUIController : MonoBehaviour
{
    private PlayerHealthSystem playerHealthSystem;
    private ProgressBar healthBar;

    private void Awake()
    {
        playerHealthSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerHealthSystem>();

        if (playerHealthSystem != null) playerHealthSystem.OnHealthChanged += OnHealthChanged;
    }

    private void Start()
    {
        VisualElement uiVisualELement = GetComponent<UIDocument>().rootVisualElement;
        healthBar = uiVisualELement.Q<ProgressBar>("health");
    }

    private void OnDestroy()
    {
        if (playerHealthSystem != null) playerHealthSystem.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float currentHP)
    {
        SetProgressBar(currentHP);
    }

    private void SetProgressBar(float currentHP)
    {
        healthBar.value = (float)Math.Round(currentHP, 2);
    }
}