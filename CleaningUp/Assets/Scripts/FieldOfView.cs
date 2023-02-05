using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{
	
	public float viewRadius;
	[Range(0, 360)]

	public float viewAngle;

	[SerializeField]
	public LayerMask targetMask;

	
	[SerializeField]
	public LayerMask obstacleMask;

	[SerializeField]
	private Material coneMaterial;

	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();

	void Start()
	{
		StartCoroutine("FindTargetsWithDelay", .2f);
	}


	IEnumerator FindTargetsWithDelay(float delay)
	{
		while (true)
		{
			yield return new WaitForSeconds(delay);
			FindVisibleTargets();
		}
	}

	void FindVisibleTargets()
	{

		visibleTargets.Clear();
		
		Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
		
		if(targetsInViewRadius.Length > 0)
        {
			for (int i = 0; i < targetsInViewRadius.Length; i++)
			{

				Transform target = targetsInViewRadius[i].transform;
				Vector3 dirToTarget = (target.position - transform.position).normalized;

				if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
				{

					float dstToTarget = Vector3.Distance(transform.position, target.position);



					if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
					{
						if (visibleTargets.Count == 0)
						{
							visibleTargets.Add(target);
							GetComponentInChildren<GuestBehaviour>().seeingPlayer = true;
							GetComponentInChildren<GuestBehaviour>().playerSeen = target.gameObject;

						}


					}
					else
					{
						visibleTargets.Clear();
						GetComponentInChildren<GuestBehaviour>().seeingPlayer = false;
						GetComponentInChildren<GuestBehaviour>().playerSeen = null;

					}
				}
                else
                {
					GetComponentInChildren<GuestBehaviour>().seeingPlayer = false;
					GetComponentInChildren<GuestBehaviour>().playerSeen = null;
				}
			}
		}
        else
        {
			GetComponentInChildren<GuestBehaviour>().seeingPlayer = false;
			GetComponentInChildren<GuestBehaviour>().playerSeen = null;
		}
		
		
	}


	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
		{
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawRay(transform.position, transform.forward);
		Gizmos.DrawWireSphere(transform.position, viewRadius);
	}

	

}