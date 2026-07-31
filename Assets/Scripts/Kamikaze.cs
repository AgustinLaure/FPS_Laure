using System.Collections;
using UnityEngine;

public class Kamikaze : Enemy
{
    public Player SetPlayer { set { player = value; } }

    [SerializeField] private SphereCollider explosionTriggerArea;
    [SerializeField] private float explosionDamage;
    [SerializeField] private float timeToExplode = 2f;

    private Coroutine bombTimerCoroutine = null;

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
            Attack();
        }
    }

    protected override void Attack()
    {
        base.Attack();

        patroller.enabled = false;
        chaser.enabled = false;
        healthPoints.enabled = false;
        perception.enabled = false;
        animator.enabled = false;

        if (bombTimerCoroutine == null)
        {
            bombTimerCoroutine = StartCoroutine(BombTimerCoroutine());
        }
    }

    private IEnumerator BombTimerCoroutine()
    {
        yield return new WaitForSeconds(timeToExplode);

        Explode();
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
