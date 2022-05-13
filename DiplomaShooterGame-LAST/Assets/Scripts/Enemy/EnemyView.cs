using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


namespace Com.Tereshchuk.Shooter
{
    public class EnemyView : MonoBehaviour
    {
        public EnemyController Controller;
        public bool PlayerIsDied { get; private set; }
        public EnemyConfig EnemyConfig { get; private set; }
        public EnemyData EnemyData{ get; private set; }
        private Transform _target;
        public Transform Target
        {
            get => _target;
            set => _target = value;
        }

        [SerializeField] private EnemyAnimationSystem animSystem;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private EnemyWeaponDmgDealingSystem weaponSystem;
        [SerializeField] private EnemyHealthSystem healthSystem;

        private Dictionary<EnumStates, AbstractState> _allStates= new Dictionary<EnumStates, AbstractState>();
        private AbstractState _currentState;
        private EnemyHpBar _hpBar;
        private void Awake()
        {
            _hpBar=GetComponentInChildren<EnemyHpBar>();
            agent.updateRotation = false;
        }
        
        // GET RID OF MONOBEH ( new() ? )AT ANIMS AND CREATE VARIABLES
        // ADD DRAWING HEALTH BAR FUNC 
        // FIND BETTER WAY TO TAKE HIT (RigDoll)

        public void SetTarget(PlayerController playerController)
        {
            _hpBar.Initialize(playerController.mainCamera);
            Target = playerController.transform;
        }

        private void RemoveTarget()
        {
            Target = null;
        }
        public void TakeDamage(float damage)
        {
            if (_currentState.Name == EnumStates.Attack)
            {
                animSystem.ResetAttack();
            }
            if (EnemyData.Health > 0)
            {
                healthSystem.ReduceHealth(damage);
                SetCurrentState(EnumStates.RecieveDmg);
            }
        }
        public EnumStates GetCurrentState()
        {
            return _currentState.Name;
        }
        public void Init(EnemyConfig enemyConfig, EnemyData enemyData,EnemyController controller)
        {
            Controller = controller;
            PlayerIsDied = false;
            EnemyConfig = enemyConfig;
            EnemyData = enemyData;
            
            AddAllStates();
            healthSystem.Initialize(EnemyData);
            healthSystem.OnDied += SetEnemyDeath;
            weaponSystem.Initialize(this);
            weaponSystem.IsDied += SetPlayerDeath;
            controller.OnTakeDamage += TakeDamage;
            healthSystem.OnHealthChange += _hpBar.FillHealthBar;
            SetCurrentState(EnumStates.Patrolling);
        }

        private void SetCurrentState(EnumStates state)
        {
            _allStates.TryGetValue(state, out _currentState);
            _currentState?.Do();
        }
        public void StopCorAll(EnumStates state)
        {
            StopAllCoroutines();
            SetCurrentState(state);
        }
        private void AddAllStates()
        {
            _allStates.Add(EnumStates.Idle, new EnemyIdleState(this,animSystem,EnumStates.Idle));
            _allStates.Add(EnumStates.Patrolling, new EnemyPatrollingState(this,animSystem,agent,EnumStates.Patrolling));
            _allStates.Add(EnumStates.Chase, new EnemyChaseState(this,animSystem,agent,_hpBar,EnumStates.Chase));
            _allStates.Add(EnumStates.Rotate, new EnemyRotateState(this, animSystem,agent,EnumStates.Rotate));
            _allStates.Add(EnumStates.Attack, new EnemyAttackingState(this, animSystem, agent,weaponSystem,EnumStates.Attack));
            _allStates.Add(EnumStates.RecieveDmg, new EnemyRecieveDmgState(this, animSystem,EnumStates.RecieveDmg,healthSystem));
        }

        private void SetEnemyDeath()
        {
            StopAllCoroutines();
            animSystem.SetDeath();
        }
        private void SetPlayerDeath()
        {
            PlayerIsDied = true;
        }

    }
}