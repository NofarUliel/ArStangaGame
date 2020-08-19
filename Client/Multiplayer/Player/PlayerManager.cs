using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private const float FORCE = 25f;
    private const float SPEED = 4f;
    private const float CLOSE_BALL = 2f;

    public static bool isSetJostick, isJoystickDrage, isKick;
    public static bool isSetGameManager;
    public static ObjectNetwork ball;
    public static bool isSetListener;


    [SerializeField]
    private NetworkIdentity networkIdentity;
    private LobbyGameManager lobbyGameManager;
    private Animator anim;
    private FixedJoystick joystick;
    private Button kick_btn, jump_btn, pass_btn, block_btn;
    private bool isCloseToBall, isPressed;


    void Start()
    {

        isJoystickDrage = false;
        isKick = false;
        lobbyGameManager = NetworkClient.lobbyGameManager;
        if (networkIdentity.IsControlling())
        {

            Init();
            isPressed = false;
        }

        anim = transform.GetComponent<Animator>();

    }

    void FixedUpdate()
    {
        if (networkIdentity.IsControlling())
        {

            Init();

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

                if (isSetListener)
                {
                    SetListener();

                }
            }
        }

    }
    public void Init()
    {


        joystick = NetworkClient.jostick;
        Button[] btns = NetworkClient.btns;
        kick_btn = btns[0];
        pass_btn = btns[1];
        jump_btn = btns[2];
        block_btn = btns[3];
    }
    public static void SetBall(ObjectNetwork newball)
    {
        ball = newball;
        isSetListener = true;

    }
    public void SetListener()
    {
        kick_btn.onClick.AddListener(OnClickKick);
        pass_btn.onClick.AddListener(OnClickPass);
        jump_btn.onClick.AddListener(OnClickJump);
        block_btn.onClick.AddListener(OnClickBlock);
        isSetListener = false;




    }
    private void PlayerMovement()
    {
        Vector3 d = Vector3.forward * joystick.Vertical + joystick.Horizontal * Vector3.right;
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
            float distance_player_ball = Vector3.Distance(this.transform.position, ball.transform.position);
            if (distance_player_ball <= CLOSE_BALL)
            {
                isCloseToBall = true;
            }
        }
    }
    private void SendData(string nameAnim)
    {
        Anim animator = new Anim();
        animator.anim = nameAnim;
        anim.SetTrigger(nameAnim);
        //send to server the update animation
        networkIdentity.GetSocket().Emit("updateAnim", new JSONObject(JsonUtility.ToJson(animator)));
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
        if (networkIdentity.IsControlling())
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
            SendData("isPass");
            if (isCloseToBall)
            {
                Vector3 balldDrection = ball.transform.position - transform.position;
                balldDrection.y = isPass == true ? 0f : UnityEngine.Random.Range(1f, 3f);
                ball.GetComponent<Rigidbody>().velocity = balldDrection.normalized * FORCE;
                sendKickBall(balldDrection);
                isPressed = true;
                lobbyGameManager.PlayerAction(networkIdentity.GetID());
                isPressed = false;
            }
            isKick = false;

        }
    }
   
    private void OnClickJump()
    {

        Debug.Log("----------jump-------------");
        SendData("isJumping");
        isPressed = true;


    }

    private void OnClickBlock()
    {

        Debug.Log("----------block-------------");
        SendData("isBlock");
        isPressed = true;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (networkIdentity.IsControlling() && joystick != null)
        {
            Debug.Log("collision");
            if (collision.gameObject.CompareTag("ball"))
            {
                Debug.Log("player (" + networkIdentity.GetID() + ") collider with ball");
                lobbyGameManager.CollisionWithBall(networkIdentity.GetID(), isPressed);
                isPressed = false;
            }
           if (collision.gameObject.CompareTag("wall"))
            {
                Debug.Log("player (" + networkIdentity.GetID() + ") collider with obstical");
                lobbyGameManager.SendScore(NetworkClient.clientID, -1);
            }
        }
    }

    public void sendKickBall(Vector3 balldDrection)
    {
        bool m = !NetworkClient.clientID.Equals(LobbyGameManager.BALL_MASTER);
        if (!NetworkClient.clientID.Equals(LobbyGameManager.BALL_MASTER))
        {
            KickBall kickBall = new KickBall();
            kickBall.ballID = ball.GetID();
            kickBall.direction = new Position();
            kickBall.direction.x = balldDrection.normalized.x;
            kickBall.direction.y = balldDrection.normalized.y;
            kickBall.direction.z = balldDrection.normalized.z;
            kickBall.force = FORCE;
            networkIdentity.GetSocket().Emit("playerKickBall", new JSONObject(JsonUtility.ToJson(kickBall)));
        }
    }

}
[Serializable]
public class Anim
{
    public string anim;
}