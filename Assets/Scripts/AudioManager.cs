using UnityEngine;
using UnityEngine.Audio;

public class AudioManager
{
    private const float epsilon = 1e-06f;
    private const float decibelConst = 20f;

    private AudioMixer audioMixer;
    private AudioSource buttonPressedSound;
    private AudioSource gameplayMusic;

    private const float initialMasterVolume = 1f;
    private const float initialSfxVolume = 1f;
    private const float initialMusicVolume = 1f;

    private const float gameplayMusicVolume = 0.1f;

    private string wasGameOpenedBefore = "WasGameOpenedBefore";
    private const string masterVolumeKey = "MasterVolume";
    private const string sfxVolumeKey = "SfxVolume";
    private const string musicVolumeKey = "MusicVolume";

    private float masterVolume;
    private float sfxVolume;
    private float musicVolume;

    public float MasterVolume { get { return masterVolume; } set { masterVolume = value; audioMixer.SetFloat(masterVolumeKey, LinearToDecibel(masterVolume)); SaveConfig(); } }
    public float SfxVolume { get { return sfxVolume; } set { sfxVolume = value; audioMixer.SetFloat(sfxVolumeKey, LinearToDecibel(sfxVolume)); SaveConfig(); } }
    public float MusicVolume { get { return musicVolume; } set { musicVolume = value; audioMixer.SetFloat(musicVolumeKey, LinearToDecibel(musicVolume)); SaveConfig(); } }
    public AudioSource GetButtonPressedSound { get { return buttonPressedSound; } }
    public AudioSource GetGameplayMusic { get { return gameplayMusic; } }

    public AudioManager(AudioMixer audioMixer, AudioSource buttonPressedSound, AudioSource gameplayMusic)
    {
        this.audioMixer = audioMixer;
        this.buttonPressedSound = buttonPressedSound;
        this.gameplayMusic = gameplayMusic;
        gameplayMusic.volume = gameplayMusicVolume;

        if (PlayerPrefs.HasKey(wasGameOpenedBefore))
        {
            masterVolume = PlayerPrefs.GetFloat(masterVolumeKey);
            sfxVolume = PlayerPrefs.GetFloat(sfxVolumeKey);
            musicVolume = PlayerPrefs.GetFloat(musicVolumeKey);
        }
        else
        {
            masterVolume = initialMasterVolume;
            sfxVolume = initialSfxVolume;
            musicVolume = initialMusicVolume;

            PlayerPrefs.SetInt(wasGameOpenedBefore, 1);

            SaveConfig();
        }

        UpdateAudioMixerValues();
    }

    private float LinearToDecibel(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, epsilon, 1f)) * decibelConst;
    }

    private void UpdateAudioMixerValues()
    {
        audioMixer.SetFloat(masterVolumeKey, masterVolume);
        audioMixer.SetFloat(sfxVolumeKey, sfxVolume);
        audioMixer.SetFloat(musicVolumeKey, musicVolume);
    }

    public void SaveConfig()
    {
        PlayerPrefs.SetFloat(masterVolumeKey, masterVolume);
        PlayerPrefs.SetFloat(sfxVolumeKey, sfxVolume);
        PlayerPrefs.SetFloat(musicVolumeKey, musicVolume);

        PlayerPrefs.Save();
    }
}