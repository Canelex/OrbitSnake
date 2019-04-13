using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform rocket;
    private Vector3 offset;
    private bool shaking;
    private float timeShake;
    private float timeLeft;
    private float intensity;

    public void UpdateCamera()
    {
        if (rocket == null) return;

        transform.position = rocket.position + offset;
        transform.rotation = rocket.rotation;
        
        if (shaking)
        {
            float percent = timeLeft / timeShake;
            float d = intensity * Mathf.Pow(percent, 2);
            transform.Translate(Random.Range(-d, d), Random.Range(-d, d), 0);

            // End of shake?
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0) shaking = false;
        }
    }

    public void SetRocket(Transform rocket)
    {
        this.rocket = rocket;
        offset = transform.position - rocket.position;
    }

    public void SetShaking(float time, float intensity)
    {
        this.intensity = intensity;
        timeShake = timeLeft = time;
        shaking = true;
    }

    private void Start()
    {
        if (rocket != null)
        {
            SetRocket(rocket);
        }
    }
}