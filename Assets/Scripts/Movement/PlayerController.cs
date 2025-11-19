using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float bobSpeed = 0.5f;
    public float bobHeight = 0.1f;

    private Vector3 initialLocalPosition;
    private bool isMoving = false;

    void Start()
    {
        initialLocalPosition = transform.localPosition;

        if (GPSController.Instance != null)
        {
            GPSController.Instance.OnLocationChanged += OnLocationChanged;
        }
    }

    void OnLocationChanged(Vector2 newLocation)
    {
        isMoving = true;
        CancelInvoke("StopMoving");
        Invoke("StopMoving", 2f);
    }

    void StopMoving()
    {
        isMoving = false;
        transform.localPosition = initialLocalPosition;
    }

    void Update()
    {
        if (isMoving)
        {
            // Animación de caminar
            float bob = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.localPosition = initialLocalPosition + new Vector3(0, 0, bob);
        }
    }

    void OnDestroy()
    {
        if (GPSController.Instance != null)
        {
            GPSController.Instance.OnLocationChanged -= OnLocationChanged;
        }
    }
}