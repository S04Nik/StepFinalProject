using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class EnemyIdleState : AbstractState
    {
        public delegate IEnumerator YieldingEventHandler();
        public static event YieldingEventHandler OnYieldTestEvent;
        public EnemyIdleState(EnemyView enemy,EnemyAnimationSystem animS,EnumStates state) : base(enemy,animS,state)
        {
            OnYieldTestEvent += Pause;
        }

        public override void Init()
        {
            
        }

        public override void Do()
        {
            AnimSystem.StartIdle();
            Enemy.StartCoroutine(OnYieldTestEvent?.Invoke());          
        }
        private IEnumerator Pause()
        {          
            yield return new WaitForSeconds(2);
            AnimSystem.StopIdle();
            Enemy.StopCorAll(EnumStates.Patrolling);
        }



    }
}