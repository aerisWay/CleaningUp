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
    [SerializeField] GameObject leftCamera;
    [SerializeField] GameObject rightCamera;
    [SerializeField] GameObject guardPrefab;
    float warningTime;
    float extremeTime;
    CinemachineVirtualCamera vcam1;
    CinemachineVirtualCamera vcam2;
    CinemachineBasicMultiChannelPerlin noise;

    private void Awake()
    {
        warningTime = maxTimeWithoutKilling * 6 / 10;
        extremeTime = maxTimeWithoutKilling * 8 / 10;

        vcam1 = leftCamera.GetComponent<CinemachineVirtualCamera>();
        vcam2 = rightCamera.GetComponent<CinemachineVirtualCamera>();
        noise = vcam1.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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
            print("ExtremeAdvises");
        }
        else if(timeWithoutKilling > warningTime)
        {
            ScreenAdvises();
            print("Advises");
        }
    }

    private void ScreenAdvises()
    {
        Shake(3.0f, 5.0f);       
    }

    private void Shake(float shakeAmplitude, float shakeFrequency)
    {
        //noise.m_AmplitudeGain = shakeAmplitude;
        //noise.m_FrequencyGain = shakeFrequency;
    }

    private void ScreenExtremeAdvises()
    {
        
    }

    private void GuardGameOver()
    {
       
    }

  
}
