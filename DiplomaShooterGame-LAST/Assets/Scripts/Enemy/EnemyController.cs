using System;
using System.Linq;
using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private EnemyView _enemy;
        [SerializeField] private int radius;
        private EnemyConfig _enemyConfig;
        private EnemyData _enemyData;
        public BoxCollider _weaponColider;
        public event Action OnDied;
        public event Action<float> OnTakeDamage;
        public bool Check=true;
        private PlayerController _playerController;

        private void DetectTarget()
        {
            float _distance = 0;
            var players = Physics.OverlapSphere(transform.position, radius)
                .Where(other => other.GetComponent<PlayerController>())
                .Select(other => other.GetComponent<PlayerController>()).ToArray();
            foreach (var player in players)
            {
                if (_distance == 0)
                {
                    _distance = Vector3.Distance(transform.position, player.transform.position);
                    _enemy.SetTarget(player);
                    _weaponColider.enabled = true;
                }
                else if (Vector3.Distance(transform.position, player.transform.position) < _distance)
                {
                    _distance = Vector3.Distance(transform.position, player.transform.position);
                    _enemy.SetTarget(player);
                }

            }
        }
        private void Update()
        {
            DetectTarget();
        }
        public void Init(EnemyConfig enemyConfig)
        {
            _enemyConfig = enemyConfig;
            _enemyData = new EnemyData(_enemyConfig);
            _enemy.Init(enemyConfig, _enemyData,this);
            //Subscribe();
        }

        public void TakeDamage(float damage)
        {
            Debug.Log("IT WOOOORKS : "+damage);
            OnTakeDamage(damage);
            //OnTakeDamage?.Invoke(damage);
        }

        public void DeInit()
        {
            Unsubscribe();
        }
        
        private void Subscribe()
        {
           // _healthSystem.OnDied += EnemyDieHandle;
        }

        private void EnemyDieHandle()
        {
            //OnDied?.Invoke();
        }
        
        private void Unsubscribe()
        {

        }
        
    }
    

}
