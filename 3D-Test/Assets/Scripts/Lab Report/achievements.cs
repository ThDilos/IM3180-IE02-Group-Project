using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievements : MonoBehaviour
{
    [System.Serializable]
    public class Badge
    {
        public string name;
        public Sprite icon;
        public bool unlocked;
        [HideInInspector] public Image imageInstance;
    }

    [Header("UI Parent to hold badges (with Vertical or Grid Layout Group)")]
    [SerializeField] private Transform badgeContainer;

    [Header("Prefab for badge Image (simple Image object)")]
    [SerializeField] private Image badgePrefab;

    [Header("List of all possible badges")]
    [SerializeField] private List<Badge> badges = new();

    [SerializeField] private GameController gameController;

    private void Start()
    {
        // start with none visible
        foreach (var badge in badges)
            badge.unlocked = false;
    }

    private void Update()
    {
        // Check each condition once
        if (gameController.gotKnife) UnlockBadge("knife");
        if (gameController.gotRubberDuck) UnlockBadge("rubberDuck");
        if (gameController.pushedFlowerPot) UnlockBadge("flowerPot");
        if (gameController.catOOB) UnlockBadge("oob");
    }

    private void UnlockBadge(string badgeName)
    {
        var badge = badges.Find(b => b.name == badgeName);
        if (badge == null || badge.unlocked) return;

        badge.unlocked = true;

        // instantiate the badge image in the container
        var newBadge = Instantiate(badgePrefab, badgeContainer);
        newBadge.sprite = badge.icon;
        newBadge.enabled = true;
        badge.imageInstance = newBadge;

        // Layout Group will automatically place this new badge in order of unlock
    }
}
