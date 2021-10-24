using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour

{
    private float speed = 5f;
    private int lives = 5;
    private float jumpForce = 10f;
    private bool isGrounded = false;

    public bool isAttacking = false;
    public bool isRecharged = true;

    public Transform attackPos;
    public float AttackRange;
    public LayerMask enemy;
    public Joystick joystick;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;

    public static Hero Instance { get; set; }

    private States State
    {
        get { return (States)anim.GetInteger("State"); }
        set { anim.SetInteger("State", (int)value); }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        Instance = this;
        isRecharged = true;
        
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

//Бег и поворот спрайта
    private void Run()
    {
        if (isGrounded) State = States.Run;

        Vector3 dir = transform.right * joystick.Horizontal;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
        sprite.flipX = dir.x < 0.0f;
    }

    private void Update()
    {
        if (isGrounded && !isAttacking) State = States.Idle;

        if (!isAttacking && joystick.Horizontal != 0)
            Run();
        if (!isAttacking && isGrounded && joystick.Vertical > 0.5f)
            Jump();
    }

//Прыжок
    private void Jump()
    {
        rb.velocity = Vector2.up * jumpForce;
    }

//Круг для радиуса атаки
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, AttackRange);
    }

//Проверяет наличие земли под ногами
    private void CheckGround()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        isGrounded = collider.Length > 0;

        if (!isGrounded) State = States.Jump;
    }

//Атака с перезарядкой
    public void Attack()
    {
        if (isGrounded & isRecharged)
        {
            State = States.Attack;
            isAttacking = true;
            isRecharged = false;

            StartCoroutine(AttackAnimation());
            StartCoroutine(AttackCollDown());
        }
    }

//Нанесение атаки по противнику
    private void OnAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, AttackRange, enemy);

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Entity>().GetDamage();
        }
    }

//Анимация атаки
    private IEnumerator AttackAnimation()
    {
        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

//Перезарядка атаки
    private IEnumerator AttackCollDown()
    {
        yield return new WaitForSeconds(0.1f);
        isRecharged = true;
    }

//Получение Героем от противников
    public void GetDamage()
    {
        lives -= 1;
        Debug.Log(lives);
    }

}

//Анимации
public enum States
{
    Idle,
    Run,
    Jump,
    Attack
}

