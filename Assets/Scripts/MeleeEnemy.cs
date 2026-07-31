using System.Collections;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField] private float damage;
    [SerializeField] private float attackCooldown;
    [SerializeField] private SphereCollider attackRangeArea;
    private EnemyAnimator enemyAnimator;

    private Coroutine attackCoroutine;

    protected override void Awake()
    {
        base.Awake();

        enemyAnimator = GetComponent<EnemyAnimator>();
    }

    protected override void Update()
    {
        base.Update();

        bool isAtRange = Physics.CheckSphere(
                attackRangeArea.transform.TransformPoint(attackRangeArea.center),
                attackRangeArea.radius,
                targetLayerMask,
                QueryTriggerInteraction.Ignore);

        if (isAtRange)
        {
            Attack();

            if (!patroller.GetIsMoving)
            {
                RotateTowardsPlayer(targetTrackSpeed);
            }
        }
    }

    protected override void Attack()
    {
        if (attackCoroutine == null)
        {
            base.Attack();

            if (enemyAnimator.GetIsAttackAnimationPlaying)
            {
                attackCoroutine = StartCoroutine(AttackCoroutine());
            }
        }
    }

    private IEnumerator AttackCoroutine()
    {
        Collider[] colliders = Physics.OverlapSphere(attackRangeArea.transform.TransformPoint(attackRangeArea.center),
               attackRangeArea.radius,
               targetLayerMask,
               QueryTriggerInteraction.Ignore);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag(playerTag) || collider.CompareTag(enemyTag))
            {
                collider.GetComponent<HealthPoints>().TakeDamage(damage);
            }
        }

        yield return new WaitForSeconds(attackCooldown);

        attackCoroutine = null;
    }
}
