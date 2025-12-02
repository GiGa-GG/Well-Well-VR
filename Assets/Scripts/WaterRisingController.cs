using UnityEngine;

public class WaterRisingController : MonoBehaviour
{
    public float riseSpeed = 0.05f;
    public float maxWaterHeight = 3.0f;

    void Update()
    {
        if (transform.position.y < maxWaterHeight)
        {
            transform.Translate(Vector3.up * riseSpeed * Time.deltaTime, Space.World);
        }
    }
}
