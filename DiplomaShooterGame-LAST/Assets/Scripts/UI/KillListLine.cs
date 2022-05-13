using Com.Tereshchuk.Shooter;
using Com.Tereshchuk.Shooter.NewWeapon_Inventory_System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillListLine : MonoBehaviour
{
    [SerializeField] private Image DeathImage;
    [SerializeField] private TMP_Text name1;
    [SerializeField] private TMP_Text name2;

    public void SetLine(int view1,int view2)
    {
        InventoryController tmpr = PhotonView.Find(view1).GetComponent<InventoryController>();
        Sprite tmprIMage = tmpr.GetActiveWeapon().ItemInfo.icon;
        DeathImage.sprite = tmprIMage;
        DeathImage.gameObject.SetActive(true);
        name1.text = PhotonView.Find(view1).Controller.NickName;
        name2.text = PhotonView.Find(view2).Controller.NickName;
    }

    public void CopyLine(KillListLine line)
    {
        name1.text = line.name1.text;
        name2.text = line.name2.text;
        DeathImage = line.DeathImage;
    }

    // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    // {
    //     if (stream.IsWriting)
    //     {
    //         stream.SendNext(name1.text);
    //         stream.SendNext(name2.text);
    //     }
    //     else
    //     {
    //         name1.text =(string) stream.ReceiveNext();
    //         name2.text =(string) stream.ReceiveNext();
    //     }
    // }
}
