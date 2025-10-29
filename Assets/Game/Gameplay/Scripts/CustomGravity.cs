using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomGravity : MonoBehaviour
{
    public float gravityScale = 1f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (rb != null && rb.useGravity)
        {
            rb.AddForce(Physics.gravity * (gravityScale - 1f), ForceMode.Acceleration);
        }
    }
}
