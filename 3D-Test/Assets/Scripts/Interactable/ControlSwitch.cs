using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class ControlSwitch : MonoBehaviour
{
    [Header("All Triggerables must be ON for this switch to work")]
    [SerializeField] private Triggerable[] controls;

    [Header("Whether the On/Off is inverted")]
    [SerializeField] private bool invert = false;

    [Header("Boolean Variable from the Animator to Trigger")]
    [Tooltip("Empty to be not triggering")]
    [SerializeField] private string animState;

    // Runtime Vars
    private Animator animator;
    private bool activated;
    private bool wasActivated;
    private bool[] _prevControlState;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip doorOpen;
    public AudioClip Switch;
    [SerializeField] private float edgeSoundCooldown = 0f; 
    private float _lastEdgeTime;

    private readonly Queue<AudioClip> clipQueue = new Queue<AudioClip>();
    private readonly Queue<float> volumeQueue = new Queue<float>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();

        if (controls != null && controls.Length > 0)
        {
            _prevControlState = new bool[controls.Length];
            for (int i = 0; i < controls.Length; i++)
                _prevControlState[i] = controls[i] != null && controls[i].Activated();
        }
        else
        {
            _prevControlState = Array.Empty<bool>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        activated = (invert) ? !ActivationStatus() : ActivationStatus();

        if (animState.Length > 0)
        {
            animator.SetBool(animState, activated);
        }

        if (audioSource != null && !audioSource.isPlaying && clipQueue.Count > 0)
        {
            var clip = clipQueue.Dequeue();
            var vol = volumeQueue.Dequeue();
            audioSource.clip = clip;
            audioSource.volume = vol;
            audioSource.Play();
        }
        bool buttonActivated = false;
        for (int i = 0; i < controls.Length; i++)
        {
            var c = controls[i];
            if (c == null) continue;

            bool now = c.Activated();
            if (now && !_prevControlState[i])
            {
                buttonActivated = true;
                EnqueueSound(Switch, 0.7f);
            }

            _prevControlState[i] = now;
        }

        if (activated && !wasActivated) EnqueueSound(doorOpen, 0.7f);
        if (!activated && wasActivated) EnqueueSound(Switch, 0.7f);

        // remember for next frame
        wasActivated = activated;

    }

    // Return True when no control exist
    // Return True when all controls Activated()
    private bool ActivationStatus()
    {
        if (controls.Length == 0) { return true; }

        foreach (Triggerable control in controls)
        {
            if (!control.Activated()) return false;
        }

        return true;
    }
    private void EnqueueSound(AudioClip clip, float volume)
    {
        if (clip == null || audioSource == null) return;
        clipQueue.Enqueue(clip);
        volumeQueue.Enqueue(volume);
    }
}
