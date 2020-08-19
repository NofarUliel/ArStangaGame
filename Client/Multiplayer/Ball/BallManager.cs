using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
  
    private Rigidbody rbody;
    private void Start()
    {
        rbody = transform.GetComponent<Rigidbody>();
        rbody.useGravity = false;

    }

    void Update()
    {
        if (LobbyGameManager.isStartGame)
        {
            rbody.useGravity = true;
            //Debug.Log("ball position" + "("+this.transform.position.x + "," + this.transform.position.y + "," + this.transform.position.z + ")");
            //Debug.Log("ball local position" + "("+this.transform.localPosition.x + "," + this.transform.localPosition.y + "," + this.transform.localPosition.z + ")");
        }
    }
 

}
