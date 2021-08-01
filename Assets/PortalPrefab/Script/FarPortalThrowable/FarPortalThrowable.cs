using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FarPortalThrowable : MonoBehaviour
{

    Rigidbody rb;
    public float throwPower = 10f;
    public LayerMask portalOriginLayers;
    [HideInInspector]
    public PortalTraveller traveller;
    [HideInInspector]
    public bool collided=false;
    [HideInInspector]
    public bool retracting=false;
    [HideInInspector]
    public GameObject followingTrail;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        if (collided)
        {
            if(traveller)traveller.farPortalReady = true;
            if (traveller) traveller.throwableMoving = false;
            collided = false;
        }
        if (retracting)
        {
            rb.isKinematic = true;
            transform.position = Vector3.Lerp(transform.position, traveller.transform.position, 0.85f);
            if(Vector3.Magnitude(transform.position - traveller.transform.position) < 0.01f)
            {
                retracting = false;
                traveller.throwableMoving = false;
                Destroy(gameObject);
                Destroy(followingTrail);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            collided = true;
            rb.isKinematic = true;
            if(traveller)traveller.farPortalReady = true;
            if (traveller) traveller.throwableMoving = false;
        }
    }

    public void Throw(LayerMask lm)
    {
        // portalOriginLayers = lm;
        gameObject.SetActive(true);
        rb = GetComponent<Rigidbody>();
        rb.transform.parent = null;
        rb.isKinematic = false;
        rb.AddForce(transform.forward * throwPower, ForceMode.Impulse);
    }
    public void Retract()
    {
        retracting = true;
    }
}
