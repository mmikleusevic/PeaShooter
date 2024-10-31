using UnityEngine;
using UnityEngine.UIElements;

public class OptionsUIController : MonoBehaviour
{
    private Button closeButton;
    private Toggle musicToggle;

    private VisualElement optionsElement;
    private Slider volumeSlider;
    public static OptionsUIController Instance { get; private set; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        optionsElement = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("optionsUI");
        musicToggle = optionsElement.Q<Toggle>("music");
        volumeSlider = optionsElement.Q<Slider>("musicVolume");
        closeButton = optionsElement.Q<Button>("close");

        if (musicToggle != null)
        {
            musicToggle.value = SoundManager.Instance.GetIsMusicEnabled();
            musicToggle.RegisterValueChangedCallback(ToggleMusic);
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = SoundManager.Instance.GetMusicVolume();
            volumeSlider.RegisterValueChangedCallback(SetVolume);
        }

        if (closeButton != null) closeButton.clicked += Hide;
    }

    private void OnDestroy()
    {
        if (musicToggle != null) musicToggle.UnregisterValueChangedCallback(ToggleMusic);
        if (volumeSlider != null) volumeSlider.UnregisterValueChangedCallback(SetVolume);
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