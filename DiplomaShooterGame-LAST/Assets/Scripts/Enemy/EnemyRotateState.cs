using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Com.Tereshchuk.Shooter
{
    public class EnemyRotateState : AbstractState
    {
        protected event YieldingEventHandler OnYieldTestEvent;
        private NavMeshAgent _agent;
        private float _distance;
        private float _angle;
        private Quaternion lookRotation;
        private Vector3 direction;
        private Transform _transformEnemy;
        public EnemyRotateState(EnemyView enemy, EnemyAnimationSystem animS,NavMeshAgent agent , EnumStates state) : base(enemy, animS , state)
        {
            OnYieldTestEvent += RotateTo;
            _agent = agent;
            _transformEnemy = enemy.transform;
        }

        public override void Init()
        {

        }

        public override void Do()
        {
            Enemy.StartCoroutine(OnYieldTestEvent?.Invoke());
        }
        private IEnumerator RotateTo()
        {
            while (true)
            {
                if(_transformEnemy && Enemy.Target)
                _distance = Vector3.Distance(Enemy.Target.position, _transformEnemy.position);
                direction = (Enemy.EnemyConfig.PatrolPoints[EnemyData.DestPoint].position - _transformEnemy.position).normalized;
                _angle = Vector3.Angle(direction, _transformEnemy.forward);
                //Debug.Log(_angle);
                AnimSystem.ChangeTurnAngle(_angle);
                //lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                //_transformEnemy.rotation = Quaternion.Slerp(_transformEnemy.rotation, lookRotation, Time.deltaTime * 1f);
 
                if (_angle < 20f)
                {
                    break;
                }
                if (_distance <= EnemyData.LookRadius && Enemy.PlayerIsDied==false)
                {
                    AnimSystem.ChangeTurnAngle(0);
                   Enemy.StopCorAll(EnumStates.Chase);
                }
                
                yield return null;
            }
            
            Enemy.StopCorAll(EnumStates.Idle);

        }

    }
}