using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    // Start is called before the first frame update
    // public GameObject player;
    public float mouseSensitivity;
    public Transform body;
    float upDownRotation = 0f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        upDownRotation -= mouseY;
        upDownRotation = Mathf.Clamp(upDownRotation, -90f, 90f);    // Don't bend over backwards to look

        transform.localRotation = Quaternion.Euler(upDownRotation, 0f, 0f);     // have the camera look up or down
        body.Rotate(Vector3.up * mouseX);     // turn yourself (the player) left or right, spinning
    }

    // void LateUpdate(){
    //     transform.position = player.transform.position;
    // }
}