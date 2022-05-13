using System;
using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class EnemyHealthSystem : MonoBehaviour
    {
        public event Action <float> OnHealthChange;
        public event Action OnDied;
        
        private EnemyData _enemyData;
        
        
        public void Initialize(EnemyData enemyData)
        {
            _enemyData = enemyData;
        }

        public void ReduceHealth(float damage)
        {
            _enemyData.SetHealth(_enemyData.Health - damage);
            if (_enemyData.Health <= 0)
            {
               OnDied?.Invoke();
            }
            FillHealthBar();
        }

        private void FillHealthBar()
        {
            OnHealthChange?.Invoke( _enemyData.Health / 100);
        }


    }
}