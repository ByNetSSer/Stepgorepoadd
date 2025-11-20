public class KalmanFilter
{
    private float q = 0.0001f;
    private float r = 0.01f;
    private float x = 0;
    private float p = 1;
    private float k = 0;

    public float Update(float measurement)
    {
        p += q;
        k = p / (p + r);
        x += k * (measurement - x);
        p *= (1 - k);
        return x;
    }
}
