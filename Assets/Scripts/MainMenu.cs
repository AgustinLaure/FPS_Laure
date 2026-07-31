using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup titleScreenCanvasGroup;
    [SerializeField] private CanvasGroup creditsScreenCanvasGroup;
    [SerializeField] private CanvasGroup settingsScreenCanvasGroup;

    [SerializeField] private Button titleScreenPlayButton;
    [SerializeField] private Button titleScreenSettingsButton;
    [SerializeField] private Button titleScreenCreditsButton;
    [SerializeField] private Button titleScreenExitButton;

    [SerializeField] private Button creditsScreenBackButton;
    [SerializeField] private Button settingsScreenBackButton;

    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;

    private void Awake()
    {
#if UNITY_WEBGL
        titleScreenExitButton.interactable = false;
#endif

        titleScreenPlayButton.onClick.AddListener(HandleTitlePlayButtonClick);
        titleScreenSettingsButton.onClick.AddListener(HandleTitleSettingsButtonClick);
        titleScreenCreditsButton.onClick.AddListener(HandleTitleCreditsButtonClick);
        titleScreenExitButton.onClick.AddListener(HandleTitleExitButtonClick);
        creditsScreenBackButton.onClick.AddListener(HandleCreditsBackButtonClick);
        settingsScreenBackButton.onClick.AddListener(HandleSettingsBackButtonClick);

        masterVolumeSlider.onValueChanged.AddListener(HandleMasterVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(HandleSfxVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(HandleMusicVolumeChanged);
    }

    private void Start()
    {
        masterVolumeSlider.value = ServiceLocator.Instance.GetService<AudioManager>().MasterVolume;
        sfxVolumeSlider.value = ServiceLocator.Instance.GetService<AudioManager>().SfxVolume;
        musicVolumeSlider.value = ServiceLocator.Instance.GetService<AudioManager>().MusicVolume;
    }

    private void HandleTitlePlayButtonClick()
    {
        SceneManager.LoadScene("Gameplay");
        ServiceLocator.Instance.GetService<AudioManager>().GetButtonPressedSound.Play();
    }

    private void HandleTitleSettingsButtonClick()
    {
        UiUtils.SetCanvasActive(titleScreenCanvasGroup, false);
        UiUtils.SetCanvasActive(settingsScreenCanvasGroup, true);

        ServiceLocator.Instance.GetService<AudioManager>().GetButtonPressedSound.Play();
    }

    private void HandleTitleCreditsButtonClick()
    {
        UiUtils.SetCanvasActive(titleScreenCanvasGroup, false);
        UiUtils.SetCanvasActive(creditsScreenCanvasGroup, true);

        ServiceLocator.Instance.GetService<AudioManager>().GetButtonPressedSound.Play();
    }

    private void HandleCreditsBackButtonClick()
    {
        UiUtils.SetCanvasActive(titleScreenCanvasGroup, true);
        UiUtils.SetCanvasActive(creditsScreenCanvasGroup, false);

        ServiceLocator.Instance.GetService<AudioManager>().GetButtonPressedSound.Play();
    }

    private void HandleSettingsBackButtonClick()
    {
        UiUtils.SetCanvasActive(titleScreenCanvasGroup, true);
        UiUtils.SetCanvasActive(settingsScreenCanvasGroup, false);

        ServiceLocator.Instance.GetService<AudioManager>().GetButtonPressedSound.Play();
    }

    private void HandleTitleExitButtonClick()
    {
        ServiceLocator.Instance.GetService<AudioManager>().GetButtonPressedSound.Play();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void HandleMasterVolumeChanged(float value)
    {
        ServiceLocator.Instance.GetService<AudioManager>().MasterVolume = value;
    }

    private void HandleSfxVolumeChanged(float value)
    {
        ServiceLocator.Instance.GetService<AudioManager>().SfxVolume = value;
    }

    private void HandleMusicVolumeChanged(float value)
    {
        ServiceLocator.Instance.GetService<AudioManager>().MusicVolume = value;
    }

    private void OnDestroy()
    {
        titleScreenPlayButton.onClick.RemoveListener(HandleTitlePlayButtonClick);
        titleScreenSettingsButton.onClick.RemoveListener(HandleTitleSettingsButtonClick);
        titleScreenCreditsButton.onClick.RemoveListener(HandleTitleCreditsButtonClick);
        titleScreenExitButton.onClick.RemoveListener(HandleTitleExitButtonClick);
        creditsScreenBackButton.onClick.RemoveListener(HandleCreditsBackButtonClick);
        settingsScreenBackButton.onClick.RemoveListener(HandleSettingsBackButtonClick);
        masterVolumeSlider.onValueChanged.RemoveListener(HandleMasterVolumeChanged);
        sfxVolumeSlider.onValueChanged.RemoveListener(HandleSfxVolumeChanged);
        musicVolumeSlider.onValueChanged.RemoveListener(HandleMusicVolumeChanged);
    }
}
