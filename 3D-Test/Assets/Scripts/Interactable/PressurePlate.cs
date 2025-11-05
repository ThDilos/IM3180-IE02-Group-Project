using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(BoxCollider))]
public class PressurePlate : Triggerable
{
    [Header("The Mass Required to Trigger the Pressure Plate")]
    [SerializeField] float triggeringMass = 1.0f;

    [Header("Change Color")]
    [SerializeField] private Color triggeredColor = Color.red;

    //[Header("Audio")]
    //[SerializeField] private AudioSource audioSource;
    //[SerializeField] private AudioClip pressurePlateOn;
    //[SerializeField] private AudioClip pressurePlateOff;
    //[SerializeField] private AudioClip doorOpen;
    //[SerializeField] private float doorOpenVol = 0.5f;

    private Animator animator;
    private BoxCollider bc;
    private Renderer render;

    private Color idleColor;

    private bool activated = false; // Triggerable Objects Read this

    //// --- AUDIO QUEUE (audio-only addition) ---
    //private readonly Queue<(AudioClip clip, float vol)> _q = new();
    //private bool _queueRunning;
    //private bool wasActivated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bc = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
        render = GetComponent<Renderer>();
        idleColor = render.material.color;
    }

    private void Update()
    {
        animator.SetBool("activated", activated);
        if (activated)
        {
            render.material.color = triggeredColor;
        }
        else
        {
            render.material.color = idleColor;
        }

        //// --- AUDIO: enqueue on edges ---
        //if (activated && !wasActivated)
        //{
        //    Debug.Log("PRESS edge");
        //    EnqueueSound(pressurePlateOn, 3f);           // volume clamped in EnqueueSound
        //    EnqueueSound(doorOpen, doorOpenVol);
        //}

        //if (!activated && wasActivated)
        //{
        //    Debug.Log("RELEASE edge");
        //    EnqueueSound(pressurePlateOff, 3f);          // volume clamped in EnqueueSound
        //    EnqueueSound(doorOpen, doorOpenVol);
        //}

        //if (Input.GetKeyDown(KeyCode.T) && audioSource && pressurePlateOn)
        //{
        //    Debug.Log("Test: playing pressurePlateOn via T key");
        //    audioSource.PlayOneShot(pressurePlateOn, 1f);
        //}

        //wasActivated = activated;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Rigidbody>() != null)
        {
            activated = triggeringMass < other.GetComponent<Rigidbody>().mass;
        }
        if (other.GetComponentInParent<Rigidbody>() != null)
        {
            activated = triggeringMass < other.GetComponentInParent<Rigidbody>().mass;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        activated = false;
    }

    public override bool Activated()
    {
        return activated;
    }

    //// --- AUDIO HELPERS (audio-only) ---
    //private void EnqueueSound(AudioClip c, float v)
    //{
    //    if (!c || !audioSource) return;
    //    _q.Enqueue((c, Mathf.Clamp01(v)));   // clamp volume into [0,1]
    //    if (!_queueRunning) StartCoroutine(RunQueue());
    //}

    //private System.Collections.IEnumerator RunQueue()
    //{
    //    _queueRunning = true;

    //    while (_q.Count > 0)
    //    {
    //        var (clip, vol) = _q.Dequeue();

    //        // fire clip
    //        audioSource.pitch = 1f; // keep timing predictable; remove if you vary pitch elsewhere
    //        audioSource.PlayOneShot(clip, vol);

    //        // wait for it to finish (pitch-aware)
    //        float dur = Mathf.Max(0.01f, clip.length / Mathf.Max(0.01f, audioSource.pitch));
    //        yield return new WaitForSeconds(dur);
    //    }

    //    _queueRunning = false;
    //}
}
