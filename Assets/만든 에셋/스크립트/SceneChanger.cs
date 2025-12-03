using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필수!

public class SceneChanger : MonoBehaviour
{
    // 이동할 씬의 이름을 인스펙터에서 적을 수 있도록 변수 선언
    public string nextSceneName; 

    // 오브젝트의 트리거 영역에 다른 물체가 들어왔을 때 실행 (2D용)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 닿은 물체가 'Player' 태그를 가지고 있는지 확인
        if (other.CompareTag("Player"))
        {
            // 씬 전환 실행
            SceneManager.LoadScene(nextSceneName);
        }
    }
}