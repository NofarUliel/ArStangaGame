using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneManagementManager : Singleton<SceneManagementManager>
{
    public static bool isAllPlayersLoadedLevel;
    private List<LevelLoadingData> levelsLoading;
    private List<string> currentlyLoadedScenes;
    [SerializeField]
    private GameObject loadingScreen;
    private bool isSendLoadedLevel;

    public override void Awake()
    {
        base.Awake();
        levelsLoading = new List<LevelLoadingData>();
        currentlyLoadedScenes = new List<string>();
        isAllPlayersLoadedLevel = false;
        isSendLoadedLevel = false;
    }

    public void Update()
    {
        for (int i = levelsLoading.Count - 1; i >= 0; i--)
        {
            if (levelsLoading[i] == null)
            {
                levelsLoading.RemoveAt(i);
                continue;
            }

            if (levelsLoading[i].ao.isDone)
            {
                //Multiplayer Game start after all players loaded gamelevel
                if (levelsLoading[i].sceneName == SceneList.MULTIPLAYER_LEVEL1 || levelsLoading[i].sceneName == SceneList.MULTIPLAYER_LEVEL2)
                {
                    if (!isSendLoadedLevel)
                    {
                        User user = NetworkClient.users[NetworkClient.clientID];
                        user.socket.Emit("loadedLevelGame", new JSONObject(JsonUtility.ToJson(user)));
                        isSendLoadedLevel = true;
                        Debug.Log("send loaded level");
                    }

                    if (isAllPlayersLoadedLevel)
                    {
                        ShowLevel(i);
                        isSendLoadedLevel = false;
                        isAllPlayersLoadedLevel = false;
                       // LobbyGameManager.SetStartGame(true);
                        Debug.Log("All players loaded level");

                    }
                }
                else
                {

                    ShowLevel(i);

                }

            }

        }
    }
    public void ShowLevel(int i)
    {

        levelsLoading[i].ao.allowSceneActivation = true; //Needed to make sure the scene while fully loaded gets turned on for the player
        levelsLoading[i].onLevelLoaded.Invoke(levelsLoading[i].sceneName);
        currentlyLoadedScenes.Add(levelsLoading[i].sceneName);
        levelsLoading.RemoveAt(i);
        //Hide loading screen 
        loadingScreen.SetActive(false);
        isAllPlayersLoadedLevel = false;

    }
    public void LoadLevel(string levelName, Action<string> onLevelLoaded, bool isShowingLoadingScreen = true)
    {
        bool value = currentlyLoadedScenes.Any(x => x == levelName);

        if (value)
        {
            Debug.LogFormat("Current level ({0}) is already loaded into the game.", levelName);
            return;
        }

        LevelLoadingData lld = new LevelLoadingData();
        lld.ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        lld.sceneName = levelName;
        lld.onLevelLoaded = onLevelLoaded;
        lld.isShowingLoadingScreen = isShowingLoadingScreen;
        levelsLoading.Add(lld);


        if (isShowingLoadingScreen)
        {
            //Turn on your loading screen here
            loadingScreen.SetActive(true);
        }

    }


    public void UnLoadLevel(string levelName)
    {
        foreach (string item in currentlyLoadedScenes)
        {
            if (item == levelName)
            {
                SceneManager.UnloadSceneAsync(levelName);
                currentlyLoadedScenes.Remove(item);
                return;
            }
        }

        Debug.LogErrorFormat("Failed to unload level ({0}), most likely was never loaded to begin with or was already unloaded.", levelName);
    }
}

[Serializable]
public class LevelLoadingData
{
    public AsyncOperation ao;
    public string sceneName;
    public Action<string> onLevelLoaded;
    public bool isShowingLoadingScreen;
}

public static class SceneList
{
    public const string MAIN_MENU = "MainMenu";
    public const string MULTIPLAYER_LEVEL1 = "MultiplayerLevel1";
    public const string MULTIPLAYER_LEVEL2 = "MultiplayerLevel2";
    public const string PLAYER_VS_COMPUTER_LEVEL1 = "PlayerVsComputerLevel1";
    public const string PLAYER_VS_COMPUTER_LEVEL2 = "PlayerVsComputerLevel2";
    public const string ONLINE = "Online";
    public const string LOGIN = "LogIn";
    public const string TEST = "TestLevel";
}