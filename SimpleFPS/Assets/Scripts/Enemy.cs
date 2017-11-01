using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    public GameObject player;
    public float detectionRadius = 30f;

    private NavMeshAgent agent;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
        if ((agent != null) && (Vector3.Distance(transform.position, player.transform.position) < detectionRadius))
        {
            agent.SetDestination(player.transform.position);
        }
    }
}
