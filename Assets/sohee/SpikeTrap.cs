using UnityEngine;
using System.Collections;
using Gamekit2D;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private float hideDuration = 1.5f;
    [SerializeField] private float showDuration = 0.2f;
    [SerializeField] private float stayDuration = 2f;
    [SerializeField] private float hideMoveDuration = 0.2f;
    [SerializeField] private float moveAmount = 0.4f;

    private Vector3 hiddenPos;
    private Vector3 shownPos;

    private Damager damager;
    private Collider2D spikeCollider;

    private void Start()
    {
        hiddenPos = transform.position;
        shownPos = hiddenPos + new Vector3(0, moveAmount, 0);

        damager = GetComponent<Damager>();
        spikeCollider = GetComponent<Collider2D>();

        StartCoroutine(SpikeRoutine());
    }

    private IEnumerator SpikeRoutine()
    {
        while (true)
        {
            damager.DisableDamage();
            spikeCollider.enabled = false;
            transform.position = hiddenPos;
            yield return new WaitForSeconds(hideDuration);

            yield return MoveSpike(hiddenPos, shownPos, showDuration);

            damager.EnableDamage();
            spikeCollider.enabled = true;
            yield return new WaitForSeconds(stayDuration);

            damager.DisableDamage();
            spikeCollider.enabled = false;
            yield return MoveSpike(shownPos, hiddenPos, hideMoveDuration);
        }
    }

    private IEnumerator MoveSpike(Vector3 start, Vector3 end, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, t / duration);
            yield return null;
        }
        transform.position = end;
    }
}
