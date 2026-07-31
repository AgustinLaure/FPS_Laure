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
    [SerializeField] private Image healthBar;

    private const string remainingEnemiesText = "Remaining enemies: ";

    private void Awake()
    {
        player.OnGunCurrentAmmoValueChanged += HandlePlayerAmmoValueChanged;
        player.OnArmedStateChanged += HandlePlayerOnArmedStateChanged;
        player.OnGunSwap += HandlePlayerGunSwap;
        player.OnHealthValueChanged += HandlePlayerHealthValueChanged;
    }

    public void UpdateEnemiesLeft(int enemiesLeft)
    {
        remainingEnemiesTMP.text = remainingEnemiesText + enemiesLeft.ToString();
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
    }
}
