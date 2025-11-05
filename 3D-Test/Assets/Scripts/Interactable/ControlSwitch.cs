using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip lever;
    [SerializeField] private AudioClip pressurePlateon;
    [SerializeField] private AudioClip pressurePlateoff;
    [SerializeField] private AudioClip doorOpen;

    // Runtime Vars
    private Animator animator;
    private bool activated;
    private bool wasActivated;
    private bool[] _prevControlState;

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
        if (audioSource != null) audioSource.spatialBlend = 0f;
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

        bool anyButtonActivated = false;
        for (int i = 0; i < controls.Length; i++)
        {
            var c = controls[i];
            if (c == null) continue;

            bool now = c.Activated();
            if (now && !_prevControlState[i])
            {
                anyButtonActivated = true;
            }

            _prevControlState[i] = now;
        }

        if (activated && !wasActivated)
        {
            if (lever != null)
                EnqueueSound(lever, 1f);
            else if (pressurePlateon != null)
                EnqueueSound(pressurePlateon, 1f);
            EnqueueSound(doorOpen, 0.5f);
        }
        //if (activated && wasActivated)
        //{
        //    if (lever != null)
        //        EnqueueSound(lever, 1f);
        //    else if (pressurePlateoff != null)
        //        EnqueueSound(pressurePlateoff, 1f);
        //    EnqueueSound(doorOpen, 0.5f);
        //}

        wasActivated = activated;

    }
    private void EnqueueSound(AudioClip clip, float volume)
    {
        if (clip == null || audioSource == null) return;
        clipQueue.Enqueue(clip);
        volumeQueue.Enqueue(volume);
    }

    // Return True when no control exist
    // Return True when all controls Activated()
    private bool ActivationStatus()
    {
        if (controls == null || controls.Length == 0) { return true; }

        foreach (Triggerable control in controls)
        {
            if (controls == null || !control.Activated()) return false;
        }

        return true;
    }
}
