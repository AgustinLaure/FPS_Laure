using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public abstract class Gun : MonoBehaviour
{
    public event Action<int> OnCurrentAmmoValueChanged;

    [SerializeField] private ParticleSystem muzzleParticle;
    [SerializeField] private GameObject bulletImpactDecalPrefab;
    [SerializeField] private AudioSource emptyGunSound;
    [SerializeField] private AudioSource shotSound;
    [SerializeField] private AudioSource reloadSound;
    [SerializeField] private int maxAmmo;
    [SerializeField] private float reloadTime;
    [SerializeField] private float timePerShot;
    [SerializeField] private float damage;
    [SerializeField] private float shootDistance = 100f;

    private string targetTag;

    [SerializeField] private float recoilDistance;

    [SerializeField] private float recoilKickTime;
    [SerializeField] private float recoilRecoverTime;

    private LayerMask ownerMask;

    public LayerMask SetOwnerMask
    {
        set
        {
            ownerMask = ~(value | LayerMask.GetMask("NonHitteable"));
        }
    }

    public string SetTargetTag { set { targetTag = value; } }

    public float GetCurrentAmmo { get { return currentAmmo; } }

    public float GetMaxAmmo { get { return maxAmmo; } }

    public bool GetIsReloading { get { return reloadCoroutine != null; } }


    private float shootCooldown = 0f;

    private int currentAmmo;
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
    }

    private void Update()
    {
        shootCooldown -= Time.deltaTime;
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
        if (currentAmmo > 0f)
        {
            if (shootCooldown <= 0f)
            {
                if (reloadCoroutine == null)
                {
                    shotSound.Play();

                    if (Physics.Raycast(shootPos, shootDir, out RaycastHit hit, shootDistance, ownerMask, QueryTriggerInteraction.Ignore))
                    {
                        Debug.Log(hit.transform.position);

                        if (hit.transform.CompareTag("Environment"))
                        {
                            Quaternion decalOrientation = Quaternion.LookRotation(-hit.normal);

                            GameObject decal = Instantiate(bulletImpactDecalPrefab, hit.point, decalOrientation);

                            decal.transform.SetParent(hit.transform);
                        }
                        if (hit.transform.CompareTag(targetTag))
                        {
                            HealthPoints targetHealthPoints = hit.transform.gameObject.GetComponent<HealthPoints>();

                            if (targetHealthPoints != null)
                            {
                                targetHealthPoints.TakeDamage(damage);
                            }
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

                    OnCurrentAmmoValueChanged?.Invoke(currentAmmo);
                }
            }

        }
        else
        {
            emptyGunSound.Play();
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

        currentAmmo = maxAmmo;

        reloadCoroutine = null;

        OnCurrentAmmoValueChanged?.Invoke(currentAmmo);

        reloadSound.Play();
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
