using UnityEngine;
using Gamekit2D;

public class SpringPad : MonoBehaviour
{
    [Header("스프링 패드 설정")]
    [Tooltip("플레이어가 튀어오를 속도 (높을수록 높이 튀어오름)")]
    public float bounceVelocity = 25f;

    [Tooltip("디버그 메시지 표시 여부")]
    public bool showDebugMessages = false;

    private bool hasBouncedPlayer = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어와 충돌했는지 확인
        if (other.CompareTag("Player"))
        {
            // 이미 한 번 튀어올랐으면 무시
            if (hasBouncedPlayer)
            {
                return;
            }

            // PlayerCharacter 컴포넌트 가져오기
            PlayerCharacter player = other.GetComponent<PlayerCharacter>();

            if (player != null)
            {
                // 플레이어의 현재 속도 확인
                Vector2 currentVelocity = player.GetMoveVector();

                // 플레이어가 아래로 떨어지고 있을 때만 튀어오르게 (또는 정지 상태)
                if (currentVelocity.y <= 0.1f)
                {
                    // PlayerCharacter의 SetVerticalMovement를 사용해서 속도 설정
                    player.SetVerticalMovement(bounceVelocity);

                    // 한 번 튀어올랐다고 표시
                    hasBouncedPlayer = true;

                    if (showDebugMessages)
                    {
                        Debug.Log($"[SpringPad] 플레이어 튀어오름! 속도: {bounceVelocity}");
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어가 트리거를 벗어나면 다시 사용 가능하도록 리셋
        if (other.CompareTag("Player"))
        {
            hasBouncedPlayer = false;
            if (showDebugMessages)
            {
                Debug.Log("[SpringPad] 플레이어 이탈 - SpringPad 리셋");
            }
        }
    }

    // 유니티 에디터에서 스프링패드 위치를 시각적으로 표시
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);

        // 위쪽 화살표 그리기
        Gizmos.color = Color.green;
        Vector3 arrowStart = transform.position;
        Vector3 arrowEnd = transform.position + Vector3.up * 2f;
        Gizmos.DrawLine(arrowStart, arrowEnd);
    }
}
