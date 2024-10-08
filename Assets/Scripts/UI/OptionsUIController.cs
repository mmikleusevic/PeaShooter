using UnityEngine;
using UnityEngine.UIElements;

public class OptionsUIController : MonoBehaviour
{
    private VisualElement optionsElement;
    private Toggle musicToggle;
    private Button closeButton;
    private Slider slider;

    private void Awake()
    {
        optionsElement = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("optionsUI");

        musicToggle = optionsElement.Q<Toggle>("music");
        closeButton = optionsElement.Q<Button>("close");
        slider = optionsElement.Q<Slider>("musicVolume");

        if (closeButton != null) closeButton.clicked += Hide;
    }

    private void Start()
    {
        if (musicToggle != null)
        {
            musicToggle.value = SoundManager.Instance.GetIsMusicEnabled();
            musicToggle.RegisterValueChangedCallback(ToggleMusic);
        }

        if (slider != null)
        {
            slider.value = SoundManager.Instance.GetMusicVolume();
            slider.RegisterValueChangedCallback(SetVolume);
        }
    }

    private void OnDestroy()
    {
        if (musicToggle != null) musicToggle.UnregisterValueChangedCallback(ToggleMusic);
        if (slider != null) slider.UnregisterValueChangedCallback(SetVolume);
        if (closeButton != null) closeButton.clicked -= Hide;
    }

    private void ToggleMusic(ChangeEvent<bool> toggleValue)
    {
        SoundManager.Instance.SetMusic(!toggleValue.newValue);
    }

    private void SetVolume(ChangeEvent<float> volumeValue)
    {
        SoundManager.Instance.SetVolume(volumeValue.newValue);
    }

    public void Show()
    {
        optionsElement.visible = true;
    }

    private void Hide()
    {
        SoundManager.Instance.SavePlayerPrefs();

        optionsElement.visible = false;
    }
}
