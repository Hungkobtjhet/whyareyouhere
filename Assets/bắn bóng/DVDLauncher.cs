using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class DVDLauncher : MonoBehaviour
{
    [Header("Cài đặt ngắm bắn")]
    public float rotationSpeed = 150f;
    public float flySpeed = 15f;
    public float maxAngle = 75f;

    private Rigidbody rb;
    private bool isFired = false;
    private float currentAngle = 0f;
    private LineRenderer laserLine;
    private Vector3 lastFlyDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        laserLine = gameObject.AddComponent<LineRenderer>();
        laserLine.startWidth = 0.1f; laserLine.endWidth = 0.05f;
        laserLine.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        laserLine.startColor = Color.red; laserLine.endColor = Color.red;
    }

    void Update()
    {
        if (rb == null || isFired) return;

        if (Keyboard.current != null)
        {
            float rotationInput = 0f;
            if (Keyboard.current.aKey.isPressed) rotationInput = -1f;
            else if (Keyboard.current.dKey.isPressed) rotationInput = 1f;

            if (rotationInput != 0f)
            {
                currentAngle += rotationInput * rotationSpeed * Time.deltaTime;
                currentAngle = Mathf.Clamp(currentAngle, -maxAngle, maxAngle);
            }

            Vector3 shootDirection = Quaternion.AngleAxis(currentAngle, Vector3.right) * Vector3.up;
            float previewDistance = 25f; // Quét xa ra để thấy rõ đường nảy

            laserLine.enabled = true;

            // [NÂNG CẤP NGẮM BẮN]: Phóng tia ảo quét trước tương lai
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 0.4f, shootDirection, out hit, previewDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.name.Contains("Wall_Side"))
                {
                    // Nếu chạm tường nảy -> Gập tia Laser làm 3 khúc
                    laserLine.positionCount = 3;
                    laserLine.SetPosition(0, transform.position);

                    // Điểm chạm tâm viên đạn
                    Vector3 hitPoint = transform.position + shootDirection * hit.distance;
                    laserLine.SetPosition(1, hitPoint);

                    // Tính toán tia phản xạ
                    Vector3 reflectDir = Vector3.Reflect(shootDirection, hit.normal);
                    float remainingDist = previewDistance - hit.distance;
                    laserLine.SetPosition(2, hitPoint + reflectDir * remainingDist);
                }
                else
                {
                    // Chạm trần hoặc chạm gạch -> Tia laser dừng lại đúng điểm đó
                    laserLine.positionCount = 2;
                    laserLine.SetPosition(0, transform.position);
                    Vector3 hitPoint = transform.position + shootDirection * hit.distance;
                    laserLine.SetPosition(1, hitPoint);
                }
            }
            else
            {
                // Không chạm gì -> Bắn thẳng
                laserLine.positionCount = 2;
                laserLine.SetPosition(0, transform.position);
                laserLine.SetPosition(1, transform.position + shootDirection * previewDistance);
            }

            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                isFired = true;
                laserLine.enabled = false;
                rb.linearVelocity = shootDirection * flySpeed;
                lastFlyDirection = shootDirection;
            }
        }
    }

    void FixedUpdate()
    {
        if (rb == null || rb.isKinematic || !isFired) return;

        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * flySpeed;
            lastFlyDirection = rb.linearVelocity.normalized;
        }
        else
        {
            rb.linearVelocity = lastFlyDirection * flySpeed;
        }

        Vector3 currentDirection = rb.linearVelocity.normalized;
        float checkDistance = (flySpeed * Time.fixedDeltaTime) + 0.1f;

        Collider myCol = GetComponent<Collider>();
        if (myCol != null) myCol.enabled = false;

        RaycastHit hit;
        bool hasHit = Physics.SphereCast(transform.position, 0.4f, currentDirection, out hit, checkDistance, ~0, QueryTriggerInteraction.Ignore);

        if (myCol != null) myCol.enabled = true;

        if (hasHit)
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.name.Contains("Wall_Bottom"))
            {
                Destroy(gameObject);
                return;
            }

            if (hitObject.name.Contains("Wall_Side"))
            {
                Vector3 reflectDir = Vector3.Reflect(currentDirection, hit.normal);
                lastFlyDirection = reflectDir;
                rb.linearVelocity = reflectDir * flySpeed;

                transform.position += hit.normal * 0.05f;
                return;
            }

            if (hitObject.name.Contains("Wall_Ceiling") || hitObject.GetComponent<BlockShatterMechanic>() != null)
            {
                transform.position = hit.point - (currentDirection * 0.5f);
                StopAndStick(hitObject);
            }
        }
    }

    private void StopAndStick(GameObject hitObject)
    {
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = false;

        if (laserLine != null) Destroy(laserLine);

        GameObject parentContainer = GameObject.Find("Inner_Cubes_Container");
        if (parentContainer != null)
        {
            transform.SetParent(parentContainer.transform, true);
        }
        else if (hitObject != null && hitObject.transform.parent != null)
        {
            transform.SetParent(hitObject.transform.parent, true);
        }
        else
        {
            MapMover map = FindObjectOfType<MapMover>();
            if (map != null) transform.SetParent(map.transform, true);
        }

        Vector3 currentPos = transform.localPosition;
        float snappedZ = Mathf.Round(currentPos.z);
        float yOffset = (Mathf.RoundToInt(snappedZ) % 2 == 0) ? 0.5f : 0f;
        float snappedY = Mathf.Round(currentPos.y - yOffset) + yOffset;

        transform.localPosition = new Vector3(0, snappedY, snappedZ);
        transform.localRotation = Quaternion.identity;

        BlockShatterMechanic myShatter = GetComponent<BlockShatterMechanic>();
        if (myShatter != null)
        {
            myShatter.enabled = true;
            myShatter.CheckSurroundingAndBreak();
        }

        this.enabled = false;
        Destroy(this);
    }
}