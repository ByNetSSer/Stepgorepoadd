using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SingleMap : MonoBehaviour
{
    [Header("Map Settings")]
    public string apiKey = "API_KEY";
    public int zoomLevel = 16;
    public float mapSize = 50f;
    public int textureSize = 1024;

    [Header("Movement Settings")]
    public float gpsToWorldScale = 1f;

    private Renderer mapRenderer;
    private Vector2 currentGPSCenter;
    private bool mapInitialized = false;
    private bool isLoadingMap = false;

    public GameObject player;

    private Material mapMaterial;

    void Start()
    {
        mapRenderer = GetComponent<Renderer>();
        transform.localScale = new Vector3(mapSize, mapSize, 1f);

        CreatePersistentMaterial();
        StartCoroutine(InitializeMap());
    }

    void CreatePersistentMaterial()
    {
        mapMaterial = new Material(Shader.Find("Unlit/Texture"));
        mapRenderer.material = mapMaterial;
    }

    IEnumerator InitializeMap()
    {
        yield return new WaitUntil(() =>
            GPSController.Instance != null &&
            GPSController.Instance.IsGPSReady()
        );

        GPSController.Instance.OnLocationChanged += OnGPSLocationChanged;

        currentGPSCenter = GPSController.Instance.GetCurrentLocation();

        yield return StartCoroutine(LoadMapTexture(currentGPSCenter));

        mapInitialized = true;

        StartCoroutine(AutoRefreshMap());
    }

    IEnumerator AutoRefreshMap()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);

            if (mapInitialized)
            {
                Debug.Log("Updating map...");
                StartCoroutine(LoadMapTexture(currentGPSCenter));
            }
        }
    }

    void OnGPSLocationChanged(Vector2 newLocation)
    {
        if (!mapInitialized) return;

        UpdatePlayerPosition(newLocation);

        // CENTRAR SIEMPRE EL MAPA EN LA UBICACIÓN ACTUAL
        currentGPSCenter = newLocation;
    }

    void UpdatePlayerPosition(Vector2 gps)
    {
        if (!mapInitialized || player == null) return;

        Vector3 newPos = new Vector3(
            0,
            0,
            0
        );

        player.transform.position = newPos;
    }

    IEnumerator LoadMapTexture(Vector2 gpsCenter)
    {
        if (isLoadingMap) yield break;
        isLoadingMap = true;

        string url =
            $"https://maps.googleapis.com/maps/api/staticmap?center={gpsCenter.x},{gpsCenter.y}&zoom={zoomLevel}&size={textureSize}x{textureSize}&scale=2&maptype=roadmap&key={apiKey}";

        Debug.Log("Downloading map @ " + gpsCenter);

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            mapMaterial.mainTexture = texture;

            Debug.Log("Map updated.");
        }
        else
        {
            Debug.LogError("ERROR loading map: " + www.error);
        }

        isLoadingMap = false;
    }
}
