using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tableBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody rb;
    private bool floorHit;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        floorHit = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision collision) {
        Debug.Log(rb.mass);
        if(collision.gameObject.tag == "Floor" && !floorHit){
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            floorHit = true;
            rb.mass = rb.mass * 1.5f;
        }
    }
}
