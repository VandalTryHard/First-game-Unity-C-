using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : Entity
{
    public float speed;

    public int positionOfPatrol;

    public Transform point;

    bool movingRight = true;

    Transform player;

    public float stoppingDistance;
    private SpriteRenderer sprite;

    bool chill = false;
    bool angry = false;
    bool goBack = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lives = 1;
    }
    void Update()
    {
        if(Vector2.Distance(transform.position, point.position) < positionOfPatrol && angry == false)
        {
            chill = true;
        }
        if (Vector2.Distance(transform.position, player.position) < stoppingDistance)
        {
            angry = true;
            chill = false;
            goBack = false;
        }
        if (Vector2.Distance(transform.position, player.position) > stoppingDistance)
        {
            goBack = true;
            angry = false;
        }

        if (chill == true)
        {
            Chill();
        }
        else if (angry == true)
        {
            Angry();
        }
        else if (goBack == true)
        {
            GoBack();
        }

    }
    void Chill()
    {
        if (transform.position.x > point.position.x + positionOfPatrol)
        {   
            movingRight = false;
        }
        else if (transform.position.x < point.position.x - positionOfPatrol)
        {
            movingRight = true;
        }
        if (movingRight)
        {
            transform.position = new Vector2(transform.position.x + speed * Time.deltaTime, transform.position.y);
        }
        else
        {
            transform.position = new Vector2(transform.position.x - speed * Time.deltaTime, transform.position.y);
        }
    }

    void Angry()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        speed = 2;
        FlipPatrol();
    }

    void GoBack()
    {
        transform.position = Vector2.MoveTowards(transform.position, point.position, speed * Time.deltaTime);
        speed = 1;
        FlipPatrol();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            Hero.Instance.GetDamage();
            lives--;
            Debug.Log("у патруля " + lives);
        }

        if (lives < 1)
            Die();
    }

    void FlipPatrol()
    {
        movingRight = !movingRight;
        if (movingRight == false)
        {
            Vector3 lTemp = transform.localScale;
            lTemp.x = 1;
            transform.localScale = lTemp;
        }
        else if (movingRight == true)
        {
            Vector3 lTemp = transform.localScale;
            lTemp.x = -1;
            transform.localScale = lTemp;
        }
    }
}
