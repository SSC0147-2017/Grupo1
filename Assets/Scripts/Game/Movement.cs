﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Movement : MonoBehaviour {


	public int MaxHP;
	private bool canBeHurt;
	private int CurrentHP;
	public float defense;
	public bool isDead;
	public float invicibilityTime;
	public float RevivalTime;
	//HP

	private bool isMovable;
	public float turnSpeed;
	//Movement

	public Rigidbody Projectile;
	public Transform Pos;
	public float spread;
	public float ProjSpd;
	public int ColLayer;
	public float SpecialTime;
	//Special

	public float rollTime;
	public float rollDmgRed;
	//Dodge

	public float AttackTime;
	//Attack

	public string Controller;
	private Animator anim;

	void Start() {
		anim = GetComponent<Animator>();
		Physics.IgnoreLayerCollision(ColLayer,ColLayer,true);
		isMovable=true;
		canBeHurt=true;
		CurrentHP=MaxHP;
		isDead=false;
	}

	public void Initiate(string Con){
		Controller=Con;
	}


	void FixedUpdate () {
		
		if(Input.GetButtonDown(Controller+"Fire1")) {
			StartCoroutine(waitAttackTime());
			anim.SetFloat("Speed",0.0f);
			anim.SetTrigger("Attack");
		}
		if(Input.GetButtonDown(Controller+"Fire2")) {
			StartCoroutine(waitSpecialTime());
			anim.SetFloat("Speed",0.0f);
			anim.SetTrigger("Special");
			Special();
		}
		if(Input.GetButtonDown(Controller+"Fire3")) {
			anim.SetTrigger("Dodge");
			StartCoroutine(RollTime());
		}
		ControlPlayer();
	}

	private IEnumerator waitAttackTime() {
		isMovable=false;
		yield return new WaitForSeconds(AttackTime);
		isMovable=true;
	}

	private IEnumerator waitSpecialTime() {
		isMovable=false;
		yield return new WaitForSeconds(SpecialTime);
		isMovable=true;
	}

	public void takeDamage(float damage){

		if (canBeHurt) {
			float netDamage = damage * defense;
			if (netDamage > 0) {
				CurrentHP -= (int)netDamage;
				StartCoroutine(waitInvinciTime());
				anim.SetFloat("Speed",0.0f);
				anim.SetTrigger("Hit");
			}
		}
		else
			return;

		if (this.CurrentHP <= 0) {
			anim.SetTrigger("Dying");
			isDead = true;
			anim.SetBool("CanAttack",false);
			//this.die ();
		}

	}

	private IEnumerator waitInvinciTime() {
		canBeHurt = false;
		isMovable=false;
		anim.SetBool("CanAttack",false);
		yield return new WaitForSeconds(invicibilityTime);
		canBeHurt = true;
		isMovable=true;
		anim.SetBool("CanAttack",true);
	}

	private IEnumerator waitReviveTime() {
		canBeHurt = false;
		isMovable=false;
		anim.SetBool("CanAttack",false);
		yield return new WaitForSeconds(RevivalTime);
		canBeHurt = true;
		isMovable=true;
		anim.SetBool("CanAttack",true);
	}

	public void GainHP(int gain){
		CurrentHP+=gain;
		if (CurrentHP>MaxHP)
			CurrentHP=MaxHP;
	}

	public void Revive(){
		if(isDead){
			anim.SetTrigger("Revive");
			StartCoroutine(waitReviveTime());
			CurrentHP=MaxHP/2;
		}
	}

	private IEnumerator RollTime(){
		defense = rollDmgRed;
		//isMovable=false;
		anim.SetBool("CanAttack",false);
		yield return new WaitForSeconds (rollTime);
		defense = 1.0f;
		//isMovable=true;
		anim.SetBool("CanAttack",true);
	}

	void ControlPlayer() {
		float h = Input.GetAxisRaw (Controller+"Horizontal");
		float v = Input.GetAxisRaw (Controller+"Vertical");

		if(!isMovable){
			//anim.SetBool("IsMoving",false);
			anim.SetFloat("Speed",0.0f);
			return;
		}

		if(h*h+v*v<0.1){
			anim.SetFloat("Speed",0.0f);
			return;
		}
		//anim.SetBool("IsMoving",true);

		Vector3 movement = new Vector3(h, 0.0f, v);

		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), turnSpeed);
		//transform.Translate (movement * ActualSpeed * Time.deltaTime, Space.World);
		anim.SetFloat("Speed", AnimSpeed(h,v));

	}

	float AnimSpeed(float sideALength, float sideBLength) {
		if(Mathf.Sqrt(sideALength * sideALength + sideBLength * sideBLength)>1.0f)
			return 1.0f;
		return Mathf.Sqrt(sideALength * sideALength + sideBLength * sideBLength);
	}

	void Special() {
		if(Pos==null)
			return;
		Rigidbody Clone = (Rigidbody) Instantiate(Projectile, Pos.position, Pos.rotation);
		Clone.transform.Rotate(Random.Range(0.0f,spread), Random.Range(0.0f,spread), Random.Range(0.0f,spread));
		Clone.velocity = Pos.TransformDirection(Vector3.forward * ProjSpd);
	}
}