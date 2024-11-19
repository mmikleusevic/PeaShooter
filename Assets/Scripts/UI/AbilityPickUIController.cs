using System.Collections.Generic;
using System.Linq;
using Game;
using Managers;
using Systems;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class AbilityPickUIController : MonoBehaviour
    {
        [SerializeField] private PauseUIController pauseUIController;
        [SerializeField] private VisualTreeAsset abilityCardTemplate;

        private VisualElement abilityCardContainer;
        private VisualElement abilityPicker;

        private bool isWaitingForPick;
        private PlayerExperienceSystem playerExperienceSystem;
        private VisualElement rootElement;

        private void Awake()
        {
            playerExperienceSystem =
                World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerExperienceSystem>();

            if (pauseUIController)
            {
                pauseUIController.OnPauseUIOpened += OnPauseUIOpened;
                pauseUIController.OnPauseUIClosed += OnPauseUIClosed;
            }

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
            if (pauseUIController)
            {
                pauseUIController.OnPauseUIOpened -= OnPauseUIOpened;
                pauseUIController.OnPauseUIClosed -= OnPauseUIClosed;
            }

            if (playerExperienceSystem != null) playerExperienceSystem.OnLevelUp -= OnLevelUp;
        }

        private void OnPauseUIOpened()
        {
            Hide();
        }

        private void OnPauseUIClosed()
        {
            if (isWaitingForPick)
            {
                Show();
            }
        }

        private void OnLevelUp()
        {
            List<AbilityData> randomAbilities = AbilityManager.Instance.GetRandomAbilityChoices();

            if (randomAbilities.Count == 0) return;

            GameStateManager.Instance.PauseOnLevelUp();

            isWaitingForPick = true;

            for (int i = 0; i < randomAbilities.Count; i++)
            {
                AbilityData abilityData = randomAbilities[i];

                VisualElement template = abilityCardTemplate.CloneTree();
                VisualElement abilityCard = template.Children().First();

                VisualElement image = abilityCard.Q<VisualElement>("image");
                Label name = abilityCard.Q<Label>("name");
                Label description = abilityCard.Q<Label>("description");

                image.style.backgroundImage = new StyleBackground(abilityData.icon);
                name.text = abilityData.abilityName;
                description.text = abilityData.Description;

                abilityCard.userData = abilityData;
                abilityCard.focusable = true;
                abilityCard.tabIndex = i;

                abilityCard.RegisterCallback<ClickEvent>(OnAbilityChosen);
                abilityCard.RegisterCallback<NavigationSubmitEvent>(OnAbilityChosen);

                abilityCardContainer.Add(abilityCard);

                if (i == 0) abilityCard.Focus();
            }

            Show();
        }

        private void OnAbilityChosen(EventBase evt)
        {
            VisualElement element = evt.currentTarget as VisualElement;

            if (element == null) return;

            AbilityData abilityData = element.userData as AbilityData;

            if (!abilityData) return;

            AbilityManager.Instance.AcquireAbility(abilityData);

            GameStateManager.Instance.OnAbilityChosen();

            isWaitingForPick = false;

            Hide();
        }

        private void Hide()
        {
            if (!isWaitingForPick) CleanupCards();

            abilityPicker.style.visibility = Visibility.Hidden;
        }

        private void Show()
        {
            abilityPicker.style.visibility = Visibility.Visible;

            abilityCardContainer.schedule.Execute(() => abilityCardContainer[0].Focus())
                .Until(() => abilityCardContainer.focusController.focusedElement == abilityCardContainer[0]);
        }

        private void CleanupCards()
        {
            for (int i = 0; i < abilityCardContainer.childCount; i++)
            {
                abilityCardContainer[i].UnregisterCallback<ClickEvent>(OnAbilityChosen);
                abilityCardContainer[i].UnregisterCallback<NavigationSubmitEvent>(OnAbilityChosen);
            }

            abilityCardContainer.Clear();
        }
    }
}