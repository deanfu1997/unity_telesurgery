using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger2 : MonoBehaviour
{
	public Color redcolor;
	public Color bluecolor;
	void Start()
	{
		transform.GetComponent<Renderer>().material.color = bluecolor;
	}

	void OnTriggerEnter(Collider collision)
	{
		transform.GetComponent<Renderer>().material.color = redcolor;
		Counter.wirecounter += 1;
		// Debug.Log(Counter.wirecounter);
	}

	void OnTriggerExit(Collider collision)
	{

		transform.GetComponent<Renderer>().material.color = bluecolor;

	}


}

