using System;
using System.Collections;
using UnityEngine;

public abstract class AbilityBase : MonoBehaviour
{
    public event Action<float> OnAbilityUse;

    [Header("Ability Info")]
    [SerializeField] private string title;
    [SerializeField] private Sprite icon;
    [SerializeField] private float cooldownTime = 1f;

    private bool isOffCooldown = true;
    public void Use()
    {
        if (!isOffCooldown) return;

        OnAbilityUse?.Invoke(cooldownTime);
        Ability();
        StartCooldown();
    }

    protected abstract void Ability();

    private void StartCooldown()
    {
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        isOffCooldown = false;
        yield return new WaitForSeconds(cooldownTime);
        isOffCooldown = true;
    }
}
