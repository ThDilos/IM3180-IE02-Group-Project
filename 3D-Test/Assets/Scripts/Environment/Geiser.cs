using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Geiser : MonoBehaviour
{
    [Header("Geiser Space Setting")]
    [SerializeField] private float waterFlowHeight = 10.0f;
    [SerializeField] private Vector3 activationZoneCenterOffset = new Vector3(0, 1, 0);
    [SerializeField] private Vector3 activationZoneSize = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 geiserZoneCenterOffset = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 geiserZoneSize = new Vector3(2, 4, 2);
    [SerializeField] private Vector3 geiserDirection = Vector3.up;
    [SerializeField] private LayerMask detectionLayer = 1;

    [Header("Geiser Stats")]
    [SerializeField] private GeiserMode geiserMode;
    [SerializeField] private float pushForce = 10.0f;
    [SerializeField] private ForceMode forceMode = ForceMode.Force;
    [Tooltip("Time between being activated to erupt (In seconds)")]
    [SerializeField] private float startDelay = 1.5f;
    [Tooltip("How long does it stays erupting? (In seconds)")]
    [SerializeField] private float duration = 2.0f;
    [Tooltip("Time until next eruption is possible, after one has ended.")]
    [SerializeField] private float cooldown = 3.0f;

    [Header("SFX")]
    [SerializeField] private Animator animator;
    [SerializeField] private string startingAnim = string.Empty;
    [SerializeField] private string eruptingAnim = string.Empty;
    [SerializeField] private string coolingdownAnim = string.Empty;

    [SerializeField] private AudioClip idleAmbient;
    [SerializeField] private AudioClip activatingNoise;
    [SerializeField] private AudioClip eruptBurseNoise;
    [SerializeField] private AudioClip eruptingNoise;
    [SerializeField] private AudioClip coolingdownNoise;

    // Runtime Vars
    [HideInInspector] public State currentState = State.IDLE;
    private float timer = 0.0f;
    private Vector3 activationZoneCenter;
    private Vector3 geiserZoneCenter;
    private AudioSource audioSource;
    private bool eruptionBurstPlayed = false;

    public enum State
    {
        IDLE,
        ACTIVATING,
        ERUPTING,
        COOLINGDOWN,
        DEACTIVATED
    }

    private enum GeiserMode
    {
        ALWAYSACTIVATAED,
        DETECTION
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update for handling SFXes
    private void Update()
    {
        HandleSFX(currentState);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (geiserMode == GeiserMode.ALWAYSACTIVATAED)
        {
            currentState = State.ERUPTING;
            timer = duration;
        }

        // To be deleted once you think the zone settings are finalized
        activationZoneCenter = transform.position + activationZoneCenterOffset;
        geiserZoneCenter = transform.position + geiserZoneCenterOffset;
        //

        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
        }

        switch (currentState)
        {
            case State.IDLE:
                if (CheckForColliders(activationZoneCenter, activationZoneSize).Length > 0)
                {
                    timer = startDelay;
                    currentState = State.ACTIVATING;
                }
                break;
            case State.ACTIVATING:
                if (timer <= 0.0f)
                {
                    timer = duration;
                    currentState = State.ERUPTING;
                }
                break;
            case State.ERUPTING:
                if (timer <= 0.0f)
                {
                    timer = cooldown;
                    currentState = State.COOLINGDOWN;
                }
                else
                {
                    Collider[] targets = CheckForColliders(geiserZoneCenter, geiserZoneSize);
                    foreach (Collider target in targets)
                    {
                        Rigidbody rb;
                        target.TryGetComponent<Rigidbody>(out rb);
                        if (rb == null)
                        {
                            rb = target.GetComponentInParent<Rigidbody>();
                        }

                        rb.AddForce(geiserDirection * pushForce, forceMode);
                    }
                }
                break;
            case State.COOLINGDOWN:
                if (timer <= 0.0f)
                {
                    currentState = State.IDLE;
                }
                break;
        }
    }

    // Used for area check for Activation and Eruption, only pass if the object has RigidBody component
    private Collider[] CheckForColliders(Vector3 center, Vector3 size)
    {
        Collider[] colliders = Physics.OverlapBox(center, size / 2, Quaternion.identity, detectionLayer);
        List<Collider> qualifiedList = new List<Collider>();
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<Rigidbody>() != null || collider.GetComponentInParent<Rigidbody>() != null)
                qualifiedList.Add(collider);
        }

        colliders = qualifiedList.ToArray();

        return colliders;
    }

    // Different SFX to be played in the different stages
    private void HandleSFX(State state)
    {
        if (animator != null)
        {
            animator.SetBool(startingAnim, false);
            animator.SetBool(eruptingAnim, false);
            animator.SetBool(coolingdownAnim, false);
        }
        if (state != State.ERUPTING)
        {
            eruptionBurstPlayed = false;
        }

        switch (state)
        {
            case State.IDLE:
                if (idleAmbient == null) break;
                audioSource.clip = idleAmbient;
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                break;
            case State.ACTIVATING:
                if (activatingNoise != null)
                {
                    audioSource.clip = activatingNoise;
                    if (!audioSource.isPlaying)
                    {
                        audioSource.Play();
                    }
                }
                if (animator != null)
                {
                    animator.SetBool(startingAnim, true);
                }
                break;
            case State.ERUPTING:
                // Play the burst sound right after the burst happens
                if (eruptBurseNoise != null && !eruptionBurstPlayed)
                {
                    eruptionBurstPlayed = true;
                    audioSource.PlayOneShot(eruptBurseNoise);
                }

                if (eruptingNoise != null)
                {
                    audioSource.clip = eruptingNoise;
                    if (!audioSource.isPlaying)
                    {
                        audioSource.Play();
                    }
                }
                if (animator != null)
                {
                    animator.SetBool(eruptingAnim, true);
                }
                break;
            case State.COOLINGDOWN:
                if (coolingdownNoise != null)
                {
                    audioSource.clip = coolingdownNoise;
                    if (!audioSource.isPlaying)
                    {
                        audioSource.Play();
                    }
                }
                if (animator != null)
                {
                    animator.SetBool(coolingdownAnim, true);
                }
                break;                
        }
    }

    // Debug Boxes
    private void OnDrawGizmos()
    {
        switch (currentState)
        {
            case State.DEACTIVATED:
            case State.COOLINGDOWN:
                return;
            case State.IDLE:
                if (activationZoneCenter != null)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireCube(transform.position + activationZoneCenterOffset, activationZoneSize);
                }
                break;
            default:
                if (geiserZoneCenter != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireCube(transform.position + geiserZoneCenterOffset, geiserZoneSize);
                }
                break;
        }
    }
}
