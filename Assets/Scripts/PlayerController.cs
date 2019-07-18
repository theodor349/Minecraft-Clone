using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float MoveVelocity = 4.317f;
    public float SprintVelocity = 5.612f;
    public float MaxFallVelocity = 78.4f;
    public float JumpForce = 10f;
    public float GravityForce = 13.888f;

    [Header("Look")]
    public float MouseSensativity = 1f;
    public Transform cam;
    public float XRotation = 0f;
    public float YRotation = 0f;

    private World world;
    private Collider col;

    private Vector3 velocity;
    private float yVelocity;
    private bool isGrounded;
    private bool jumpRequest;

    private void Start()
    {
        world = World.Instance;
        col = GetComponent<Collider>();
    }

    private void Update()
    {
        if (Input.GetAxisRaw("Jump") != 0 && isGrounded)
            jumpRequest = true;

        HandleLook();
    }

    private void HandleLook()
    {
        XRotation += Input.GetAxisRaw("Mouse X") * MouseSensativity;
        transform.rotation = Quaternion.Euler(0, XRotation, 0);

        YRotation += Input.GetAxisRaw("Mouse Y") * MouseSensativity;
        YRotation = Mathf.Clamp(YRotation, -90, 90);
        cam.localRotation = Quaternion.Euler(-YRotation, 0, 0);
    }

    private void FixedUpdate()
    {
        HandelInput();
        HandleColliders();

        transform.Translate(velocity);
    }

    private void HandleColliders()
    {
        Vector3 v = velocity.RotateAroundY(transform.rotation.eulerAngles.y);
        Debug.Log(velocity);
        if(velocity.z > 0)
        {
            if (col.Forward(v))
                velocity.z = 0;
        }
        else if (velocity.z < 0)
        {
            if (col.Backward(v))
                velocity.z = 0;
        }
        Debug.Log(velocity);

        isGrounded = col.Down(v.y);
        if (velocity.y < 0)
            velocity.y = col.Down(v.y) ? 0 : velocity.y;
        else if (velocity.y > 0)
            velocity.y = col.Up(v.y) ? 0 : velocity.y;
    }

    private void HandelInput()
    {
        Vector3 input = new Vector3();

        input.x = Input.GetAxisRaw("Horizontal") * MoveVelocity;
        input.z = Input.GetAxisRaw("Vertical") * MoveVelocity;

        if (jumpRequest)
        {
            jumpRequest = false;
            yVelocity = JumpForce;
        }

        if (!isGrounded)
        {
            yVelocity -= GravityForce * Time.deltaTime;
            if (yVelocity < -MaxFallVelocity)
                yVelocity = -GravityForce;
        }

        input.y = yVelocity;

        velocity = input;
        velocity *= Time.fixedDeltaTime;
    }

}
