using UnityEngine;
using System.Collections;

public class DisappearingPlatform : MonoBehaviour
{
    [SerializeField] private float interval = 2f;

    private Collider2D[] colliders;
    private SpriteRenderer[] spriteRenderers;

    private bool isActive = true;

    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(ToggleRoutine());
    }

    private IEnumerator ToggleRoutine()
    {
        while (true)
        {
            SetPlatformState(!isActive);
            yield return new WaitForSeconds(interval);
        }
    }

    private void SetPlatformState(bool active)
    {
        isActive = active;

        foreach (var col in colliders)
            col.enabled = active;

        foreach (var sr in spriteRenderers)
            sr.enabled = active;
    }
}