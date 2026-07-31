using UnityEngine;
using UnityEngine.Audio;

public static class BootStrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

    public static void Init()
    {
        AudioMixer audioMixer = Resources.Load<AudioMixer>("AudioMixer");

        GameObject audioManagerObject = new GameObject("[AudioManager]");
        Object.DontDestroyOnLoad(audioManagerObject);

        AudioSource clickSound = audioManagerObject.AddComponent<AudioSource>();
        clickSound.clip = Resources.Load<AudioClip>("ClickSound");

        AudioMixerGroup[] sfxGroups = audioMixer.FindMatchingGroups("Sfx");
        clickSound.outputAudioMixerGroup = sfxGroups[0];

        AudioSource gameplayMusic = audioManagerObject.AddComponent<AudioSource>();
        gameplayMusic.clip = Resources.Load<AudioClip>("GameplayMusic");

        AudioMixerGroup[] musicGroups = audioMixer.FindMatchingGroups("Music");
        gameplayMusic.outputAudioMixerGroup = musicGroups[0];

        ServiceLocator.Instance.AddService(new AudioManager(audioMixer, clickSound, gameplayMusic));
    }
}
