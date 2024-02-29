using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCheck : MonoBehaviour
{
    public SpriteRenderer mainSprite;
    EnemyAnimationController enemyAnim;

    private void Awake()
    {
        enemyAnim = GetComponentInParent<EnemyAnimationController>();
        mainSprite = GetComponentInParent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            Attack();
    }
    void Attack()
    {
        enemyAnim.EnemyAttack(true);
        //Ŭ������ ü�� ���ݷ¼��� �ɷ� �����ϰ� �ֱ�
        Debug.Log("Į ��Ҵ�.");
    }
    private void FixedUpdate()
    {
        if (mainSprite.flipX == true)
            gameObject.transform.rotation = Quaternion.Euler(0, -180, 0);
        else
            gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
