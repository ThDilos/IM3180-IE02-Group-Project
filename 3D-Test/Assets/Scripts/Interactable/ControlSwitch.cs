using System;
using System.Linq;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        activated = (invert) ? !ActivationStatus() : ActivationStatus();

        if (animState.Length > 0)
        {
            animator.SetBool(animState, activated);
        }
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
}
