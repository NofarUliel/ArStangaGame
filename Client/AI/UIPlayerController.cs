using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject ring, timerTxt;

    // Start is called before the first frame update
    void Start()
    {
        ring.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {

        if (this.tag.Equals("HumanPlayer"))//Human player
        {
            if (GameController.playerTurn == GameController.PLAYER)
            {
                timerTxt.GetComponent<TextMeshPro>().text = GameController.current_time.ToString("0");
                ring.SetActive(true);
            }
            else
            {
                timerTxt.GetComponent<TextMeshPro>().text = "";
                ring.SetActive(false);
            }
        }
        else//AI player
        {

            if (GameController.playerTurn == GameController.AI_PLAYER)
            {

                timerTxt.GetComponent<TextMeshPro>().text = GameController.current_time.ToString("0");
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
