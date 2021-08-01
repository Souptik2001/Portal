using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTraveller : MonoBehaviour
{
    public bool isClone = true;
    public GameObject portalPrefab;
    public GameObject farPortalThrowablePrefab;
    public GameObject farPortalThrowableTrailPrefab;
    [HideInInspector]
    public Vector3 farPortalPosition;
    [HideInInspector]
    public Transform farPortalThrowablePosition;
    [HideInInspector]
    public bool farPortalReady=false;
    [HideInInspector]
    public bool portalOpen=false;
    [HideInInspector]
    public bool throwableMoving=false;
    [HideInInspector]
    public GameObject farPortalThrowableGameObject;
    [HideInInspector]
    public PortalTraveller cloneObject;
    [HideInInspector]
    public float reversePortalSlice;
    [HideInInspector]
    public GameObject nearPortal;
    [HideInInspector]
    public GameObject farPortal;
    [HideInInspector]
    public Vector3 portalDimension;
    public Vector3 prevOffsetFromPortal { get; set; }
    public LayerMask portalOriginPlatforms;
    [HideInInspector]
    public float maxDistancePortalOrigin = 5f;
    public virtual void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        // Debug.Log("Teleported");
        // Debug.DrawRay(pos, Vector3.up * 10, Color.red);
        transform.position = pos;
        transform.rotation = rot;
    }


    public virtual void CreatePortal(GameObject portalPrefab)
    {
        if (!isClone)
        {
            portalDimension = portalPrefab.transform.GetChild(0).transform.localScale;
            nearPortal = (GameObject)Instantiate(portalPrefab);
            farPortal = (GameObject)Instantiate(portalPrefab);
            nearPortal.GetComponentInChildren<Portal>().otherSidePortal = farPortal.GetComponentInChildren<Portal>();
            farPortal.GetComponentInChildren<Portal>().otherSidePortal = nearPortal.GetComponentInChildren<Portal>();
        }

    }

    public virtual void EnterPortalThreshold()
    {

    }

    public virtual void ExitPortalThreshold()
    {

    }


    public virtual void togglePortalStateManual(Vector3 nearPortalPosition, Vector3 farPortalPos)
    {
        if (!isClone)
        {
            if (nearPortal.GetComponent<OpenAndClose>().presentPortalState())
            {
                nearPortal.transform.position = nearPortalPosition;
                farPortal.transform.position = farPortalPos;
            }
            nearPortal.GetComponent<OpenAndClose>().togglePortalState();
            farPortal.GetComponent<OpenAndClose>().togglePortalState();
        }
    }

    public virtual void togglePortalState(Transform camTransform)
    {
        if (!farPortalReady)
        {
            Debug.Log("Far Portal not ready");
            return;
        }
        if (!isClone && nearPortal != null && farPortal != null)
        {
            if (!portalOpen)
            {
                RaycastHit hit;
                if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, maxDistancePortalOrigin, portalOriginPlatforms))
                {
                    RaycastHit hitBox;
                    Vector3 centerOfPortal = hit.point + (Vector3.up * (portalDimension.y / 2));
                    if (Physics.BoxCast(hit.point, new Vector3(portalDimension.x, 1, portalDimension.z) / 2, hit.point + Vector3.up, out hitBox, transform.rotation, portalDimension.y, portalOriginPlatforms))
                    {
                        Debug.Log("Sorry portal can't be formed");
                    }
                    else
                    {
                        if (nearPortal.GetComponent<OpenAndClose>().presentPortalState())
                        {
                            RaycastHit hit2;
                            if (Physics.Raycast(farPortalThrowablePosition.position, Vector3.down, out hit2, maxDistancePortalOrigin, portalOriginPlatforms))
                            {
                                farPortalPosition = hit2.point + (Vector3.up * (portalDimension.y / 2));
                                nearPortal.transform.position = centerOfPortal;
                                farPortal.transform.position = farPortalPosition;
                                // nearPortal.transform.rotation = transform.rotation;
                                // farPortal.transform.rotation = transform.rotation;
                                nearPortal.GetComponent<OpenAndClose>().togglePortalState();
                                farPortal.GetComponent<OpenAndClose>().togglePortalState();
                                portalOpen = true;
                            }
                            else
                            {
                                Debug.Log("No platforms near to creat the far portal");
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("No platforms near to create the near portal");
                }
            }
            else
            {
                nearPortal.GetComponent<OpenAndClose>().togglePortalState();
                farPortal.GetComponent<OpenAndClose>().togglePortalState();
                portalOpen = false;
            }
        }
    }

    public virtual void ThrowPortal()
    {
        if (!farPortalReady && !throwableMoving)
        {
            throwableMoving = true;
            farPortalThrowableGameObject = (GameObject)Instantiate(farPortalThrowablePrefab, Camera.main.transform);
            GameObject followingTrail = Instantiate(farPortalThrowableTrailPrefab, farPortalThrowableGameObject.transform);
            followingTrail.GetComponent<FarPortalThrowableTrail>().leader = farPortalThrowableGameObject.transform;
            farPortalThrowablePosition = farPortalThrowableGameObject.transform;
            farPortalThrowableGameObject.GetComponent<FarPortalThrowable>().traveller = this;
            farPortalThrowableGameObject.GetComponent<FarPortalThrowable>().followingTrail = followingTrail;
            farPortalThrowableGameObject.GetComponent<FarPortalThrowable>().Throw(portalOriginPlatforms);
        }
        else
        {
            if (!portalOpen)
            {
                Debug.Log("Retracting");
                throwableMoving = true;
                farPortalReady = false;
                farPortalThrowableGameObject.GetComponent<FarPortalThrowable>().Retract();
            }
        }
    }

}
