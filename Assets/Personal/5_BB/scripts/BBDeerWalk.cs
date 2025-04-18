using UnityEngine;

public class BBDeerWalk : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Transform target;
    private bool isMoving = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void FixedUpdate()
    {
        if (isMoving && target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Vector3 move = direction * moveSpeed * Time.fixedDeltaTime;

            rb.MovePosition(rb.position + move);

            // หยุดเมื่อใกล้เป้าหมาย
            if (Vector3.Distance(transform.position, target.position) < 0.1f)
            {
                isMoving = false;
                Destroy(gameObject);
            }
        }
    }

    public void MoveTo(Transform destination)
    {
        Debug.Log("Move");
        target = destination;
        isMoving = true;
    }
}
