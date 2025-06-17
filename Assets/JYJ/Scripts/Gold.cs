using UnityEngine;

public class Gold : MonoBehaviour
{

    SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject MoneySprite;
    [SerializeField] private GameObject SoldOut1Sprite;
    [SerializeField] private GameObject SoldOut2Sprite;
    [SerializeField] private GameObject SoldOut3Sprite;

    public int Store1Money; 
    public int Store2Money; 
    public int Store3Money; 

    public GameObject NPCUI3; 
    public GameObject NPCBeggar;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 0.3f;
                spriteRenderer.color = color;
            }

            Collider2D col = GetComponent<Collider2D>();
            
            if (col != null) col.enabled = false;

            GainMoney();
            Invoke("MoneyBye", 1f);
        }
    }

    public void MoneyBye()
    {
        if (MoneySprite != null)
        {
            MoneySprite.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void GainMoney()
    {
        if (GameManager.Instance != null)
        {
            int amountToGain = UnityEngine.Random.Range(22222, 44444);
            GameManager.Instance.AddMoney(amountToGain);
        }
    }

    public void BuyBtn1()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.currentHealth >= GameManager.Instance.maxHealth)
            {
                if (NPCUI3 != null) NPCUI3.SetActive(true);
            }
            else
            {
                if (GameManager.Instance.TrySpendMoney(Store1Money))
                {
                    HealthController playerHealth = GameObject.FindObjectOfType<HealthController>();
                    if (playerHealth != null)
                    {
                        playerHealth.Heal();
                        if (SoldOut1Sprite != null) SoldOut1Sprite.SetActive(true);
                    }
                }
                else
                {
                    if (NPCBeggar != null) NPCBeggar.SetActive(true);
                }
            }
        }
    }

    public void BuyBtn2()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.currentHealth >= GameManager.Instance.maxHealth)
            {
                if (NPCUI3 != null) NPCUI3.SetActive(true);
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
                    }
                }
                else
                {
                    if (NPCBeggar != null) NPCBeggar.SetActive(true);
                }
            }
        }
    }

    public void BuyBtn3()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.currentHealth >= GameManager.Instance.maxHealth)
            {
                if (NPCUI3 != null) NPCUI3.SetActive(true);
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
                    }
                }
                else
                {
                    if (NPCBeggar != null) NPCBeggar.SetActive(true);
                }
            }
        }
    }
}
