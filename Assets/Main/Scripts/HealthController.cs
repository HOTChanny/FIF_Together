using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthController : MonoBehaviour
{
    public float cooldownHit;
    private float rateOfHit;
    private GameObject[] life;

    void Start()
    {
        rateOfHit = Time.time;

        if (GameManager.Instance != null)
        {
            life = GameManager.Instance.GetHpSprites();
            if (life != null && life.Length > 0)
            {
                GameManager.Instance.UpdateHpSpritesVisual();
            }
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
                if (GameManager.Instance.currentHealth > 0)
                {
                    GameManager.Instance.DecreaseHealth(1);

                    if (GameManager.Instance.currentHealth <= 0)
                    {
                        string currentSceneName = SceneManager.GetActiveScene().name;

                        string bossSceneName = "BossScene";
                        string normalEndSceneName = "NormalEnd";
                        string worstEndSceneName = "WorstEndScene";

                        if (currentSceneName == bossSceneName)
                        {
                            GameManager.Instance.GameOver(worstEndSceneName);
                        }
                        else
                        {
                            GameManager.Instance.GameOver(normalEndSceneName);
                        }
                    }
                }
            }
        }
    }

    public void Heal()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.currentHealth < GameManager.Instance.maxHealth)
        {
            GameManager.Instance.IncreaseHealth(1);
        }
    }
}
