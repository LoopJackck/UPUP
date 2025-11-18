using UnityEngine;
using System.Collections;
using Gamekit2D;

public class AnimatedWallTrap : MonoBehaviour
{
    [Header("타이밍 설정")]
    [Tooltip("몇 초마다 애니메이션을 재생할지")]
    public float repeatInterval = 2f;

    [Header("대상 설정")]
    [Tooltip("Door 프리팹의 Animator")]
    public Animator doorAnimator;

    [Tooltip("애니메이션 트리거 이름")]
    public string animationTrigger = "SwitchFlipped";

    [Header("데미지 설정")]
    [Tooltip("Door의 Damager")]
    public Damager damager;

    [Tooltip("Damager가 따라갈 Transform (보통 애니메이션되는 Sprite)")]
    public Transform damagerFollowTarget;

    [Tooltip("애니메이션이 시작되고 몇 초 후에 데미지를 활성화할지")]
    public float damageStartDelay = 0.1f;

    [Tooltip("데미지가 활성화된 후 몇 초 동안 유지할지")]
    public float damageDuration = 0.5f;

    [Header("옵션")]
    [Tooltip("체크하면 애니메이션 없이 상시 데미지 활성화 (애니메이션은 재생되지만 데미지는 항상 ON)")]
    public bool alwaysDamage = false;

    [Tooltip("체크하면 한 번 맞은 후 바로 데미지 비활성화")]
    public bool disableAfterFirstHit = true;

    [Header("디버그")]
    [Tooltip("데미지 타이밍을 콘솔에 출력")]
    public bool showDebugMessages = false;

    private float timer = 0f;
    private Coroutine damageCoroutine;

    void Start()
    {
        // Animator 자동 찾기
        if (doorAnimator == null)
        {
            doorAnimator = GetComponentInChildren<Animator>();
        }

        // Damager 자동 찾기
        if (damager == null)
        {
            damager = GetComponentInChildren<Damager>();
        }

        // Follow Target 자동 설정 (지정되지 않았다면 Animator가 있는 오브젝트로)
        if (damagerFollowTarget == null && doorAnimator != null)
        {
            damagerFollowTarget = doorAnimator.transform;
            if (showDebugMessages)
            {
                Debug.Log($"[AnimatedWallTrap] Follow Target을 자동으로 설정: {damagerFollowTarget.name}");
            }
        }

        // Damager를 애니메이션되는 타겟의 자식으로 이동
        if (damager != null && damagerFollowTarget != null)
        {
            // Damager의 현재 로컬 위치와 회전 저장
            Vector3 currentLocalPos = damager.transform.localPosition;
            Quaternion currentLocalRot = damager.transform.localRotation;
            Vector3 currentLocalScale = damager.transform.localScale;

            // 부모를 변경
            damager.transform.SetParent(damagerFollowTarget, false);

            // 원래 로컬 Transform 유지
            damager.transform.localPosition = currentLocalPos;
            damager.transform.localRotation = currentLocalRot;
            damager.transform.localScale = currentLocalScale;

            if (showDebugMessages)
            {
                Debug.Log($"[AnimatedWallTrap] Damager를 {damagerFollowTarget.name}의 자식으로 이동시켰습니다.");
            }
        }

        // 데미지 설정
        if (damager != null)
        {
            // 상시 데미지 모드면 disableAfterFirstHit을 강제로 false로 설정
            if (alwaysDamage)
            {
                damager.disableDamageAfterHit = false;
                damager.EnableDamage();
                if (showDebugMessages)
                {
                    Debug.Log("[AnimatedWallTrap] 상시 데미지 모드 활성화! (계속 데미지 입힘)");
                }
            }
            else
            {
                // disableAfterFirstHit 옵션 설정 (애니메이션 모드일 때만)
                if (disableAfterFirstHit)
                {
                    damager.disableDamageAfterHit = true;
                }
                else
                {
                    damager.disableDamageAfterHit = false;
                }

                damager.DisableDamage();
            }
        }

        if (showDebugMessages)
        {
            Debug.Log($"[AnimatedWallTrap] 초기화 완료. 상시 데미지: {alwaysDamage}, 데미지 시작 딜레이: {damageStartDelay}초, 지속 시간: {damageDuration}초");
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        // repeatInterval마다 애니메이션 재생
        if (timer >= repeatInterval)
        {
            // 애니메이션 트리거 발동
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger(animationTrigger);
            }

            // 상시 데미지 모드가 아닐 때만 데미지 타이밍 제어
            if (!alwaysDamage)
            {
                // 이전 데미지 코루틴이 있다면 중단 (중복 방지)
                if (damageCoroutine != null)
                {
                    StopCoroutine(damageCoroutine);
                }

                // 데미지 활성화 코루틴 시작
                if (damager != null)
                {
                    damageCoroutine = StartCoroutine(DamageCoroutine());
                }
            }

            timer = 0f;
        }
    }

    IEnumerator DamageCoroutine()
    {
        // 애니메이션 시작 후 잠시 대기 (문이 튀어나오는 동안)
        yield return new WaitForSeconds(damageStartDelay);

        if (showDebugMessages)
        {
            Debug.Log("[AnimatedWallTrap] 데미지 활성화!");
        }

        // 데미지 활성화
        damager.EnableDamage();

        // damageDuration 동안 대기
        yield return new WaitForSeconds(damageDuration);

        if (showDebugMessages)
        {
            Debug.Log("[AnimatedWallTrap] 데미지 비활성화!");
        }

        // 데미지 비활성화
        damager.DisableDamage();

        damageCoroutine = null;
    }

    void OnDisable()
    {
        // 오브젝트가 비활성화될 때 코루틴 정리
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }
}
