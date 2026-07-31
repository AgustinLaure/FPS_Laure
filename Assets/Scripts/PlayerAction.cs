using System;
using System.Collections;
using UnityEngine;


public class PlayerAction : MonoBehaviour
{
    public event Action OnPause;

    [SerializeField] private Transform cameraTransform;
    private Player player;

    private Coroutine swapGunCoroutine = null;

    private void Awake()
    {
        player = gameObject.GetComponent<Player>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            OnPause?.Invoke();
        }

        if (Time.timeScale > 0f)
        {
            if (player.GetCurrentTool != CURRENT_TOOL.None && swapGunCoroutine == null)
            {
                if (Input.GetButtonDown("Shoot"))
                {
                    player.GetGun.Shoot(cameraTransform.position, cameraTransform.forward);
                }

                if (Input.GetButtonDown("Reload"))
                {
                    player.GetGun.Reload();
                }
            }

            if (Input.GetButtonDown("DrawPistol"))
            {
                TrySwapTool(CURRENT_TOOL.Pistol);
            }

            if (Input.GetButtonDown("DrawRifle"))
            {
                TrySwapTool(CURRENT_TOOL.Rifle);
            }

            if (Input.GetButtonDown("SwapHands"))
            {
                TrySwapTool(CURRENT_TOOL.None);
            }
        }
    }

    private void TrySwapTool(CURRENT_TOOL gun)
    {
        bool canSwap = true;

        if (swapGunCoroutine == null)
        {
            if (player.GetCurrentTool != CURRENT_TOOL.None)
            {
                if (player.GetGun.GetIsReloading)
                {
                    canSwap = false;
                }
            }

            if (canSwap)
            {
                swapGunCoroutine = StartCoroutine(SwapGunCoroutine(gun));
            }
        }
    }

    private IEnumerator SwapGunCoroutine(CURRENT_TOOL gun)
    {
        if (player.GetCurrentTool != CURRENT_TOOL.None)
        {
            yield return StartCoroutine(SeatheGunCoroutine());
        }

        if (gun != CURRENT_TOOL.None)
        {
            yield return StartCoroutine(DrawGunCoroutine(gun));
        }
        else
        {
            player.SetCurrentTool = CURRENT_TOOL.None;
        }

        swapGunCoroutine = null;
    }

    private IEnumerator DrawGunCoroutine(CURRENT_TOOL gunName)
    {
        player.GetGuns[(int)gunName].gameObject.SetActive(true);
        player.SetCurrentTool = gunName;

        yield return new WaitForSeconds(0f);

        //Pull gun animation
    }

    private IEnumerator SeatheGunCoroutine()
    {
        CURRENT_TOOL lastCurrentTool = player.GetCurrentTool;

        yield return new WaitForSeconds(0f);

        //Anim

        player.GetGun.gameObject.SetActive(false);
    }
}
