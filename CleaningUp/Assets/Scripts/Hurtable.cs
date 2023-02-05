using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtable : MonoBehaviour
{
    [SerializeField] GameObject ragdoll;
    Animator guestAnimator;

    private void Awake()
    {
        guestAnimator = GetComponentInParent<Animator>();
    }

    public void PlayAnim(GameObject player)
    {
        print("Animación");

        Vector3 rotation = player.transform.position - transform.position;
        transform.parent.rotation = Quaternion.LookRotation(new Vector3(rotation.x, 0, rotation.z));
        guestAnimator.SetBool("Hurt",true);
        
    }
    public void Die()
    {
        ragdoll.SetActive(true);
        ragdoll.GetComponentInChildren<Rigidbody>().AddExplosionForce(20.0f, transform.position, 2.0f);
        Destroy(gameObject);
    }
}
