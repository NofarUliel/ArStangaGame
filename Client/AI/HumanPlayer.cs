using System;
using UnityEngine;
using UnityEngine.UI;

public class HumanPlayer : BasePlayer
{
    public static bool isJoystickDrage, isKick;
    [SerializeField]
    private FixedJoystick joystick;
    [SerializeField]
    private Button kick_btn, jump_btn, pass_btn, block_btn;
    private bool isCloseToBall;


    protected override void Start()
    {
        base.Start();
        Debug.Log("human init=" + this.initPosition);
        isJoystickDrage = false;
        isKick = false;
        SetListener();
        player = GameController.PLAYER;

    }

    void FixedUpdate()
    {
        if (joystick != null)
        {
            if (joystick.Vertical == 0 && joystick.Horizontal == 0)
            {
                isJoystickDrage = false;
            }
            else
            {
                isJoystickDrage = true;

            }

            PlayerMovement();
        }
       
    }

    public void SetListener()
    {
        kick_btn.onClick.AddListener(OnClickKick);
        pass_btn.onClick.AddListener(OnClickPass);
        jump_btn.onClick.AddListener(OnClickJump);
        block_btn.onClick.AddListener(OnClickBlock);
    }
    private void PlayerMovement()
    {
        Vector3 d = Vector3.right * joystick.Vertical + joystick.Horizontal * (-1) * Vector3.forward;
        Vector3 targetPosition = d * SPEED * Time.deltaTime;
        targetPosition.y = 0;
        targetPosition = this.transform.position + targetPosition;
        transform.LookAt(targetPosition);
        transform.position = targetPosition;

    }
    private void IsCloseBall()
    {
        isCloseToBall = false;
        if (ball != null)
        {
            Vector3 ballPos = ball.transform.position;
            Vector3 playerPos = this.transform.position;
            ballPos.y = 0;
            playerPos.y = 0;
            float distance_player_ball = Vector3.Distance(ballPos, playerPos);
            if (distance_player_ball <= OBJ_DISTANCE)
            {
                isCloseToBall = true;
            }
        }
    }
    public void OnClickKick()
    {
        isKick = true;
        Kick(false);
    }
    private void OnClickPass()
    {
        isKick = true;
        Kick(true);


    }
    public void Kick(bool isPass)
    {

        if (isPass)
        {
            Debug.Log("----------pass-------------");

        }
        else
        {
            Debug.Log("----------kick-------------");

        }
        IsCloseBall();
        anim.SetTrigger("isPass");
        if (isCloseToBall)
        {

            float distanceGatePercent = Vector3.Distance(enemyGate.transform.position, this.transform.position) /
            Vector3.Distance(enemyGate.transform.position, my_gate.transform.position);

            Vector3 balldDrection = ball.transform.position - transform.position;
            balldDrection.y = isPass == true ? 0f : UnityEngine.Random.Range(1f, 3f);
            ball.GetComponent<Rigidbody>().velocity = balldDrection.normalized * FORCE* distanceGatePercent;
            isAction = true;
            game_controller.PlayerAction(GameController.PLAYER);
            isAction = false;

        }
        isKick = false;

    }
    private void OnClickJump()
    {

        Debug.Log("----------jump-------------");
        anim.SetTrigger("isJumping");
        isAction = true;


    }

    private void OnClickBlock()
    {

        Debug.Log("----------block-------------");
        anim.SetTrigger("isBlock");
        isAction = true;

    }


}
