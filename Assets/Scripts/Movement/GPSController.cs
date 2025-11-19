using UnityEngine;
using UnityEngine.Android;
using System.Collections;

public class GPSController : MonoBehaviour
{
    public static GPSController Instance;

    public float latitude;
    public float longitude;
    private bool gpsInitialized = false;

    public System.Action<Vector2> OnLocationChanged;

    private Vector2 lastLocation;
    [Header("Step Counter Integration")]
    public float minimumDistanceForStep = 2f;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartCoroutine(InitializeGPS());
    }

    IEnumerator InitializeGPS()
    {
        Debug.Log("=== INICIANDO GPS ===");

        // Solicitar permisos en Android
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Debug.Log("Solicitando permiso de ubicación...");
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(3);
        }

        // Verificar si el usuario concedió permisos
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("GPS no habilitado por el usuario");

            // Mostrar diálogo nativo para activar GPS
            ShowEnableGPSDialog();
            yield return new WaitForSeconds(5);

            // Verificar nuevamente después del diálogo
            if (!Input.location.isEnabledByUser)
            {
                Debug.LogError("Usuario no activó el GPS");
                yield break;
            }
        }
#endif

        // Iniciar servicio de ubicación
        Debug.Log("Iniciando servicio de ubicación...");
        Input.location.Start(1f, 1f); // Alta precisión

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
            Debug.Log($"Esperando GPS... {maxWait}s");
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Error al inicializar GPS");
            yield break;
        }
        else
        {
            gpsInitialized = true;
            lastLocation = GetCurrentLocation();
            Debug.Log($"? GPS INICIALIZADO: {lastLocation}");

            // Notificar primera ubicación
            OnLocationChanged?.Invoke(lastLocation);

            // Iniciar actualizaciones continuas
            StartCoroutine(UpdateLocationCoroutine());
        }
    }

    IEnumerator UpdateLocationCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (gpsInitialized && Input.location.status == LocationServiceStatus.Running)
            {
                Vector2 currentLocation = GetCurrentLocation();
                float distance = Vector2.Distance(currentLocation, lastLocation);

                // ? Usar distancia mayor para evitar micro-actualizaciones
                if (distance > 0.00001f) // Aproximadamente 1 metro
                {
                    lastLocation = currentLocation;
                    Debug.Log($"?? Nueva ubicación: {currentLocation} | Distancia: {distance:F6}");
                    OnLocationChanged?.Invoke(currentLocation);
                }
            }
        }
    }
    public Vector2 GetCurrentLocation()
    {
        if (gpsInitialized && Input.location.status == LocationServiceStatus.Running)
        {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            return new Vector2(latitude, longitude);
        }
        return Vector2.zero;
    }

    public bool IsGPSReady()
    {
        return gpsInitialized && Input.location.status == LocationServiceStatus.Running;
    }

#if UNITY_ANDROID
    private void ShowEnableGPSDialog()
    {
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.settings.LOCATION_SOURCE_SETTINGS");
            currentActivity.Call("startActivity", intent);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al abrir configuraciones: " + e.Message);
        }
    }
#endif

    void OnDestroy()
    {
        if (Input.location.isEnabledByUser)
        {
            Input.location.Stop();
        }
    }
}