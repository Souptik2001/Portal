using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : PortalTraveller
{
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if(Vector3.Magnitude(rb.velocity) < 4 && !isClone)
        {
            rb.AddForce(transform.forward * 600 * Time.fixedDeltaTime);
        }
    }
}
