using System.Collections;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    [SerializeField] private float damage;
    [SerializeField] private float attackCooldown;
    [SerializeField] private SphereCollider attackRangeArea;

    private Coroutine attackCoroutine;

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
        }
    }

    protected override void Attack()
    {
        base.Attack();

        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(AttackCoroutine());
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
