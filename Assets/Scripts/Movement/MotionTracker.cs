using TMPro;
using UnityEngine;

public class MotionTracker : MonoBehaviour
{
    [Header("Referencias")]
    public TextMeshProUGUI distanceText;

    [Header("Configuración")]
    [Tooltip("Sensibilidad del movimiento detectado")]
    [Range(0.1f, 5f)] public float sensitivity = 1.5f;

    [Tooltip("Tiempo mínimo entre detecciones (segundos)")]
    public float minInterval = 0.4f;

    [Tooltip("Distancia aproximada que representa un paso o sacudida (en metros)")]
    public float metersPerStep = 0.8f;

    private float lastMagnitude = 0f;
    private float timer = 0f;
    private float totalDistance = 0f;

    void Update()
    {
        Vector3 accel = Input.acceleration;
        float magnitude = accel.magnitude;

        // Detectar sacudida fuerte (simulando un paso o desplazamiento)
        if (magnitude - lastMagnitude > sensitivity && timer <= 0f)
        {
            totalDistance += metersPerStep;
            timer = minInterval;
            UpdateText();
        }

        if (timer > 0f)
            timer -= Time.deltaTime;

        lastMagnitude = magnitude;
    }

    private void UpdateText()
    {
        if (distanceText != null)
            distanceText.text = "Distancia: " + totalDistance.ToString("F2") + " m";
    }

    // Permite reiniciar la distancia
    public void ResetDistance()
    {
        totalDistance = 0f;
        UpdateText();
    }
}
