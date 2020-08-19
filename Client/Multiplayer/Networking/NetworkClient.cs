using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

/* connection to io socket and get event from the server */
public class NetworkClient : SocketIOComponent
{
    public static LobbyGameManager lobbyGameManager { get; private set; }
    public static FixedJoystick jostick { get; private set; }
    public static Button[] btns{ get; private set; }
    public static string clientID { get; private set; }
    public static Action<SocketIOEvent, string,string> OnGameStateChange, OnStateChange;
    public static Action<bool, string, string> UpdateUsernameEnd;
    public static Action<bool,string> UpdatePasswordEnd;
    public static Action<bool,string> UpdateAvatarEnd;
    public static Action OnSignInComplete = () => { };
    public static Action<bool, string> OnSignInUpMessage;
    public static Action<string,float> GoalScore;
    public static Dictionary<string, User> users;
    public static ObjectNetwork ballObject;
    public static  GameObject ballObj, resultGameContainer,goalUi;
    public static Vector3 oldBallPosition, oldPlayer1Position, oldPlayer2Position;
    public static Quaternion oldPlayer1Rotation, oldPlayer2Rotation;
    [Header("NetworkClient")]
  
    [SerializeField]
    private GameObject  GoalUI;
    private GameObject lobbyGameManagerContainer;
    public static Dictionary<string, NetworkIdentity> playersObjects;
    [SerializeField]
    public static Dictionary<string, ObjectNetwork> gateObjects;
    [SerializeField]
    private GameObject Resultcontainer,playerPrefab1, playerPrefab2, ballPrefab, beachBallPrefab, gatePrefab1, gatePrefab2, jostickPrefab, scoreUiPrefab, invitationPrefab, invitationContainer,timerUiPrefab;
    [SerializeField]
    private Sprite spriteAddPlayerBtn;
    private GameObject scoreUi ;
    private Avatar avatar;
  

    public override void Start()
    {
        base.Start();
        Initialize();
        SetupEvents();
    }

    private void Initialize()
    {
        playersObjects = new Dictionary<string, NetworkIdentity>();
        gateObjects = new Dictionary<string, ObjectNetwork>();
        users = new Dictionary<string, User>();
        avatar = new Avatar();
        resultGameContainer = Resultcontainer;
        goalUi = GoalUI;
    }

    private void SetupEvents()
    {
        On("open", (e) =>
        {
            Debug.Log("Connection made to the server");
        });

        On("register", (e) =>
        {
            clientID = e.data["id"].ToString().RemoveQuotes();
            Debug.LogFormat("our client id ({0})", clientID);

        });

        On("addPlayer", (e) =>
        {
            Debug.Log("add player serverobj=" + MultiplayerManager.serverObject.transform);

            Vector3 playersScale = new Vector3(0.9f, 0.9f, 0.9f);
            Vector3 currentPositionPlayer1 = MainMenuManager.currentLevelGame == SceneList.MULTIPLAYER_LEVEL1 ? new Vector3(2f, 0.3f, 1.8f):new Vector3(-1.07f,0.73f,1.27f);
            Quaternion currentRotationPlayer1 = Quaternion.Euler(0, -180f, 0);
            Vector3 currentPositionPlayer2= MainMenuManager.currentLevelGame == SceneList.MULTIPLAYER_LEVEL1 ? new Vector3(2f, 0.3f, -2.5f):new Vector3(-1.27f,0.73f,-4.22f);
            Quaternion currentRotationPlayer2 = Quaternion.Euler(0, 0, 0);

            if (LobbyState.cameraState.Equals(LobbyState.AR_OFF))
            {
                currentPositionPlayer1 = oldPlayer1Position;
                currentPositionPlayer2 = oldPlayer2Position;
                currentRotationPlayer1 = oldPlayer1Rotation;
                currentRotationPlayer2 = oldPlayer2Rotation;

            }
            string id = e.data["id"].ToString().RemoveQuotes();
           
            GameObject obj = Instantiate(playersObjects.Count == 0 ? playerPrefab1 : playerPrefab2, MultiplayerManager.serverObject.transform);
            obj.name = string.Format("Player({0})", id);
            NetworkIdentity netIdentity = obj.GetComponent<NetworkIdentity>();
            netIdentity.SetControllerID(id);
            Debug.Log("id controlling" + netIdentity.GetID());
            netIdentity.SetSocketReference(this);
            obj.transform.SetParent(MultiplayerManager.serverObject.transform);
            if (netIdentity.IsControlling())
            {
                GameObject j = Instantiate(jostickPrefab, MultiplayerManager.serverObject.transform);
                j.name = string.Format("Jostick({0})", id);
                jostick = j.GetComponentInChildren<FixedJoystick>();
                btns = j.GetComponentsInChildren<Button>();

                if (!LobbyState.cameraState.Equals(LobbyState.AR_OFF))
                {
                    Debug.Log("game lobby");
                    lobbyGameManagerContainer = GameObject.FindGameObjectWithTag("GameLobbyManager");
                    lobbyGameManager = lobbyGameManagerContainer.AddComponent<LobbyGameManager>() as LobbyGameManager;

                }
            }
            if(playersObjects.Count == 0)//player1
            {
                netIdentity.transform.localPosition = currentPositionPlayer1;
                netIdentity.transform.localRotation = currentRotationPlayer1;
                netIdentity.transform.localScale = playersScale;
                Debug.Log("player 1 position=" + currentPositionPlayer1);

            }
            else//player2
            {
                netIdentity.transform.localPosition = currentPositionPlayer2;
                netIdentity.transform.localRotation = currentRotationPlayer2;
                netIdentity.transform.localScale = playersScale;
            }
            playersObjects.Add(id, netIdentity);

        });
        On("addBall", (e) =>
        {
            Vector3 currentBallPosition= MainMenuManager.currentLevelGame == SceneList.MULTIPLAYER_LEVEL1? new Vector3(2f, 3f, -0.5f): new Vector3(-1.11f,2.5f,-1.62f);
            Vector3 ballScale = MainMenuManager.currentLevelGame == SceneList.MULTIPLAYER_LEVEL1 ? new Vector3(2.4f, 2.4f, 2.4f):new Vector3(0.32f,0.32f,0.32f);
            Quaternion ballRotation = Quaternion.Euler(0, 0f, 0);
            Debug.Log("ball camera state=" + LobbyState.cameraState);
            if (LobbyState.cameraState.Equals(LobbyState.AR_OFF))
            {
                currentBallPosition = oldBallPosition;

            }
            string id = e.data["id"].ToString().RemoveQuotes();
            ballObj= ballPrefab;
            if (MainMenuManager.currentLevelGame== SceneList.MULTIPLAYER_LEVEL1)
            {
                ballObj = ballPrefab;
            }else if (MainMenuManager.currentLevelGame == SceneList.MULTIPLAYER_LEVEL2)
            {
                ballObj = beachBallPrefab;
            }
            GameObject obj = Instantiate(ballObj, MultiplayerManager.serverObject.transform);
            obj.name = string.Format("Ball({0})", id);
            ObjectNetwork netIdentity = obj.GetComponent<ObjectNetwork>();
            obj.transform.SetParent(MultiplayerManager.serverObject.transform);
            netIdentity.transform.localPosition = currentBallPosition;
            netIdentity.transform.localScale = ballScale;
            netIdentity.transform.localRotation = ballRotation;
            netIdentity.SetControllerID(id);
            netIdentity.SetSocketReference(this);
            ballObject = netIdentity;
            PlayerManager.SetBall(ballObject);
            Debug.Log("add ball");
           

        });
        On("addGate", (e) =>
        {

            Vector3 gatesScale = MainMenuManager.currentLevelGame == SceneList.MULTIPLAYER_LEVEL1 ?new Vector3(1.32f, 0.87f, 1f):new Vector3(1.22f,0.35f,0.8f);
            Vector3 currentPositionGate1 = MainMenuManager.currentLevelGame == SceneList.MULTIPLAYER_LEVEL1 ? new Vector3(1.8f, 0.29f, 10.4f):new Vector3(-1f,0.72f,14.3f);
            Quaternion currentRotationGate1 = Quaternion.Euler(0, 90, 0);
            Vector3 currentPositionGate2 = MainMenuManager.currentLevelGame == SceneList.MULTIPLAYER_LEVEL1 ? new Vector3(1.8f, 0.29f, -11f) :new Vector3(-0.98f,0.72f,-16f);
            Quaternion currentRotationGate2 = Quaternion.Euler(0, -90, 0);

            Gate gate = new Gate();
            string id = e.data["id"].ToString().RemoveQuotes();
            GameObject obj = Instantiate(gateObjects.Count == 0 ? gatePrefab1 : gatePrefab2, MultiplayerManager.serverObject.transform);
            obj.name = string.Format("Gate({0})", id);
            ObjectNetwork netIdentity = obj.GetComponent<ObjectNetwork>();
            netIdentity.SetControllerID(id);
            netIdentity.SetSocketReference(this);
            obj.transform.SetParent(MultiplayerManager.serverObject.transform);

            if (gateObjects.Count == 0)//gate 1
            {
                obj.transform.localPosition = currentPositionGate1;
                obj.transform.localRotation = currentRotationGate1;
                obj.transform.localScale = gatesScale;

            }
            else//gate 2 
            {
                obj.transform.localPosition = currentPositionGate2;
                obj.transform.localRotation = currentRotationGate2;
                obj.transform.localScale = gatesScale;
                lobbyGameManager.SetBallNet(netIdentity);
                lobbyGameManager.SetPlayers(playersObjects);
                lobbyGameManager.InitGame(resultGameContainer, scoreUi, MultiplayerManager.serverObject.transform);

                

            }
            gateObjects.Add(id, netIdentity);
            Debug.Log("add gate ( " + id + ") to GameLobby");
        });

        On("addScoreUI", (e) => {

            if (playersObjects[clientID].IsControlling())
            {
                scoreUi = Instantiate(scoreUiPrefab, MultiplayerManager.serverObject.transform);
                scoreUi.name = string.Format("ScoreUI({0})", clientID);
                scoreUi.transform.SetParent(MultiplayerManager.serverObject.transform);
                Image avatarPlayer1 = scoreUi.transform.Find("ScoreUI").Find("BackgroundPlayer1").Find("AvatarPlayer1").Find("Image1").GetComponent<Image>();
                Image avatarPlayer2 = scoreUi.transform.Find("ScoreUI").Find("BackgroundPlayer2").Find("AvatarPlayer2").Find("Image2").GetComponent<Image>();
                string avatar1 = users[playersObjects.Keys.ElementAt(0)].avatar;
                string avatar2 = users[playersObjects.Keys.ElementAt(1)].avatar;
                avatar.DisplayAvatar(avatarPlayer1, avatar1);
                avatar.DisplayAvatar(avatarPlayer2, avatar2);

            }
        });

        On("updateAnim", (e) =>
        {
            string id = e.data["id"].ToString().RemoveQuotes();
            string anim = e.data["anim"].ToString().RemoveQuotes();
           
            NetworkIdentity netIdentity = playersObjects[id];
            netIdentity.transform.GetComponent<Animator>().SetTrigger(anim);
        });
         On("updatePosition", (e) =>
        {
            if (LobbyState.currentState.Equals(LobbyState.GAME))
            {
              //  Debug.Log("in update positin");
                string id = e.data["id"].ToString().RemoveQuotes();

                float x1 = e.data["rotation"]["x"].f;
                float y1 = e.data["rotation"]["y"].f;
                float z1 = e.data["rotation"]["z"].f;

                float x2 = e.data["position"]["x"].f;
                float y2 = e.data["position"]["y"].f;
                float z2 = e.data["position"]["z"].f;


                NetworkIdentity netIdentity = playersObjects[id];
                netIdentity.transform.localEulerAngles = new Vector3(x1, y1, z1);
                netIdentity.transform.localPosition = new Vector3(x2, y2, z2);

                //     Debug.Log("recive position (" + x2 + "," + y2 +","+ z2 + ")");

            }
        });
      

        On("updateBallPosition", (e) =>
        {
          

                string id = e.data["id"].ToString().RemoveQuotes();
                float x2 = e.data["position"]["x"].f;
                float y2 = e.data["position"]["y"].f;
                float z2 = e.data["position"]["z"].f;


              //  Debug.Log("player id=" + id + "recive ball position (" + x2 + ", " + y2 + "," + z2 + ")");
                ObjectNetwork netIdentity = ballObject;
                netIdentity.transform.localPosition = new Vector3(x2, y2, z2);
           

        });
        On("playerKickBall", (e) =>
        {
            Debug.Log("master not" );
            KickBall kickBall = new KickBall();
            kickBall.direction = new Position();
            kickBall.ballID = e.data["ballID"].ToString().RemoveQuotes();
            kickBall.direction.x = e.data["direction"]["x"].f;
            kickBall.direction.y = e.data["direction"]["y"].f;
            kickBall.direction.z = e.data["direction"]["z"].f;
            kickBall.force = e.data["force"].f;
            Debug.Log("not masterr =(" + kickBall.direction.x + "," + kickBall.direction.y + "," + kickBall.direction.z + ")");
            Vector3 direction = new Vector3(kickBall.direction.x, kickBall.direction.y, kickBall.direction.z);
            ballObject.GetComponent<Rigidbody>().velocity = direction.normalized * kickBall.force;
            Debug.Log("not masterr =(" + kickBall.direction.x + "," + kickBall.direction.y + "," + kickBall.direction.z + ")");

        });

        On("loadGame", (e) =>
        {
            Debug.Log("Loading game level "+ MainMenuManager.currentLevelGame);
            if(MainMenuManager.currentLevelGame == SceneList.TEST)
            {
                SceneManagementManager.Instance.LoadLevel(SceneList.TEST, (levelName) =>
                {
                    invitationContainer.SetActive(false);
                });
            }
            if (MainMenuManager.currentLevelGame == SceneList.MULTIPLAYER_LEVEL1)
            {
                SceneManagementManager.Instance.LoadLevel(SceneList.MULTIPLAYER_LEVEL1, (levelName) =>
                {
                    invitationContainer.SetActive(false);
                });
            }
            else if (MainMenuManager.currentLevelGame == SceneList.MULTIPLAYER_LEVEL2)
            {
                SceneManagementManager.Instance.LoadLevel(SceneList.MULTIPLAYER_LEVEL2, (levelName) =>
                {
                    invitationContainer.SetActive(false);
                });
            }

           

        });
        On("allPlayersLoadedLevelGame", (e) => {
            SceneManagementManager.isAllPlayersLoadedLevel = true;
        });
        On("allPlayersDetectedMarker", (e) =>
        {
            Debug.Log("All players detected marker");
            MultiplayerManager.isAllPlayersDetectedMarker = true;

        });

        On("lobbyUpdate", (e) =>
        {
            string state = e.data["state"].ToString().RemoveQuotes();
            LobbyState.currentState = state;
            Debug.Log("lobby update:" + state);
            if (state.Equals(LobbyState.PLAYER_LEFT_GAME) )
            {
                DestroyServerObject();
                string name = e.data["name"].ToString().RemoveQuotes();
                OnGameStateChange.Invoke(e, "",name);
                OnStateChange.Invoke(e, "", "");
                Debug.Log("update state:" + state);
            }
            else
            {
                if (state.Equals(LobbyState.GAME_OVER))
                {
                    DestroyServerObject();

                    Emit("removePlayerFromLobby");
                }
              
                OnGameStateChange.Invoke(e, "","");
                OnStateChange.Invoke(e, "", "");
            }
        });
       
        On("updateTurn", (e) =>
        {
            string playerTurn = e.data["playerTurn"].ToString().RemoveQuotes();
            lobbyGameManager.UpdateTurn(playerTurn);

        });
        On("goalUI", (e) =>
        {
            string player = e.data["player"].ToString().RemoveQuotes();
            float score = e.data["score"].f;

             Debug.Log("scoreUi ui player =" + player);
             Debug.Log("score="+score);
             //pause game
            LobbyGameManager.isStartGame = false;
            GoalUI.SetActive(true);
            StartCoroutine(Wait(player,score));//wait 1 seconds
        });
        On("updateScore", (e) =>
        {
            string player = e.data["player"].ToString().RemoveQuotes();
            float score = e.data["score"].f;
            lobbyGameManager.UpdateScore(player, score);

        });
        On("initPosition", (e) =>
        {
            if (LobbyState.currentState != LobbyState.GAME_OVER)
            {
                lobbyGameManager.InitPosition();


            }
        });
        On("initUsers", (e) => {
            users.Clear();
        });
        On("saveUser", (e) => {
            User user = new User();
            user.id = e.data["id"].ToString().RemoveQuotes();
            user.name = e.data["username"].ToString().RemoveQuotes();
            user.avatar = e.data["avatar"].ToString().RemoveQuotes();
            user.played = e.data["played"].f;
            user.won = e.data["won"].f;
            user.socket = this;
            user.lobby = 0;
            users.Add(user.id, user);

            Debug.Log("Player loged in (  id: " + user.id+ ", name: " + user.name + ", played= "+user.played+")");
            if (LobbyState.currentState == LobbyState.UNCONECCTED_USERS)
            {
                Emit("invitationAddPlayer");
            }
           
        });
        On("signInUpMessage", (e) =>
         {
             Debug.Log("signInUpMessage");
             bool result = e.data["result"].b;
             string message = e.data["message"].ToString().RemoveQuotes();
             Debug.Log("result="+result+"mess="+message);
             OnSignInUpMessage.Invoke(result, message);
         });
        On("signIn", (e) => {
            OnSignInComplete.Invoke();
        });
        On("invitationInit", (e) =>
        {
            GameObject[] invitations = invitationContainer.GetComponents<GameObject>();
            foreach (GameObject obj in invitations)
            {
                Destroy(obj);
            }

        });
        On("removeInvitation", (e) =>
         {
             Debug.Log("removeInvitation");
             string id = e.data["id"].ToString().RemoveQuotes();
             string name = e.data["name"].ToString().RemoveQuotes();
             users.Remove(id);
             Destroy(MainMenuManager.allInvitationObj[id]);
             MainMenuManager.allInvitationObj.Remove(id);
             if (MainMenuManager.allInvitationObj.Count() == 0)
             {
                 Debug.Log("count=" + MainMenuManager.allInvitationObj.Count());

                 Emit("lobbyUpdate");
                 //OnStateChange.Invoke(e, LobbyState.UNCONECCTED_USERS, "");
             }
         });
        On("invitationPlayer", (e) =>
        {
            Debug.Log("invitationPlayer");
            OnStateChange(e, LobbyState.SEND_INVITATION, "");
            User player = new User();
            player.id = e.data["id"].ToString().RemoveQuotes();
            player.name = e.data["name"].ToString().RemoveQuotes();
            users[player.id].lobby = player.lobby;
            MainMenuManager.OnSendInvitation(player, invitationPrefab, invitationContainer.transform);

        });
        On("receiveInvitation", (e) =>
        {
            float lobby = e.data["lobby"].f;
            string sendername = e.data["sendername"].ToString().RemoveQuotes();
            string level = e.data["level"].ToString().RemoveQuotes();
            users[clientID].lobby = lobby;
            Debug.Log("Recive invitation to join lobby: " + lobby+","+level);
            MainMenuManager.currentLevelGame = level;
            OnStateChange(e, LobbyState.RECEIVE_INVITATION, sendername);
            Debug.Log("state:" + e.data["state"].ToString());

        });
        On("denyInvitation", (e) =>
        {
            LobbyState.isdeny = true;
            string username = e.data["username"].ToString().RemoveQuotes();
            MainMenuManager.userInvitation = username;
            username = "InvitationPlayer(" + username + ")";
            Transform invitationObject = invitationContainer.transform.Find(username);
          

        });
        On("updateUsername", (e) =>
         {
             bool isError = e.data["err"].b;
             string msg = e.data["msg"].ToString().RemoveQuotes();
             string name = e.data["name"].ToString().RemoveQuotes();
             if (!isError)
             {
                 users[clientID].name = name;
             }
             UpdateUsernameEnd.Invoke(isError,msg,name);
         });
        On("updatePassword", (e) =>
         {
             bool isError = e.data["err"].b;
             string msg = e.data["msg"].ToString().RemoveQuotes();
            
             UpdatePasswordEnd.Invoke(isError,msg);
         });
        On("updateAvatar", (e) =>
        {
            bool isError = e.data["err"].b;
            string msg = e.data["msg"].ToString().RemoveQuotes();
            string avatar = e.data["avatar"].ToString().RemoveQuotes();
            if (!isError)
            {
                users[clientID].avatar = avatar;
            }
            UpdateAvatarEnd.Invoke(isError,msg);
        });
        On("disconnected", (e) =>
        {
            string id = e.data["id"].ToString().RemoveQuotes();
            GameObject obj = playersObjects[id].gameObject;
            Destroy(obj);  //remove from game
            playersObjects.Remove(id); //remove from memory
        });
    }
    public static void DestroyServerObject()
    {
        if (LobbyState.cameraState.Equals(LobbyState.AR_OFF))
        {
            oldBallPosition = ballObject.transform.localPosition;
            oldPlayer1Position = playersObjects.ElementAt(0).Value.transform.localPosition;
            oldPlayer2Position = playersObjects.ElementAt(1).Value.transform.localPosition;
            oldPlayer1Rotation = playersObjects.ElementAt(0).Value.transform.localRotation;
            oldPlayer2Rotation = playersObjects.ElementAt(1).Value.transform.localRotation;
        }

            Transform[] obj = MultiplayerManager.serverObject.GetComponentsInChildren<Transform>();
        for (var i = 1; i < obj.Length; i++)
        {
            GameObject.Destroy(obj[i].gameObject);

        }
        playersObjects = new Dictionary<string, NetworkIdentity>();
        gateObjects = new Dictionary<string, ObjectNetwork>();
        Debug.Log("camera state=" + LobbyState.cameraState);
        if (!(LobbyState.cameraState.Equals(LobbyState.AR_OFF)))
        {
            Destroy(lobbyGameManager);

        }
    }
    IEnumerator Wait(string player,float updateScore)
    {
        
        yield return new WaitForSeconds(2);
        GoalUI.SetActive(false);
        LobbyGameManager.isStartGame = true;
        Debug.Log("wait player=" + player);
        GoalScore.Invoke(player, updateScore);
    }
}
[Serializable]
public class Player
{
    public string id;
    public Position position;
    public Position rotation;
}
[Serializable]
public class User
{
    public string id;
    public string name;
    public string avatar;
    public float played;
    public float won;
    public float lobby;
    public SocketIOComponent socket;

}
[Serializable]
public class Ball
{
    public string id;
    public Position position;
}
[Serializable]
public class KickBall
{
    public string ballID;
    public Position direction;
    public float force;
}

[Serializable]
public class Position
{
    public float x;
    public float y;
    public float z;
}
[Serializable]
public class Turn
{
    public string playerTurn;
    public string sendId;
}
public class Score
{
    public string player;
    public float score;
}
[Serializable]
public class Gate
{
    public string id;
    public string playerId;
}

public class LobbyState
{
    public const string DETECTED_MARKER = "DetectedMarker";
    public const string GAME = "Game";
    public const string START_GAME="StartGame";
    public const string LOBBY = "Lobby";
    public const string SEND_INVITATION = "SendInvitation";
    public const string RECEIVE_INVITATION = "ReceiveInvitation";
    public const string RESULT_INVITATION = "ResultInvitation";
    public const string DENY_INVITATION = "DenyInvitation";
    public const string UNCONECCTED_USERS = "UnconnectedUsers";
    public const string GAME_OVER = "GameOver";
    public const string PLAYER_LEFT_GAME = "PlayerLeftGame";
    public const string AR_ON = "ArOn";
    public const string AR_OFF = "ArOff";
    public static string currentState;
    public static string cameraState=GAME;
    public static bool isdeny;
}
