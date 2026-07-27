using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    enum GunName
    {
        Rifle,
        Pistol
    }

    [SerializeField] private Transform cameraTransform;

    [SerializeField] private Transform[] gunPresets;
    [SerializeField] private GameObject[] gunPrefabs;

    private List<Gun> guns = new List<Gun>();

    private int currentGunIndex;

    private Coroutine pullGunCoroutine = null;


    private void Awake()
    {
        AddGun(GunName.Rifle, false);
        AddGun(GunName.Pistol, true);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Shoot"))
        {
            guns[currentGunIndex].Shoot(cameraTransform.position, cameraTransform.forward);
        }

        if (Input.GetButtonDown("Reload"))
        {
            guns[currentGunIndex].Reload();
        }
    }

    private void PullGun(GunName gunName)
    {
        if (pullGunCoroutine == null)
        {
            pullGunCoroutine = StartCoroutine(PullGunAnim(gunName));

            guns[(int)gunName].gameObject.SetActive(true);

        }
    }

    private void AddGun(GunName gunName, bool isEquipped = false)
    {
        Vector3 worldPosition = cameraTransform.TransformPoint(gunPresets[(int)gunName].localPosition);

        Quaternion worldRotation = cameraTransform.rotation * gunPresets[(int)gunName].localRotation;

        GameObject gun = Instantiate(gunPrefabs[(int)gunName], worldPosition, worldRotation);

        gun.transform.SetParent(cameraTransform);

        guns.Add(gun.GetComponent<Gun>());

        if (isEquipped)
        {
            PullGun(gunName);
        }

        guns[(int)gunName].gameObject.SetActive(isEquipped);
    }

    private IEnumerator PullGunAnim(GunName gunName)
    {
        yield return new WaitForSeconds(0f);

        //Pull gun animation

        currentGunIndex = (int)gunName;
    }
}
