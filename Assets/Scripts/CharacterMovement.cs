using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class BoxCastDrawer
{
    /// <summary>
    ///     Visualizes BoxCast with help of debug lines.
    /// </summary>
    /// <param name="hitInfo"> The cast result. </param>
    /// <param name="origin"> The point in 2D space where the box originates. </param>
    /// <param name="size"> The size of the box. </param>
    /// <param name="angle"> The angle of the box (in degrees). </param>
    /// <param name="direction"> A vector representing the direction of the box. </param>
    /// <param name="distance"> The maximum distance over which to cast the box. </param>
    public static void Draw(
        RaycastHit2D hitInfo,
        Vector2 origin,
        Vector2 size,
        float angle,
        Vector2 direction,
        float distance = Mathf.Infinity)
    {
        // Set up points to draw the cast.
        Vector2[] originalBox = CreateOriginalBox(origin, size, angle);

        Vector2 distanceVector = GetDistanceVector(distance, direction);
        Vector2[] shiftedBox = CreateShiftedBox(originalBox, distanceVector);

        // Draw the cast.
        Color castColor = hitInfo ? Color.red : Color.green;
        DrawBox(originalBox, castColor);
        DrawBox(shiftedBox, castColor);
        ConnectBoxes(originalBox, shiftedBox, Color.gray);

        if (hitInfo)
        {
            Debug.DrawLine(hitInfo.point, hitInfo.point + (hitInfo.normal.normalized * 0.2f), Color.yellow);
        }
    }

    /// <summary>
    ///     Casts a box against colliders in the Scene, returning the first collider to contact with it, and visualizes it.
    /// </summary>
    /// <param name="origin"> The point in 2D space where the box originates. </param>
    /// <param name="size"> The size of the box. </param>
    /// <param name="angle"> The angle of the box (in degrees). </param>
    /// <param name="direction"> A vector representing the direction of the box. </param>
    /// <param name="distance"> The maximum distance over which to cast the box. </param>
    /// <param name="layerMask"> Filter to detect Colliders only on certain layers. </param>
    /// <param name="minDepth"> Only include objects with a Z coordinate (depth) greater than or equal to this value. </param>
    /// <param name="maxDepth"> Only include objects with a Z coordinate (depth) less than or equal to this value. </param>
    /// <returns>
    ///     The cast result.
    /// </returns>
    public static RaycastHit2D BoxCastAndDraw(
        Vector2 origin,
        Vector2 size,
        float angle,
        Vector2 direction,
        float distance = Mathf.Infinity,
        int layerMask = Physics2D.AllLayers,
        float minDepth = -Mathf.Infinity,
        float maxDepth = Mathf.Infinity)
    {
        var hitInfo = Physics2D.BoxCast(origin, size, angle, direction, distance, layerMask, minDepth, maxDepth);
        Draw(hitInfo, origin, size, angle, direction, distance);
        return hitInfo;
    }

    private static Vector2[] CreateOriginalBox(Vector2 origin, Vector2 size, float angle)
    {
        float w = size.x * 0.5f;
        float h = size.y * 0.5f;
        Quaternion q = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));

        var box = new Vector2[4]
        {
            new Vector2(-w, h),
            new Vector2(w, h),
            new Vector2(w, -h),
            new Vector2(-w, -h),
        };

        for (int i = 0; i < 4; i++)
        {
            box[i] = (Vector2)(q * box[i]) + origin;
        }

        return box;
    }

    private static Vector2[] CreateShiftedBox(Vector2[] box, Vector2 distance)
    {
        var shiftedBox = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            shiftedBox[i] = box[i] + distance;
        }

        return shiftedBox;
    }

    private static void DrawBox(Vector2[] box, Color color)
    {
        Debug.DrawLine(box[0], box[1], color);
        Debug.DrawLine(box[1], box[2], color);
        Debug.DrawLine(box[2], box[3], color);
        Debug.DrawLine(box[3], box[0], color);
    }

    private static void ConnectBoxes(Vector2[] firstBox, Vector2[] secondBox, Color color)
    {
        Debug.DrawLine(firstBox[0], secondBox[0], color);
        Debug.DrawLine(firstBox[1], secondBox[1], color);
        Debug.DrawLine(firstBox[2], secondBox[2], color);
        Debug.DrawLine(firstBox[3], secondBox[3], color);
    }

    private static Vector2 GetDistanceVector(float distance, Vector2 direction)
    {
        if (distance == Mathf.Infinity)
        {
            // Draw some large distance e.g. 5 scene widths long.
            float sceneWidth = Camera.main.orthographicSize * Camera.main.aspect * 2f;
            distance = sceneWidth * 5f;
        }

        return direction.normalized * distance;
    }
}


public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10;
    [SerializeField] private float jumpHeight = 5;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int MaxExtraJumps = 1;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private float horisontalInput;
    private int jumpBuffer = 1;
    private bool jumpAgain = false;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        bool flag = isGrounded();
        horisontalInput = Input.GetAxis("Horizontal");

        Flip();

        if (horisontalInput != 0)
            Move();
        else
            Stop();
        
        if (Input.GetKeyDown(KeyCode.Space) || jumpAgain)
            Jump();
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