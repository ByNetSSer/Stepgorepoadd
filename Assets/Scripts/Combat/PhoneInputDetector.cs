using UnityEngine;

public class PhoneInputDetector : MonoBehaviour
{
    [Header("Sensibilidad de Detección")]
    public float movementThreshold = 0.20f;  // qué tan fuerte debe ser el movimiento
    public float minimumForce = 0.35f;       // fuerza mínima para validar
    public float cooldown = 0.25f;           // tiempo entre detecciones

    [Header("Referencias")]
    public CombatManager combat;

    private Vector3 lastAccel;
    private float nextAllowedInput = 0f;

    void Start()
    {
        lastAccel = Input.acceleration;
    }

    void Update()
    {
        if (!combat.IsFighting()) return;

        DetectMovementDirection();
    }


    void DetectMovementDirection()
    {
        Vector3 currentAccel = Input.acceleration;
        Vector3 delta = currentAccel - lastAccel;

        float force = delta.magnitude;

        // Enfriamiento para evitar spam
        if (Time.time < nextAllowedInput)
        {
            lastAccel = currentAccel;
            return;
        }

        // Debe superar la fuerza mínima
        if (force < minimumForce)
        {
            lastAccel = currentAccel;
            return;
        }


        // -----------------------------
        //  DIRECCIÓN PRINCIPAL
        // -----------------------------
        if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
        {
            // Movimiento vertical
            if (delta.y > movementThreshold)
            {
                combat.TryInput(ArrowType.Up);
                nextAllowedInput = Time.time + cooldown;
            }
            else if (delta.y < -movementThreshold)
            {
                combat.TryInput(ArrowType.Down);
                nextAllowedInput = Time.time + cooldown;
            }
        }
        else
        {
            // Movimiento horizontal
            if (delta.x > movementThreshold)
            {
                combat.TryInput(ArrowType.Right);
                nextAllowedInput = Time.time + cooldown;
            }
            else if (delta.x < -movementThreshold)
            {
                combat.TryInput(ArrowType.Left);
                nextAllowedInput = Time.time + cooldown;
            }
        }

        lastAccel = currentAccel;
    }
}
