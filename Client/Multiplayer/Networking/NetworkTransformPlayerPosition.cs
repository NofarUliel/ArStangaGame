using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkIdentity))]
public class NetworkTransformPlayerPosition : MonoBehaviour
{

    [SerializeField]
    [GreyOut]
    private Vector3 oldPosition;

    private NetworkIdentity networkIdentity;
    private Player player;
    private float stillCounter = 0;
    private Vector3 initPosition;
    private Quaternion initRotation;

    void Start()
    {
       
        networkIdentity = GetComponent<NetworkIdentity>();
        initPosition = transform.localPosition;
        initRotation = transform.localRotation;
        oldPosition = initPosition;
        player = new Player();
        player.position = new Position();
        player.position.x = 0;
        player.position.y = 0;
        player.position.z = 0;
        player.rotation = new Position();
        player.rotation.x = 0;
        player.rotation.y = 0;
        player.rotation.z = 0;
        if (!networkIdentity.IsControlling())
        {
            enabled = false;
        }


    }

    // Update is called once per frame
    void Update()
    {
      
        if (networkIdentity.IsControlling())
        {
            
            if (oldPosition != transform.localPosition)
            {
                oldPosition = transform.localPosition;
                stillCounter = 0;
                sendData();
              

            }
            else
            {
                stillCounter += Time.deltaTime;
                if (stillCounter >= 1)//send data every 1 second 
                {
                    stillCounter = 0;
                    sendData();

                }
            }

        }

    }

     


    private void sendData()
    {
        if (LobbyState.currentState == LobbyState.GAME)
        {
           
            player.position.x = transform.localPosition.x.ThreeDecimals();
            player.position.y = transform.localPosition.y.ThreeDecimals();
            player.position.z = transform.localPosition.z.ThreeDecimals();
       
            Vector3 rotation = transform.localEulerAngles;
            player.rotation.x = rotation.x.ThreeDecimals();
            player.rotation.y = rotation.y.ThreeDecimals();
            player.rotation.z = rotation.z.ThreeDecimals();
      
            //send to server the update location and rotation
            networkIdentity.GetSocket().Emit("updatePosition", new JSONObject(JsonUtility.ToJson(player)));
           // Debug.Log("send my location (" + player.position.x + "," + player.position.y + "," + player.position.z + ")");

        }
    }
  


    public void InitPosition()
    {

        if (networkIdentity.IsControlling())
        {
            Debug.Log("init player position ("+ networkIdentity.GetID()+ ")");
            transform.localPosition = initPosition;
            transform.localRotation= initRotation;
        }
    }

}

