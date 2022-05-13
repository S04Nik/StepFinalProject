using Photon.Pun;
using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class PlayerUiController:MonoBehaviourPun
    {
        private PlayerUI _playerUi;
        private bool _menuIsOpened;
        [SerializeField] private Animator playerAnimator;
        private static readonly int IsAiming = Animator.StringToHash("isAiming");
        private static readonly int WeaponIndex = Animator.StringToHash("WeaponIndex");

        public void UpdateKillList(int id1, int id2)
        {
            _playerUi.UpdateKillList(id1,id2);
        }
        private void Start()
        {
            _playerUi = GameObject.Find("PlayerUI").GetComponent<PlayerUI>();
        }
        private void Update()
        {
            if (photonView.IsMine)
            {
                if (Input.GetKeyDown(KeyCode.Escape)&&!_menuIsOpened)
                {
                    _playerUi.OpenMenu();
                    _menuIsOpened = true;
                }else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    _playerUi.CloseMenu();
                    _menuIsOpened = false;
                }
                if (playerAnimator.GetBool(IsAiming) && playerAnimator.GetInteger(WeaponIndex)<=1)
                {
                    _playerUi.ActivateAimingDot();
                }
                else
                {
                    _playerUi.DisableAimingDot();
                }
            }

        }
    }
}