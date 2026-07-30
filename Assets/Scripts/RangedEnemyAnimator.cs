using System;
using System.Collections.Generic;
using System.Xml;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class RangedEnemyAnimator : MonoBehaviour
{
    [SerializeField] private GameObject rendererObject;
    private Patroller patroller;
    private RangedEnemy rangedEnemy;
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

        rangedEnemy = GetComponent<RangedEnemy>();
        patroller = GetComponent<Patroller>();
    }

    private void Start()
    {
        player = GameObject.FindWithTag(playerTag).GetComponent<Player>();
        playerHealthPoints = player.GetHealthPoints;

        IdleState idleState = new IdleState(this, patroller);
        rangedEnemy.OnShot += idleState.OnShoot;

        RunState runState = new RunState(this, patroller);
        rangedEnemy.OnShot += runState.OnShoot;

        ShootState shootState = new ShootState(this, patroller, animator);
        rangedEnemy.OnShot += shootState.OnShoot;


        states = new Dictionary<Type, IState>()
        {
            [typeof(IdleState)] = idleState,
            [typeof(RunState)] = runState,
            [typeof(ShootState)] = shootState,
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
        rangedEnemy.OnShot -= ((IdleState)auxState).OnShoot;

        states.TryGetValue(typeof(RunState), out auxState);
        rangedEnemy.OnShot -= ((RunState)auxState).OnShoot;

        states.TryGetValue(typeof(ShootState), out auxState);
        rangedEnemy.OnShot -= ((ShootState)auxState).OnShoot;
    }

    private class IdleState : IState
    {
        private RangedEnemyAnimator rangedEnemyAnimator;
        private Patroller patroller;

        public IdleState(RangedEnemyAnimator rangedEnemyAnimator, Patroller patroller)
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

        public void OnShoot()
        {
            rangedEnemyAnimator.fsm.TryChange<IdleState>(typeof(ShootState));
        }
    }

    private class RunState : IState
    {
        private RangedEnemyAnimator rangedEnemyAnimator;
        private Patroller patroller;

        public RunState(RangedEnemyAnimator rangedEnemyAnimator, Patroller patroller)
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

        public void OnShoot()
        {
            rangedEnemyAnimator.fsm.TryChange<RunState>(typeof(ShootState));
        }
    }

    private class ShootState : IState
    {
        private RangedEnemyAnimator rangedEnemyAnimator;
        private Patroller patroller;
        private Animator animator;

        public ShootState(RangedEnemyAnimator rangedEnemyAnimator, Patroller patroller, Animator animator)
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
                    rangedEnemyAnimator.fsm.TryChange<ShootState>(typeof(RunState));
                }
                else
                {
                    rangedEnemyAnimator.fsm.TryChange<ShootState>(typeof(IdleState));
                }
            }
        }

        public void Exit()
        {

        }

        public void OnShoot()
        {
            rangedEnemyAnimator.fsm.TryChange<IdleState>(typeof(ShootState));
        }
    }
}