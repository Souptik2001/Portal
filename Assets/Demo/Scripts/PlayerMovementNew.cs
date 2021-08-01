using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovementNew : PortalTraveller
{
    public float normalSpeed = 700f;
    public float sprintSpeed = 900f;
    public float airSpeed = 400f;
    public float maxSpeed = 20f;
    float speed;
    public float sensitivity = 10f;
    public Camera cam;
    Rigidbody rb;
    public Transform jumpRay;
    private Vector2 velocity;
    public float jumpForce = 100f;

    public float maxStepHeight = 0.25f;
    public int stepDetail = 1;
    public LayerMask stepMask;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Application.targetFrameRate = 100;
    }

    void Update()
    {
        if(nearPortal == null && farPortal == null && !isClone) { CreatePortal(portalPrefab); }
        // Debug.DrawRay(cam.transform.position, cam.transform.forward * 50, Color.red);
        if (isClone) { return; }
        if (Input.GetKeyDown(KeyCode.LeftControl) && cam)
        {
            togglePortalState(cam.transform);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ThrowPortal();
        }
        rb.AddForce(new Vector3(0, -10f, 0));
        bool isGrounded = Physics.BoxCastAll(jumpRay.transform.position, new Vector3(.2f, .1f, .2f), Vector3.down, Quaternion.identity, .1f).Length > 1;
        Vector2 xMov = new Vector2(Input.GetAxisRaw("Horizontal") * transform.right.x, Input.GetAxisRaw("Horizontal") * transform.right.z);
        Vector2 zMov = new Vector2(Input.GetAxisRaw("Vertical") * transform.forward.x, Input.GetAxisRaw("Vertical") * transform.forward.z);

        if (!isGrounded)
        {
            speed = airSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = sprintSpeed;
            // animator.speed = 1.3f;
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            speed = normalSpeed;
            // animator.speed = 1;
        }

        //Rotation
        float yRot = Input.GetAxisRaw("Mouse X") * sensitivity;
        rb.rotation *= Quaternion.Euler(0, yRot, 0);
        if (cam)
        {
            float xRot = Input.GetAxisRaw("Mouse Y") * sensitivity;
            float x_rot = cam.transform.rotation.eulerAngles.x;
            x_rot -= xRot;

            //cam.transform.rotation = Quaternion.Euler(x_rot, cam.transform.rotation.eulerAngles.y, cam.transform.rotation.eulerAngles.z);

            float camEulerAngleX = cam.transform.localEulerAngles.x;

            camEulerAngleX -= xRot * sensitivity;

            if (camEulerAngleX < 180)
            {
                camEulerAngleX += 360;
            }

            camEulerAngleX = Mathf.Clamp(camEulerAngleX, 270, 450);

            cam.transform.localEulerAngles = new Vector3(camEulerAngleX, 0, 0);
        }

        //Jumping

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            // Debug.Log("Jump");
            Jump();
        }
        velocity = velocity + (xMov + zMov).normalized * speed * Time.deltaTime;

        if(isGrounded && (xMov + zMov).normalized == Vector2.zero)
        {
            velocity = Vector2.Lerp(velocity, Vector2.zero, 0.55f);
        }
        else
        {
            velocity = Vector2.Lerp(velocity, Vector2.ClampMagnitude(velocity, maxSpeed), 0.55f);
        }

        //stairs
        bool isFirstStepCheck = false;
        bool canMove = true;

        //for (int i = stepDetail; i >= 1; i--)
        //{
        //    Collider[] c = Physics.OverlapBox(transform.position + new Vector3(0, i * maxStepHeight / stepDetail - transform.localScale.y, 0), new Vector3(1.05f, maxStepHeight / stepDetail / 2, 1.05f), Quaternion.identity, stepMask);

        //    if (velocity != Vector2.zero)
        //    {
        //        if (c.Length > 0 && i == stepDetail)
        //        {
        //            isFirstStepCheck = true;
        //            if (!isGrounded)
        //            {
        //                canMove = false;
        //            }
        //        }

        //        if (c.Length > 0 && !isFirstStepCheck)
        //        {
        //            transform.position += new Vector3(0, i * maxStepHeight / stepDetail, 0);
        //            break;
        //        }
        //    }
        //}

        //gunHolder.transform.position += new Vector3(0, -rb.velocity.y, 0);
        if (canMove)
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.y);
    }

    public void changeSensitivity(float _sensitivity)
    {
        sensitivity = _sensitivity;
    }

    void Jump()
    {
        rb.AddForce(new Vector3(0, jumpForce, 0));
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(jumpRay.transform.position, new Vector3(.2f, .1f, .2f));

        for (int i = stepDetail; i >= 1; i--)
        {
            Gizmos.DrawWireCube(transform.position + new Vector3(0, i * maxStepHeight / stepDetail - transform.localScale.y, 0), new Vector3(1.05f, maxStepHeight / stepDetail, 1.05f));
        }
    }
}
