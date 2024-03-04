using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Bullet : MonoBehaviour
{
    public float speed;

    public float distance;
    public LayerMask isLayer;

    //float full = 5.0f;
    //float energy = 0.0f;
    void Start()
    {
        Invoke("DestroyBullet", 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.right, distance, isLayer); // (������, ����, ����, ���̾��)
        if(ray.collider != null)
        {
            if (ray.collider.tag == "Enemy") {
                Debug.Log("����");
                Destroy(ray.collider.gameObject);
                DestroyBullet();
            }


           

        }

        if(transform.rotation.y == 0) {
            transform.Translate(transform.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(transform.right * -1 * speed * Time.deltaTime);
        }
    }
    /*void OnTriggerEnter2D (Collider2D coll)
    {
        Debug.Log("����");
        if (energy<full)
        {
            energy += 1.0f;
            DestroyBullet();
            gameObject.transform.Find("BossHPbar/Canvas/HPFront").transform.localScale = new Vector3(energy / full, 1.0f, 1.0f);
        }
        else
        {
            Debug.Log("�谡 �� á���");
        }
    }*/

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
