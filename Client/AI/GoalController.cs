using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    [SerializeField]
    private GameController game_controller;
    private void Start()
    {
        Debug.Log("game_controller =" + game_controller.name);
    }

   
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger ");

        if (other.gameObject.CompareTag("ball"))
        {
            Debug.Log("trigger with ball ");

            if (this.tag.Equals("HumanGate"))
            {
                Debug.Log("AI player entered goal ");
                game_controller.UpdateScore(1, GameController.AI_PLAYER);
                game_controller.GoalEntered();


            }
            else if (this.tag.Equals("AiGate"))
            {
                Debug.Log("Human player entered goal ");


                game_controller.UpdateScore(1, GameController.PLAYER);
                game_controller.GoalEntered();


            }

        }

    }

}
