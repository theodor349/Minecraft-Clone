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
    public Vector3Int BlockInFocus = new Vector3Int(-1, -1, -1);
    public GameObject PlacementBlock;

    private World world;
    private Collider col;

    private Vector3 velocity;
    private float yVelocity;
    private float XRotation = 0f;
    private float YRotation = 0f;
    private bool isGrounded;
    private bool jumpRequest;


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
    }

    private void FixedUpdate()
    {
        SetBlockFocus();
        PlacementBlock.transform.position = BlockInFocus + new Vector3(0.5f, 0.5f, 0.5f);
    }

    private void SetBlockFocus()
    {
        BlockInFocus = new Vector3Int(-1,-1,-1);

        Vector3 dir = cam.forward;
        List<float> ts = new List<float>();

        float x = cam.position.x % 1;
        float y = cam.position.y % 1;
        float z = cam.position.z % 1;

        if(dir.x != 0)
            for (float r = 1; r < Range; r++)
            {
                if (dir.x > 0)
                    ts.Add((r - x) / dir.x + 0.01f);
                else
                    ts.Add((-r - x + 1) / dir.x + 0.01f);
            }
        if(dir.y != 0)
            for (float r = 1; r < Range; r++)
            {
                if (dir.y > 0)
                    ts.Add((r - y) / dir.y + 0.01f);
                else
                    ts.Add((-r - y + 1) / dir.y + 0.01f);
            }
        if(dir.z != 0)
            for (float r = 1; r < Range; r++)
            {
                if (dir.z > 0)
                    ts.Add((r - z) / dir.z + 0.01f);
                else
                    ts.Add((-r - z + 1) / dir.z + 0.01f);
            }

        ts.Sort();

        int t = 0;
        while (BlockInFocus.x < 0 && t < ts.Count && ts[t] < Range)
        {
            Vector3Int block = (dir * ts[t] + cam.position).Floor();
            if (world.GetBlockTypeAt(block) != 0)
                BlockInFocus = block;
            ++t;
        }
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
