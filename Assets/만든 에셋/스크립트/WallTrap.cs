using UnityEngine;
using Gamekit2D;

public class WallTrap : MonoBehaviour
{
    [Header("타이밍 설정")]
    [Tooltip("숨어있는 시간 (초)")]
    public float waitTime = 2f;

    [Tooltip("나오는데 걸리는 시간 (초)")]
    public float extendTime = 0.3f;

    [Tooltip("나와있는 시간 (초)")]
    public float stayTime = 1.5f;

    [Tooltip("들어가는데 걸리는 시간 (초)")]
    public float retractTime = 0.3f;

    [Header("이동 설정")]
    [Tooltip("벽에서 얼마나 멀리 나올지 (유닛)")]
    public float extendDistance = 2f;

    [Tooltip("시작할 때 딜레이 (초) - 0이면 즉시 시작")]
    public float initialDelay = 0f;

    [Header("데미지 설정")]
    [Tooltip("나올 때만 데미지를 주려면 체크")]
    public bool damageOnlyWhenExtending = false;

    [Header("디버그")]
    public bool showDebugMessages = false;

    private enum State
    {
        Hidden,      // 숨어있음
        Extending,   // 나오는 중
        Extended,    // 완전히 나옴
        Retracting   // 들어가는 중
    }

    private State currentState = State.Hidden;
    private float stateTimer = 0f;
    private Vector3 hiddenPosition;
    private Vector3 extendedPosition;
    private Damager damager;

    void Start()
    {
        // 시작 위치 저장 (숨어있는 위치)
        hiddenPosition = transform.localPosition;

        // 나온 위치 계산 (로컬 X축 방향으로)
        extendedPosition = hiddenPosition + Vector3.right * extendDistance;

        // Damager 컴포넌트 가져오기
        damager = GetComponent<Damager>();

        // 초기 딜레이 설정
        if (initialDelay > 0)
        {
            stateTimer = -initialDelay;
        }
        else
        {
            stateTimer = 0f;
        }

        if (showDebugMessages)
        {
            Debug.Log($"[WallTrap] 초기화 완료. Hidden: {hiddenPosition}, Extended: {extendedPosition}");
        }
    }

    void Update()
    {
        // 초기 딜레이 처리
        if (stateTimer < 0)
        {
            stateTimer += Time.deltaTime;
            return;
        }

        stateTimer += Time.deltaTime;

        switch (currentState)
        {
            case State.Hidden:
                UpdateHidden();
                break;

            case State.Extending:
                UpdateExtending();
                break;

            case State.Extended:
                UpdateExtended();
                break;

            case State.Retracting:
                UpdateRetracting();
                break;
        }
    }

    void UpdateHidden()
    {
        if (stateTimer >= waitTime)
        {
            // 나가기 시작
            ChangeState(State.Extending);

            if (showDebugMessages)
            {
                Debug.Log("[WallTrap] 나가기 시작!");
            }
        }
    }

    void UpdateExtending()
    {
        // 0에서 1까지의 진행도
        float progress = Mathf.Clamp01(stateTimer / extendTime);

        // 부드럽게 이동
        transform.localPosition = Vector3.Lerp(hiddenPosition, extendedPosition, progress);

        // 나가는 중에만 데미지를 주는 옵션
        if (damageOnlyWhenExtending && damager != null)
        {
            damager.EnableDamage();
        }

        if (stateTimer >= extendTime)
        {
            // 완전히 나옴
            transform.localPosition = extendedPosition;
            ChangeState(State.Extended);

            if (showDebugMessages)
            {
                Debug.Log("[WallTrap] 완전히 나옴!");
            }
        }
    }

    void UpdateExtended()
    {
        // 나와있는 상태에서 데미지 활성화
        if (!damageOnlyWhenExtending && damager != null)
        {
            damager.EnableDamage();
        }

        if (stateTimer >= stayTime)
        {
            // 들어가기 시작
            ChangeState(State.Retracting);

            if (showDebugMessages)
            {
                Debug.Log("[WallTrap] 들어가기 시작!");
            }
        }
    }

    void UpdateRetracting()
    {
        // 0에서 1까지의 진행도
        float progress = Mathf.Clamp01(stateTimer / retractTime);

        // 부드럽게 이동
        transform.localPosition = Vector3.Lerp(extendedPosition, hiddenPosition, progress);

        // 들어갈 때 데미지 비활성화
        if (damager != null)
        {
            damager.DisableDamage();
        }

        if (stateTimer >= retractTime)
        {
            // 완전히 들어감
            transform.localPosition = hiddenPosition;
            ChangeState(State.Hidden);

            if (showDebugMessages)
            {
                Debug.Log("[WallTrap] 완전히 들어감!");
            }
        }
    }

    void ChangeState(State newState)
    {
        currentState = newState;
        stateTimer = 0f;
    }

    // 에디터에서 시각화
    void OnDrawGizmos()
    {
        Vector3 startPos = Application.isPlaying ? hiddenPosition : transform.localPosition;
        Vector3 endPos = Application.isPlaying ? extendedPosition : transform.localPosition + Vector3.right * extendDistance;

        // 부모의 위치와 회전을 고려
        if (transform.parent != null)
        {
            startPos = transform.parent.TransformPoint(startPos);
            endPos = transform.parent.TransformPoint(endPos);
        }

        // 숨어있는 위치 (파란색)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startPos, 0.2f);

        // 나온 위치 (빨간색)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(endPos, 0.2f);

        // 이동 경로 (노란색)
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPos, endPos);
    }

    void OnDrawGizmosSelected()
    {
        // 선택되었을 때 더 자세한 정보 표시
        Vector3 currentPos = transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(currentPos, 0.3f);
    }
}
