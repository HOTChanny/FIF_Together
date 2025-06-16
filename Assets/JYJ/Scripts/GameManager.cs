using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 컴포넌트 사용을 위해 필요
using UnityEngine.SceneManagement; // 씬 관리 사용을 위해 필요

public class GameManager : MonoBehaviour
{
    // GameManager의 싱글톤 인스턴스
    public static GameManager Instance;

    // 체력 이미지, 골드 텍스트 등을 포함하는 UI 부모 오브젝트
    // 이 오브젝트는 GameManager의 자식으로 배치하고 Inspector에서 연결해주세요.
    [SerializeField] private GameObject gameUIContainer;

    // MainScene 또는 BossScene 로드 시 찾아서 연결할 골드 텍스트
    // 이 변수는 씬 로드 시마다 다시 할당됩니다.
    private Text UserMoney;

    // 게임 상태 변수 (GameManager가 실제 값을 관리)
    public int Money { get; private set; } // 골드 값 (외부에서 읽기만 가능)
    public int currentHealth { get; private set; } // 현재 체력 (외부에서 읽기만 가능)
    public int maxHealth { get; private set; } // 최대 체력 (외부에서 읽기만 가능)

    // HealthController에게 전달할 체력 이미지 배열
    // 이 배열은 Awake에서 한 번 찾아서 저장해 둡니다.
    private GameObject[] hpSprites;

    // 씬 이름 상수 (실제 씬 이름으로 변경해주세요)
    private const string MainSceneName = "MainScene";
    private const string BossSceneName = "BossScene";
    private const string StartSceneName = "StartScene";
    private const string NormalEndSceneName = "NormalEnd";
    private const string WorstEndSceneName = "WorstEndScene";
    private const string BestEndSceneName = "BestEndScene"; // 보스 클리어 후 이동하는 씬 이름

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance != null)
        {
            // 이미 GameManager 인스턴스가 있으면 새로 생성된 오브젝트는 파괴합니다.
            Debug.Log($"GameManager: 중복 인스턴스 발견, 파괴합니다. (새 오브젝트: {gameObject.name}, 기존 오브젝트: {Instance.gameObject.name})");
            Destroy(gameObject);
            return;
        }
        // 이 오브젝트를 GameManager 인스턴스로 설정하고 씬 전환 시 파괴되지 않도록 합니다.
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"GameManager: 인스턴스 생성 및 DontDestroyOnLoad 설정 완료. 오브젝트 이름: {gameObject.name}");

        // GameManager 오브젝트의 자식 중에서 gameUIContainer를 찾습니다.
        // Inspector에서 연결되지 않았다면 이름으로 찾아봅니다.
        if (gameUIContainer == null)
        {
            Transform uiContainerTransform = transform.Find("GameUIContainer"); // 예시 이름, 실제 이름으로 변경
            if (uiContainerTransform != null)
            {
                gameUIContainer = uiContainerTransform.gameObject;
                Debug.Log($"GameManager: 자식에서 'GameUIContainer' 오브젝트 찾음.");
            }
            else
            {
                Debug.LogError("GameManager: 'gameUIContainer' 오브젝트가 Inspector에서 연결되지 않았거나 자식으로 없습니다. 이름 확인이 필요합니다.");
            }
        }
        else
        {
            Debug.Log($"GameManager: 'gameUIContainer' 오브젝트가 Inspector에서 연결되었습니다.");
        }


        // gameUIContainer의 자식 중에서 "Life" 태그를 가진 오브젝트들을 찾아서 hpSprites 배열에 저장합니다.
        // 이 작업은 Awake에서 한 번만 수행하여 배열을 미리 준비해 둡니다.
        if (gameUIContainer != null)
        {
            // GetComponentsInChildren<Transform>(true)를 사용하여 비활성화된 자식도 찾습니다.
            Transform[] children = gameUIContainer.GetComponentsInChildren<Transform>(true);
            List<GameObject> lifeList = new List<GameObject>();
            foreach (Transform child in children)
            {
                if (child.CompareTag("Life"))
                {
                    lifeList.Add(child.gameObject);
                }
            }
            hpSprites = lifeList.ToArray();

            // 체력 이미지 순서가 중요하므로 이름 기준으로 정렬합니다. ("HP1", "HP2", "HP3" 순서)
            System.Array.Sort(hpSprites, (g1, g2) => string.Compare(g1.name, g2.name));

            // 최대 체력 설정
            maxHealth = hpSprites.Length;
            // currentHealth와 Money는 InitializeNewGame() 또는 씬 로드 시 설정됩니다.

            Debug.Log($"GameManager: 체력 이미지 {hpSprites.Length}개 찾음. 최대 체력: {maxHealth}");
        }
        else
        {
            Debug.LogError("GameManager: gameUIContainer가 설정되지 않아 체력 이미지를 찾을 수 없습니다.");
        }

        // 게임 시작 시 초기화 (타이틀 씬에서 시작할 때만 호출되도록 SceneChanger에서 호출)
        // InitializeNewGame(); // Awake에서 직접 호출하지 않고 SceneChanger에서 호출하도록 변경
    }

    // 오브젝트가 활성화될 때 씬 로드 이벤트 구독
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("GameManager: SceneManager.sceneLoaded 이벤트 구독 시작");
    }

    // 오브젝트가 비활성화될 때 씬 로드 이벤트 구독 해제
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("GameManager: SceneManager.sceneLoaded 이벤트 구독 해제");
    }

    // 씬이 로드될 때 호출되는 함수
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"GameManager: 씬 로드됨 - {scene.name}");

        if (gameUIContainer == null)
        {
            Debug.LogError("GameManager: gameUIContainer가 설정되지 않아 씬 로드 시 UI를 제어할 수 없습니다.");
            return;
        }

        // 로드된 씬 이름에 따라 UI 컨테이너의 활성화 상태를 설정합니다.
        if (scene.name == MainSceneName || scene.name == BossSceneName)
        {
            // 메인 씬 또는 보스 씬에서는 UI 컨테이너를 활성화합니다.
            gameUIContainer.SetActive(true);
            Debug.Log($"GameManager: {scene.name} 로드, UI 컨테이너 활성화.");

            // *** 중요: 씬 로드 시마다 UI 요소들을 다시 찾아서 연결합니다. ***
            AssignUIReferences();

            // UI를 현재 게임 상태에 맞게 업데이트합니다.
            UpdateGoldText();
            UpdateHpSpritesVisual();

            Debug.Log($"GameManager: {scene.name} 로드 후 UI 업데이트 완료. 현재 골드: {Money}, 현재 체력: {currentHealth}");
        }
        else // 타이틀, 게임 오버, 보스 클리어 씬 등
        {
            // 그 외 씬에서는 UI 컨테이너를 비활성화하여 숨깁니다.
            gameUIContainer.SetActive(false);
            Debug.Log($"GameManager: {scene.name} 로드, UI 컨테이너 비활성화.");

            // UI 참조를 해제합니다.
            UserMoney = null;
            // hpSprites 배열 자체는 DontDestroyOnLoad된 gameUIContainer의 자식이므로 참조를 유지합니다.
        }
    }

    // 현재 씬에서 필요한 UI 요소들을 찾아서 연결하는 함수
    // 이 함수는 MainScene 또는 BossScene 로드 시 OnSceneLoaded에서 호출됩니다.
    private void AssignUIReferences()
    {
        if (gameUIContainer != null)
        {
            // 예: gameUIContainer의 자식 중에서 "MoneyText" 이름의 Text 컴포넌트를 찾습니다.
            // 실제 프로젝트 구조에 맞게 수정해주세요.
            Transform moneyTextTransform = gameUIContainer.transform.Find("MoneyText"); // 골드 텍스트 오브젝트 이름 확인
            if (moneyTextTransform != null)
            {
                UserMoney = moneyTextTransform.GetComponent<Text>();
                if (UserMoney != null)
                {
                    Debug.Log($"GameManager: 'MoneyText' 오브젝트에서 Text 컴포넌트 찾음.");
                }
                else
                {
                    Debug.LogWarning($"GameManager: '{moneyTextTransform.name}' 오브젝트에서 Text 컴포넌트를 찾을 수 없습니다.");
                }
            }
            else
            {
                Debug.LogWarning("GameManager: 'MoneyText' 오브젝트를 찾을 수 없습니다. 이름 확인이 필요합니다.");
                UserMoney = null; // 찾지 못했으면 null로 설정
            }

            // 체력 이미지 배열은 Awake에서 이미 찾아서 저장해 두었으므로 여기서 다시 찾을 필요는 없습니다.
            // 만약 씬마다 체력 이미지 오브젝트가 다르다면 여기서 다시 찾아야 합니다.
            // 현재 구조에서는 DontDestroyOnLoad된 gameUIContainer 아래에 체력 이미지가 있다고 가정합니다.
            if (hpSprites == null || hpSprites.Length == 0)
            {
                Debug.LogWarning("GameManager: hpSprites 배열이 비어 있습니다. Awake에서 체력 이미지를 제대로 찾았는지 확인하세요.");
            }
        }
        else
        {
            Debug.LogError("GameManager: gameUIContainer가 설정되지 않아 UI 참조를 할당할 수 없습니다.");
        }
    }

    // 새로운 게임 시작 시 호출되어 게임 상태를 초기화하는 함수
    // 이 함수는 타이틀 씬에서 메인 씬으로 전환하는 SceneChanger 스크립트에서 호출해야 합니다.
    public void InitializeNewGame()
    {
        Money = 1000; // 초기 골드 값
        currentHealth = maxHealth; // 체력 최대치로 초기화
        Debug.Log($"GameManager: 새로운 게임 초기화 완료. 초기 골드: {Money}, 초기 체력: {currentHealth}");

        // UI 업데이트는 MainScene 로드 후 OnSceneLoaded에서 처리됩니다.
        // 하지만 초기화 직후 UI를 바로 업데이트하고 싶다면 여기서 호출할 수도 있습니다.
        // UpdateGoldText();
        // UpdateHpSpritesVisual();
    }

    // HealthController가 체력 이미지 배열을 가져갈 수 있도록 하는 함수
    public GameObject[] GetHpSprites()
    {
        if (hpSprites == null || hpSprites.Length == 0)
        {
            Debug.LogWarning("GameManager: GetHpSprites() 호출 시 체력 이미지가 비어 있습니다.");
        }
        return hpSprites;
    }

    // 현재 골드 값에 따라 골드 텍스트를 업데이트하는 함수
    public void UpdateGoldText()
    {
        if (UserMoney != null)
        {
            UserMoney.text = String.Format("{0:#,###}", Money);
            // 골드 텍스트 오브젝트 자체도 활성화되어 있는지 확인 (OnSceneLoaded에서 처리됨)
            if (!UserMoney.gameObject.activeSelf && gameUIContainer != null && gameUIContainer.activeSelf)
            {
                UserMoney.gameObject.SetActive(true);
                Debug.Log("GameManager: UserMoney Text 오브젝트 활성화됨.");
            }
            Debug.Log($"GameManager: 골드 텍스트 업데이트 완료. 현재 골드: {Money}");
        }
        else
        {
            // Debug.LogWarning("GameManager: UserMoney Text가 연결되지 않아 골드 텍스트를 업데이트할 수 없습니다.");
            // 이 경고는 UI가 비활성화된 씬에서는 정상일 수 있습니다.
        }
    }

    // 현재 체력 값에 따라 체력 이미지의 활성화 상태를 업데이트하는 함수
    public void UpdateHpSpritesVisual()
    {
        if (hpSprites == null || hpSprites.Length == 0)
        {
            Debug.LogWarning("GameManager: UpdateHpSpritesVisual() 호출 시 체력 이미지가 비어 있습니다.");
            return;
        }

        // 모든 체력 이미지를 일단 활성화합니다. (혹시 모를 비활성화 상태 방지)
        foreach (GameObject hpSprite in hpSprites)
        {
            if (hpSprite != null)
            {
                hpSprite.SetActive(true);
            }
        }

        // 현재 체력 값보다 낮은 인덱스의 체력 이미지만 활성화 상태로 유지하고,
        // 현재 체력 값부터 끝까지는 비활성화합니다.
        // 예: maxHealth=3, currentHealth=2 이면 hpSprites[0], hpSprites[1]만 활성화, hpSprites[2] 비활성화
        for (int i = 0; i < hpSprites.Length; i++)
        {
            if (hpSprites[i] != null)
            {
                hpSprites[i].SetActive(i < currentHealth);
            }
        }
        Debug.Log($"GameManager: 체력 이미지 시각적 상태 업데이트 완료. 현재 체력: {currentHealth}");
    }

    // 돈 추가 함수 (Gold 스크립트에서 호출)
    public void AddMoney(int amount)
    {
        Money += amount;
        UpdateGoldText(); // 골드 값 변경 후 UI 업데이트
        Debug.Log($"GameManager: 골드 추가됨. 현재 골드: {Money}");
    }

    // 돈 감소 함수 (예: 상점 구매 - Gold 스크립트에서 호출)
    public bool TrySpendMoney(int amount)
    {
        if (Money >= amount)
        {
            Money -= amount;
            UpdateGoldText(); // 골드 값 변경 후 UI 업데이트
            Debug.Log($"GameManager: 골드 사용됨. 현재 골드: {Money}");
            return true; // 구매 성공
        }
        else
        {
            Debug.Log($"GameManager: 골드 부족. 필요: {amount}, 현재: {Money}");
            return false; // 구매 실패
        }
    }

    // 체력 감소 함수 (HealthController에서 호출)
    public void DecreaseHealth(int amount = 1)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0); // 체력 감소 (0 미만으로 내려가지 않도록)
        UpdateHpSpritesVisual(); // 체력 변경 후 UI 업데이트
        Debug.Log($"GameManager: 체력 감소됨. 현재 체력: {currentHealth}");

        // 체력이 0 이하가 되면 게임 오버 처리 (HealthController에서 씬 전환 담당)
        // 씬 전환 로직은 HealthController의 Hurt 함수에서 currentHealth <= 0 체크 후 호출
    }

    // 체력 회복 함수 (HealthController 또는 Gold 스크립트에서 호출)
    public void IncreaseHealth(int amount = 1)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // 체력 회복 (최대 체력 초과하지 않도록)
        UpdateHpSpritesVisual(); // 체력 변경 후 UI 업데이트
        Debug.Log($"GameManager: 체력 회복됨. 현재 체력: {currentHealth}");
    }

    // 게임 오버 씬 전환 함수 (HealthController에서 호출)
    public void GameOver(string sceneToLoad)
    {
        Debug.Log($"GameManager: 게임 오버! 씬 전환 -> {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
    }

    // 보스 클리어 씬 전환 함수 (보스 스크립트 등에서 호출)
    public void BossClear(string sceneToLoad)
    {
        Debug.Log($"GameManager: 보스 클리어! 씬 전환 -> {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
    }
}
