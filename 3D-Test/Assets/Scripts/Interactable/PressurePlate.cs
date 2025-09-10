using System;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

[Serializable]
[RequireComponent(typeof(BoxCollider))]
public class PressurePlate : Triggerable
{
    [Header("The Mass Required to Trigger the Pressure Plate")]
    [SerializeField] float triggeringMass = 1.0f;

    private Animator animator;
    private BoxCollider bc;

    private bool activated = false; // Triggerable Objects Read this

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bc = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetBool("activated", activated);
        if (activated)
        {
            Debug.Log("Pressure Plate Activated");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Rigidbody>() != null)
        {
            activated = triggeringMass < other.GetComponent<Rigidbody>().mass;
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
}
