using System;
using UnityEngine;
using UnityEngine.UI;

namespace Com.Tereshchuk.Shooter
{
    // bland / layers
    // возможность накладывать аватаров и анимаций
    public class EnemyHpBar : MonoBehaviour
    {
        [SerializeField] private Image healthBar;
        [SerializeField] private GameObject hpBarPanel;
        [SerializeField] private Canvas _canvas;
        public Camera _cameraMain;
        
        private void Awake()
        {
            HideBar();
        }

        public void Initialize(Camera cameraMain)
        {
            _canvas.worldCamera = cameraMain;
            _cameraMain = cameraMain;
            ShowBar();
        }

        public void ShowBar()
        {
            hpBarPanel.SetActive(true);
        }

        public void HideBar()
        {
            hpBarPanel.SetActive(false);
        }
        public void FillHealthBar(float amount)
        {
            // better to make coroutine and reduce hp slowly
            healthBar.fillAmount =amount ; 
            if (amount == 0)
            {
                Destroy(hpBarPanel);
            }
        }
        public void LateUpdate()
        {
            if (_cameraMain)
            {
                hpBarPanel.transform.LookAt(_cameraMain.transform);
            }
       
        }
    }
}