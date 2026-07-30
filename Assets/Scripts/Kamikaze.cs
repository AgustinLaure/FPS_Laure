using UnityEngine;

public class Kamikaze : Enemy
{
    public Player SetPlayer { set { player = value; } }

    [SerializeField] private SphereCollider explosionTriggerArea;
    [SerializeField] private float explosionDamage;

    protected override void Awake()
    {
        base.Awake();
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
            Explode();
        }
    }
    private void Explode()
    {
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

        Destroy(gameObject);
    }
}
