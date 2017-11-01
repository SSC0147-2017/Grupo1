﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BugfolkBehaviour : MonoBehaviour {

	public NavMeshAgent navAgent;
	public GameObject target;
	public float damage;

	private HealthController tgtHealth;
	private bool isAttacking;

	void Start () {
		target = null;
	}

	void OnTriggerStay(Collider col){

        print(col);

		if (col.gameObject.tag == "Player") {
			if (target == null) {
				target = col.gameObject;	//Novo alvo
			} else {
				//Caso haja mais de um, escolher o alvo mais próximo
				if (Vector3.Distance (col.gameObject.transform.position, transform.position) <
				   Vector3.Distance (target.transform.position, transform.position)) {
					target = col.gameObject;//Novo alvo
				}
			}

			tgtHealth = target.GetComponent<HealthController> ();
		}
	}


	void OnTriggerExit(Collider col){
		if (col.gameObject == target) {
			target = null;
		}
	}

	void Update () {
        //print("tgt " + target);

		//PERSEGUINDO/ATACANDO
        if (target != null)
        {
			Chase ();
			//Raio de ataque
			if ( (target.transform.position - transform.position).magnitude <= navAgent.stoppingDistance) {
				Attack ();
			}
        }
        //PATRULHANDO/PARADO
		else
            navAgent.isStopped = true;
		
	}
		
	/*
	 * Função usada para a perseguição
	 */
	void Chase(){
		navAgent.isStopped = false;
		navAgent.SetDestination(target.transform.position);
	}

	/*
	 * Função usada para atacar um inimigo
	 */
	void Attack(){
		navAgent.isStopped = true;//Para antes de atacar
		print ("Attacking!");
		tgtHealth.takeDamage (damage);
		//Talvez um Yield para o ataque?
		//TODO - Animações e coisas chiques
		navAgent.isStopped = false;
	}
}
