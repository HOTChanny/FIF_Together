using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;

    [SerializeField]
    private float Jump;
    [SerializeField]
    Transform pos;
    [SerializeField]
    float checkRadius;
    [SerializeField]
    LayerMask islayer;

    bool IsGround;
    SpriteRenderer spriteRenderer;

    //바닥 점프시 통과
    int playerLayer, platformLayer;
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerLayer = LayerMask.NameToLayer("Player");
        platformLayer = LayerMask.NameToLayer("Platform");
    }

    
    void Update()
    {
        if (Mathf.Abs(rigid.velocity.x) < 0.2f)
        {
            anim.SetBool("IsWalking", false);
        }
        else
        {
            anim.SetBool("IsWalking", true);
        }





        IsGround = Physics2D.OverlapCircle(pos.position, checkRadius, islayer);

        if (Input.GetKeyDown(KeyCode.Space) && IsGround == true)
        { // 점프&&바닥에 닿았을때만
            rigid.velocity = Vector2.up * Jump;
            anim.SetBool("IsJumping", true);
        }

        //바닥 점프시 통과
        if (rigid.velocity.y > 0)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, false);
        }
    }

    private void FixedUpdate()
    {
        float hor = Input.GetAxis("Horizontal"); // 캐릭터 이동
        rigid.velocity = new Vector2(hor * 4, rigid.velocity.y);

        //hor left > -1 , hor right > 1 방향 변경
        if (hor > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (hor < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        if (rigid.velocity.y < 0) { 
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if(rayHit.collider != null)
            {
                if (rayHit.distance < 1f)
                    anim.SetBool("IsJumping", false);
            }
        }
        if (Input.GetKey(KeyCode.X)) // 웅크리기
        {
            anim.SetTrigger("Crouch");

        }
    }
    void OnCollisionEnter2D(Collision2D collision) // 플레이어 피격 이벤트
    {
        if (collision.gameObject.tag == "Enemy")
        {
            OnDamaged(collision.transform.position);
        }

    }
    void OnDamaged(Vector2 targetPos) // 맞았을 때, 레이어 바꿔서 적용하기
    {
        gameObject.layer = 12;

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        //피격 애니메이션
        anim.SetTrigger("doDamaged");

        Invoke("OffDamaged", 2);
    }

    void OffDamaged() // 피격 후 2초 후에 레이어 원래대로
    {
        gameObject.layer = 11;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
}
