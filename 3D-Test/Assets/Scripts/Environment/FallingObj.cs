using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FallingObj : MonoBehaviour
{
    [Header("Check if player is standing on the log")]
    [Header("The entire functionality relies on Animator.")]
    [SerializeField] private float duration = 3f;
    [SerializeField] private float respawnDelay = 5f;
    [SerializeField] private LayerMask playerLayerMask = 0;

    [Header("Animations Vars")]
    [SerializeField] private string aboutToFall = "";
    [SerializeField] private string falling = "";
    [SerializeField] private string respawning = "";

    // Runtime Var
    public bool triggered = false;
    private BoxCollider bc;
    private float timer = 0f;
    private int state = 0; // 0 = Idle, 1 = triggered, 2 = falling, 3 = respawning
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();

        if (GetComponent<BoxCollider>() != null)
        {
            bc = GetComponent<BoxCollider>();
        }
        else
        {
            bc = GetComponentInChildren<BoxCollider>();
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.fixedDeltaTime;
        }

        switch (state)
        {
            case 0:
                CheckForPlayerOnTop(); break;
            case 1:
                if (timer <= 0f)
                {
                    state = 2;
                    animator.SetTrigger(falling);
                    timer = respawnDelay;
                }
                break;
            case 2:
                if (timer <= 0f)
                {
                    state = 3;
                }
                break;
            case 3:
                Reset();
                break;
        }
    }

    private void CheckForPlayerOnTop()
    {
        // If player detected, enter state 1
        if (Physics.OverlapBox(bc.bounds.center + new Vector3(0, 1, 0), bc.bounds.size / 2, Quaternion.identity, playerLayerMask).Length > 0)
        {
            animator.SetTrigger(aboutToFall);
            timer = duration;
            state = 1;
        }
    }

    private void Reset()
    {
        state = 0;
        timer = 0f;
        animator.SetTrigger(respawning);
    }
}
