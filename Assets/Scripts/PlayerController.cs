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
        transform.Rotate(Vector3.up, Input.GetAxisRaw("Mouse X") * MouseSensativity, Space.World);
        cam.Rotate(Vector3.left, Input.GetAxisRaw("Mouse Y") * MouseSensativity, Space.Self);
    }

    private void FixedUpdate()
    {
        HandelInput();
        HandleColliders();

        transform.Translate(velocity);
    }

    private void HandleColliders()
    {
        Vector3 distance = Vector3.Scale(velocity, transform.forward);

        velocity.x = col.CheckX(distance.x) ? 0 : velocity.x;
        velocity.z = col.CheckZ(distance.z) ? 0 : velocity.z;
        isGrounded = col.CheckY(velocity.y);
        velocity.y = col.CheckY(velocity.y) ? 0 : velocity.y;
            
    }

    private void HandelInput()
    {
        velocity.x = Input.GetAxisRaw("Horizontal") * MoveVelocity;
        velocity.z = Input.GetAxisRaw("Vertical") * MoveVelocity;

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

        velocity.y = yVelocity;

        velocity *= Time.fixedDeltaTime;
    }

}
