using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class StepCounter : MonoBehaviour
{
    public static StepCounter Instance;

    [Header("Step Counter Settings")]
    public float metersPerStep = 0.7f; // Distancia promedio por paso en metros
    public bool countInBackground = true;

    [Header("UI Reference")]
    public TextMeshProUGUI stepsText;

    private int totalSteps = 0;
    private Vector2 lastRecordedPosition;
    private float accumulatedDistance = 0f;
    private bool isInitialized = false;
    private DateTime lastSaveTime;

    // Datos persistentes
    private const string STEPS_KEY = "TotalSteps";
    private const string POSITION_KEY = "LastPosition";
    private const string TIME_KEY = "LastSaveTime";
    // Añadir esta línea en la clase StepCounter existente:
    public System.Action<int> OnStepsUpdated;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeStepCounter();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeStepCounter()
    {
        LoadPersistentData();

        if (GPSController.Instance != null)
        {
            GPSController.Instance.OnLocationChanged += OnLocationChanged;
        }

        StartCoroutine(UpdateStepsUICoroutine());
        isInitialized = true;
    }

    void OnLocationChanged(Vector2 newLocation)
    {
        if (!isInitialized) return;

        CalculateStepsFromMovement(newLocation);
    }

    void CalculateStepsFromMovement(Vector2 currentLocation)
    {
        if (lastRecordedPosition == Vector2.zero)
        {
            lastRecordedPosition = currentLocation;
            return;
        }

        // Calcular distancia en metros usando fórmula de Haversine
        float distance = CalculateDistanceInMeters(lastRecordedPosition, currentLocation);

        if (distance > 0.1f) // Filtro para evitar micro-movimientos
        {
            accumulatedDistance += distance;

            // Calcular pasos basados en distancia acumulada
            int newSteps = Mathf.FloorToInt(accumulatedDistance / metersPerStep);

            if (newSteps > 0)
            {
                totalSteps += newSteps;
                accumulatedDistance -= newSteps * metersPerStep; // Mantener el resto

                UpdateStepsUI();
                SavePersistentData();
            }

            lastRecordedPosition = currentLocation;
        }
    }

    float CalculateDistanceInMeters(Vector2 pos1, Vector2 pos2)
    {
        // Fórmula de Haversine para calcular distancia en metros
        float R = 6371000f; // Radio de la Tierra en metros
        float dLat = (pos2.x - pos1.x) * Mathf.Deg2Rad;
        float dLon = (pos2.y - pos1.y) * Mathf.Deg2Rad;

        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                 Mathf.Cos(pos1.x * Mathf.Deg2Rad) * Mathf.Cos(pos2.x * Mathf.Deg2Rad) *
                 Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);

        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        return R * c;
    }

    void UpdateStepsUI()
    {
        if (stepsText != null)
        {
            stepsText.text = $"{totalSteps}";
        }
    }

    IEnumerator UpdateStepsUICoroutine()
    {
        while (true)
        {
            UpdateStepsUI();
            yield return new WaitForSeconds(1f); // Actualizar UI cada segundo
        }
    }

    void LoadPersistentData()
    {
        // Cargar pasos totales
        totalSteps = PlayerPrefs.GetInt(STEPS_KEY, 0);

        // Cargar última posición
        string positionData = PlayerPrefs.GetString(POSITION_KEY, "");
        if (!string.IsNullOrEmpty(positionData))
        {
            string[] parts = positionData.Split('|');
            if (parts.Length == 2)
            {
                lastRecordedPosition = new Vector2(
                    float.Parse(parts[0]),
                    float.Parse(parts[1])
                );
            }
        }

        // Cargar última hora de guardado
        string timeData = PlayerPrefs.GetString(TIME_KEY, "");
        if (!string.IsNullOrEmpty(timeData))
        {
            if (DateTime.TryParse(timeData, out DateTime savedTime))
            {
                lastSaveTime = savedTime;

                // Si ha pasado mucho tiempo, podrías resetear o ajustar
                TimeSpan timeSinceLastSave = DateTime.Now - savedTime;
                if (timeSinceLastSave.TotalHours > 24)
                {
                    Debug.Log("Más de 24 horas desde el último guardado");
                }
            }
        }

        Debug.Log($"?? Pasos cargados: {totalSteps}");
    }

    void SavePersistentData()
    {
        PlayerPrefs.SetInt(STEPS_KEY, totalSteps);

        // Guardar posición actual
        string positionData = $"{lastRecordedPosition.x}|{lastRecordedPosition.y}";
        PlayerPrefs.SetString(POSITION_KEY, positionData);

        // Guardar hora actual
        lastSaveTime = DateTime.Now;
        PlayerPrefs.SetString(TIME_KEY, lastSaveTime.ToString());

        PlayerPrefs.Save();
    }

    // Método para simular pasos (para testing)
    public void AddTestSteps(int steps)
    {
        totalSteps += steps;
        UpdateStepsUI();
        SavePersistentData();
    }

    // Método para resetear contador
    public void ResetSteps()
    {
        totalSteps = 0;
        accumulatedDistance = 0f;
        UpdateStepsUI();
        SavePersistentData();
    }

    // Métodos públicos para obtener datos
    public int GetTotalSteps() => totalSteps;
    public float GetDistanceWalked() => totalSteps * metersPerStep;

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // App en segundo plano - guardar datos
            Debug.Log("?? Guardando datos antes de ir a segundo plano...");
            SavePersistentData();
        }
        else
        {
            // App reactivada - recalcular pasos si es necesario
            Debug.Log("?? App reactivada - recalculando posibles pasos...");
            if (GPSController.Instance != null && GPSController.Instance.IsGPSReady())
            {
                Vector2 currentPos = GPSController.Instance.GetCurrentLocation();
                CalculateStepsFromMovement(currentPos);
            }
        }
    }

    void OnDestroy()
    {
        SavePersistentData();
        if (GPSController.Instance != null)
        {
            GPSController.Instance.OnLocationChanged -= OnLocationChanged;
        }
    }
}