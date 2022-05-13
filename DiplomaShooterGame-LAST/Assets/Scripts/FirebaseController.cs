using System.Collections;
using System.Collections.Generic;
using Com.Tereshchuk.Shooter;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using Slider = UnityEngine.UI.Slider;

public class FirebaseController : MonoBehaviour
{
   public static FirebaseController Instance;
   //Firebase variables
   public DependencyStatus dependencyStatus;
   public FirebaseAuth Auth;
   public FirebaseUser User;
   public DatabaseReference DBReference;
   
   //LOGIN
   public TMP_InputField emailLoginField;
   public TMP_InputField passwordLoginField;
   public TMP_Text warningLoginText;
   public TMP_Text confirmLoginText;
   //REGISTER
   public TMP_InputField usernameRegisterField;
   public TMP_InputField emailRegisterField;
   public TMP_InputField passwordRegisterField;
   public TMP_InputField passwordRegisterVerifyField;
   public TMP_Text warningRegisterText;

   //DATABASE

   [HideInInspector]public UserDB _userDB ;
   public void InitializeFirebase()
   {
      Auth = FirebaseAuth.DefaultInstance;
      DBReference = FirebaseDatabase.DefaultInstance.RootReference;
      Instance = this;
   }
   public void Awake()
   {
      FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
      {
         dependencyStatus = task.Result;
         if (dependencyStatus == DependencyStatus.Available)
         {
            InitializeFirebase();
         }
         else
         {
            Debug.Log("dependencyStatus != DependencyStatus.Available");
         }
         
      });
   }
   
   public async void LoginButton()
   {
      await Login(emailLoginField.text, passwordLoginField.text);
   }
   public async void RegisterButton()
   {
      await Register(emailRegisterField.text, passwordRegisterField.text,usernameRegisterField.text);
   }
   private async UniTask Login(string _email, string _password)
   {
      var LoginTask = Auth.SignInWithEmailAndPasswordAsync(_email, _password);
      await UniTask.WaitUntil(predicate: () => LoginTask.IsCompleted);
      if (LoginTask.Exception != null)
      {
         Debug.LogWarning(message:$"Failed to register task with {LoginTask.Exception}");
         FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
         AuthError errorCode = (AuthError) firebaseEx.ErrorCode;

         string message = "Login Failed !";
         switch (errorCode)
         {
            case AuthError.MissingEmail:
               message = "Missing Email";
               break;
            case AuthError.MissingPassword:
               message = "Missing Password";
               break;
            case AuthError.WrongPassword:
               message = "Wrong Password";
               break;
            case AuthError.InvalidEmail:
               message = "Invalid Email";
               break;
            case AuthError.UserNotFound:
               message = "Account does not exist";
               break;
         }

         warningLoginText.text = message;
      }
      else
      {
         User = LoginTask.Result;
         Debug.LogFormat("User signed in successfully : {0} ({1})",User.DisplayName,User.Email);
         warningLoginText.text = "";
         confirmLoginText.text = "Logged In";
         await UniTask.Delay(2000);
         confirmLoginText.text = "";
         await LoadUserData();
      }
   }
   private async UniTask Register(string _email, string _password, string _username)
   {
      if (_username == "")
      {
         warningRegisterText.text = "Missing Username";
      }else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
      {
         warningRegisterText.text = "Password Does Not Match!";
      }
      else
      {
         var RegisterTask = Auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

         await UniTask.WaitUntil(predicate: () => RegisterTask.IsCompleted);

         if (RegisterTask.Exception != null)
         {
            Debug.LogWarning(message:$"Failed to register task with {RegisterTask.Exception}");
            FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Register Failed!";
            switch (errorCode)
            {
               case AuthError.MissingEmail:
                  message = "Missing Email";
                  break;
               case AuthError.MissingPassword:
                  message = "Missing Password";
                  break;
               case AuthError.WeakPassword:
                  message = "Weak Password";
                  break;
               case AuthError.EmailAlreadyInUse:
                  message = "Email already in Use";
                  break;
            }

            warningRegisterText.text = message;
         }
         else
         {
            User = RegisterTask.Result;

            if (User != null)
            {
               UserProfile profile = new UserProfile();
               profile.DisplayName = _username;
               var ProfileTask = User.UpdateUserProfileAsync(profile);
               await UniTask.WaitUntil(() => ProfileTask.IsCompleted);
               if (ProfileTask.Exception != null)
               {
                  Debug.LogWarning(message:$"Failed to register task with {ProfileTask.Exception}");
                  FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                  AuthError errorCode = (AuthError) firebaseEx.ErrorCode;
                  warningRegisterText.text = "Username Set Failed !";
               }
               else
               {
                  warningRegisterText.text = "";
                  await DefaultUserUpdate();
               }
               
            }

         }
      }
   }
   private async UniTask DefaultUserUpdate()
   {
      if (!_userDB)
      {
         _userDB = ScriptableObject.CreateInstance<UserDB>();
         _userDB.InitializeDefault();
         _userDB.Name = User.DisplayName;
      }

      Dictionary<string, string> mainInfo = new Dictionary<string, string>();
      mainInfo.Add("UserName",_userDB.Name);
      mainInfo.Add("Level",_userDB.level.ToString());
      mainInfo.Add("Experience",_userDB.experienceValue.ToString());

      var DBTask = DBReference.Child("users").Child(User.UserId).SetValueAsync(mainInfo);
      await UniTask.WaitUntil(() => DBTask.IsCompleted);
      if (DBTask.Exception != null)
      {
         Debug.LogWarning(message:$"Failed to register task with {DBTask.Exception}");
      }else
      {
         await UpdateUserAppearance();
         await UpdateUserAvailableCostumes();
         await UpdateUserAvailableHats();
      }

   }
   
   public async UniTask UpdateUserNameAuth(string _username)
   {
      UserProfile profile = new UserProfile();
      profile.DisplayName = _username;
      var ProfileTask = User.UpdateUserProfileAsync(profile);

      await UniTask.WaitUntil(() =>  ProfileTask.IsCompleted);
      if (ProfileTask.Exception != null)
      {
         Debug.LogWarning(message:$"Failed to register task with {ProfileTask.Exception}");
      }
      else
      {
         Debug.Log("!!!!! SUCCESSSSSSSSSS ");
      }
   }
   public async UniTask UpdateUsernameDatabase(string _username)
   {
      var DBTask = DBReference.Child("users").Child(User.UserId).Child("UserName").SetValueAsync(_username);

      await UniTask.WaitUntil(() => DBTask.IsCompleted);
      if (DBTask.Exception != null)
      {
         Debug.LogWarning(message:$"Failed to register task with {DBTask.Exception}");
      }
      else
      {
         Debug.Log("!!!!! SUCCESSSSSSSSSS ");
      }
   }
   public async UniTask UpdateUserExp(float expPoints)
   {
      var DBTask = DBReference.Child("users").Child(User.UserId).Child("Experience").SetValueAsync(expPoints);

      await UniTask.WaitUntil(() => DBTask.IsCompleted);
      if (DBTask.Exception != null)
      {
         Debug.LogWarning(message:$"Failed to register task with {DBTask.Exception}");
      }
      else
      {
         Debug.Log("!!!!! SUCCESSSSSSSSSS ");
      }
   }
   public async UniTask UpdateUserAppearance()
   {
        Debug.Log("USER APPEARENCE UPDATE");
      var DBTask = DBReference.Child("users").Child(User.UserId).Child("Appearance").SetValueAsync(_userDB.Appearance);
      await UniTask.WaitUntil( () => DBTask.IsCompleted);
      if (DBTask.Exception != null)
      {
         Debug.LogWarning(message:$"Failed to register task with {DBTask.Exception}");
      }
      else
      {
         Debug.Log("!!!!! SUCCESSSSSSSSSS ");
      }
   }
   public async UniTask UpdateUserName(string name)
   {
      _userDB.Name = name;
      PhotonNetwork.NickName = name;
      MenuManager.Instance.UpdateMainMenuInfo();
      await UpdateUsernameDatabase(name);
   }
   
   private async UniTask UpdateUserData()
   {
      var DBTask = DBReference.Child("users").Child(User.UserId).SetValueAsync(_userDB);
      
      await UniTask.WaitUntil(() => DBTask.IsCompleted);
      if (DBTask.Exception != null)
      {
         Debug.LogWarning(message:$"Failed to register task with {DBTask.Exception}");
      }else
      {
         Debug.Log("SUCCESS");
      }
   }
   public async UniTask LoadUserData()
   {
      var DBTask = DBReference.Child("users").Child(User.UserId).GetValueAsync();
      
      await UniTask.WaitUntil(() => DBTask.IsCompleted);
      if (DBTask.Exception != null)
      {
         Debug.LogWarning(message:$"Failed to register task with {DBTask.Exception}");
      }
      else if(DBTask.Result.Value == null)
      {
         Debug.Log("!!!!! USER ID VALUE NULL ");
      }
      else
      {
         if (!_userDB)
         {
            _userDB = ScriptableObject.CreateInstance<UserDB>();
            _userDB.InitializeDefault();
         }
         _userDB.CopyUserData(DBTask.Result);
         SceneManager.LoadScene(2);
      }
   }
   public async UniTask UpdateUserAvailableCostumes()
   {
      var DBTask = DBReference.Child("users").Child(User.UserId).Child("CostumesAvailable").SetValueAsync(_userDB.CostumesAvailable);

      await UniTask.WaitUntil(() => DBTask.IsCompleted);
      if (DBTask.Exception != null)
      {
         Debug.LogWarning(message:$"Failed to register task with {DBTask.Exception}");
      }
      else
      {
         Debug.Log("!!!!! SUCCESSSSSSSSSS ");
      }
   }
   public async UniTask UpdateUserAvailableHats()
   {
      var DBTask = DBReference.Child("users").Child(User.UserId).Child("HatsAvailable").SetValueAsync(_userDB.CostumesAvailable);

      await UniTask.WaitUntil(() => DBTask.IsCompleted);
      if (DBTask.Exception != null)
      {
         Debug.LogWarning(message:$"Failed to register task with {DBTask.Exception}");
      }
      else
      {
         Debug.Log("!!!!! SUCCESSSSSSSSSS ");
      }
   }
   public async UniTask UpdateUserLevel(int level)
   {
      var DBTask = DBReference.Child("users").Child(User.UserId).Child("Level").SetValueAsync(level);

      await UniTask.WaitUntil(predicate: () => DBTask.IsCompleted);
      if (DBTask.Exception != null)
      {
         Debug.LogWarning(message:$"Failed to register task with {DBTask.Exception}");
      }
      else
      {
         Debug.Log("!!!!! SUCCESSSSSSSSSS ");
      }

   }
   

}