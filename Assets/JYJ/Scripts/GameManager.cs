using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private GameObject gameUIContainer;

    private Text UserMoney;

    public int Money { get; private set; }
    public int currentHealth { get; private set; }
    public int maxHealth { get; private set; }

    private GameObject[] hpSprites;

    private const string MainSceneName = "MainScene";
    private const string BossSceneName = "BossScene";

    private void Awake()
    {

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (gameUIContainer == null)
        {
            Transform uiContainerTransform = transform.Find("GameUIContainer");
            if (uiContainerTransform != null)
            {
                gameUIContainer = uiContainerTransform.gameObject;
            }
        }

        if (gameUIContainer != null)
        {
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
            System.Array.Sort(hpSprites, (g1, g2) => string.Compare(g1.name, g2.name));
            maxHealth = hpSprites.Length;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (gameUIContainer == null)
        {
            return;
        }
        if (scene.name == MainSceneName || scene.name == BossSceneName)
        {
            gameUIContainer.SetActive(true);

            AssignUIReferences();
            UpdateGoldText();
            UpdateHpSpritesVisual();

        }
        else
        {
            gameUIContainer.SetActive(false);
            UserMoney = null;
        }
    }
    private void AssignUIReferences()
    {
        if (gameUIContainer != null)
        {
            Transform moneyTextTransform = gameUIContainer.transform.Find("MoneyText");
            if (moneyTextTransform != null)
            {
                UserMoney = moneyTextTransform.GetComponent<Text>();
            }
            else
            {
                UserMoney = null;
            }
        }
    }
    public void InitializeNewGame()
    {
        Money = 1000;
        currentHealth = maxHealth;
    }

    public GameObject[] GetHpSprites()
    {
        if (hpSprites == null || hpSprites.Length == 0)
        {
            Debug.LogWarning("GameManager: GetHpSprites() 호출 시 체력 이미지가 비어 있습니다.");
        }
        return hpSprites;
    }

    public void UpdateGoldText()
    {
        if (UserMoney != null)
        {
            UserMoney.text = String.Format("{0:#,###}", Money);

            if (!UserMoney.gameObject.activeSelf && gameUIContainer != null && gameUIContainer.activeSelf)
            {
                UserMoney.gameObject.SetActive(true);
            }        
        }
    }

    public void UpdateHpSpritesVisual()
    {
        if (hpSprites == null || hpSprites.Length == 0)
        {
            return;
        }
        foreach (GameObject hpSprite in hpSprites)
        {
            if (hpSprite != null)
            {
                hpSprite.SetActive(true);
            }
        }

        for (int i = 0; i < hpSprites.Length; i++)
        {
            if (hpSprites[i] != null)
            {
                hpSprites[i].SetActive(i < currentHealth);
            }
        }
    }

    public void AddMoney(int amount)
    {
        Money += amount;
        UpdateGoldText();
    }

    public bool TrySpendMoney(int amount)
    {
        if (Money >= amount)
        {
            Money -= amount;
            UpdateGoldText();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DecreaseHealth(int amount = 1)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        UpdateHpSpritesVisual();
    }
    public void IncreaseHealth(int amount = 1)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHpSpritesVisual();
    }

    public void GameOver(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void BossClear(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
