using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardBehaviour : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private GameObject player;
    private bool atacked = false;
    private Animator guardAnimator;

    private void Awake()
    {
        guardAnimator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(player.transform.position);

    }

    private void Update()
    {        
        if(Vector3.Distance(player.transform.position, transform.position) < 2 && !atacked)
        {
            atacked = true;
            transform.rotation = Quaternion.LookRotation(new Vector3(player.transform.rotation.x, 0, player.transform.rotation.z));
            GameObject.Find("GameManager").GetComponent<GameManager>().GameOver();
            guardAnimator.SetBool("Punching", true);
            player.GetComponent<Hurtable>().Die();
        }
    }
}
