using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private GameObject rendererObject;
    private Patroller patroller;
    private Enemy enemy;
    private Player player;
    private HealthPoints playerHealthPoints;
    protected const string playerTag = "Player";

    private readonly string shootStateName = "Shoot";
    private int shootAnimHash;

    private Animator animator;

    private FSM fsm;

    private Dictionary<Type, IState> states;

    private int controllerStateHash;
    private readonly string controllerStateVarName = "State";

    private void Awake()
    {
        shootAnimHash = Animator.StringToHash(shootStateName);

        animator = rendererObject.GetComponent<Animator>();
        controllerStateHash = Animator.StringToHash(controllerStateVarName);

        enemy = GetComponent<Enemy>();
        patroller = GetComponent<Patroller>();
    }

    private void Start()
    {
        player = GameObject.FindWithTag(playerTag).GetComponent<Player>();
        playerHealthPoints = player.GetHealthPoints;

        IdleState idleState = new IdleState(this, patroller);
        enemy.OnAttacked += idleState.OnAttack;

        RunState runState = new RunState(this, patroller);
        enemy.OnAttacked += runState.OnAttack;

        AttackState attackState = new AttackState(this, patroller, animator);
        enemy.OnAttacked += attackState.OnAttack;


        states = new Dictionary<Type, IState>()
        {
            [typeof(IdleState)] = idleState,
            [typeof(RunState)] = runState,
            [typeof(AttackState)] = attackState,
        };

        fsm = new FSM(states);
        fsm.SetInitialState(typeof(RunState));
    }

    private void Update()
    {
        fsm.Update();
    }

    private void OnDestroy()
    {
        IState auxState;

        states.TryGetValue(typeof(IdleState), out auxState);
        enemy.OnAttacked -= ((IdleState)auxState).OnAttack;

        states.TryGetValue(typeof(RunState), out auxState);
        enemy.OnAttacked -= ((RunState)auxState).OnAttack;

        states.TryGetValue(typeof(AttackState), out auxState);
        enemy.OnAttacked -= ((AttackState)auxState).OnAttack;
    }

    private class IdleState : IState
    {
        private EnemyAnimator rangedEnemyAnimator;
        private Patroller patroller;

        public IdleState(EnemyAnimator rangedEnemyAnimator, Patroller patroller)
        {
            this.rangedEnemyAnimator = rangedEnemyAnimator;
            this.patroller = patroller;
        }
        public void Enter()
        {
            rangedEnemyAnimator.animator.SetInteger(rangedEnemyAnimator.controllerStateHash, 0);
        }

        public void Update()
        {
            if (patroller.GetIsMoving)
            {
                rangedEnemyAnimator.fsm.TryChange<IdleState>(typeof(RunState));
            }
        }

        public void Exit()
        {

        }

        public void OnAttack()
        {
            rangedEnemyAnimator.fsm.TryChange<IdleState>(typeof(AttackState));
        }
    }

    private class RunState : IState
    {
        private EnemyAnimator rangedEnemyAnimator;
        private Patroller patroller;

        public RunState(EnemyAnimator rangedEnemyAnimator, Patroller patroller)
        {
            this.rangedEnemyAnimator = rangedEnemyAnimator;
            this.patroller = patroller;
        }
        public void Enter()
        {
            rangedEnemyAnimator.animator.SetInteger(rangedEnemyAnimator.controllerStateHash, 1);
        }

        public void Update()
        {
            if (!patroller.GetIsMoving)
            {
                rangedEnemyAnimator.fsm.TryChange<RunState>(typeof(IdleState));
            }
        }

        public void Exit()
        {

        }

        public void OnAttack()
        {
            rangedEnemyAnimator.fsm.TryChange<RunState>(typeof(AttackState));
        }
    }

    private class AttackState : IState
    {
        private EnemyAnimator rangedEnemyAnimator;
        private Patroller patroller;
        private Animator animator;

        public AttackState(EnemyAnimator rangedEnemyAnimator, Patroller patroller, Animator animator)
        {
            this.rangedEnemyAnimator = rangedEnemyAnimator;
            this.patroller = patroller;
            this.animator = animator;
        }
        public void Enter()
        {
            rangedEnemyAnimator.animator.SetInteger(rangedEnemyAnimator.controllerStateHash, 2);
        }

        public void Update()
        {
            if (UnityUtils.CurrentAnimationEnded(rangedEnemyAnimator.shootAnimHash, animator))
            {
                if (patroller.GetIsMoving)
                {
                    rangedEnemyAnimator.fsm.TryChange<AttackState>(typeof(RunState));
                }
                else
                {
                    rangedEnemyAnimator.fsm.TryChange<AttackState>(typeof(IdleState));
                }
            }
        }

        public void Exit()
        {

        }

        public void OnAttack()
        {
            rangedEnemyAnimator.fsm.TryChange<IdleState>(typeof(AttackState));
        }
    }
}