using Systems;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class ExperienceUIController : MonoBehaviour
    {
        private ProgressBar experienceProgressBar;
        private PlayerExperienceSystem playerExperienceSystem;

        private void Awake()
        {
            playerExperienceSystem =
                World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerExperienceSystem>();

            if (playerExperienceSystem != null) playerExperienceSystem.OnGainedExp += OnGainedExp;
        }

        private void Start()
        {
            VisualElement uiVisualELement = GetComponent<UIDocument>().rootVisualElement;
            experienceProgressBar = uiVisualELement.Q<ProgressBar>("experience");
        }

        private void OnDestroy()
        {
            if (playerExperienceSystem != null) playerExperienceSystem.OnGainedExp -= OnGainedExp;
        }

        private void OnGainedExp(uint currentExp, uint maxExp)
        {
            SetProgressBar(currentExp, maxExp);
        }

        private void SetProgressBar(uint currentExp, uint maxExp)
        {
            experienceProgressBar.value = currentExp;
            experienceProgressBar.highValue = maxExp;
            experienceProgressBar.title = "EXP: " + experienceProgressBar.value + "/" + experienceProgressBar.highValue;
        }
    }
}