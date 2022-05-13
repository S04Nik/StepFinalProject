using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using Photon.Pun;
using UnityEngine;
using Random = System.Random;

namespace Com.Tereshchuk.Shooter
{
    public class UserDB:ScriptableObject
    {
        public string Name;
        public int level;
        public float experienceValue;
        public Dictionary<string, int> Appearance;
        public Dictionary<string, int> HatsAvailable;
        public Dictionary<string, int> CostumesAvailable;
        private static int _hatsTotalAmount = 30;
        private static int _costumesTotalAmount = 7;

        public void InitializeDefault()
        {
            HatsAvailable =  new Dictionary<string, int>();
            HatsAvailable.Add("0",1);
            HatsAvailable.Add("1",1);
            CostumesAvailable = new Dictionary<string, int>();
            CostumesAvailable.Add("0",1);
            CostumesAvailable.Add("1",1);
            
            Appearance = new Dictionary<string, int>();
            Appearance.Add("Active Beard",0);
            Appearance.Add("Active Costume",0);
            Appearance.Add("Active HairCut",0);
            Appearance.Add("Active Hat",0);
            Appearance.Add("Active Gender",1);
            experienceValue = 0;
            level = 1;
        }
        public int GetGenderIndx()
        {
            return Appearance["Active Gender"];
        }
        private async UniTask AddRandomClothes()
        {
            int random = new Random().Next(0, 1);
            if (random == 0 && HatsAvailable.Count<_hatsTotalAmount)
            {
                HatsAvailable.Add(HatsAvailable.Count.ToString(),HatsAvailable.Count);
                await FirebaseController.Instance.UpdateUserAvailableHats();
                Debug.Log("!!!! ADD HAT ");
            }
            else if(random == 0 && HatsAvailable.Count>=_hatsTotalAmount || random == 1)
            {
                if (CostumesAvailable.Count < _costumesTotalAmount)
                {
                    CostumesAvailable.Add(CostumesAvailable.Count.ToString(),CostumesAvailable.Count);
                    await FirebaseController.Instance.UpdateUserAvailableCostumes();
                    Debug.Log("!!!! ADD COSTUME ");
                }
            }
        }
        public void UpdateUserExperience(float exp)
        {
            float calculatedExperience = experienceValue + exp ;
            if (calculatedExperience > 1)
            {
                experienceValue =  1 - calculatedExperience;
                level++;
                AddRandomClothes();
            }
            else
            {
                experienceValue = calculatedExperience;
            }
        }
        public int GetBeardIndx()
        {
            return Appearance["Active Beard"];
        }  
        public int GetHatIndx()
        {
            return Appearance["Active Hat"];
        }
        public int GetCostumeIndx()
        {
            return Appearance["Active Costume"];
        }
        public int GetHairCutIndx()
        {
            return Appearance["Active HairCut"];
        }
        public void CopyAppearance(DataSnapshot snapshot)
        {
            Appearance["Active Costume"] =  int.Parse(snapshot.Child("Active Costume").Value.ToString());

            Appearance["Active HairCut"] = int.Parse(snapshot.Child("Active HairCut").Value.ToString());

            Appearance["Active Hat"] =  int.Parse(snapshot.Child("Active Hat").Value.ToString());

            Appearance["Active Gender"] =  int.Parse(snapshot.Child("Active Gender").Value.ToString());
                
            if ( Appearance["Active Gender"]==0)
            {
                Appearance["Active Beard"] =  int.Parse(snapshot.Child("Active Beard").Value.ToString());
            }
        }
        public void CopyUserData(DataSnapshot snapshot)
        {
            DataSnapshot snapshotCostumes = snapshot.Child("CostumesAvailable");
            DataSnapshot snapshotHats = snapshot.Child("HatsAvailable");
            DataSnapshot snapshotAppearance = snapshot.Child("Appearance");
            
            experienceValue = float.Parse(snapshot.Child("Experience").Value.ToString());
            level =int.Parse(snapshot.Child("Level").Value.ToString());
            Name = snapshot.Child("UserName").Value as string;
            CopyAppearance(snapshotAppearance);
            PhotonNetwork.NickName = Name;
            if (snapshotCostumes.ChildrenCount > 0) 
            {
                InitializeAvailableCostumes(snapshotCostumes);
            }
            if (snapshotHats.ChildrenCount > 0) 
            {
                InitializeAvailableHats(snapshotHats);
            }
        }
        public void InitializeAvailableCostumes(DataSnapshot value)
        {
            if (CostumesAvailable.Count>0)
            {
                CostumesAvailable.Clear();
            }
            foreach (var childSnapshot in value.Children) {
                Debug.Log("+++++   ADDED");
                CostumesAvailable.Add(childSnapshot.Key,int.Parse(childSnapshot.Value.ToString()));
            }
            foreach (var v in CostumesAvailable)
            {
                Debug.Log("+++++  SHOW AVAILABLE"+v);
            }
        }        
        public void InitializeAvailableHats(DataSnapshot value)
        {
            if (HatsAvailable.Count>0)
            {
                HatsAvailable.Clear();
            }
            foreach (var childSnapshot in value.Children) {
               HatsAvailable.Add(childSnapshot.Key,int.Parse(childSnapshot.Value.ToString()));
            }
            foreach (var v in CostumesAvailable)
            {
                Debug.Log("!!!  "+v);
            }
        }
        public async void ApplyAppearenceChanges( int genderIndx, int hairCatIndx, int hatIndx,int costumeIndx, int beardIndx)
        {
            Appearance["Active Costume"] = costumeIndx;
            Appearance["Active HairCut"] = hairCatIndx;
            Appearance["Active Hat"] = hatIndx;
            Appearance["Active Gender"] = genderIndx;
            if ( genderIndx==0)
            {
                Appearance["Active Beard"] = beardIndx;
            }
            await FirebaseController.Instance.UpdateUserAppearance();
        }
    }
}