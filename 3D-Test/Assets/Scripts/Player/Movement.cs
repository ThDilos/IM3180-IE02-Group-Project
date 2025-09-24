using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

[RequireComponent(typeof(SwitchCharacter))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
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

    [Tooltip("Smooth Flip is Currently Jiorking cuz the Sprite isn't Perfectly Vertical :( Disable to see the effect")]
    [SerializeField] bool directFlip = true;
    private bool grounded;
    private int facing = 1;

    [Header("Respawn Mechanics 1")]
    [Tooltip("Coordinates of the Spawn Point")]
    [SerializeField] private Vector3 spawnPoint;

    [Header("Respawn Mechanics 2")]
    [Tooltip("Teleport to the previous safe position")]
    [SerializeField] private bool rewindPosition = false;
    [Tooltip("How many times the safe positions are updated per second?")]
    [SerializeField] private int updateFrequency = 5;
    [Tooltip("How fast (second) the player dies again for this position to be abandoned, and routed to the spawn Point?")]
    [SerializeField] private float failSafeTimer = 1f; // Abolish rewinding and use spawnPoint if dies too quickly

    [Header("Animations - Input the String name of the Animation States Bool")]
    [Tooltip("Trigger")]
    [SerializeField] private string jumpAnim = "jump";
    [SerializeField] private string runAnim = "running";
    [SerializeField] private string fallAnim = "falling";
    [SerializeField] private string walkAnim = "walking";

    // Runtime Vars
    private Rigidbody rb;
    private BoxCollider bc;
    private SwitchCharacter sc;
    private Animator animator;

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

    private float respawnSafeTime = 1.0f; // Only respawnable again after 1s of respawning
    private float timeSinceRespawn = 0.0f;

    private Vector3 safePosition;
    private float positionUpdateTimer = 0f;
    private float softlockTimer = 0f;

    private Transform[] characterSprites;

    private Vector2 originalRotation;

    private DialogPopUp dpu;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SwitchCharacter>();
        dpu = GetComponent<DialogPopUp>();

        animator = GetComponent<Animator>();
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
        if (characterSprites == null)
        {
            if (sc.GetAllSpriteTransforms() != null)
            {
                characterSprites = sc.GetAllSpriteTransforms();
                originalRotation = characterSprites[0].rotation.eulerAngles;
            }
        }
        else
        {
            FlipSpriteSmooth(moveInput.x);
        }

        if (sc.GetActiveBoxCollider() != null)
        {
            bc = sc.GetActiveBoxCollider();
        }
        else return;

        moveInput = new Vector2(lrAction.ReadValue<float>(), fbAction.ReadValue<float>());
        if (jump.WasPerformedThisFrame()) Jump(jumpForce);

        if (sc.activatedCharacter == SwitchCharacter.ActivatedCharacter.GOOSE)
        {
            ClassMechanicsGoose();
        }

        if ( 1 / updateFrequency > positionUpdateTimer)
        {
            positionUpdateTimer += Time.deltaTime;
        }
        else
        {
            positionUpdateTimer = 0;
            if (grounded && sc.activatedCharacter != SwitchCharacter.ActivatedCharacter.GOOSE)
                safePosition = transform.position;
        }

        if (softlockTimer > 0) softlockTimer -= Time.deltaTime;
        if (timeSinceRespawn > 0) timeSinceRespawn -= Time.deltaTime;

        //HandleAnimation();
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

            //animator.SetTrigger(jumpAnim);
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
            return false;
        }
        Bounds b = bc.bounds;
        Vector3 size = new Vector3(b.size.x * 0.5f, groundCheckDepth);
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
                gameObject.layer = 6;
                break;
            case SwitchCharacter.ActivatedCharacter.GOOSE:
                walkSpeed = gooseWalkSpeed;
                runSpeed = gooseRunSpeed;
                jumpForce = gooseJumpForce;
                gameObject.layer = 0;
                break;
            case SwitchCharacter.ActivatedCharacter.CAT:
                walkSpeed = catWalkSpeed;
                runSpeed = catRunSpeed;
                jumpForce = catJumpForce;
                gameObject.layer = 6;
                break;
        }
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

        if (directFlip)
        {
            foreach (Transform characterSprite in characterSprites)
            {
                SpriteRenderer renderer = characterSprite.GetComponent<SpriteRenderer>();
                renderer.flipX = facing == 0;
            }
            return;
        }

        var e = originalRotation;
        float targetX = (facing == 1) ? originalRotation.x : originalRotation.x + 90;
        float targetY = (facing == 1) ? originalRotation.y : originalRotation.y + 180;

        e.y = Mathf.SmoothDampAngle(e.y, targetY, ref flipVel, 0.005f);
        e.x = Mathf.SmoothDampAngle(e.x, targetX, ref flipVel, 0.005f);

        foreach (Transform characterSprite in characterSprites)
        {
            characterSprite.eulerAngles = e;
        }
    }

    public void Respawn()
    {
        if (timeSinceRespawn > 0) return;

        dpu.PopUpDialog(DialogPopUp.CommonDialog.WaterWarning);
        timeSinceRespawn = respawnSafeTime;
        Vector3 targetPos = spawnPoint;
        if (!rewindPosition || softlockTimer > 0)
        {
            if (spawnPoint != null)
            {
                targetPos = spawnPoint;
            }
        }
        else if (rewindPosition)
        {
            softlockTimer = failSafeTimer;
            targetPos = safePosition;
        }

        Debug.Log("Player Respawned to " + targetPos);
        transform.position = targetPos;
    }

    private void HandleAnimation()
    {
        animator.SetBool(fallAnim, !grounded && rb.linearVelocity.y < 0f); // Falling Animation
        if (run.IsPressed() && moveInput.magnitude > 0)
        {
            animator.SetBool(runAnim, true);
        }
        else if (moveInput.magnitude > 0) 
        {
            animator.SetBool(walkAnim, true);
        }
        animator.SetBool(runAnim, run.IsPressed() && moveInput.magnitude > 0);
        animator.SetBool(runAnim, !run.IsPressed() && moveInput.magnitude > 0);
    }


    private void OnDrawGizmosSelected()
    {
        if (!bc)
        {
            return;
        }

        Bounds b = bc.bounds;
        Vector3 size = new Vector3(b.size.x * 0.75f, groundCheckDepth, b.size.z);
        Vector3 center = new Vector3(b.center.x, b.min.y - size.y * 0.5f, b.center.z);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);
    }
}
