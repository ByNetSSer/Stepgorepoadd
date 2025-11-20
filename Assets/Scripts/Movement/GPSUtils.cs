using UnityEngine;

public static class GPSUtils
{
    public static Vector3 GPSToWorld(Vector2 gps)
    {
        float x = gps.y * 111320f;
        float z = gps.x * 110540f;

        return new Vector3(x, 0, z);
    }
}
