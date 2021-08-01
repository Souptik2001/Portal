using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Portal : MonoBehaviour
{
    public Portal otherSidePortal;
    List<PortalTraveller> trackedTravellers;
    void Start()
    {
        trackedTravellers = new List<PortalTraveller>();
    }



    void onTravellerEnterPortal(PortalTraveller traveller)
    {
        if (!trackedTravellers.Contains(traveller))
        {
            traveller.prevOffsetFromPortal = traveller.transform.position - transform.position;
            traveller.EnterPortalThreshold();
            traveller.reversePortalSlice = Mathf.Sign(Vector3.Dot(transform.forward, traveller.transform.position - transform.position));
            if (!traveller.isClone)
            {
                traveller.reversePortalSlice = -traveller.reversePortalSlice;
                traveller.isClone = true;
                GameObject cloneObject = (GameObject)Instantiate(traveller.gameObject);
                traveller.isClone = false;
                cloneObject.GetComponent<PortalTraveller>().isClone = true;
                traveller.cloneObject = cloneObject.GetComponent<PortalTraveller>();
                Camera cloneCamera = cloneObject.GetComponentInChildren<Camera>();
                PostProcessLayer ppLayerClone = cloneObject.GetComponentInChildren<PostProcessLayer>();
                if (ppLayerClone) { Destroy(ppLayerClone); }
                if (cloneCamera){Destroy(cloneCamera);}
                var travellerRenderer = cloneObject.GetComponent<MeshRenderer>();
                if (travellerRenderer)
                {
                    foreach (var mat in travellerRenderer.materials)
                    {
                        mat.SetFloat("_sliceEnabled", 0);
                    }
                }
                var childRenderes = cloneObject.GetComponentsInChildren<MeshRenderer>();
                foreach (var childRenderer in childRenderes)
                {
                    foreach (var childMat in childRenderer.materials)
                    {
                        childMat.SetFloat("_sliceEnabled", 0);
                    }
                }
            }
            trackedTravellers.Add(traveller);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller && trackedTravellers.Contains(traveller))
        {
            var travellerRenderer = traveller.GetComponent<MeshRenderer>();
            if (travellerRenderer)
            {
                foreach (var mat in travellerRenderer.materials)
                {
                    mat.SetFloat("_sliceEnabled", 0);
                }
            }
            var childRenderes = traveller.GetComponentsInChildren<MeshRenderer>();
            foreach (var childRenderer in childRenderes)
            {
                foreach (var childMat in childRenderer.materials)
                {
                    childMat.SetFloat("_sliceEnabled", 0);
                }
            }
            if (traveller.cloneObject)
            {
                Destroy(traveller.cloneObject.gameObject);
            }
            traveller.ExitPortalThreshold();
            trackedTravellers.Remove(traveller);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        Vector3 sizeOfTraveller = other.bounds.size;
        Vector3 sizeOfPortal = GetComponent<Collider>().bounds.size;
        if (traveller && traveller.transform.position.x + sizeOfTraveller.x/2 <= transform.position.x + sizeOfPortal.x/2 && traveller.transform.position.x - sizeOfTraveller.x/2 >= transform.position.x - sizeOfPortal.x/2 && traveller.transform.position.y + sizeOfTraveller.y / 2 <= transform.position.y + sizeOfPortal.y / 2)
        {
            onTravellerEnterPortal(traveller);
        }
        else
        {
            // Debug.Log("Nope not allowed");
        }
    }


    private void LateUpdate()
    {
        for(int i=0; i<trackedTravellers.Count; i++)
        {
            PortalTraveller traveller = trackedTravellers[i];
            UpdateSliceParams(traveller);
            if (traveller.cloneObject)
            {
                var m = otherSidePortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * traveller.transform.localToWorldMatrix;
                traveller.cloneObject.transform.position = m.GetColumn(3);
                traveller.cloneObject.transform.rotation = m.rotation;
            }
            if (!traveller.isClone)
            {
                var m = otherSidePortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * traveller.transform.localToWorldMatrix;
                Vector3 offsetFromPortal = traveller.transform.position - transform.position;
                int portalSide = System.Math.Sign(Vector3.Dot(transform.forward, offsetFromPortal));
                int portalPrevSide = System.Math.Sign(Vector3.Dot(transform.forward, traveller.prevOffsetFromPortal));
                if (portalPrevSide != portalSide)
                {
                    m = otherSidePortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * traveller.transform.localToWorldMatrix;
                    Destroy(traveller.cloneObject.gameObject);
                    traveller.Teleport(transform, otherSidePortal.transform, m.GetColumn(3), m.rotation);
                    otherSidePortal.onTravellerEnterPortal(traveller);
                    trackedTravellers.RemoveAt(i);
                    i--;
                }
                traveller.prevOffsetFromPortal = offsetFromPortal;
            }
        }
    }


    void UpdateSliceParams(PortalTraveller traveller)
    {
        if (traveller)
        {
            var travellerRenderer = traveller.GetComponent<MeshRenderer>();
            Vector3 sizeOfPortal = GetComponent<Collider>().bounds.size;
            if (travellerRenderer)
            {
                foreach (var mat in travellerRenderer.materials)
                {
                    mat.SetFloat("_sliceEnabled", 1);
                    mat.SetVector("sliceCentre", transform.position + (transform.forward * 0.9f * -traveller.reversePortalSlice *(sizeOfPortal.z / 2)));
                    mat.SetVector("sliceNormal", traveller.reversePortalSlice * transform.forward);
                    // mat.SetFloat("sliceOffsetDst", sizeOfPortal.z / 2);
                    mat.SetFloat("sliceOffsetDst", 0);
                }
            }
            var childRenderes = traveller.GetComponentsInChildren<MeshRenderer>();
            foreach (var childRenderer in childRenderes)
            {
                foreach (var childMat in childRenderer.materials)
                {
                    childMat.SetFloat("_sliceEnabled", 1);
                    childMat.SetVector("sliceCentre", transform.position + (transform.forward * 0.9f * -traveller.reversePortalSlice * (sizeOfPortal.z / 2)));
                    childMat.SetVector("sliceNormal", traveller.reversePortalSlice * transform.forward);
                    // childMat.SetFloat("sliceOffsetDst", sizeOfPortal.z / 2);
                    childMat.SetFloat("sliceOffsetDst", 0);
                }
            }
        }
    }

}
