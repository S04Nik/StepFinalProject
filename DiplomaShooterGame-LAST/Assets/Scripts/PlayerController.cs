using System.Collections;
using Cinemachine;
using Com.Tereshchuk.Shooter.NewWeapon_Inventory_System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

namespace Com.Tereshchuk.Shooter
{
    public class PlayerController : MonoBehaviourPunCallbacks
    {
        #region Variables
        
        [SerializeField]
        public CinemachineVirtualCamera followCamera;
        public Camera mainCamera;
        public MultiAimConstSetUp rigging;
        [SerializeField] private InventoryController inventoryController;
        private SpawnManager _spawnManager;
        private Transform _canvasHitSuccess;
        public Light light;
        [SerializeField] private Transform leftHand;
        public int score = 0;
        [SerializeField] private PlayerUiController widjectController;
        [SerializeField] private PlayerAudioController playerAudioController;
        [SerializeField] private ParticleSystem hitEffectPrefab;
        [SerializeField] private HealthController healthController;
        private ParticleSystem _hitEffect;
        
        #endregion

        #region MonoBehaviour Callbacks
        
        public void SetGenderVoice(int gender)
        {
            playerAudioController.SetVoice(gender);
        }
        private void Awake()
        {
            if (photonView.IsMine) return;
            mainCamera = GameObject.FindWithTag("FreeCamera").GetComponent<Camera>();
            mainCamera.gameObject.tag = "BusyCamera";
        }
        public void Start()
        {
            _spawnManager = GameObject.Find("Manager").GetComponent<SpawnManager>();
            light = GameObject.FindWithTag("Light").GetComponent<Light>();
            light.intensity = PlayerPrefs.GetFloat("masterBrightness");
            _hitEffect = Instantiate(hitEffectPrefab, transform.position, quaternion.identity, transform);

            if (!photonView.IsMine) // LAYERS
            {
                mainCamera.enabled = false;
                mainCamera.GetComponent<CinemachineBrain>().gameObject.SetActive(false);
                mainCamera.GetComponent<AudioListener>().gameObject.SetActive(false);
                followCamera.enabled = false;
                gameObject.layer = _spawnManager.GetPlayerLayer();
                _spawnManager.ConfigLayers(mainCamera,followCamera);
            }

            if (photonView.IsMine)
            {
                _canvasHitSuccess = GameObject.FindWithTag("CanvasAimingSuccessHit").transform;
                _canvasHitSuccess.gameObject.SetActive(false);
            }
            rigging.InitializeBonesConstraints();
        }
        void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            healthController.UpdateHealth();
        }

        #endregion
        
        #region Public Methods
        
        [PunRPC]
        public void RPCCameraSet(int cameraViewID)
        {
            mainCamera = PhotonView.Find(cameraViewID).GetComponent<Camera>();
        }
        public void SetCamera(int cameraViewID)
        {
            photonView.RPC(nameof(RPCCameraSet),RpcTarget.AllBuffered,cameraViewID);
        }
        
        [PunRPC]
        public void UpdateKillList(int view1,int view2)
        {
            if (view1 == photonView.ViewID)
            {
                Debug.Log("ENEMY !!!!!");
            }
            widjectController.UpdateKillList(view1,view2);
        }

        [PunRPC]
        public void IncreaseScore(int _score)
        {
            if (photonView.IsMine)
            {
                score += _score;
            }
        }
        [PunRPC]
        public void TakeDamage(int damage ,Vector3 hitInfoPoint,int enemyId)
        {
            if (photonView.IsMine)
            {
                if (!healthController.DecreaseHealth(damage))
                {
                    PhotonView enemyView =  PhotonView.Find(enemyId);
                    inventoryController.ItemWidjet.ClearWidjets();
                    photonView.RPC(nameof(UpdateKillList),RpcTarget.All,enemyId,photonView.ViewID);
                    // wont find if it will be disactive
                    _canvasHitSuccess.gameObject.SetActive(true);
                    //_spawnManager.Spawn();
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    PhotonNetwork.Destroy(gameObject);
                    PhotonNetwork.Destroy(mainCamera.gameObject);
                    PhotonNetwork.LeaveRoom();
                    SceneManager.LoadScene(1);
                }
                playerAudioController.PlayPainVoice();
            }
            // BLEEDING
            _hitEffect.transform.position = hitInfoPoint;
             _hitEffect.Play();
        }
        public void TakeDamageNPC(int damage ,Vector3 hitInfoPoint)
        {
            if (photonView.IsMine)
            {
                if (!healthController.DecreaseHealth(damage))
                {
                    inventoryController.ItemWidjet.ClearWidjets();
                    // wont find if it will be disactive
                    _canvasHitSuccess.gameObject.SetActive(true);
                    //_spawnManager.Spawn();
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    PhotonNetwork.Destroy(gameObject);
                    PhotonNetwork.Destroy(mainCamera.gameObject);
                    PhotonNetwork.LeaveRoom();
                    SceneManager.LoadScene(2);
                }
                playerAudioController.PlayPainVoice();
            }
            // BLEEDING
            _hitEffect.transform.position = hitInfoPoint;
            _hitEffect.Play();
        }
        
        public void GiveDamage(int damage , int enemyId,Vector3 hitInfoPoint)
        {
            if (photonView.IsMine)
            {
                PlayerController enemyController = PhotonView.Find(enemyId).GetComponent<PlayerController>();
                PhotonView tmprView = enemyController.photonView;
                tmprView.RPC(nameof(TakeDamage), RpcTarget.All, damage, hitInfoPoint,photonView.ViewID);
                FirebaseController.Instance._userDB.UpdateUserExperience(0.005f);
                UpdateExperience();
                ActivateHitSuccessUi();
            }
        }

        private async UniTask ActivateHitSuccessUi()
        {
            _canvasHitSuccess.gameObject.SetActive(true);
            await UniTask.Delay(300);
            _canvasHitSuccess.gameObject.SetActive(false);
        }
        #endregion

        async UniTask UpdateExperience()
        {
            await FirebaseController.Instance.UpdateUserExp( FirebaseController.Instance._userDB.experienceValue);
            await FirebaseController.Instance.UpdateUserLevel( FirebaseController.Instance._userDB.level);
        }
    }

}
