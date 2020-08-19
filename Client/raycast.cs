using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raycast : MonoBehaviour
{
    [SerializeField]
    private GameObject gate;
    [SerializeField]
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 distance = gate.transform.position - this.transform.position;


        if (Physics.Raycast(this.transform.position+new Vector3(0,0.5f,0), distance, out hit))
        {
            Debug.DrawRay(this.transform.position + new Vector3(0, 0.5f, 0), distance, Color.red);
            Debug.DrawRay(this.transform.position, hit.point, Color.blue);
            Debug.Log(hit.collider.name);
            if (hit.collider.gameObject.tag == "HumanPlayer")
            {
                Debug.Log("hit HumanPlayer");
            }
    } }
}
