using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleportation : MonoBehaviour
{
    private Dictionary<Collider, int> objectCountInPortal = new Dictionary<Collider, int>();
    
    GameObject inPortal;
    GameObject outPortal;
    public Collider wallCollider = null;

    // Start is called before the first frame update
    void Start()
    {
        wallCollider = this.GetComponent<Collider>();
        if (this.name == "Portal 0") {
            inPortal = this.gameObject;
            outPortal = this.transform.parent.Find("Portal 1").gameObject;
        } else {
            inPortal = this.gameObject;
            outPortal = this.transform.parent.Find("Portal 0").gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var obj in objectCountInPortal) {
            Vector3 objPos = transform.InverseTransformPoint(obj.Key.transform.position);
            if (objPos.z > 0.0f && obj.Value == 0) {
                teleport(obj.Key);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!objectCountInPortal.ContainsKey(other)) {
            objectCountInPortal.Add(other, 0);
        } else {
            objectCountInPortal[other] += 1;
        }
        Physics.IgnoreCollision(other, wallCollider, true);
        // Physics.IgnoreCollision(other, outPortal.GetComponent<PortalTeleportation>().wallCollider, true);
    }

    private void OnTriggerExit(Collider other) {
        objectCountInPortal[other] -= 1;
        Physics.IgnoreCollision(other, wallCollider, false);
        // Physics.IgnoreCollision(other, outPortal.GetComponent<PortalTeleportation>().wallCollider, false);
    }

    private void teleport(Collider other)
    {
        Quaternion flip = Quaternion.Euler(0.0f, 180.0f, 0.0f);

        Vector3 relativePos = inPortal.transform.InverseTransformPoint(other.transform.position);
        other.transform.position = outPortal.transform.TransformPoint(relativePos);

        Quaternion relativeRot = flip * Quaternion.Inverse(inPortal.transform.rotation) * other.transform.rotation;
        other.transform.rotation = outPortal.transform.rotation * relativeRot;
    }
}
