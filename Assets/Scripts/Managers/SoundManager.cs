using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const string MUSIC = "Music";
    private const string MUSIC_VOLUME = "MusicVolume";

    [SerializeField] private AudioSource musicSource;
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
        musicSource.mute = isMuted;

        float volume = PlayerPrefs.GetFloat(MUSIC_VOLUME);
        musicSource.volume = volume;
    }

    public void SavePlayerPrefs()
    {
        PlayerPrefs.SetString(MUSIC, musicSource.mute.ToString());
        PlayerPrefs.SetFloat(MUSIC_VOLUME, musicSource.volume);
    }

    public void SetMusic(bool value)
    {
        musicSource.mute = value;
    }

    public void SetVolume(float value)
    {
        musicSource.volume = value;
    }

    public bool GetIsMusicEnabled()
    {
        return !musicSource.mute;
    }

    public float GetMusicVolume()
    {
        return musicSource.volume;
    }
}