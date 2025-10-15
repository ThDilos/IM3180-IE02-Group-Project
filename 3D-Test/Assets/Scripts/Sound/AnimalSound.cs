using UnityEngine;
using UnityEngine.Audio;


public class AnimalSound : MonoBehaviour
{
    public SwitchCharacter switchCharacter;
    public Movement movementScript;

    public AudioSource audioSource;
    public AudioClip[] catNoise;
    public AudioClip[] gooseNoise;
    public AudioClip[] bearNoise;
    public AudioClip normalJump;
    public AudioClip highJump;
    public AudioClip glide;
    public AudioClip respawn;
    public AudioClip waterSplash;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayRandomCurrentAnimalNoise();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (movementScript.CanJump())
                PlayJumpSound();
        }
        if (movementScript.ClassMechanicsGoose())
        {
            gooseGlide();
        }
        //if (movementScript.respawned)
        //{
        //    audioSource.PlayOneShot(respawn);
        //}
        //if (waterScript.GooseInWater)
        //{
        //    audioSource.PlayOneShot(waterSplash);
        //}
    }
    void PlayRandomCurrentAnimalNoise()
    {
        SwitchCharacter.ActivatedCharacter current = switchCharacter.activatedCharacter;
        AudioClip[] chosenClips = null;

        if (current == SwitchCharacter.ActivatedCharacter.CAT)
        {
            chosenClips = catNoise;
        }
        else if (current == SwitchCharacter.ActivatedCharacter.GOOSE)
        {
            chosenClips = gooseNoise;
        }
        else 
        {
            chosenClips = bearNoise;
        }

        int index = Random.Range(0, chosenClips.Length);
        AudioClip clip = chosenClips[index];
        audioSource.PlayOneShot(clip);

    }
    void PlayJumpSound()
    {
        SwitchCharacter.ActivatedCharacter current = switchCharacter.activatedCharacter;
        if (current == SwitchCharacter.ActivatedCharacter.CAT)
        {
            audioSource.PlayOneShot(highJump);
        }
        else if (current == SwitchCharacter.ActivatedCharacter.GOOSE)
        {
            audioSource.PlayOneShot(normalJump);
        }
        else 
        {
            audioSource.PlayOneShot(normalJump);
        }
    }
    void gooseGlide()
    {
        SwitchCharacter.ActivatedCharacter current = switchCharacter.activatedCharacter;
        float originalPitch = audioSource.pitch;

        if (current == SwitchCharacter.ActivatedCharacter.GOOSE)
        {
            audioSource.pitch = 3f; //faster
            audioSource.PlayOneShot(glide, 0.5f); //softer
            audioSource.pitch = originalPitch;

        }
    }
}
