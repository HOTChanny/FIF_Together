using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    private float curTime;
    public float coolTime = 0.5f;
    public Transform pos;
    public Vector2 boxSize;

    //public GameManager manager;
    
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim= GetComponent<Animator>();
        
    }

    //바닥 점프시 통과
    int playerLayer, platformLayer;
    void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        platformLayer = LayerMask.NameToLayer("Platform");
    }
    void Update()
    {
        //바닥 점프시 통과
        if(rigid.velocity.y > 0)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, false);
        }



        if (Input.GetButtonUp("Horizontal"))
        {
            //키 뗐을 때
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //방향전환
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //애니메이션 전환(서있기<>걷기)
        if (Mathf.Abs(rigid.velocity.x) < 0.48) // 속도가 얼마나 떨어졌을 때 모션이 바뀌는지
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);

        //애니메이션 전환 - 점프
        if (Input.GetKeyDown(KeyCode.Space) && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }

        //애니메이션 전환-공격 : q키를 눌렀을 때 어택 애니메이션이 실행되고 있지 않다면 트리거 진행
        if (curTime <=0)
            //&& !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")
        {
            if (Input.GetKey(KeyCode.Q) && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {//공격

                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
                foreach (Collider2D collider in collider2Ds)
                {
                    Debug.Log(collider.tag);
                }
                anim.SetTrigger("atk");
                curTime = coolTime;
            }
            else
            {
                curTime -= Time.deltaTime;
            }
       
        
        }

        
    }
    


    void FixedUpdate()
    {
        //Move Speed
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Max Speed
        if(rigid.velocity.x > maxSpeed) // Right Max Speed
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed*(-1)) // Left Max Speed
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        //RayCast>> 오브젝트 검색을 위해 Ray를 쏘는 방식, 바닥감지
        if(rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.6f)
                    anim.SetBool("isJumping", false);
            }
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            OnDamaged(collision.transform.position);
        }
    }
   
    void OnDamaged(Vector2 targetPos) // 맞았을 때, 레이어 바꿔서 적용하기
    {
        gameObject.layer = 11;

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

        //피격 애니메이션
        anim.SetTrigger("doDamaged");

        Invoke("OffDamaged", 2);
    }

    void OffDamaged() // 피격 후 2초 후에 레이어 원래대로
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }
}
