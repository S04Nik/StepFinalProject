using System.Collections;
using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class EnemyRecieveDmgState : AbstractState
    {
        protected event YieldingEventHandler OnYieldTestEvent;
        private EnemyHealthSystem _healthSystem;
        
        public EnemyRecieveDmgState(EnemyView enemy, EnemyAnimationSystem animS, EnumStates stateName, EnemyHealthSystem healthSystem) : base(enemy, animS, stateName)
        {
            _healthSystem = healthSystem;
            OnYieldTestEvent += RecieveHit;
        }

        public override void Init()
        {
            
        }

        public override void Do()
        {
            AnimSystem.ResetAttack();
            if (EnemyData.Health > 0)
            {
                AnimSystem.RecieveHit();
                Enemy.StartCoroutine(OnYieldTestEvent?.Invoke());
            }
        }

        public IEnumerator RecieveHit()
        {
            while (true)
            {
                if (AnimSystem.GetRecievingHit() == false && EnemyData.Alive)
                {
                    Enemy.StopCorAll(EnumStates.Attack);                   
                }
                yield return new WaitForSeconds(0.05f);
                AnimSystem.FinishRecievingHit();break;
            }
        }
        
    }
}