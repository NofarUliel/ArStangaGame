using SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkIdentity : MonoBehaviour
{
    [Header("Helful Values")]
    [SerializeField]
    [GreyOut]
    private string id;
    [SerializeField]
    [GreyOut]
    private bool isConrolling;
    private SocketIOComponent socket;
   


    public void Awake()
    {
        isConrolling = false;
    }
  
    public void SetControllerID(string ID)
    {
        //Check incomming id versuses the one we have saved from the server
        id = ID;
        isConrolling = (NetworkClient.clientID == ID) ? true : false;
    }
    private void Update()
    {
        isConrolling = (NetworkClient.clientID == id) ? true : false;
        if (isConrolling)
        {
            GetComponent<NetworkTransformPlayerPosition>().enabled = true;
        }

    }
    public void SetSocketReference(SocketIOComponent Socket)
    {
        socket = Socket;
    }

    public string GetID() { return id; }

    public bool IsControlling() { return isConrolling; }

    public SocketIOComponent GetSocket() { return socket; }
   

}