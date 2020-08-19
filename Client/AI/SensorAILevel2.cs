using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorAILevel2 : BasePlayer

{
    private const float MAX_DISTANCE = 3f;
    private const float FRONT_SIDE_SENSOR_POS = 0.2f;
    private const float FRONT_SENSOR_ANGLE = 30;
    private Vector3 frontSensorPosition = new Vector3(0, 0.2f, 0.5f);
    private bool avoiding = false;
    private float startAngle;
    protected override void Start()
    {
        base.Start();
        startAngle = this.transform.eulerAngles.y;
        Debug.Log("startAngle" + startAngle);
    }
    private void FixedUpdate()
    {

        RaycastHit hit;
        float avoidMultiplier = 0; //turn right>0 turn left<0 center=0
        avoiding = false;
        Vector3 sensorPos = this.transform.position;
        sensorPos.y += 0.5f;
        Vector3 sensorStartPos = this.transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;

        sensorStartPos += transform.right * FRONT_SIDE_SENSOR_POS;
        //front right sensor
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, MAX_DISTANCE))
        {
            if (hit.collider.gameObject.tag.Equals("wall"))
            {
                Debug.DrawLine(sensorPos, hit.point, Color.green);
                avoiding = true;
                avoidMultiplier -= 1f;
                Debug.Log("avoidMultiplier = " + avoidMultiplier);
            }

        }
        //front right angle sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(FRONT_SENSOR_ANGLE, transform.up) * transform.forward, out hit, MAX_DISTANCE))
        {
            if (hit.collider.gameObject.tag.Equals("wall"))
            {
                Debug.DrawLine(sensorPos, hit.point, Color.green);
                avoiding = true;
                avoidMultiplier -= 0.5f;
            }

        }
        sensorStartPos -= 2 * transform.right * FRONT_SIDE_SENSOR_POS; ;
        //front left sensor
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, MAX_DISTANCE))
        {
            if (hit.collider.gameObject.tag.Equals("wall"))
            {
                Debug.DrawLine(sensorPos, hit.point, Color.green);
                avoiding = true;
                avoidMultiplier += 1f;
            }
        }

        //front left angle sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-FRONT_SENSOR_ANGLE, transform.up) * transform.forward, out hit, MAX_DISTANCE))
        {
            if (hit.collider.gameObject.tag.Equals("wall"))
            {
                Debug.DrawLine(sensorPos, hit.point, Color.green);
                avoiding = true;
                avoidMultiplier += 0.5f;
            }
        }
        sensorStartPos = this.transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;
        //front center forward sensor
        if (avoidMultiplier == 0)
        {
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, MAX_DISTANCE))
            {
                if (hit.collider.gameObject.tag.Equals("wall"))
                {
                    Debug.DrawLine(sensorPos, hit.point, Color.green);
                    avoiding = true;
                    if (hit.normal.x < 0)
                    {
                        avoidMultiplier = -1;
                    }
                    else { avoidMultiplier = 1; }
                }

            }
        }
        if (avoiding)
        {
            Debug.Log("multi=" + avoidMultiplier);
            float y_rotation = transform.rotation.eulerAngles.y + (avoidMultiplier * FRONT_SENSOR_ANGLE);
            transform.rotation = Quaternion.Euler(0, y_rotation, 0);
            this.transform.position += SPEED * 4 * Time.deltaTime * transform.forward;
        }




    }
}