using UnityEngine;
using System.Collections;

public class CrumblingPlatform : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 1f;
    [SerializeField] private float respawnDelay = 3f;
    [SerializeField] private float shakeAmount = 0.1f;

    private Vector3 originalPos;

    private Collider2D[] colliders;   // 모든 Collider
    private Renderer[] renderers;     // 모든 Renderer (SpriteRenderer, TilemapRenderer 등 전부 포함)

    private bool isBreaking = false;

    private void Awake()
    {
        originalPos = transform.localPosition;

        colliders = GetComponentsInChildren<Collider2D>();
        renderers = GetComponentsInChildren<Renderer>();   // ★ 모든 렌더러 자동 감지
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isBreaking && collision.collider.CompareTag("Player"))
        {
            StartCoroutine(BreakPlatform());
        }
    }

    private IEnumerator BreakPlatform()
    {
        isBreaking = true;

        // 1) 흔들림
        float timer = 0f;
        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;
            float offset = Mathf.Sin(Time.time * 50f) * shakeAmount;
            transform.localPosition = originalPos + new Vector3(offset, 0f, 0f);
            yield return null;
        }

        // 원위치 복원
        transform.localPosition = originalPos;

        // 2) 비활성화 (Collider + Renderer)
        SetPlatformVisible(false);
        SetPlatformCollidable(false);

        // 3초 대기
        yield return new WaitForSeconds(respawnDelay);

        // 3) 재활성화
        SetPlatformVisible(true);
        SetPlatformCollidable(true);

        isBreaking = false;
    }

    private void SetPlatformVisible(bool isVisible)
    {
        foreach (var r in renderers)
            r.enabled = isVisible;
    }

    private void SetPlatformCollidable(bool isCollidable)
    {
        foreach (var c in colliders)
            c.enabled = isCollidable;
    }
}