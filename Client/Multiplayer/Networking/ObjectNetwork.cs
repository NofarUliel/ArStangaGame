using SocketIO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectNetwork : MonoBehaviour
{
    [Header("Helful Values")]
    [SerializeField]
    [GreyOut]
    private string id;
    [SerializeField]
    [GreyOut]
    private SocketIOComponent socket;



    public void SetControllerID(string ID)
    {
        id = ID;
    }

    public void SetSocketReference(SocketIOComponent Socket)
    {
        socket = Socket;
    }

    public string GetID() { return id; }

    public SocketIOComponent GetSocket() { return socket; }
}
