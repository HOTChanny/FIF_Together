using System;
using System.Collections;
using System.Collections.Generic;
// using Unity.VisualScripting; // 이 using 지시문이 필요한지 확인하세요.
using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager 사용 시 필요

public class Gold : MonoBehaviour
{
    // float currentPositionX; // 사용되지 않는 변수 같으니 필요 없으면 삭제하세요.

    SpriteRenderer spriteRenderer; // 골드 아이템 자체의 스프라이트 렌더러
    // HealthController health; // 이제 GameManager를 통해 체력 회복을 처리할 수 있으므로 필요 없을 수 있습니다.
    [SerializeField] private GameObject MoneySprite; // 이 오브젝트가 골드 아이템 자체를 의미하는지 확인하세요.
    [SerializeField] private GameObject SoldOut1Sprite; // 상점 구매 완료 표시 스프라이트
    [SerializeField] private GameObject SoldOut2Sprite;
    [SerializeField] private GameObject SoldOut3Sprite;

    public int Store1Money; // 상점1 가격
    public int Store2Money; // 상점2 가격
    public int Store3Money; // 상점3 가격

    public GameObject NPCUI3; // 체력 만땅일 때 뜨는 UI?
    public GameObject NPCBeggar; // 돈 부족할 때 뜨는 UI?

    private void Start()
    {
        // 이 스크립트가 붙어있는 오브젝트의 SpriteRenderer를 가져옵니다. (골드 아이템 자체)
        spriteRenderer = GetComponent<SpriteRenderer>(); // GetComponentsInChildren 대신 GetComponent 사용
        // health = GameObject.Find("Player").GetComponent<HealthController>(); // 이제 GameManager를 통해 체력 회복을 처리할 수 있으므로 필요 없을 수 있습니다.

        // MoneySprite가 이 스크립트가 붙은 오브젝트 자체라면 MoneySprite 필드는 필요 없을 수 있습니다.
        // 만약 MoneySprite가 별도의 자식 오브젝트라면 GetComponentInChildren<SpriteRenderer>()를 사용해야 합니다.
        // 현재 코드에서는 MoneySprite.SetActive(false)를 호출하므로 MoneySprite 필드가 필요해 보입니다.
        // MoneySprite가 이 스크 스크립트가 붙은 오브젝트 자체라면 MoneyBye 함수를 transform.gameObject.SetActive(false)로 수정해야 합니다.
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 오브젝트의 태그가 "Player"인지 확인합니다.
        if (collision.gameObject.CompareTag("Player")) // == "Player" 대신 CompareTag 사용 권장
        {
            // 골드 아이템의 시각적/물리적 요소를 비활성화하여 한 번만 획득되도록 합니다.
            // 스프라이트 투명도 조절
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 0.3f;
                spriteRenderer.color = color;
            }
            // Collider 등 컴포넌트 비활성화
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
            // Rigidbody 등 다른 Behaviour 컴포넌트도 비활성화할 수 있습니다.
            // foreach (Behaviour component in transform.GetComponentsInChildren<Behaviour>())
            // {
            //     component.enabled = false;
            // } // 이 코드는 자식 오브젝트의 모든 Behaviour를 끄므로 주의해서 사용해야 합니다.

            GainMoney(); // 골드 획득 함수 호출
            Invoke("MoneyBye", 1f); // 1초 후 MoneyBye 함수 호출 (오브젝트 비활성화 등)
        }
    }

    public void MoneyBye()
    {
        // 골드 아이템 오브젝트 자체를 비활성화합니다.
        // MoneySprite 필드가 이 오브젝트 자체를 가리킨다면 이대로 사용하거나 transform.gameObject.SetActive(false)로 변경
        if (MoneySprite != null)
        {
            MoneySprite.SetActive(false);
        }
        else
        {
            // MoneySprite 필드가 설정되지 않았다면 이 스크립트가 붙은 오브젝트를 비활성화
            gameObject.SetActive(false);
        }
    }

    // private int currentValue; // GameManager에서 골드 값을 관리하므로 필요 없습니다.
    public void GainMoney()
    {
        if (GameManager.Instance != null)
        {
            // GameManager의 Money 값에 랜덤 값을 더합니다.
            int amountToGain = UnityEngine.Random.Range(22222, 44444);
            GameManager.Instance.AddMoney(amountToGain); // GameManager의 AddMoney 함수 사용
            // GameManager.Instance.UserMoney.text 업데이트는 AddMoney 함수 안에서 처리됩니다.
        }
        else
        {
            Debug.LogError("Gold: GameManager 인스턴스를 찾을 수 없어 골드를 획득할 수 없습니다.");
        }
    }

    // 상점 구매 함수들
    public void BuyBtn1()
    {
        if (GameManager.Instance != null)
        {
            // 체력 만땅 체크는 HealthController가 아닌 GameManager의 currentHealth 사용
            if (GameManager.Instance.currentHealth >= GameManager.Instance.maxHealth)
            {
                if (NPCUI3 != null) NPCUI3.SetActive(true); // 체력 만땅 UI 표시
                Debug.Log("상점1 구매 시도: 이미 최대 체력입니다.");
            }
            else
            {
                // GameManager의 TrySpendMoney 함수를 사용하여 돈을 사용하고 성공 여부 확인
                if (GameManager.Instance.TrySpendMoney(Store1Money))
                {
                    // 돈 사용 성공 시 체력 회복
                    // HealthController의 Heal 함수 호출 (HealthController가 GameManager의 체력 값을 업데이트)
                    HealthController playerHealth = GameObject.FindObjectOfType<HealthController>(); // 씬에서 HealthController 찾기
                    if (playerHealth != null)
                    {
                        playerHealth.Heal();
                        if (SoldOut1Sprite != null) SoldOut1Sprite.SetActive(true); // 판매 완료 표시
                        Debug.Log($"상점1 구매 성공: {Store1Money} 골드 사용, 체력 회복.");
                    }
                    else
                    {
                        Debug.LogError("Gold: HealthController를 찾을 수 없어 체력 회복을 할 수 없습니다.");
                    }
                }
                else
                {
                    // 돈 사용 실패 시 (골드 부족)
                    if (NPCBeggar != null) NPCBeggar.SetActive(true); // 돈 부족 UI 표시
                    Debug.Log("상점1 구매 실패: 골드 부족.");
                }
            }
        }
        else
        {
            Debug.LogError("Gold: GameManager 인스턴스를 찾을 수 없어 상점 구매를 할 수 없습니다.");
        }
    }

    public void BuyBtn2()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.currentHealth >= GameManager.Instance.maxHealth)
            {
                if (NPCUI3 != null) NPCUI3.SetActive(true);
                Debug.Log("상점2 구매 시도: 이미 최대 체력입니다.");
            }
            else
            {
                if (GameManager.Instance.TrySpendMoney(Store2Money))
                {
                    HealthController playerHealth = GameObject.FindObjectOfType<HealthController>();
                    if (playerHealth != null)
                    {
                        playerHealth.Heal();
                        if (SoldOut2Sprite != null) SoldOut2Sprite.SetActive(true);
                        Debug.Log($"상점2 구매 성공: {Store2Money} 골드 사용, 체력 회복.");
                    }
                    else
                    {
                        Debug.LogError("Gold: HealthController를 찾을 수 없어 체력 회복을 할 수 없습니다.");
                    }
                }
                else
                {
                    if (NPCBeggar != null) NPCBeggar.SetActive(true);
                    Debug.Log("상점2 구매 실패: 골드 부족.");
                }
            }
        }
        else
        {
            Debug.LogError("Gold: GameManager 인스턴스를 찾을 수 없어 상점 구매를 할 수 없습니다.");
        }
    }

    public void BuyBtn3()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.currentHealth >= GameManager.Instance.maxHealth)
            {
                if (NPCUI3 != null) NPCUI3.SetActive(true);
                Debug.Log("상점3 구매 시도: 이미 최대 체력입니다.");
            }
            else
            {
                if (GameManager.Instance.TrySpendMoney(Store3Money))
                {
                    HealthController playerHealth = GameObject.FindObjectOfType<HealthController>();
                    if (playerHealth != null)
                    {
                        playerHealth.Heal();
                        if (SoldOut3Sprite != null) SoldOut3Sprite.SetActive(true);
                        Debug.Log($"상점3 구매 성공: {Store3Money} 골드 사용, 체력 회복.");
                    }
                    else
                    {
                        Debug.LogError("Gold: HealthController를 찾을 수 없어 체력 회복을 할 수 없습니다.");
                    }
                }
                else
                {
                    if (NPCBeggar != null) NPCBeggar.SetActive(true);
                    Debug.Log("상점3 구매 실패: 골드 부족.");
                }
            }
        }
        else
        {
            Debug.LogError("Gold: GameManager 인스턴스를 찾을 수 없어 상점 구매를 할 수 없습니다.");
        }
    }
}
