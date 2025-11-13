using TMPro;
using UnityEngine;
using System.Collections;
public class GpsManager : MonoBehaviour
{
    public TextMeshProUGUI gpsText;
    public Transform playerMarker; // tu avatar en el mapa

    private void Start()
    {
        StartCoroutine(StartLocationService());
    }

    IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            gpsText.text = "GPS desactivado";
            yield break;
        }

        Input.location.Start();

        // Esperar a que se inicie
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            gpsText.text = "Error al obtener GPS";
            yield break;
        }

        gpsText.text = $"Lat: {Input.location.lastData.latitude:F6}\nLon: {Input.location.lastData.longitude:F6}";
    }

    void Update()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            var data = Input.location.lastData;
            gpsText.text = $"Lat: {data.latitude:F6}\nLon: {data.longitude:F6}";

            // Aquí puedes actualizar la posición del jugador en tu mapa
            // playerMarker.position = ConvertGeoToMap(data.latitude, data.longitude);
        }
    }
}
