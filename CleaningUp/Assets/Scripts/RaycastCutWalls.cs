using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCutWalls : MonoBehaviour
{
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] GameObject player;
    GameObject[] allTheWalls;

    private void Awake()
    {
        allTheWalls = FindGameObjectsInLayer(13);
    }
    private void Update()
    {
        Ray ray = new Ray(transform.position, player.transform.position - transform.position);

        
        
        RaycastHit[] hitInfos = Physics.RaycastAll(ray, 100, obstacleMask);

        foreach (RaycastHit hitInfo in hitInfos)
        {
            print("Hago cosas.");
            hitInfo.collider.gameObject.layer = 12;
        }



        //StartCoroutine("RedrawWall");

        
    }

    IEnumerator RedrawWall()
    {
        yield return new WaitForSeconds(1);

        foreach (var wall in allTheWalls)
        {
            if(player.transform.position.z > wall.transform.position.z)
            {
                wall.layer = 13;
            }
        }
    }

    GameObject[] FindGameObjectsInLayer(int layer)
    {
        var goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        var goList = new System.Collections.Generic.List<GameObject>();
        for (int i = 0; i < goArray.Length; i++)
        {
            if (goArray[i].layer == layer)
            {
                goList.Add(goArray[i]);
            }
        }
        if (goList.Count == 0)
        {
            return null;
        }
        return goList.ToArray();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, player.transform.position - transform.position);
    }
}
