using Com.Tereshchuk.Shooter.NewWeapon_Inventory_System;
using Photon.Pun;
using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class ReloadWeapon : MonoBehaviourPunCallbacks
    {
        private WeaponAnimationController _weaponAnimationController;
        private GunSoundController _gunSoundController;
        private Transform leftHand;
        public bool isReloading;
        private GameObject magazineAtHand;
        [SerializeField] private GameObject originalMagazine;
        private Gun loadOut;
        
        private void Start()
        {
            _weaponAnimationController = GetComponent<WeaponAnimationController>();
            _weaponAnimationController.AnimationEvents.WeaponAnimationEvent.AddListener(OnAnimationEvent);
            _gunSoundController = GetComponent<GunSoundController>();
        }
        public void Initialize(Transform leftH,Gun LoadOut)
        {
            leftHand = leftH;
            loadOut = LoadOut;
        }
        public bool IsReloading()
        {
            return isReloading;
        }
        void Update()
        {
            if (photonView.IsMine)
            {
                if (loadOut)
                {
                    if (loadOut.GetClip() == 0 && loadOut.GetStash() > 0 || isReloading && loadOut.GetStash()>0)
                    {
                        // if (!weaponController._isHolstered)
                        //  {
                        isReloading = true;
                        _weaponAnimationController.PlayReloading();
                        // }
                    }
                }
            }
        }

        void OnAnimationEvent(string eventName)
        {
            if (photonView.IsMine && isReloading)
            {
                Debug.Log("!!!!! OnAnimationEvent"+photonView.ViewID);
                switch (eventName)
                {
                    case "detaching_magazine":
                        DetachMagazine();
                        break;
                    case "droping_magazine":
                        DropMagazine();
                        break;
                    case "refilling_magazine":
                        RefillMagazine();
                        break;
                    case "attaching_magazine":
                        AttachMagazine();
                        break;
                }
            }
        }
        // ot imeni controller personaga wizov zbrosa magazinov
        private void DetachMagazine()
        {
            if (photonView.IsMine && isReloading)
            {
                    magazineAtHand = Instantiate(originalMagazine, leftHand, true);
               originalMagazine.SetActive(false);
               _gunSoundController.DetachMagazine();
            }
        }

        private void DropMagazine()
        {
            if (photonView.IsMine && isReloading)
            {
                GameObject droppedMagazine = Instantiate(magazineAtHand, magazineAtHand.transform.position,
                magazineAtHand.transform.rotation);
                droppedMagazine.AddComponent<Rigidbody>();
                droppedMagazine.AddComponent<BoxCollider>();
                magazineAtHand.SetActive(false);
            }
        }

        private void RefillMagazine()
        {
            magazineAtHand.SetActive(true);
        }

        private void AttachMagazine()
        {
            if (photonView.IsMine && isReloading)
            {
                originalMagazine.SetActive(true);
                Destroy(magazineAtHand);
                loadOut.Reload();
                _weaponAnimationController.FinishReloading();
                _gunSoundController.AttachMagazine();

                // _ammoWidget.Refresh();

                isReloading = false;
            }
        }
    }
}