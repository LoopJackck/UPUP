using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSceneChanger : MonoBehaviour
{
    public string nextSceneName; // 이동할 씬 이름 (인스펙터에서 입력)
    public bool isReturnToMainMenu = false; // 메인 메뉴로 돌아가는 버튼인지 체크

    public void ChangeSceneByButton()
    {
        // 씬 전환 전에 게임 시간을 정상으로 되돌림
        Time.timeScale = 1f;

        // 메인 메뉴로 돌아갈 때는 2DGamekit의 SceneController를 파괴
        if (isReturnToMainMenu)
        {
            // 2DGamekit의 SceneController 찾아서 파괴
            var sceneController = FindObjectOfType<Gamekit2D.SceneController>();
            if (sceneController != null)
            {
                Destroy(sceneController.gameObject);
            }

            // PersistentDataManager도 리셋 (있다면)
            var persistentDataManager = FindObjectOfType<Gamekit2D.PersistentDataManager>();
            if (persistentDataManager != null)
            {
                Destroy(persistentDataManager.gameObject);
            }
        }

        SceneManager.LoadScene(nextSceneName);
    }
}