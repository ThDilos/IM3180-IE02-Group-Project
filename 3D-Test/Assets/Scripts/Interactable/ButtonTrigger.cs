using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonTrigger : MonoBehaviour, IInteractable
{
    [Header("Which objects to be triggered by this interactible?")]
    [SerializeField] private ControlSwitch[] triggerObjects; // Door that it's gonna open
    [Header("How long would this interactible remain triggered?")]
    [SerializeField] private float buttonDuration;
    [Header("Animator Variable for Trigger / Boolean")]
    [SerializeField] string animVar = "";

    [Header("Can be toggled, animator changes Boolean instead of Trigger")]
    [SerializeField] private bool isClanker = false;

    [Header("Change Color")]
    [SerializeField] private Color triggeredColor = Color.red;

    // Runtime var
    Animator animator;
    private bool activated = false;
    private Renderer render;
    private Color idleColor;
    private Coroutine buttonCoroutine;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        render = GetComponent<Renderer>();
        idleColor = render.material.color;
    }
    private void Update()
    {
        if (isClanker && activated)
        {
            // Infinitely Triggering when activated
            foreach (ControlSwitch cs in triggerObjects)
            {
                cs.Override(0.2f);
            }
        }

        if (activated)
        {
            render.material.color = triggeredColor;
        }
        else
        {
            render.material.color = idleColor;
        }
    }

    public Transform getTransform()
    {
        return transform;
    }

    public void Interact()
    {
        if (buttonCoroutine != null) StopCoroutine(buttonCoroutine);
        if (isClanker)
        {
            activated = !activated;
            if (animVar.Length > 0) animator.SetBool(animVar, activated);
        }
        else
        {
            foreach (ControlSwitch cs in triggerObjects)
            {
                cs.Override(buttonDuration);
            }
            activated = true;
            if (animVar.Length > 0) animator.SetTrigger(animVar);
            buttonCoroutine = StartCoroutine(ReleaseButton(buttonDuration));
        }
    }

    IEnumerator ReleaseButton(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool(animVar, false);
        activated = false;
    }

    public bool Condition()
    {
        return true;
    }

    public string AlternativeText()
    {
        return null;
    }
}
