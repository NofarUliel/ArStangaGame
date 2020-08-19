using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectNetwork))]
public class NetworkTransformBallPosition : MonoBehaviour
{
    [SerializeField]
    [GreyOut]
    private Vector3 oldPosition;
    private ObjectNetwork ballNet;
    private Ball ball;
    private Vector3 initPosition;


    void Start()
    {
        ballNet = GetComponent<ObjectNetwork>();
        initPosition = transform.localPosition;
        oldPosition = initPosition;
        ball = new Ball();
        ball.position = new Position();
        ball.position.x = initPosition.x;
        ball.position.y = initPosition.y;
        ball.position.z =initPosition.z;


        //if (!NetworkClient.clientID.Equals(LobbyGameManager.BALL_MASTER))
        //{
        //    this.enabled = false;
        //}


    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkClient.clientID.Equals(LobbyGameManager.BALL_MASTER))
        {
            if (oldPosition != transform.localPosition)
            {
                oldPosition = transform.localPosition;
                sendData();
            }
        }

    }


    private void sendData()
    {

      //if (LobbyState.currentState.Equals(LobbyState.GAME))// && NetworkClient.clientID.Equals(LobbyGameManager.BALL_MASTER)) 
      //  {
            //update player information
            ball.position.x = transform.localPosition.x.ThreeDecimals();
            ball.position.y = transform.localPosition.y.ThreeDecimals();
            ball.position.z = transform.localPosition.z.ThreeDecimals();

        

            //send to server the update location
            ballNet.GetSocket().Emit("updateBallPosition", new JSONObject(JsonUtility.ToJson(ball)));
          //  Debug.Log("id= "+NetworkClient.clientID + " send Ball position " + ball.position.x + "," + ball.position.y + "," + ball.position.z);
        //}
    }

    public void InitPosition()
    {

        Debug.Log("init ball position ");
        transform.localPosition = initPosition;
        sendData();

    }
}