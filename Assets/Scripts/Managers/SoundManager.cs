using UnityEngine;

namespace Managers
{
    public class SoundManager : MonoBehaviour
    {
        private const string MUSIC = "Music";
        private const string MUSIC_VOLUME = "MusicVolume";

        [SerializeField] private AudioSource musicAudioSource;
        public static SoundManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            SetPlayerPrefValues();
        }

        private void SetPlayerPrefValues()
        {
            bool.TryParse(PlayerPrefs.GetString(MUSIC), out bool isMuted);
            musicAudioSource.mute = isMuted;

            float volume = PlayerPrefs.GetFloat(MUSIC_VOLUME);
            musicAudioSource.volume = volume;
        }

        public void SavePlayerPrefs()
        {
            PlayerPrefs.SetString(MUSIC, musicAudioSource.mute.ToString());
            PlayerPrefs.SetFloat(MUSIC_VOLUME, musicAudioSource.volume);
        }

        public void SetMusic(bool value)
        {
            musicAudioSource.mute = value;
        }

        public void SetVolume(float value)
        {
            musicAudioSource.volume = value;
        }

        public bool GetIsMusicEnabled()
        {
            return !musicAudioSource.mute;
        }

        public float GetMusicVolume()
        {
            return musicAudioSource.volume;
        }
    }
}