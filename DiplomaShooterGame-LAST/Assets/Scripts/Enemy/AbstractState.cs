using System.Collections;

namespace Com.Tereshchuk.Shooter
{
    public abstract class AbstractState
    {
        protected delegate IEnumerator YieldingEventHandler ();
        public EnumStates Name { get; private set; }
        public EnemyData EnemyData { get; private set; }

        protected EnemyView Enemy;
        protected EnemyAnimationSystem AnimSystem;
        
        
        protected AbstractState(EnemyView enemy,EnemyAnimationSystem animS, EnumStates stateName)
        {
            Enemy = enemy;
            AnimSystem = animS;
            Name = stateName;
            EnemyData = enemy.EnemyData;
        }
        public abstract void Init();
        public abstract void Do();
    }
}