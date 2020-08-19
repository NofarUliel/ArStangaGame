using SocketIO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
public class MainMenuManager : MonoBehaviour
{
    public static Dictionary<string, User> invitations;
    public static Dictionary<string, GameObject> allInvitationObj;
    public static string currentLevelGame, userInvitation;
   

    [SerializeField]
    private GameObject settingContainer, UIContainer, userUIContainer, MenuUserContainer, ProfileContainer, MainMenuContainer, startGameContainer, gameLobbyContainer, sendInvitationConatiner, receiveInvitationContainer, signInContainer, playerVsPlayerContainer, playerVsComputerContainer;
    [SerializeField]
    private Button joinBtn, creatGameBtn;
    [SerializeField]
    private Text message, usernameMessage, passwordMessage, updateUsernameMessage, updatePasswordMessage, updateAvatardMessage, newUsernameInput;
    private bool isProfileMenuShow,isPasswordEqual, isUpdateAvatar,isClosedEditAvatar;
    private string username, newUsername, password, newPassword; 
    private SocketIOComponent socketReference;
    private Avatar Avatar;
 

    public SocketIOComponent SocketReference
    {
        get
        {
            return socketReference = (socketReference == null) ? FindObjectOfType<NetworkClient>() : socketReference;
        }
    }

    public void Start()
    {
        invitations = new Dictionary<string, User>();
        allInvitationObj = new Dictionary<string, GameObject>();
        Avatar = new Avatar();

        playerVsPlayerContainer.SetActive(false);
        playerVsComputerContainer.SetActive(false);
        startGameContainer.SetActive(false);
        userUIContainer.SetActive(false);
        gameLobbyContainer.SetActive(false);
        MainMenuContainer.SetActive(false);
        signInContainer.SetActive(true);
        MenuUserContainer.SetActive(false);
        isProfileMenuShow = false;
        isUpdateAvatar = false;
        isClosedEditAvatar=false;
        
        NetworkClient.OnSignInComplete += OnSignInComplete;
        NetworkClient.OnStateChange += OnStateChange;
        NetworkClient.OnSignInUpMessage += OnSignInUpMessage;
        NetworkClient.UpdateUsernameEnd += UpdateUsernameEnd;
        NetworkClient.UpdatePasswordEnd += UpdatePasswordEnd;
        NetworkClient.UpdateAvatarEnd += UpdateAvatarEnd;
    }

    private void Update()
    {
        if (GameLobby.isClosedGameLooby)
        {

            sendInvitationConatiner.SetActive(false);
            playerVsPlayerContainer.SetActive(false);
            playerVsComputerContainer.SetActive(false);
            startGameContainer.SetActive(false);
            gameLobbyContainer.SetActive(false);
            signInContainer.SetActive(false);
            MenuUserContainer.SetActive(false);
            receiveInvitationContainer.SetActive(false);
            userUIContainer.SetActive(true);
            MainMenuContainer.SetActive(true);
            UIContainer.SetActive(true);
            GameLobby.isClosedGameLooby = false;
        }

        if (isUpdateAvatar && isClosedEditAvatar)
        {
            Image avatar = ProfileContainer.transform.Find("[ProfileUser]").Find("AvatarNameContainer").Find("Avatar").Find("Image").GetComponent<Image>();
            string nameAvatar = NetworkClient.users[NetworkClient.clientID].avatar;
            Avatar.DisplayAvatar(avatar, nameAvatar);
            isUpdateAvatar = false;
            isClosedEditAvatar = false;
        }
    }
    public void OnJoinGame()
    {
        SocketReference.Emit("joinGame",new JSONObject(JsonUtility.ToJson( new CurrentLevelGame(currentLevelGame))));
    }
    public void OnCreateGame()
    {
        SocketReference.Emit("createNewGame",new JSONObject(JsonUtility.ToJson(new CurrentLevelGame(currentLevelGame))));
        GameLobby.isCloseInvitation = false;
        signInContainer.SetActive(false);
        MainMenuContainer.SetActive(false);
        playerVsPlayerContainer.SetActive(false);
        playerVsComputerContainer.SetActive(false);
       startGameContainer.SetActive(false);
    }
    public void OnClickPlayerVsPlayer()
    {
        GameLobby.isClosedGameLooby = false;

        signInContainer.SetActive(false);
        MainMenuContainer.SetActive(false);
        playerVsPlayerContainer.SetActive(true);
        playerVsComputerContainer.SetActive(false);
        startGameContainer.SetActive(false);

    }
    public void OnClickPlayerVsComputerLevel1()
    {
        currentLevelGame = SceneList.PLAYER_VS_COMPUTER_LEVEL1;
        signInContainer.SetActive(false);
        MainMenuContainer.SetActive(false);
        playerVsPlayerContainer.SetActive(false);
        playerVsComputerContainer.SetActive(false);
        startGameContainer.SetActive(false);
        UIContainer.SetActive(false);
        
        SceneManagementManager.Instance.LoadLevel(SceneList.PLAYER_VS_COMPUTER_LEVEL1, (levelName) => { });
    }
    public void OnClickPlayerVsComputerLevel2()
    {
        currentLevelGame = SceneList.PLAYER_VS_COMPUTER_LEVEL2;
        signInContainer.SetActive(false);
        MainMenuContainer.SetActive(false);
        playerVsPlayerContainer.SetActive(false);
        playerVsComputerContainer.SetActive(false);
        startGameContainer.SetActive(false);
        UIContainer.SetActive(false);

        SceneManagementManager.Instance.LoadLevel(SceneList.PLAYER_VS_COMPUTER_LEVEL2, (levelName) => { });
    }
    public void OnClickPlayerVsComputer()
    {
        signInContainer.SetActive(false);
        MainMenuContainer.SetActive(false);
        playerVsPlayerContainer.SetActive(false);
        playerVsComputerContainer.SetActive(true);
        startGameContainer.SetActive(false);

    }
    public void OnClickMultiplayerLevel1()
    {
        currentLevelGame =SceneList.MULTIPLAYER_LEVEL1;
        signInContainer.SetActive(false);
        MainMenuContainer.SetActive(false);
        playerVsPlayerContainer.SetActive(false);
        playerVsComputerContainer.SetActive(false);
        startGameContainer.SetActive(true);

    }
    public void OnClickMultiplayerLevel2()
    {
        currentLevelGame =SceneList.MULTIPLAYER_LEVEL2;
        signInContainer.SetActive(false);
        MainMenuContainer.SetActive(false);
        playerVsPlayerContainer.SetActive(false);
        playerVsComputerContainer.SetActive(false);
        startGameContainer.SetActive(true);

    }
    public void OnClickSettings()
    {
        signInContainer.SetActive(false);
        MainMenuContainer.SetActive(false);
        playerVsPlayerContainer.SetActive(false);
        playerVsComputerContainer.SetActive(false);
        startGameContainer.SetActive(false);
        settingContainer.SetActive(true);





    }
    public void OnClickQuit()
    {
        Debug.Log("Quit game");
        Application.Quit();

    }

    public void OnSignIn()
    {
        if (this.username!=null && this.password != null)
        {
        SocketReference.Emit("signIn", new JSONObject(JsonUtility.ToJson(new SignInData(this.username, this.password))));
        userUIContainer.transform.Find("User").Find("username").GetComponent<Text>().text = this.username;
            this.message.text = "";
        }
        else
        {
            this.message.text = "Username or password is empty";
        }
        
    }
    public void OnSignInUpMessage(bool result, string message)
    {
        if (result==true)
        {
           this.message.color = Color.green;
        }
        else {this.message.color = Color.red;  }
        this.message.text = message;
    }
    public void OnSignInComplete()
    {
        signInContainer.SetActive(false);
        MainMenuContainer.SetActive(true);
        playerVsPlayerContainer.SetActive(false);
        playerVsComputerContainer.SetActive(false);
        userUIContainer.SetActive(true);
    }
    public void OnCreateAccount()
    {
  
        if (this.username != null && this.password!=null)
        {
            Debug.Log("name=" + this.username);
            SocketReference.Emit("createAccount", new JSONObject(JsonUtility.ToJson(new SignInData(this.username, this.password))));
            this.message.text = "";
        }
        else
        {
            this.message.text = "Username or password is empty";
        }
        

    }
    
    public void OnEditUsername(string text)
    {
        bool isValid = UsernameValidation(text);
        if (isValid)
        {
            this.username = text;
            this.usernameMessage.text = "";
        }
        else
        {
            this.usernameMessage.text = "Invalide username! input need to contain at least one upper and lower letter";

        }

    }
   
    public void OnEditPassword(string text)
    {

        bool isValid = PasswordValidation(text, passwordMessage);
        if (isValid)
        {
            this.password = text;
            this.passwordMessage.text = "";
        }
    }
    public static void OnSendInvitation(User player, GameObject invitationPrefab, Transform invitationContainer)
    {
        if (!(allInvitationObj.ContainsKey(player.id)))
        {
            GameObject playerInvitation = Instantiate(invitationPrefab, invitationContainer);
            playerInvitation.name = string.Format("InvitationPlayer({0})", player.name);
            Text[] texts = playerInvitation.GetComponentsInChildren<Text>();
            Text name = texts[0];
            name.text = player.name.ToString();
            Text wins = texts[1];
            float precent = (NetworkClient.users[player.id].won * 100) / NetworkClient.users[player.id].played;
            precent = (float)System.Math.Round(precent, 1);
            wins.text = precent.ToString() + "% wins";
            invitations.Add(player.name, player);
            allInvitationObj.Add(player.id, playerInvitation);
        }
    }
    public void OnClickAcceptInvitation()
    {
        User user = NetworkClient.users[NetworkClient.clientID];
        Debug.Log("Accept to join lobby:" + user.lobby);
        user.socket.Emit("acceptInvitation", new JSONObject(JsonUtility.ToJson(user)));

    }
    public void OnClickDenyInvitation()
    {
        User user = NetworkClient.users[NetworkClient.clientID];
        Debug.Log("Deny to join lobby:" + user.lobby);
        user.socket.Emit("denyInvitation", new JSONObject(JsonUtility.ToJson(user)));
        NetworkClient.users[NetworkClient.clientID].lobby = 0;
        receiveInvitationContainer.SetActive(false);


    }
    public void OnClickProfileMenu()
    {
        isProfileMenuShow = isProfileMenuShow==true?false:true;
       MenuUserContainer.SetActive(isProfileMenuShow);
    }
    public void OnClickLogout()
    {
        NetworkClient.users[NetworkClient.clientID].socket.Emit("disconnect");
        playerVsPlayerContainer.SetActive(false);
        playerVsComputerContainer.SetActive(false);
        startGameContainer.SetActive(false);
        userUIContainer.SetActive(false);
        gameLobbyContainer.SetActive(false);
        MainMenuContainer.SetActive(false);
        sendInvitationConatiner.SetActive(false);
        signInContainer.SetActive(true);

    }
    
    public void OnClickProfile()
    {
        Image avatar= ProfileContainer.transform.Find("[ProfileUser]").Find("AvatarNameContainer").Find("Avatar").Find("Image").GetComponent<Image>();
        string nameAvatar = NetworkClient.users[NetworkClient.clientID].avatar;
        Avatar.DisplayAvatar(avatar, nameAvatar);
        Text name =ProfileContainer.transform.Find("[ProfileUser]").Find("AvatarNameContainer").Find("UserName-txt").GetComponent<Text>();
        name.text = NetworkClient.users[NetworkClient.clientID].name;
        Text played=ProfileContainer.transform.Find("[ProfileUser]").Find("Played-txt").GetComponent<Text>();
        played.text ="played : "+ NetworkClient.users[NetworkClient.clientID].played.ToString();
        Text won=ProfileContainer.transform.Find("[ProfileUser]").Find("Won-txt").GetComponent<Text>();
        won.text = "won : "+NetworkClient.users[NetworkClient.clientID].won.ToString();

    }
   
    public void OncClickEditUsername()
    {

        newUsernameInput.text= NetworkClient.users[NetworkClient.clientID].name;

    }

    public void OnEditNewUsername(string text)
    {
        bool isValid = UsernameValidation(text);
        if (isValid)
        {

            this.newUsername = text;
            this.updateUsernameMessage.text = "";
           
        }
        else
        {
            this.updateUsernameMessage.text = "Invalide username! input need to contain at least one upper and lower english letter";
          
        }

    }
    public void OnClickSaveUsername()
    {
        if (this.newUsername != null && this.username != null)
        {
        SocketReference.Emit("updateUsername", new JSONObject(JsonUtility.ToJson(new UpdateData(this.username,this.newUsername))));
        Debug.Log("update username");
        }
       
    }
    public void UpdateUsernameEnd(bool isErr,string msg,string name)
    {
        if (!isErr)
        {
            this.username = name;
            Text txt = ProfileContainer.transform.Find("[ProfileUser]").Find("AvatarNameContainer").Find("UserName-txt").GetComponent<Text>();
            updateUsernameMessage.color = Color.green;
            txt.text =name;
        }
        else
        {
            updateUsernameMessage.color = Color.red;
        }
        this.updateUsernameMessage.text = msg;
    }

    public void OnEditNewPassword(string text)
    {

        bool isValid = PasswordValidation(text, updatePasswordMessage);
        //if (isValid)
        //{
            this.newPassword = text;
            updatePasswordMessage.color = Color.green;
            this.updatePasswordMessage.text = "";
       // }
        //else { updatePasswordMessage.color = Color.red; }

    }
    public void OnComfirm(string pass)
    {
        if(string.Compare(newPassword, pass) == 0)
        {
            newPassword = pass;
            isPasswordEqual = true;
            updatePasswordMessage.text = "";
        }
        else
        {
            updatePasswordMessage.color = Color.red;
            updatePasswordMessage.text = "The password not equale";
            isPasswordEqual = false;

        }
    }
    public void OnClickUpdatePassword()
    {
        if (isPasswordEqual && newPassword!=null)
        {
            SocketReference.Emit("updatePassword", new JSONObject(JsonUtility.ToJson(new UpdateData(NetworkClient.users[NetworkClient.clientID].name, this.newPassword))));
            Debug.Log("update password");
        }
    }
    public void UpdatePasswordEnd(bool err, string msg)
    {
        if (!err)
        {
            this.updatePasswordMessage.color = Color.green;
        }
        else { this.updatePasswordMessage.color = Color.red; }
        this.updatePasswordMessage.text = msg;
    }
    public void OnClickSaveAvatar()
    {
        Toggle[] toggleGroup = GameObject.FindObjectsOfType<Toggle>();
        foreach (Toggle toggle in toggleGroup)
        {
           if(toggle.isOn == true)
            {
                string avatar = toggle.transform.Find("Avatar").Find("Image").GetComponent<Image>().sprite.name.ToString();
                SocketReference.Emit("updateAvatar", new JSONObject(JsonUtility.ToJson(new UpdateData(NetworkClient.users[NetworkClient.clientID].name, avatar))));
                Debug.Log("update avatar to ("+avatar+")");
            }
        }

    }
    public void UpdateAvatarEnd(bool isError, string msg)
    {
        
        if (!isError)
        {
            updateAvatardMessage.color = Color.green;
            isUpdateAvatar = true;
        }
        else
        {
            updateAvatardMessage.color = Color.red;
        }
        updateAvatardMessage.text = msg;
    }
    public void OnClickCloseEditProfile()
    {
        isClosedEditAvatar = true;
    }
    public void OnClickBackFromCreateGame()
    {
        NetworkClient.users[NetworkClient.clientID].socket.Emit("removePlayerFromLobby");
        GameLobby.isCloseInvitation = true;
    }
  public void OnClickBackFromEditUsername()
    {
        updateUsernameMessage.text = " ";
    }
    private void OnStateChange(SocketIOEvent e, string state, string sendername)
    {
        if (state == "")
        {
            state = e.data["state"].str;
        }

        switch (state)
        {
            case LobbyState.LOBBY:
                startGameContainer.SetActive(false);
                receiveInvitationContainer.SetActive(false);
                MainMenuContainer.SetActive(false);
                sendInvitationConatiner.SetActive(false);
                gameLobbyContainer.SetActive(true);
                UIContainer.SetActive(true);
                break;

            case LobbyState.UNCONECCTED_USERS:
                Debug.Log("UNCONECCTED_USERS");
                startGameContainer.SetActive(false);
                receiveInvitationContainer.SetActive(false);
                MainMenuContainer.SetActive(false);
                sendInvitationConatiner.SetActive(false);
                gameLobbyContainer.SetActive(false);
                UIContainer.SetActive(true);

                Text txt = sendInvitationConatiner.transform.Find("[Invitation]").Find("Text").GetComponent<Text>();
                txt.text = "There is no available players...";
                sendInvitationConatiner.SetActive(true);
                NetworkClient.users[NetworkClient.clientID].socket.Emit("invitationPlayerConnect");
                break;
            case LobbyState.SEND_INVITATION:

                playerVsPlayerContainer.SetActive(false);
                playerVsComputerContainer.SetActive(false);
                startGameContainer.SetActive(false);
                userUIContainer.SetActive(false);
                gameLobbyContainer.SetActive(false);
                MainMenuContainer.SetActive(false);
                signInContainer.SetActive(false);
                sendInvitationConatiner.SetActive(true);
                UIContainer.SetActive(true);
                Text invitationTxt = sendInvitationConatiner.transform.Find("[Invitation]").Find("Text").GetComponent<Text>();
                invitationTxt.text = "Add player to your game";


                break;
            case LobbyState.RECEIVE_INVITATION:
                receiveInvitationContainer.SetActive(true);
                UIContainer.SetActive(true);

                Debug.Log("ReceiveInvitation"+receiveInvitationContainer.name); 

                 Text name = receiveInvitationContainer.transform.Find("Message").Find("SenderName").GetComponent<Text>();
                Debug.Log("ReceiveInvitation text" + name.name);

                name.text = sendername.ToString();
                Text msg = receiveInvitationContainer.transform.Find("Message").Find("textMessage").GetComponent<Text>();
                msg.text = currentLevelGame== SceneList.MULTIPLAYER_LEVEL1? "invited you to multiplayer game level 1": "invited you to multiplayer game level 2";
                break;

            case LobbyState.RESULT_INVITATION:
                playerVsPlayerContainer.SetActive(false);
                playerVsComputerContainer.SetActive(false);
                startGameContainer.SetActive(false);
                userUIContainer.SetActive(false);
                gameLobbyContainer.SetActive(false);
                MainMenuContainer.SetActive(false);
                signInContainer.SetActive(false);
                UIContainer.SetActive(true);

                Text denymsg =sendInvitationConatiner.transform.Find("[Invitation]").Find("Text").GetComponent<Text>();
                denymsg.text = userInvitation + " deny your invitation game..";
                sendInvitationConatiner.SetActive(true);

                break;

            case LobbyState.GAME:
                UIContainer.SetActive(false);
                break;

            case LobbyState.GAME_OVER :
                UIContainer.SetActive(false);

                break;

            default:
                receiveInvitationContainer.SetActive(false);
                MainMenuContainer.SetActive(false);
                gameLobbyContainer.SetActive(false);
                startGameContainer.SetActive(false);
                sendInvitationConatiner.SetActive(false);


                break;

        }
    }
    public bool PasswordValidation(string text,Text meassage)
    {
        Regex hasNumber = new Regex(@"[0-9]+");
        Regex hasUpperChar = new Regex(@"[A-Z]+");
        Regex hasLowerChar = new Regex(@"[a-z]+");
        Regex hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
        Regex hasMinimum8Chars = new Regex(@".{8,}");


        if (!hasLowerChar.IsMatch(text))
        {
            meassage.text = "Password should contain at least one lower case letter";
            return false;

        }
        else if (!hasUpperChar.IsMatch(text))
        {
            meassage.text = "Password should contain at least one upper case letter";
            return false;

        }
        else if (!hasNumber.IsMatch(text))
        {
            meassage.text = "Password should contain at least one numeric value";
            return false;

        }

        else if (!hasSymbols.IsMatch(text))
        {
            meassage.text = "Password should contain at least one special case characters";
            return false;


        }
        else if (!hasMinimum8Chars.IsMatch(text))
        {
            meassage.text = "Password should not be less than than 8 characters";
            return false;
        }
        return true;
    }
    public bool UsernameValidation(string text)
    {
        string nameRegex = "([A-Za-z]+[0-9_]*)";
        Match match = Regex.Match(text, nameRegex);

        if (match.Success)
        {
            return true;

        }
        return false;
    }
 public void OnClickDownloadBarcode()
    {
        Application.OpenURL("https://drive.google.com/file/d/1Hk4U5ByF1hwAxFQqjMylqH9YaEjvAMWZ/view?usp=sharing");
    }
    public void OnClicDemoVideo()
    {
        Application.OpenURL("https://www.youtube.com/watch?v=BYNLeMKKpgo&feature=youtu.be");
    }
    [Serializable]
    public class CurrentLevelGame
    {
        public string currentLevel;
        public CurrentLevelGame(string current)
        {
            this.currentLevel = current;
        }
    }

        [Serializable]
    public class SignInData
    {
        public string username;
        public string password;
        public SignInData(string name, string password)
        {
            this.username = name;
            this.password = password;
        }
    }
    [Serializable]
    public class UpdateData
    {
        public string oldData;
        public string newData;
        public UpdateData(string oldData, string newData)
        {
            this.oldData = oldData;
            this.newData = newData;
        }
    }
}