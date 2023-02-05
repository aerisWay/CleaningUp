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
    [SerializeField] GameObject rightCamera;
    [SerializeField] GameObject leftCamera;
    float warningTime;
    float extremeTime;

    private void Awake()
    {
        warningTime = maxTimeWithoutKilling * 6 / 10;
        extremeTime = maxTimeWithoutKilling * 8/ 10;
    }

    private void Update()
    {
        timeWithoutKilling += Time.deltaTime;

        if(timeWithoutKilling > maxTimeWithoutKilling)
        {
            GameOver();
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
        rightCamera.GetComponent<CameraShake>().Shake(5f, 10f);
        leftCamera.GetComponent<CameraShake>().Shake(5f, 10f);
    }

    private void ScreenExtremeAdvises()
    {
        rightCamera.GetComponent<CameraShake>().Shake(10f, 10f);
        leftCamera.GetComponent<CameraShake>().Shake(10f, 10f);
    }

    private void GameOver()
    {
       
    }
}
