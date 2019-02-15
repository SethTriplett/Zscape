﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinny : MonoBehaviour {

	public Animator animator;
	public List<GameObject> materialChangeComponents;
	public Material defaultMaterial;
	public Material telegraphMaterial;

	public GameObject beamPrefab;
	private GameObject myBeam;

	void Start() {
		StartCoroutine(Attack());
	}

	private IEnumerator Attack() {
		yield return new WaitForSeconds(1f);

		//Telegraph the attack
		animator.SetBool("startAttack", true);
		assignMaterial(telegraphMaterial);
		myBeam = Instantiate(beamPrefab, gameObject.transform.position, beamPrefab.transform.rotation, gameObject.transform);

		//Wait until we're in full attack mode
		while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
			yield return null;
		}

		StartCoroutine(myBeam.GetComponent<Beam>().extendBeam());

		yield return new WaitForSeconds(5f);

		//Return to default
		StartCoroutine(myBeam.GetComponent<Beam>().collapseBeam());
		animator.SetBool("startAttack", false);
		assignMaterial(defaultMaterial);

		//Make sure the beam anim finishes, then destroy the (already shrunk) beam
		yield return new WaitForSeconds(5f);
		Destroy(myBeam);

		//loop!
		yield return new WaitForSeconds(12f);
		StartCoroutine(Attack());
	}

	//commented mouse activation. It would be timing based in game anyways

	// void OnMouseEnter() {
	// 	animator.SetBool("startAttack", true);
	// 	assignMaterial(telegraphMaterial);
	// 	myBeam = Instantiate(beamPrefab, new Vector3(0, 0, 0), beamPrefab.transform.rotation, gameObject.transform);
	// }

	// void OnMouseExit() {
	// 	animator.SetBool("startAttack", false);
	// 	assignMaterial(defaultMaterial);
	// 	Destroy(myBeam);
	// }


	//This guy just changes the color

	private void assignMaterial(Material mat) {
		foreach (GameObject component in materialChangeComponents) {
			component.GetComponent<Renderer>().material = mat;
		}
	}

}
