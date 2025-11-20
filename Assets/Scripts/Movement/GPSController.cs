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
    private double lastTimestamp = -1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(InitializeGPS());
    }

    IEnumerator InitializeGPS()
    {
        Debug.Log("=== INICIANDO GPS ===");

#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(3);
        }
#endif

        // ALTA PRECISIÓN REAL
        Input.location.Start(5f, 0.1f);

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            Debug.Log($"⏳ Esperando GPS... {maxWait}s");
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("❌ Error al iniciar GPS");
            yield break;
        }

        gpsInitialized = true;
        lastLocation = GetCurrentLocation();
        lastTimestamp = Input.location.lastData.timestamp;

        Debug.Log($"📍 GPS READY: {lastLocation}");

        OnLocationChanged?.Invoke(lastLocation);

        StartCoroutine(GPSUpdateLoop());
    }

    IEnumerator GPSUpdateLoop()
    {
        while (true)
        {
            if (!gpsInitialized)
            {
                yield return new WaitForSeconds(1);
                continue;
            }

            if (Input.location.status != LocationServiceStatus.Running)
            {
                Debug.LogWarning("⚠ GPS dejó de correr, reiniciando...");

                Input.location.Stop();
                Input.location.Start(5f, 0.1f);
                yield return new WaitForSeconds(2);
                continue;
            }

            LocationInfo data = Input.location.lastData;

            // SI EL TIMESTAMP CAMBIÓ → NUEVO DATO REAL
            if (data.timestamp != lastTimestamp)
            {
                lastTimestamp = data.timestamp;

                Vector2 newPos = new Vector2(data.latitude, data.longitude);

                lastLocation = newPos;
                OnLocationChanged?.Invoke(newPos);

                Debug.Log($"📡 GPS actualizado: {newPos}");
            }
            else
            {
                Debug.Log("⏳ GPS no da datos nuevos, reseteando...");
                Input.location.Stop();
                Input.location.Start(5f, 0.1f);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public Vector2 GetCurrentLocation()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            return new Vector2(latitude, longitude);
        }

        return lastLocation;
    }

    public bool IsGPSReady()
    {
        return gpsInitialized && Input.location.status == LocationServiceStatus.Running;
    }
}
