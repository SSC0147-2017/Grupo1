﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DepthsBehaviour : MonoBehaviour {

    public List<GameObject> Targets = new List<GameObject>();
    public GameObject CurrTarget;
    public GameObject ClosestTarget;

    public float Multiplier;

    NavMeshAgent navAgent;

    private bool isAttacking;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        //print("tgt " + target);
        CurrTarget = CalculateTarget();

        //PERSEGUINDO/ATACANDO
        if (CurrTarget != null)
        {
            Chase();
            //Raio de ataque
            if ((CurrTarget.transform.position - transform.position).magnitude <= navAgent.stoppingDistance)
            {
                Attack();
            }
        }
        //PATRULHANDO/PARADO
        else
            navAgent.isStopped = true;

    }

    /*
	 * Função usada para a perseguição
	 */
    void Chase()
    {
        navAgent.isStopped = false;
        navAgent.SetDestination(CurrTarget.transform.position);
    }

    /*
	 * Função usada para atacar um inimigo
	 */
    void Attack()
    {
        //Para antes de atacar
        if (!isAttacking)
        {
            print("Attacking!");
            StartCoroutine(attackCooldown());
            //TODO - Animações e coisas chiques
        }
    }

    GameObject CalculateTarget()
    {
        int i = 0;
        GameObject isolated = null;
        float dist, highest = 0;

        while(i < Targets.Count)
        {
            dist = CalculateIsolation(i);
            if(dist > highest)
            {
                highest = dist;
                isolated = Targets[i];
            }
            i++;
        }

        float closest;
        if (ClosestTarget != null) closest = Vector3.Distance(ClosestTarget.transform.position, transform.position);
        else closest = 1000;

        //print("m " + Multiplier + " c " + closest + " h " + highest);
        //if ((Multiplier / closest) > highest) return ClosestTarget;
        if (closest < Multiplier) return ClosestTarget;

        return isolated;
    }

    float CalculateIsolation(int index)
    {
        int i = 0;
        float dist = 0;
        while(i < Targets.Count)
        {
            if(i != index)
            {
                dist += Vector3.Distance(Targets[index].transform.position, Targets[i].transform.position);
            }
            i++;
        }
        if(Targets.Count > 1) dist /= Targets.Count - 1;
        return dist;
    }

    void OnTriggerStay(Collider col)
    {

        if (col.gameObject.tag == "Player")
        {
            if (ClosestTarget == null)
            {
                ClosestTarget = col.gameObject;    //Novo alvo
            }
            else
            {
                //Caso haja mais de um, escolher o alvo mais próximo
                if (Vector3.Distance(col.gameObject.transform.position, transform.position) <
                   Vector3.Distance(ClosestTarget.transform.position, transform.position))
                {
                    ClosestTarget = col.gameObject;//Novo alvo
                }
            }
            //tgtHealth = target.GetComponent<HealthController>();
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject == ClosestTarget)
        {
            ClosestTarget = null;
        }
    }

    //FUNÇÃO DE TESTE DE ATTACK COOLDOWN
    private IEnumerator attackCooldown()
    {
        navAgent.isStopped = true;
        isAttacking = true;
        //tgtHealth.takeDamage(damage);
        yield return new WaitForSeconds(2);
        isAttacking = false;
        navAgent.isStopped = false;

    }
}
