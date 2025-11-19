using UnityEngine;

using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float distance = 10f;
    public float height = 5f;
    public float rotationSpeed = 2f;
    public float touchSensitivity = 0.1f;
    public float smoothFactor = 0.1f;

    private float currentRotation = 0f;
    private Vector2 lastTouchPosition;
    private bool isRotating = false;
    private bool useGyro = false;

    // Variables para rotación por sensores
    private Quaternion initialOrientation;
    private Quaternion cameraBaseRotation;
    private Vector3 compassRaw;
    private float magneticHeading = 0f;
    private float trueHeading = 0f;

    void Start()
    {
        InitializeSensors();
        UpdateCameraPosition();
    }

    void InitializeSensors()
    {
        // Activar giroscopio
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            useGyro = true;
            initialOrientation = Quaternion.Euler(90, 0, 0); // Ajuste inicial
            Debug.Log("✅ Giroscopio activado");
        }
        else
        {
            Debug.Log("❌ Giroscopio no disponible");
        }

        // Activar brújula
        Input.compass.enabled = true;
        Debug.Log("✅ Brújula activada");

        // Calibrar orientación inicial
        CalibrateOrientation();
    }

    void CalibrateOrientation()
    {
        if (useGyro)
        {
            cameraBaseRotation = Quaternion.Inverse(Input.gyro.attitude);
        }

        // Usar brújula para orientación inicial
        magneticHeading = Input.compass.magneticHeading;
        trueHeading = Input.compass.trueHeading;

        Debug.Log($"🧭 Brújula calibrada - Magnético: {magneticHeading}°, Verdadero: {trueHeading}°");
    }

    void LateUpdate()
    {
        if (player == null) return;

        HandleTouchInput();
        HandleSensorInput();
        UpdateCameraPosition();
    }

    void HandleTouchInput()
    {
        // Rotación táctil como respaldo
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    isRotating = true;
                    lastTouchPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                    if (isRotating)
                    {
                        Vector2 delta = touch.position - lastTouchPosition;
                        currentRotation += delta.x * touchSensitivity;
                        lastTouchPosition = touch.position;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isRotating = false;
                    break;
            }
        }
    }

    void HandleSensorInput()
    {
        if (!useGyro) return;

        try
        {
            // Obtener orientación del giroscopio
            Quaternion gyroAttitude = Input.gyro.attitude;

            // Aplicar rotación base y ajustar orientación
            Quaternion fixedRotation = cameraBaseRotation * gyroAttitude;

            // Convertir a ángulo de rotación en Y (horizontal)
            Vector3 euler = fixedRotation.eulerAngles;

            // Usar la componente Y de la rotación del giroscopio
            // Invertir porque el giroscopio da rotación invertida
            float gyroYRotation = -euler.z; // Usar Z porque el celular está en modo landscape

            // Suavizar la transición
            currentRotation = Mathf.LerpAngle(currentRotation, gyroYRotation, smoothFactor);

            // Debug ocasional
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"📱 Gyro: {gyroYRotation:F1}° | Current: {currentRotation:F1}°");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error en sensor: {e.Message}");
            useGyro = false;
        }
    }

    void UpdateCameraPosition()
    {
        // Cámara orbital alrededor del jugador
        Vector3 offset = Quaternion.Euler(0, currentRotation, 0) * new Vector3(0, height, -distance);
        transform.position = player.position + offset;
        transform.LookAt(player.position);
    }

    // Método para recalibrar orientación (útil cuando cambia la orientación del celular)
    public void Recalibrate()
    {
        CalibrateOrientation();
        Debug.Log("🔄 Cámara recalibrada");
    }

    // Método para cambiar entre modos de control
    public void ToggleControlMode()
    {
        useGyro = !useGyro;
        Debug.Log(useGyro ? "🎯 Modo: Sensores" : "👆 Modo: Táctil");
    }

    void Update()
    {
        // Controles de debug en Editor
        if (Application.isEditor && player != null)
        {
            // Rotación con teclas
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                currentRotation -= rotationSpeed * Time.deltaTime * 50f;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                currentRotation += rotationSpeed * Time.deltaTime * 50f;
            }

            // Zoom con teclas
            if (Input.GetKey(KeyCode.UpArrow))
            {
                distance = Mathf.Max(5f, distance - 1f);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                distance = Mathf.Min(20f, distance + 1f);
            }

            // Recalibrar con tecla R
            if (Input.GetKeyDown(KeyCode.R))
            {
                Recalibrate();
            }

            // Cambiar modo con tecla G
            if (Input.GetKeyDown(KeyCode.G))
            {
                ToggleControlMode();
            }

            UpdateCameraPosition();
        }

        // En dispositivo móvil, recalibrar cuando se detecte cambio brusco de orientación
        if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft ||
            Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        {
            // Podrías recalibrar automáticamente cuando cambia la orientación
        }
    }

    // Para debug en pantalla (opcional)
    void OnGUI()
    {
        if (Application.isMobilePlatform)
        {
            GUI.Label(new Rect(10, 10, 300, 30), $"Rotación: {currentRotation:F1}°");
            GUI.Label(new Rect(10, 40, 300, 30), $"Modo: {(useGyro ? "Sensores" : "Táctil")}");
            GUI.Label(new Rect(10, 70, 300, 30), $"Brújula: {Input.compass.trueHeading:F1}°");

            if (GUI.Button(new Rect(10, 100, 150, 40), "Recalibrar"))
            {
                Recalibrate();
            }

            if (GUI.Button(new Rect(10, 150, 150, 40), "Cambiar Modo"))
            {
                ToggleControlMode();
            }
        }
    }
}