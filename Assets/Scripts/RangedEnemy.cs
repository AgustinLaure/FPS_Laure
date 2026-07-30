using System.Collections;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [SerializeField] private float damage;
    [SerializeField] private float shootColdown;
    [SerializeField] private GameObject target;

    //0-100 values
    [SerializeField] private float hitRatio;

    private Coroutine shootTargetCoroutine = null;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        target = player.gameObject;
    }

    protected override void Update()
    {
        base.Update();

        if (perception.GetIsTargetVisible)
        {
            if (shootTargetCoroutine == null)
            {
                shootTargetCoroutine = StartCoroutine(ShootTarget());
            }
        }
    }

    private IEnumerator ShootTarget()
    {
        bool landedHit = Random.Range(1, 101) <= hitRatio;

        if (landedHit)
        {
            HealthPoints targetHealthPoints = target.GetComponent<HealthPoints>();

            if (targetHealthPoints != null)
            {
                targetHealthPoints.TakeDamage(damage);
            }
        }

        yield return new WaitForSeconds(shootColdown);

        shootTargetCoroutine = null;
    }
}
