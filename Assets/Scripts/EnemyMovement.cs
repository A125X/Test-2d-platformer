using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private float jumpHeight = 5;
    [SerializeField] private LayerMask groundLayer;
    //[SerializeField] private LayerMask projetileLayer;
    [SerializeField] private int MaxExtraJumps = 1;
    //horisontal, jump, attack
    [SerializeField] public float[] inputValues = { 0.75f, 1.0f, 1.0f };
    [SerializeField] private int maxHp = 5;
    [SerializeField] private int currentHp = 5;

    private bool hit;
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private float horisontalInput;
    private int jumpBuffer = 1;
    private bool jumpAgain = false;
    private EnemyAttack enemyAttack;
    private CharacterAttack characterAttack;

    private float timer = 0.0f;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void SwitchDirection()
    {
        if (timer > 7.0f)
        {
            inputValues[0] *= -1.0f;
            inputValues[0] += 1.0f;
            timer = 0.0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Projectile")
            hit = true;
    }

    private void checkLife()
    {
        if (hit == true)
            currentHp--;

        if (currentHp <= 0)
            gameObject.SetActive(false);

        hit = false;
    }

    void Update()
    {
        //bool flag = isGrounded();
        timer += Time.deltaTime;
        //horisontalInput = Input.GetAxis("Horizontal");
        horisontalInput = (inputValues[0] - 0.5f) * 2f;
        //Debug.Log(inputValues[0]);

        SwitchDirection();

        Flip();

        if (horisontalInput != 0)
            Move();
        else
            Stop();

        //if (Input.GetKeyDown(KeyCode.Space) || jumpAgain)
        //    Jump();
        if (Mathf.Round(inputValues[1]) == 1 || jumpAgain)
            Jump();

        checkLife();
    }

    void Move()
    {
        body.velocity = new Vector2(
            horisontalInput * speed,
            body.velocity.y);
    }

    void Stop()
    {
        body.velocity = new Vector2(
            0,
            body.velocity.y);
    }

    void Flip()
    {
        if (body.velocity.x > 0.01f)
            transform.localScale = Vector3.one;
        if (body.velocity.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void Jump()
    {
        if (isGrounded())
        {
            //////////////////////////////////////////////////////////////
            jumpBuffer = MaxExtraJumps;

            body.velocity = new Vector2(
                    body.velocity.x,
                    speed * jumpHeight);

            jumpAgain = false;
        }
        else if (jumpBuffer > 0)
        {
            body.velocity = new Vector2(
                    body.velocity.x,
                    speed * jumpHeight);

            jumpBuffer--;
        }
        else if (jumpBuffer == 0)
            jumpAgain = true;
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center - new Vector3(0, 0.05f, 0),
            boxCollider.bounds.size * 1.5f - new Vector3(0, 0.5f, 0),
            0,
            Vector2.down,
            0.1f,
            groundLayer);

        BoxCastDrawer.Draw(
            raycastHit,
            boxCollider.bounds.center - new Vector3(0, 0.05f, 0),
            boxCollider.bounds.size * 1.5f - new Vector3(0, 0.5f, 0),
            0,
            Vector2.down);

        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        //return horisontalInput == 0 && isGrounded();
        return true;
    }
}