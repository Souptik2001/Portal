using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarPortalThrowableTrail : MonoBehaviour
{
    [HideInInspector]
    public Transform leader;
    void Start()
    {
        
    }

    void Update()
    {
        if (leader)
        {
            transform.position = leader.position;
        }
    }
}
