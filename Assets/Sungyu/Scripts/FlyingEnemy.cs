using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public float speed;
    public bool chase = false;
    public Transform startinPoint;
    private GameObject player;    
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;
        if(chase==true)
            Chase();
        else
            ReturnStartPoint();
        Flip();
    }

    private void Chase()
    {
         transform.position= Vector2.MoveTowards(transform.position,player.transform.position, Time.deltaTime*speed);
        
    }

    private void ReturnStartPoint()
    {
        transform.position = Vector2.MoveTowards(transform.position, startinPoint.position, Time.deltaTime * speed);
    }
    private void Flip()
    {
        if (transform.position.x > player.transform.position.x)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}
