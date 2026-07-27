using UnityEngine;
using System.Collections;

public abstract class Gun : MonoBehaviour
{
    [SerializeField] private GameObject bulletImpactDecalPrefab;
    [SerializeField] protected float maxAmmo;
    [SerializeField] protected float reloadTime;
    [SerializeField] protected float timePerShot;
    [SerializeField] private float damage;
    [SerializeField] private float shootDistance = 100f;

    private LayerMask ownerMask;

    public LayerMask SetOwnerMask { set { ownerMask = value; } }

    private float shootCooldown = 0f;

    private float currentAmmo;
    private Coroutine reloadCoroutine = null;

    private void Awake()
    {
        ownerMask = ~ownerMask;
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        shootCooldown -= Time.deltaTime;

        Debug.Log("current ammo = " + currentAmmo);
    }

    public void Shoot(Vector3 shootPos, Vector3 shootDir)
    {
        if (currentAmmo > 0f && shootCooldown <= 0f && reloadCoroutine == null)
        {
            if (Physics.Raycast(shootPos, shootDir, out RaycastHit hit, shootDistance, ownerMask))
            {
                if (hit.transform.CompareTag("Environment"))
                {
                    Quaternion decalOrientation = Quaternion.LookRotation(-hit.normal);

                    GameObject decal = Instantiate(bulletImpactDecalPrefab, hit.point, decalOrientation);

                    decal.transform.SetParent(hit.transform);
                }
            }

            currentAmmo--;
            shootCooldown = timePerShot;
        }
    }

    public void Reload()
    {
        if (reloadCoroutine == null && currentAmmo < maxAmmo)
        {
            reloadCoroutine = StartCoroutine(ReloadCoroutine());
        }
    }

    private IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadTime);

        Debug.Log("empezo a recargar");

        currentAmmo = maxAmmo;

        reloadCoroutine = null;

        Debug.Log("termino de recargar");
    }
}
