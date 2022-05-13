using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    [Serializable]
    public class EnemyConfig
    {
        [SerializeField] private List<Transform> _patrolPoints;
        public List<Transform> PatrolPoints             // динамически изменяемый массив
        { 
            get => _patrolPoints;
            set => _patrolPoints = value;
        }
    }
}