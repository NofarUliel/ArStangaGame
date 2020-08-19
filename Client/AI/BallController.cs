using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Vector3 initPosition;
    private GameController game_controller;


    void Start()
    {
        this.initPosition = this.transform.localPosition;
        
    }
   
    public void GoalEntered()
    {
        this.transform.localPosition = initPosition;
        this.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Debug.Log("init ball location = "+ this.transform.localPosition);
    }
    public Vector3 GetInitPosition()
    {
        return this.initPosition;
    }
    public void SetInitPosition(Vector3 pos)
    {
        this.initPosition = pos;
    }
}
