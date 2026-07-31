using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private enum ACTION_STATE
    {
        Idle,
        Dead,
        Patrolling,
        Chasing
    }

    private enum ENEMY_TYPE
    {
        Patroller,
        Camper
    }

    public event Action OnAttacked;

    [SerializeField] protected Player player;
    protected const string playerTag = "Player";
    protected const string enemyTag = "Enemy";

    protected Patroller patroller;
    private Chaser chaser;
    protected Perception perception;
    protected HealthPoints healthPoints;

    [SerializeField] protected LayerMask targetLayerMask;

    private FSM fsm;
    private Dictionary<Type, IState> states;

    [SerializeField] private ACTION_STATE currentActionState = ACTION_STATE.Patrolling;
    [SerializeField] private ENEMY_TYPE enemyType;
    [SerializeField] protected float targetTrackSpeed;

    public Player GetPlayer { get { return player; } }

    protected virtual void Awake()
    {
        healthPoints = GetComponent<HealthPoints>();

        healthPoints.OnDied += HandleDie;
        healthPoints.OnTakenDamage += HandleTakeDamage;
    }

    protected virtual void Start()
    {
        perception = gameObject.GetComponent<Perception>();
        patroller = gameObject.GetComponent<Patroller>();
        chaser = gameObject.GetComponent<Chaser>();

        IdleState idleState = new IdleState(this, currentActionState, perception);
        healthPoints.OnDied += idleState.OnDie;

        DeadState deadState = new DeadState();

        PatrolState patrolState = new PatrolState(this, patroller, currentActionState, perception, enemyType);
        healthPoints.OnDied += patrolState.OnDie;

        ChaseState chaseState = new ChaseState(this, chaser, currentActionState, perception);
        healthPoints.OnDied += chaseState.OnDie;

        states = new Dictionary<Type, IState>()
        {
            [typeof(IdleState)] = idleState,
            [typeof(DeadState)] = deadState,
            [typeof(PatrolState)] = patrolState,
            [typeof(ChaseState)] = chaseState,
        };

        fsm = new FSM(states);

        switch (enemyType)
        {
            case ENEMY_TYPE.Patroller:
                fsm.SetInitialState(typeof(PatrolState));
                break;
            case ENEMY_TYPE.Camper:
                fsm.SetInitialState(typeof(IdleState));
                break;

            default:
                break;
        }

        player = GameObject.FindWithTag(playerTag).GetComponent<Player>();
    }
    protected virtual void Update()
    {
        fsm.Update();
    }

    protected virtual void Attack()
    {
        OnAttacked?.Invoke();
    }

    protected void RotateTowardsPlayer(float atSpeed)
    {
        Vector3 direction = player.transform.position - transform.position;

        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, atSpeed * Time.deltaTime);
    }

    private void HandleTakeDamage()
    {
        Debug.Log("ouch");
    }
    private void HandleDie()
    {
        Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
        IState auxState;

        states.TryGetValue(typeof(IdleState), out auxState);
        healthPoints.OnDied -= ((IdleState)auxState).OnDie;

        states.TryGetValue(typeof(PatrolState), out auxState);
        healthPoints.OnDied -= ((PatrolState)auxState).OnDie;

        states.TryGetValue(typeof(ChaseState), out auxState);
        healthPoints.OnDied -= ((ChaseState)auxState).OnDie;

        healthPoints.OnDied -= HandleDie;
        healthPoints.OnTakenDamage -= HandleTakeDamage;
    }

    private class IdleState : IState
    {
        private Enemy enemy;
        private Perception perception;
        private ACTION_STATE currentActionState;

        public IdleState(Enemy enemy, ACTION_STATE currentActionState, Perception perception)
        {
            this.enemy = enemy;
            this.currentActionState = currentActionState;
            this.perception = perception;
        }

        public void Enter()
        {
            currentActionState = ACTION_STATE.Chasing;
        }

        public void Update()
        {
            if (perception.GetIsTargetVisible)
            {
                enemy.fsm.TryChange<IdleState>(typeof(ChaseState));
            }
        }

        public void Exit()
        {
            currentActionState = ACTION_STATE.Idle;
        }

        public void OnDie()
        {
            enemy.fsm.TryChange<ChaseState>(typeof(DeadState));
        }
    }

    private class DeadState : IState
    {
        private Enemy enemy;

        public DeadState()
        {

        }

        public void Enter()
        {

        }

        public void Update()
        {

        }

        public void Exit()
        {

        }
    }

    private class PatrolState : IState
    {
        private Enemy enemy;
        private Patroller patroller;
        private Perception perception;
        private ACTION_STATE currentActionState;
        private ENEMY_TYPE enemyType;

        public PatrolState(Enemy enemy, Patroller patroller, ACTION_STATE currentActionState, Perception perception, ENEMY_TYPE enemyType)
        {
            this.enemy = enemy;
            this.patroller = patroller;
            this.currentActionState = currentActionState;
            this.perception = perception;
            this.enemyType = enemyType;
        }

        public void Enter()
        {
            currentActionState = ACTION_STATE.Patrolling;
            patroller.StartPatrolling();
        }

        public void Update()
        {
            if (perception.GetIsTargetVisible)
            {
                enemy.fsm.TryChange<PatrolState>(typeof(ChaseState));
            }

            if (enemyType == ENEMY_TYPE.Camper && patroller.GetHasReachedDestination)
            {
                enemy.fsm.TryChange<PatrolState>(typeof(IdleState));
            }
        }

        public void Exit()
        {
            patroller.StopPatrolling();
        }

        public void OnDie()
        {
            enemy.fsm.TryChange<PatrolState>(typeof(DeadState));
        }
    }

    private class ChaseState : IState
    {
        private Enemy enemy;
        private Chaser chaser;
        private Perception perception;
        private ACTION_STATE currentActionState;
        private float timeSinceLastSawPlayer = 0f;

        private const float maxTimeSinceLastSawPlayer = 5f;

        public ChaseState(Enemy enemy, Chaser chaser, ACTION_STATE currentActionState, Perception perception)
        {
            this.enemy = enemy;
            this.chaser = chaser;
            this.currentActionState = currentActionState;
            this.perception = perception;
        }

        public void Enter()
        {
            currentActionState = ACTION_STATE.Chasing;
            chaser.StartChase();
        }

        public void Update()
        {
            if (perception.GetIsTargetVisible)
            {
                currentActionState = ACTION_STATE.Chasing;
                chaser.StartChase();

                timeSinceLastSawPlayer = 0f;
            }
            else
            {
                timeSinceLastSawPlayer += Time.deltaTime;
            }

            if (timeSinceLastSawPlayer >= maxTimeSinceLastSawPlayer)
            {
                enemy.fsm.TryChange<ChaseState>(typeof(PatrolState));
            }
        }

        public void Exit()
        {
            chaser.StopChase();

            timeSinceLastSawPlayer = 0f;
        }

        public void OnDie()
        {
            enemy.fsm.TryChange<ChaseState>(typeof(DeadState));
        }
    }
}
