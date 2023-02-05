using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 movementInput = Vector2.zero;
    public bool killing = false;
    private bool doingAction = false;
    Animator playerAnimator;

    [SerializeField] float playerSpeed;
    [SerializeField] float detectionRate;
    [SerializeField] float detectRadius;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] GameObject currentSelectedInteractable;
    [SerializeField] GameObject equipedObject;
    [SerializeField] float takeObjectCooldown;
    [SerializeField] bool canTakeObject = true;    
    [SerializeField] GameObject knife;
    CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerAnimator = GetComponentInParent<Animator>();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        
    }
    public void OnKill(InputAction.CallbackContext context)
    {
        killing = context.action.triggered;
        print("Atacó");
    }
    public void OnAction(InputAction.CallbackContext context)
    {
        doingAction = context.action.triggered;
        print("Acción");
    }
    private void Update()
    {
        MovePlayer();
        ObjectsBehaviour();        
    }

    private void ObjectsBehaviour()
    {
        DetectInRange();
        CanTakeObject();
        ThrowObject();
        TalkGuest();
        AtackGuest();
    }

    private void TalkGuest()
    {
        if (currentSelectedInteractable != null)
        {
            if (currentSelectedInteractable.CompareTag("Human") && doingAction)
            {
                StartCoroutine("TurnToPlayer");
                doingAction = false;
            }
        }
    }

    IEnumerator TurnToPlayer()
    {
        yield return new WaitForSeconds(0.1f);
        print("Me giro.");
        Vector3 rotation = transform.position - currentSelectedInteractable.transform.position;
        currentSelectedInteractable.transform.parent.rotation = Quaternion.LookRotation(new Vector3(rotation.x,0,rotation.z));
    }

    private void AtackGuest()
    {
        if(currentSelectedInteractable != null)
        {
            if (currentSelectedInteractable.CompareTag("Human") && killing)
            {
                Debug.Log("Se murió");
                playerAnimator.SetBool("Attacking", true);
                StartCoroutine("AtackRecuperation");
                killing = false;
                
            }
        }
    }

    IEnumerator AtackRecuperation()
    {

        knife.SetActive(true);
        currentSelectedInteractable.GetComponent<Hurtable>().PlayAnim(gameObject);

        yield return new WaitForSeconds(2);


        currentSelectedInteractable.GetComponent<Hurtable>().Die();
        print("Atack");
        playerAnimator.SetBool("Attacking", false);
        knife.SetActive(false);
        currentSelectedInteractable = null;
    
    }
    private void ThrowObject()
    {
        if(equipedObject != null && doingAction && canTakeObject)
        {
            equipedObject.transform.parent = null;
            equipedObject = null;
            canTakeObject = false;
            StartCoroutine("ObjectCooldown");
        }
    }

    private void CanTakeObject()
    {
        if(currentSelectedInteractable != null)
        {
            if(currentSelectedInteractable.CompareTag("Object") && doingAction && equipedObject == null && canTakeObject)
            {
                equipedObject = currentSelectedInteractable;
                equipedObject.transform.parent = gameObject.transform;
                canTakeObject = false;
                StartCoroutine("ObjectCooldown");
            }
            

        }
    }

    IEnumerator ObjectCooldown()
    {
        yield return new WaitForSeconds(takeObjectCooldown);
        canTakeObject = true;
    }

    private void DetectInRange()
    {
        StartCoroutine("Detect");
    }

    private void MovePlayer()
    {
        if(movementInput != Vector2.zero && !playerAnimator.GetBool("Attacking"))
        {
            playerAnimator.SetBool("Walking", true);
            
            Vector3 movementVector = new Vector3(movementInput.x, 0, movementInput.y).normalized;
            characterController.Move(movementVector * playerSpeed);
            transform.rotation = Quaternion.LookRotation(movementVector);
        }
        else
        {
            playerAnimator.SetBool("Walking", false);
        }
    }

    IEnumerator Detect()
    {
        yield return new WaitForSeconds(detectionRate);
        Collider[] interactableColliders = Physics.OverlapSphere(transform.position, detectRadius, interactableLayer);
        List<GameObject> interactableObjects = new List<GameObject>();
        if(interactableColliders.Length != 0)
        {
            foreach (var collider in interactableColliders)
            {
                interactableObjects.Add(collider.gameObject);
            }
            float minDistance = Mathf.Infinity;
            int minIndex = 0;
            for (int objectIndex = 0; objectIndex < interactableObjects.Count; objectIndex++)
            {
                if (Vector3.Distance(transform.position, interactableObjects[objectIndex].transform.position) < minDistance)
                {
                    minDistance = Vector3.Distance(transform.position, interactableObjects[objectIndex].transform.position);
                    minIndex = objectIndex;
                }
            }
            //print("Index: " + minIndex);
            if(currentSelectedInteractable != null && currentSelectedInteractable != interactableObjects[minIndex])
            {
                currentSelectedInteractable.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineHidden;
                currentSelectedInteractable.GetComponent<Outline>().OutlineWidth = 0f;
                currentSelectedInteractable = interactableObjects[minIndex];
                currentSelectedInteractable.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineAll;
                currentSelectedInteractable.GetComponent<Outline>().OutlineWidth = 5f;
            }
            else
            {
                if(currentSelectedInteractable != interactableObjects[minIndex])
                {
                    currentSelectedInteractable = interactableObjects[minIndex];
                    currentSelectedInteractable.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineAll;
                    currentSelectedInteractable.GetComponent<Outline>().OutlineWidth = 5f;
                }
            }
          

        }
        else
        {
            if(currentSelectedInteractable != null)
            {
                currentSelectedInteractable.GetComponent<Outline>().OutlineWidth = 0f;
                currentSelectedInteractable.GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineHidden;
                currentSelectedInteractable = null;
            }
            
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
