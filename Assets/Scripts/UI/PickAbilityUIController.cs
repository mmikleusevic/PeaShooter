using System.Collections.Generic;
using System.Linq;
using Game;
using Managers;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class PickAbilityUIController : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset abilityCardTemplate;
    private VisualElement abilityCardContainer;
    private VisualElement abilityPicker;

    private PlayerExperienceSystem playerExperienceSystem;

    private VisualElement rootElement;

    private void Awake()
    {
        playerExperienceSystem =
            World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerExperienceSystem>();

        if (playerExperienceSystem != null) playerExperienceSystem.OnLevelUp += OnLevelUp;
    }

    private void Start()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        abilityPicker = rootElement.Q<VisualElement>("abilityPicker");
        abilityCardContainer = rootElement.Q<VisualElement>("abilityCardContainer");
    }

    private void OnDestroy()
    {
        if (playerExperienceSystem != null) playerExperienceSystem.OnLevelUp -= OnLevelUp;
    }

    private void OnLevelUp()
    {
        List<AbilityData> randomAbilities = AbilityManager.Instance.GetRandomAbilityChoices();

        if (randomAbilities.Count == 0) return;

        foreach (AbilityData ability in randomAbilities)
        {
            VisualElement template = abilityCardTemplate.CloneTree();
            VisualElement abilityCard = template.Children().First();

            VisualElement image = abilityCard.Q<VisualElement>("image");
            Label name = abilityCard.Q<Label>("name");
            Label description = abilityCard.Q<Label>("description");

            image.style.backgroundImage = new StyleBackground(ability.icon);
            name.text = ability.abilityName;
            description.text = ability.abilityDescription;

            abilityCard.userData = ability;
            abilityCard.RegisterCallback<ClickEvent>(OnAbilityChosen);

            abilityCardContainer.Add(abilityCard);
        }

        Show();
    }

    private void OnAbilityChosen(ClickEvent evt)
    {
        VisualElement abilityCard = evt.currentTarget as VisualElement;
        AbilityData ability = abilityCard?.userData as AbilityData;

        AbilityManager.Instance.AcquireAbility(ability);

        GameStateManager.Instance.OnAbilityChosen();
        Hide();

        // TODO: Remove later if making particle abilities works
        // if (!prefabsQuery.HasSingleton<Prefabs>()) return;
        //
        // GameObject vfxPrefab = prefabsQuery.GetSingleton<Prefabs>().fireflyBoltsVFX;
        // GameObject newVfx = Instantiate(vfxPrefab, vfxPrefab.transform.position, vfxPrefab.transform.rotation);
        //
        // if (!playerEntityQuery.HasSingleton<PlayerComponent>()) return;
        //
        // Entity entity = playerEntityQuery.GetSingletonEntity();
        //
        // ParticleReference particleReference = new ParticleReference
        // {
        //     value = newVfx,
        //     updateTransform = false
        // };
        //
        // entityManager.AddComponentData(entity, particleReference);
    }

    private void Hide()
    {
        CleanupCards();

        abilityPicker.visible = false;
    }

    private void Show()
    {
        abilityPicker.visible = true;
    }

    private void CleanupCards()
    {
        for (int i = 0; i < abilityCardContainer.childCount; i++)
        {
            abilityCardContainer[i].UnregisterCallback<ClickEvent>(OnAbilityChosen);
        }

        abilityCardContainer.Clear();
    }
}