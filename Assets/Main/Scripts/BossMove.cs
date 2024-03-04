using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
    Animator anim;
    SpriteRenderer spriteRenderer;

    //float full = 5.0f;
    //float energy = 0.0f;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Invoke("Think", 3);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //���� ���� üũ, ������ ����
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 4, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)
            Turn();
    }

    void Think()    //���� ������ AI�Լ�
    {
        // �����̰� �� �Ŀ� ������ ��� �ؾ�����?
        nextMove = Random.Range(-1, 2);

        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime); // ����Լ�

        //���� �ִϸ��̼�
        anim.SetInteger("WalkSpeed", nextMove);

        //���� ������ȯ
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;
    }
    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 3);
    }
    void OnCollisionEnter2D(Collision2D collision) // ���� �ǰݽ�, ���� �ִϸ��̼� �̺�Ʈ
    {
        if (collision.gameObject.tag == "Player")
        {
            anim.SetTrigger("DoAtk");
        }


        /*if ( (collision.gameObject.tag == "Bullet") && (energy < full) )
        {
            Debug.Log("����!");
            energy += 1.0f;
            Destroy(collision.gameObject);
            gameObject.transform.Find("Boss/Canvas/HPFront").transform.localScale = new Vector3(energy / full, 1.0f, 1.0f);
        }
        else
        {
            Debug.Log("�谡 �� á���");
        }

    }*/

    }
}
