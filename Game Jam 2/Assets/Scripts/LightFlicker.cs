using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light spotLight;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2.0f;
    public float flickerSpeed = 0.1f;

    void Start()
    {
        if (spotLight == null)
            spotLight = GetComponent<Light>();
    }

    void Update()
    {
        if (spotLight != null)
        {
            float flicker = Random.Range(minIntensity, maxIntensity);
            spotLight.intensity = Mathf.Lerp(spotLight.intensity, flicker, flickerSpeed);
        }
    }
}
