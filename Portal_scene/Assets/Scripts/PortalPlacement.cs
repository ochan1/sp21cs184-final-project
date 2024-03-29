using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPlacement : MonoBehaviour
{
    public GameObject portal0;
    public GameObject portal1;

    public GameObject emitter0;
    public GameObject emitter1;
    
    private GameObject inPortal;
    private GameObject outPortal;

    public Camera playerCamera;
    public Camera portal1Camera;
    public Camera portal0Camera;

    private int iterations = 3;

    private bool tempDeactivated = false;
    private Vector3 previousPortalPos0;
    private Vector3 previousPortalPos1;

    private void Awake()
    {
        inPortal = portal0;
        outPortal = portal1;
        previousPortalPos0 = new Vector3(0, 0, 0);
        previousPortalPos1 = new Vector3(0, 0, 0);
    }

    void OnGUI() {
        GUI.Box(new Rect(Screen.width/2,Screen.height/2, 10, 10), " ");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FirePortal(portal0, portal0Camera, portal1, new Color(1.0f, 0.60f, 0.0f), emitter0);
            previousPortalPos0 = portal0.transform.position;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            FirePortal(portal1, portal1Camera, portal0, new Color(0.0f, 0.40f, 1.0f), emitter1);
            previousPortalPos1 = portal1.transform.position;
        }
        
        UpdateCamera(portal1Camera, portal0, portal1);
        UpdatePortalCamera(portal1Camera, portal0, portal1);
        ClipPortalCameraView(portal1Camera, portal0, portal1);
        
        UpdateCamera(portal0Camera, portal1, portal0);
        UpdatePortalCamera(portal0Camera, portal1, portal0);
        ClipPortalCameraView(portal0Camera, portal1, portal0);
        
        CheckInView(portal0, portal1);
    }

    private void FirePortal(GameObject portal, Camera portalCamera, GameObject otherPortal, Color color,
                            GameObject emitter)
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
                //Avoid with original portal as well (this is for wall collider purposes)
                if (hit.transform.gameObject == portal
                    || hit.transform.gameObject == portal.transform.GetChild(0).gameObject) {
                    return;
                }
            }
            //
            
            Transform objectHit = hit.transform;
            if (portal.GetComponent<PortalTeleportation>() != null) {
                portal.GetComponent<PortalTeleportation>().wallCollider = hit.collider;
            }
    
            portal.transform.position = hit.point;
            portal.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            
            // Don't rotate the portal, keep it rightside up (but can rotate it on other x and y axis)
            Vector3 portal_angles = portal.transform.eulerAngles;
            portal_angles.z = 0.0f;
            portal.transform.eulerAngles = portal_angles;

            portal.gameObject.SetActive(true);
            emitter.transform.position = portal.transform.position;
            // emitter.transform.rotation = portal.transform.rotation;
            emitter.transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal);
            
            Vector3 emitter_angles = emitter.transform.eulerAngles;
            emitter_angles.y = emitter_angles.y + 180;
            emitter.transform.eulerAngles = emitter_angles;

            emitter.transform.localScale = new Vector3(1f, 1f, 1f);
            emitter.gameObject.SetActive(true);
            portalCamera.gameObject.SetActive(true);
            
            // portal.GetComponent<Renderer>().material.SetColor("_Color", color);
        }
    }

    private void UpdatePortalCamera(Camera portalCamera, GameObject inPortal, GameObject outPortal) {
        Quaternion flip = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        portalCamera.transform.position = outPortal.transform.TransformPoint(flip * inPortal.transform.InverseTransformPoint(playerCamera.transform.position));
        portalCamera.transform.rotation = outPortal.transform.rotation * flip * Quaternion.Inverse(inPortal.transform.rotation) * playerCamera.transform.rotation;
    }

    private void ClipPortalCameraView(Camera portalCamera, GameObject inPortal, GameObject outPortal) {
        // credits to Daniel Ilett
        Plane p = new Plane(outPortal.transform.forward, outPortal.transform.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace = Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlane;
        portalCamera.projectionMatrix = playerCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
    }

    void UpdateCamera(Camera portalCamera, GameObject inPortal, GameObject outPortal)
    {
        if (!inPortal.activeSelf || !outPortal.activeSelf)
        {
            return;
        }
       
        for (int i = iterations - 1; i >= 0; --i)
        {
            RenderCamera(portalCamera, inPortal, outPortal, i);
        }
    }

    private void RenderCamera(Camera portalCamera, GameObject inPortal, GameObject outPortal, int iterationID)
    {
        UpdatePortalCamera(portalCamera, inPortal, outPortal);

        // credits to Daniel Ilett
        for(int i = 0; i <= iterationID; ++i)
        {
            // Position the camera behind the other portal.
            Vector3 relativePos = inPortal.transform.InverseTransformPoint(portalCamera.transform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            portalCamera.transform.position = outPortal.transform.TransformPoint(relativePos);

            // Rotate the camera to look through the other portal.
            Quaternion relativeRot = Quaternion.Inverse(inPortal.transform.rotation) * portalCamera.transform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            portalCamera.transform.rotation = outPortal.transform.rotation * relativeRot;
        }

        // Set the camera's oblique view frustum.
        ClipPortalCameraView(portalCamera, inPortal, outPortal);

        // Render the camera to its render target.
        // Fixed - Causing slowdown from rendering. Solution: Change texture resolution to 480p
        Debug.Log(portalCamera.projectionMatrix);
        portalCamera.Render();
        // if (IsInView(portalCamera, outPortal.transform.position)) {
        //     portalCamera.Render();
        // } else {
        //     Debug.Log("LOL Bug get recked");
        // }
    }

    private void CheckInView(GameObject inPortal, GameObject outPortal) {
        if(tempDeactivated) {
            bool prevOnScreen0 = IsInView(previousPortalPos0);
            bool prevOnScreen1 = IsInView(previousPortalPos1);
            if(prevOnScreen0 || prevOnScreen1) {
                inPortal.SetActive(true);
                outPortal.SetActive(true);
                tempDeactivated = false;
                previousPortalPos0 = new Vector3(0, 0, 0);
                previousPortalPos1 = new Vector3(0, 0, 0);
                return;
            }
        }
        if (!inPortal.activeSelf || !outPortal.activeSelf)
        {
            return;
        }
        bool onScreen0 = IsInView(inPortal.transform.position);
        bool onScreen1 = IsInView(outPortal.transform.position);

        if(!onScreen0 && !onScreen1) {
            if(Vector3.Distance(inPortal.transform.position, playerCamera.transform.position) > 3 &&
               Vector3.Distance(outPortal.transform.position, playerCamera.transform.position) > 3) {
                // Despawn if BOTH are out of my range (and out of my sight via the original "if")
                previousPortalPos0 = inPortal.transform.position;
                previousPortalPos1 = outPortal.transform.position;
                inPortal.SetActive(false);
                outPortal.SetActive(false);
                tempDeactivated = true;
                return;
            }
        }

    }

    private bool IsInView(Camera referenceCam, Vector3 portalPos) {
        Vector3 portalPoint = referenceCam.WorldToViewportPoint(portalPos);
        bool onScreen = portalPoint.x > -0.5 && portalPoint.x < 1.5 &&
                         portalPoint.y > -0.5 && portalPoint.y < 1.5 &&
                         portalPoint.z > 0;
        return onScreen;
    }

    private bool IsInView(Vector3 portalPos) {
        return IsInView(playerCamera, portalPos);
    }

}

