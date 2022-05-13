using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class EnemyData
    {
        public float Health { get; private set; } // изменить может только playerData - сокрытие данных 
        public float LookRadius { get; private set;}
        public float TurnSpeed { get; private set; }
        public float Damage { get; private set; }
        public float WeakDamage { get; private set; }
        public int DestPoint { get; private set; }
        public bool Alive { get; private set; }
        
        private int _patrolPointsCount;
        public EnemyData(EnemyConfig config)
        {
            Health = 100;
            LookRadius = 10f;
            TurnSpeed = 3f; 
            Damage = 50; 
            WeakDamage = 10;
            DestPoint = 0;
            _patrolPointsCount=config.PatrolPoints.Count-1;
            Alive = true;
        }


        public void IncrementDestPoint()
        {
            if (DestPoint==_patrolPointsCount)
            {
                DestPoint = 0;
            }else
                DestPoint++;
        }

        public void SetHealth(float health)
        {
            Debug.Log(health);
            Health = health;
            if (Health <= 0)
            {
                Alive = false;
            }
        }
    }
}