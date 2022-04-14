using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private float jumpHeight = 5;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;

    [SerializeField] private int jumpBuffer = 1;
    [SerializeField] private int MaxExtraJumps = 1;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetAxis("Horizontal") != 0)
            Move();

        Flip();

        if (Input.GetKey(KeyCode.Space))
            Jump();
    }

    void Move()
    {
        float horisontalInput = Input.GetAxis("Horizontal");

        body.velocity = new Vector2(
            horisontalInput * speed,
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
            if (isGrounded())
                jumpBuffer = MaxExtraJumps;

            body.velocity = new Vector2(
                    body.velocity.x,
                    speed * jumpHeight);
        }
        else if (jumpBuffer > 0)
        {
            body.velocity = new Vector2(
                    body.velocity.x,
                    speed * jumpHeight);

            jumpBuffer--;
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size, 
            0,
            Vector2.down,
            0.1f, 
            groundLayer);
            return raycastHit.collider != null;
    }
}
