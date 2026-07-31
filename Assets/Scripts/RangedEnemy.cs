using System;
using System.Collections;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [SerializeField] private float damage;
    [SerializeField] private float shootColdown;
    [SerializeField] private GameObject target;
    [SerializeField] private ParticleSystem shootParticle;
    [SerializeField] private float alertedRotationSpeed;
    private bool isAlerted = false;

    //0-100 values
    [SerializeField] private float hitRatio;

    private Coroutine shootTargetCoroutine = null;

    protected override void Awake()
    {
        base.Awake();

        healthPoints.OnTakenDamage += HandleTakeDamage;

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
            RotateTowardsPlayer(targetTrackSpeed);
            Attack();
        }

        if (isAlerted)
        {
            if (!perception.GetIsTargetVisible)
            {
                RotateTowardsPlayer(alertedRotationSpeed);
            }
            else
            {
                isAlerted = false;
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        healthPoints.OnTakenDamage -= HandleTakeDamage;
    }

    protected override void Attack()
    {
        if (shootTargetCoroutine == null)
        {
            base.Attack();

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

    private void HandleTakeDamage()
    {
        isAlerted = true;
    }
}
