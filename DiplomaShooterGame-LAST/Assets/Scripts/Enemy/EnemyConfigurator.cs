using UnityEngine;


namespace Com.Tereshchuk.Shooter
{
    public class EnemyConfigurator:MonoBehaviour
    {
        private EnemyController _enemyController;
        [SerializeField] private EnemyConfig _enemyConfig;
        [SerializeField] private Camera _camera;
        private void Start()
        {
            GameObject enemyObj = Instantiate(Resources.Load("Enemy") as GameObject);
            _enemyController = enemyObj.GetComponent<EnemyController>();
            _enemyController.Init(_enemyConfig);
        }
    }
}