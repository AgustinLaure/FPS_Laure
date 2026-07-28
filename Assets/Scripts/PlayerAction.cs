using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    enum CurrentTool
    {
        None = -1,
        Rifle,
        Pistol
    }

    [SerializeField] private Transform cameraTransform;

    [SerializeField] private GameObject gunsContainer;

    [SerializeField] private Transform[] gunPresets;
    [SerializeField] private GameObject[] gunPrefabs;

    private List<Gun> guns = new List<Gun>();

    private CurrentTool currentTool;

    private Coroutine swapGunCoroutine = null;
    private Coroutine drawGunCoroutine = null;

    private void Awake()
    {
        AddGun(CurrentTool.Rifle, false);
        AddGun(CurrentTool.Pistol, true);
    }

    private void Update()
    {
        if (currentTool != CurrentTool.None && swapGunCoroutine == null)
        {
            if (Input.GetButtonDown("Shoot"))
            {
                guns[(int)currentTool].Shoot(cameraTransform.position, cameraTransform.forward);
            }

            if (Input.GetButtonDown("Reload"))
            {
                guns[(int)currentTool].Reload();
            }
        }

        if (Input.GetButtonDown("SwapGun"))
        {
            if (swapGunCoroutine == null)
            {
                swapGunCoroutine = StartCoroutine(SwapGun());
            }
        }
    }

    private IEnumerator SwapGun()
    {
        CurrentTool nextGun = (int)currentTool + 1 >= guns.Count ? 0 : currentTool + 1;

        yield return StartCoroutine(SeatheGunCoroutine());

        yield return StartCoroutine(DrawGunCoroutine(nextGun));

        swapGunCoroutine = null;
    }

    private IEnumerator DrawGunCoroutine(CurrentTool gunName)
    {
        guns[(int)gunName].gameObject.SetActive(true);

        yield return new WaitForSeconds(0f);

        //Pull gun animation

        currentTool = gunName;

        drawGunCoroutine = null;
    }

    private IEnumerator SeatheGunCoroutine()
    {
        CurrentTool lastCurrentTool = currentTool;

        yield return new WaitForSeconds(0f);

        //Anim

        guns[(int)currentTool].gameObject.SetActive(false);

        currentTool = CurrentTool.None;
    }

    private void DrawGun(CurrentTool gunName)
    {
        if (drawGunCoroutine == null)
        {
            drawGunCoroutine = StartCoroutine(DrawGunCoroutine(gunName));

            guns[(int)gunName].gameObject.SetActive(true);
        }
    }

    private void AddGun(CurrentTool gunName, bool isEquipped = false)
    {
        Vector3 worldPosition = cameraTransform.TransformPoint(gunPresets[(int)gunName].localPosition);

        Quaternion worldRotation = cameraTransform.rotation * gunPresets[(int)gunName].localRotation;

        GameObject gun = Instantiate(gunPrefabs[(int)gunName], worldPosition, worldRotation, gunsContainer.transform);

        Gun gunComponent = gun.GetComponent<Gun>();
        guns.Add(gunComponent);
        gunComponent.SetLayer(LayerMask.NameToLayer("PlayerGun"), gun.transform);

        if (isEquipped)
        {
            DrawGun(gunName);
        }

        guns[(int)gunName].gameObject.SetActive(isEquipped);
    }
}
