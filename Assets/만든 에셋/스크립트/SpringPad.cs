using UnityEngine;
using Gamekit2D;

public class SpringPad : MonoBehaviour
{
    [Header("스프링 패드 설정")]
    [Tooltip("플레이어가 튀어오를 속도 (높을수록 높이 튀어오름)")]
    public float bounceVelocity = 25f;

    [Tooltip("디버그 메시지 표시 여부")]
    public bool showDebugMessages = false;

    [Header("스프라이트 설정")]
    [Tooltip("자식 오브젝트의 SpriteRenderer를 여기에 드래그하세요")]
    public SpriteRenderer spriteRenderer;

    [Tooltip("눌리지 않은 상태 (PressurePad_1)")]
    public Sprite unpressedSprite;

    [Tooltip("눌린 상태 (PressurePad_0)")]
    public Sprite pressedSprite;

    [Tooltip("눌린 이미지가 유지되는 시간 (초)")]
    public float pressedDuration = 0.3f;

    private bool hasBouncedPlayer = false;
    private Coroutine resetSpriteCoroutine = null;

    private void Start()
    {
        // 시작할 때 PressurePad_1로 설정
        if (spriteRenderer != null && unpressedSprite != null)
        {
            spriteRenderer.sprite = unpressedSprite;
        }
    }

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
                    // 스프링의 회전 각도에 따른 방향으로 튀어오르기
                    // transform.up은 스프링이 가리키는 방향 (회전에 따라 변함)
                    Vector2 bounceDirection = transform.up.normalized;
                    Vector2 bounceVector = bounceDirection * bounceVelocity;

                    // PlayerCharacter의 MoveVector를 설정 (이렇게 해야 PlayerCharacter의 움직임 시스템과 충돌하지 않음)
                    player.SetMoveVector(bounceVector);

                    // 한 번 튀어올랐다고 표시
                    hasBouncedPlayer = true;

                    // 스프라이트를 눌린 상태로 변경하고 일정 시간 후 복원
                    if (spriteRenderer != null && pressedSprite != null)
                    {
                        spriteRenderer.sprite = pressedSprite;

                        // 이전에 실행 중인 코루틴이 있으면 중지
                        if (resetSpriteCoroutine != null)
                        {
                            StopCoroutine(resetSpriteCoroutine);
                        }

                        // 일정 시간 후 스프라이트 복원 코루틴 시작
                        resetSpriteCoroutine = StartCoroutine(ResetSpriteAfterDelay());
                    }

                    if (showDebugMessages)
                    {
                        Debug.Log($"[SpringPad] 플레이어 튀어오름! 방향: {bounceDirection}, 속도: {bounceVelocity}");
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

    private System.Collections.IEnumerator ResetSpriteAfterDelay()
    {
        // 지정된 시간만큼 대기
        yield return new WaitForSeconds(pressedDuration);

        // 스프라이트를 눌리지 않은 상태로 복원
        if (spriteRenderer != null && unpressedSprite != null)
        {
            spriteRenderer.sprite = unpressedSprite;

            if (showDebugMessages)
            {
                Debug.Log("[SpringPad] 스프라이트 복원");
            }
        }

        resetSpriteCoroutine = null;
    }

    // 유니티 에디터에서 스프링패드 위치를 시각적으로 표시
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.localScale);

        // 스프링이 가리키는 방향으로 화살표 그리기
        Gizmos.color = Color.green;
        Vector3 arrowStart = transform.position;
        Vector3 arrowEnd = transform.position + transform.up * 2f;
        Gizmos.DrawLine(arrowStart, arrowEnd);

        // 화살표 끝에 작은 원 그리기
        Gizmos.DrawWireSphere(arrowEnd, 0.2f);
    }
}
