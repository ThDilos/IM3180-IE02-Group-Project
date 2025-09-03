using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

[RequireComponent(typeof(SwitchCharacter))]

public class Movement : MonoBehaviour
{
    [Header("Character-based Stats")]
    [Header("Bear")]
    [SerializeField] private float bearWalkSpeed = 1.0f;
    [SerializeField] private float bearRunSpeed = 2.0f;
    [SerializeField] private float bearJumpForce = 5.0f;

    [Header("Goose")]
    [SerializeField] private float gooseWalkSpeed = 1.0f;
    [SerializeField] private float gooseRunSpeed = 2.0f;
    [SerializeField] private float gooseJumpForce = 5.0f;

    [Header("Cat")]
    [SerializeField] private float catWalkSpeed = 1.0f;
    [SerializeField] private float catRunSpeed = 2.0f;
    [SerializeField] private float catJumpForce = 5.0f;

    [Header("Character Special Abilities")]
    [Header("Goose")]
    [Tooltip("The Max Falling Velocity when Gliding")]
    [SerializeField] private float glidingMaxVelY = 2.0f;
    [Header("Bear")]
    [Tooltip("Mass decides what movable Object the Bear can move. Higher Mass = Easily Pushing a Low-Mass Object")]
    [SerializeField] private float bearMass = 2.0f;

    [Header("Technical Adjustments")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundCheckDepth = 0.06f;
    [SerializeField] float flipVel = 50f;
    private bool grounded;
    private int facing = 1;

    // Runtime Vars
    private Rigidbody rb;
    private BoxCollider bc;
    private SwitchCharacter sc;

    private float walkSpeed;
    private float runSpeed;
    private float jumpForce;

    private InputActionAsset inputActions;
    private InputAction lrAction;
    private InputAction fbAction;
    private InputAction run;
    private InputAction jump;

    private Vector2 moveInput;

    private float originalMass;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
        sc = GetComponent<SwitchCharacter>();
        UpdateStats(sc.activatedCharacter);


        inputActions = sc.inputActions;
        InputActionMap map = inputActions.FindActionMap("Move");
        lrAction = map.FindAction("LR");
        fbAction = map.FindAction("FB");
        jump = map.FindAction("Jump");
        run = map.FindAction("Run");

        originalMass = rb.mass;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = new Vector2(lrAction.ReadValue<float>(), fbAction.ReadValue<float>());
        if (jump.WasPerformedThisFrame()) Jump(jumpForce);

        FlipSpriteSmooth(moveInput.x);

        if (sc.activatedCharacter == SwitchCharacter.ActivatedCharacter.GOOSE)
        {
            ClassMechanicsGoose();
        }
        else if (sc.activatedCharacter == SwitchCharacter.ActivatedCharacter.BEAR)
        {
            ClassMechanicsBear();
        }
    }

    private void FixedUpdate()
    {
        grounded = IsGrounded();

        float targetSpeed = run.IsPressed() ? runSpeed : walkSpeed;
        Vector2 velocity = moveInput * targetSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.y);
    }

    private void Jump(float force)
    {
        if (CanJump())
        {
            rb.AddForce(Vector3.up * rb.mass * force, ForceMode.Impulse);
            grounded = false;
        }
    }

    private bool CanJump()
    {
        return IsGrounded();
    }

    bool IsGrounded()
    {
        if (!bc)
        {
            bc = GetComponent<BoxCollider>();
        }
        Bounds b = bc.bounds;
        Vector3 size = new Vector3(b.size.x * 0.75f, groundCheckDepth);
        Vector3 center = new Vector3(b.center.x, b.min.y - size.y * 0.5f, b.center.z);
        return Physics.OverlapBox(center, size, Quaternion.identity, groundMask).Length > 0;
    }

    public void UpdateStats(SwitchCharacter.ActivatedCharacter character)
    {
        if (rb != null)
        {
            rb.mass = originalMass;
        }
        switch (character)
        {
            case SwitchCharacter.ActivatedCharacter.BEAR:
                walkSpeed = bearWalkSpeed;
                runSpeed = bearRunSpeed;
                jumpForce = bearJumpForce;
                rb.mass = bearMass;
                break;
            case SwitchCharacter.ActivatedCharacter.GOOSE:
                walkSpeed = gooseWalkSpeed;
                runSpeed = gooseRunSpeed;
                jumpForce = gooseJumpForce;
                break;
            case SwitchCharacter.ActivatedCharacter.CAT:
                walkSpeed = catWalkSpeed;
                runSpeed = catRunSpeed;
                jumpForce = catJumpForce;
                break;
        }
        Debug.Log(string.Format("Stats updated to {0}", character));
    }

    private void ClassMechanicsGoose()
    {
        if (jump.IsPressed() && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                Mathf.Clamp(rb.linearVelocity.y, -Mathf.Abs(glidingMaxVelY), Mathf.Abs(glidingMaxVelY)),
                rb.linearVelocity.z);
        }
    }

    private void ClassMechanicsBear()
    {
        // Empty. To be Added
    }

    void FlipSpriteSmooth(float xVel) // Replacing the flipX part in Update() with FlipSprite()
    {
        if (xVel > 0)
        {
            facing = 1;
        }
        else if (xVel < 0)
        {
            facing = 0;
        }
        var e = transform.eulerAngles;
        float target = (facing == 1) ? 0f : 180f;
        e.y = Mathf.SmoothDampAngle(e.y, target, ref flipVel, 0.05f);
        transform.eulerAngles = e;
    }

    private void OnDrawGizmosSelected()
    {
        if (!bc)
        {
            bc = GetComponent<BoxCollider>();
        }

        Bounds b = bc.bounds;
        Vector3 size = new Vector3(b.size.x * 0.75f, groundCheckDepth, b.size.z);
        Vector3 center = new Vector3(b.center.x, b.min.y - size.y * 0.5f, b.center.z);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);
    }
}
