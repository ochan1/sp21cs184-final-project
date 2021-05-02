using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public float rotationSpeed = 2f;
    
    private float red = 0f;
    private float green = 0f;
    private float blue = 1f;
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

        red += 0.05f;
        if(red >= 1f){
            red = 0f;
        }
        blue -= 0.05f;
        if(blue <= 0f){
            blue = 1f;
        }

        ParticleSystem.MainModule settings = GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(new Color(red, green, blue, 0.5f));
    }
}
