using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGameMangaer : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;
    private bool isShowMenu;
    // Start is called before the first frame update
    void Start()
    {
        isShowMenu = false;
        menu.SetActive(isShowMenu);

    }

    public void onClickMenuGame()
    {

        isShowMenu = isShowMenu == true ? false : true;
        menu.SetActive(isShowMenu);
    }

    public void onClickLeaveGame()
    {

        switch (MainMenuManager.currentLevelGame)
        {
            case SceneList.MULTIPLAYER_LEVEL1:
                MultiplayerManager.maincamera.enabled = true;
                NetworkClient.users[NetworkClient.clientID].socket.Emit("playerLeaveGame");
                GameLobby.isClosedGameLooby = true;
                SceneManagementManager.Instance.UnLoadLevel(SceneList.MULTIPLAYER_LEVEL1);
                break;
            case SceneList.MULTIPLAYER_LEVEL2:
                MultiplayerManager.maincamera.enabled = true;
                NetworkClient.users[NetworkClient.clientID].socket.Emit("playerLeaveGame");
                GameLobby.isClosedGameLooby = true;
                SceneManagementManager.Instance.UnLoadLevel(SceneList.MULTIPLAYER_LEVEL2);
                break;
            case SceneList.PLAYER_VS_COMPUTER_LEVEL1:
                GameLobby.isClosedGameLooby = true;
                GameController.maincamera.enabled = true;
                LobbyState.currentState = LobbyState.GAME_OVER;
                SceneManagementManager.Instance.UnLoadLevel(SceneList.PLAYER_VS_COMPUTER_LEVEL1);
                break;
            case SceneList.PLAYER_VS_COMPUTER_LEVEL2:
                GameController.maincamera.enabled = true;
                GameLobby.isClosedGameLooby = true;
                LobbyState.currentState = LobbyState.GAME_OVER;
                SceneManagementManager.Instance.UnLoadLevel(SceneList.PLAYER_VS_COMPUTER_LEVEL2);
                break;
        }
      
    }
}
