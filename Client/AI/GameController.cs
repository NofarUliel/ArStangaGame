using UnityEngine;
using UnityEngine.UI;
using Panda;
using TMPro;
using Vuforia;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static float current_time;
    public static int playerTurn, PLAYER = 0, AI_PLAYER = 1;
    private const float TURN_TIMER = 10f, INIT_TIMER = 5f, GAME_TIME = 120f;
    private const int MAX_GOL = 5, GOAL_SCORE = 1, DECREASE_SCORE = -1;
    public static bool isArCameraOn;
    public static Camera maincamera;
    [SerializeField]
    private TextMeshProUGUI AI_score_txt, player_score_txt, time, timer_minute_txt, timer_second_txt;
    [SerializeField]
    private GameObject scanBarcodeContainer,scoreUi,joystick,timerUiContainer,ArOnMeesContainer,menuGameUiContainer;
    [SerializeField]
    private int level;
    [SerializeField]
    private UnityEngine.UI.Image aiPlayerAvatar, humanPlayerAvatar;
    [SerializeField]
    private Camera arCamera;
    [SerializeField]
    private ImageTargetBehaviour my_target;
    [SerializeField]
    private Toggle checkMarkBtn;
    [SerializeField]
    private GameObject gameWithArContainer, gameWithoutArContainer, arOnMessageContainer;
    private int player_score, AI_score;
    private Avatar avatar;
    private HumanPlayer humanPlayer;
    private AIPlayerLevel1 AI_player;
    private BallController ballController;
    private bool isTimerFinish, isPauseGame, isMarkerDetected, isGameOver, isTimerOn, isStartGame;
    private Text resultHumanPlayerTxt, resultAIPlayerTxt, resultTxt, initTimer;
    private float gameTimer, second, minute;
    private GameObject stadium,ball, AIplayer,resultGameContainer, gameObjectsContainer,GoalUI;
    

    private void Awake()
    {
        maincamera = GameObject.FindGameObjectWithTag("camera").GetComponent<Camera>();
        maincamera.enabled = false;
        arCamera.GetComponent<VuforiaBehaviour>().enabled = false;
        arCamera.GetComponent<Camera>().enabled = false;
        GoalUI = NetworkClient.goalUi;
    }
    // Start is called before the first frame updatex
    void Start()
    {
        LobbyState.currentState = LobbyState.GAME;
        isMarkerDetected = false;
        isGameOver = false;
        isTimerOn = false;
        isTimerFinish = false;
        isPauseGame = false;
        playerTurn = PLAYER;
        player_score = 0;
        AI_score = 0;
        current_time = INIT_TIMER;
        avatar = new Avatar();
        initTimer = GameObject.FindGameObjectWithTag("timer").GetComponent<Text>();
        isStartGame = true;
        Debug.Log(" CameraPermission.isCameraPermssion= " + CameraPermission.isCameraPermssion);
        isArCameraOn = CameraPermission.isCameraPermssion;
        checkMarkBtn.isOn = isArCameraOn;
        isStartGame = false;

     
        if (isArCameraOn)
        {
            scanBarcodeContainer.SetActive(true);
            arCamera.GetComponent<VuforiaBehaviour>().enabled = true;
            arCamera.GetComponent<Camera>().enabled = true;
            gameWithoutArContainer.SetActive(false);
            gameWithArContainer.SetActive(true);
            Debug.Log("AR IS ON");
            

        }
        else
        {
            scanBarcodeContainer.SetActive(false);
            arCamera.GetComponent<VuforiaBehaviour>().enabled = false;
            arCamera.GetComponent<Camera>().enabled = false;
            gameWithoutArContainer.SetActive(true);
            gameWithArContainer.SetActive(false);
            Debug.Log("AR IS OFF");

        }
        gameObjectsContainer = GameObject.FindGameObjectWithTag("GameObjects");
        AIplayer = GameObject.FindGameObjectWithTag("ai");
        humanPlayer = GameObject.FindGameObjectWithTag("HumanPlayer").GetComponent<HumanPlayer>();
        ball = GameObject.FindGameObjectWithTag("ball");
        IsEnableObjects(false);

        if (!isArCameraOn)
        {
            OnTargetFound();
        }
    }
    public void OnTargetFound()
    {
        isMarkerDetected = true;
        isTimerOn = true;
        scanBarcodeContainer.SetActive(false);
        timerUiContainer.SetActive(true);
        scoreUi.SetActive(true);
        joystick.SetActive(true);
        menuGameUiContainer.SetActive(true);
        gameObjectsContainer.SetActive(true);
        isPauseGame = false;


        string HumanPlayerAvatar = NetworkClient.users[NetworkClient.clientID].avatar;
        avatar.DisplayAvatar(humanPlayerAvatar, HumanPlayerAvatar);
        avatar.DisplayAvatar(aiPlayerAvatar, Avatar.AI_AVATAR);

        if (level == 1)
        {
            AI_player = GameObject.FindObjectOfType<AIPlayerLevel1>();

        }
        else if (level == 2)
        {
            AI_player = GameObject.FindObjectOfType<AIPlayerLevel2>();

        }
        GameObject[] g = GameObject.FindGameObjectsWithTag("BlockPlane");
        foreach (GameObject obj in g)
        {
            MeshRenderer mesh = obj.GetComponent<MeshRenderer>();
            mesh.enabled = false;
        }
        AIplayer.GetComponent<PandaBehaviour>().enabled = false;
        ballController = GameObject.FindObjectOfType<BallController>();
        if(isTimerFinish && !isGameOver)
        {
            IsEnableObjects(true);
        }
    }
    public void OnTargetLoss()
    {
        if (isArCameraOn)
        {
            isTimerOn = false;
            timerUiContainer.SetActive(false);
            isMarkerDetected = false;
            isPauseGame = true;
            IsEnableObjects(false);
        }
    }

    private void Update()
    {

        if ( isTimerOn && !isTimerFinish )
        {
            if (current_time <= 0.0f)
            {
                isTimerOn = false;
                initTimer.enabled = false;
                isTimerFinish = true;
                gameTimer = 0;
                second = 0;
                minute = 0;
                current_time = TURN_TIMER;
                IsEnableObjects(true);
            }
            else
            {
                current_time -= Time.deltaTime;
                if (current_time.ToString("0") == "0")
                {
                    initTimer.text = "GO!";
                    initTimer.GetComponent<AudioSource>().Play();
                }
                else
                {
                    initTimer.text = current_time.ToString("0");
                }
            }
        }
        if (!isGameOver && isTimerFinish && !isPauseGame)
        {

            if (player_score < MAX_GOL && AI_score < MAX_GOL && gameTimer < GAME_TIME)
            {
                if (current_time <= 0.0f)
                {
                    SwitchTurn();
                    current_time = TURN_TIMER;
                }
                else
                {
                    current_time -= Time.deltaTime;
                }
                minute = Mathf.Floor(gameTimer / 60);
                second = Mathf.RoundToInt(gameTimer % 60);
                timer_minute_txt.text = minute < 10 ? "0" + minute.ToString() : minute.ToString();
                timer_second_txt.text = second < 10 ? "0" + Mathf.RoundToInt(second).ToString() : Mathf.RoundToInt(second).ToString();
                gameTimer += Time.deltaTime;
            }
            else
            {
                GameOver();
            }
        }
    }
       
    

  public void IsEnableObjects(bool value)
    {
        AIplayer.transform.GetComponent<Rigidbody>().useGravity = value;
        AIplayer.GetComponent<PandaBehaviour>().enabled = value;
        ball.transform.GetComponent<Rigidbody>().useGravity = value;
        humanPlayer.transform.GetComponent<Rigidbody>().useGravity = value;
    }
    public void UpdateScore(int score, int player)
    {
        if (player == PLAYER)
        {
            player_score = player_score + score;
            player_score_txt.text = player_score.ToString();
        }
        else if (player == AI_PLAYER)
        {
            AI_score = AI_score + score;
            AI_score_txt.text = AI_score.ToString();

        }
    }


    public int GetTurn()
    {
        return playerTurn;
    }
  



    public bool IsMyTurn(int player)
    {
        if (player == AI_PLAYER)
        {
            if (playerTurn == AI_PLAYER)
            {
                return true;
            }
            else
                return false;
        }
        else
        {
            if (playerTurn == PLAYER)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    public void SwitchTurn()
    {
        playerTurn = playerTurn == PLAYER ? AI_PLAYER : PLAYER;
        current_time = TURN_TIMER;
    }
    public void PlayerAction(int player)
    {
        if (player== playerTurn)
        {
            SwitchTurn();
        }
    }
    public void GameOver()
    {
        print("game over");
        isGameOver = true;
        GameController.maincamera.enabled = true;
        OnTargetLoss();
        scoreUi.SetActive(false);
        joystick.SetActive(false);
        menuGameUiContainer.SetActive(false);
        resultGameContainer =NetworkClient.resultGameContainer;
        resultHumanPlayerTxt = resultGameContainer.transform.Find("ResultPlayer1-Text").GetComponent<Text>();
        resultAIPlayerTxt = resultGameContainer.transform.Find("ResultPlayer2-Text").GetComponent<Text>();
        resultTxt = resultGameContainer.transform.Find("Result-Text").GetComponent<Text>();

        if (player_score > AI_score)
        {
            resultTxt.text = NetworkClient.users[NetworkClient.clientID].name + " is the winner!!!!";
        }
        else if (AI_score > player_score)
        {
            resultTxt.text = "AI computer is the winner!!!!";

        }
        else//ticko
        {
            resultTxt.text = "Ticko";
        }
        resultHumanPlayerTxt.text = NetworkClient.users[NetworkClient.clientID].name + "'s score  : " + player_score.ToString();
        resultAIPlayerTxt.text = "computer's score : " + AI_score.ToString();
        LobbyState.currentState = LobbyState.GAME_OVER;
    }

    public void GoalEntered()
    {
        isPauseGame= true;
        AIplayer.GetComponent<PandaBehaviour>().enabled = false;
        GoalUI.SetActive(true);
        StartCoroutine( Wait());//wait 1 seconds

        AI_player.BackToInitPosition();
        humanPlayer.BackToInitPosition();
        Debug.Log("AI_player init pos= " + humanPlayer.GetPos());

        ballController.GoalEntered();
        current_time = TURN_TIMER;
    }
    public void CollisionWithBall(int player, bool isAction)
    {
        Debug.Log("collider with ball player turn=" + playerTurn);
        if (player == playerTurn)
        {  //the player touched the ball and it his turn -->switch turn
            if (isAction)
            {
                SwitchTurn();
                Debug.Log("collider with ball SWITCH TURN");

            }
            else { Debug.Log("collider with ball need to do action"); }
        }
        else
        {
            //the player touched the ball and it is not his turn --> decrease 1 point
            UpdateScore(DECREASE_SCORE, player);
            Debug.Log("collider with ball DECREASE score");
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
                Vector3 aiPos = AIplayer.transform.localPosition;
                Quaternion aiRot = AIplayer.transform.localRotation;
                Vector3 humanPos = humanPlayer.transform.localPosition;
                Quaternion humanRot = humanPlayer.transform.localRotation;
                Vector3 ballPos = ball.transform.localPosition;
                Vector3 aiInitPos = AI_player.GetInitPosition();
                Vector3 humanInitPos = humanPlayer.GetInitPosition();

                Debug.Log("hunman old pos=" + humanInitPos);

                Vector3 ballInitPos = ballController.GetInitPosition();
                arCamera.GetComponent<VuforiaBehaviour>().enabled = false;
                arCamera.GetComponent<Camera>().enabled = false;
                gameWithArContainer.SetActive(false);
                gameWithoutArContainer.SetActive(true);
                AIplayer = GameObject.FindGameObjectWithTag("ai");
                humanPlayer = GameObject.FindGameObjectWithTag("HumanPlayer").GetComponent<HumanPlayer>();
                ball = GameObject.FindGameObjectWithTag("ball");
                ballController = GameObject.FindObjectOfType<BallController>();
                gameObjectsContainer = GameObject.FindGameObjectWithTag("GameObjects");
                stadium = GameObject.FindGameObjectWithTag("stadium");
                AIplayer.transform.localPosition = aiPos;
                AIplayer.transform.localRotation = aiRot;
                IsEnableObjects(true);
                humanPlayer.transform.localPosition = humanPos;
                humanPlayer.transform.localRotation = humanRot;
                ball.transform.localPosition = ballPos;
                stadium.SetActive(true);
                AI_player.SetInitPosition(aiInitPos);
                humanPlayer.SetInitPosition(humanInitPos);
                Debug.Log("set hunman pos=" + humanPlayer.GetPos());
                ballController.SetInitPosition(ballInitPos);
                scanBarcodeContainer.SetActive(false);
                menuGameUiContainer.SetActive(true);
            }
        }

    }
    IEnumerator Wait()
    {

        yield return new WaitForSeconds(2);
        GoalUI.SetActive(false);
        isPauseGame = false;
        AIplayer.GetComponent<PandaBehaviour>().enabled = true;

    }
}






