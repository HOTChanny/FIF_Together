using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class trapOver : MonoBehaviour
{
    // 트랩에 닿았을 때 게임 오버 씬으로 전환할 씬 이름
    public string gameOverSceneName = "NormalEnd"; // Inspector에서 게임 오버 씬 이름("NormalEnd")을 연결해주세요.

    // 플레이어 오브젝트의 태그 (예: "Player")
    // 플레이어 오브젝트에 이 태그를 달아주세요.
    public string playerTag = "Player"; // Inspector에서 플레이어 태그("Player")를 연결해주세요.

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트의 태그가 플레이어 태그와 같은지 확인합니다.
        if(collision.gameObject.CompareTag(playerTag))
        {
            Debug.Log("트랩 발동! 게임 오버."); // 디버그 로그 추가

            // GameManager를 비활성화하는 코드는 제거합니다.
            // GameManager는 DontDestroyOnLoad로 유지되며, 씬 로드 시 UI 상태를 스스로 관리합니다.
            // GameManager.Instance.gameObject.SetActive(false); // 이 줄을 제거합니다.

            // 지정된 게임 오버 씬으로 전환합니다.
            SceneManager.LoadScene(gameOverSceneName);
        }
    }

    // 만약 OnCollisionEnter2D가 작동하지 않고 OnTriggerEnter2D를 사용해야 한다면 (트랩이 Is Trigger인 경우)
    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 오브젝트의 태그가 플레이어 태그와 같은지 확인합니다.
        if(other.gameObject.CompareTag(playerTag))
        {
            Debug.Log("트랩 발동! 게임 오버."); // 디버그 로그 추가

            // GameManager를 비활성화하는 코드는 제거합니다.
            // GameManager.Instance.gameObject.SetActive(false); // 이 줄을 제거합니다.

            // 지정된 게임 오버 씬으로 전환합니다.
            SceneManager.LoadScene(gameOverSceneName);
        }
    }
    */
}
