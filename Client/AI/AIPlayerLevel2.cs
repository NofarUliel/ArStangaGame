using Panda;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Vuforia;
using Panda.Examples.PlayTag;


public class AIPlayerLevel2 : AIPlayerLevel1
{
    private bool isFound;
    private Vector3 kickDirection, shortestDirection;
    private float startX, minDistance;

    private void Awake()
    {
        Init();
    }
    public void Init()
    {
        isFound = false;
        startX = leftGateCorner.transform.position.x;
        minDistance = Vector3.Distance(my_gate.transform.position, enemyGate.transform.position);
        shortestDirection = new Vector3(0, 0, 0);
    }
  
    //public void Sensors(string hitTarget)
    //{
    //    RaycastHit hit;
    //    float avoidMultiplier = 0; //turn right>0 turn left<0 center=0
    //    avoiding = false;
    //    Vector3 sensorPos = this.transform.position;
    //    sensorPos.y += 0.5f;
    //    Vector3 sensorStartPos = this.transform.position;
    //    sensorStartPos += transform.forward * frontSensorPosition.z;
    //    sensorStartPos += transform.up * frontSensorPosition.y;

    //    sensorStartPos += transform.right * FRONT_SIDE_SENSOR_POS;
    //    //front right sensor
    //    if (Physics.Raycast(sensorStartPos, transform.forward, out hit, MAX_DISTANCE))
    //    {
    //        if (hit.collider.gameObject.tag.Equals(hitTarget))
    //        {
    //            Debug.DrawLine(sensorPos, hit.point, Color.green);
    //            avoiding = true;
    //            avoidMultiplier -= 1f;
    //            Debug.Log("avoidMultiplier = " + avoidMultiplier);
    //        }

    //    }
    //    //front right angle sensor
    //    else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(FRONT_SENSOR_ANGLE, transform.up) * transform.forward, out hit, MAX_DISTANCE))
    //    {
    //        if (hit.collider.gameObject.tag.Equals(hitTarget))
    //        {
    //            Debug.DrawLine(sensorPos, hit.point, Color.green);
    //            avoiding = true;
    //            avoidMultiplier -= 0.5f;
    //        }

    //    }
    //    sensorStartPos -= 2 * transform.right * FRONT_SIDE_SENSOR_POS; ;
    //    //front left sensor
    //    if (Physics.Raycast(sensorStartPos, transform.forward, out hit, MAX_DISTANCE))
    //    {
    //        if (hit.collider.gameObject.tag.Equals(hitTarget))
    //        {
    //            Debug.DrawLine(sensorPos, hit.point, Color.green);
    //            avoiding = true;
    //            avoidMultiplier += 1f;
    //        }
    //    }

    //    //front left angle sensor
    //    else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-FRONT_SENSOR_ANGLE, transform.up) * transform.forward, out hit, MAX_DISTANCE))
    //    {
    //        if (hit.collider.gameObject.tag.Equals(hitTarget))
    //        {
    //            Debug.DrawLine(sensorPos, hit.point, Color.green);
    //            avoiding = true;
    //            avoidMultiplier += 0.5f;
    //        }
    //    }
    //    sensorStartPos = this.transform.position;
    //    sensorStartPos += transform.forward * frontSensorPosition.z;
    //    sensorStartPos += transform.up * frontSensorPosition.y;
    //    //front center forward sensor
    //    if (avoidMultiplier == 0)
    //    {
    //        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, MAX_DISTANCE))
    //        {
    //            if (hit.collider.gameObject.tag.Equals(hitTarget))
    //            {
    //                Debug.DrawLine(sensorPos, hit.point, Color.green);
    //                avoiding = true;
    //                if (hit.normal.x < 0)
    //                {
    //                    avoidMultiplier = -1;
    //                }
    //                else { avoidMultiplier = 1; }
    //            }

    //        }
    //    }
    //    if (avoiding)
    //    {
    //        Debug.Log("multi=" + avoidMultiplier);
    //        float y_rotation = transform.rotation.eulerAngles.y + (avoidMultiplier * FRONT_SENSOR_ANGLE);
    //        transform.rotation = Quaternion.Euler(0, y_rotation, 0);
    //        this.transform.position += SPEED*4 * Time.deltaTime * transform.forward;
    //    }
      

    //}


    [Task]
    protected override void KickToGateArea()
    {//find angle with shortest distance
    
        Debug.Log("startX= " + startX);
        Debug.Log("rightGateCorner x= " + rightGateCorner.transform.position.x);
        if (startX < rightGateCorner.transform.position.x)
        {
            startX += 0.5f;
            kickDirection = enemyGate.transform.position - this.transform.position;
            kickDirection = kickDirection.normalized;
            kickDirection.z += startX;
            kickDirection.y += Random.Range(0.5f, 3f);
            Debug.DrawRay(this.transform.position+new Vector3(0,0.5f,0), kickDirection, Color.black);

            if (!IsSeePlayer(kickDirection))
            {
                float dis = Vector3.Distance(kickDirection, enemyGate.transform.position);
                if (dis < minDistance)
                {
                    minDistance = dis;
                    shortestDirection = kickDirection;
                    isFound = true;
                    Debug.Log("minDistance =" + minDistance);
                }
            }

        }
        if (startX >= rightGateCorner.transform.position.x)
        {
            if (isFound)
            {
                Debug.Log("found shortest angle");
                Debug.DrawRay(this.transform.position + new Vector3(0, 0.7f, 0), shortestDirection, Color.blue);
                Kick(shortestDirection);

            }
            else
            {//random kick
                Debug.Log("not found shortest angle");
                base.KickToGateArea();
            }
            Init();
        }


    }
    //[Task]
    //protected override void KickToGateArea()
    //{//find angle with shortest distance
    //    bool isFound = false;
    //    Vector3 kickDirection, shortestDirection = new Vector3(0, 0, 0);
    //    float startX = leftGateCorner.transform.position.x;
    //    float minDistance = Vector3.Distance(my_gate.transform.position, enemyGate.transform.position);
    //    Debug.Log("startX= " + startX);
    //    Debug.Log("rightGateCorner x= " + rightGateCorner.transform.position.x);
    //    if (startX < rightGateCorner.transform.position.x)
    //    {
    //        startX += 0.5f;
    //        kickDirection = new Vector3(startX, enemyGate.transform.position.y, enemyGate.transform.position.z);
    //        Vector3 distance = enemyGate.transform.position - kickDirection;
    //        if (!IsSeePlayer(distance))
    //         {
    //            float dis = Vector3.Distance(kickDirection, enemyGate.transform.position);
    //            if (dis< minDistance)
    //            {
    //                minDistance = dis;
    //                shortestDirection = kickDirection;
    //                isFound = true;
    //            }

    //         }

    //    }
    //    if (startX >= rightGateCorner.transform.position.x)
    //    {
    //        if (isFound)
    //        {
    //            Debug.Log("found shortest angle");
    //            Kick(shortestDirection);

    //        }
    //        else
    //        {//random kick
    //            Debug.Log("not found shortest angle");
    //            base.KickToGateArea();
    //        }
    //    }


    //}



}

//tree("Root")

//    parallel
//        repeat mute tree("MyTurn")

//        repeat mute tree("EnemyTurn")


//tree("MyTurn")
//	while IsMyTurn
//        sequence

//            LookAtTarget("ball")

//            GoToBall
//            fallback

//                tree("BallBehindPlayer")
//				while SeePlayer
//                    sequence

//                        KickToGateArea
//                        SwitchTurn
//				while not SeePlayer

//                    tree("Attack")


//tree("EnemyTurn")
//	while not IsMyTurn

//        fallback
//			while IsBallCloseToMyGate
//                tree("GoalKepper")
//			while not IsBallCloseToMyGate

//                tree("BlockEnemy")






//tree("BlockEnemy")

//    sequence
//        GoToEnemy

//        BlockEnemy

//tree("GoalKepper")

//    sequence
//        fallback
//			while not IsPlayerCloseToHisGate

//                sequence
//                    LookAtTarget("myGate")

//                    GoToMyGate
//			while IsPlayerCloseToHisGate
//                sequence

//                    LookAtTarget("enemyGate")

//                    GateKeeper

//tree("Attack")

//    sequence
//        LookAtTarget("enemyGate")

//        KickToGate
//        SwitchTurn



//tree("BallBehindPlayer")
//	while IsBallBehindPlayer
//        sequence

//            PlayerGoBehindBall
//            LookAtTarget("ball")
