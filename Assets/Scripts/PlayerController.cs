using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MyCollider))]
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
    public Vector3Int HighlightBlockPos = new Vector3Int(-1, -1, -1);
    public Vector3Int PlacementBlockPos = new Vector3Int(-1, -1, -1);
    public Vector3 LookAtPoint;
    public Transform HighlightBlock;
    public Transform PlacementBlock;

    private World world;
    private MyCollider col;
    [SerializeField]
    private Inventory inventory;

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
        col = GetComponent<MyCollider>();
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
        SetHighlightBlockPos();
        CalculatePlacementPos();
    }

    private float DiffToNaturalNumber(float x)
    {
        float a = 1 - x;
        return Mathf.Min(a, x);
    }

    private void HandleLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        XRotation += Input.GetAxisRaw("Mouse X") * MouseSensativity;
        transform.rotation = Quaternion.Euler(0, XRotation, 0);

        YRotation += Input.GetAxisRaw("Mouse Y") * MouseSensativity;
        YRotation = Mathf.Clamp(YRotation, -90, 90);
        cam.localRotation = Quaternion.Euler(-YRotation, 0, 0);
    }

    private void HandelInput()
    {
        InputMovement();
        InputMouseButtons();
    }

    private void InputMouseButtons()
    {
        if (inventory.InventoryUI.activeSelf)
            return;

        if (Input.GetMouseButtonDown(0))
            world.EditBlock(HighlightBlockPos, 0);
        else if (Input.GetMouseButtonDown(1))
            world.EditBlock(PlacementBlockPos, inventory.GetSlectedItem().Type);
    }

    private void SetHighlightBlockPos()
    {
        HighlightBlockPos = new Vector3Int(-1,-1,-1);

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
        while (HighlightBlockPos.x < 0 && t < ts.Count && ts[t] < Range)
        {
            LookAtPoint = (dir * ts[t] + cam.position);
            Vector3Int block = LookAtPoint.Floor();
            if (world.GetBlockTypeAt(block) != 0)
            {
                HighlightBlockPos = block;
            }
            ++t;
        }

        HighlightBlock.position = HighlightBlockPos + new Vector3(0.5f, 0.5f, 0.5f);
    }

    private void CalculatePlacementPos()
    {
        PlacementBlockPos = new Vector3Int(-1, -1, -1);

        float x = LookAtPoint.x % 1;
        float y = LookAtPoint.y % 1;
        float z = LookAtPoint.z % 1;

        float dx = DiffToNaturalNumber(x);
        float dy = DiffToNaturalNumber(y);
        float dz = DiffToNaturalNumber(z);

        float min = Mathf.Min(new float[] { dx, dy, dz });
        if (min == dx)
        {
            if (x > 0.5f)
                PlacementBlockPos = new Vector3Int(HighlightBlockPos.x + 1, HighlightBlockPos.y, HighlightBlockPos.z);
            else
                PlacementBlockPos = new Vector3Int(HighlightBlockPos.x - 1, HighlightBlockPos.y, HighlightBlockPos.z);
        }
        else if (min == dy)
        {
            if (y > 0.5f)
                PlacementBlockPos = new Vector3Int(HighlightBlockPos.x, HighlightBlockPos.y + 1, HighlightBlockPos.z);
            else
                PlacementBlockPos = new Vector3Int(HighlightBlockPos.x, HighlightBlockPos.y - 1, HighlightBlockPos.z);
        }
        else
        {
                if (z > 0.5f)
                PlacementBlockPos = new Vector3Int(HighlightBlockPos.x, HighlightBlockPos.y, HighlightBlockPos.z + 1);
            else
                PlacementBlockPos = new Vector3Int(HighlightBlockPos.x, HighlightBlockPos.y, HighlightBlockPos.z - 1);
        }

        PlacementBlock.position = PlacementBlockPos + new Vector3(0.5f, 0.5f, 0.5f);
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

    private void InputMovement()
    {
        Vector3 input = new Vector3();

        if (Input.GetAxisRaw("Sprint") > 0)
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
