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
    public float SneakVelocity = 1.295f;
    public float MaxFallVelocity = 78.4f;
    public float JumpForce = 10f;
    public float GravityForce = 13.888f;

    [Header("Look")]
    public float MouseSensativity = 1f;
    public Transform cam;
    public int Range = 7;

    private World world;
    private Collider col;

    private Vector3 velocity;
    private float yVelocity;
    private float XRotation = 0f;
    private float YRotation = 0f;
    private bool isGrounded;
    private bool jumpRequest;

    private Vector3Int blockInFocus = new Vector3Int(-1, -1, -1);

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        world = World.Instance;
        col = GetComponent<Collider>();
    }

    private void Update()
    {
        if (Input.GetAxisRaw("Jump") != 0 && isGrounded)
            jumpRequest = true;

        HandleLook();
        HandelInput();
        HandleColliders();
        transform.Translate(velocity);
        if(Input.GetMouseButtonDown(0)) 
            SetBlockFocus();
    }

    private void SetBlockFocus()
    {
        blockInFocus = new Vector3Int(-1,-1,-1);

        Vector3 dir = cam.forward;
        List<float> ts = new List<float>();

        float x = 1 + cam.position.x % 1;
        float y = 1 + cam.position.y % 1;
        float z = 1 + cam.position.z % 1;
        for (int i = 0; i < Range; i++, x++, y++, z++)
        {
            ts.Add(x / dir.x);
            ts.Add(y / dir.y);
            ts.Add(z / dir.z);
        }

        ts.Sort();

        int t = 0;
        while (blockInFocus.x < 0 && t < ts.Count)
        {
            Vector3Int block = (dir * t + cam.position).Floor();
            if (world.GetBlockTypeAt(block) != 0)
                blockInFocus = block;
            ++t;
        }

        Debug.Log(world.GetBlockTypeAt(blockInFocus) + " at " + blockInFocus + " length: " + (cam.position - blockInFocus).magnitude);
        if ((cam.position - blockInFocus).magnitude > Range)
            Debug.LogError("Range does not work");
    }

    private void HandleLook()
    {
        XRotation += Input.GetAxisRaw("Mouse X") * MouseSensativity;
        transform.rotation = Quaternion.Euler(0, XRotation, 0);

        YRotation += Input.GetAxisRaw("Mouse Y") * MouseSensativity;
        YRotation = Mathf.Clamp(YRotation, -90, 90);
        cam.localRotation = Quaternion.Euler(-YRotation, 0, 0);
    }

    private void HandleColliders()
    {
        Vector3 v = velocity.RotateAroundY(transform.rotation.eulerAngles.y);
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

        if (velocity.x > 0)
        {
            if (col.Right(v))
                velocity.x = 0;
        }
        else if (velocity.x < 0)
        {
            if (col.Left(v))
                velocity.x = 0;
        }

        isGrounded = col.Down(v.y);
        if (velocity.y < 0)
            velocity.y = col.Down(v.y) ? 0 : velocity.y;
        else if (velocity.y > 0)
            velocity.y = col.Up(v.y) ? 0 : velocity.y;
    }

    private void HandelInput()
    {
        Vector3 input = new Vector3();

        if(Input.GetAxisRaw("Sprint") > 0)
        {
            input.x = Input.GetAxisRaw("Horizontal") * SprintVelocity;
            input.z = Input.GetAxisRaw("Vertical") * SprintVelocity;
        }
        else
        {
            input.x = Input.GetAxisRaw("Horizontal") * MoveVelocity;
            input.z = Input.GetAxisRaw("Vertical") * MoveVelocity;
        }

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
        velocity *= Time.deltaTime;
    }

}
