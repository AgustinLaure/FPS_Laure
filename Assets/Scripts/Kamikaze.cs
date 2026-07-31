using System.Collections;
using UnityEngine;

public class Kamikaze : Enemy
{
    public Player SetPlayer { set { player = value; } }

    private SphereCollider collider;

    [SerializeField] private SphereCollider explosionTriggerArea;
    [SerializeField] private SphereCollider explosionRadiusArea;
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] private AudioSource alertSound;
    [SerializeField] private AudioSource explosionSound;
    [SerializeField] private float explosionDamage;
    [SerializeField] private float timeToExplode = 2f;

    private Coroutine bombTimerCoroutine = null;

    protected override void Awake()
    {
        base.Awake();

        collider = GetComponent<SphereCollider>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        bool isExplosionTrigger = Physics.CheckSphere(
                explosionTriggerArea.transform.TransformPoint(explosionTriggerArea.center),
                explosionTriggerArea.radius,
                targetLayerMask,
                QueryTriggerInteraction.Ignore);

        if (isExplosionTrigger)
        {
            Attack();
        }
    }

    protected override void Attack()
    {
        if (bombTimerCoroutine == null)
        {
            base.Attack();

            patroller.enabled = false;
            chaser.enabled = false;
            healthPoints.enabled = false;
            perception.enabled = false;
            animator.enabled = false;
            collider.enabled = false;

            bombTimerCoroutine = StartCoroutine(BombTimerCoroutine());
        }
    }

    private IEnumerator BombTimerCoroutine()
    {
        alertSound.Play();

        yield return new WaitForSeconds(timeToExplode);

        alertSound.Stop();

        Explode();

        bool hasEndedParticleAnimation = false;

        while (!hasEndedParticleAnimation)
        {
            hasEndedParticleAnimation = !explosionParticles.isPlaying;

            yield return new WaitForSeconds(1f);
        }

        Destroy(gameObject);
    }

    private void Explode()
    {
        explosionParticles.Play();

        Collider[] colliders = Physics.OverlapSphere(explosionTriggerArea.transform.TransformPoint(explosionTriggerArea.center),
                explosionTriggerArea.radius,
                targetLayerMask,
                QueryTriggerInteraction.Ignore);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag(playerTag) || collider.CompareTag(enemyTag))
            {
                collider.GetComponent<HealthPoints>().TakeDamage(explosionDamage);
            }
        }

        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        GameObject tempSound = new GameObject("ExplosionSound");

        AudioSource tempAudioSource = tempSound.AddComponent<AudioSource>();

        tempAudioSource.clip = explosionSound.clip;
        tempAudioSource.spatialBlend = 0f;
        tempAudioSource.volume = 1f;

        tempAudioSource.Play();

        Destroy(tempSound,tempAudioSource.clip.length);
    }
}
