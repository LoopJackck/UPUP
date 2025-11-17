using UnityEngine;

public class Shooter_Bullet : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke(nameof(Deactivate), 3f); // 3초 후 자동 비활성화
    }

    private void OnDisable()
    {
        CancelInvoke(); // 비활성 중 중복 타이머 방지
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}
