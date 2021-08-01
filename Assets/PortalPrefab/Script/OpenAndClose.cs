using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class OpenAndClose : MonoBehaviour
{

    bool portalClosing;
    public PostProcessVolume portalPostProcessEffects;
    public void togglePortalState()
    {
        if(portalClosing)
        {
            portalPostProcessEffects.enabled = true;
            portalClosing = false;
        }
        else
        {
            portalPostProcessEffects.enabled = false;
            portalClosing = true;
        }
    }
    public bool presentPortalState()
    {
        return portalClosing;
    }
    void Start()
    {
        portalClosing = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (portalClosing)
        {
            transform.localScale = new Vector3(Mathf.Lerp(transform.localScale.x, 0, 2.8f * Time.deltaTime), transform.localScale.y, transform.localScale.z);
            if(transform.localScale.x < 0.05f)
            {
                transform.localScale = new Vector3(transform.localScale.x, Mathf.Lerp(transform.localScale.y, 0, 3.5f * Time.deltaTime), transform.localScale.z);
            }
            if(transform.localScale.y < 0.05f)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, Mathf.Lerp(transform.localScale.z, 0, 4.2f * Time.deltaTime));
            }
            if(transform.localScale.z < 0.005f)
            {
                GetComponentInChildren<Portal>().enabled = false;
            }
        }
        else
        {
            GetComponentInChildren<Portal>().enabled = true;
            if(transform.localScale.y < 0.01f)
            {
                transform.localScale = new Vector3(transform.localScale.x, Mathf.Lerp(transform.localScale.y, 0.04f, 5.8f * Time.deltaTime), transform.localScale.z);
            }
            if(transform.localScale.y > 0.01f)
            {
                transform.localScale = new Vector3(Mathf.Lerp(transform.localScale.x, 1, 2.8f * Time.deltaTime), transform.localScale.y, transform.localScale.z);
            }
            if (transform.localScale.x > 0.88f)
            {
                transform.localScale = new Vector3(transform.localScale.x, Mathf.Lerp(transform.localScale.y, 1, 3.5f * Time.deltaTime), transform.localScale.z);
            }
            if (transform.localScale.y > 0.88f)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, Mathf.Lerp(transform.localScale.z, 1, 4.2f * Time.deltaTime));
            }
        }
    }
}
