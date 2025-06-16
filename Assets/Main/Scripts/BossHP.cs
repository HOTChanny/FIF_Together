using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BossHP : MonoBehaviour
{
    float full = 267f;
    float energy = 0.0f;


    Animator anim;
    SpriteRenderer spriteRenderer;
    Rigidbody2D rigid;

    public string sceneName;

    public AudioSource mySfx;
    public AudioClip dieSfx;
    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
    }
    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.CompareTag("Bullet"))
        {
            //�Ҹ� �±׿� ����� �� ������? �ִϸ��̼� ��ȯ(OnDamaged����)
            OnDamaged(coll.transform.position);

            energy += 5f;
            Destroy(coll.gameObject);
            gameObject.transform.Find("Canvas/HPFront").transform.localScale = new Vector3(energy / full, 0.2666667f, 2.4f);

            if(gameObject.transform.Find("Canvas/HPFront").transform.localScale.x >= 0.267f)
            {
                DieSound();
                gameObject.transform.Find("Canvas/HPFront").transform.localScale = new Vector3(0.267f, 0.2666667f, 2.4f);
                //���� �״°� ����
                anim.SetTrigger("Die");
                
                Invoke("BossDie", 1f);
            }


        }
    }
    void OnDamaged(Vector2 targetPos) // ������ �¾��� ��
    {
        //�ǰ� �ִϸ��̼�
        anim.SetTrigger("Damaged");

        spriteRenderer.color = new Color(0.8396226f, 0.3287202f, 0.3287202f, 1f); // �ǰݽ� �� �ٲ�

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 3, ForceMode2D.Impulse);


        Invoke("OffDamaged", 1f);

    }

    void OffDamaged() // �ǰ� �� ���̾� �������
    {
      
        spriteRenderer.color = new Color(0.8301887f, 0.7322891f, 0.7322891f, 1f);
    }

    void BossDie()
    {
        gameObject.SetActive(false);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BossClear("BestEndScene"); // GameManager를 통해 씬 전환
        }
        else
        {
            Debug.LogError("BossHP: GameManager 인스턴스를 찾을 수 없어 씬 전환을 할 수 없습니다!");
            // GameManager가 없으면 직접 씬 전환 (비상용)
            SceneManager.LoadScene("BestEndScene");
        }
    }

    void DieSound()
    {
        mySfx.PlayOneShot(dieSfx);
    }
}
