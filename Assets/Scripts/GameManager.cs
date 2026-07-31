using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] private float timeAfterWinning;

    [SerializeField] private CanvasGroup pauseScreenCanvasGroup;
    [SerializeField] private Button pauseScreenResumeButton;
    [SerializeField] private Button pauseScreenSettingsButton;
    [SerializeField] private Button pauseScreenMainMenuButton;

    [SerializeField] private CanvasGroup endScreenCanvasGroup;
    [SerializeField] private Button endScreenContinueButton;
    [SerializeField] private Button endScreenMainMenuButton;

    [SerializeField] private CanvasGroup settingsScreenCanvasGroup;
    [SerializeField] private Button settingsBackButton;
    [SerializeField] private Slider settingsMasterSlider;
    [SerializeField] private Slider settingsMusicSlider;
    [SerializeField] private Slider settingsSfxSlider;

    [SerializeField] private Image endScreenTitle;
    [SerializeField] private Sprite loseTitleSprite;
    [SerializeField] private Sprite winTitleSprite;

    [SerializeField] private AudioSource winSound;

    private const string loseText = "You lost!";
    private const string winText = "You win!";
    private const string enemyTag = "Enemy";

    private bool isPaused = false;
    private bool hasLost = false;

    private List<Enemy> enemies = new List<Enemy>();

    [SerializeField] private PlayerHud playerHud;
    private PlayerAction playerAction;
    private PlayerMovement playerMovement;

    public bool GetIsPaused { get { return isPaused; } }

    private Coroutine winGameCoroutine = null;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1f;
    }
    private void Start()
    {
        GameObject[] enemiesInScene = GameObject.FindGameObjectsWithTag(enemyTag);

        foreach (GameObject enemyInScene in enemiesInScene)
        {
            Enemy enemyComponent = enemyInScene.GetComponent<Enemy>();
            enemies.Add(enemyComponent);

            enemyComponent.OnDied += HandleEnemyDeath;
        }

        playerHud.UpdateEnemiesLeft(enemies.Count);

        if (!ServiceLocator.Instance.GetService<AudioManager>().GetGameplayMusic.isPlaying)
        {
            ServiceLocator.Instance.GetService<AudioManager>().GetGameplayMusic.Play();
        }

        playerAction = player.GetComponent<PlayerAction>();
        playerMovement = player.GetComponent<PlayerMovement>();

        playerAction.OnPause += HandlePlayerPause;
        player.GetHealthPoints.OnDied += HandlePlayerDeath;

        pauseScreenResumeButton.onClick.AddListener(HandleResumeButtonClick);
        pauseScreenSettingsButton.onClick.AddListener(HandleSettingsButtonClick);
        pauseScreenMainMenuButton.onClick.AddListener(HandleMainMenuButtonClick);
        endScreenContinueButton.onClick.AddListener(HandleContinueButtonClick);
        endScreenMainMenuButton.onClick.AddListener(HandleMainMenuButtonClick);

        settingsBackButton.onClick.AddListener(HandleSettingsBackButtonClick);
        settingsMasterSlider.onValueChanged.AddListener(HandleMasterVolumeChange);
        settingsMusicSlider.onValueChanged.AddListener(HandleMusicVolumeChange);
        settingsSfxSlider.onValueChanged.AddListener(HandleSfxVolumeChange);
    }

    private void Update()
    {
        if (enemies.Count <= 0)
        {
            if (winGameCoroutine == null)
            {
                winGameCoroutine = StartCoroutine(WinGameCoroutine());
            }
        }

        Debug.Log(enemies.Count);
    }

    private void OnDestroy()
    {
        playerAction.OnPause -= HandlePlayerPause;
        player.GetHealthPoints.OnDied -= HandlePlayerDeath;

        pauseScreenResumeButton.onClick.RemoveListener(HandleResumeButtonClick);
        pauseScreenSettingsButton.onClick.RemoveListener(HandleSettingsButtonClick);
        pauseScreenMainMenuButton.onClick.RemoveListener(HandleMainMenuButtonClick);
        endScreenContinueButton.onClick.RemoveListener(HandleContinueButtonClick);
        endScreenMainMenuButton.onClick.RemoveListener(HandleMainMenuButtonClick);

        settingsBackButton.onClick.RemoveListener(HandleSettingsBackButtonClick);
        settingsMasterSlider.onValueChanged.RemoveListener(HandleMasterVolumeChange);
        settingsMusicSlider.onValueChanged.RemoveListener(HandleMusicVolumeChange);
        settingsSfxSlider.onValueChanged.RemoveListener(HandleSfxVolumeChange);

        foreach (Enemy enemy in enemies)
        {
            enemy.OnDied -= HandleEnemyDeath;
        }
    }

    private void SetPause(bool state)
    {
        UiUtils.SetCanvasActive(pauseScreenCanvasGroup, state);

        Time.timeScale = state ? 0f : 1f;
        Cursor.visible = state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;

        isPaused = state;
    }

    private void EndGame()
    {
        UiUtils.SetCanvasActive(endScreenCanvasGroup, true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        endScreenTitle.sprite = hasLost ? loseTitleSprite : winTitleSprite;

        playerAction.enabled = false;
        playerMovement.enabled = false;
    }

    private IEnumerator WinGameCoroutine()
    {
        yield return new WaitForSeconds(timeAfterWinning);

        EndGame();
    }

    private void HandlePlayerDeath()
    {
        hasLost = true;

        EndGame();
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        enemies.Remove(enemy);
        playerHud.UpdateEnemiesLeft(enemies.Count);
    }

    private void HandleWinTrigger(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            EndGame();
            winSound.Play();
        }
    }

    private void HandlePlayerPause()
    {
        SetPause(!isPaused);
    }

    private void HandleResumeButtonClick()
    {
        ServiceLocator.Instance.GetService<AudioManager>().GetButtonPressedSound.Play();
        SetPause(false);
    }

    private void HandleSettingsButtonClick()
    {
        ServiceLocator.Instance.GetService<AudioManager>().GetButtonPressedSound.Play();
        UiUtils.SetCanvasActive(pauseScreenCanvasGroup, false);
        UiUtils.SetCanvasActive(settingsScreenCanvasGroup, true);
    }

    private void HandleMasterVolumeChange(float value)
    {
        ServiceLocator.Instance.GetService<AudioManager>().MasterVolume = value;
    }

    private void HandleMusicVolumeChange(float value)
    {
        ServiceLocator.Instance.GetService<AudioManager>().MusicVolume = value;
    }

    private void HandleSfxVolumeChange(float value)
    {
        ServiceLocator.Instance.GetService<AudioManager>().SfxVolume = value;
    }

    private void HandleSettingsBackButtonClick()
    {
        ServiceLocator.Instance.GetService<AudioManager>().GetButtonPressedSound.Play();

        UiUtils.SetCanvasActive(settingsScreenCanvasGroup, false);
        UiUtils.SetCanvasActive(pauseScreenCanvasGroup, true);
    }

    private void HandleMainMenuButtonClick()
    {
        ServiceLocator.Instance.GetService<AudioManager>().GetButtonPressedSound.Play();
        ServiceLocator.Instance.GetService<AudioManager>().GetGameplayMusic.Pause();
        SceneManager.LoadScene("MainMenu");
    }
    private void HandleContinueButtonClick()
    {
        ServiceLocator.Instance.GetService<AudioManager>().GetButtonPressedSound.Play();
        SceneManager.LoadScene("Gameplay");
    }
}
