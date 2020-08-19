using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : MonoBehaviour
{
    protected const float SPEED = 4f;
    protected const float FORCE = 10f;
    protected const float BLOCK_DISTANCE = 1.5f;
    protected const float CLOSE = 2.5f;
    protected const float OBJ_DISTANCE = 2f;


    [SerializeField]
    protected GameObject ball, enemyGate, my_gate;
    [SerializeField]
    protected GameController game_controller;
    protected Animator anim;
    protected Vector3 initPosition;
    protected int turn, player;
    protected bool isAction;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        isAction = false;
        initPosition = this.transform.localPosition;
        anim = transform.GetComponent<Animator>();
    }

    public void BackToInitPosition()
    {
    
        this.transform.LookAt(enemyGate.transform.position);
        this.transform.localPosition = initPosition;
        Debug.Log("init pos =" + this.transform.localPosition);
    }
    public Vector3 GetInitPosition()
    {
        return this.initPosition;
    }
    public void SetInitPosition(Vector3 pos)
    {
        this.initPosition=pos;
    }
    public Vector3 GetPos() { return initPosition; }
   public void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("ball"))
        {
            string txt = player == GameController.PLAYER ? "Human player collider with ball" : "AI player collider with ball";
            Debug.Log(txt);
            game_controller.CollisionWithBall(player, isAction);
            isAction = false;
        }
        if (collision.gameObject.CompareTag("wall"))
        {
            string txt = player == GameController.PLAYER ? "Human player collider with wall" : "AI player collider with wall";
            Debug.Log(txt);
           game_controller.UpdateScore(-1, player);
        }
    }

}
