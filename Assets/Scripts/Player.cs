using System;
using System.Collections.Generic;
using UnityEngine;

public enum CURRENT_TOOL
{
    None = -1,
    Rifle,
    Pistol
}

public class Player : MonoBehaviour
{
    private HealthPoints healthPoints;

    public event Action<int> OnGunCurrentAmmoValueChanged;
    public event Action<Gun> OnGunSwap;
    public event Action<bool> OnArmedStateChanged;
    public event Action<float, float> OnHealthValueChanged;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform[] gunPresets;
    [SerializeField] private GameObject[] gunPrefabs;
    [SerializeField] private GameObject gunsContainer;

    [SerializeField] private LayerMask playerLayerMask;

    private CURRENT_TOOL prevTool = CURRENT_TOOL.None;
    private CURRENT_TOOL currentTool = CURRENT_TOOL.None;

    private const string enemyTag = "Enemy";

    private List<Gun> guns = new List<Gun>();

    public HealthPoints GetHealthPoints { get { return healthPoints; } }


    public CURRENT_TOOL SetCurrentTool
    {
        set
        {
            prevTool = currentTool;
            currentTool = value;

            if (currentTool == CURRENT_TOOL.None && prevTool != CURRENT_TOOL.None)
            {
                OnArmedStateChanged?.Invoke(false);
            }
            else if (currentTool != CURRENT_TOOL.None && prevTool == CURRENT_TOOL.None)
            {
                OnArmedStateChanged?.Invoke(true);
            }

            if (currentTool != CURRENT_TOOL.None)
            {
                OnGunSwap?.Invoke(GetGun);
            }
        }
    }

    public Gun GetGun { get { return guns[(int)currentTool]; } }

    public List<Gun> GetGuns { get { return guns; } }

    public CURRENT_TOOL GetCurrentTool { get { return currentTool; } }

    private void Awake()
    {
        healthPoints = GetComponent<HealthPoints>();

        healthPoints.OnTakenDamage += HandleHealthPointsTakenDamage;
        healthPoints.OnDied += HandleHealthPointsDied;

        AddGun(CURRENT_TOOL.Rifle);
        AddGun(CURRENT_TOOL.Pistol);
    }

    private void AddGun(CURRENT_TOOL gunName)
    {
        Vector3 worldPosition = cameraTransform.TransformPoint(gunPresets[(int)gunName].localPosition);

        Quaternion worldRotation = cameraTransform.rotation * gunPresets[(int)gunName].localRotation;

        GameObject gun = Instantiate(gunPrefabs[(int)gunName], worldPosition, worldRotation, gunsContainer.transform);

        Gun gunComponent = gun.GetComponent<Gun>();

        gunComponent.OnCurrentAmmoValueChanged += HandleGunCurrentAmmoValueChanged;

        guns.Add(gunComponent);
        gunComponent.SetOwnerMask = playerLayerMask;
        gunComponent.SetLayer(LayerMask.NameToLayer("PlayerGun"), gun.transform);
        gunComponent.SetTargetTag = enemyTag;

        guns[(int)gunName].gameObject.SetActive(false);
    }

    private void HandleGunCurrentAmmoValueChanged(int value)
    {
        OnGunCurrentAmmoValueChanged?.Invoke(value);
    }

    private void HandleHealthPointsTakenDamage()
    {
        OnHealthValueChanged?.Invoke(healthPoints.GetCurrentHealth, healthPoints.GetMaxHealth);

        //hurt
    }

    private void HandleHealthPointsDied()
    {

    }

    private void OnDestroy()
    {
        foreach (Gun gun in guns)
        {
            gun.OnCurrentAmmoValueChanged -= HandleGunCurrentAmmoValueChanged;
        }

        healthPoints.OnTakenDamage += HandleHealthPointsTakenDamage;
        healthPoints.OnDied += HandleHealthPointsDied;
    }
}
