using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Text UserMoney;
    public int Money;

    public int MaxLife = 3;
    public int currentLife = 3;

    [SerializeField] private GameObject HP1Sprite;
    [SerializeField] private GameObject HP2Sprite;
    [SerializeField] private GameObject HP3Sprite;

    [SerializeField] private GameObject NPCUI3;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        UserMoney.text = String.Format("{0:#,###}", Money);

        if (currentLife == 3)
        {
            HP1Sprite.SetActive(true);
            HP2Sprite.SetActive(true);
            HP3Sprite.SetActive(true);
        }
        if (currentLife == 2)
        {
            HP1Sprite.SetActive(true);
            HP2Sprite.SetActive(true);
            HP3Sprite.SetActive(false);
        }
        if (currentLife == 1)
        {
            HP1Sprite.SetActive(true);
            HP2Sprite.SetActive(false);
            HP3Sprite.SetActive(false);
        }
    }
    
    public void Heal()
    {
        if (currentLife == 3)
        {
            NPCUI3.SetActive(true);
        }
        if (currentLife == 2)
        {
            currentLife = currentLife + 1;
            HP3Sprite.SetActive(true);
        }
        if (currentLife == 1)
        {
            currentLife = currentLife + 1;
            HP2Sprite.SetActive(true);
        }
    }
}
