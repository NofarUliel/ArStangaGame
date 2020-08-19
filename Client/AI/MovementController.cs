using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    private Rigidbody rb;
    private Animation anim;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //anim = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        
        float y = Input.GetAxis("Vertical") ;
        float x = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(x, 0, y);
        rb.velocity = movement * 4;
        //if (x != 0 && y != 0)
        //{
        //    transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Atan2(x, y) * Mathf.Rad2Deg, transform.eulerAngles.z);
        //}
        //if (x != 0 || y != 0)
        //{
        //    anim.Play("walk");
        //}

    }
}
