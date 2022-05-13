using System.Collections.Generic;
using Com.Tereshchuk.Shooter;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerCustomization : MonoBehaviour
{
    
   // [HideInInspector]public PlayerSettings _settings;
    [SerializeField] private Transform[] _maleCharacterSkins;
    [SerializeField] private Transform[] _femaleCharacterSkins;
    [SerializeField] private TMP_Dropdown _dropdownGenders;
    [SerializeField] private TMP_InputField _playerName;
    [SerializeField] private Transform[] _hats;
    [SerializeField] private Transform[] _hairCutsMale;
    [SerializeField] private Transform[] _hairCutsFemale;
    [SerializeField] private Transform[] _beards;
    [SerializeField] private GameObject _beardsMenuString;

    private bool beardChosen;
    private int indxSkin;
    private int indxHats;
    private int indxHairCuts;
    private int indxBeards;
    private int indxGender;
    private List<int> _costumesAvailable=new List<int>();
    private  List<int> _hatsAvailable=new List<int>();
    private  Transform[] currentCostumes;
    private  Transform[] currentHairCuts;

    private void Start()
    {
        // Instance = this;
        // _settings = ScriptableObject.CreateInstance<PlayerSettings>();

//        DontDestroyOnLoad(_settings);
        _dropdownGenders.onValueChanged.AddListener(delegate
        {
            OnGenderChange(_dropdownGenders);
        });
    }

    public void InitializeUserCustomizeMenu()
    {
        indxSkin = FirebaseController.Instance._userDB.GetCostumeIndx();
        indxHats = FirebaseController.Instance._userDB.GetHatIndx();
        indxHairCuts = FirebaseController.Instance._userDB.GetHairCutIndx();
        indxBeards = FirebaseController.Instance._userDB.GetBeardIndx();
        indxGender = FirebaseController.Instance._userDB.GetGenderIndx();
        _costumesAvailable.Clear();
        _hatsAvailable.Clear();
        
        Debug.Log("@ COUNT :  "+FirebaseController.Instance._userDB.CostumesAvailable.Count);
        foreach (var v in FirebaseController.Instance._userDB.CostumesAvailable)
        {
            Debug.Log("@ COSTUMES AVAILABLE AT USERDB CLASSS"+v.Value);
        }
        foreach (var item in FirebaseController.Instance._userDB.CostumesAvailable)
        {
            Debug.Log("@ COSTUME AVAILABLE : "+item.Value);
            _costumesAvailable.Add(item.Value);
        }
        foreach (var item in FirebaseController.Instance._userDB.HatsAvailable)
        {
            Debug.Log("@ HAT AVAILABLE : "+item.Value);
            _hatsAvailable.Add(item.Value);
        }
        if (FirebaseController.Instance._userDB.GetGenderIndx() == 0)
        {
            currentCostumes = _maleCharacterSkins;
            currentHairCuts = _hairCutsMale;
        }
        else
        {
            currentCostumes = _femaleCharacterSkins;
            currentHairCuts = _hairCutsFemale;
            _beardsMenuString.SetActive(false);
            _beards[indxBeards].gameObject.SetActive(false);
        }
        
        ActivateUserLook();
    }
    private void SetActiveStyle(Transform[]arr,int activeIndx)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (i != activeIndx)
            {
                arr[i].gameObject.SetActive(false);
            }
            else
            {
                arr[i].gameObject.SetActive(true);
            }
        }
    }
    public void ActivateUserLook()
    {
        _dropdownGenders.value = FirebaseController.Instance._userDB.GetGenderIndx();
        if (FirebaseController.Instance._userDB.GetGenderIndx() == 0)
        {
            SetActiveStyle(_maleCharacterSkins,FirebaseController.Instance._userDB.GetCostumeIndx());
            SetActiveStyle(_hairCutsMale,FirebaseController.Instance._userDB.GetHairCutIndx());
            SetActiveStyle(_beards,FirebaseController.Instance._userDB.GetBeardIndx());
        }
        else
        {
            SetActiveStyle(_femaleCharacterSkins,FirebaseController.Instance._userDB.GetCostumeIndx());
            SetActiveStyle(_hairCutsFemale,FirebaseController.Instance._userDB.GetHairCutIndx());
            
        }
        SetActiveStyle(_hats, FirebaseController.Instance._userDB.GetHatIndx());
    }
    public void OnGenderChange(TMP_Dropdown dropdown)
    {
        indxGender = dropdown.value;
        if (indxGender == 0)
        {
            currentCostumes = _maleCharacterSkins;
            currentHairCuts = _hairCutsMale;
            // beards
            _beardsMenuString.SetActive(true);
            if(beardChosen)
                _beards[indxBeards].gameObject.SetActive(true);
            // skins
            _femaleCharacterSkins[indxSkin].gameObject.SetActive(false);
            _maleCharacterSkins[indxSkin].gameObject.SetActive(true);
            // hair cuts 
            _hairCutsFemale[indxHairCuts].gameObject.SetActive(false);
            _hairCutsMale[indxHairCuts].gameObject.SetActive(true);
        }
        else
        {
            currentCostumes = _femaleCharacterSkins;
            currentHairCuts = _hairCutsFemale;
            // beards
            _beardsMenuString.SetActive(false);
            _beards[indxBeards].gameObject.SetActive(false);
            // skins
            _maleCharacterSkins[indxSkin].gameObject.SetActive(false);
            _femaleCharacterSkins[indxSkin].gameObject.SetActive(true);
            // hair cuts 
            _hairCutsFemale[indxHairCuts].gameObject.SetActive(true);
            _hairCutsMale[indxHairCuts].gameObject.SetActive(false);
        }
    }
    public void MoveToNext(int direction)
    {
        if (direction > 0)
        {
            if (_costumesAvailable.Count-1 <= indxSkin)
            {
                indxSkin = 0;
            }else
                indxSkin++;
        }
        else
        {
            if (indxSkin == 0)
            {
                indxSkin = _costumesAvailable.Count-1;
            }else
                indxSkin--;
        }

        if (indxSkin > 0 && direction>0)
        {
            currentCostumes[indxSkin-1].gameObject.SetActive(false);
        }
        else if (direction>0)
        {
            currentCostumes[_costumesAvailable.Count-1].gameObject.SetActive(false);
        }
        if (direction < 0 && indxSkin <_costumesAvailable.Count-1)
        {
            currentCostumes[indxSkin+1].gameObject.SetActive(false);
        }else if (direction < 0)
        {
            currentCostumes[0].gameObject.SetActive(false);
        }
        currentCostumes[indxSkin].gameObject.SetActive(true);
    }
    public void MoveToNextHat(int direction)
    {
        if (direction > 0)
        {
            if (_hatsAvailable.Count - 1 <= indxHats)
            {
                indxHats = 0;
            }
            else
                indxHats++;
        }
        else
        {
            if (indxHats == 0)
            {
                indxHats = _hatsAvailable.Count-1;
            }else
                indxHats--;
        }

        if (indxHats > 0 && direction >0)
        {
            _hats[indxHats-1].gameObject.SetActive(false);
        }
        else if (direction >0)
        {
            _hats[_hatsAvailable.Count-1].gameObject.SetActive(false);
        }
        if (direction < 0 && indxHats <_hatsAvailable.Count-1)
        {
            _hats[indxHats+1].gameObject.SetActive(false);
        }else if (direction < 0)
        {
            _hats[0].gameObject.SetActive(false);
        }
        _hats[indxHats].gameObject.SetActive(true);
    }
    public void MoveToNextBeard(int direction)
    {
        if (beardChosen == false) beardChosen = true;
        if (direction > 0)
        {
            if (_beards.Length - 1 <= indxBeards)
            {
                indxBeards = 0;
            }
            else
                indxBeards++;
        }
        else
        {
            if (indxBeards == 0)
            {
                indxBeards = _beards.Length-1;
            }else
                indxBeards--;
        }

        if (indxBeards > 0 && direction > 0)
        {
            _beards[indxBeards-1].gameObject.SetActive(false);
        }
        else if (direction > 0)
        {
            _beards[_beards.Length-1].gameObject.SetActive(false);
        }
        
        if (direction < 0 && indxBeards <_beards.Length-1)
        {
            _beards[indxBeards+1].gameObject.SetActive(false);
        }else if (direction < 0)
        {
            _beards[0].gameObject.SetActive(false);
        }

        _beards[indxBeards].gameObject.SetActive(true);
    }
    public void MoveToNextHairCut(int direction)
    {
        if (direction > 0)
        {
            if (currentHairCuts.Length - 1 <= indxHairCuts)
            {
                indxHairCuts = 0;
            }
            else
                indxHairCuts++;
        }
        else
        {
            if (indxHairCuts == 0)
            {
                indxHairCuts = currentHairCuts.Length-1;
            }else
                indxHairCuts--;
        }

        if (indxHairCuts > 0 && direction > 0)
        {
            currentHairCuts[indxHairCuts-1].gameObject.SetActive(false);
        }
        else if (direction > 0)
        {
            currentHairCuts[_hairCutsMale.Length-1].gameObject.SetActive(false);
        }
        
        if (direction < 0 && indxHairCuts <currentHairCuts.Length-1)
        {
            currentHairCuts[indxHairCuts+1].gameObject.SetActive(false);
        }else if (direction < 0)
        {
            currentHairCuts[0].gameObject.SetActive(false);
        }

        currentHairCuts[indxHairCuts].gameObject.SetActive(true);
    }

    public void Done()
    {
        if (_playerName.text.Length > 0)
        {
            FirebaseController.Instance.UpdateUserName(_playerName.text);
        }
        FirebaseController.Instance._userDB.ApplyAppearenceChanges(indxGender,indxHairCuts,indxHats,indxSkin,indxBeards);
        // if(indxGender==0)
        //     _settings.Skins.Add(PhotonNetwork.LocalPlayer.NickName,_maleCharacterSkins[indxSkin].name);
        // else
        //     _settings.Skins.Add(PhotonNetwork.LocalPlayer.NickName,_femaleCharacterSkins[indxSkin].name);
        //
        // _settings.Genders.Add(PhotonNetwork.LocalPlayer.NickName,indxGender);
        // _settings.HairCuts.Add(PhotonNetwork.LocalPlayer.NickName,indxHairCuts);
        // _settings.Hats.Add(PhotonNetwork.LocalPlayer.NickName,indxHats);
        // if(indxGender==1)
        //     _settings.Beards.Add(PhotonNetwork.LocalPlayer.NickName,0);
        // else
        //     _settings.Beards.Add(PhotonNetwork.LocalPlayer.NickName,indxBeards);

    }

}