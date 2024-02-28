using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject bullet;
    public Transform pos;
    public float cooltime;
    private float curtime;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (curtime <= 0)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                Instantiate(bullet, pos.position, transform.rotation);

            }
            curtime = cooltime;
        }
        curtime -= Time.deltaTime;
    }
}