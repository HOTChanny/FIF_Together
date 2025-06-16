using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void SceneChange()
    {
        // GameManager 인스턴스가 있는지 확인하고 새로운 게임 초기화 함수를 호출합니다.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.InitializeNewGame(); // 새로운 게임 상태 초기화 (골드 1000, 체력 최대치)
        }
        else
        {
            Debug.LogError("SceneChanger: GameManager 인스턴스를 찾을 수 없습니다! 새로운 게임 초기화 실패.");
        }

        // MainScene 로드
        SceneManager.LoadScene("MainScene");
    }
}
