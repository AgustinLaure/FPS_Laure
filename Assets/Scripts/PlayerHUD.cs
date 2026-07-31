using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] private CanvasGroup armedHUDCanvasGroup;
    [SerializeField] private TextMeshProUGUI currentAmmoTMP;
    [SerializeField] private TextMeshProUGUI maxAmmoTMP;
    [SerializeField] private TextMeshProUGUI remainingEnemiesTMP;

    [SerializeField] private CanvasGroup handIconCanvasGroup;
    [SerializeField] private CanvasGroup rifleIconCanvasGroup;
    [SerializeField] private CanvasGroup pistolIconCanvasGroup;

    [SerializeField] private Image healthBar;

    private const string remainingEnemiesText = "Remaining enemies: ";
    [SerializeField] private PlayerAction playerAction;

    [SerializeField] private float inactiveToolAlpha;

    private void Awake()
    {
        player.OnGunCurrentAmmoValueChanged += HandlePlayerAmmoValueChanged;
        player.OnArmedStateChanged += HandlePlayerOnArmedStateChanged;
        player.OnGunSwap += HandlePlayerGunSwap;
        player.OnHealthValueChanged += HandlePlayerHealthValueChanged;
        playerAction.OnSwapTool += HandleSwapTool;
    }

    private void Start()
    {
        HandleSwapTool(player.GetCurrentTool);
    }

    public void UpdateEnemiesLeft(int enemiesLeft)
    {
        remainingEnemiesTMP.text = remainingEnemiesText + enemiesLeft.ToString();
    }

    private void HandleSwapTool(CURRENT_TOOL currentTool)
    {
        handIconCanvasGroup.alpha = inactiveToolAlpha;
        rifleIconCanvasGroup.alpha = inactiveToolAlpha;
        pistolIconCanvasGroup.alpha = inactiveToolAlpha;

        CanvasGroup toTurnOn = handIconCanvasGroup;

        switch (currentTool)
        {
            case CURRENT_TOOL.None:
                toTurnOn = handIconCanvasGroup;
                break;

            case CURRENT_TOOL.Rifle:
                toTurnOn = rifleIconCanvasGroup;
                break;

            case CURRENT_TOOL.Pistol:
                toTurnOn = pistolIconCanvasGroup;
                break;

            default:
                break;
        }

        toTurnOn.alpha = 1f;
    }

    private void HandlePlayerGunSwap(Gun gun)
    {
        currentAmmoTMP.text = gun.GetCurrentAmmo.ToString();
        maxAmmoTMP.text = gun.GetMaxAmmo.ToString();
    }

    private void HandlePlayerAmmoValueChanged(int value)
    {
        currentAmmoTMP.text = value.ToString();
    }

    private void HandlePlayerOnArmedStateChanged(bool isArmed)
    {
        SetSpriteActive(armedHUDCanvasGroup, isArmed);
    }

    private void HandlePlayerHealthValueChanged(float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    private void SetSpriteActive(CanvasGroup canvasGroup, bool isActive)
    {
        canvasGroup.alpha = isActive ? 1f : 0f;
    }

    private void OnDestroy()
    {
        player.OnGunCurrentAmmoValueChanged -= HandlePlayerAmmoValueChanged;
        player.OnArmedStateChanged -= HandlePlayerOnArmedStateChanged;
        player.OnGunSwap -= HandlePlayerGunSwap;
        player.OnHealthValueChanged -= HandlePlayerHealthValueChanged;
        playerAction.OnSwapTool += HandleSwapTool;
    }
}
