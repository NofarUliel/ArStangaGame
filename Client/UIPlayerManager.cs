using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject ring, timerTxt;
    [SerializeField]
    private NetworkIdentity networkIdentity;

    // Start is called before the first frame update
    void Start()
    {
        ring.SetActive(false);
      
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkClient.lobbyGameManager != null && LobbyState.currentState.Equals(LobbyState.GAME) )//start game
        {
            if (networkIdentity.IsControlling())
            {
                if (LobbyGameManager.playerTurn.Equals(NetworkClient.clientID))
                {
                    
                    timerTxt.GetComponent<TextMeshPro>().text=LobbyGameManager.current_time.ToString("0");
                    ring.SetActive(true);

                }
                else
                {
                    timerTxt.GetComponent<TextMeshPro>().text = "";
                    ring.SetActive(false);
                }

            }
            else
            {
                if (!LobbyGameManager.playerTurn.Equals(NetworkClient.clientID))
                {
    
                    timerTxt.GetComponent<TextMeshPro>().text = LobbyGameManager.current_time.ToString("0");
                    ring.SetActive(true);
                }
                else
                {
                    timerTxt.GetComponent<TextMeshPro>().text = "";
                    ring.SetActive(false);

                }

            }
        }
    }
}
