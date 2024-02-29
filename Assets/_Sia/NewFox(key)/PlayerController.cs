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
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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
        { // ����&&�ٴڿ� ���������
            rigid.velocity = Vector2.up * Jump;
        }
    }

    private void FixedUpdate()
    {
        float hor = Input.GetAxis("Horizontal"); // ĳ���� �̵�
        rigid.velocity = new Vector2(hor * 3, rigid.velocity.y);

        //hor left > -1 , hor right > 1 ���� ����
        if (hor > 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else if (hor < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
}
