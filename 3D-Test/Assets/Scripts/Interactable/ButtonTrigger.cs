using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonTrigger : Triggerable, IInteractable
{
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
        if (GetComponent<Renderer>() != null)
        {
            render = GetComponent<Renderer>();
            idleColor = render.material.color;
        }
    }
    private void Update()
    {
        if (render != null)
        {
            if (activated)
            {
                render.material.color = triggeredColor;
            }
            else
            {
                render.material.color = idleColor;
            }
        }
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    public void Interact()
    {
        if (buttonCoroutine != null) StopCoroutine(buttonCoroutine);

        Debug.Log(gameObject.name + " is Interacted.");
        if (isClanker)
        {
            activated = !activated;
            if (animVar.Length > 0) animator.SetBool(animVar, activated);
        }
        else
        {
            activated = true;
            if (animVar.Length > 0) animator.SetTrigger(animVar);
            buttonCoroutine = StartCoroutine(ReleaseButton(buttonDuration));
        }
    }

    IEnumerator ReleaseButton(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (animVar.Length > 0) animator.SetBool(animVar, false);
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

    public override bool Activated()
    {
        return activated;
    }
}
