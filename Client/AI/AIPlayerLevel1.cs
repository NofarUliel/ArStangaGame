using Panda;
using UnityEngine;



public class AIPlayerLevel1 : BasePlayer
{
   

    [SerializeField]
    protected Transform leftGateCorner, rightGateCorner, other_player;
    protected const float BALL_RADIUS = 1f;
    protected const float MAX_DISTANCE = 5f;
    protected const float FRONT_SIDE_SENSOR_POS = 0.2f;
    protected const float FRONT_SENSOR_ANGLE = 50;
    protected Vector3 frontSensorPosition = new Vector3(0, 0.2f, 0.5f);
    protected bool avoiding = false, isSeeBall;
    protected float angle;
    protected Vector3 centreCircle;

    protected override void Start()
    {
 
        base.Start();
        player = GameController.AI_PLAYER;
    }
  

    [Task]
    public bool IsMyTurn()
    {
        return this.game_controller.IsMyTurn(GameController.AI_PLAYER);
    }
    [Task]
    public void SwitchTurn()
    {
        game_controller.SwitchTurn();
        isAction = false;
        Task.current.Succeed();
    }
    [Task]
    public void LookAtTarget(string target_name)
    {
        Vector3 target;
        switch (target_name)
        {
            case "ball":
                target = ball.transform.position;
                target.y =this.transform.position.y;
                break;
            case "myGate":
                target = my_gate.transform.position;
                break;
            case "enemyGate":
                target = enemyGate.transform.position;
                break;
            case "enemy":
                target = other_player.transform.position;
                break;
            default:
                target = new Vector3(0, 0, 0);
                break;

        }

        Vector3 direction = target - this.transform.position;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * SPEED);
        if (Task.isInspected)
        {
            Task.current.debugInfo = string.Format("angle={0}", Vector3.Angle(this.transform.forward, direction));

        }

        if (Vector3.Angle(this.transform.forward, direction) < 5.0f)
        {
            Task.current.Succeed();
        }


    }
    [Task]
    public bool IsPlayerCloseToHisGate()
    {

        if (Vector3.Distance(this.transform.position, my_gate.transform.position) <= CLOSE)
        {
            return true;
        }
        return false;
    }
    [Task]
    protected virtual void GoToBall()
    {
        Debug.LogFormat("AI go to ball -started at {0} miliseconds", System.DateTime.Now.Millisecond);
        Vector3 target = new Vector3(ball.transform.position.x, this.transform.position.y, ball.transform.position.z);
        GoToTarget(target);
        Debug.LogFormat("AI go to ball -finished at {0} miliseconds", System.DateTime.Now.Millisecond);

    }
    [Task]
    public bool IsBallBehindPlayer()
    {

        if (ball.transform.position.z < this.transform.position.z)
        {
            Debug.Log("ball is behind player");
            return true;
        }
        else
        {
            Debug.Log("ball is not behind player");
            angle = 0;
            return false;
        }

    }
    [Task]
    public bool IsAICloseToBall()
    {
       
        Vector3 aiPos = this.transform.position;
        aiPos.y = 0;
        Vector3 ballPos = ball.transform.position;
        ballPos.y = 0;
        if (Vector3.Distance(aiPos, ballPos) <=OBJ_DISTANCE)
        {
            return true;
        }
        else { return false; }
    }
    public void GoToTarget(Vector3 target)
    {
        Vector3 direction = target - this.transform.position;

       if (Vector3.Distance(this.transform.position, target) > OBJ_DISTANCE)
        {
            direction.Normalize();
            direction = direction * SPEED * Time.deltaTime;
            direction.y = 0;
            this.transform.position += direction;

        }
        if (Task.isInspected) //debug
        {
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);

        }

        Vector3 aiPos = this.transform.position;
        aiPos.y = 0;
        Vector3 targetPos = target;
        targetPos.y = 0;
        if (Vector3.Distance(aiPos , targetPos) <= OBJ_DISTANCE)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }
    [Task]
    public void PlayerGoBehindBall()
    {
        centreCircle = new Vector3( ball.transform.position.x,this.transform.position.y,ball.transform.position.z);
        angle += Time.deltaTime * SPEED; // multiply all this with some speed variable (* speed);
        Debug.Log("ang=" + angle);
        if (angle < 5 && angle > 2)
        {

            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * BALL_RADIUS;  
            transform.position = centreCircle + offset;

        }
        if (angle >=5)
        {
            Task.current.Succeed();

        }
        else { Task.current.Fail(); }

    }
    

    [Task]
    protected virtual void GoToEnemy()
    {
        isSeeBall = false;
         RaycastHit hit;
        if (Physics.Raycast(this.transform.position + new Vector3(0, 0.5f, 0), other_player.transform.position-this.transform.position, out hit))
        {
            if (hit.collider.gameObject.tag.Equals("ball"))
            {
                Debug.DrawLine(this.transform.position + new Vector3(0, 0.5f, 0), hit.point, Color.green);
                isSeeBall = true;
            }
        }
        Vector3 target = my_gate.transform.position - other_player.transform.position;
        target.y = 0;
        target = other_player.transform.position + target.normalized * BLOCK_DISTANCE;
        GoToTarget(target);
        
    }
  

    [Task]
    protected virtual void GoToMyGate()
    {
        if (Vector3.Distance(this.transform.position, my_gate.transform.position) >CLOSE)
        {
            Vector3 target = my_gate.transform.position - this.transform.position;
            GoToTarget(target);
        }

    }
    [Task]
    public bool IsBallCloseToMyGate()
    {
        float distanceGatePercent = Vector3.Distance(my_gate.transform.position, ball.transform.position) /
          Vector3.Distance(enemyGate.transform.position, my_gate.transform.position);
        if (distanceGatePercent <= 0.35f)
        {
            return true;
        }
        return false;

    }




    [Task]
    public bool SeePlayer()
    {
        Vector3 distance = enemyGate.transform.position - this.transform.position;
        return IsSeePlayer(distance);

    }

    public bool IsSeePlayer(Vector3 distance)
    {
        RaycastHit hit;
        bool seeEnemy = false, seeWall = false;
        if (Physics.Raycast(this.transform.position + new Vector3(0, 0.7f,0), distance, out hit))
        {
            Debug.Log("name=" + hit.collider.name);
            Debug.DrawRay(this.transform.position + new Vector3(0, 0.7f, 0), distance, Color.red);

            if (hit.collider.gameObject.tag.Equals( "wall"))
            {
                seeWall = true;
            }
            if (hit.collider.name.Equals(other_player.name))
            {
                seeEnemy = true;
            }
        }
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("wall={0}", seeWall);

        if (!seeWall && seeEnemy)
        {
            Debug.Log("see the player");
            return true;

        }
        else
        {
            Debug.Log("not see the player");
            return false;

        }
    }

    public void Kick(Vector3 kickDirection)
    {
        float distanceGatePercent = Vector3.Distance(enemyGate.transform.position, this.transform.position) /
            Vector3.Distance(enemyGate.transform.position, my_gate.transform.position);
        Debug.Log("per=" + distanceGatePercent);
        //ai player is close to gate -> pass kick
        if (distanceGatePercent < 0.25f)
        {
            kickDirection.y = 0;
            Debug.Log("AI kick pass ");

        }

        Debug.DrawRay(this.transform.position, kickDirection * FORCE, Color.yellow);
        anim.SetTrigger("isPass");
        ball.GetComponent<Rigidbody>().velocity = kickDirection * FORCE * distanceGatePercent;

        Debug.Log("----------AI Kick to gate-------------" + kickDirection.ToString());
        isAction = true;
        Task.current.Succeed();
    }

    [Task]
    public void KickToGate()
    {
        Debug.Log("AI random kick to gate");

        Vector3 kickDirection = enemyGate.transform.position - ball.transform.position;
        kickDirection = kickDirection.normalized;
        kickDirection.y += Random.Range(0.5f, 3f);
        Kick(kickDirection);
    }

    [Task]
    protected virtual void KickToGateArea()
    {

        Vector3 kickDirection;

        Debug.Log("see player kick random with other angle");
        kickDirection = enemyGate.transform.position - ball.transform.position;
        kickDirection= kickDirection.normalized;
        kickDirection.z += Random.Range(rightGateCorner.transform.position.x - 1, leftGateCorner.transform.position.x - 1);
        kickDirection.y += Random.Range(0.5f, 3f);
        Kick(kickDirection);

    }
    [Task]
    public void KickToEnemy()
    {
        Vector3 kickDirection;
        Debug.Log("see player kick to enemy");
        kickDirection = other_player.transform.position - ball.transform.position;
        kickDirection = kickDirection.normalized;
        kickDirection.y = 0;
        ball.GetComponent<Rigidbody>().velocity = kickDirection * FORCE;
        isAction = true;
        Task.current.Succeed();
    }
    [Task]
    public void BlockEnemy()
    {
        Debug.Log("end go ball");
        Debug.Log("block go ball");
        anim.SetTrigger("isSidestep");
        Task.current.Succeed();

    }
    [Task]
    public void GateKeeper()
    {
        anim.SetTrigger("isBlock");
        Task.current.Succeed();

    }
    [Task]
    public void PlayerMove()
    {
        this.transform.position += SPEED * Time.deltaTime* transform.forward;
        Task.current.Succeed();

    }
    [Task]
   public void PassBall()
    {
       
        if (isSeeBall)
        {
            centreCircle = ball.transform.position;
            angle += Time.deltaTime * SPEED; // multiply all this with some speed variable (* speed);
            Debug.Log("ang=" + angle);
            if (angle < 2)
            {
                Debug.Log("ang=" + angle);
                Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * BALL_RADIUS;
                offset.y = 0;
                transform.position = centreCircle + offset;
                Task.current.Fail();
            }
            else { Task.current.Succeed(); }
        }
        else
        {
            Task.current.Succeed();
        }
    }
    

    [Task]
    public bool IsEnemyCloseToMe()
    {
        if (Vector3.Distance(other_player.transform.position, this.transform.position) <= CLOSE)
        {
            return true;
        }
        return false;

    }

}

