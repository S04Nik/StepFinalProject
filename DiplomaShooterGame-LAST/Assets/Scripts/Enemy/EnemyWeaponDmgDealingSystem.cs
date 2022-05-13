using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class EnemyWeaponDmgDealingSystem : MonoBehaviour
    {
        public delegate void PlayerIsDied();
        public event PlayerIsDied IsDied;
        private PlayerController _playerView;
        private EnemyView _view;
        private EnemyData _enemyData;
        private EnemyAnimationSystem _animationSystem;
        public bool canAttack = true;
        public void Initialize(EnemyView view)
        {
            _view = view;
            _enemyData = view.EnemyData;
            _animationSystem = view.GetComponent<EnemyAnimationSystem>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (_view.GetCurrentState() == EnumStates.Attack && canAttack && _animationSystem.GetAttack())
            {
                if (other.CompareTag(Consts.PLAYER_TAG)) 
                {
                    if (_playerView == null)
                    {
                        _playerView = other.GetComponent<PlayerController>();
                        
                    }
                    Debug.Log("ZOMBIE ATTACK");
                    // _playerView.TakeDamage(_enemyData.Damage);
                }
                else if (other.CompareTag(Consts.PLAYER_WEAPON)) // weapon (оружие наслудетс от базовго класса) проверка через компонент
                {
                    if (_playerView == null)
                    {
                        _playerView = other.GetComponentInParent<PlayerController>();
                    }
                    Debug.Log("ZOMBIE ATTACK");
                    // _playerView.TakeDamage(_enemyData.WeakDamage);
                }

                // if (_playerView.PlayerData.Alive == false)
                // {
                //     IsDied?.Invoke();
                // }
                canAttack = false;
            }

        }
    }
}