using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] float timeWithoutKilling;
    [SerializeField] float maxTimeWithoutKilling;
    [SerializeField] GameObject extremeCanvas;
    [SerializeField] GameObject adviseCanvas;
    [SerializeField] GameObject guardPrefab;
    float warningTime;
    float extremeTime;
    [SerializeField] CinemachineVirtualCamera vcam1;
    [SerializeField] CinemachineVirtualCamera vcam2;

    private void Awake()
    {
        warningTime = maxTimeWithoutKilling * 6 / 10;
        extremeTime = maxTimeWithoutKilling * 8 / 10;
    }

    private void Update()
    {
        timeWithoutKilling += Time.deltaTime;

        if(timeWithoutKilling > maxTimeWithoutKilling)
        {
            GuardGameOver();
        }
        else if(timeWithoutKilling > extremeTime)
        {
            ScreenExtremeAdvises();
        }
        else if(timeWithoutKilling > warningTime)
        {
            ScreenAdvises();
        }
    }
    public void Noise(CinemachineVirtualCamera vcam, float amplitudeGain, float frequencyGain)
    {
        CinemachineBasicMultiChannelPerlin noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = amplitudeGain;
        noise.m_FrequencyGain = frequencyGain;
    }

    private void ScreenAdvises()
    {
        Noise(vcam1, 0.5f, 0.2f);
        Noise(vcam2, 0.5f, 0.2f);
    }

    private void ScreenExtremeAdvises()
    {
        Noise(vcam1, 2f, 0.8f);
        Noise(vcam2, 2f, 0.8f);
    }

    private void GuardGameOver()
    {
       
    }

  
}
