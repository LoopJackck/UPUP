using UnityEngine;
using Gamekit2D;

/// <summary>
/// 일정 간격으로 켜졌다 꺼졌다 하는 레이저 트랩
/// Transform 회전으로 방향 설정 가능
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class LaserTrap : MonoBehaviour
{
    [Header("타이밍 설정")]
    [Tooltip("레이저가 꺼져있는 시간 (초)")]
    public float offTime = 2f;

    [Tooltip("레이저가 켜지는데 걸리는 시간 (초) - 0이면 즉시")]
    public float fadeInTime = 0.2f;

    [Tooltip("레이저가 켜져있는 시간 (초)")]
    public float onTime = 3f;

    [Tooltip("레이저가 꺼지는데 걸리는 시간 (초) - 0이면 즉시")]
    public float fadeOutTime = 0.2f;

    [Tooltip("시작할 때 딜레이 (초) - 0이면 즉시 시작")]
    public float initialDelay = 0f;

    [Header("레이저 설정")]
    [Tooltip("레이저 길이 (유닛)")]
    public float laserLength = 5f;

    [Tooltip("레이저 두께")]
    public float laserWidth = 0.1f;

    [Tooltip("레이저 색상")]
    public Color laserColor = Color.red;

    [Tooltip("경고 색상 (켜지기 직전)")]
    public Color warningColor = new Color(1f, 0.5f, 0f, 0.5f); // 주황색 반투명

    [Header("데미지 설정")]
    [Tooltip("레이저 데미지 (0이면 Damager 컴포넌트 필요)")]
    public int damage = 1;

    [Tooltip("레이저가 꺼질 때 데미지도 비활성화")]
    public bool damageOnlyWhenOn = true;

    [Header("레이어 설정")]
    [Tooltip("레이저가 막히는 레이어 (벽 등)")]
    public LayerMask obstacleLayer;

    [Tooltip("레이저가 데미지를 주는 레이어")]
    public LayerMask damageLayer;

    [Header("디버그")]
    [Tooltip("상태 변화를 콘솔에 출력")]
    public bool showDebugMessages = false;

    private enum State
    {
        Off,        // 꺼짐
        FadingIn,   // 켜지는 중
        On,         // 완전히 켜짐
        FadingOut   // 꺼지는 중
    }

    private State currentState = State.Off;
    private float stateTimer = 0f;
    private LineRenderer lineRenderer;
    private BoxCollider2D laserCollider;
    private Damager damager;
    private float currentLaserLength;

    void Start()
    {
        // LineRenderer 설정
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = false;

        // BoxCollider2D 추가 또는 가져오기
        laserCollider = GetComponent<BoxCollider2D>();
        if (laserCollider == null)
        {
            laserCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        laserCollider.isTrigger = true;

        // Damager 가져오기 또는 추가
        damager = GetComponent<Damager>();
        if (damager == null && damage > 0)
        {
            damager = gameObject.AddComponent<Damager>();
            damager.damage = damage;
            damager.hittableLayers = damageLayer;
            damager.offsetBasedOnSpriteFacing = false;
            damager.canHitTriggers = false;

            // Damager의 offset과 size를 레이저에 맞게 설정
            damager.offset = new Vector2(0f, laserLength / 2f);
            damager.size = new Vector2(laserWidth, laserLength);

            // ⭐ 중요: ContactFilter를 다시 초기화 (hittableLayers 적용)
            damager.RefreshContactFilter();

            if (showDebugMessages)
            {
                Debug.Log($"[LaserTrap] Damager 자동 생성. offset: {damager.offset}, size: {damager.size}, hittableLayers: {damageLayer.value}");
            }
        }
        else if (damager != null)
        {
            // 기존 Damager가 있으면 설정 업데이트
            damager.hittableLayers = damageLayer;
            damager.offset = new Vector2(0f, laserLength / 2f);
            damager.size = new Vector2(laserWidth, laserLength);

            // ContactFilter도 업데이트
            damager.RefreshContactFilter();

            if (showDebugMessages)
            {
                Debug.Log($"[LaserTrap] 기존 Damager 사용. offset: {damager.offset}, size: {damager.size}, hittableLayers: {damageLayer.value}");
            }
        }

        // 초기 딜레이 설정
        if (initialDelay > 0)
        {
            stateTimer = -initialDelay;
        }
        else
        {
            stateTimer = 0f;
        }

        // 처음엔 레이저 끄기
        SetLaserActive(false, 0f);

        if (showDebugMessages)
        {
            Debug.Log($"[LaserTrap] 초기화 완료. 길이: {laserLength}, 방향: {transform.right}");
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
            case State.Off:
                UpdateOff();
                break;

            case State.FadingIn:
                UpdateFadingIn();
                break;

            case State.On:
                UpdateOn();
                break;

            case State.FadingOut:
                UpdateFadingOut();
                break;
        }

        // 레이저 렌더링 업데이트
        UpdateLaserVisual();
        UpdateLaserCollider();
    }

    void UpdateOff()
    {
        // 레이저 완전히 끄기
        SetLaserActive(false, 0f);

        if (stateTimer >= offTime)
        {
            // 켜지기 시작
            ChangeState(State.FadingIn);

            if (showDebugMessages)
            {
                Debug.Log("[LaserTrap] 켜지기 시작!");
            }
        }
    }

    void UpdateFadingIn()
    {
        float progress = fadeInTime > 0 ? Mathf.Clamp01(stateTimer / fadeInTime) : 1f;

        // 경고 색상에서 레이저 색상으로 페이드
        Color currentColor = Color.Lerp(warningColor, laserColor, progress);
        SetLaserActive(true, progress, currentColor);

        if (stateTimer >= fadeInTime)
        {
            // 완전히 켜짐
            ChangeState(State.On);

            if (showDebugMessages)
            {
                Debug.Log("[LaserTrap] 완전히 켜짐!");
            }
        }
    }

    void UpdateOn()
    {
        // 완전히 켜진 상태 유지
        SetLaserActive(true, 1f);

        if (stateTimer >= onTime)
        {
            // 꺼지기 시작
            ChangeState(State.FadingOut);

            if (showDebugMessages)
            {
                Debug.Log("[LaserTrap] 꺼지기 시작!");
            }
        }
    }

    void UpdateFadingOut()
    {
        float progress = fadeOutTime > 0 ? Mathf.Clamp01(stateTimer / fadeOutTime) : 1f;

        // 레이저 색상에서 투명으로 페이드
        SetLaserActive(true, 1f - progress);

        if (stateTimer >= fadeOutTime)
        {
            // 완전히 꺼짐
            ChangeState(State.Off);

            if (showDebugMessages)
            {
                Debug.Log("[LaserTrap] 완전히 꺼짐!");
            }
        }
    }

    void ChangeState(State newState)
    {
        currentState = newState;
        stateTimer = 0f;
    }

    void SetLaserActive(bool active, float intensity, Color? color = null)
    {
        lineRenderer.enabled = active;

        if (active)
        {
            Color useColor = color ?? laserColor;
            useColor.a *= intensity;
            lineRenderer.startColor = useColor;
            lineRenderer.endColor = useColor;
        }

        // 데미지 활성화/비활성화
        if (damager != null)
        {
            if (damageOnlyWhenOn)
            {
                if (active && intensity > 0.5f)
                {
                    damager.EnableDamage();
                    if (showDebugMessages)
                    {
                        Debug.Log($"[LaserTrap] 데미지 활성화! (intensity: {intensity})");
                    }
                }
                else
                {
                    damager.DisableDamage();
                    if (showDebugMessages)
                    {
                        Debug.Log($"[LaserTrap] 데미지 비활성화 (intensity: {intensity})");
                    }
                }
            }
            else
            {
                damager.EnableDamage();
            }
        }

        // 콜라이더 활성화/비활성화
        if (laserCollider != null)
        {
            laserCollider.enabled = active && intensity > 0.5f;
        }
    }

    void UpdateLaserVisual()
    {
        // 시작점 (로컬 원점)
        Vector3 startPos = Vector3.zero;

        // 레이저 방향 (로컬 X축 = 오른쪽)
        Vector3 direction = Vector3.right;

        // 장애물 체크
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, laserLength, obstacleLayer);

        if (hit.collider != null)
        {
            // 장애물에 막힘
            currentLaserLength = hit.distance;
        }
        else
        {
            // 전체 길이
            currentLaserLength = laserLength;
        }

        Vector3 endPos = direction * currentLaserLength;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    void UpdateLaserCollider()
    {
        if (laserCollider == null)
            return;

        // 콜라이더를 레이저 중심에 배치
        laserCollider.offset = new Vector2(0f, currentLaserLength / 2f);
        laserCollider.size = new Vector2(laserWidth, currentLaserLength);

        // Damager의 offset과 size도 업데이트
        if (damager != null)
        {
            damager.offset = new Vector2(0f, currentLaserLength / 2f);
            damager.size = new Vector2(laserWidth, currentLaserLength);
        }
    }

    // 에디터에서 시각화
    void OnDrawGizmos()
    {
        // 레이저 방향 표시
        Gizmos.color = Color.red;
        Vector3 start = transform.position;
        Vector3 end = transform.position + transform.right * laserLength;
        Gizmos.DrawLine(start, end);

        // 시작점 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(start, 0.1f);

        // 끝점 표시
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(end, 0.1f);
    }

    void OnDrawGizmosSelected()
    {
        // 선택되었을 때 더 자세한 정보 표시
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);

        Vector3 start = transform.position;
        Vector3 direction = transform.right;

        // 레이저 두께 표시
        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0) * (laserWidth / 2f);
        Vector3 p1 = start + perpendicular;
        Vector3 p2 = start - perpendicular;
        Vector3 p3 = start + direction * laserLength + perpendicular;
        Vector3 p4 = start + direction * laserLength - perpendicular;

        Gizmos.DrawLine(p1, p3);
        Gizmos.DrawLine(p2, p4);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p3, p4);
    }
}
