using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class OptionsUIController : MonoBehaviour
{
    private Button closeButton;
    private Toggle musicToggle;

    private VisualElement optionsElement;
    private Slider volumeSlider;
    private VisualElement volumeSliderElement;
    private Label volumeSliderLabel;
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

        optionsElement?.RegisterCallback<NavigationCancelEvent>(OnCancel);

        if (musicToggle != null)
        {
            musicToggle.value = SoundManager.Instance.GetIsMusicEnabled();
            musicToggle.RegisterValueChangedCallback(ToggleMusic);
        }

        if (volumeSlider != null)
        {
            volumeSlider.value = SoundManager.Instance.GetMusicVolume();
            volumeSliderLabel = volumeSlider.Children().First() as Label;
            volumeSliderElement = volumeSlider.Children().Last();
            volumeSliderElement?.RegisterCallback<FocusInEvent>(FocusVolume);
            volumeSliderElement?.RegisterCallback<FocusOutEvent>(UnfocusVolume);
            volumeSlider.RegisterValueChangedCallback(SetVolume);
        }

        if (closeButton != null) closeButton.clicked += Hide;
    }

    private void OnDestroy()
    {
        optionsElement?.UnregisterCallback<NavigationCancelEvent>(OnCancel);
        musicToggle?.UnregisterValueChangedCallback(ToggleMusic);
        volumeSlider?.UnregisterValueChangedCallback(SetVolume);
        volumeSliderElement?.UnregisterCallback<FocusInEvent>(FocusVolume);
        volumeSliderElement?.UnregisterCallback<FocusOutEvent>(UnfocusVolume);

        if (closeButton != null) closeButton.clicked -= Hide;
    }

    public event Action OnOptionsClosed;

    private void OnCancel(NavigationCancelEvent evt)
    {
        Hide();
    }

    private void ToggleMusic(ChangeEvent<bool> toggleValue)
    {
        SoundManager.Instance.SetMusic(!toggleValue.newValue);
    }

    private void FocusVolume(FocusInEvent evt)
    {
        volumeSliderLabel.AddToClassList("musicVolumeFocus");
    }

    private void UnfocusVolume(FocusOutEvent evt)
    {
        volumeSliderLabel.RemoveFromClassList("musicVolumeFocus");
    }

    private void SetVolume(ChangeEvent<float> volumeValue)
    {
        SoundManager.Instance.SetVolume(volumeValue.newValue);
    }

    public void Show()
    {
        optionsElement.style.visibility = Visibility.Visible;
        optionsElement.schedule.Execute(() => musicToggle.Focus())
            .Until(() => optionsElement.focusController.focusedElement == musicToggle);
    }

    private void Hide()
    {
        SoundManager.Instance.SavePlayerPrefs();
        optionsElement.style.visibility = Visibility.Hidden;
        OnOptionsClosed?.Invoke();
    }
}