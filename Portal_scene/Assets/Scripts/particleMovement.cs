using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public float rotationSpeed = 2f;
    void Start()
    {
        // this.GetComponent<ParticleSystem>().main.startColor = new Color(1, 0, 1, .5f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 emitter_angles = this.gameObject.transform.eulerAngles;
        emitter_angles.z = emitter_angles.z += rotationSpeed;
        this.gameObject.transform.eulerAngles = emitter_angles;
    }
}
