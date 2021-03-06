﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Script genérico para gerenciar o estado de saude e defesa de objetos do jogo
 */

public class HealthController : MonoBehaviour {


	#region variables

	[Header("Stats")]
	//Verifica se o personagem pode ou não levar dano
	private bool canBeHurt;
	//Maximo da vida
	public float maxHp;
	//Vida atual
	private float currentHp;
	//Defesa, usada para diminuir dano
	public float defense;
    //Tempo de invencibilidade
    public float invicibilityTime;

	public bool LevandoDano;
    //Medida de equilíbrio em que 0 é pouco equilibrado e 1 muito equilibrado

	[Space(20)]
	[Header("Ref. to own components")]
	//Referencia ao Rigidbody da entidade
	public Rigidbody entityRigidBody;
	//Referencia ao animator da entidade
	public Animator animator;


	//Verificando se o objeto está morto
	public bool isDead;

    #endregion variables

	private void Awake(){
		if(entityRigidBody==null)
			entityRigidBody = (Rigidbody)GetComponent(typeof(Rigidbody));
		
		currentHp = maxHp;
        canBeHurt = true;
	}

	/**
	 * Método usado para tomar dano.
	 * O HP é alterado subtraindo damage - defense
	 * 
	 * @param	damage	quantidade de dano a ser tomado
	 * @param	direction	Vetor de direção e intensidade do knockback
	 */
	public void takeDamage(float damage){

		if (canBeHurt && !isDead) {

            if (gameObject.name == "Bugfolk(Clone)")
            {
                GetComponent<AudioSource>().PlayOneShot(SoundManager.SM.GetBugfolkGrunt());
                SoundManager.SM.PlayHit();
            }
            else if (gameObject.name == "Prole(Clone)")
            {
                GetComponent<AudioSource>().PlayOneShot(SoundManager.SM.GetDepthsGrunt());
                SoundManager.SM.PlayHit();
            }

            float netDamage = damage * this.defense;
			if (netDamage > 0) {
                this.currentHp -= netDamage;
				StartCoroutine(waitInvinciTime());
			}
            
        }
        else
            return;


		if (this.currentHp <= 0) {
			this.die ();
		} else {
			
            if (animator != null)
            {
                //animator.SetTrigger("Hit");
                //print("Ai");
            }
		}
		
		//movement.anim.play
	}
		

	public float getMaxHealth(){
		return this.maxHp;
	}

	public float getCurrentHealth(){
		return this.currentHp;
	}

	/**
	 * Método para matar a entidade.
	 * Deve ser overwriten para efeitos de morte específicos
	 * @param void
	 * @return void
	 */
	private void die (){
		this.isDead = true;		
		this.canBeHurt = false;
		this.gameObject.GetComponent<Animator>().SetBool("IsAlive",false);
		StartCoroutine(Destroi());
	}

	private IEnumerator Destroi(){
		yield return new WaitForSeconds (5.0f);
		Destroy(this.gameObject);
	}

	void Update(){
		if (this.gameObject.tag == "Player") {
			//print ("Invec? " + this.canBeHurt);
			//print ("HP: " + this.currentHp);
		}
	}

    private IEnumerator waitInvinciTime() {
        canBeHurt = false;
		LevandoDano = true;
        yield return new WaitForSeconds(invicibilityTime);
        canBeHurt = true;
		LevandoDano = false;
    }

}
