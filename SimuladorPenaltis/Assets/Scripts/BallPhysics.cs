using UnityEngine;
using UnityEngine.InputSystem;

public class BallPhysics : MonoBehaviour
{
    [Header("Referencias")]
    public Rigidbody rb;
    public Collider goalArea;

    [Header("Potencia")]
    public float minKickForce = 25f;
    public float maxKickForce = 35f;
    public float currentKickForce;

    [Header("Rango dentro del arco")]
    public float marginX = 0.2f;
    public float marginY = 0.2f;

    [Header("Apuntado")]
    public float aimChangeInterval = 0.5f;

    [Header("Cambio de potencia")]
    public float powerChangeInterval = 0.5f;

    private bool kicked = false;
    private Vector3 currentTargetPoint;

    private float aimTimer;
    private float powerTimer;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        GenerateRandomTarget();
        GenerateRandomPower();

        aimTimer = aimChangeInterval;
        powerTimer = powerChangeInterval;
    }

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!kicked)
        {
            UpdateAim();
            UpdatePower();
        }

        if (goalArea != null)
        {
            Debug.DrawLine(transform.position, currentTargetPoint, Color.red);
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            KickBall();
        }

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetBall();
        }
    }

    private void UpdateAim()
    {
        if (goalArea == null) return;

        aimTimer -= Time.deltaTime;

        if (aimTimer <= 0f)
        {
            GenerateRandomTarget();
            aimTimer = aimChangeInterval;
        }
    }

    private void UpdatePower()
    {
        powerTimer -= Time.deltaTime;

        if (powerTimer <= 0f)
        {
            GenerateRandomPower();
            powerTimer = powerChangeInterval;
        }
    }

    public void GenerateRandomTarget()
    {
        if (goalArea == null) return;

        Bounds bounds = goalArea.bounds;

        float randomX = Random.Range(bounds.min.x + marginX, bounds.max.x - marginX);
        float randomY = Random.Range(bounds.min.y + marginY, bounds.max.y - marginY);
        float randomZ = bounds.center.z;

        currentTargetPoint = new Vector3(randomX, randomY, randomZ);
    }

    public void GenerateRandomPower()
    {
        currentKickForce = Random.Range(minKickForce, maxKickForce);
    }

    public void KickBall()
    {
        if (kicked) return;
        if (rb == null || goalArea == null) return;

        Vector3 direction = (currentTargetPoint - transform.position).normalized;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(direction * currentKickForce, ForceMode.Impulse);

        kicked = true;
    }

    public void ResetBall()
    {
        if (rb == null) return;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = startPosition;
        transform.rotation = startRotation;

        kicked = false;

        aimTimer = aimChangeInterval;
        powerTimer = powerChangeInterval;

        GenerateRandomTarget();
        GenerateRandomPower();
    }
}
