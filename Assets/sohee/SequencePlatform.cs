using UnityEngine;

public class SequencePlatform : MonoBehaviour
{
    public bool isOdd = true;

    private Collider2D[] colliders;
    private SpriteRenderer[] spriteRenderers;

    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        UpdatePlatformState();
    }

    private void Update()
    {
        UpdatePlatformState();
    }

    private void UpdatePlatformState()
    {
        bool shouldBeActive = (PlayerJumpCounter.jumpCount % 2 == 1 && isOdd) ||
                              (PlayerJumpCounter.jumpCount % 2 == 0 && !isOdd);

        foreach (var col in colliders)
            col.enabled = shouldBeActive;

        foreach (var sr in spriteRenderers)
            sr.enabled = shouldBeActive;
    }
}