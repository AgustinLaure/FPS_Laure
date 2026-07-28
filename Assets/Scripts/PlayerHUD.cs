using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    [SerializeField] private Player player;

    [SerializeField] private CanvasGroup armedHUDCanvasGroup;
    [SerializeField] private TextMeshProUGUI currentAmmoTMP;
    [SerializeField] private TextMeshProUGUI maxAmmoTextTMP;
    [SerializeField] private Image healthBar;

    private void Awake()
    {
        player.OnGunCurrentAmmoValueChanged += HandlePlayerAmmoValueChanged;
        player.OnArmedStateChanged += HandlePlayerOnArmedStateChanged;
        player.OnGunSwap += HandlePlayerGunSwap;
        player.OnHealthValueChanged += HandlePlayerHealthValueChanged;
    }

    private void HandlePlayerGunSwap(Gun gun)
    {
        currentAmmoTMP.text = gun.GetCurrentAmmo.ToString();
        maxAmmoTextTMP.text = gun.GetMaxAmmo.ToString();
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

        Debug.Log(healthBar.fillAmount);
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
