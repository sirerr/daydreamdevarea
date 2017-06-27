﻿using UnityEngine;
using System.Collections;

public class centralaction : MonoBehaviour {

	//elements added to the central object
	public int elementadded;
	// power needed to be complete
	public float neededpower=0;
	//power collected by the center object
	public int collectedpower = 0;

	//for central mode
	public bool fullpower = false;

	//Rigidbody and collider ref
	public Rigidbody rbody;
	public Collider col;
	//the state of the central object
	public int centralstate =0;
	//is it being raycasted to
	public bool acklook = false;
	//speed to hole
	public float vel = 20f;
	// is it set in a hole
	private bool setinhole = false;
	//the first child object the outer area
	private GameObject outerarea;
	private MeshRenderer meshren;

	public float overridemovementspeed = 0;
	public bool movetoparent = false;

	public float minForce = -10;
	public float maxForce =10;

	public virtual void Awake()
	{	
		rbody = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();
		meshren = GetComponent<MeshRenderer>();
		rbody.constraints = RigidbodyConstraints.None;

		float ranx = Random.Range(minForce,maxForce);
		float rany = Random.Range(minForce,maxForce);
		float ranz = Random.Range(minForce,maxForce);
		rbody.AddForce(ranx,rany,ranz);
		outerarea = transform.GetChild(0).gameObject;
		outerarea.SetActive(false);
	}

	public virtual void tohole(Transform holepoint)
	{
		rbody.constraints = RigidbodyConstraints.None;
		setinhole = true;
		col.enabled = true;
		Vector3 dir = (holepoint.position - transform.position) *vel;
		rbody.velocity = dir;
		meshren.enabled = true;
		rbody.isKinematic = false;
		int childs = transform.childCount;
		for(int i=0;i<childs;i++)
		{
			transform.GetChild(i).gameObject.SetActive(true);
		}
		transform.parent = holepoint;
	}


	public virtual void centralstateaction()
	{
		if(centralstate == 0)
		{
			becomeactive();
		}

		///maybe change this
	
		// possible change in how this works coming maybe
		else if(centralstate ==1 && !fullpower)
		{
				rbody.constraints = RigidbodyConstraints.None;
			float ranx = Random.Range(-5,5);
			float rany = Random.Range(-5,5);
			float ranz = Random.Range(-5,5);
			rbody.AddForce(ranx,rany,ranz);
			int childs = transform.childCount;
			for(int i=0;i<childs;i++)
			{
				transform.GetChild(i).gameObject.SetActive(false);
			}
			centralstate =0;
		}

//		else if (centralstate==1 &&fullpower)
//		{
//			StartCoroutine(centralstatewait());
//		}
	}

	IEnumerator centralstatewait()
	{

		yield return new WaitForSeconds(.5f);
		rbody.constraints = RigidbodyConstraints.FreezeRotation;
		centralstate =2;
	}

	public virtual void grabbed(Transform centercollect)
	{

		if(!setinhole && fullpower)
		{
			//this state shows the object has been grabbed
			centralstate =3;
			rbody.constraints = RigidbodyConstraints.FreezeAll;
			rbody.isKinematic = true;
			transform.position = centercollect.position;
			meshren.enabled = false;
			int childs = transform.childCount;
			for(int i=0;i<childs;i++)
			{
				transform.GetChild(i).gameObject.SetActive(false);
			}

		}

	}

		
	public virtual void becomeactive()
	{
		centralstate =1;
		rbody.velocity = Vector3.zero;
		rbody.constraints = RigidbodyConstraints.FreezePosition;
		int childs = transform.childCount;
		for(int i=0;i<childs;i++)
		{
			transform.GetChild(i).gameObject.SetActive(true);
		}
		//also expand the circle around it and make it look good
	}
		

	public virtual void poweredup()
	{
		//do action when fully powered
		StartCoroutine(centralstatewait());
		rbody.AddForce(Vector3.zero);
	}

	public virtual void Update()
	{

		if(collectedpower >= neededpower && !fullpower )
		{
			fullpower = true;
			poweredup();
		}

		if(centralstate ==0 && movetoparent)
		{
			Vector3 dir = (transform.parent.position - transform.position) *overridemovementspeed;
			rbody.AddForce(dir);
		}

		if(playerinteraction.lookedatobj == transform.gameObject)
		{
			acklook = true;
		}
		else
		{
			acklook = false;
		}

	}

	public virtual void OnCollisionEnter(Collision col)
	{
		if(col.collider.CompareTag("element"))
		{
			if(col.gameObject.GetComponent<elementaction>().captured)
			{
				col.transform.parent = transform;
				col.gameObject.GetComponent<Rigidbody>().isKinematic = true;
				elementadded++;
				collectedpower += col.gameObject.GetComponent<elementaction>().elementpower;
				col.collider.enabled = false;
			}
		}

	}

	public virtual void breakapart()
	{
		rbody.constraints = RigidbodyConstraints.None;
		float ranx = Random.Range(-5,5);
		float rany = Random.Range(-5,5);
		float ranz = Random.Range(-5,5);
		rbody.AddForce(ranx,rany,ranz);
		int childs = transform.childCount;
		for(int i=0;i<childs;i++)
		{ 
			elementaction ele = transform.GetChild(0).GetComponent<elementaction>();
			transform.GetChild(i).gameObject.SetActive(true);
			ele.breakfromcentral();
		}
		centralstate =0;

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
