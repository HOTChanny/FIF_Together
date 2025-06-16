using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Text 컴포넌트 사용 시 필요

public class HealthController : MonoBehaviour
{
    public float cooldownHit;
    private float rateOfHit;
    private GameObject[] life; // GameManager에서 가져올 체력 이미지 배열
    // 로컬 체력 변수 (qtdLife, maxLife)는 이제 GameManager에서 관리하므로 필요 없습니다.

    void Start()
    {
        rateOfHit = Time.time;

        if (GameManager.Instance != null)
        {
            life = GameManager.Instance.GetHpSprites();
            if (life != null && life.Length > 0)
            {
                // 씬 로드 시 현재 체력 값에 맞게 체력 이미지의 시각적 상태를 업데이트합니다.
                // 이 부분은 GameManager의 OnSceneLoaded에서도 호출되지만,
                // HealthController가 체력 이미지를 가져온 후 한 번 더 호출하여 확실하게 상태를 맞춥니다.
                GameManager.Instance.UpdateHpSpritesVisual();

                Debug.Log($"HealthController 초기화 완료. GameManager 현재 체력: {GameManager.Instance.currentHealth}, 최대 체력: {GameManager.Instance.maxHealth}");
            }
            else
            {
                Debug.LogError("HealthController: GameManager에서 체력 이미지를 가져올 수 없거나 배열이 비어 있습니다. GameManager의 gameUIContainer 설정 및 자식 오브젝트 확인이 필요합니다.");
            }
        }
        else
        {
            Debug.LogError("HealthController: GameManager 인스턴스를 찾을 수 없습니다! GameManager 오브젝트가 타이틀 씬에 있고 DontDestroyOnLoad 설정되었는지 확인하세요.");
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss"))
        {
            Hurt();
        }
    }

    public void Hurt()
    {
        if (rateOfHit < Time.time)
        {
            rateOfHit = Time.time + cooldownHit;

            if (GameManager.Instance != null)
            {
                // GameManager의 currentHealth 값을 직접 변경하는 대신, GameManager의 함수를 호출합니다.
                if (GameManager.Instance.currentHealth > 0)
                {
                    GameManager.Instance.DecreaseHealth(1); // GameManager의 체력 감소 함수 호출
                    Debug.Log("맞았다! 남은 체력: " + GameManager.Instance.currentHealth);

                    // 체력 이미지의 시각적 상태 업데이트는 GameManager.DecreaseHealth() 함수 안에서 처리됩니다.
                    // GameManager.Instance.UpdateHpSpritesVisual(); // 이 줄은 필요 없습니다.

                    // 체력이 0 이하가 되면 게임 오버 처리
                    if (GameManager.Instance.currentHealth <= 0)
                    {
                        Debug.Log("체력 소진! 게임 오버.");

                        // 현재 로드된 씬의 이름을 가져옵니다.
                        string currentSceneName = SceneManager.GetActiveScene().name;

                        // 보스 씬의 실제 이름으로 "BossScene" 부분을 바꿔주세요.
                        string bossSceneName = "BossScene"; // <-- 여기에 실제 보스 씬 이름 입력
                        string normalEndSceneName = "NormalEnd"; // <-- NormalEnd 씬의 실제 이름 사용
                        string worstEndSceneName = "WorstEndScene"; // <-- WorstEndScene의 실제 이름 사용

                        if (currentSceneName == bossSceneName)
                        {
                            // 현재 씬이 보스 씬이면 WorstEndScene으로 전환
                            GameManager.Instance.GameOver(worstEndSceneName); // GameManager의 게임 오버 함수 호출
                            Debug.Log("보스전 패배! WorstEndScene으로 전환.");
                        }
                        else
                        {
                            // 보스 씬이 아니면 NormalEnd 씬으로 전환 (메인 게임 씬 등)
                            GameManager.Instance.GameOver(normalEndSceneName); // GameManager의 게임 오버 함수 호출
                            Debug.Log("일반 게임 오버! NormalEnd 씬으로 전환.");
                        }
                    }
                }
                // else (currentHealth <= 0) 이미 체력이 0 이하면 추가 데미지 처리 안 함 (GameManager.DecreaseHealth() 안에서 처리)
            }
            else
            {
                Debug.LogError("HealthController: GameManager 인스턴스를 찾을 수 없어 데미지 처리를 할 수 없습니다.");
            }
        }
    }

    public void Heal()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("HealthController: GameManager 인스턴스를 찾을 수 없어 회복 처리를 할 수 없습니다.");
            return;
        }

        // GameManager의 currentHealth 값을 직접 변경하는 대신, GameManager의 함수를 호출합니다.
        if (GameManager.Instance.currentHealth < GameManager.Instance.maxHealth)
        {
            GameManager.Instance.IncreaseHealth(1); // GameManager의 체력 회복 함수 호출
            Debug.Log("체력 회복! 현재 체력: " + GameManager.Instance.currentHealth);

            // 체력 이미지의 시각적 상태 업데이트는 GameManager.IncreaseHealth() 함수 안에서 처리됩니다.
            // GameManager.Instance.UpdateHpSpritesVisual(); // 이 줄은 필요 없습니다.
        }
        else
        {
            Debug.Log("체력 회복 시도: 이미 최대 체력입니다.");
        }
    }
}
