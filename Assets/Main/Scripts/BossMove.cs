using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
    Animator anim;
    SpriteRenderer spriteRenderer;

    
    
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        
        Invoke("Think", 3);
    }
   

    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove*3, rigid.velocity.y); // 보스이동속도

        //발판 유뮤 체크, 없으면 멈춤
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 3f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 4, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)
            Turn();

        
       
    }

    void Think()    //몬스터 움직임 AI함수
    {
        // 움직이고 난 후에 다음에 어떻게 해야하지?
        nextMove = Random.Range(-1, 2);

        float nextThinkTime = Random.Range(1f, 2f);
        Invoke("Think", nextThinkTime); // 재귀함수

        //몬스터 애니메이션
        anim.SetInteger("WalkSpeed", nextMove);

        //몬스터 방향전환
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;
    }
    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 1);
    }
    void OnCollisionEnter2D(Collision2D collision) // 몬스터 피격시, 공격 애니메이션 이벤트
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            nextMove = 0;
            anim.SetTrigger("DoAtk");
        }

    }
}
