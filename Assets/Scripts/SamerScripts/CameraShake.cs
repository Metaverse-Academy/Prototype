using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private CinemachineCamera cinemachineCam;
    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;
    private float shakeTimerTotal;
    private float startAmplitude;

    void Awake()
    {
        Instance = this;
        cinemachineCam = GetComponent<CinemachineCamera>();
        noise = (CinemachineBasicMultiChannelPerlin)cinemachineCam.GetCinemachineComponent(CinemachineCore.Stage.Noise);
    }

    public void Shake(float intensity, float time)
    {
        noise.AmplitudeGain = intensity; 
            startAmplitude = intensity;
            shakeTimer = time; // Make sure these variables are defined
            shakeTimerTotal = time;
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            noise.AmplitudeGain = Mathf.Lerp(startAmplitude, 0f, 1 - (shakeTimer / shakeTimerTotal));
        }
    }
}
