using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateManager : MonoBehaviour
{
    private LobbyGameManager lobbyGameManager;

    private void Awake()
    {
        lobbyGameManager = GameObject.FindObjectOfType<LobbyGameManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger ");
        if (other.gameObject.CompareTag("ball"))
        {
            if (this.tag.Equals("gate1"))
            {
                Debug.Log("gate 1");

                lobbyGameManager.Goal(lobbyGameManager.GetPlayer2());
           

            }
            else if (this.tag.Equals( "gate2"))
            {
                Debug.Log("gate 2");

                lobbyGameManager.Goal(lobbyGameManager.GetPlayer1());


            }

        }

    }

}
