using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Com.Tereshchuk.Shooter
{
    public class EnemyChaseState : AbstractState
    {
        protected static event YieldingEventHandler OnYieldTestEvent;
        private event Action ShowHpBar;
        private event Action HideHpBar;
        private int _destPoint = 0;
        private NavMeshAgent _agent;
        private float _distance;
        
        public EnemyChaseState(EnemyView enemy, EnemyAnimationSystem animS, NavMeshAgent agent,EnemyHpBar hpBar,EnumStates state) : base(enemy, animS, state)
        {
            _agent = agent;
            OnYieldTestEvent += Chase;
            ShowHpBar += hpBar.ShowBar;
            HideHpBar += hpBar.HideBar;
        }
        
        public override void Init()
        {
        }

        public override void Do()
        {
            _agent.updateRotation = true;
            AnimSystem.StartWallk();
            ShowHpBar?.Invoke();
            Enemy.StartCoroutine(OnYieldTestEvent?.Invoke());
        }

        private IEnumerator Chase()
        {
            while (Enemy.PlayerIsDied==false && Enemy.Target)
            {
                _distance = Vector3.Distance(Enemy.Target.position, Enemy.transform.position);
                if (_distance <= EnemyData.LookRadius)
                {
                    if (_agent.speed < 2f)
                        _agent.speed += (_agent.speed + 0.5f)*Time.deltaTime;                 
                    if (_distance <=_agent.stoppingDistance)
                    {
                        _agent.speed = 1f;
                        RotateToPlayer();
                        _agent.SetDestination(Enemy.transform.position); // STOPPING NavMeshAgent
                        AnimSystem.StopWallk();
                        Enemy.StopCorAll(EnumStates.Attack);
                    }
                    else
                    {
                        _agent.SetDestination(Enemy.Target.position);
                    }
                }else
                {
                    HideHpBar?.Invoke();
                    Enemy.Target = null;
                    _agent.speed = 1f;
                    Enemy.StopCorAll(EnumStates.Patrolling);
                }
                
                yield return null;
            }
            HideHpBar?.Invoke();
            Enemy.StopCorAll(EnumStates.Patrolling);
        }
        
        // NEED TO FIX WHEN PLAYER ISNT AT Y AXIS WITH ENEMY
        private void RotateToPlayer()
        {
    
            Vector3 direction = (Enemy.Target.position - Enemy.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            Enemy.transform.rotation = Quaternion.Slerp(Enemy.transform.rotation, lookRotation, Time.deltaTime * EnemyData.TurnSpeed);           
        }
    }
}