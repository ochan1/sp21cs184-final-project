using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPlacement : MonoBehaviour
{
    public GameObject portal0;
    public GameObject portal1;
    
    private GameObject inPortal;
    private GameObject outPortal;

    public Camera playerCamera;
    public Camera portal1Camera;
    public Camera portal0Camera;

    private void Awake()
    {
        inPortal = portal0;
        outPortal = portal1;
    }

    void OnGUI() {
        GUI.Box(new Rect(Screen.width/2,Screen.height/2, 10, 10), " ");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FirePortal(portal0, portal1, new Color(1.0f, 0.60f, 0.0f));
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            FirePortal(portal1, portal0, new Color(0.0f, 0.40f, 1.0f));
        }

        // SetPortalCamera();
        UpdatePortalCamera(portal1Camera, portal0, portal1);
        ClipPortalCameraView(portal1Camera, portal0, portal1);
        UpdatePortalCamera(portal0Camera, portal1, portal0);
        ClipPortalCameraView(portal0Camera, portal1, portal0);
    }

    private void FirePortal(GameObject portal, GameObject otherPortal, Color color)
    {
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        bool collision = Physics.Raycast(ray, out hit);
        
        if (collision) {
            
            if (otherPortal != null) {
                if (hit.transform.gameObject == otherPortal
                    || hit.transform.gameObject == otherPortal.transform.GetChild(0).gameObject) {
                    return;
                }
            }
            //
            
            Transform objectHit = hit.transform;
            // portal.GetComponent<PortalTeleportation>().wallCollider = hit.collider;
    
            portal.transform.position = hit.point;
            portal.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
            portal.gameObject.SetActive(true);
            
            portal.GetComponent<Renderer>().material.SetColor("_Color", color);
        }
    }

    // private void SetPortalCamera() {
    //     //set portalcamera to furthest portal
    //     Vector3 vec0 = portal0.transform.position - playerCamera.transform.position;
    //     Vector3 vec1 = portal1.transform.position - playerCamera.transform.position;
    //     float dist0 = (vec0).magnitude;
    //     float dist1 = (vec1).magnitude;
    //     float theta = Mathf.Cos(Mathf.Deg2Rad * playerCamera.fieldOfView / 2.0f);

    //     bool inView0 = Vector3.Dot(playerCamera.transform.forward, vec0) >= theta;
    //     bool inView1 = Vector3.Dot(playerCamera.transform.forward, vec1) >= theta;
    //     if (inView0 && inView1) {
    //         if (dist0 < dist1) {
    //             inPortal = portal0;
    //             outPortal = portal1;
    //         } else {
    //             inPortal = portal1;
    //             outPortal = portal0;
    //         }
    //     }
    //     else if (inView0) {
    //         inPortal = portal0;
    //         outPortal = portal1;
    //     } else {
    //         inPortal = portal1;
    //         outPortal = portal0;
    //     }
    // }

    private void UpdatePortalCamera(Camera portalCamera, GameObject inPortal, GameObject outPortal) {
        Quaternion flip = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        portalCamera.transform.position = outPortal.transform.TransformPoint(flip * inPortal.transform.InverseTransformPoint(playerCamera.transform.position));
        portalCamera.transform.rotation = outPortal.transform.rotation * flip * Quaternion.Inverse(inPortal.transform.rotation) * playerCamera.transform.rotation;
    }

    private void ClipPortalCameraView(Camera portalCamera, GameObject inPortal, GameObject outPortal) {
        //credits to Daniel Ilett
        Plane p = new Plane(-outPortal.transform.forward, outPortal.transform.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlane;
        portalCamera.projectionMatrix = playerCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
    }
}
