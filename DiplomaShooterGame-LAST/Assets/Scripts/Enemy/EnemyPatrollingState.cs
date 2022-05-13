using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;

namespace Com.Tereshchuk.Shooter
{
    public class EnemyPatrollingState : AbstractState
    {
        protected static event YieldingEventHandler OnYieldTestEvent;
        private NavMeshAgent _agent;
        private Transform followSkin;
        private float _distance;

        public EnemyPatrollingState(EnemyView enemy, EnemyAnimationSystem animS, NavMeshAgent agent, EnumStates state) : base(enemy, animS , state)
        {
            followSkin = enemy.transform;
            _agent = agent;
            OnYieldTestEvent += Patrolling;
        }

        public override void Init()
        {
            
        }

        public override void Do()
        {         
            AnimSystem.StartWallk();
            Enemy.StartCoroutine(OnYieldTestEvent?.Invoke());
        }

        private IEnumerator Patrolling()
        {
            while (EnemyData.Alive)
            {
                if (AnimSystem.GetAttack() == false)
                {
                    if (Enemy.Target != null)
                    {
                        _distance = Vector3.Distance(Enemy.Target.position, Enemy.transform.position);
                        if (_distance >= EnemyData.LookRadius || Enemy.PlayerIsDied)
                        {
                            _agent.destination = Enemy.EnemyConfig.PatrolPoints[EnemyData.DestPoint].position;
                            RotateTo();

                            if (Enemy.EnemyConfig.PatrolPoints.Count == 0)
                                yield return null;
   
                            if (_agent.remainingDistance < 1f && _agent.pathPending==false)
                            {
                                EnemyData.IncrementDestPoint();

                                AnimSystem.StopWallk();
                        
                                _agent.SetDestination(Enemy.transform.position); // STOPPING NavMeshAgent
                        
                                Enemy.StopCorAll(EnumStates.Rotate);
                            }
                        }
                        else
                        {
                            Enemy.StopCorAll(EnumStates.Chase);
                        }
                    }
                    else
                    {
                        _agent.destination = Enemy.EnemyConfig.PatrolPoints[EnemyData.DestPoint].position;
                        RotateTo();

                        if (Enemy.EnemyConfig.PatrolPoints.Count == 0)
                            yield return null;
                       
                        if (_agent.remainingDistance < 1f && _agent.pathPending==false)
                        {
                            EnemyData.IncrementDestPoint();

                            AnimSystem.StopWallk();
                        
                            _agent.SetDestination(Enemy.transform.position); // STOPPING NavMeshAgent
                        
                            Enemy.StopCorAll(EnumStates.Rotate);
                        }
                        yield return null;
                    }
                    
                }
                yield return null;
            }
            yield return null;
        }
        
        
        // Rotate State or this
        // NEED TO CHANGE NavMeshAgent Rotation with this piece of code
        private void RotateTo()
        {
            _agent.updateRotation = true;
            // Vector3 direction = (Enemy.EnemyConfig.PatrolPoints[EnemyData.DestPoint].position - Enemy.transform.position).normalized;
            // Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            // Enemy.transform.rotation =Quaternion.Slerp(Enemy.transform.rotation, lookRotation, Time.deltaTime * EnemyData.TurnSpeed);
        }


    }
}