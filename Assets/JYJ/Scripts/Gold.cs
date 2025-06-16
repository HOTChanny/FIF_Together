using System;
using System.Collections;
using System.Collections.Generic;
// using Unity.VisualScripting; // �� using ���ù��� �ʿ����� Ȯ���ϼ���.
using UnityEngine;
using UnityEngine.SceneManagement; // SceneManager ��� �� �ʿ�

public class Gold : MonoBehaviour
{
    // float currentPositionX; // ������ �ʴ� ���� ������ �ʿ� ������ �����ϼ���.

    SpriteRenderer spriteRenderer; // ��� ������ ��ü�� ��������Ʈ ������
    // HealthController health; // ���� GameManager�� ���� ü�� ȸ���� ó���� �� �����Ƿ� �ʿ� ���� �� �ֽ��ϴ�.
    [SerializeField] private GameObject MoneySprite; // �� ������Ʈ�� ��� ������ ��ü�� �ǹ��ϴ��� Ȯ���ϼ���.
    [SerializeField] private GameObject SoldOut1Sprite; // ���� ���� �Ϸ� ǥ�� ��������Ʈ
    [SerializeField] private GameObject SoldOut2Sprite;
    [SerializeField] private GameObject SoldOut3Sprite;

    public int Store1Money; // ����1 ����
    public int Store2Money; // ����2 ����
    public int Store3Money; // ����3 ����

    public GameObject NPCUI3; // ü�� ������ �� �ߴ� UI?
    public GameObject NPCBeggar; // �� ������ �� �ߴ� UI?

    private void Start()
    {
        // �� ��ũ��Ʈ�� �پ��ִ� ������Ʈ�� SpriteRenderer�� �����ɴϴ�. (��� ������ ��ü)
        spriteRenderer = GetComponent<SpriteRenderer>(); // GetComponentsInChildren ��� GetComponent ���
        // health = GameObject.Find("Player").GetComponent<HealthController>(); // ���� GameManager�� ���� ü�� ȸ���� ó���� �� �����Ƿ� �ʿ� ���� �� �ֽ��ϴ�.

        // MoneySprite�� �� ��ũ��Ʈ�� ���� ������Ʈ ��ü��� MoneySprite �ʵ�� �ʿ� ���� �� �ֽ��ϴ�.
        // ���� MoneySprite�� ������ �ڽ� ������Ʈ��� GetComponentInChildren<SpriteRenderer>()�� ����ؾ� �մϴ�.
        // ���� �ڵ忡���� MoneySprite.SetActive(false)�� ȣ���ϹǷ� MoneySprite �ʵ尡 �ʿ��� ���Դϴ�.
        // MoneySprite�� �� ��ũ ��ũ��Ʈ�� ���� ������Ʈ ��ü��� MoneyBye �Լ��� transform.gameObject.SetActive(false)�� �����ؾ� �մϴ�.
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // �浹�� ������Ʈ�� �±װ� "Player"���� Ȯ���մϴ�.
        if (collision.gameObject.CompareTag("Player")) // == "Player" ��� CompareTag ��� ����
        {
            // ��� �������� �ð���/������ ��Ҹ� ��Ȱ��ȭ�Ͽ� �� ���� ȹ��ǵ��� �մϴ�.
            // ��������Ʈ ���� ����
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 0.3f;
                spriteRenderer.color = color;
            }
            // Collider �� ������Ʈ ��Ȱ��ȭ
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
            // Rigidbody �� �ٸ� Behaviour ������Ʈ�� ��Ȱ��ȭ�� �� �ֽ��ϴ�.
            // foreach (Behaviour component in transform.GetComponentsInChildren<Behaviour>())
            // {
            //     component.enabled = false;
            // } // �� �ڵ�� �ڽ� ������Ʈ�� ��� Behaviour�� ���Ƿ� �����ؼ� ����ؾ� �մϴ�.

            GainMoney(); // ��� ȹ�� �Լ� ȣ��
            Invoke("MoneyBye", 1f); // 1�� �� MoneyBye �Լ� ȣ�� (������Ʈ ��Ȱ��ȭ ��)
        }
    }

    public void MoneyBye()
    {
        // ��� ������ ������Ʈ ��ü�� ��Ȱ��ȭ�մϴ�.
        // MoneySprite �ʵ尡 �� ������Ʈ ��ü�� ����Ų�ٸ� �̴�� ����ϰų� transform.gameObject.SetActive(false)�� ����
        if (MoneySprite != null)
        {
            MoneySprite.SetActive(false);
        }
        else
        {
            // MoneySprite �ʵ尡 �������� �ʾҴٸ� �� ��ũ��Ʈ�� ���� ������Ʈ�� ��Ȱ��ȭ
            gameObject.SetActive(false);
        }
    }

    // private int currentValue; // GameManager���� ��� ���� �����ϹǷ� �ʿ� �����ϴ�.
    public void GainMoney()
    {
        if (GameManager.Instance != null)
        {
            // GameManager�� Money ���� ���� ���� ���մϴ�.
            int amountToGain = UnityEngine.Random.Range(22222, 44444);
            GameManager.Instance.AddMoney(amountToGain); // GameManager�� AddMoney �Լ� ���
            // GameManager.Instance.UserMoney.text ������Ʈ�� AddMoney �Լ� �ȿ��� ó���˴ϴ�.
        }
        else
        {
            Debug.LogError("Gold: GameManager �ν��Ͻ��� ã�� �� ���� ��带 ȹ���� �� �����ϴ�.");
        }
    }

    // ���� ���� �Լ���
    public void BuyBtn1()
    {
        if (GameManager.Instance != null)
        {
            // ü�� ���� üũ�� HealthController�� �ƴ� GameManager�� currentHealth ���
            if (GameManager.Instance.currentHealth >= GameManager.Instance.maxHealth)
            {
                if (NPCUI3 != null) NPCUI3.SetActive(true); // ü�� ���� UI ǥ��
                Debug.Log("����1 ���� �õ�: �̹� �ִ� ü���Դϴ�.");
            }
            else
            {
                // GameManager�� TrySpendMoney �Լ��� ����Ͽ� ���� ����ϰ� ���� ���� Ȯ��
                if (GameManager.Instance.TrySpendMoney(Store1Money))
                {
                    // �� ��� ���� �� ü�� ȸ��
                    // HealthController�� Heal �Լ� ȣ�� (HealthController�� GameManager�� ü�� ���� ������Ʈ)
                    HealthController playerHealth = GameObject.FindObjectOfType<HealthController>(); // ������ HealthController ã��
                    if (playerHealth != null)
                    {
                        playerHealth.Heal();
                        if (SoldOut1Sprite != null) SoldOut1Sprite.SetActive(true); // �Ǹ� �Ϸ� ǥ��
                        Debug.Log($"����1 ���� ����: {Store1Money} ��� ���, ü�� ȸ��.");
                    }
                    else
                    {
                        Debug.LogError("Gold: HealthController�� ã�� �� ���� ü�� ȸ���� �� �� �����ϴ�.");
                    }
                }
                else
                {
                    // �� ��� ���� �� (��� ����)
                    if (NPCBeggar != null) NPCBeggar.SetActive(true); // �� ���� UI ǥ��
                    Debug.Log("����1 ���� ����: ��� ����.");
                }
            }
        }
        else
        {
            Debug.LogError("Gold: GameManager �ν��Ͻ��� ã�� �� ���� ���� ���Ÿ� �� �� �����ϴ�.");
        }
    }

    public void BuyBtn2()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.currentHealth >= GameManager.Instance.maxHealth)
            {
                if (NPCUI3 != null) NPCUI3.SetActive(true);
                Debug.Log("����2 ���� �õ�: �̹� �ִ� ü���Դϴ�.");
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
                        Debug.Log($"����2 ���� ����: {Store2Money} ��� ���, ü�� ȸ��.");
                    }
                    else
                    {
                        Debug.LogError("Gold: HealthController�� ã�� �� ���� ü�� ȸ���� �� �� �����ϴ�.");
                    }
                }
                else
                {
                    if (NPCBeggar != null) NPCBeggar.SetActive(true);
                    Debug.Log("����2 ���� ����: ��� ����.");
                }
            }
        }
        else
        {
            Debug.LogError("Gold: GameManager �ν��Ͻ��� ã�� �� ���� ���� ���Ÿ� �� �� �����ϴ�.");
        }
    }

    public void BuyBtn3()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.currentHealth >= GameManager.Instance.maxHealth)
            {
                if (NPCUI3 != null) NPCUI3.SetActive(true);
                Debug.Log("����3 ���� �õ�: �̹� �ִ� ü���Դϴ�.");
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
                        Debug.Log($"����3 ���� ����: {Store3Money} ��� ���, ü�� ȸ��.");
                    }
                    else
                    {
                        Debug.LogError("Gold: HealthController�� ã�� �� ���� ü�� ȸ���� �� �� �����ϴ�.");
                    }
                }
                else
                {
                    if (NPCBeggar != null) NPCBeggar.SetActive(true);
                    Debug.Log("����3 ���� ����: ��� ����.");
                }
            }
        }
        else
        {
            Debug.LogError("Gold: GameManager �ν��Ͻ��� ã�� �� ���� ���� ���Ÿ� �� �� �����ϴ�.");
        }
    }
}
