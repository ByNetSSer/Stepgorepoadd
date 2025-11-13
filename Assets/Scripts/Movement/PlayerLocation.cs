using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;
using System.Collections;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;

public class PlayerLocation : MonoBehaviour
{
    [Header("Referencias")]
    public AbstractMap map; // Asigna tu mapa de Mapbox desde el inspector

    [Header("Opciones")]
    public bool centerMapOnStart = true;
    public float updateInterval = 2f; // cada 2 segundos actualiza la posición

    private bool locationReady = false;
    private Vector2d currentLocation;

    private IEnumerator Start()
    {
        // Verificar permisos de ubicación
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogWarning("Ubicación deshabilitada en el dispositivo.");
            yield break;
        }

        // Iniciar el servicio de ubicación
        Input.location.Start();

        // Esperar a que se inicie
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogError("No se pudo obtener la ubicación.");
            yield break;
        }

        // Obtener la ubicación inicial
        currentLocation = new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude);
        locationReady = true;

        // Centrar el mapa en el jugador (solo al inicio)
        if (centerMapOnStart && map != null)
        {
            map.Initialize(currentLocation, (int)map.Zoom);
        }

        // Actualizar posición constantemente
        StartCoroutine(UpdatePositionRoutine());
    }

    private IEnumerator UpdatePositionRoutine()
    {
        while (locationReady)
        {
            var newLoc = new Vector2d(Input.location.lastData.latitude, Input.location.lastData.longitude);

            // Convertir coordenadas GPS a posición en el mapa
            Vector3 worldPos = map.GeoToWorldPosition(newLoc, true);
            transform.localPosition = worldPos;

            yield return new WaitForSeconds(updateInterval);
        }
    }
}
