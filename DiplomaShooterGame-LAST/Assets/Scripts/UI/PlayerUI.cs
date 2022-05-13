using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject playerMenu;
    [SerializeField] private GameObject canvasWidjets;
    [SerializeField] private GameObject aimingDot;
    [SerializeField] private GameObject aimingCross;
    [SerializeField] private GameObject audioMenu;
    [SerializeField] private GameObject GameSettingsMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private KillListContainer _killListContainer;

    public void UpdateKillList(int playerId1, int playerId2)
    {
        _killListContainer.ShowNewDeath(playerId1,playerId2);
    }

    public void DisableAimingDot()
    {
        aimingDot.SetActive(false);
    }
    public void ActivateAimingDot()
    {
        aimingDot.SetActive(true);
    }
    public void OpenMenu()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.lockState = CursorLockMode.None;
        playerMenu.SetActive(true);
        mainMenu.SetActive(true);
        canvasWidjets.SetActive(false);
    }
    public void CloseMenu()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerMenu.SetActive(false);
        mainMenu.SetActive(false);
        canvasWidjets.SetActive(true);
    }

    public void ActivateMainMenu()
    {
        mainMenu.SetActive(true);
        audioMenu.SetActive(false);
        GameSettingsMenu.SetActive(false);
    }
    public void OpenAudioSettings()
    {
        audioMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void OpenGameSettings()
    {
        GameSettingsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void OpenLobby()
    {
        SceneManager.LoadScene(1);
        PhotonNetwork.LeaveRoom();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
