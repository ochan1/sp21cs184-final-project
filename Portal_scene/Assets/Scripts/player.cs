using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class player : MonoBehaviour
{
    // Start is called before the first frame update

    CharacterController characterController;


    public Text info;
    public float speed;
    private Vector3 moveDirection = Vector3.zero;

    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        speed = .25f;
    }

    // Update is called once per frame
    void Update()
    {
        /*
            Input.GetAxis("Horizontal") = 1 if player presses d, -1 if player presses a, 0 else
            Vector3 transform.right = (1, 0, 0) but in world space
            Vector3 transform.forward = (0, 0, 1) but in world space
        */
        moveDirection = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        characterController.Move(moveDirection * speed);
    }

    void OnControllerColliderHit(ControllerColliderHit controllerCollide) {
        Rigidbody body = controllerCollide.collider.attachedRigidbody;
         if (body == null || body.isKinematic)
            return;

        if (body.gameObject.tag == "Wall") {
            info.text = "You hit a wall";
        }

        // no rigidbody
       

        // We dont want to push objects below us
        if (controllerCollide.moveDirection.y < -0.3f)
            return;

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(controllerCollide.moveDirection.x, 0, controllerCollide.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        // body.velocity = pushDir * 2;
        body.AddForce(pushDir * 5);
    }


}
