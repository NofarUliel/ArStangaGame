
using UnityEngine;
using Vuforia;
using UnityEngine.UI;

public class MultiplayerManager : MonoBehaviour
{
    public static bool isAllPlayersDetectedMarker, isArCameraOn;
    public static GameObject serverObject;

    [SerializeField]
    private ImageTargetBehaviour my_target;
    [SerializeField]
    private Toggle checkMarkBtn;
    [SerializeField]
    private GameObject scanUI, menuUI, gameWithArContainer, gameWithoutArContainer,arOnMessageContainer;
    
    [SerializeField]
    private Camera arCamera;
    public static Camera maincamera;
    private GameObject stadium;
    private bool isSendDetected ,isStartGame;



    void Start()
    {
        isStartGame = true;
        Debug.Log(" CameraPermission.isCameraPermssion= " + CameraPermission.isCameraPermssion);
        isArCameraOn = CameraPermission.isCameraPermssion;
        checkMarkBtn.isOn = isArCameraOn;
        isStartGame = false;
        maincamera = GameObject.FindGameObjectWithTag("camera").GetComponent<Camera>();
        maincamera.enabled = false;


        if (isArCameraOn)
        {
            arCamera.GetComponent<VuforiaBehaviour>().enabled = true;
            arCamera.GetComponent<Camera>().enabled = true;
            maincamera.enabled=false;
            gameWithoutArContainer.SetActive(false);
            gameWithArContainer.SetActive(true);
            isAllPlayersDetectedMarker = false;
            isSendDetected = false;
            Debug.Log("AR IS ON");

        }
        else
        {

            arCamera.GetComponent<VuforiaBehaviour>().enabled = false;
            arCamera.GetComponent<Camera>().enabled = false;
            maincamera.enabled = true;
            gameWithoutArContainer.SetActive(true);
            gameWithArContainer.SetActive(false);
            isSendDetected = true;
            NetworkClient.users[NetworkClient.clientID].socket.Emit("markerDetected");
            Debug.Log("AR IS OFF");

        }
        serverObject = GameObject.FindGameObjectWithTag("serverObjects");
        Debug.Log("serverObjects=" + serverObject.name);
        stadium = GameObject.FindGameObjectWithTag("stadium");
        Debug.Log("stadium=" + stadium.name);

       
        stadium.SetActive(false);
        menuUI.SetActive(false);

    }
   // Update is called once per frame
    void Update()
    {

        if (!isAllPlayersDetectedMarker)
        {
            if (SceneManagementManager.isAllPlayersLoadedLevel)
            {
                scanUI.SetActive(true);
            }
            if (!isArCameraOn)//without AR
            {
                scanUI.GetComponentInChildren<Text>().text = "Waiting for other player to scan the barcode..";
                scanUI.GetComponentInChildren<UnityEngine.UI.Image>().enabled = false;
            }
            else// with AR
            {
                if (my_target.CurrentStatus != TrackableBehaviour.Status.NO_POSE && !isSendDetected)
                {
                    NetworkClient.users[NetworkClient.clientID].socket.Emit("markerDetected");
                    isSendDetected = true;
                    scanUI.GetComponentInChildren<Text>().text = "Waiting for other player to scan the barcode..";
                    scanUI.GetComponentInChildren<UnityEngine.UI.Image>().enabled = false;

                }
            }
          
        }
        else
        {
            stadium.SetActive(true);

            if (MainMenuManager.currentLevelGame.Equals(SceneList.MULTIPLAYER_LEVEL1))
            {
                GameObject[] g = GameObject.FindGameObjectsWithTag("BlockPlane");
                foreach (GameObject obj in g)
                {
                    MeshRenderer mesh = obj.GetComponent<MeshRenderer>();
                    mesh.enabled = false;
                }
            }
            scanUI.SetActive(false);
            menuUI.SetActive(true);
           


        }



    }

  

    public void OnChangeIsOnARcamera()
    {
        if (!isStartGame)
        {
            bool isArCameraMark = checkMarkBtn.isOn;
            if (isArCameraMark) //ar camera on
            {
                checkMarkBtn.isOn = false;
                isArCameraOn = true;
                //show message to user that he cant convert to ar camera while game
                arOnMessageContainer.SetActive(true);

            }
            else //ar camera off
            {
                isArCameraOn = false;
                Debug.Log("AR camera off (" + NetworkClient.users[NetworkClient.clientID].name + ")");
                LobbyState.cameraState = LobbyState.AR_OFF;
                NetworkClient.DestroyServerObject();
                arCamera.GetComponent<VuforiaBehaviour>().enabled = false;
                arCamera.GetComponent<Camera>().enabled = false;
                gameWithArContainer.SetActive(false);
                gameWithoutArContainer.SetActive(true);
                serverObject = GameObject.FindGameObjectWithTag("serverObjects");//update to serverObject2
                NetworkClient.users[NetworkClient.clientID].socket.Emit("ARcameraOff");
                maincamera.enabled = true;
                stadium = GameObject.FindGameObjectWithTag("stadium");
                Debug.Log("stadium=" + stadium.name);
                stadium.SetActive(true);
                scanUI.SetActive(false);
                menuUI.SetActive(true);

            }
        }
       
    }

    public void OnTargetLoss()
    {
        //LobbyGameManager.isStartGame = false;
        serverObject.SetActive(false);
        stadium.SetActive(false);
    }
    public void OnTargetFound()
    {
        if (isAllPlayersDetectedMarker)
        {
        //LobbyGameManager.isStartGame = true;
        serverObject.SetActive(true);
        stadium.SetActive(true);
        }
       
    }
}
