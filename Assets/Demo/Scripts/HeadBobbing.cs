using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobbing : MonoBehaviour
{


    float timer = 0.0f;
    public float bobbingSlowSpeed = 0.1f;
    public float bobbingFastSpeed = 0.13f;
    public float bobbingSlowAmount = 0.14f;
    public float bobbingFastAmount = 0.16f;
    public float midPoint = 1.0f;
    void Start()
    {
        
    }

    void Update()
    {
        float bobbingSpeed;
        float bobbingAmount;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            bobbingSpeed = bobbingFastSpeed;
            bobbingAmount = bobbingFastAmount;
        }
        else
        {
            bobbingSpeed = bobbingSlowSpeed;
            bobbingAmount = bobbingSlowAmount;
        }
        float waveSlice = 0.0f;
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //if (playerController.jumping == true)
        //{
        //    // input = new Vector2(0, 0);
        //    bobbingAmount = 0.0f;
        //    bobbingSpeed = 0.0f;
        //}
        if (input.x == 0 && input.y == 0) { timer = 0.0f; }
        else
        {
            waveSlice = Mathf.Sin(timer);
            timer += bobbingSpeed;
            if (timer > Mathf.PI * 2) { timer -= (Mathf.PI * 2); }
        }
        if (waveSlice != 0)
        {
            float translateChange = waveSlice * bobbingAmount;
            float totalAxes = Mathf.Abs(input.x) + Mathf.Abs(input.y);
            totalAxes = Mathf.Clamp(totalAxes, 0.0f, 1.0f);
            translateChange = totalAxes * translateChange;
            transform.localPosition = new Vector3(transform.localPosition.x, midPoint + translateChange, transform.localPosition.z);
        }


    }
}
