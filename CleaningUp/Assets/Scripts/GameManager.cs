using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject leftCamera;
    [SerializeField] GameObject rightCamera;
    [SerializeField] GameObject fullCamera;
    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;
    [SerializeField] float maxDistanceBetween;


    private void Update()
    {
        if(Vector3.Distance(player1.transform.position, player2.transform.position) > maxDistanceBetween)
        {
            fullCamera.SetActive(false);
            rightCamera.SetActive(true);
            leftCamera.SetActive(true);
        }
        else
        {
            fullCamera.SetActive(true);
            rightCamera.SetActive(false);
            leftCamera.SetActive(false);
        }
    }
}
