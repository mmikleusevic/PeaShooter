using System;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class OptionsUIController : MonoBehaviour
    {
        private Button closeButton;
        private Toggle musicToggle;

        private VisualElement optionsUI;
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
            optionsUI = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("optionsUI");
            musicToggle = optionsUI.Q<Toggle>("music");
            volumeSlider = optionsUI.Q<Slider>("musicVolume");
            closeButton = optionsUI.Q<Button>("close");

            optionsUI?.RegisterCallback<NavigationCancelEvent>(OnCancel);

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
            optionsUI?.UnregisterCallback<NavigationCancelEvent>(OnCancel);
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
            optionsUI.style.visibility = Visibility.Visible;
            optionsUI.schedule.Execute(() => musicToggle.Focus())
                .Until(() => optionsUI.focusController.focusedElement == musicToggle);
        }

        private void Hide()
        {
            SoundManager.Instance.SavePlayerPrefs();
            optionsUI.style.visibility = Visibility.Hidden;
            OnOptionsClosed?.Invoke();
        }
    }
}