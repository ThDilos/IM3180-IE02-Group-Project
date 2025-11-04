using System.Collections;
using UnityEngine;

public class RespawnableObject : MonoBehaviour
{
    [Header("If not set, default to the object's position on Scene load")]
    [SerializeField] private Transform respawnTransform;
    [SerializeField] private float respawnDelay = 5.0f;

    [Header("SFX")]
    [SerializeField] private Animator animator;
    [SerializeField] private string respawnAnimTrigger;

    // Runtime Vars
    private Vector3 pos;
    private Quaternion rot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (respawnTransform == null) 
            respawnTransform = gameObject.transform;

        pos = respawnTransform.position;
        rot = respawnTransform.rotation;
    }

    private void OnDisable()
    {
        if (GameController.Instance != null)
            GameController.Instance.DelayedSpawnObject(pos, rot, this.gameObject, respawnDelay);
    }

    public void TriggerRespawnSFX()
    {
        if (animator != null)
        {
            animator.SetTrigger(respawnAnimTrigger);
        }
    }
}
