using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float bobSpeed = 1f;
    public float bobHeight = 0.2f;

    private Vector3 initialPosition;
    private bool isWalking = false;

    void Start()
    {
        initialPosition = transform.localPosition;

        if (GPSController.Instance != null)
            GPSController.Instance.OnLocationChanged += OnGPSLocationChanged;
    }

    void OnGPSLocationChanged(Vector2 pos)
    {
        isWalking = true;
        CancelInvoke(nameof(StopWalk));
        Invoke(nameof(StopWalk), 2f);
    }

    void StopWalk()
    {
        isWalking = false;
        transform.localPosition = initialPosition;
    }

    void Update()
    {
        if (isWalking)
        {
            float bob = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.localPosition = initialPosition + new Vector3(0, 0, bob);
        }
    }
}
