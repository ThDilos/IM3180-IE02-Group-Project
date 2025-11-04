using UnityEngine;

public class LogRollingAudio : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;   // optional; auto-created if missing
    [SerializeField] private AudioClip rollingLoop;     // looped rolling sound
    [SerializeField] private float baseVolume = 0.15f;  // quiet at low speed
    [SerializeField] private float maxVolume = 0.6f;    // cap volume
    [SerializeField] private float minSpeedToPlay = 0.5f;   // start playing above this linear speed
    [SerializeField] private float fadeSpeed = 6f;      // how fast volume changes (smooth)

    [Header("Lava Detection")]
    [SerializeField] private GameObject lavaSurface;
    [SerializeField] private bool inLava;

    private Rigidbody rb;
    private int groundContacts;        
    private float targetVolume;          
    

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;    // 3D
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
        if (rollingLoop) audioSource.clip = rollingLoop;
        audioSource.volume = 0f;
    }

    void FixedUpdate()
    {
        // Decide if "rolling" this physics step:
        bool isGrounded = groundContacts > 0;
        float speed = rb.linearVelocity.magnitude;
        bool shouldPlay = isGrounded && speed >= minSpeedToPlay && rollingLoop;

        // Target volume scales with speed, clamped
        targetVolume = shouldPlay
            ? Mathf.Clamp(baseVolume + (speed - minSpeedToPlay) * 0.1f, baseVolume, maxVolume)
            : 0f;

        // Start/stop clip as needed (actual volume smoothing happens in Update)
        if (shouldPlay)
        {
            if (!audioSource.isPlaying) audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying && Mathf.Approximately(targetVolume, 0f))
                audioSource.Stop();
        }

        // Reset for next FixedUpdate; OnCollisionStay will repopulate
        groundContacts = 0;
    }

    void Update()
    {
        // Smoothly approach targetVolume (nice fade in/out)
        audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);

    }
    void OnCollisionStay(Collision collision)
    {
        if (lavaSurface != null && collision.collider.gameObject == lavaSurface)
        {
            // Stop sound immediately
            targetVolume = 0f;
            if (audioSource.isPlaying)
                audioSource.Stop();

            return; // don't process ground contacts anymore
        }
    }
}
