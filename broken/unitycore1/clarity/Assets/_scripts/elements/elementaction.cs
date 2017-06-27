﻿using UnityEngine;
using System.Collections;
using Gvr.Internal;

public class elementaction : MonoBehaviour {

	IGvrGazePointer gazepoint;

	//captured by player at all
	public bool captured = false;

	// positive or negative energy
	public bool purestate = true;

	public Material puremat;
	public Material badmat;

	private Collider col;
	private Rigidbody rbody;
	private MeshRenderer meshren;

	public int elementpower = 0;
	public int elementhighlimit = 0;
	public bool acklook = false;
	public float vel = 5f;

	public float overridemovementspeed = 0;
	public bool movetoparent = false;

	public float minForce = -10;
	public float maxForce =10;

	public virtual void Awake()
	{
		elementpower = Random.Range(0,elementhighlimit);

		rbody = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();

		rbody.constraints = RigidbodyConstraints.None;
		float ranx = Random.Range(minForce,maxForce);
		float rany = Random.Range(minForce,maxForce);
		float ranz = Random.Range(minForce,maxForce);
		rbody.AddForce(ranx,rany,ranz);

		meshren = GetComponent<MeshRenderer>();

		if(!purestate)
		{
			meshren.material = badmat;
		}
		else
		{
			meshren.material = puremat;
		}
	}

 
	// Update is called once per frame
	public virtual	void Update () {

		if(playerinteraction.lookedatobj == transform.gameObject)
		{
			acklook = true;
		}
		else
		{
			acklook = false;
		}
	
		if(!captured && movetoparent)
		{
			Vector3 dir = (transform.parent.position - transform.position) *overridemovementspeed;
			rbody.AddForce(dir);
			//move to parent object
		}
	}

	public virtual void collected(Transform point)
	{

			transform.position = point.position;

			col.enabled = false;
			meshren.enabled = false;
			rbody.isKinematic = true;
			transform.parent =null;
			transform.parent = point;
			captured =true;

	}

	public virtual void cleanelement()
	{
		if(!purestate)
		{
			purestate = true;
			meshren.material = puremat;
		}
	}

	public virtual void corruptelement()
	{
		purestate = false;
		meshren.material = badmat;

	}
		
	public virtual void letloose(Vector3 par)
	{
		Vector3 dir = (par - transform.position) * vel;
		rbody.velocity = dir;

		col.enabled = true;
		meshren.enabled = true;
		rbody.isKinematic = false;

	}

	public virtual void breakfromcentral()
	{
		col.enabled = true;
		meshren.enabled = true;
		rbody.isKinematic = true;
		transform.parent =null;
		captured =false;

		float ranx = Random.Range(-5,5);
		float rany = Random.Range(-5,5);
		float ranz = Random.Range(-5,5);
		rbody.AddForce(ranx,rany,ranz);

	}
	public virtual void stopmovement()
	{
		rbody.velocity = Vector3.zero;

	}
	public virtual void startmovement()
	{
		float ranx = Random.Range(-5,5);
		float rany = Random.Range(-5,5);
		float ranz = Random.Range(-5,5);
		rbody.AddForce(ranx,rany,ranz);
	}
}
