using Com.Tereshchuk.Shooter;
using Photon.Pun;
using UnityEngine;

public class SkinApply : MonoBehaviourPunCallbacks,IPunObservable
{
    [SerializeField] private Transform[] maleCharacterSkins;
    [SerializeField] private Transform[] femaleCharacterSkins;
    [SerializeField] private Transform[] hats; 
    [SerializeField] private Transform[] maleHairCuts;
    [SerializeField] private Transform[] femaleHairCuts;
    [SerializeField] private Transform[] beards;
    [SerializeField] private PlayerController playerController;
    
    private int _costumeIndx,_genderIndx, _hatIndx, _hairCutIndx, _beardIndx;

    public void ApplySettings()
    {
        Debug.Log("GENDER :"+_genderIndx);
        _costumeIndx = FirebaseController.Instance._userDB.GetCostumeIndx();
        _hatIndx = FirebaseController.Instance._userDB.GetHatIndx();
        _genderIndx =  FirebaseController.Instance._userDB.GetGenderIndx();
        _hatIndx = FirebaseController.Instance._userDB.GetHatIndx();
        _hairCutIndx = FirebaseController.Instance._userDB.GetHairCutIndx();
        _beardIndx = FirebaseController.Instance._userDB.GetBeardIndx();
       
        ActivateProperSkin();
    }
    
    // INDEXS AT ARRAY SHOULD BE SIMILAR 
    // MADE LIKE THIS BECAUSE OF STRING ( i think) consumes more data
    private void ActivateProperSkin()
    {
        playerController.SetGenderVoice(_genderIndx);
        if (_genderIndx == 0)
        {
            for (int i =0 ; i < maleCharacterSkins.Length;i++)
            {
                if (i ==_costumeIndx)
                    maleCharacterSkins[i].gameObject.SetActive(true);
            }
            for (int i =0 ; i < maleHairCuts.Length;i++)
            {
                if (i ==_hairCutIndx)
                    maleHairCuts[i].gameObject.SetActive(true);
            }
            for (int i =0 ; i < beards.Length;i++)
            {
                if (i ==_beardIndx)
                    beards[i].gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i =0 ; i < femaleCharacterSkins.Length;i++)
            {
                if (i ==_costumeIndx)
                    femaleCharacterSkins[i].gameObject.SetActive(true);
            }
            for (int i =0 ; i < femaleHairCuts.Length;i++)
            {
                if (i ==_hairCutIndx)
                    femaleHairCuts[i].gameObject.SetActive(true);
            }
            beards[0].gameObject.SetActive(true); // NO BEARD
        }
        
        for (int i =0 ; i < hats.Length;i++)
        {
            if (i ==_hatIndx)
                hats[i].gameObject.SetActive(true);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_costumeIndx);
            stream.SendNext(_genderIndx);
            stream.SendNext(_hatIndx);
            stream.SendNext(_hairCutIndx);
            stream.SendNext(_beardIndx);
        }
        else {
            _costumeIndx =(int) stream.ReceiveNext();
            _genderIndx =(int) stream.ReceiveNext();
            _hatIndx =(int) stream.ReceiveNext();
            _hairCutIndx =(int) stream.ReceiveNext();
            _beardIndx =(int) stream.ReceiveNext();
            
            ActivateProperSkin(); 
        }
    }
}
