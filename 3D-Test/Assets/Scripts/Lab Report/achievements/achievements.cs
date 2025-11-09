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

    public static bool gotKnife = false;
    public static bool gotRubberDuck = false;
    public static bool pushedFlowerPot = false;
    public static bool catOOB = false;

    private void Start()
    {
        // start with none visible
        foreach (var badge in badges)
            badge.unlocked = false;

        // if collected in prev scene
        if (gotKnife) UnlockBadge("knife");
        if (gotRubberDuck) UnlockBadge("rubberDuck");
        if (pushedFlowerPot) UnlockBadge("flowerPot");
        if (catOOB) UnlockBadge("oob");
    }

    private void Update()
    {
        // Check each condition once
        if (gameController.gotKnife) { UnlockBadge("knife"); gotKnife = true; }
        if (gameController.gotRubberDuck) { UnlockBadge("rubberDuck"); gotRubberDuck = true; }
        if (gameController.pushedFlowerPot) { UnlockBadge("flowerPot"); pushedFlowerPot = true; }
        if (gameController.catOOB) { UnlockBadge("oob"); catOOB = true; }
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
