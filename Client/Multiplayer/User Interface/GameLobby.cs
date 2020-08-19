using SocketIO;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameLobby : MonoBehaviour
{

    [SerializeField]
    private GameObject PlayersInvitationConatiner;
    [SerializeField]
    private GameObject resultGameConatiner;
    [SerializeField]
    private GameObject playerLeftGameConatiner;



    public static Action<SocketIOEvent, string, string> OnStateChange;
    public static bool isClosedGameLooby,isCloseInvitation;
    

    void Start()
    {
        isClosedGameLooby = false;
        isCloseInvitation = false;
        SceneManagementManager.Instance.LoadLevel(SceneList.MAIN_MENU, (levelName) =>
        {
            NetworkClient.OnGameStateChange += OnGameStateChange; //all the subscribes do OnGameStateChange function
                                                                  //Initial turn off screnns
                                                                  //sendInvitationConatiner.SetActive(false);
            PlayersInvitationConatiner.SetActive(false);
            resultGameConatiner.SetActive(false);
            playerLeftGameConatiner.SetActive(false);

        });
    }
    private void Update()
    {
        if (isCloseInvitation)
        {
            PlayersInvitationConatiner.SetActive(false);
        }
        if (MainMenuManager.currentLevelGame != null)
        {
            if (MainMenuManager.currentLevelGame.Equals(SceneList.PLAYER_VS_COMPUTER_LEVEL1) || MainMenuManager.currentLevelGame.Equals(SceneList.PLAYER_VS_COMPUTER_LEVEL2))
            {
                if (LobbyState.currentState != null)
                {
                    if (LobbyState.currentState.Equals(LobbyState.GAME))
                    {
                        resultGameConatiner.SetActive(false);
                    }
                        if (LobbyState.currentState.Equals(LobbyState.GAME_OVER))
                    {

                        playerLeftGameConatiner.SetActive(false);
                        PlayersInvitationConatiner.SetActive(false);
                        resultGameConatiner.SetActive(true);
                        resultGameConatiner.transform.Find("Background-Img").GetComponent<Image>().enabled = true;
                        resultGameConatiner.transform.Find("[ResultGame-UI]").Find("GameOver-Text").GetComponent<Text>().enabled = true;
                        resultGameConatiner.transform.Find("[ResultGame-UI]").Find("Button").GetComponent<Button>().gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    public void onCloseLobbyGame()
    {
        isClosedGameLooby = true;
        GameObject.FindGameObjectWithTag("camera").SetActive(true);
        if (MainMenuManager.currentLevelGame.Equals(SceneList.MULTIPLAYER_LEVEL1))
        {
            SceneManagementManager.Instance.UnLoadLevel(SceneList.MULTIPLAYER_LEVEL1);

        }
        else if (MainMenuManager.currentLevelGame.Equals(SceneList.MULTIPLAYER_LEVEL2))
        {
            SceneManagementManager.Instance.UnLoadLevel(SceneList.MULTIPLAYER_LEVEL2);
        }
        else if (MainMenuManager.currentLevelGame.Equals(SceneList.PLAYER_VS_COMPUTER_LEVEL1))
        {
            SceneManagementManager.Instance.UnLoadLevel(SceneList.PLAYER_VS_COMPUTER_LEVEL1);

        }
        else if(MainMenuManager.currentLevelGame.Equals(SceneList.PLAYER_VS_COMPUTER_LEVEL2))
        {
            SceneManagementManager.Instance.UnLoadLevel(SceneList.PLAYER_VS_COMPUTER_LEVEL2);

        }
    }
        public void onClickBack()
    {
        isClosedGameLooby = true;
        ResultGame resultGame = new ResultGame();
        resultGame.winnerId = NetworkClient.clientID;
        NetworkClient.users[NetworkClient.clientID].socket.Emit("gameOver", new JSONObject(JsonUtility.ToJson(resultGame)));
        isClosedGameLooby = true;
        Debug.Log("isclosed=" + isClosedGameLooby);
        GameObject.FindGameObjectWithTag("camera").SetActive(true);
        if (MainMenuManager.currentLevelGame == SceneList.MULTIPLAYER_LEVEL1)
        {
            SceneManagementManager.Instance.UnLoadLevel(SceneList.MULTIPLAYER_LEVEL1);
        }else if(MainMenuManager.currentLevelGame == SceneList.MULTIPLAYER_LEVEL2)
        {
            SceneManagementManager.Instance.UnLoadLevel(SceneList.MULTIPLAYER_LEVEL2);
        }
    }
    public void onClickNewGame()
    {
        NetworkClient.users[NetworkClient.clientID].socket.Emit("newGame");
        if(MainMenuManager.currentLevelGame == SceneList.MULTIPLAYER_LEVEL1)
        {
            SceneManagementManager.Instance.UnLoadLevel(SceneList.MULTIPLAYER_LEVEL1);

        }
        else if (MainMenuManager.currentLevelGame == SceneList.MULTIPLAYER_LEVEL2)
        {
            SceneManagementManager.Instance.UnLoadLevel(SceneList.MULTIPLAYER_LEVEL2);

        }
    }
    private void OnGameStateChange(SocketIOEvent e, string state,string username)
    {
        if (state == "")
        {
            state = e.data["state"].str;
        }
        switch (state)
        {


            case LobbyState.SEND_INVITATION :
                PlayersInvitationConatiner.SetActive(true);
                resultGameConatiner.SetActive(false);
                playerLeftGameConatiner.SetActive(false);
                break;


            case LobbyState.RESULT_INVITATION:
                PlayersInvitationConatiner.SetActive(true);
                resultGameConatiner.SetActive(false);
                playerLeftGameConatiner.SetActive(false);

                break;

            case LobbyState.GAME:
                playerLeftGameConatiner.SetActive(false);
                PlayersInvitationConatiner.SetActive(false);
                resultGameConatiner.SetActive(true);
                resultGameConatiner.transform.Find("Background-Img").GetComponent<Image>().enabled = false;
                resultGameConatiner.transform.Find("[ResultGame-UI]").Find("GameOver-Text").GetComponent<Text>().enabled = false;
                resultGameConatiner.transform.Find("[ResultGame-UI]").Find("Button").GetComponent<Button>().gameObject.SetActive(false);
                resultGameConatiner.transform.Find("[ResultGame-UI]").Find("ResultPlayer1-Text").GetComponent<Text>().text = "";
                resultGameConatiner.transform.Find("[ResultGame-UI]").Find("ResultPlayer2-Text").GetComponent<Text>().text = "";
                resultGameConatiner.transform.Find("[ResultGame-UI]").Find("Result-Text").GetComponent<Text>().text = "";
                break;

            case LobbyState.PLAYER_LEFT_GAME:
                LobbyGameManager.isStartGame = false;
                PlayersInvitationConatiner.SetActive(false);
                resultGameConatiner.SetActive(false);
                playerLeftGameConatiner.SetActive(true);
                Debug.Log("username=" + username);
                Text name = GameObject.FindGameObjectWithTag("leftGamePlayerName").GetComponent<Text>();
                Debug.Log("name txt=" + name.name);
                name.text = username.ToString();
                break;

            case LobbyState.GAME_OVER:
                playerLeftGameConatiner.SetActive(false);
                PlayersInvitationConatiner.SetActive(false);
                resultGameConatiner.SetActive(true);
                resultGameConatiner.transform.Find("Background-Img").GetComponent<Image>().enabled = true;
                resultGameConatiner.transform.Find("[ResultGame-UI]").Find("GameOver-Text").GetComponent<Text>().enabled = true;
                resultGameConatiner.transform.Find("[ResultGame-UI]").Find("Button").GetComponent<Button>().gameObject.SetActive(true);
                break;

            default:
                PlayersInvitationConatiner.SetActive(false);
                resultGameConatiner.SetActive(false);
                playerLeftGameConatiner.SetActive(false);
                break;

        }
    }
}