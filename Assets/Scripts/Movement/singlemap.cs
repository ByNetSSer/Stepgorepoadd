using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SingleMap : MonoBehaviour
{
    [Header("Map Settings")]
    public string apiKey = "TU_API_KEY";
    public int zoomLevel = 16;
    public float mapSize = 50f;
    public int textureSize = 1024;

    [Header("Movement Settings")]
    public float gpsToWorldScale = 1f;

    private Renderer mapRenderer;
    private Vector2 initialGPSPosition;
    private bool mapInitialized = false;
    private bool isLoadingMap = false;
    public GameObject player;

    // ✅ NUEVO: Material persistente
    private Material mapMaterial;

    void Start()
    {
        Debug.Log("=== INICIANDO SINGLE MAP ===");

        mapRenderer = GetComponent<Renderer>();
        transform.position = Vector3.zero;
        transform.localScale = new Vector3(mapSize, mapSize, 1f);

        // ✅ CORRECCIÓN: Crear material persistente UNA sola vez
        CreatePersistentMaterial();

        StartCoroutine(InitializeMap());
    }

    void CreatePersistentMaterial()
    {
        // ✅ Crear material persistente que no se destruya
        mapMaterial = new Material(Shader.Find("Unlit/Texture"));
        mapMaterial.name = "MapMaterial_Persistent";
        mapMaterial.color = Color.white; // ✅ Forzar color blanco
        mapMaterial.mainTextureScale = Vector2.one;
        mapMaterial.mainTextureOffset = Vector2.zero;

        // ✅ Asignar el material persistente al renderer
        mapRenderer.material = mapMaterial;

        Debug.Log("✅ Material persistente creado y asignado");
    }

    IEnumerator InitializeMap()
    {
        Debug.Log("Esperando GPS...");
        yield return new WaitUntil(() => GPSController.Instance != null && GPSController.Instance.IsGPSReady());

        Debug.Log("GPS listo, inicializando mapa...");
        GPSController.Instance.OnLocationChanged += OnGPSLocationChanged;

        initialGPSPosition = GPSController.Instance.GetCurrentLocation();
        yield return StartCoroutine(LoadMapTexture(initialGPSPosition));
    }

    void OnGPSLocationChanged(Vector2 newLocation)
    {
        // ✅ SOLO actualizar posición del jugador, NUNCA recargar mapa
        if (mapInitialized)
        {
            UpdatePlayerPosition(newLocation);
        }
    }

    void UpdatePlayerPosition(Vector2 currentGPS)
    {
        if (!mapInitialized || player == null) return;

        Vector2 gpsOffset = currentGPS - initialGPSPosition;

        float metersPerPixel = CalculateMetersPerPixel();
        float latToMeters = gpsOffset.x * 111320f;
        float lonToMeters = gpsOffset.y * 111320f * Mathf.Cos(initialGPSPosition.x * Mathf.Deg2Rad);

        float calibratedScale = gpsToWorldScale * metersPerPixel / 10f;

        Vector3 worldPosition = new Vector3(
            lonToMeters * calibratedScale,
            latToMeters * calibratedScale,
            0f
        );

        player.transform.position = worldPosition;

        // ✅ DEBUG reducido para evitar spam
        if (Time.frameCount % 60 == 0) // Log cada ~1 segundo
            Debug.Log($"🎯 Jugador movido | Pos: {worldPosition}");
    }

    IEnumerator LoadMapTexture(Vector2 gpsCenter)
    {
        if (isLoadingMap) yield break;

        isLoadingMap = true;
        Debug.Log($"🗺️ Cargando mapa UNA SOLA VEZ en: {gpsCenter}");

        string url = $"https://maps.googleapis.com/maps/api/staticmap?" +
                    $"center={gpsCenter.x},{gpsCenter.y}&" +
                    $"zoom={zoomLevel}&" +
                    $"size={textureSize}x{textureSize}&" +
                    $"scale=2&" +
                    $"maptype=roadmap&" +
                    $"key={apiKey}";

        Debug.Log($"🌐 URL API: {url.Substring(0, 100)}..."); // ✅ Log recortado

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);

                if (texture != null)
                {
                    // ✅ ASIGNAR TEXTURA AL MATERIAL PERSISTENTE
                    if (mapMaterial != null)
                    {
                        mapMaterial.mainTexture = texture;
                        Debug.Log($"✅ TEXTURA ASIGNADA - {texture.width}x{texture.height}");
                    }
                    else
                    {
                        Debug.LogError("❌ Material no existe, recreando...");
                        CreatePersistentMaterial();
                        mapMaterial.mainTexture = texture;
                    }

                    mapInitialized = true;

                    Debug.Log($"✅ MAPA CARGADO - UNA SOLA LLAMADA API COMPLETADA");
                    Debug.Log($"📍 Centro: {gpsCenter}");
                    Debug.Log($"📐 Metros/píxel: {CalculateMetersPerPixel():F2}m");

                    if (player != null)
                    {
                        player.transform.position = Vector3.zero;
                    }

                    // ✅ FORZAR actualización del renderer
                    mapRenderer.enabled = false;
                    mapRenderer.enabled = true;
                }
                else
                {
                    Debug.LogError("❌ Textura descargada es nula");
                }
            }
            else
            {
                Debug.LogError($"❌ Error API: {www.error}");

                // ✅ Crear textura de fallback para evitar oscurecimiento
                CreateFallbackTexture();
            }
        }

        isLoadingMap = false;
    }

    // ✅ CREAR TEXTURA DE FALLBACK (color sólido)
    void CreateFallbackTexture()
    {
        Texture2D fallbackTexture = new Texture2D(256, 256);
        Color[] pixels = new Color[256 * 256];

        // Textura de patrón de damero
        for (int y = 0; y < 256; y++)
        {
            for (int x = 0; x < 256; x++)
            {
                bool isDark = ((x / 32) + (y / 32)) % 2 == 0;
                pixels[y * 256 + x] = isDark ? Color.gray : Color.black;
            }
        }

        fallbackTexture.SetPixels(pixels);
        fallbackTexture.Apply();

        if (mapMaterial != null)
        {
            mapMaterial.mainTexture = fallbackTexture;
            mapInitialized = true;
            Debug.Log("✅ Textura de fallback asignada");
        }
    }

    float CalculateMetersPerPixel()
    {
        if (initialGPSPosition == Vector2.zero) return 1f;
        return (156543.034f * Mathf.Cos(initialGPSPosition.x * Mathf.Deg2Rad)) / Mathf.Pow(2f, zoomLevel);
    }

    void OnDestroy()
    {
        if (GPSController.Instance != null)
        {
            GPSController.Instance.OnLocationChanged -= OnGPSLocationChanged;
        }

        // ✅ Limpiar material si es necesario
        if (mapMaterial != null && !mapInitialized)
        {
            DestroyImmediate(mapMaterial);
        }
    }

    // ✅ NUEVO: Verificar estado cada frame para debug
    void Update()
    {
        // Debug ocasional del estado del material
        if (Time.frameCount % 300 == 0 && mapRenderer != null)
        {
            bool hasTexture = mapRenderer.material != null && mapRenderer.material.mainTexture != null;
            Debug.Log($"🔍 Estado Material: {(hasTexture ? "CON TEXTURA" : "SIN TEXTURA")}");
        }
    }
}