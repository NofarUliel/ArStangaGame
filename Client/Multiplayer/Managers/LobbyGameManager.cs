

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LobbyGameManager : MonoBehaviour
{
    public static bool isStartTimerGame, isStartGame;
    public static float current_time;
    public static string BALL_MASTER;
    public static string playerTurn;

    private const float TURN_TIMER = 10f,  INIT_TIMER = 5f, GAME_TIME = 120f;
    private const int MAX_GOL = 5, GOAL_SCORE = 1,DECREASE_SCORE = -1;
    private string PLAYER1, PLAYER2, winner; 
    private Dictionary<string, NetworkIdentity> playersNet;
    private ObjectNetwork ballNet;
    private float scorePlayer1, scorePlayer2;
    private NetworkTransformPlayerPosition netTransformPlayer1,netTransformPlayer2;
    private NetworkTransformBallPosition netTransformBall;
    private bool  isTimerFinish,isInittGame, isGameOver, isTimerOn;
    private Text resultPlayer1Txt, resultPlayer2Txt, resultTxt, initTimer;
    private TextMeshProUGUI timer_minute_txt, timer_second_txt, player1_score_txt, player2_score_txt;
    private Score score;
    private float gameTimer, second, minute;
    

    private void Awake()
    {
        isStartGame = false;
        isStartTimerGame = false;
        isInittGame = false;
        isGameOver = false;
        isTimerOn = false;
        isTimerFinish = false;


    }
    public void Start()
    {
        NetworkClient.GoalScore += GoalScore;
    }

    public void InitGame(GameObject resultGameContainer, GameObject scoreUI, Transform serverObject)
    {
        Debug.Log("start init");

        NetworkTransformPlayerPosition[] playerstransform = serverObject.GetComponentsInChildren<NetworkTransformPlayerPosition>();
        netTransformPlayer1 = playerstransform[0];
        netTransformPlayer2 = playerstransform[1];
        netTransformBall = serverObject.GetComponentInChildren<NetworkTransformBallPosition>();
     

        initTimer = GameObject.FindGameObjectWithTag("timer").GetComponent<Text>();
      
      
        timer_minute_txt = scoreUI.transform.Find("ScoreUI").Find("minute").GetComponent<TextMeshProUGUI>();
        timer_second_txt = scoreUI.transform.Find("ScoreUI").Find("second").GetComponent<TextMeshProUGUI>();
        player1_score_txt = scoreUI.transform.Find("ScoreUI").Find("ScorePlayer1").GetComponent<TextMeshProUGUI>();
        player2_score_txt = scoreUI.transform.Find("ScoreUI").Find("ScorePlayer2").GetComponent<TextMeshProUGUI>();
        resultPlayer1Txt = resultGameContainer.transform.Find("ResultPlayer1-Text").GetComponent<Text>();
        resultPlayer2Txt = resultGameContainer.transform.Find("ResultPlayer2-Text").GetComponent<Text>();
        resultTxt = resultGameContainer.transform.Find("Result-Text").GetComponent<Text>();

        PLAYER1 = playersNet.Keys.ElementAt(0);
        PLAYER2 = playersNet.Keys.ElementAt(1);
        BALL_MASTER = PLAYER1;
        Debug.Log("MASTER=" + BALL_MASTER);
       
        if (LobbyState.cameraState.Equals(LobbyState.AR_OFF))
        {
            isStartGame = true;
            isTimerOn = false;
            initTimer.enabled = false;
            Debug.Log("player score1=" + scorePlayer1 + "score2= " + scorePlayer2);
            player1_score_txt.text = scorePlayer1.ToString();
            player2_score_txt.text = scorePlayer2.ToString();
            LobbyState.cameraState = LobbyState.GAME;

        }
        else
        {
            isTimerOn = true;
            scorePlayer1 = 0;
            scorePlayer2 = 0;
            current_time = INIT_TIMER;
         

            //init score 
            Score s = GetObjectScore(NetworkClient.clientID, 0);
            playersNet[NetworkClient.clientID].GetSocket().Emit("updateScore", new JSONObject(JsonUtility.ToJson(s)));
            isInittGame = true;
            Debug.Log("end init");
        }
        current_time = INIT_TIMER;
        initTimer.text = INIT_TIMER.ToString();
    }
    private void Update()
    {
        
        if (isTimerOn && !isTimerFinish && isInittGame && MultiplayerManager.isAllPlayersDetectedMarker)
        {

            if (current_time <= 0.0f)
            {
                isTimerOn = false;
                initTimer.enabled = false;
                isStartGame = true;
                isTimerFinish = true;
                gameTimer = 0;
                second = 0;
                minute = 0;
                current_time = TURN_TIMER;


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
        if (isStartGame && !isGameOver && isTimerFinish)
        {
            if (scorePlayer1 < MAX_GOL && scorePlayer2 < MAX_GOL && gameTimer<GAME_TIME)
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

    
    public void SetBallNet(ObjectNetwork ballNet)
    {
        this.ballNet = ballNet;
        Debug.Log("ballnet" + this.ballNet);
    }
    public void SetPlayers(Dictionary<string, NetworkIdentity> players)
    {
        this.playersNet = players;
    }
    public void UpdateTurn(string idPlayer)
    {
        playerTurn = idPlayer;
        //current_time = TURN_TIMER;


    }
    public void SwitchTurn()
    {
        if (playerTurn.Equals(PLAYER1))
        {
            playerTurn = PLAYER2;

        }
        else
        {

            playerTurn = PLAYER1;

        }
        current_time = TURN_TIMER;
        Turn t = new Turn();
        t.playerTurn = playerTurn;
        t.sendId = NetworkClient.clientID;

        playersNet[NetworkClient.clientID].GetSocket().Emit("updateTurn", new JSONObject(JsonUtility.ToJson(t)));
        Debug.Log("update turn to player (" + playerTurn + ")");

    }

    public string GetPlayer1() { return PLAYER1 == null ? "" : this.PLAYER1; }
    public string GetPlayer2() { return PLAYER2 == null ? "" : this.PLAYER2; }


    public void Goal(string player)
    {

        score = GetObjectScore(player, GOAL_SCORE);
        Debug.Log("ui player =" + player);
        playersNet[player].GetSocket().Emit("goalUI", new JSONObject(JsonUtility.ToJson(score)));


    }

    public void GoalScore(string player,float updateScore)
    {
        score.player = player;
        score.score = updateScore;
        playersNet[NetworkClient.clientID].GetSocket().Emit("goal", new JSONObject(JsonUtility.ToJson(score)));
        Debug.Log("player (" + player + ") have goal");

    }
    public void UpdateScore(string player, float score)
    {
        if (player.Equals( PLAYER1))
        {
            scorePlayer1 = score;
            player1_score_txt.text =  scorePlayer1.ToString();
        }
        else
        {
            scorePlayer2 = score;
            player2_score_txt.text =  scorePlayer2.ToString();
        }
        Debug.Log("update score (" + score + ") to player (" + player + ")");

    }

    public void InitPosition()
    {
        netTransformBall.InitPosition();
        netTransformPlayer1.InitPosition();
        netTransformPlayer2.InitPosition();
    }
    public Score GetObjectScore(string player, float score)
    {
        Score s = new Score();
        if (player.Equals(PLAYER1))
        {
            s.score = scorePlayer1 + score;
            s.player = PLAYER1;
        }
        else
        {
            s.score = scorePlayer2 + score;
            s.player = PLAYER2;
        }
        return s;
    }
    public void SendScore(string player, float score)
    {
        Score s = GetObjectScore(player, score);
        playersNet[player].GetSocket().Emit("updateScore", new JSONObject(JsonUtility.ToJson(s)));
    }
    public void PlayerAction(string player)
    {
        if (player.Equals(playerTurn))
        {
            SwitchTurn();
        }
    }
    public void CollisionWithBall(string player, bool isAction)
    {
        Debug.Log("collider with ball player turn=" + playerTurn);
        if (string.Compare(player, playerTurn) == 0)
        {  //the player touched the ball and it his turn -->switch turn
           if (isAction)
           {
                SwitchTurn();
                Debug.Log("collider with ball SWITCH TURN");

            }
            else { Debug.Log("collider with ball -----------"); }
        }
        else
        {
            //the player touched the ball and it is not his turn --> decrease 1 point
            SendScore(player, player == PLAYER1 ? scorePlayer1 + DECREASE_SCORE : scorePlayer2 + DECREASE_SCORE);
            Debug.Log("collider with ball SCOREE");
        }
    }
    public void GameOver()
    {
        isGameOver = true;
        isStartGame = false;
        if (scorePlayer1 > scorePlayer2)
        {
            winner = PLAYER1;
            resultTxt.text = NetworkClient.users[PLAYER1].name + " is the winner!!!!";

        }
        else if (scorePlayer2 > scorePlayer1)
        {
            winner = PLAYER2;
            resultTxt.text = NetworkClient.users[PLAYER2].name + " is the winner!!!!";

        }
        else//ticko
        {
            winner = "ticko";
            resultTxt.text = "Ticko";

        }


        resultPlayer1Txt.text = NetworkClient.users[PLAYER1].name + "'s score : " + scorePlayer1.ToString();
        resultPlayer2Txt.text = NetworkClient.users[PLAYER2].name + "'s score : " + scorePlayer2.ToString();

        ResultGame resultGame = new ResultGame();
        resultGame.winnerId = winner;

        playersNet[NetworkClient.clientID].GetSocket().Emit("gameOver", new JSONObject(JsonUtility.ToJson(resultGame)));
        Debug.Log("send game over");


    }

   

}


public class ResultGame
{
    public string winnerId;
}