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

    private readonly string attackStateName = "Attack";
    private int attackAnimHash;

    private Animator animator;

    private FSM fsm;

    private Dictionary<Type, IState> states;

    private int controllerStateHash;
    private readonly string controllerStateVarName = "State";

    public int GetAnimationPlaying { get { return animator.GetInteger(controllerStateHash); } }

    public bool GetIsAttackAnimationPlaying { get { return animator.GetCurrentAnimatorStateInfo(0).shortNameHash == attackAnimHash; } }


    private void Awake()
    {
        attackAnimHash = Animator.StringToHash(attackStateName);

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
        private EnemyAnimator enemyAnimator;
        private Patroller patroller;

        public IdleState(EnemyAnimator enemyAnimator, Patroller patroller)
        {
            this.enemyAnimator = enemyAnimator;
            this.patroller = patroller;
        }
        public void Enter()
        {
            enemyAnimator.animator.SetInteger(enemyAnimator.controllerStateHash, 0);
        }

        public void Update()
        {
            if (patroller.GetIsMoving)
            {
                enemyAnimator.fsm.TryChange<IdleState>(typeof(RunState));
            }
        }

        public void Exit()
        {

        }

        public void OnAttack()
        {
            enemyAnimator.fsm.TryChange<IdleState>(typeof(AttackState));
        }
    }

    private class RunState : IState
    {
        private EnemyAnimator enemyAnimator;
        private Patroller patroller;

        public RunState(EnemyAnimator enemyAnimator, Patroller patroller)
        {
            this.enemyAnimator = enemyAnimator;
            this.patroller = patroller;
        }
        public void Enter()
        {
            enemyAnimator.animator.SetInteger(enemyAnimator.controllerStateHash, 1);
        }

        public void Update()
        {
            if (!patroller.GetIsMoving)
            {
                enemyAnimator.fsm.TryChange<RunState>(typeof(IdleState));
            }
        }

        public void Exit()
        {

        }

        public void OnAttack()
        {
            enemyAnimator.fsm.TryChange<RunState>(typeof(AttackState));
        }
    }

    private class AttackState : IState
    {
        private EnemyAnimator enemyAnimator;
        private Patroller patroller;
        private Animator animator;
        
        public AttackState(EnemyAnimator enemyAnimator, Patroller patroller, Animator animator)
        {
            this.enemyAnimator = enemyAnimator;
            this.patroller = patroller;
            this.animator = animator;
        }
        public void Enter()
        {
            enemyAnimator.animator.SetInteger(enemyAnimator.controllerStateHash, 2);
        
            animator.Play(enemyAnimator.attackAnimHash, 0, 0f);
        }
        
        public void Update()
        {
            if (UnityUtils.CurrentAnimationEnded(enemyAnimator.attackAnimHash, animator))
            {
                if (patroller.GetIsMoving)
                {
                    enemyAnimator.fsm.TryChange<AttackState>(typeof(RunState));
                }
                else
                {
                    enemyAnimator.fsm.TryChange<AttackState>(typeof(IdleState));
                }
            }
        }
        
        public void Exit()
        {
        
        }
        
        public void OnAttack()
        {
            enemyAnimator.fsm.TryChange<AttackState>(typeof(AttackState));
        }
    }
}