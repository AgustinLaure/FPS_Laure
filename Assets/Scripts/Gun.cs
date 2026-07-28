using UnityEngine;
using System.Collections;

public abstract class Gun : MonoBehaviour
{
    [SerializeField] private ParticleSystem muzzleParticle;
    [SerializeField] private GameObject bulletImpactDecalPrefab;
    [SerializeField] protected float maxAmmo;
    [SerializeField] protected float reloadTime;
    [SerializeField] protected float timePerShot;
    [SerializeField] private float damage;
    [SerializeField] private float shootDistance = 100f;

    [SerializeField] private float recoilDistance;

    [SerializeField] private float recoilKickTime;
    [SerializeField] private float recoilRecoverTime;

    private LayerMask ownerMask;

    public LayerMask SetOwnerMask { set { ownerMask = value; } }

    private float shootCooldown = 0f;

    private float currentAmmo;
    private Coroutine reloadCoroutine = null;
    private Vector3 originalLocalPos;
    private Coroutine recoilCoroutine = null;

    private void Awake()
    {
        currentAmmo = maxAmmo;
    }

    private void Start()
    {
        originalLocalPos = transform.localPosition;
        ownerMask = ~ownerMask;
    }

    private void Update()
    {
        shootCooldown -= Time.deltaTime;

        Debug.Log("current ammo = " + currentAmmo);
    }

    public void SetLayer(int layer, Transform self)
    {
        float childAmount = self.childCount;

        self.gameObject.layer = layer;

        for (int i = 0; i < childAmount; i++)
        {
            Transform child = self.GetChild(i);
            child.gameObject.layer = layer;

            if (child.childCount > 0)
            {
                SetLayer(layer, child);
            }
        }
    }

    public virtual void Shoot(Vector3 shootPos, Vector3 shootDir)
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

            if (recoilCoroutine != null)
            {
                StopCoroutine(recoilCoroutine);
            }

            muzzleParticle.Play();

            recoilCoroutine = StartCoroutine(RecoilCoroutine());

            currentAmmo--;
            shootCooldown = timePerShot;
        }
    }

    public virtual void Reload()
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

    private IEnumerator RecoilCoroutine()
    {
        Vector3 kickDir = Vector3.Normalize(-Vector3.forward + (Vector3.right / 2f));
        Vector3 startingPos = transform.localPosition;
        Vector3 endingPos = originalLocalPos + kickDir * recoilDistance;

        float kickDistance = Vector3.Magnitude(endingPos - startingPos);

        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime / recoilKickTime;

            transform.localPosition = Vector3.Lerp(startingPos, endingPos, elapsedTime);

            yield return null;
        }

        elapsedTime = 0f;

        endingPos = transform.localPosition;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime / recoilRecoverTime;

            transform.localPosition = Vector3.Lerp(endingPos, originalLocalPos, elapsedTime);

            yield return null;
        }
    }
}
