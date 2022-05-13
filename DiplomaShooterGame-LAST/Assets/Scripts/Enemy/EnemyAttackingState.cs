
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Com.Tereshchuk.Shooter
{
    public class EnemyAttackingState : AbstractState
    {
        protected static event YieldingEventHandler OnYieldTestEvent;
        
        private NavMeshAgent _agent;
        private float _distance;
        private EnemyWeaponDmgDealingSystem _weaponSys;
        public EnemyAttackingState(EnemyView enemy, EnemyAnimationSystem animS,NavMeshAgent agent,EnemyWeaponDmgDealingSystem WeaponSys , EnumStates state) : base(enemy, animS,state)
        {
            _agent = agent;
            OnYieldTestEvent += Hit;
            _weaponSys = WeaponSys;
        }

        public override void Init()
        {
            
        }

        public override void Do()
        {
            AnimSystem.StartAttack(); 
            Enemy.StartCoroutine(OnYieldTestEvent?.Invoke());
        }

        public IEnumerator Hit()
        {
            while (Enemy.PlayerIsDied==false && Enemy.Target)
            {
                RotateToPlayer();
                _distance = Vector3.Distance(Enemy.Target.position, Enemy.transform.position);
                Debug.Log(_distance+" $$$$");
                if (AnimSystem.GetAttack() == false && _distance <= _agent.stoppingDistance)
                {
                    yield return new WaitForSeconds(2);
                    AnimSystem.StartAttack();
                    _weaponSys.canAttack = true;
                }else if (_distance > _agent.stoppingDistance && AnimSystem.GetAttack() == false)
                {
                    Enemy.StopCorAll(EnumStates.Chase);
                }

                yield return null;
            }
            Enemy.StopCorAll(EnumStates.Patrolling);
          
        }
        private void RotateToPlayer()
        {            
            Vector3 direction = (Enemy.Target.position - Enemy.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            Enemy.transform.rotation = Quaternion.Slerp(Enemy.transform.rotation, lookRotation, Time.deltaTime * EnemyData.TurnSpeed);           
        }
    }
}