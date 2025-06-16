using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI ���� ������Ʈ ����� ���� �ʿ�
using UnityEngine.SceneManagement; // �� ���� ����� ���� �ʿ�

public class GameManager : MonoBehaviour
{
    // GameManager�� �̱��� �ν��Ͻ�
    public static GameManager Instance;

    // ü�� �̹���, ��� �ؽ�Ʈ ���� �����ϴ� UI �θ� ������Ʈ
    // �� ������Ʈ�� GameManager�� �ڽ����� ��ġ�ϰ� Inspector���� �������ּ���.
    [SerializeField] private GameObject gameUIContainer;

    // MainScene �Ǵ� BossScene �ε� �� ã�Ƽ� ������ ��� �ؽ�Ʈ
    // �� ������ �� �ε� �ø��� �ٽ� �Ҵ�˴ϴ�.
    private Text UserMoney;

    // ���� ���� ���� (GameManager�� ���� ���� ����)
    public int Money { get; private set; } // ��� �� (�ܺο��� �б⸸ ����)
    public int currentHealth { get; private set; } // ���� ü�� (�ܺο��� �б⸸ ����)
    public int maxHealth { get; private set; } // �ִ� ü�� (�ܺο��� �б⸸ ����)

    // HealthController���� ������ ü�� �̹��� �迭
    // �� �迭�� Awake���� �� �� ã�Ƽ� ������ �Ӵϴ�.
    private GameObject[] hpSprites;

    // �� �̸� ��� (���� �� �̸����� �������ּ���)
    private const string MainSceneName = "MainScene";
    private const string BossSceneName = "BossScene";
    private const string StartSceneName = "StartScene";
    private const string NormalEndSceneName = "NormalEnd";
    private const string WorstEndSceneName = "WorstEndScene";
    private const string BestEndSceneName = "BestEndScene"; // ���� Ŭ���� �� �̵��ϴ� �� �̸�

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance != null)
        {
            // �̹� GameManager �ν��Ͻ��� ������ ���� ������ ������Ʈ�� �ı��մϴ�.
            Debug.Log($"GameManager: �ߺ� �ν��Ͻ� �߰�, �ı��մϴ�. (�� ������Ʈ: {gameObject.name}, ���� ������Ʈ: {Instance.gameObject.name})");
            Destroy(gameObject);
            return;
        }
        // �� ������Ʈ�� GameManager �ν��Ͻ��� �����ϰ� �� ��ȯ �� �ı����� �ʵ��� �մϴ�.
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"GameManager: �ν��Ͻ� ���� �� DontDestroyOnLoad ���� �Ϸ�. ������Ʈ �̸�: {gameObject.name}");

        // GameManager ������Ʈ�� �ڽ� �߿��� gameUIContainer�� ã���ϴ�.
        // Inspector���� ������� �ʾҴٸ� �̸����� ã�ƺ��ϴ�.
        if (gameUIContainer == null)
        {
            Transform uiContainerTransform = transform.Find("GameUIContainer"); // ���� �̸�, ���� �̸����� ����
            if (uiContainerTransform != null)
            {
                gameUIContainer = uiContainerTransform.gameObject;
                Debug.Log($"GameManager: �ڽĿ��� 'GameUIContainer' ������Ʈ ã��.");
            }
            else
            {
                Debug.LogError("GameManager: 'gameUIContainer' ������Ʈ�� Inspector���� ������� �ʾҰų� �ڽ����� �����ϴ�. �̸� Ȯ���� �ʿ��մϴ�.");
            }
        }
        else
        {
            Debug.Log($"GameManager: 'gameUIContainer' ������Ʈ�� Inspector���� ����Ǿ����ϴ�.");
        }


        // gameUIContainer�� �ڽ� �߿��� "Life" �±׸� ���� ������Ʈ���� ã�Ƽ� hpSprites �迭�� �����մϴ�.
        // �� �۾��� Awake���� �� ���� �����Ͽ� �迭�� �̸� �غ��� �Ӵϴ�.
        if (gameUIContainer != null)
        {
            // GetComponentsInChildren<Transform>(true)�� ����Ͽ� ��Ȱ��ȭ�� �ڽĵ� ã���ϴ�.
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

            // ü�� �̹��� ������ �߿��ϹǷ� �̸� �������� �����մϴ�. ("HP1", "HP2", "HP3" ����)
            System.Array.Sort(hpSprites, (g1, g2) => string.Compare(g1.name, g2.name));

            // �ִ� ü�� ����
            maxHealth = hpSprites.Length;
            // currentHealth�� Money�� InitializeNewGame() �Ǵ� �� �ε� �� �����˴ϴ�.

            Debug.Log($"GameManager: ü�� �̹��� {hpSprites.Length}�� ã��. �ִ� ü��: {maxHealth}");
        }
        else
        {
            Debug.LogError("GameManager: gameUIContainer�� �������� �ʾ� ü�� �̹����� ã�� �� �����ϴ�.");
        }

        // ���� ���� �� �ʱ�ȭ (Ÿ��Ʋ ������ ������ ���� ȣ��ǵ��� SceneChanger���� ȣ��)
        // InitializeNewGame(); // Awake���� ���� ȣ������ �ʰ� SceneChanger���� ȣ���ϵ��� ����
    }

    // ������Ʈ�� Ȱ��ȭ�� �� �� �ε� �̺�Ʈ ����
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("GameManager: SceneManager.sceneLoaded �̺�Ʈ ���� ����");
    }

    // ������Ʈ�� ��Ȱ��ȭ�� �� �� �ε� �̺�Ʈ ���� ����
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("GameManager: SceneManager.sceneLoaded �̺�Ʈ ���� ����");
    }

    // ���� �ε�� �� ȣ��Ǵ� �Լ�
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"GameManager: �� �ε�� - {scene.name}");

        if (gameUIContainer == null)
        {
            Debug.LogError("GameManager: gameUIContainer�� �������� �ʾ� �� �ε� �� UI�� ������ �� �����ϴ�.");
            return;
        }

        // �ε�� �� �̸��� ���� UI �����̳��� Ȱ��ȭ ���¸� �����մϴ�.
        if (scene.name == MainSceneName || scene.name == BossSceneName)
        {
            // ���� �� �Ǵ� ���� �������� UI �����̳ʸ� Ȱ��ȭ�մϴ�.
            gameUIContainer.SetActive(true);
            Debug.Log($"GameManager: {scene.name} �ε�, UI �����̳� Ȱ��ȭ.");

            // *** �߿�: �� �ε� �ø��� UI ��ҵ��� �ٽ� ã�Ƽ� �����մϴ�. ***
            AssignUIReferences();

            // UI�� ���� ���� ���¿� �°� ������Ʈ�մϴ�.
            UpdateGoldText();
            UpdateHpSpritesVisual();

            Debug.Log($"GameManager: {scene.name} �ε� �� UI ������Ʈ �Ϸ�. ���� ���: {Money}, ���� ü��: {currentHealth}");
        }
        else // Ÿ��Ʋ, ���� ����, ���� Ŭ���� �� ��
        {
            // �� �� �������� UI �����̳ʸ� ��Ȱ��ȭ�Ͽ� ����ϴ�.
            gameUIContainer.SetActive(false);
            Debug.Log($"GameManager: {scene.name} �ε�, UI �����̳� ��Ȱ��ȭ.");

            // UI ������ �����մϴ�.
            UserMoney = null;
            // hpSprites �迭 ��ü�� DontDestroyOnLoad�� gameUIContainer�� �ڽ��̹Ƿ� ������ �����մϴ�.
        }
    }

    // ���� ������ �ʿ��� UI ��ҵ��� ã�Ƽ� �����ϴ� �Լ�
    // �� �Լ��� MainScene �Ǵ� BossScene �ε� �� OnSceneLoaded���� ȣ��˴ϴ�.
    private void AssignUIReferences()
    {
        if (gameUIContainer != null)
        {
            // ��: gameUIContainer�� �ڽ� �߿��� "MoneyText" �̸��� Text ������Ʈ�� ã���ϴ�.
            // ���� ������Ʈ ������ �°� �������ּ���.
            Transform moneyTextTransform = gameUIContainer.transform.Find("MoneyText"); // ��� �ؽ�Ʈ ������Ʈ �̸� Ȯ��
            if (moneyTextTransform != null)
            {
                UserMoney = moneyTextTransform.GetComponent<Text>();
                if (UserMoney != null)
                {
                    Debug.Log($"GameManager: 'MoneyText' ������Ʈ���� Text ������Ʈ ã��.");
                }
                else
                {
                    Debug.LogWarning($"GameManager: '{moneyTextTransform.name}' ������Ʈ���� Text ������Ʈ�� ã�� �� �����ϴ�.");
                }
            }
            else
            {
                Debug.LogWarning("GameManager: 'MoneyText' ������Ʈ�� ã�� �� �����ϴ�. �̸� Ȯ���� �ʿ��մϴ�.");
                UserMoney = null; // ã�� �������� null�� ����
            }

            // ü�� �̹��� �迭�� Awake���� �̹� ã�Ƽ� ������ �ξ����Ƿ� ���⼭ �ٽ� ã�� �ʿ�� �����ϴ�.
            // ���� ������ ü�� �̹��� ������Ʈ�� �ٸ��ٸ� ���⼭ �ٽ� ã�ƾ� �մϴ�.
            // ���� ���������� DontDestroyOnLoad�� gameUIContainer �Ʒ��� ü�� �̹����� �ִٰ� �����մϴ�.
            if (hpSprites == null || hpSprites.Length == 0)
            {
                Debug.LogWarning("GameManager: hpSprites �迭�� ��� �ֽ��ϴ�. Awake���� ü�� �̹����� ����� ã�Ҵ��� Ȯ���ϼ���.");
            }
        }
        else
        {
            Debug.LogError("GameManager: gameUIContainer�� �������� �ʾ� UI ������ �Ҵ��� �� �����ϴ�.");
        }
    }

    // ���ο� ���� ���� �� ȣ��Ǿ� ���� ���¸� �ʱ�ȭ�ϴ� �Լ�
    // �� �Լ��� Ÿ��Ʋ ������ ���� ������ ��ȯ�ϴ� SceneChanger ��ũ��Ʈ���� ȣ���ؾ� �մϴ�.
    public void InitializeNewGame()
    {
        Money = 1000; // �ʱ� ��� ��
        currentHealth = maxHealth; // ü�� �ִ�ġ�� �ʱ�ȭ
        Debug.Log($"GameManager: ���ο� ���� �ʱ�ȭ �Ϸ�. �ʱ� ���: {Money}, �ʱ� ü��: {currentHealth}");

        // UI ������Ʈ�� MainScene �ε� �� OnSceneLoaded���� ó���˴ϴ�.
        // ������ �ʱ�ȭ ���� UI�� �ٷ� ������Ʈ�ϰ� �ʹٸ� ���⼭ ȣ���� ���� �ֽ��ϴ�.
        // UpdateGoldText();
        // UpdateHpSpritesVisual();
    }

    // HealthController�� ü�� �̹��� �迭�� ������ �� �ֵ��� �ϴ� �Լ�
    public GameObject[] GetHpSprites()
    {
        if (hpSprites == null || hpSprites.Length == 0)
        {
            Debug.LogWarning("GameManager: GetHpSprites() ȣ�� �� ü�� �̹����� ��� �ֽ��ϴ�.");
        }
        return hpSprites;
    }

    // ���� ��� ���� ���� ��� �ؽ�Ʈ�� ������Ʈ�ϴ� �Լ�
    public void UpdateGoldText()
    {
        if (UserMoney != null)
        {
            UserMoney.text = String.Format("{0:#,###}", Money);
            // ��� �ؽ�Ʈ ������Ʈ ��ü�� Ȱ��ȭ�Ǿ� �ִ��� Ȯ�� (OnSceneLoaded���� ó����)
            if (!UserMoney.gameObject.activeSelf && gameUIContainer != null && gameUIContainer.activeSelf)
            {
                UserMoney.gameObject.SetActive(true);
                Debug.Log("GameManager: UserMoney Text ������Ʈ Ȱ��ȭ��.");
            }
            Debug.Log($"GameManager: ��� �ؽ�Ʈ ������Ʈ �Ϸ�. ���� ���: {Money}");
        }
        else
        {
            // Debug.LogWarning("GameManager: UserMoney Text�� ������� �ʾ� ��� �ؽ�Ʈ�� ������Ʈ�� �� �����ϴ�.");
            // �� ���� UI�� ��Ȱ��ȭ�� �������� ������ �� �ֽ��ϴ�.
        }
    }

    // ���� ü�� ���� ���� ü�� �̹����� Ȱ��ȭ ���¸� ������Ʈ�ϴ� �Լ�
    public void UpdateHpSpritesVisual()
    {
        if (hpSprites == null || hpSprites.Length == 0)
        {
            Debug.LogWarning("GameManager: UpdateHpSpritesVisual() ȣ�� �� ü�� �̹����� ��� �ֽ��ϴ�.");
            return;
        }

        // ��� ü�� �̹����� �ϴ� Ȱ��ȭ�մϴ�. (Ȥ�� �� ��Ȱ��ȭ ���� ����)
        foreach (GameObject hpSprite in hpSprites)
        {
            if (hpSprite != null)
            {
                hpSprite.SetActive(true);
            }
        }

        // ���� ü�� ������ ���� �ε����� ü�� �̹����� Ȱ��ȭ ���·� �����ϰ�,
        // ���� ü�� ������ �������� ��Ȱ��ȭ�մϴ�.
        // ��: maxHealth=3, currentHealth=2 �̸� hpSprites[0], hpSprites[1]�� Ȱ��ȭ, hpSprites[2] ��Ȱ��ȭ
        for (int i = 0; i < hpSprites.Length; i++)
        {
            if (hpSprites[i] != null)
            {
                hpSprites[i].SetActive(i < currentHealth);
            }
        }
        Debug.Log($"GameManager: ü�� �̹��� �ð��� ���� ������Ʈ �Ϸ�. ���� ü��: {currentHealth}");
    }

    // �� �߰� �Լ� (Gold ��ũ��Ʈ���� ȣ��)
    public void AddMoney(int amount)
    {
        Money += amount;
        UpdateGoldText(); // ��� �� ���� �� UI ������Ʈ
        Debug.Log($"GameManager: ��� �߰���. ���� ���: {Money}");
    }

    // �� ���� �Լ� (��: ���� ���� - Gold ��ũ��Ʈ���� ȣ��)
    public bool TrySpendMoney(int amount)
    {
        if (Money >= amount)
        {
            Money -= amount;
            UpdateGoldText(); // ��� �� ���� �� UI ������Ʈ
            Debug.Log($"GameManager: ��� ����. ���� ���: {Money}");
            return true; // ���� ����
        }
        else
        {
            Debug.Log($"GameManager: ��� ����. �ʿ�: {amount}, ����: {Money}");
            return false; // ���� ����
        }
    }

    // ü�� ���� �Լ� (HealthController���� ȣ��)
    public void DecreaseHealth(int amount = 1)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0); // ü�� ���� (0 �̸����� �������� �ʵ���)
        UpdateHpSpritesVisual(); // ü�� ���� �� UI ������Ʈ
        Debug.Log($"GameManager: ü�� ���ҵ�. ���� ü��: {currentHealth}");

        // ü���� 0 ���ϰ� �Ǹ� ���� ���� ó�� (HealthController���� �� ��ȯ ���)
        // �� ��ȯ ������ HealthController�� Hurt �Լ����� currentHealth <= 0 üũ �� ȣ��
    }

    // ü�� ȸ�� �Լ� (HealthController �Ǵ� Gold ��ũ��Ʈ���� ȣ��)
    public void IncreaseHealth(int amount = 1)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // ü�� ȸ�� (�ִ� ü�� �ʰ����� �ʵ���)
        UpdateHpSpritesVisual(); // ü�� ���� �� UI ������Ʈ
        Debug.Log($"GameManager: ü�� ȸ����. ���� ü��: {currentHealth}");
    }

    // ���� ���� �� ��ȯ �Լ� (HealthController���� ȣ��)
    public void GameOver(string sceneToLoad)
    {
        Debug.Log($"GameManager: ���� ����! �� ��ȯ -> {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
    }

    // ���� Ŭ���� �� ��ȯ �Լ� (���� ��ũ��Ʈ ��� ȣ��)
    public void BossClear(string sceneToLoad)
    {
        Debug.Log($"GameManager: ���� Ŭ����! �� ��ȯ -> {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
    }
}
