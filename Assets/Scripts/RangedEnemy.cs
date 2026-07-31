using System;
using System.Collections;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [SerializeField] private float damage;
    [SerializeField] private float shootColdown;
    [SerializeField] private GameObject target;
    [SerializeField] private ParticleSystem shootParticle;
    [SerializeField] private float rotationSpeed;
    private Patroller patroller;

    //0-100 values
    [SerializeField] private float hitRatio;

    private Coroutine shootTargetCoroutine = null;

    protected override void Awake()
    {
        base.Awake();

        patroller = GetComponent<Patroller>();
    }

    protected override void Start()
    {
        base.Start();

        target = player.gameObject;
    }

    protected override void Update()
    {
        base.Update();

        if (perception.GetIsTargetVisible && !patroller.GetIsMoving)
        {
            Vector3 direction = player.transform.position - transform.position;

            direction.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            Attack();
        }
    }

    protected override void Attack()
    {
        base.Attack();

        if (shootTargetCoroutine == null)
        {
            shootTargetCoroutine = StartCoroutine(ShootTarget());
        }
    }

    private IEnumerator ShootTarget()
    {
        shootParticle.Play();

        bool landedHit = UnityEngine.Random.Range(1, 101) <= hitRatio;

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
