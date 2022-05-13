using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance; // SINGLTONE
    //variable bind to the class . not actual obj 
    [SerializeField] private Menu[] menus;
    private string previousMenuName;
    [SerializeField] private PlayerCustomization _playerCustomizeUi;
    public TMP_Text usernameText;
    public Slider sliderExperience;
    public TMP_Text levelText;

    public void ExitGame()
    {
        //PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        Application.Quit();
    }

    public void ExitRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(1);
    }
    public void UpdateMainMenuInfo()
    {
        Debug.Log(usernameText.text);
        usernameText.text = FirebaseController.Instance._userDB.Name;
        sliderExperience.value = FirebaseController.Instance._userDB.experienceValue;
        levelText.text = FirebaseController.Instance._userDB.level.ToString();
        _playerCustomizeUi.InitializeUserCustomizeMenu();
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateMainMenuInfo();
    }

    public void OpenPreviousMenu()
    {
        OpenMenu(previousMenuName);
    }
    public void OpenMenu(string menuName)
    {
        foreach (var m in menus)
        {
            if (m.menuName == menuName)
            {
              m.Open();
            }else if (m.open)
            {
                previousMenuName = m.menuName;
                CloseMenu(m);
            }
        }
    }
    public void OpenMenu(Menu menu)
    {
        foreach (var m in menus)
        {
            if (m.open)
            {
                previousMenuName = m.menuName;
                CloseMenu(m);
            }
        }
        menu.Open();
    }
    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}
