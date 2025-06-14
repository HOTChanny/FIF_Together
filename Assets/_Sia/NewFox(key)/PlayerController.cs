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

    public GameObject Hp1;

    bool IsGround;
    bool isCrouching = false;
    SpriteRenderer spriteRenderer;

    int playerLayer, platformLayer;

    public AudioSource mySfx;
    public AudioClip jumpSfx;

    public AudioSource mySfx2;
    public AudioClip hitSfx;

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
        // 웅크리기 상태 체크
        if (Input.GetKeyDown(KeyCode.X))
        {
            anim.SetTrigger("Crouch");
            isCrouching = true;
        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            isCrouching = false;
        }

        // 걷기 애니메이션 (단, 웅크릴 땐 꺼짐)
        if (!isCrouching && Mathf.Abs(rigid.velocity.x) > 0.2f)
        {
            anim.SetBool("IsWalking", true);
        }
        else
        {
            anim.SetBool("IsWalking", false);
        }

        // 점프 체크
        IsGround = Physics2D.OverlapCircle(pos.position, checkRadius, islayer);
        if (Input.GetKeyDown(KeyCode.Space) && IsGround && !isCrouching)
        {
            rigid.velocity = Vector2.up * Jump;
            anim.SetBool("IsJumping", true);
            JumpSound();
        }

        // 바닥 점프 시 통과
        if (rigid.velocity.y > 0)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, false);
        }
    }

    void FixedUpdate()
    {
        if (!isCrouching)
        {
            float hor = Input.GetAxis("Horizontal");
            rigid.velocity = new Vector2(hor * 4, rigid.velocity.y);

            if (hor > 0)
                transform.eulerAngles = new Vector3(0, 0, 0);
            else if (hor < 0)
                transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            // 웅크리면 이동 차단
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }

        // 점프 상태 해제
        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null && rayHit.distance < 1f)
            {
                anim.SetBool("IsJumping", false);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss"))
        {
            HitSound();
            OnDamaged(collision.transform.position);
        }
    }

    void OnDamaged(Vector2 targetPos)
    {
        gameObject.layer = 12;
        spriteRenderer.color = new Color(0.6981132f, 0.1522784f, 0.1522784f, 0.4f);
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);
        anim.SetTrigger("doDamaged");

        Invoke("OffDamaged", 2f);
        Invoke("Super", 5f);
    }

    void OffDamaged()
    {
        spriteRenderer.color = new Color(0.6981132f, 0.6981132f, 0.6981132f, 1);
    }

    void Super()
    {
        gameObject.layer = 11;
    }

    void JumpSound()
    {
        mySfx.PlayOneShot(jumpSfx);
    }

    void HitSound()
    {
        mySfx2.PlayOneShot(hitSfx);
    }
}
