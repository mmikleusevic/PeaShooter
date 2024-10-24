using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public class ExperienceUIController : MonoBehaviour
{
    private PlayerExperienceSystem playerExperienceSystem;
    private ProgressBar experienceBar;

    private void Awake()
    {
        playerExperienceSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerExperienceSystem>();

        if (playerExperienceSystem != null) playerExperienceSystem.OnGainedExp += OnGainedExp;
    }

    private void OnDestroy()
    {
        if (playerExperienceSystem != null) playerExperienceSystem.OnGainedExp -= OnGainedExp;
    }

    private void OnGainedExp(uint currentEXP, uint maxEXP)
    {
        SetProgressBar(currentEXP, maxEXP);
    }

    private void Start()
    {
        VisualElement uiVisualELement = GetComponent<UIDocument>().rootVisualElement;
        experienceBar = uiVisualELement.Q<ProgressBar>("experience");
    }

    private void SetProgressBar(uint currentEXP, uint maxEXP)
    {
        experienceBar.value = currentEXP;
        experienceBar.highValue = maxEXP;
        experienceBar.title = "EXP: " + experienceBar.value + "/" + experienceBar.highValue;
    }
}