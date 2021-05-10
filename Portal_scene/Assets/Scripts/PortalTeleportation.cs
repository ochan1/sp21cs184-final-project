using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleportation : MonoBehaviour
{
    GameObject inPortal;
    GameObject outPortal;
    Collider teleportingObject;
    public Collider wallCollider = null;
    public float teleportOffset = 1.1f;

    private bool objectInPortal = false;

    // Start is called before the first frame update
    void Start()
    {
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
        // if (objectInPortal) {
        //     Vector3 objPos = transform.InverseTransformPoint(teleportingObject.transform.position);
		// 	if (objPos.z < 0.0f)
		// 	{
		// 		teleport();
        //         objectInPortal = false;
		// 	}
		// }
    }

    void FixedUpdate() {
        if (objectInPortal) {
            Vector3 objPos = transform.InverseTransformPoint(teleportingObject.transform.position);
			if (objPos.z < 0.0f)
			{
				teleport();
                objectInPortal = false;
			}
		}
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" && inPortal.activeSelf && outPortal.activeSelf) {
            teleportingObject = other;
            objectInPortal = true;
            Physics.IgnoreCollision(other, wallCollider, true);
            Physics.IgnoreCollision(other, outPortal.GetComponent<PortalTeleportation>().wallCollider, true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player" && inPortal.activeSelf && outPortal.activeSelf) {
            objectInPortal = false;
            Physics.IgnoreCollision(other, wallCollider, false);
            Physics.IgnoreCollision(other, outPortal.GetComponent<PortalTeleportation>().wallCollider, false);
        }
    }

    private void teleport()
    {
        Quaternion flip = Quaternion.Euler(0.0f, 180.0f, 0.0f);

        Vector3 relativePos = inPortal.transform.InverseTransformPoint(teleportingObject.transform.position);
        teleportingObject.transform.position = outPortal.transform.TransformPoint(relativePos) + outPortal.transform.forward * teleportOffset;

        Quaternion relativeRot = flip * Quaternion.Inverse(inPortal.transform.rotation) * teleportingObject.transform.rotation;
        teleportingObject.transform.rotation = outPortal.transform.rotation * relativeRot;

    }
}
