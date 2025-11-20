using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;

    [Header("Distance")]
    public float distance = 15f;
    public float minDistance = 5f;
    public float maxDistance = 30f;

    [Header("Rotation")]
    public float rotationSpeed = 0.2f;
    private float currentRotation = 0f;

    [Header("Pitch")]
    public float pitch = 45f;
    public float minPitch = 20f;
    public float maxPitch = 70f;
    public float pitchSpeed = 0.01f;

    [Header("Pinch Zoom")]
    public float pinchZoomSpeed = 0.01f;

    // internas para gestos
    private Vector2 lastTouchPos;
    private float initialFingerDistance;
    private float initialCameraDistance;
    private float initialPitch;
    private float lastTwoFingerAngle;

    void LateUpdate()
    {
        if (player == null) return;

        HandleOneFingerRotation();
        HandleTwoFingerGestures();
        UpdateCameraPosition();
    }

    // ================================================================
    //        1 DEDO → ROTAR LA CÁMARA (igual a Pokémon GO)
    // ================================================================
    void HandleOneFingerRotation()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Began)
            {
                lastTouchPos = t.position;
            }
            else if (t.phase == TouchPhase.Moved)
            {
                Vector2 delta = t.position - lastTouchPos;
                currentRotation += delta.x * rotationSpeed;
                lastTouchPos = t.position;
            }
        }
    }

    // ================================================================
    //        2 DEDOS → PINCH ZOOM + ROTACIÓN + PITCH
    // ================================================================
    void HandleTwoFingerGestures()
    {
        if (Input.touchCount < 2) return;

        Touch t1 = Input.GetTouch(0);
        Touch t2 = Input.GetTouch(1);

        float currentDistance = Vector2.Distance(t1.position, t2.position);

        // Inicializar gesto cuando tocan 2 dedos
        if (t2.phase == TouchPhase.Began)
        {
            initialFingerDistance = currentDistance;
            initialCameraDistance = distance;
            initialPitch = pitch;

            lastTwoFingerAngle = Mathf.Atan2(
                t2.position.y - t1.position.y,
                t2.position.x - t1.position.x
            ) * Mathf.Rad2Deg;

            return;
        }

        // ---------------------- PINCH ZOOM --------------------------
        float pinchDelta = currentDistance - initialFingerDistance;

        distance = Mathf.Clamp(
            initialCameraDistance - pinchDelta * pinchZoomSpeed,
            minDistance,
            maxDistance
        );

        // ---------------------- PITCH (inclinación) ------------------
        pitch = Mathf.Clamp(
            initialPitch - pinchDelta * pitchSpeed,
            minPitch,
            maxPitch
        );

        // ---------------------- ROTACIÓN CON 2 DEDOS -----------------
        float angle = Mathf.Atan2(
            t2.position.y - t1.position.y,
            t2.position.x - t1.position.x
        ) * Mathf.Rad2Deg;

        float angleDelta = Mathf.DeltaAngle(lastTwoFingerAngle, angle);

        currentRotation += angleDelta * 0.2f; // velocidad de rotación

        lastTwoFingerAngle = angle;
    }

    // ================================================================
    //        ACTUALIZAR POSICIÓN DE LA CÁMARA
    // ================================================================
    void UpdateCameraPosition()
    {
        Quaternion rot = Quaternion.Euler(pitch, currentRotation, 0);

        Vector3 offset = rot * new Vector3(0, 0, -distance);

        transform.position = player.position + offset;
        transform.LookAt(player.position);
    }
}
