using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuestBehaviour : MonoBehaviour
{
    public enum NPCState
    {
        STAND, SCARED, ROOM_ACTION, GOING_HELP, DEAD
    }

    private bool alive = true;
    public bool seeingPlayer = false;
    public GameObject playerSeen;
    private NPCState currentState;
    private NavMeshAgent navMesh;
    private Animator guestAnimator;


    [Header("Reaction Time")]
    GameObject gameManager;
    [SerializeField] float minReactionTime;
    [SerializeField] float maxReactionTime;
    [SerializeField] bool decisionDone = false;
    [SerializeField] float initialSearchRadius;
    [SerializeField] int maxRadiusMultiplier;
    private bool inRoom;
    private string currentRoomTag;
    [Space]
    [Header("Atributes")]
    [SerializeField] int extremeNecessityValue;
    [SerializeField] [Range(0,20)] float bathDesire; // 0-20 x1.5
    [SerializeField] [Range(0, 20)] float fuckDesire; // 0-20 x0.9
    [SerializeField] [Range(0, 20)] float consumeDesire; //0-20 1.1
    [SerializeField] [Range(0, 20)] float talkingDesire; // 0-20 x0.8
    [SerializeField] [Range(0, 20)] float curiosity; // 0-20 x1.3    
    private List<float> valueNecessities = new List<float>();
    private int maxIndex;
    [Space]
    [Header("Multipliers")]
    [SerializeField] [Range(0.0f, 2.0f)] float bathMultiplier;
    [SerializeField] [Range(0.0f, 2.0f)] float fuckMultiplier;
    [SerializeField] [Range(0.0f, 2.0f)] float consumeMultiplier;
    [SerializeField] [Range(0.0f, 2.0f)] float talkingMultiplier;
    [SerializeField] [Range(0.0f, 2.0f)] float curiosityMultiplier;
    private List<float> multiplierNecessities = new List<float>();
    [Header("Pathfinder")]
    [SerializeField] Vector3 destination;
   
    [SerializeField] int alertLevel; //0 si no ven nada, 1 si han visto un cuerpo, 2 sospechan de ti y huirán

    private void Awake()
    {
        guestAnimator = GetComponentInParent<Animator>();
        navMesh = GetComponentInParent<NavMeshAgent>();
        RandomizeValues();
        AddValueList();
        AddMultiplierList();
        PutInRandomLocation();
        currentState = NPCState.STAND;
        TakeDecision();
        destination = GetRandomPoint(transform.position, 100.0f);
        navMesh.SetDestination(destination);
        StartCoroutine("DestinationDecision");
        gameManager = GameObject.Find("GameManager");

    }

    IEnumerator DestinationDecision()
    {
        yield return new WaitForSeconds(15);
        destination = GetRandomPoint(transform.position, 10.0f);
        navMesh.SetDestination(destination);
        StartCoroutine("DestinationDecision");

    }

    public static Vector3 GetRandomPoint(Vector3 center, float maxDistance)
    {
        // Get Random Point inside Sphere which position is center, radius is maxDistance
        Vector3 randomPos = Random.insideUnitSphere * maxDistance + center;

        NavMeshHit hit; // NavMesh Sampling Info Container

        // from randomPos find a nearest point on NavMesh surface in range of maxDistance
        NavMesh.SamplePosition(randomPos, out hit, maxDistance, NavMesh.AllAreas);

        return hit.position;
    }
    private void RandomizeValues()
    {
        
    }

    private void AddMultiplierList()
    {
        multiplierNecessities.Add(bathMultiplier);
        multiplierNecessities.Add(fuckMultiplier);
        multiplierNecessities.Add(consumeMultiplier);
        multiplierNecessities.Add(talkingMultiplier);
        multiplierNecessities.Add(curiosityMultiplier);
    }

    private void AddValueList()
    {
        //(0 bath, 1 fuck, 2 consume, 3 talk, 4 curiosity)
        valueNecessities.Add(bathDesire);
        valueNecessities.Add(fuckDesire);
        valueNecessities.Add(consumeDesire);
        valueNecessities.Add(talkingDesire);
        valueNecessities.Add(curiosity);
    }

    private void PutInRandomLocation()
    {
       
    }

    IEnumerator DecideBehaviour()
    {
        float randomReactionTime = Random.Range(minReactionTime, maxReactionTime);
        yield return new WaitForSeconds(randomReactionTime);

        TakeDecision();

    }

    private void TakeDecision()
    {
        //Si no tiene necesidades, no está alterado o no tiene ganas de hablar, se queda parado
        //Si tiene alguna necesidad crítica, hará la acción de la sala. 
        //Si tiene nivel de alerta 3 irá a avisar a un guardia
        //Si tiene nivel de alerta 2 te evitará.
        //Si tiene alerta uno, su cono de visión se hará más grande.
        //Si no le pasa nada en un rato, la alerta irá bajando.
        //Hay una pequeña posibilidad de que mate a otra persona si se la encuentra sola en una habitación.
        //Si alguien lo mata permanecerá en el suelo hasta disolverse.
        //PRIORIDAD: MUERTE -> BUSCAR AYUDA -> ASUSTARSE -> ACCIÓN DE HABITACIÓN -> HABLAR CON ALGUIEN -> QUEDARSE PARADO (DE IZQUIERDA A DERECHA)

        float maxValue = 0;
        maxIndex = 0;
        for (int atributeIndex = 0; atributeIndex < valueNecessities.Count; atributeIndex++)
        {
            if (valueNecessities[atributeIndex] * multiplierNecessities[atributeIndex] > maxValue)
            {
                maxValue = valueNecessities[atributeIndex] * multiplierNecessities[atributeIndex];
                maxIndex = atributeIndex;
            }
        }

        if (!alive)
        {
            currentState = NPCState.DEAD;
        }
        else if(alertLevel == 3)
        {
            currentState = NPCState.GOING_HELP;
        }
        else if(alertLevel == 2)
        {
            currentState = NPCState.SCARED;
        }       
        else if(maxValue > extremeNecessityValue && maxIndex != 3)
        {
            currentState = NPCState.ROOM_ACTION;
        }
        else
        {               
            currentState = NPCState.STAND;
        }

        decisionDone = true;
        StartCoroutine("DecideBehaviour");

    }

    private void Update()
    {
        if(navMesh.isStopped)
        {
            guestAnimator.SetBool("Walking", false);
        }
        else
        {
            guestAnimator.SetBool("Walking", true);
        }
        if(alertLevel != 3)
        {
            if (seeingPlayer && playerSeen != null)
            {
                if (playerSeen.GetComponent<PlayerController>().killing)
                {
                    StopAllCoroutines();
                    gameManager.GetComponent<GameManager>().GuardGameOver();
                }
                
            }
        }
        

        if (decisionDone)
        {
            switch (currentState)
            {
                case NPCState.DEAD:
                    StartDeath();
                    break;
                case NPCState.GOING_HELP:
                    StartGoingForHelp();
                    break;
                case NPCState.SCARED:
                    StartScared();
                    break;
                case NPCState.ROOM_ACTION:
                    StartRoomAction();
                    break;                
                default:
                    StartStanding();
                    break;
            }    
        }
    }

    private void StartStanding()
    {
        
    }    

    private void StartRoomAction()
    {
        string searchingTag = "";
        //(0 bath, 1 fuck, 2 consume, 3 talk, 4 curiosity)
        switch (maxIndex)
        {          
            case 0:
                searchingTag = "Bath";
                break;
            case 1:
                searchingTag = "Room";
                break;
            case 2:
                searchingTag = "Bar";
                break;
            case 3:
                searchingTag = "LivingRoom";
                break;
            case 4:
                searchingTag = "CrimeZone";
                break;             

        }

        if(currentRoomTag== searchingTag){
            switch (searchingTag)
            {
                case "Bath":
                    BathAction();
                    break;
                case "Room":
                    RoomAction();
                    break;
                case "Bar":
                    BarAction();
                    break;
                case "LivingRoom":
                    LivingRoomAction();
                    break;
                case "CrimeZone":
                    CrimeZoneAction();
                    break;

            }
        }

        for (int multiplier = 1; multiplier > maxRadiusMultiplier; multiplier++)
        {
            Collider[] collisions = Physics.OverlapSphere(transform.position, multiplier * initialSearchRadius);

            foreach (var collider in collisions)
            {
                if (collider.gameObject.CompareTag(searchingTag))
                {

                }
            }
        }
    }

    private void CrimeZoneAction()
    {
        throw new System.NotImplementedException();
    }

    private void LivingRoomAction()
    {
        throw new System.NotImplementedException();
    }

    private void BarAction()
    {
        throw new System.NotImplementedException();
    }

    private void RoomAction()
    {
        throw new System.NotImplementedException();
    }

    private void BathAction()
    {
        //if()
    }

    private void StartScared()
    {
        //Random NavMeshPoint
    }

    private void StartGoingForHelp()
    {
        for (int multiplier = 1; multiplier > maxRadiusMultiplier; multiplier++)
        {
            Collider[] collisions = Physics.OverlapSphere(transform.position, multiplier * initialSearchRadius);

            foreach (var collider in collisions)
            {
                if (collider.gameObject.CompareTag("Guard"))
                {
                    destination = collider.transform.position;
                    NoticeGuard(collider);
                }
            }
        }
    }

    private void NoticeGuard(Collider collider)
    {
        if(transform.position == destination)
        {
            //collider.gameObject.GetComponent<GuardBehaviour>().HuntPlayer();
        }
    }


    private void StartDeath()
    {
        //RagDoll y se queda en el suelo, desaparece después de un rato
    }

    private void OnTriggerEnter(Collider other)
    {
        currentRoomTag = other.tag;
    }
}
